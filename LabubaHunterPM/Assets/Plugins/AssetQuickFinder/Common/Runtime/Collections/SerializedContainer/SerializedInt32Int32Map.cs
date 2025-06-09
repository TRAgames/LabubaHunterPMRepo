//------------------------------------------------------------//
// yanfei 2024.10.20
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

//don't support unsigined integer
using kint = System.Int32;
using vint = System.Int32;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuickFinder.Container
{
    public class SerializedInt32Int32Map : ISerializedMap<kint, vint>
    {
        public const int KEY_SIZE = sizeof(kint);
        public const int VALUE_SIZE = sizeof(vint);
        public const int HEAD_SIZE = KEY_SIZE * 3 + 4;
        public const int SLOT_SIZE = KEY_SIZE * 2 + VALUE_SIZE + 1;

        public const kint MAX_UNSIGN_VALUE = unchecked((kint)(((ulong)1 << KEY_SIZE * 8) - 1));
        public const kint MAX_SIGN_VALUE = unchecked((kint)(((ulong)1 << (KEY_SIZE * 8 - 1)) - 1));

        public const kint SLOT_MASK = MAX_SIGN_VALUE;
        public const kint SET_HEAD_MASK = unchecked((kint)((ulong)1 << (KEY_SIZE * 8 - 1)));

        public const kint HEAD_ONLY_SLOT = MAX_UNSIGN_VALUE;
        public const kint TAIL_SLOT = MAX_SIGN_VALUE;
        public const kint INVALID_SLOT = MAX_UNSIGN_VALUE;

        public const kint MAX_CAPACITY = MAX_SIGN_VALUE;

        private kint capacity { get { return ReadCapacity(); } set { SaveCapacity(value); } }
        private kint count { get { return ReadCount(); } set { SaveCount(value); } }
        private kint freeList { get { return ReadFreelist(); } set { SaveFreelist(value); } }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveCapacity(kint capacity_) { WriteKInt(bytes, BOUNDARY_START, capacity_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveCount(kint count_) { WriteKInt(bytes, BOUNDARY_START + KEY_SIZE, count_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveFreelist(kint freeList_) { WriteKInt(bytes, BOUNDARY_START + KEY_SIZE * 2, freeList_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private void SaveMode(uint mode_) { BitConverter.TryWriteBytes(new Span<byte>(bytes, BOUNDARY_START + KEY_SIZE * 3, 4), mode_); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private kint ReadCapacity() { return ReadKInt(bytes, BOUNDARY_START); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private kint ReadCount() { return ReadKInt(bytes, BOUNDARY_START + KEY_SIZE); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private kint ReadFreelist() { return ReadKInt(bytes, BOUNDARY_START + KEY_SIZE * 2); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private uint ReadMode() { return BitConverter.ToUInt32(bytes, BOUNDARY_START + KEY_SIZE * 3); }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] private Span<Slot> GetSpan() { return MemoryMarshal.Cast<byte, Slot>(new Span<byte>(bytes, BUFFER_START, BUFFER_LENGTH)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpan<Slot> GetReadonlySpan() { return MemoryMarshal.Cast<byte, Slot>(new ReadOnlySpan<byte>(bytes, BUFFER_START, BUFFER_LENGTH)); }

        public bool Init(kint estimateCapacity, uint mode_ = (uint)(ContainerMode.AutoResize))
        {
            estimateCapacity = (kint)HashHelpers.GetPrime(estimateCapacity);
            BUFFER_LENGTH = estimateCapacity * SLOT_SIZE;
            bytes = new byte[HEAD_SIZE + BUFFER_LENGTH];
            capacity = estimateCapacity;
            count = 0;
            mode = mode_;

            BOUNDARY_START = 0;
            BOUNDARY_LENGTH = bytes.Length;
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;

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
            capacity = (kint)HashHelpers.GetPrime((length - HEAD_SIZE) / SLOT_SIZE);
            BUFFER_START = BOUNDARY_START + HEAD_SIZE;
            BUFFER_LENGTH = capacity * SLOT_SIZE;
            count = 0;
            mode = mode_;
            BakeFreeList(0, capacity, -1);
            //WriteHead(bytes, BOUNDARY_START, capacity, count, freeList, mode);
            return true;
        }

        public bool Load(byte[] buffer, int istart)
        {
            //ReadHead(buffer, istart, out var capacity_, out var count_, out var freeList_, out var mode_);
            //capacity = capacity_;  count = count_; freeList = freeList_; mode = mode_;
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

        private static void WriteHead(byte[] buffer, int boundaryStart, kint capacity_, kint count_, kint freeList_, uint mode_)
        {
            WriteKInt(buffer, boundaryStart, capacity_);
            WriteKInt(buffer, boundaryStart + KEY_SIZE, count_);
            WriteKInt(buffer, boundaryStart + KEY_SIZE * 2, freeList_);
            BitConverter.TryWriteBytes(new Span<byte>(buffer, boundaryStart + KEY_SIZE * 3, 4), mode_);
        }

        private static void ReadHead(byte[] buffer, int boundaryStart, out kint capacity_, out kint count_, out kint freeList_, out uint mode_)
        {
            capacity_ = ReadKInt(buffer, boundaryStart);
            count_ = ReadKInt(buffer, boundaryStart + KEY_SIZE);
            freeList_ = ReadKInt(buffer, boundaryStart + KEY_SIZE * 2);
            mode_ = BitConverter.ToUInt32(buffer, boundaryStart + KEY_SIZE * 3);
        }

        private void BakeFreeList(kint newSlotStart, kint newSlotCount, kint free)
        {
            freeList = free;
            var span = GetSpan();
            for (kint i = (kint)(newSlotStart + newSlotCount - 1); i >= newSlotStart; i--)
            {
                PushFree(span, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushFree(Span<Slot> slots, kint islot)
        {
            slots[islot].Clear();
            slots[islot].next = freeList;
            slots[islot].prev = INVALID_SLOT;
            if (freeList != INVALID_SLOT)
            { slots[freeList].prev = islot; }
            freeList = islot;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public kint PopFree(Span<Slot> slots, kint free)
        {
            var prev = slots[free].prev;
            var next = slots[free].next;
            if (prev != INVALID_SLOT)
            { slots[prev].next = next; }
            if (next != INVALID_SLOT)
            { slots[next].prev = prev; }

            if (freeList == free)
            { freeList = next; }

            slots[free].SetUsed();
            return free;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Find(kint key, out vint value)
        {
            return Find(key, out value, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Find(kint key, out vint value, out kint location)
        {
            var slots = GetReadonlySpan();
            var islot = (kint)((key & MAX_SIGN_VALUE) % capacity);
            //var islot = (int)(((int)key[HASH_INDEX] & SLOT_MASK) % capacity);
            var current = islot;
            for (; ; )
            {
                if (slots[current].IsEmpty())
                {
                    value = 0; location = islot;
                    return false;
                }
                if (slots[current].key == key)
                {
                    value = slots[current].value;
                    location = current;
                    return true;
                }
                current = (kint)(slots[current].next & SLOT_MASK);
                if (current == TAIL_SLOT)
                {
                    value = 0; location = islot;
                    return false;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(kint key, vint value)
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
                slots[freeOld].next = (kint)(slots[current].next & SLOT_MASK);
                slots[current].next = (kint)(freeOld + SET_HEAD_MASK);
                return true;
            }
            else
            {
                var freeOld = PopFree(slots, freeList);
                var prev = Prev(slots, islot);
                slots[prev].next = (kint)(slots[prev].IsHead() ? (kint)(freeOld + SET_HEAD_MASK) : freeOld);

                slots[freeOld] = slots[islot];

                slots[islot].next = HEAD_ONLY_SLOT;
                slots[islot].key = key;
                slots[islot].value = value;
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Delete(kint key, out vint value)
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
                    var next = (kint)(slots[islot].next & SLOT_MASK);
                    slots[islot] = slots[next];
                    slots[islot].next = (kint)(slots[next].next + SET_HEAD_MASK);

                    PushFree(slots, next);
                }
            }
            else
            {
                var prev = Prev(slots, islot);
                var next = slots[islot].next;
                next = (kint)(slots[prev].IsHead() ? next + SET_HEAD_MASK : next);
                slots[prev].next = next;
                PushFree(slots, islot);
            }

            return true;
        }

        public void Clear()
        {
            count = 0;
            BakeFreeList(0, capacity, -1);
            freeList = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Resize()
        {
            var slots = GetSpan();
            var oldCapacity = capacity;
            var oldCount = count;
            var oldFreeList = freeList;

            var newCapacity = (kint)HashHelpers.GetPrime(capacity + capacity / 2);
            newCapacity = Math.Min(MAX_CAPACITY, newCapacity);
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
        public bool Resize(byte[] newbuffer, int istart, int boundaryLength)
        {
            var newCapacity = (kint)((boundaryLength - HEAD_SIZE) / SLOT_SIZE);
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

            BakeFreeList(capacity, (kint)(newCapacity - capacity), freeList);
            capacity = newCapacity;

            var slots = GetSpan();
            for (int i = 0; i < capacity; i++)
            {
                if (slots[i].IsEmpty())
                { continue; }
                slots[i].next = HEAD_ONLY_SLOT;
            }

            for (kint i = 0; i < capacity;)
            {
                if (slots[i].IsEmpty())
                { i++; continue; }

                var inewSlot = (kint)((slots[i].key & MAX_SIGN_VALUE) % newCapacity);
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
                    var inewSlotKeySlot = (slots[inewSlot].key & MAX_SIGN_VALUE) % newCapacity;
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
                            slots[i].next = (kint)(slots[inewSlot].next & SLOT_MASK);
                        }
                        else
                        {
                            slots[i].next = TAIL_SLOT;
                        }
                        slots[inewSlot].next =(kint)(i + SET_HEAD_MASK);
                        i++;

                        continue;
                    }
                }
                else
                {
                    var prev = Prev(slots, inewSlot);
                    slots[prev].next = (kint)(slots[prev].IsHead() ? i + SET_HEAD_MASK : i);

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
        public kint Prev(Span<Slot> slots, kint islot)
        {
            var head = (kint)((slots[islot].key & MAX_SIGN_VALUE) % capacity);
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
        private bool TryAddCount(kint num)
        {
            int newvalue = System.Math.Max(0, count + num);
            if (newvalue > capacity)
            { return false; }

            count = (kint)(count + num);
            return true;
        }


        public int NextResizeMemoryLength()
        {
            int newCapacity = (int)HashHelpers.GetPrime(capacity + capacity / 4);
            if (newCapacity > MAX_CAPACITY)
            { newCapacity = MAX_CAPACITY; }
            return newCapacity;
        }

        //public void Optimise()
        //{
        //    var slots = GetSpan();
        //    var optimiseCount = 0;
        //    for (kint i = 0; i < capacity; i++)
        //    {
        //        if (slots[i].IsEmpty() || !slots[i].IsHead())
        //        { continue; }

        //        var current = i;
        //        var lastk = i;
        //        for (; ; )
        //        {
        //            var next = (kint)(slots[current].next & SLOT_MASK);
        //            if (next == TAIL_SLOT)
        //            { break; }
        //            if (next - i > 8)
        //            {
        //                for (kint k = lastk; k < lastk + 8; k++)
        //                {
        //                    if (k > slots.Length - 1)
        //                    { break; }
        //                    if (slots[k].IsHead())
        //                    { continue; }
        //                    if (slots[k].IsEmpty())
        //                    {
        //                        PopFree(slots, k);
        //                        slots[k] = slots[next];
        //                        slots[current].next = (kint)(slots[current].IsHead() ? k + SET_HEAD_MASK : k);
        //                        lastk = k;
        //                        optimiseCount++;
        //                        break;
        //                    }
        //                    var kKeySlot = (slots[k].key & MAX_SIGN_VALUE) % capacity;
        //                    if (Math.Abs(kKeySlot - k) < 8)
        //                    { continue; }

        //                    var kprev = Prev(slots, k);
        //                    var tmp = slots[next];
        //                    slots[next] = slots[k];
        //                    slots[k] = tmp;
        //                    slots[current].next = (kint)(slots[current].IsHead() ? k + SET_HEAD_MASK : k);
        //                    slots[kprev].next = (kint)(slots[kprev].IsHead() ? next + SET_HEAD_MASK : next);
        //                    optimiseCount++;
        //                    break;
        //                }
        //            }
        //            current = next;
        //        }
        //    }
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValueEnumerator EveryKeyValue()
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

            public KeyValuePair<kint, vint> Current
            {
                get
                {
                    return new KeyValuePair<kint, vint>(container[islot].key, container[islot].value);
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

        #region HELPER FUNCTION

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static kint ReadKInt(byte[] buffer, long istart)
        {
            switch (sizeof(kint))
            {
                case 4:
                    return (kint)BitConverter.ToInt32(buffer, (int)istart);
                case 2:
                    return (kint)BitConverter.ToInt16(buffer, (int)istart);
                case 1:
                    return (kint)buffer[istart];
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteKInt(byte[] buffer, long istart, kint value)
        {
            switch (sizeof(kint))
            {
                case 4:
                    BitConverter.TryWriteBytes(new Span<byte>(buffer, (int)istart, 4), value); break;
                case 2:
                    BitConverter.TryWriteBytes(new Span<byte>(buffer, (int)istart, 2), value); break;
                case 1:
                    buffer[istart] = (byte)value; break;
            }
        }
        #endregion

        [StructLayout(LayoutKind.Explicit, Pack = sizeof(byte))]
        public struct Slot
        {
            [FieldOffset(0)] public kint next;
            [FieldOffset(KEY_SIZE)] public kint key;
            [FieldOffset(KEY_SIZE*2)] public vint value;

            [FieldOffset(KEY_SIZE*2)] public kint prev;

            [FieldOffset(KEY_SIZE * 2 + VALUE_SIZE)] public byte flag;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsHead() { return next < 0; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Clear() { flag = byte.MaxValue; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsEmpty() { return flag == byte.MaxValue; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetUsed() { flag = 1; }
        }
    }
}

