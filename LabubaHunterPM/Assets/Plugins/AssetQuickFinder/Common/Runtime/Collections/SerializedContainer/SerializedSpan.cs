//------------------------------------------------------------//
// yanfei 2024.10.27
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace QuickFinder.Container
{
    public ref struct SerializedSpan
    {
        byte[]? bytes;
        int start;
        int address;

        public static SerializedSpan FAIL { get { return new SerializedSpan(null, -1, -1); } }
        public static SerializedSpan FULL { get { return new SerializedSpan(null, -2, -2); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializedSpan(byte[]? bytes, int start, int address)
        {
            this.bytes = bytes; this.start = start; this.address = address;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Address() { return address; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Start() { return start; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length() { return BitConverter.ToInt32(bytes, start - SerializedPool.SLOT_HEAD_SIZE); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Valid() { return address > -1 && start > -1; }


        public SerializedPool.ErrorCode ErrorCode()
        {
            if (start == -1 && address == -1) { return SerializedPool.ErrorCode.Fail; }
            if (start == -2 && address == -2) { return SerializedPool.ErrorCode.Full; }
            return SerializedPool.ErrorCode.OK;
        }

        public byte this[int index]
        {
            get { return bytes[start + index]; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BlockCopy(SerializedSpan src, int srcStart, SerializedSpan dst, int dstStart, int length_)
        {
            Buffer.BlockCopy(src.bytes, src.start + srcStart, dst.bytes, dst.start + dstStart, length_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Memset(byte m = 0)
        {
            for (int i = start; i < Length(); i++)
            {
                bytes[i] = m;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool StartWith(string str, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (bytes[start + i] != str[i])
                { return false; }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString(int spanStart, int n)
        {
            var str = System.Text.Encoding.UTF8.GetString(bytes, start + spanStart, n);
            return str;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSameString(int spanStart, string str, int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (bytes[start + spanStart + i] != (byte)str[i])
                { return false; }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(int spanStart, string str, int n)
        {
            //if(spanStart + n > Length())
            //{ throw new IndexOutOfRangeException(); }
            for (int i = 0; i < n; i++)
            {
                bytes[start + spanStart + i] = (byte)str[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt(int spanStart, int value)
        {
            //if(spanStart + 4 > Length())
            //{ throw new IndexOutOfRangeException(); }
            BitConverter.TryWriteBytes(new Span<byte>(bytes, start + spanStart, 4), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt(int spanStart)
        {
            return BitConverter.ToInt32(bytes, start + spanStart);
        }

        #region ISerializedSet
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BuildSet<IntegerKey>(ISerializedSet<IntegerKey> set, int setStart)
        {
            set.Init(bytes, start + setStart, Length() - setStart, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LoadSet<IntegerKey>(ISerializedSet<IntegerKey> set, int setStart)
        {
            set.Load(bytes, start + setStart);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeSet<IntegerKey>(ISerializedSet<IntegerKey> set, int setStart)
        {
            set.Load(bytes, start + setStart);
            set.Resize(bytes, start + setStart, Length() - setStart);
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void QuickSortInt(int spanStart, int count)
        {
            AlgorithmX.QuickSortInt(bytes, start + spanStart, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int BinarySearchInt(int spanStart, int count, int value)
        {
            return AlgorithmX.BinarySearchInt(bytes, start + spanStart, count, value);
        }

        public bool HasInt(int spanStart, int value)
        {
            int count = (Length() - spanStart) / 4;
            count = Math.Max(0, count);
            int rawStart = start + spanStart;
            for (int i = 0; i < count; i++)
            {
                var v = BitConverter.ToInt32(bytes, rawStart + i * 4);
                if (v == value)
                { return true; }
            }
            return false;
        }

        /// <summary>
        /// if target slot less then compareValue, replace it with value
        /// </summary>
        /// <param name="spanStart"></param>
        /// <param name="replaceValue"></param>
        /// <param name="compareValue"></param>
        /// <returns></returns>
        public bool ReplaceIfLessThan(int spanStart, int replaceValue, int compareValue)
        {
            int count = (Length() - spanStart) / 4;
            count = Math.Max(0, count);
            int rawStart = start + spanStart;
            for (int i = 0; i < count; i++)
            {
                int rawIndex = rawStart + i * 4;
                var v = ReadInt(rawIndex);
                if (v < compareValue)
                {
                    WriteInt(rawIndex, replaceValue);
                    return true;
                }
            }
            return false;
        }
    }

    //public ref struct SerializedSpan
    //{
    //    byte[]? bytes;
    //    int start;
    //    int address;

    //    public static SerializedSpan FAIL { get { return new SerializedSpan(null, -1, -1); } }
    //    public static SerializedSpan FULL { get { return new SerializedSpan(null, -2, -2); } }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public SerializedSpan(byte[]? bytes, int start, int address)
    //    {
    //        this.bytes = bytes; this.start = start; this.address = address;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public int Address() { return address; }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public int Start() { return start; }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public int Length() { return BitConverter.ToInt32(bytes, start - SerializedPool.SLOT_HEAD_SIZE); }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool Valid() { return address > -1 && start > -1; }


    //    public SerializedPool.ErrorCode ErrorCode()
    //    {
    //        if (start == -1 && address == -1) { return SerializedPool.ErrorCode.Fail; }
    //        if (start == -2 && address == -2) { return SerializedPool.ErrorCode.Full; }
    //        return SerializedPool.ErrorCode.OK;
    //    }

    //    public byte this[int index]
    //    {
    //        get { return bytes[start + index]; }
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static void BlockCopy(SerializedSpan src, int srcStart, SerializedSpan dst, int dstStart, int length_)
    //    {
    //        Buffer.BlockCopy(src.bytes, src.start + srcStart, dst.bytes, dst.start + dstStart, length_);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void Memset(byte m = 0)
    //    {
    //        for (int i = start; i < Length(); i++)
    //        {
    //            bytes[i] = m;
    //        }
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool StartWith(string str, int count)
    //    {
    //        for (int i = 0; i < count; i++)
    //        {
    //            if (bytes[start + i] != str[i])
    //            { return false; }
    //        }
    //        return true;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public string ReadString(int spanStart, int n)
    //    {
    //        var str = System.Text.Encoding.UTF8.GetString(bytes, start + spanStart, n);
    //        return str;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public bool IsSameString(int spanStart, string str, int n)
    //    {
    //        for (int i = 0; i < n; i++)
    //        {
    //            if (bytes[start + spanStart + i] != (byte)str[i])
    //            { return false; }
    //        }
    //        return true;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void WriteString(int spanStart, string str, int n)
    //    {
    //        //if(spanStart + n > Length())
    //        //{ throw new IndexOutOfRangeException(); }
    //        for (int i = 0; i < n; i++)
    //        {
    //            bytes[start + spanStart + i] = (byte)str[i];
    //        }
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void WriteInt(int spanStart, int value)
    //    {
    //        //if(spanStart + 4 > Length())
    //        //{ throw new IndexOutOfRangeException(); }
    //        BitConverter.TryWriteBytes(new Span<byte>(bytes, start + spanStart, 4), value);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public int ReadInt(int spanStart)
    //    {
    //        return BitConverter.ToInt32(bytes, start + spanStart);
    //    }

    //    #region ISerializedSet
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void BuildSet<IntegerKey>(ISerializedSet<IntegerKey> set, int setStart)
    //    {
    //        set.Init(bytes, start + setStart, Length() - setStart, 0);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void LoadSet<IntegerKey>(ISerializedSet<IntegerKey> set, int setStart)
    //    {
    //        set.Load(bytes, start + setStart);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void ResizeSet<IntegerKey>(ISerializedSet<IntegerKey> set, int setStart)
    //    {
    //        set.Load(bytes, start + setStart);
    //        set.Resize(bytes, start + setStart, Length() - setStart);
    //    }
    //    #endregion

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void QuickSortInt(int spanStart, int count)
    //    {
    //        AlgorithmX.QuickSortInt(bytes, start + spanStart, count);
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public int BinarySearchInt(int spanStart, int count, int value)
    //    {
    //        return AlgorithmX.BinarySearchInt(bytes, start + spanStart, count, value);
    //    }

    //    public bool HasInt(int spanStart, int value)
    //    {
    //        int count = (Length() - spanStart) / 4;
    //        count = Math.Max(0, count);
    //        int rawStart = start + spanStart;
    //        for (int i = 0; i < count; i++)
    //        {
    //            var v = BitConverter.ToInt32(bytes, rawStart + i * 4);
    //            if (v == value)
    //            { return true; }
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// if target slot less then compareValue, replace it with value
    //    /// </summary>
    //    /// <param name="spanStart"></param>
    //    /// <param name="replaceValue"></param>
    //    /// <param name="compareValue"></param>
    //    /// <returns></returns>
    //    public bool ReplaceIfLessThan(int spanStart, int replaceValue, int compareValue)
    //    {
    //        int count = (Length() - spanStart) / 4;
    //        count = Math.Max(0, count);
    //        int rawStart = start + spanStart;
    //        for (int i = 0; i < count; i++)
    //        {
    //            int rawIndex = rawStart + i * 4;
    //            var v = ReadInt(rawIndex);
    //            if (v < compareValue)
    //            {
    //                WriteInt(rawIndex, replaceValue);
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //}

}

