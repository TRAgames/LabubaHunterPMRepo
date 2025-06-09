//------------------------------------------------------------//
// yanfei 2024.11.22
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuickFinder.Container
{
    public class SerializedGuid2IntMap
    {
        public const int HEAD_SIZE = 16;
        public const int SLOT_SIZE = 40;
        public const int HASH_INDEX = 2;

        public const int MAX_UNSIGN_VALUE = -1;
        public const int MAX_SIGN_VALUE = int.MaxValue;
        public const int MAX_CAPACITY = int.MaxValue;

        public const int SLOT_MASK = MAX_SIGN_VALUE;
        public const int SET_HEAD_MASK = unchecked((int)0x80000000);

        public const int HEAD_ONLY_SLOT = -1;
        public const int TAIL_SLOT = MAX_SIGN_VALUE;
        public const int INVALID_SLOT = MAX_UNSIGN_VALUE;

        private int capacity;
        private int count;
        private int freeList;
        private uint mode;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private Span<Slot> GetSpan() { return MemoryMarshal.Cast<byte, Slot>(new Span<byte>(bytes, BUFFER_START, BUFFER_LENGTH)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpan<Slot> GetReadonlySpan() { return MemoryMarshal.Cast<byte, Slot>(new ReadOnlySpan<byte>(bytes, BUFFER_START, BUFFER_LENGTH)); }

        public bool Init(int estimateCapacity, uint mode_ = (uint)(ContainerMode.AutoResize))
        {
            capacity = (int)HashHelpers.GetPrime(estimateCapacity);
            bytes = new byte[HEAD_SIZE + capacity * SLOT_SIZE];
            count = 0;
            mode = mode_;

            BOUNDARY_START = 0;
            BOUNDARY_LENGTH = bytes.Length;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
            BUFFER_LENGTH = capacity * SLOT_SIZE;

            BakeFreeList(0, capacity, -1);
            freeList = 0;
            //WriteHead(bytes, BOUNDARY_START, capacity, count, freeList, mode);
            return true;
        }

        public bool Init(byte[] buffer, int istart, int length, uint mode_ = (uint)(ContainerMode.AutoResize))
        {
            bytes = buffer;
            BOUNDARY_START = istart;
            BOUNDARY_LENGTH = length;
            capacity = (int)(length - HEAD_SIZE) / SLOT_SIZE;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
            BUFFER_LENGTH = capacity * SLOT_SIZE;
            mode = mode_;
            BakeFreeList(0, capacity, -1);
            //WriteHead(bytes, BOUNDARY_START, capacity, count, freeList, mode);
            return true;
        }

        public bool Load(byte[] buffer, int istart)
        {
            ReadHead(buffer, istart, out capacity, out count, out freeList, out mode);
            bytes = buffer;
            BOUNDARY_START = istart;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
            BUFFER_LENGTH = capacity * SLOT_SIZE;
            BOUNDARY_LENGTH = HEAD_SIZE + BUFFER_LENGTH;
            return true;
        }

        public void MoveTo(byte[] buffer, int istart)
        {
            WriteHead(bytes, BOUNDARY_START, capacity, count, freeList, mode);
            Buffer.BlockCopy(bytes, BOUNDARY_START, buffer, istart, BOUNDARY_LENGTH);
            bytes = buffer;
            BOUNDARY_START = istart;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
        }

        private static void WriteHead(byte[] buffer, int boundaryStart, int capacity_, int count_, int freeList_, uint mode_)
        {
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart, 4), capacity_);
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart + 4, 4), count_);
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart + 8, 4), freeList_);
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart + 12, 4), mode_);
        }

        private static void ReadHead(byte[] buffer, int boundaryStart, out int capacity_, out int count_, out int freeList_, out uint mode_)
        {
            capacity_ = BitConverter.ToInt32(buffer, boundaryStart);
            count_ = BitConverter.ToInt32(buffer, boundaryStart + 4);
            freeList_ = BitConverter.ToInt32(buffer, boundaryStart + 8);
            mode_ = BitConverter.ToUInt32(buffer, boundaryStart + 12);
        }

        private void BakeFreeList(int newSlotStart, int newSlotCount, int free)
        {
            freeList = free;
            var span = GetSpan();
            for (int i = newSlotStart + newSlotCount - 1; i >= newSlotStart; i--)
            {
                PushFree(span, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushFree(Span<Slot> slots, int islot)
        {
            slots[islot].Clear();
            slots[islot].next = freeList;
            slots[islot].prev = INVALID_SLOT;
            if (freeList != INVALID_SLOT)
            { slots[freeList].prev = islot; }
            freeList = islot;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PopFree(Span<Slot> slots, int free)
        {
            var prev = slots[free].prev;
            var next = slots[free].next;
            if (prev != INVALID_SLOT)
            { slots[prev].next = next; }
            if (next != INVALID_SLOT)
            { slots[next].prev = prev; }

            if (freeList == free)
            { freeList = next; }
            return free;
        }

        public string GetGuid(int islot)
        {
            var slots = GetReadonlySpan();
            return slots[islot].GetGuid();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Find(string key, out int value, out int location)
        {
            Span<byte> span = stackalloc byte[32];
            for (int i = 0; i < 32; i++) { span[i] = (byte)key[i]; }
            var longSpan = MemoryMarshal.Cast<byte, GuidStruct>(span);
            return Find(longSpan[0], out value, out location);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Find(ReadOnlySpan<byte> key, out int value, out int location)
        {
            var longSpan = MemoryMarshal.Cast<byte, GuidStruct>(key);
            return Find(longSpan[0], out value, out location);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Find(GuidStruct key, out int value, out int location)
        {
            var slots = GetSpan();
            var islot = (int)((key.KeyHash()) % (ulong)capacity);
            //var islot = (int)(((int)hashKey & SLOT_MASK) % capacity);
            var current = islot;
            for (; ; )
            {
                if (slots[current].IsEmpty())
                {
                    value = 0; location = islot;
                    return false;
                }
                if (slots[current].key.IsSameKey(key))
                {
                    value = slots[current].value;
                    location = current;
                    return true;
                }
                current = slots[current].next & SLOT_MASK;
                if (current == TAIL_SLOT)
                {
                    value = 0; location = islot;
                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(string key, int value)
        {
            Span<byte> span = stackalloc byte[32];
            for (int i = 0; i < 32; i++)
            { span[i] = (byte)key[i]; }
            var longSpan = MemoryMarshal.Cast<byte, GuidStruct>(span);
            return Add(longSpan[0], value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(byte[] key, int value)
        {
            var longSpan = MemoryMarshal.Cast<byte, GuidStruct>(new ReadOnlySpan<byte>(key));
            return Add(longSpan[0], value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Add(GuidStruct key, int value)
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
                    islot = (int)((key.KeyHash()) % (ulong)capacity);
                    //islot = (int)(((int)hashKey & SLOT_MASK) % capacity);
                }
                else
                { return false; }
            }

            int current = islot;
            if (slots[current].IsEmpty())
            {
                PopFree(slots, current);
                slots[current].next = HEAD_ONLY_SLOT;
                slots[current].SetKey(key);
                slots[current].value = value;
                return true;
            }

            if (slots[current].IsHead())
            {
                var freeOld = PopFree(slots, freeList);

                slots[freeOld].SetKey(key);
                slots[freeOld].value = value;
                slots[freeOld].next = (int)(slots[current].next & SLOT_MASK);
                slots[current].next = (int)(freeOld + SET_HEAD_MASK);

                return true;
            }
            else
            {
                var freeOld = PopFree(slots,freeList);
                var prev = Prev(slots, islot);
                slots[prev].next = slots[prev].IsHead() ? (int)(freeOld + SET_HEAD_MASK) : freeOld;

                slots[freeOld] = slots[islot];

                slots[islot].next = HEAD_ONLY_SLOT;
                slots[islot].SetKey(key);
                slots[islot].value = value;
                return true;
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Delete(string key, out int value)
        {
            Span<byte> span = stackalloc byte[32];
            for (int i = 0; i < 32; i++)
            { span[i] = (byte)key[i]; }
            var longSpan = MemoryMarshal.Cast<byte, GuidStruct>(span);
            return Delete(longSpan[0], out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Delete(byte[] key, out int value)
        {
            var longSpan = MemoryMarshal.Cast<byte, GuidStruct>(new ReadOnlySpan<byte>(key));
            return Delete(longSpan[0], out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Delete(GuidStruct key, out int value)
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
                    slots[islot].next = (int)(slots[next].next + SET_HEAD_MASK);

                    PushFree(slots, next);
                }
            }
            else
            {
                var prev = Prev(slots,islot);
                var next = slots[islot].next;
                next = (int)(slots[prev].IsHead() ? next + SET_HEAD_MASK : next);
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

            var newCapacity = (int)HashHelpers.GetPrime(capacity + capacity / 4);
            if (allocator == null)
            {
                Init(newCapacity, mode);

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

            if (newbuffer == bytes && istart == BOUNDARY_START && capacity == newCapacity)
            { return false; }

            if (newbuffer != bytes || istart != BOUNDARY_START)
            { MoveTo(newbuffer, (int)istart); }
            BOUNDARY_LENGTH = (int)boundaryLength;
            BUFFER_LENGTH = newCapacity * SLOT_SIZE;
            if (capacity == newCapacity)
            { return true; }

            var oldCapacity = capacity;
            capacity = newCapacity;
            BakeFreeList(oldCapacity, newCapacity - oldCapacity, freeList);

            var slots = GetSpan();
            for (int i = 0; i < oldCapacity; i++)
            {
                if (slots[i].IsEmpty())
                { continue; }
                slots[i].next = HEAD_ONLY_SLOT;
            }

            for (int i = 0; i < capacity;)
            {
                if (slots[i].IsEmpty())
                { i++; continue; }

                var inewSlot = (int)((slots[i].KeyHash()) % (ulong)capacity);
                if (slots[inewSlot].IsEmpty())
                {
                    var popSlot = PopFree(slots, inewSlot);
                    slots[inewSlot] = slots[i];
                    slots[inewSlot].next = HEAD_ONLY_SLOT;
                    PushFree(slots, i);

                    i++;
                    continue;
                }

                if(!slots[i].IsHead())
                { i++; continue; }

                if (inewSlot == i)
                { i++; continue; }

                if (slots[inewSlot].IsHead())
                {
                    var inewSlotKeySlot = (int)(slots[inewSlot].KeyHash() % (ulong)capacity);
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
                            slots[i].next = slots[inewSlot].next & SLOT_MASK;
                        }
                        else
                        {
                            slots[i].next = TAIL_SLOT;
                        }
                        slots[inewSlot].next = i + SET_HEAD_MASK;
                        i++;

                        continue;
                    }
                }
                else
                {
                    var prev = Prev(slots, inewSlot);
                    slots[prev].next = slots[prev].IsHead() ? i + SET_HEAD_MASK : i;

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
            var head = (int)((slots[islot].KeyHash()) % (ulong)capacity);
            var next = head;
            for (; ; )
            {
                if ((slots[next].next & SLOT_MASK) == islot)
                { return next; }

                next = slots[next].next;
                next &= SLOT_MASK;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryAddCount(int num)
        {
            int newvalue = System.Math.Max(0, count + num);
            if (newvalue > capacity)
            { return false; }

            count = (count + num);
            return true;
        }

        public void Optimise()
        {
            var slots = GetSpan();
            var optimiseCount = 0;
            for (int i = 0; i < capacity; i++)
            {
                if (slots[i].IsEmpty() || !slots[i].IsHead())
                { continue; }

                var current = i;
                var lastk = i;
                for (; ; )
                {
                    var next = slots[current].next & SLOT_MASK;
                    if (next == TAIL_SLOT)
                    { break; }
                    if (next - i > 8)
                    {
                        for (int k = lastk; k < lastk + 8; k++)
                        {
                            if (k > slots.Length - 1)
                            { break; }
                            if (slots[k].IsHead())
                            { continue; }
                            if (slots[k].IsEmpty())
                            {
                                PopFree(slots, k);
                                slots[k] = slots[next];
                                slots[current].next = slots[current].IsHead() ? k + SET_HEAD_MASK : k;
                                lastk = k;
                                optimiseCount++;
                                break;
                            }
                            var kKeySlot = (int)((slots[k].KeyHash()) % (ulong)capacity);
                            if (Math.Abs(kKeySlot - k) < 8)
                            { continue; }

                            var kprev = Prev(slots, k);
                            var tmp = slots[next];
                            slots[next] = slots[k];
                            slots[k] = tmp;
                            slots[current].next = slots[current].IsHead() ? k + SET_HEAD_MASK : k;
                            slots[kprev].next = slots[kprev].IsHead() ? next + SET_HEAD_MASK : next;
                            optimiseCount++;
                            break;
                        }
                    }
                    current = next;
                }
            }
        }

        public int NextResizeMemoryLength()
        {
            int newCapacity = (int)HashHelpers.GetPrime(capacity + capacity / 4);
            if(newCapacity > MAX_CAPACITY)
            { newCapacity = MAX_CAPACITY; }
            return newCapacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyIndexValueEnumerator EveryKeyIndexValueEnumerator()
        {
            return new KeyIndexValueEnumerator(GetSpan());
        }

        public ref struct KeyIndexValueEnumerator
        {
            private readonly Span<Slot> container;
            private int islot;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyIndexValueEnumerator(Span<Slot> container)
            {
                this.container = container;
                this.islot = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public KeyIndexValueEnumerator GetEnumerator()
            {
                return this;
            }

            public KeyValuePair<int, int> Current
            {
                get
                {
                    var value = container[islot].value;
                    return new KeyValuePair<int, int>(islot, value);
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


        [StructLayout(LayoutKind.Explicit, Pack = sizeof(ulong))]
        public struct Slot 
        {
            [FieldOffset(0)] public GuidStruct key;

            [FieldOffset(32)] public int next;
            [FieldOffset(36)] public int value;

            [FieldOffset(36)] public int prev;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsEmpty() { return key.IsEmpty(); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Clear() { key.Clear(); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsZero() { return key.IsZero(); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsSameKey(ReadOnlySpan<ulong> key)
            {
                return this.key.IsSameKey(key);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetKey(ReadOnlySpan<ulong> key)
            {
                this.key.SetKey(key);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetKey(GuidStruct key)
            {
                this.key.SetKey(key);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsHead() { return next < 0; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ulong KeyHash()
            {
                return key.KeyHash();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsSameGuid(string guid)
            {
                return key.IsSameGuid(guid);
            }

            public string GetGuid()
            {
                return key.GetGuid();
            }

            #region debug

            public void DebugLog(string headInfo)
            {
                string.Format("{0:12} {1} {2 }{3}", headInfo, GetGuid(), next, value);
            }
            #endregion
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
