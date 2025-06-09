//------------------------------------------------------------//
// yanfei 2024.10.27
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuickFinder
{
    public static class MathX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextPowerOfTwo(int value)
        {
            value--;
            value |= value >> 16;
            value |= value >> 8;
            value |= value >> 4;
            value |= value >> 2;
            value |= value >> 1;
            return value + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClosestPowerOfTwo(int value)
        {
            int num = NextPowerOfTwo(value);
            int num2 = num >> 1;
            if (value - num2 < num - value)
            {
                return num2;
            }

            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinaryLength(uint value)
        {
            int c = 0;
            while (value > 0)
            {
                value = value >> 1;
                c++;
            }
            return c;
        }
    }

    public static class AlgorithmX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSortInt(byte[] bytes, int left, int intCount)
        {
            var span = new Span<byte>(bytes, (int)left, (int)intCount * 4);
            var nums = MemoryMarshal.Cast<byte, int>(span);
            QuickSort(nums, (int)0, (int)intCount - 1);
        }

        //I'm lazy. Here come from good blog https://www.hello-algo.com/chapter_sorting/quick_sort/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort(Span<int> nums, int left, int right)
        {

            [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
            static int Partition(Span<int> nums, int left, int right)
            {

                [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
                static void Swap(Span<int> nums, int i, int j)
                {
                    (nums[j], nums[i]) = (nums[i], nums[j]);
                }


                int i = left, j = right;
                while (i < j)
                {
                    while (i < j && nums[j] >= nums[left])
                        j--;          
                    while (i < j && nums[i] <= nums[left])
                        i++; 
                    Swap(nums, i, j); 
                }
                Swap(nums, i, left); 
                return i;   
            }

            if (left >= right)
                return;

            int pivot = Partition(nums, left, right);

            QuickSort(nums, left, pivot - 1);
            QuickSort(nums, pivot + 1, right);
        }

        //I'm lazy. Here come from good blog https://www.hello-algo.com/chapter_sorting/quick_sort/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort(int[] nums, int left, int right)
        {

            [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
            static int Partition(int[] nums, int left, int right)
            {

                [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
                static void Swap(int[] nums, int i, int j)
                {
                    (nums[j], nums[i]) = (nums[i], nums[j]);
                }


                int i = left, j = right;
                while (i < j)
                {
                    while (i < j && nums[j] >= nums[left])
                        j--;
                    while (i < j && nums[i] <= nums[left])
                        i++;
                    Swap(nums, i, j);
                }
                Swap(nums, i, left);
                return i;
            }


            if (left >= right)
                return;

            int pivot = Partition(nums, left, right);

            QuickSort(nums, left, pivot - 1);
            QuickSort(nums, pivot + 1, right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearchInt(byte[] bytes, long spanStart, long intCount, int value)
        {
            var span = new Span<byte>(bytes, (int)spanStart, (int)intCount * 4);
            var nums = MemoryMarshal.Cast<byte, int>(span);
            return BinarySearch(nums, 0, (int)intCount - 1, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch(Span<int> nums, int left, int right, int value)
        {
            int mid;
            while (left <= right)
            {
                mid = left + (right - left) / 2;
                if (nums[mid] > value)
                {
                    right = mid - 1;
                }
                else if (nums[mid] < value)
                {
                    left = mid + 1;
                }
                else
                {
                    return mid;
                }
            }
            return -1;
        }
    }
}