//------------------------------------------------------------//
// yanfei 2024.11.22
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
namespace QuickFinder.Container
{
    public class SerializedFastMapInt16Int32
    {
        public const int HEAD_SIZE = 10;
        public const int SLOT_SIZE = 8;

        public const int MAX_UNSIGN_VALUE = -1;
        public const int MAX_SIGN_VALUE = short.MaxValue;
        public const int MAX_CAPACITY = short.MaxValue - 1;

        public const int SLOT_MASK = MAX_SIGN_VALUE;
        public const int SET_HEAD_MASK = unchecked((int)0x8000);

        public const int HEAD_ONLY_SLOT = MAX_UNSIGN_VALUE;
        public const int TAIL_SLOT = MAX_SIGN_VALUE;
        public const int INVALID_SLOT = MAX_UNSIGN_VALUE;

        private short capacity { get { return ReadCapacity(); } set { SaveCapacity(value); } }
        private short count { get { return ReadCount(); } set { SaveCount(value); } }
        private short freeList { get { return ReadFreelist(); } set { SaveFreelist(value); } }
        private uint mode { get { return ReadMode(); } set { SaveMode(value); } }
        private byte[] bytes;

        private int BOUNDARY_START;
        private int BOUNDARY_LENGTH;
        private int BUFFER_START;
        private int BUFFER_LENGTH;

        Func<long, ResizeParams> allocator = null;

        public static long MemoryWillAllocatedWithCapacity(int capacity)
        {
            return HEAD_SIZE + SLOT_SIZE * HashHelpers.GetPrime(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int Capacity() { return capacity; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int Count() { return count; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsAutoResize() { return ((uint)mode & (uint)ContainerMode.AutoResize) > 0; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int RawLength() { return HEAD_SIZE + BUFFER_LENGTH; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveCapacity(short capacity_) { BitConverter.TryWriteBytes(new Span<byte>(bytes, BOUNDARY_START, 2), capacity_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveCount(short count_) { BitConverter.TryWriteBytes(new Span<byte>(bytes, BOUNDARY_START + 2, 2), count_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveFreelist(short freeList_) { BitConverter.TryWriteBytes(new Span<byte>(bytes, BOUNDARY_START + 4, 2), freeList_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveMode(uint mode_) { BitConverter.TryWriteBytes(new Span<byte>(bytes, BOUNDARY_START + 6, 4), mode_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private short ReadCapacity() { return BitConverter.ToInt16(bytes, BOUNDARY_START); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private short ReadCount() { return BitConverter.ToInt16(bytes, BOUNDARY_START + 2); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private short ReadFreelist() { return BitConverter.ToInt16(bytes, BOUNDARY_START + 4); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private uint ReadMode() { return BitConverter.ToUInt32(bytes, BOUNDARY_START + 6); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private Span<Slot> GetSpan() { return MemoryMarshal.Cast<byte, Slot>(new Span<byte>(bytes, BUFFER_START, BUFFER_LENGTH)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpan<Slot> GetReadonlySpan() { return MemoryMarshal.Cast<byte, Slot>(new ReadOnlySpan<byte>(bytes, BUFFER_START, BUFFER_LENGTH)); }

        public bool Init(short estimateCapacity, uint mode_ = (uint)(ContainerMode.AutoResize))
        {
            long capacity = HashHelpers.GetPrime(estimateCapacity);
            if(capacity > MAX_CAPACITY)
            {
                capacity = MAX_CAPACITY;
            }
            this.capacity = (short)capacity;
            BUFFER_LENGTH = (short)capacity * SLOT_SIZE;
            bytes = new byte[HEAD_SIZE + BUFFER_LENGTH];
            count = 0;
            mode = mode_;

            BOUNDARY_START = 0;
            BOUNDARY_LENGTH = bytes.Length;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;

            BakeFreeList(0, (short)capacity, -1);
            freeList = 0;
            return true;
        }

        public bool Init(byte[] buffer, int istart, int length, uint mode_ = (uint)(ContainerMode.AutoResize))
        {
            bytes = buffer;
            BOUNDARY_START = istart;
            BOUNDARY_LENGTH = length;
            capacity = (short)((length - HEAD_SIZE) / SLOT_SIZE);
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
            BUFFER_LENGTH = capacity * SLOT_SIZE;
            mode = mode_;
            BakeFreeList(0, capacity, -1);
            return true;
        }

        public bool Load(byte[] buffer, int istart)
        {
            //ReadHead(buffer, istart, out capacity, out count, out freeList, out mode);
            bytes = buffer;
            BOUNDARY_START = istart;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
            BUFFER_LENGTH = capacity * SLOT_SIZE;
            BOUNDARY_LENGTH = HEAD_SIZE + BUFFER_LENGTH;

            return true;
        }

        public void MoveTo(byte[] buffer, int istart)
        {
            Buffer.BlockCopy(bytes, BOUNDARY_START, buffer, istart, BOUNDARY_LENGTH);
            bytes = buffer;
            BOUNDARY_START = istart;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
            WriteHead(bytes, BOUNDARY_START, capacity, count, freeList, mode);
        }

        private static void WriteHead(byte[] buffer, int boundaryStart, short capacity_, short count_, short freeList_, uint mode_)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart,     2), capacity_);
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart + 2, 2), count_);
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart + 4, 2), freeList_);
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart + 6, 4), mode_);
        }

        private static void ReadHead(byte[] buffer, int boundaryStart, out short capacity_, out short count_, out short freeList_, out uint mode_)
        {
            capacity_ = BitConverter.ToInt16(buffer, boundaryStart);
            count_ = BitConverter.ToInt16(buffer,    boundaryStart + 2);
            freeList_ = BitConverter.ToInt16(buffer, boundaryStart + 4);
            mode_ = BitConverter.ToUInt32(buffer,    boundaryStart + 6);
        }

        private void BakeFreeList(short newSlotStart, short newSlotCount, short free)
        {
            freeList = free;
            var span = GetSpan();
            for (short i = (short)(newSlotStart + newSlotCount - 1); i >= newSlotStart; i--)
            {
                PushFree(span, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushFree(Span<Slot> slots, short islot)
        {
            slots[islot].Clear();
            slots[islot].nextFree = freeList;
            slots[islot].prevFree = INVALID_SLOT;
            if (freeList != INVALID_SLOT)
            { slots[freeList].prevFree = islot; }
            freeList = islot;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short PopFree(Span<Slot> slots, short free)
        {
            var prev = slots[free].prevFree;
            var next = slots[free].nextFree;
            if (prev != INVALID_SLOT)
            { slots[prev].nextFree = next; }
            if (next != INVALID_SLOT)
            { slots[next].prevFree = prev; }

            if (freeList == free)
            { freeList = next; }

            return free;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Find(short key, out int value)
        {
            return Find(key, out value, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Find(short key, out int value, out short location)
        {
            var slots = GetReadonlySpan();
            var islot = (key & SLOT_MASK) % capacity;
            //var islot = (int)(((int)key[HASH_INDEX] & SLOT_MASK) % capacity);
            var current = islot;
            for (; ; )
            {
                if (slots[current].IsEmpty())
                {
                    value = 0; location = (short)islot;
                    return false;
                }
                if (slots[current].key == key)
                {
                    value = slots[current].value;
                    location = (short)current;
                    return true;
                }
                current = slots[current].next & SLOT_MASK;
                if (current == TAIL_SLOT)
                {
                    value = 0; location = (short)islot;
                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(short key, int value)
        {
            var slots = GetSpan();
            if (Find(key, out _, out var islot))
            {
                slots[islot].value = value;
                return true;
            }

            if (!TryAddCount(1))
            {
                if (IsAutoResize())
                {
                    Resize();
                    return Add(key, value);
                }
                else
                { return false; }
            }

            var current = islot;
            if (slots[current].IsEmpty())
            {
                PopFree(slots, current);
                slots[current].next = HEAD_ONLY_SLOT;
                slots[current].key = key;
                slots[current].value = value;
                return true;
            }

            if (slots[current].IsHead())
            {
                var freeOld = PopFree(slots, freeList);

                slots[freeOld].key = key;
                slots[freeOld].value = value;
                slots[freeOld].next = (short)(slots[current].next & SLOT_MASK);
                slots[current].next = (short)(freeOld + SET_HEAD_MASK);
                return true;
            }
            else
            {
                var freeOld = PopFree(slots, freeList);
                var prev = Prev(slots, islot);
                slots[prev].next = slots[prev].IsHead() ? (short)(freeOld + SET_HEAD_MASK) : freeOld;

                slots[freeOld] = slots[islot];

                slots[islot].next = HEAD_ONLY_SLOT;
                slots[islot].key = key;
                slots[islot].value = value;
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Delete(short key, out int value)
        {
            var slots = GetSpan();
            if (!Find(key, out value, out var islot))
            { return false; }

            TryAddCount(-1);

            value = slots[islot].value;

            if (slots[islot].IsHead())
            {
                if (slots[islot].next == HEAD_ONLY_SLOT)
                {
                    PushFree(slots, islot);
                }
                else
                {
                    var next = slots[islot].next & SLOT_MASK;
                    slots[islot] = slots[next];
                    slots[islot].next = (short)(slots[next].next + SET_HEAD_MASK);

                    PushFree(slots, (short)next);
                }
            }
            else
            {
                var prev = Prev(slots, islot);
                var next = slots[islot].next;
                next = (short)(slots[prev].IsHead() ? next + SET_HEAD_MASK : next);
                slots[prev].next = next;
                PushFree(slots, islot);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Resize()
        {
            var slots = GetSpan();
            var oldCapacity = capacity;
            var oldCount = count;
            var oldFreeList = freeList;

            if(oldCapacity == MAX_CAPACITY)
            {
                throw new ArgumentOutOfRangeException("capacity out of MAX_CAPACITY");
            }
            var newCapacity = (int)HashHelpers.GetPrime(capacity + capacity / 2);
            if(newCapacity > MAX_CAPACITY)
            {
                newCapacity = MAX_CAPACITY;
            }

            //Console.WriteLine("resize old count " + Count() + " old capacity " + capacity + "  new capacity " + newCapacity);

            if (allocator == null)
            {
                Init((short)newCapacity, mode);

                for (int i = 0; i < oldCapacity; i++)
                {
                    if (slots[i].IsEmpty())
                    { continue; }

                    Add(slots[i].key, slots[i].value);
                }
            }
            else
            {
                var newLength = MemoryWillAllocatedWithCapacity(newCapacity);
                var param = allocator.Invoke(newLength);
                Resize(param.buffer, param.start, param.lengh);
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Resize(byte[] newbuffer, long istart, long boundaryLength)
        {
            var newCapacity = (int)((boundaryLength - HEAD_SIZE) / SLOT_SIZE);
            if (newCapacity < capacity)
            { return false; }

            if(newCapacity > MAX_CAPACITY)
            { return false; }

            if (newbuffer == bytes && istart == BOUNDARY_START && capacity == newCapacity)
            { return false; }

            if (newbuffer != bytes || istart != BOUNDARY_START)
            { MoveTo(newbuffer, (int)istart); }
            if (capacity == newCapacity)
            { return true; }
            BOUNDARY_LENGTH = (int)boundaryLength;
            BUFFER_LENGTH = newCapacity * SLOT_SIZE;

            BakeFreeList(capacity, (short)(newCapacity - capacity), freeList);
            capacity = (short)newCapacity;

            var slots = GetSpan();
            for (int i = 0; i < capacity; i++)
            {
                if (slots[i].IsEmpty())
                { continue; }
                slots[i].next = HEAD_ONLY_SLOT;
            }

            for (short i = 0; i < capacity;)
            {
                if (slots[i].IsEmpty())
                { i++; continue; }

                var inewSlot = (short)((slots[i].key & SLOT_MASK) % newCapacity);
                if (slots[inewSlot].IsEmpty())
                {
                    var popSlot = PopFree(slots, inewSlot);
                    slots[inewSlot] = slots[i];
                    slots[inewSlot].next = HEAD_ONLY_SLOT;
                    PushFree(slots, i);

                    i++;
                    continue;
                }

                if (!slots[i].IsHead())
                { i++; continue; }

                if (inewSlot == i)
                { i++; continue; }

                if (slots[inewSlot].IsHead())
                {
                    var inewSlotKeySlot = (slots[inewSlot].key & SLOT_MASK) % newCapacity;
                    if (inewSlotKeySlot != inewSlot)
                    {
                        var tmp = slots[i];
                        slots[i] = slots[inewSlot];
                        slots[inewSlot] = tmp;

                        continue; //just swap, don't move i, check again
                    }
                    else
                    {
                        if (slots[inewSlot].next != HEAD_ONLY_SLOT)
                        {
                            slots[i].next = (short)(slots[inewSlot].next & SLOT_MASK);
                        }
                        else
                        {
                            slots[i].next = TAIL_SLOT;
                        }
                        slots[inewSlot].next = (short)(i + SET_HEAD_MASK);
                        i++;

                        continue;
                    }
                }
                else
                {
                    var prev = Prev(slots, inewSlot);
                    slots[prev].next = (short)(slots[prev].IsHead() ? i + SET_HEAD_MASK : i);

                    var tmp = slots[i];
                    slots[i] = slots[inewSlot];
                    slots[inewSlot] = tmp;
                    i++;

                    continue;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Prev(Span<Slot> slots, int islot)
        {
            var head = (slots[islot].key & SLOT_MASK) % capacity;
            var next = head;
            //var next = (int)(((int)slots[islot].key & SLOT_MASK) % capacity);
            for (; ; )
            {
                if ((slots[next].next & SLOT_MASK) == islot)
                { return next; }

                next = slots[next].next;
                next &= SLOT_MASK;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryAddCount(short num)
        {
            int newvalue = System.Math.Max(0, count + num);
            if (newvalue > capacity)
            { return false; }

            count = (short)(count + num);
            return true;
        }


        public int NextResizeMemoryLength()
        {
            int newCapacity = (int)HashHelpers.GetPrime(capacity + capacity / 4);
            if (newCapacity > MAX_CAPACITY)
            { newCapacity = MAX_CAPACITY; }

            return (int)MemoryWillAllocatedWithCapacity(newCapacity);
        }

        public void Optimise()
        {
            var slots = GetSpan();
            var optimiseCount = 0;
            for (short i = 0; i < capacity; i++)
            {
                if (slots[i].IsEmpty() || !slots[i].IsHead())
                { continue; }

                var current = i;
                var lastk = i;
                for (; ; )
                {
                    var next = (short)(slots[current].next & SLOT_MASK);
                    if (next == TAIL_SLOT)
                    { break; }
                    if (next - i > 8)
                    {
                        for (short k = lastk; k < lastk + 8; k++)
                        {
                            if (k > slots.Length - 1)
                            { break; }
                            if (slots[k].IsHead())
                            { continue; }
                            if (slots[k].IsEmpty())
                            {
                                PopFree(slots, k);
                                slots[k] = slots[next];
                                slots[current].next = (short)(slots[current].IsHead() ? k + SET_HEAD_MASK : k);
                                lastk = k;
                                optimiseCount++;
                                break;
                            }
                            var kKeySlot = (slots[k].key & SLOT_MASK) % capacity;
                            if (Math.Abs(kKeySlot - k) < 8)
                            { continue; }

                            var kprev = Prev(slots, k);
                            var tmp = slots[next];
                            slots[next] = slots[k];
                            slots[k] = tmp;
                            slots[current].next = (short)(slots[current].IsHead() ? k + SET_HEAD_MASK : k);
                            slots[kprev].next = (short)(slots[kprev].IsHead() ? next + SET_HEAD_MASK : next);
                            optimiseCount++;
                            break;
                        }
                    }
                    current = next;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValueEnumerator EveryKeyIndexValueEnumerator()
        {
            return new KeyValueEnumerator(GetSpan());
        }

        public ref struct KeyValueEnumerator
        {
            Span<Slot> container;
            private int islot;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueEnumerator(Span<Slot> container)
            {
                this.container = container;
                this.islot = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyValueEnumerator GetEnumerator()
            {
                return this;
            }

            public KeyValuePair<int, int> Current
            {
                get
                {
                    return new KeyValuePair<int, int>(container[islot].key, container[islot].value);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                for (; ; )
                {
                    islot++;
                    if (islot >= container.Length)
                    { return false; }

                    if (container[islot].IsEmpty())
                    {
                        continue;
                    }

                    return true;
                }

            }

            public void Reset()
            {
                this.islot = -1;
            }
        }

        public void SetAllocator(Func<long, ResizeParams> allocator_)
        {
            allocator = allocator_;
        }


        [StructLayout(LayoutKind.Explicit, Pack = sizeof(byte))]
        public struct Slot
        {
            [FieldOffset(0)] public short next;
            [FieldOffset(2)] public short key;
            [FieldOffset(4)] public int value;

            [FieldOffset(2)] public short prevFree;
            [FieldOffset(4)] public short nextFree;
            [FieldOffset(0)] public short flag;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsHead() { return next < 0; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Clear() { flag = MAX_CAPACITY; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsEmpty() { return flag == MAX_CAPACITY; }
        }

        #region analysic

        //public Dictionary<int, int> Tongji()
        //{
        //    var map = new Dictionary<int, int>();
        //    var slots = GetSlotSpan();

        //    int totalcount = 0, zeroCount = 0;
        //    for (int i = 0; i < slots.Length; i++)
        //    {
        //        if (slots[i].IsZero())
        //        { zeroCount++; };
        //        if (slots[i].IsEmpty())
        //        { continue; }
        //        if (!slots[i].IsHead())
        //        { continue; }
        //        totalcount++;
        //        int chainCount = 0;
        //        var current = i;
        //        for (; ; )
        //        {
        //            chainCount++;
        //            current = slots[current].next;
        //            if (current == HEAD_ONLY_SLOT)
        //            { break; }
        //            current &= SLOT_MASK;
        //            if (current == TAIL_SLOT)
        //            { break; }
        //        }
        //        if (!map.TryGetValue(chainCount, out var cc))
        //        {
        //            map[chainCount] = 1;
        //        }
        //        else
        //        {
        //            map[chainCount] = cc + 1;
        //        }
        //    }

        //    Console.WriteLine("tongji head count " + totalcount);
        //    Console.WriteLine("tongji zero count " + zeroCount);
        //    var list = map.Keys.ToList();
        //    list.Sort();
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        var chainCount = list[i];
        //        Console.WriteLine(chainCount.ToString() + "     " + map[chainCount]);
        //    }

        //    return map;
        //}
        #endregion

    }
}
