//------------------------------------------------------------//
// yanfei 2024.11.28
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System;

namespace QuickFinder.Container
{
    public interface ISerializedContainer<IntegerKey, IntegerValue>
    {
        public bool Init(IntegerKey estimateCapacity, uint mode_);

        public bool Init(byte[] buffer, int istart, int boundaryLength, uint mode_);

        public bool Load(byte[] buffer, int istart);

        public void MoveTo(byte[] buffer, int istart);

        public IntegerKey Capacity();

        public IntegerKey Count();

        public bool Resize(byte[] newbuffer, int istart, int boundaryLength);
    }

    public interface ISerializedSet<IntegerKey> : ISerializedContainer<IntegerKey, IntegerKey>
    {
        public bool Add(IntegerKey key);

        public bool Find(IntegerKey key, out IntegerKey location);

        public bool Delete(IntegerKey key);
    }

    public interface ISerializedMap<IntegerKey, IntegerValue> : ISerializedContainer<IntegerKey, IntegerValue>
    {
        public bool Add(IntegerKey key, IntegerValue value);

        public bool Find(IntegerKey key, out IntegerValue value, out IntegerKey location);

        public bool Delete(IntegerKey key, out IntegerValue value);
    }

    public interface ISerializedBucket<IntegerKey, IntegerValue> : ISerializedContainer<IntegerKey, IntegerValue>
    {
        public bool Add(IntegerKey key, IntegerValue value);

        public bool Find(IntegerKey key, IntegerValue value, out IntegerKey location);

        public bool Find(IntegerKey key, out IntegerKey headLocation);

        public IntegerKey IterateFrom(IntegerKey location, out IntegerValue value);

        public bool Delete(IntegerKey key, IntegerValue value);

        public bool Delete(IntegerKey key);

        public void Every(Action<IntegerKey, IntegerValue> action);
    }
}
