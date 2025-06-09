//------------------------------------------------------------//
// yanfei 2024.10.27
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuickFinder.Container
{
    [StructLayout(LayoutKind.Sequential, Pack = sizeof(ulong))]
    public struct GuidStruct
    {
        public const int HASH_INDEX = 2;

        public ulong g1;
        public ulong g2;
        public ulong g3;
        public ulong g4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty() { return g1 == ulong.MaxValue && g2 == ulong.MaxValue && g3 == ulong.MaxValue && g4 == ulong.MaxValue; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() { g1 = ulong.MaxValue; g2 = ulong.MaxValue; g3 = ulong.MaxValue; g4 = ulong.MaxValue; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsZero() { return g1 == 0 && g2 == 0 && g3 == 0 && g4 == 0; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSameKey(ReadOnlySpan<ulong> key)
        {
            return key[0] == g1 && key[1] == g2 && key[2] == g3 && key[3] == g4;
        }

        public bool IsSameKey(GuidStruct other)
        {
            return g1 == other.g1 && g2 == other.g2 && g3 == other.g3 && g4 == other.g4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSameKey(ulong key1, ulong key2, ulong key3, ulong key4)
        {
            return g1 == key1 && g2 == key2 && g3 == key3 && g4 == key4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetKey(GuidStruct key)
        {
            g1 = key.g1; g2 = key.g2; g3 = key.g3; g4 = key.g4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetKey(ReadOnlySpan<ulong> key)
        {
            g1 = key[0]; g2 = key[1]; g3 = key[2]; g4 = key[3];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetKey(ulong key1, ulong key2, ulong key3, ulong key4)
        {
            g1 = key1; g2 = key2; g3 = key3; g4 = key4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong KeyHash()
        {
            if (HASH_INDEX == 0) { return g1; }
            if (HASH_INDEX == 1) { return g2; }
            if (HASH_INDEX == 2) { return g3; }
            if (HASH_INDEX == 3) { return g4; }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSameGuid(string guid)
        {
            Span<byte> span = stackalloc byte[32];
            for (int i = 0; i < 32; i++)
            { span[i] = (byte)guid[i]; }
            var ulongSpan = MemoryMarshal.Cast<byte, ulong>(span);
            return g1 == ulongSpan[0] && g2 == ulongSpan[1] && g3 == ulongSpan[2] && g4 == ulongSpan[3];
        }

        public string GetGuid()
        {
            var longs = new Span<ulong>(new ulong[] { g1, g2, g3, g4 });
            var bytes = MemoryMarshal.Cast<ulong, byte>(longs);
            var guid = System.Text.Encoding.UTF8.GetString(bytes);
            return guid;
        }

    }

    public enum ContainerMode : byte
    {
        AutoResize = 1,
        MemoryEasing = 2,
    }

    public struct ResizeParams
    {
        public byte[] buffer;
        public int start;
        public int lengh;
    }

    public static class Utility
    {
        public static uint MakeMode(params ContainerMode[] modes)
        {
            uint mode = 0;
            for (int i = 0; i < modes.Length; i++)
            {
                mode = mode | (uint)modes[i];
            }
            return mode;
        }

        //djb2 long version(two int version combine)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong KeyToHash(byte[] key, long istart, byte n)
        {
            return HashHelpers.DJB2(key, istart, n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long KeyToNumber(byte[] key, long istart, byte n)
        {
            long r = 0; byte s = 0;
            for (byte i = 0; i < n; i++)
            {
                var d = key[istart + s + n - i - 1];
                if (d >= 48 && d <= 57) { r += (long)((d - 48) * (1 << (i << 2))); }
                else { r += (long)((d - 87) * (1 << (i << 2))); }
                //else if (d >= 97 && d <= 102) { r += (d - 87) * (1 << (i << 2)); }
                //else { r += (d - 55) * (1 << (i << 2)); }
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long KeyToNumber(string key, int istart, byte n)
        {
            long r = 0; byte s = 0;
            for (byte i = 0; i < n; i++)
            {
                var d = key[istart + s + n - i - 1];
                if (d >= 48 && d <= 57) { r += (long)((d - 48) * (1 << (i << 2))); }
                else { r += (long)((d - 87) * (1 << (i << 2))); }
                //else if (d >= 97 && d <= 102) { r += (d - 87) * (1 << (i << 2)); }
                //else { r += (d - 55) * (1 << (i << 2)); }
            }
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBool(byte[] buffer, long istart)
        {
            return buffer[istart] > 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(byte[] buffer, long istart)
        {
            return buffer[istart];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShort(byte[] buffer, long istart)
        {
#if SERIALIZED_CONTAINER_BUFFER_LONG
            return (short)((0xff & buffer[istart]) | (0xff & buffer[istart + 1]) << 8);
#else
            return BitConverter.ToInt16(buffer, (int)istart);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt(byte[] buffer, long istart)
        {
#if SERIALIZED_CONTAINER_BUFFER_LONG
            return (0xff & buffer[istart]) | (0xff & buffer[istart + 1]) << 8 | (0xff & buffer[istart + 2]) << 16 | buffer[istart + 3] << 24;
#else
            return BitConverter.ToInt32(buffer, (int)istart);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLong(byte[] buffer, long istart)
        {
#if SERIALIZED_CONTAINER_BUFFER_LONG
            return ((0xff & buffer[istart]) | (0xff & buffer[istart + 1]) << 8 | (0xff & buffer[istart + 2]) << 16 | (0xff & buffer[istart + 3]) << 24
                  | (0xff & buffer[istart + 4]) << 32 | (0xff & buffer[istart + 5]) << 40 | (0xff & buffer[istart + 6]) << 48 | (0xff & buffer[istart + 7]) << 56);
#else
            return BitConverter.ToInt64(buffer, (int)istart);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadXInt(byte[] buffer, long istart, int valueSize)
        {
            switch (valueSize)
            {
                case 4:
                    return (long)ReadInt(buffer, istart);
                case 2:
                    return (long)ReadShort(buffer, istart);
                case 1:
                    return (long)ReadByte(buffer, istart);
                case 8:
                    return (long)ReadLong(buffer, istart);
                default:
                    return (long)ReadInt(buffer, istart);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBool(byte[] buffer, long istart, bool value)
        {
            buffer[istart] = value ? (byte)1 : (byte)0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteByte(byte[] buffer, long istart, byte value)
        {
            buffer[istart] = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteShort(byte[] buffer, long istart, short value)
        {
#if SERIALIZED_CONTAINER_BUFFER_LONG
            buffer[istart] = (byte)value; buffer[istart + 1] = (byte)(value >> 8);
#else
            BitConverter.TryWriteBytes(new Span<byte>(buffer, (int)istart, 2), value);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt(byte[] buffer, long istart, int value)
        {
#if SERIALIZED_CONTAINER_BUFFER_LONG
            buffer[istart] = (byte)value; buffer[istart + 1] = (byte)(value >> 8); buffer[istart + 2] = (byte)(value >> 16); buffer[istart + 3] = (byte)(value >> 24);
#else
            BitConverter.TryWriteBytes(new Span<byte>(buffer, (int)istart, 4), value);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLong(byte[] buffer, long istart, long value)
        {
            buffer[istart] = (byte)value; buffer[istart + 1] = (byte)(value >> 8); buffer[istart + 2] = (byte)(value >> 16); buffer[istart + 3] = (byte)(value >> 24);
            buffer[istart + 4] = (byte)(value >> 32); buffer[istart + 5] = (byte)(value >> 40); buffer[istart + 6] = (byte)(value >> 48); buffer[istart + 7] = (byte)(value >> 56);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteXInt(byte[] buffer, long istart, long value, int valueSize)
        {
            switch (valueSize)
            {
                case 4:
                    WriteInt(buffer, istart, (int)value); break;
                case 2:
                    WriteShort(buffer, istart, (short)value); break;
                case 1:
                    WriteByte(buffer, istart, (byte)value); break;
                case 8:
                    WriteLong(buffer, istart, (long)value); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyToBuffer(byte[] src, long srcStart, byte[] dst, long dstStart, long length)
        {
#if SERIALIZED_CONTAINER_BUFFER_LONG
            for (long i = length - 1; i >= 0; i--)//from tail to head invoid overriding
            {
                dst[dstStart + i] = src[srcStart + i];
            }
            
#else
            Buffer.BlockCopy(src, (int)srcStart, dst, (int)dstStart, (int)length);
#endif
        }
    }

    public static class HashHelpers
    {
        // Some primes between 2^63 and 2^64 for various uses.
        const ulong K0 = 0xc3a5c85c97cb3127;
        const ulong K1 = 0xb492b66fbe98f273;
        const ulong K2 = 0x9ae16a3b2f90404f;

        // Magic numbers for 32-bit hashing.  Copied from Murmur3.
        const uint C1 = 0xcc9e2d51;
        const uint C2 = 0x1b873593;

        // A 32-bit to 32-bit integer hash copied from Murmur3.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MurMur3_Mix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        // A 32-bit to 32-bit integer hash copied from Murmur3.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MurMur3_Mix(ulong h)
        {
            h ^= h >> 32;
            h *= K0;
            h ^= h >> 26;
            h *= K1;
            h ^= h >> 32;
            return h;
        }

        //djb2 long version(two int version combine)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong KeyToHash(byte[] key, long istart, byte n)
        {
            return DJB2(key, istart, n);
        }

        //djb2 long version(two int version combine)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong DJB2(byte[] key, long istart, byte n)
        {
            ulong r = 5381;
            ulong m = 52711;
            for (byte i = 0; i < n; i++)
            {
                r = r * 31 + ((ulong)key[istart + i] << 23);
                m = m * 31 + key[istart + i] + r;
            }
            r = ((r + m) << 31) + m;
            return r * 231 + K0;
        }

        #region come from microsoft .net reference source
        // Table of prime numbers to use as hash table sizes. 
        // A typical resize algorithm would pick the smallest prime number in this array
        // that is larger than twice the previous capacity. 
        // Suppose our Hashtable currently has capacity x and enough elements are added 
        // such that a resize needs to occur. Resizing first computes 2x then finds the 
        // first prime in the table greater than 2x, i.e. if primes are ordered 
        // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n. 
        // Doubling is important for preserving the asymptotic complexity of the 
        // hashtable operations such as add.  Having a prime guarantees that double 
        // hashing does not lead to infinite loops.  IE, your hash function will be 
        // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
        public static readonly long[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369, //end of microsoft origin
            8388617, 12582967, 16777259,25165897, 33554467, 50331683, 67108879, 100663319, 134217757, 201326611, 268435459,
            402653201, 536870923, 805306457, 1073741827, 1610612741, 2147483647, //end of int
            3221225473, 4294967291,6442450967, 8589934609, 12884901877, 17179869143, 25769803799, 34359738421, 51539607599,
            68719476767, 103079215153, 137438953481, 206158430227,  274877906951, 412316860441, 549755813911,774877906997, 999999999989
            };


        public static bool IsPrime(long candidate)
        {
            if ((candidate & 1) != 0)
            {
                long limit = (long)Math.Sqrt(candidate);
                for (long divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }
                return true;
            }
            return (candidate == 2);
        }

        public static long GetPrime(long min)
        {
            for (long i = 0; i < primes.Length; i++)
            {
                long prime = primes[i];
                if (prime >= min) return prime;
            }

            return min;
        }

        public static long GetMinPrime()
        {
            return primes[0];
        }
        #endregion
    }
}

