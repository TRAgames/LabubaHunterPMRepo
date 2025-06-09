//------------------------------------------------------------//
// yanfei 2024.10.27
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System;
using System.Runtime.CompilerServices;
using static QuickFinder.Container.SerializedInt32Int32Set;

namespace QuickFinder.Container
{
    public class SerializedPool
    {
        public enum PoolMode : ushort
        {
            AutoResize = 1,
        }
        public enum ErrorCode
        {
            Full = -2,
            Fail = -1,
            OK = 1,
        }

        public const int BOUNDARY_LENGTH_SIZE = 4;
        public const int UNIT_LENGTH_SIZE = 2;
        public const int MODE_SIZE = 2;
        public const int FREELIST_SIZE = 4;
        public const int FREELIST_OFFSET = BOUNDARY_LENGTH_SIZE + UNIT_LENGTH_SIZE + MODE_SIZE;
        public const int DELETELIST_OFFSET = FREELIST_OFFSET + FREELIST_SIZE;
        public const int DELETELIST_SIZE = DLEETELIST_COUNT * 4;
        public const int HEAD_SIZE = DELETELIST_OFFSET + DELETELIST_SIZE;

        public const int DLEETELIST_COUNT = 16;
        public const int DELETEMAP_LENGTH_SIZE = 4;

        public const int SLOT_HEAD_SIZE = 4;
        public const short NEXT_SIZE = 4;
        public const int MINIMAL_ALLOCATE_LENGTH = 4;

        public const int INVALID_DELETELIST = -1;
        public const int INVALID_FREELIST = -1;

        int BOUNDARY_START;

        byte[] bytes;

        SerializedFastMapInt16Int32 deleteMap = null;
        Func<int, ResizeParams> allocator = null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort MakePoolMode(params PoolMode[] modes)
        {
            uint mode = 0;
            for (int i = 0; i < modes.Length; i++)
            {
                mode = mode | (uint)modes[i];
            }
            return (ushort)mode;
        }

        public bool Init(int boundaryLength, short unitLength, ushort mode = (ushort)PoolMode.AutoResize)
        {
            if (boundaryLength < HEAD_SIZE + SLOT_HEAD_SIZE + Math.Max(NEXT_SIZE, unitLength))
            { return false; }

            BOUNDARY_START = 0;
            bytes = new byte[boundaryLength];

            WriteRawLength(boundaryLength);
            WriteUnitLength(unitLength);
            WriteMode(mode);
            WriteFreeHead(0);
            BakeDeleteList();
            InitDeleteMap();
            return true;
        }


        public bool Init(int boundaryLength, short unitLength, params PoolMode[] modes)
        {
            var mode = MakePoolMode(modes);
            return Init(boundaryLength, unitLength, mode);
        }

        public bool Init(byte[] buffer, int boundaryStart, int boundaryLength, short unitLength, ushort mode = (ushort)PoolMode.AutoResize)
        {
            if (boundaryLength < HEAD_SIZE + SLOT_HEAD_SIZE + Math.Max(NEXT_SIZE, unitLength))
            { return false; }

            BOUNDARY_START = boundaryStart;
            bytes = buffer;

            WriteRawLength(boundaryLength);
            WriteUnitLength(unitLength);
            WriteMode(mode);
            WriteFreeHead(0);
            BakeDeleteList();
            InitDeleteMap();
            return true;
        }

        public bool Init(byte[] buffer, int boundaryStart, int boundaryLength, short unitLength, params PoolMode[] modes)
        {
            var mode = MakePoolMode(modes);
            return Init(buffer, boundaryStart, boundaryLength, unitLength, (ushort)mode);
        }

        public bool Load(byte[] buffer, int boundaryStart)
        {
            BOUNDARY_START = boundaryStart;

            bytes = buffer;

            InitDeleteMap();

            return true;
        }

        public void MoveTo(byte[] buffer, int start)
        {
            Buffer.BlockCopy(bytes, BOUNDARY_START, buffer, start, ReadRawLength());
        }

        public SerializedSpan ReadBuffer(int address)
        {
            var allocated = ReadSlotLengthInRelative(address);
            if (allocated == 0)
            { return SerializedSpan.FAIL; }

            return new SerializedSpan(bytes, TO_ABSOLUTE(address) + SLOT_HEAD_SIZE, address);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializedSpan CreateBuffer(int writeLen)
        {
            if (writeLen <= 0)
            { return SerializedSpan.FAIL; }

            writeLen = Math.Max(writeLen, MINIMAL_ALLOCATE_LENGTH);
            var units = (uint)((writeLen - 1) / ReadUnitLength() + 1);
            writeLen = (int)(units * ReadUnitLength());
            var bucket = CalcBucket((uint)units);

            int deleted = GetUsableDeletedHead(ref bucket, writeLen);
            if (deleted != INVALID_DELETELIST)
            {
                RemoveFromBucket(bucket, deleted);
                return new SerializedSpan(bytes, TO_ABSOLUTE(deleted) + SLOT_HEAD_SIZE, deleted);
            }

            var finalLength = (int)(SLOT_HEAD_SIZE + writeLen);
            if (IsFreeEnough(finalLength, out int free))
            {
                var nextfree = free + finalLength;
                WriteFreeHead(nextfree);
                WriteSlotLengthInRelative(free, writeLen);
                return new SerializedSpan(bytes, TO_ABSOLUTE(free) + SLOT_HEAD_SIZE, free);
            }

            var increasement = Math.Max(ReadRawLength() / 4, finalLength + finalLength / 4);
            if (IsAutoResize())
            {
                Resize(ReadRawLength() + increasement);
                return CreateBuffer(writeLen);
            }
            
            return SerializedSpan.FULL;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ErrorCode FreeBuffer(int address)
        {
            var allocated = ReadSlotLengthInRelative(address);
            if (allocated <= 0)
            { return ErrorCode.Fail; }
            var bucket = CalcBucket((uint)((allocated - 1) / ReadUnitLength() + 1));
            if (bucket < DLEETELIST_COUNT)
            {
                int current = ReadDeletedHead(bucket);
#if DELETELIST_ORDERED
                if (current == INVALID_DELETELIST)
                {
                    WriteSlotNextInRelative(address, INVALID_DELETELIST);
                    WriteDeletedHead(bucket, address);
                    return ErrorCode.OK;
                }
                var last = INVALID_DELETELIST;
                while (current > INVALID_DELETELIST)
                {
                    var currentLen = ReadSlotLengthInRelative(current);
                    if (currentLen < allocated)
                    {
                        last = current;
                        current = ReadSlotNextInRelative(current);
                        continue;
                    }
                    else
                    {
                        if (last == INVALID_DELETELIST)
                        {
                            WriteSlotNextInRelative(address, current);
                            WriteDeletedHead(bucket, address);
                            return ErrorCode.OK;
                        }
                        WriteSlotNextInRelative(last, address);
                        WriteSlotNextInRelative(address, current);
                        return ErrorCode.OK;
                    }
                }

                WriteSlotNextInRelative(last, address);
                WriteSlotNextInRelative(address, INVALID_DELETELIST);
                return ErrorCode.OK;
#else
                WriteSlotNextInRelative(address, current);
                WriteDeletedHead(bucket, address);
                return ErrorCode.OK;
#endif
            }
            else
            {
                return AddToDynamicBucket(bucket, address);
            }
        }

        public bool Resize(int boundaryLength)
        {
            if (allocator == null)
            {
                var newbytes = new byte[boundaryLength];
                var mapLength = ReadDeleteMapLength();
                Buffer.BlockCopy(bytes, 0, newbytes, 0, ReadRawLength());

                bytes = newbytes;
                WriteRawLength(boundaryLength);
                WriteDeleteMapLength(mapLength);
                deleteMap.MoveTo(newbytes, BUFFER_END());
                return true;
            }
            else
            {
                var resizeParams = allocator.Invoke(boundaryLength);
                return Resize(resizeParams.buffer, (int)resizeParams.start, (int)resizeParams.lengh);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Resize(byte[] buffer, int boundaryStart, int boundaryLength)
        {
            if (boundaryLength < ReadRawLength())
            { return false; }

            var mapLength = ReadDeleteMapLength();
            if (buffer == bytes && boundaryStart == BOUNDARY_START)
            {
                WriteRawLength(boundaryLength);
                WriteDeleteMapLength(mapLength);
                deleteMap.MoveTo(buffer, BUFFER_END());
                return true;
            }
            Buffer.BlockCopy(bytes, BOUNDARY_START, buffer, boundaryStart, ReadRawLength());
            BOUNDARY_START = boundaryStart;

            bytes = buffer;
            WriteRawLength(boundaryLength);
            WriteDeleteMapLength(mapLength);
            deleteMap.MoveTo(buffer, BUFFER_END());

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializedSpan RequireFirstBuffer()
        {
            var len = ReadSlotLengthInRelative(0);
            return ReadBuffer(0);
        }

        public void SetAllocator(Func<int, ResizeParams> allocator_)
        {
            allocator = allocator_;
        }

        private void BakeDeleteList()
        {
            for (int i = 0; i < DELETELIST_SIZE; i++)
            {
                bytes[BOUNDARY_START + DELETELIST_OFFSET + i] = byte.MaxValue;
            }
        }

        private void InitDeleteMap()
        {
            deleteMap = new SerializedFastMapInt16Int32();
            var deleteMapLength = ReadDeleteMapLength();
            if (deleteMapLength > 0)
            {
                deleteMap.Load(bytes, POOL_END() - DELETEMAP_LENGTH_SIZE - deleteMapLength);
            }
            else
            {
                deleteMapLength = (int)SerializedFastMapInt16Int32.MemoryWillAllocatedWithCapacity(32);
                WriteDeleteMapLength(deleteMapLength);
                deleteMap.Init(bytes, BUFFER_END(), deleteMapLength, 0);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsFreeEnough(int slotLength, out int free)
        {
            free = ReadFreeHead();
            if (free <= INVALID_FREELIST)
            { return false; }

            var freeAbsulute = TO_ABSOLUTE(free);
            var bufferEnd = BUFFER_END();
            var newFree = freeAbsulute + slotLength;
            return freeAbsulute + slotLength < bufferEnd;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short CalcBucket(uint allocated)
        {
            if (allocated < 16)
            { return (short)allocated; }

#if UNITY_2017_1_OR_NEWER
            var leadCount = 32 - QuickFinder.MathX.BinaryLength(allocated);
#else
            var leadCount = BitOperations.LeadingZeroCount(allocated);
#endif
            uint r = 0;
            int shift = 0;
            while (shift < 4 && r < 8)
            {
                r <<= 1;
                if (unchecked((int)(allocated << (leadCount + shift))) < 0)
                {
                    r += 1;
                }
                shift++;
                if (allocated >> shift == 0)
                {
                    break;
                }
            }

            r = (uint)((31 - leadCount) * 8 + (uint)((r << (leadCount + 1)) >> (leadCount + 1)));
            return (short)(r - 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUsableDeletedHead(ref short bucket, int requireLength)
        {
            var head = 0;
            if (bucket < DLEETELIST_COUNT)
            {
                head = ReadDeletedHead(bucket);
                while (head > INVALID_DELETELIST)
                {
                    var slotLength = ReadSlotLengthInRelative(head);
                    if (slotLength >= requireLength)
                    { return head; }
                    head = ReadSlotNextInRelative(head);
                }
                if (bucket < DLEETELIST_COUNT - 1)
                {
                    head = ReadDeletedHead(++bucket);
                    while (head > INVALID_DELETELIST)
                    {
                        var slotLength = ReadSlotLengthInRelative(head);
                        if (slotLength >= requireLength)
                        { return head; }
                        head = ReadSlotNextInRelative(head);
                    }
                }
            }

            if (bucket == DLEETELIST_COUNT - 1)
            { ++bucket; }

            if (deleteMap.Find(bucket, out head))
            {
                return head;
            }
            if (deleteMap.Find(++bucket, out head))
            {
                return head;
            }

            return INVALID_DELETELIST;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveFromBucket(short bucket, int deleted)
        {
            if (bucket < DLEETELIST_COUNT)
            {
                var head = ReadDeletedHead(bucket);
                if (head == deleted)
                {
                    var next = ReadSlotNextInRelative(head);
                    WriteDeletedHead(bucket, next);
                    return;
                }
#if DELETELIST_ORDERED
                var last = head;
                head = ReadSlotNextInRelative(head);
                while (head > INVALID_DELETELIST)
                {
                    if (head == deleted)
                    {
                        WriteSlotNextInRelative(last, ReadSlotNextInRelative(deleted));
                        break;
                    }
                    last = head;
                    head = ReadSlotNextInRelative(head);
                }
#endif
            }
            else
            {
                RemoveFromDynamicBucket(bucket, deleted);
            }
        }

        private ErrorCode AddToDynamicBucket(short bucket, int address)
        {
            if (deleteMap.Find(bucket, out var head))
            {
                WriteSlotNextInRelative(address, head);
            }
            else
            {
                WriteSlotNextInRelative(address, INVALID_DELETELIST);
            }

            if (!deleteMap.Add(bucket, address))
            {
                var newBucketsLength = deleteMap.NextResizeMemoryLength();
                if (DELETE_MAP_START((int)newBucketsLength) > TO_ABSOLUTE(ReadFreeHead()) + 1)
                {
                    WriteDeleteMapLength((int)newBucketsLength);
                    deleteMap.Resize(bytes, BUFFER_END(), newBucketsLength);
                }
                else
                {
                    var newBoundaryLength = ReadRawLength();
                    newBoundaryLength += (int)newBucketsLength;
                    newBoundaryLength += newBoundaryLength / 4;
                    if(IsAutoResize())
                    {
                        Resize(newBoundaryLength);
                    }
                    else
                    {
                        return ErrorCode.Full;
                    }
                }
                deleteMap.Add(bucket, address);
            }

            return ErrorCode.OK;
        }

        private void RemoveFromDynamicBucket(short bucket, int address)
        {
            if (!deleteMap.Find(bucket, out var head))
            {
#if SERIALIZEDPOOL_EXCEPTION
                throw new InvalidOperationException("ERROR! NO SUCH BUCKET " + bucket);
#else
                return;
#endif
            }

            if(head == INVALID_DELETELIST)
            { return; }

            var next = ReadSlotNextInRelative(head);
            deleteMap.Add(bucket, next);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ReadDeletedHead(short bucket)
        {
            return ReadInt(BOUNDARY_START + DELETELIST_OFFSET + bucket * 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteDeletedHead(short bucket, int deleted)
        {
            //var index = GetDeletedListIndex(bucket, out var minLength);
            WriteInt(BOUNDARY_START + DELETELIST_OFFSET + bucket * 4, deleted);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int ReadRawLength() { return ReadInt(BOUNDARY_START); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WriteRawLength(int boundaryLengh) { WriteInt(BOUNDARY_START, boundaryLengh); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public short ReadUnitLength() { return ReadShort(BOUNDARY_START + BOUNDARY_LENGTH_SIZE); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WriteUnitLength(short boundaryLengh) { WriteShort(BOUNDARY_START + BOUNDARY_LENGTH_SIZE, boundaryLengh); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ushort ReadMode() { return (ushort)ReadShort(BOUNDARY_START + BOUNDARY_LENGTH_SIZE + UNIT_LENGTH_SIZE); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WriteMode(ushort mode_) { WriteShort(BOUNDARY_START + BOUNDARY_LENGTH_SIZE + UNIT_LENGTH_SIZE, (short)mode_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int ReadDeleteMapLength() { return ReadInt(POOL_END() - DELETEMAP_LENGTH_SIZE); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WriteDeleteMapLength(int mapLength) { WriteInt(POOL_END() - DELETEMAP_LENGTH_SIZE, mapLength); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsAutoResize() { return ReadMode() == 1; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int ReadSlotLengthInRelative(int address) { return ReadInt(BOUNDARY_START + HEAD_SIZE + address); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WriteSlotLengthInRelative(int address, int next) { WriteInt(BOUNDARY_START + HEAD_SIZE + address, next); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int ReadSlotNextInRelative(int address) { return ReadInt(BOUNDARY_START + HEAD_SIZE + address + SLOT_HEAD_SIZE); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WriteSlotNextInRelative(int address, int next) { WriteInt(BOUNDARY_START + HEAD_SIZE + address + SLOT_HEAD_SIZE, next); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int ReadFreeHead() { return ReadInt(BOUNDARY_START + FREELIST_OFFSET); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void WriteFreeHead(int head) { WriteInt(BOUNDARY_START + FREELIST_OFFSET, head); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void WriteInt(int start, int value) { Utility.WriteInt(bytes, start, value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private int ReadInt(int start) { return Utility.ReadInt(bytes, start); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void WriteShort(int start, short value) { Utility.WriteShort(bytes, start, value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private short ReadShort(int start) { return Utility.ReadShort(bytes, start); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int TO_RELATIVE(int absolute) { return absolute - BOUNDARY_START - HEAD_SIZE; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int TO_ABSOLUTE(int relative) { return relative + BOUNDARY_START + HEAD_SIZE; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int BUFFER_START() { return BOUNDARY_START + HEAD_SIZE; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int BUFFER_END() { return BOUNDARY_START + ReadRawLength() - DELETEMAP_LENGTH_SIZE - ReadDeleteMapLength(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int DELETE_MAP_START(int deleteMapLength) { return BOUNDARY_START + ReadRawLength() - DELETEMAP_LENGTH_SIZE - deleteMapLength; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int POOL_END() { return BOUNDARY_START + ReadRawLength(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int DELETELIST_START() { return BOUNDARY_START + BOUNDARY_LENGTH_SIZE; }

    }
}
