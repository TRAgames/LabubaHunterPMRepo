//------------------------------------------------------------//
// yanfei 2024.10.27
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

#define QUEICKFINDER_RELATIONSMAP_ADVANCE_VERSION

#if UNITY_2017_1_OR_NEWER
using UnsafeUtility = Unity.Collections.LowLevel.Unsafe.UnsafeUtility;
#else
using UnsafeUtility = System.Runtime.CompilerServices.Unsafe;
#endif

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using QuickFinder.Container;
using System;
using System.Collections.Generic;
using QuickFinder.Container.Serialization;

using System.Linq;


namespace QuickFinder.Assets
{
#if QUEICKFINDER_RELATIONSMAP_ADVANCE_VERSION

    public class SerializedRelationsMap
    {
        const int SHORTKEY_SIZE = 4;
        const int RAW_SEARCH_COUNT = 8;
        const int RAW_SEARCH_DATA_SIZE = SHORTKEY_SIZE * RAW_SEARCH_COUNT;

        SerializedGuid2IntMap guid2IntMap;
        SerializedInt2GuidMap int2GuidMap;
        SerializedInt32Int32Map dependMap;
        SerializedInt32Int32Map ownMap;
        SerializedPool pool;

        SerializedInt32Int32Set relationsSet;
        int guidIndexer = 1;


        public SerializedRelationsMap()
        {
            guid2IntMap = new SerializedGuid2IntMap();
            int2GuidMap = new SerializedInt2GuidMap();
            dependMap = new SerializedInt32Int32Map();
            ownMap = new SerializedInt32Int32Map();
            pool = new SerializedPool();

            relationsSet = new SerializedInt32Int32Set();
        }

        public void Init(int capacity)
        {
            capacity = Math.Max(capacity, 4096);

            guidIndexer = 1;
            guid2IntMap.Init(capacity);
            int2GuidMap.Init(capacity);
            dependMap.Init(capacity);
            ownMap.Init(capacity);
            pool.Init(1024 * 256, SHORTKEY_SIZE);
        }

        public byte[] Save()
        {
            int totalSize = 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependMap.RawLength() + ownMap.RawLength() + pool.ReadRawLength();
            var bytes = new byte[totalSize];

            BitConverter.TryWriteBytes(bytes, guidIndexer);
            guid2IntMap.MoveTo(bytes, 4);
            int2GuidMap.MoveTo(bytes, 4 + guid2IntMap.RawLength());
            dependMap.MoveTo(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength());
            ownMap.MoveTo(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependMap.RawLength());
            pool.MoveTo(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependMap.RawLength() + ownMap.RawLength());

            return bytes;
        }

        public bool Load(byte[] bytes)
        {
            guidIndexer = BitConverter.ToInt32(bytes, 0);

            guid2IntMap.Load(bytes, 4);
            int2GuidMap.Load(bytes, 4 + guid2IntMap.RawLength());
            dependMap.Load(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength());
            ownMap.Load(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependMap.RawLength());
            pool.Load(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependMap.RawLength() + ownMap.RawLength());

            return true;
        }

        public void Prebuild(Dictionary<string, IEnumerable<string>> dependMap_)
        {
            GetOrBakeShortKey(dependMap_.Keys);

            var dependShortMap = new Dictionary<int, List<int>>();
            var ownShortMap = new Dictionary<int, List<int>>();

            foreach (var kvp in dependMap_)
            {
                var host = kvp.Key;
                var dependencies = kvp.Value;

                if (dependencies.Count() == 0)
                { continue; }

                guid2IntMap.Find(host, out var hostId, out _);

                var dependSet = new List<int>();
                dependShortMap[hostId] = dependSet;
                foreach (var dependency in dependencies)
                {
                    guid2IntMap.Find(dependency, out var dependId, out _);
                    dependSet.Add(dependId);

                    if (!ownShortMap.TryGetValue(dependId, out var ownSet))
                    {
                        ownSet = new List<int>();
                        ownShortMap[dependId] = ownSet;
                    }
                    ownSet.Add(hostId);
                }
            }

            SaveMap(pool, relationsSet, dependMap, dependShortMap);
            SaveMap(pool, relationsSet, ownMap, ownShortMap);
        }

        private static void SaveMap(SerializedPool pool, ISerializedSet<int> set, SerializedInt32Int32Map saveMap, Dictionary<int, List<int>> dataMap)
        {
            foreach (var kvp in dataMap)
            {
                var hostId = kvp.Key;
                var relations = kvp.Value;

                var relationsCount = relations.Count();
                if (relationsCount == 0)
                { continue; }

                if (relationsCount <= RAW_SEARCH_COUNT)
                {
                    var writeSpan = pool.CreateBuffer(SHORTKEY_SIZE * relationsCount);
                    writeSpan.Memset(byte.MaxValue);
                    saveMap.Add(hostId, writeSpan.Address());

                    for (int i = 0; i < relationsCount; i++)
                    {
                        var dependency = relations[i];
                        writeSpan.WriteInt(i * SHORTKEY_SIZE, dependency);
                    }
                }
                else
                {
                    int dependRawSize = (int)SerializedInt32Int32Set.MemoryWillAllocatedWithCapacity(relationsCount);
                    var writeSpan = pool.CreateBuffer(dependRawSize);
                    saveMap.Add(hostId, writeSpan.Address());

                    writeSpan.BuildSet(set, 0);
                    for (int i = 0; i < relationsCount; i++)
                    {
                        var dependency = relations[i];
                        set.Add(dependency);
                    }
                }

            }
        }

        public List<int> GetOrBakeShortKey(IEnumerable<string> guids)
        {
            var keys = new List<int>();
            foreach (var host in guids)
            {
                if (!guid2IntMap.Find(host, out int value, out _))
                {
                    guidIndexer = guidIndexer + 1;
                    guid2IntMap.Add(host, guidIndexer);
                    int2GuidMap.Add(guidIndexer, host);
                    value = guidIndexer;
                }
                keys.Add(value);
            }
            return keys;
        }

        public ICollection<int> GetOrBakeShortKey(IEnumerable<string> guids, ICollection<int> keys)
        {
            foreach (var host in guids)
            {
                if (!guid2IntMap.Find(host, out int value, out _))
                {
                    guidIndexer = guidIndexer + 1;
                    guid2IntMap.Add(host, guidIndexer);
                    int2GuidMap.Add(guidIndexer, host);
                    value = guidIndexer;
                }
                keys.Add(value);
            }
            return keys;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOrBakeShortKey(string host)
        {
            if (!guid2IntMap.Find(host, out int value, out _))
            {
                guidIndexer = guidIndexer + 1;
                guid2IntMap.Add(host, guidIndexer);
                int2GuidMap.Add(guidIndexer, host);
                value = guidIndexer;
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetShotKey(string host, out int shortKey)
        {
            return guid2IntMap.Find(host, out shortKey, out _);
        }

        public bool HasDependency(string host, string dependency)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }

            if (!dependMap.Find(hostId, out var address))
            { return false; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return false; }

            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                if (!readSpan.HasInt(0, dependencyId))
                { return false; }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                if (!relationsSet.Find(dependencyId, out _))
                { return false; }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasDependency(string host, string dependency, bool recursive)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }

            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            return HasDependency(hostId, dependencyId, recursive);
        }

        public bool HasDependency(int hostId, int dependencyId)
        {
            if (!dependMap.Find(hostId, out var address))
            { return false; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return false; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                if (!readSpan.HasInt(0, dependencyId))
                { return false; }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                if (!relationsSet.Find(dependencyId, out _))
                { return false; }
            }

            return true;
        }

        public bool HasDependency(int hostId, int dependencyId, bool recursive)
        {
            var searched = HashSetPool<int>.Get();
            var has = HasDependencyInternal(searched, hostId, dependencyId, recursive);
            HashSetPool<int>.Return(searched);
            return has;
        }

        internal bool HasDependencyInternal(ICollection<int> searched, int hostId, int dependencyId, bool recursive)
        {
            searched.Add(hostId);

            if (!dependMap.Find(hostId, out var address))
            { return false; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return false; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                if (readSpan.HasInt(0, dependencyId))
                { return true; }

                if (!recursive)
                { return false; }

                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var savedDependencyId = readSpan.ReadInt(i * 4);
                    if (savedDependencyId < 0)
                    { continue; }

                    if(searched.Contains(savedDependencyId))
                    { continue; }

                    if(HasDependencyInternal(searched, savedDependencyId, dependencyId, recursive))
                    { return true; }
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                if (relationsSet.Find(dependencyId, out _))
                { return true; }

                if (!recursive)
                { return false; }

                foreach (var savedDependencyId in relationsSet.EveryKey())
                {
                    if (searched.Contains(savedDependencyId))
                    { continue; }

                    if (HasDependencyInternal(searched, savedDependencyId, dependencyId, recursive))
                    { return true; }
                }
            }

            return false;
        }

        public void GetDependencies(string host, ICollection<string> dependencies)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return; }

            if (!dependMap.Find(hostId, out var address))
            { return; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var dependencyId = readSpan.ReadInt(i * 4);
                    if (dependencyId < 0)
                    { continue; }
                    if (!int2GuidMap.Find(dependencyId, out var guidStruct))
                    { continue; }
                    dependencies.Add(guidStruct.GetGuid());
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                foreach (var dependencyId in relationsSet.EveryKey())
                {
                    if (!int2GuidMap.Find(dependencyId, out var guidStruct))
                    { continue; }
                    dependencies.Add(guidStruct.GetGuid());
                }
            }
        }

        public void GetDependencies(int hostId, ICollection<int> dependencies)
        {
            if (!dependMap.Find(hostId, out var address))
            { return; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var dependencyId = readSpan.ReadInt(i * 4);
                    if (dependencyId < 0)
                    { continue; }
                    dependencies.Add(dependencyId);
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                foreach (var dependencyId in relationsSet.EveryKey())
                {
                    dependencies.Add(dependencyId);
                }
            }
        }

        public void GetDependencies(string host, ICollection<string> dependencies, bool recursive)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return; }

            var dependKeys = HashSetPool<int>.Get();

            GetDependencies(hostId, dependKeys, recursive);
            foreach (var dependKey in dependKeys)
            {
                if (!int2GuidMap.Find(dependKey, out var guidStruct))
                { continue; }
                dependencies.Add(guidStruct.GetGuid());
            }

            HashSetPool<int>.Return(dependKeys);
        }

        public void GetDependencies(int hostId, ICollection<int> dependencies, bool recursive)
        {
            if (!dependMap.Find(hostId, out var address))
            { return; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var dependencyId = readSpan.ReadInt(i * 4);
                    if (dependencyId < 0)
                    { continue; }

                    if (dependencies.Contains(dependencyId))
                    { continue; }
                    dependencies.Add(dependencyId);

                    if (!recursive)
                    { continue; }

                    GetDependencies(dependencyId, dependencies, recursive);
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                foreach (var dependencyId in relationsSet.EveryKey())
                {
                    if (dependencies.Contains(dependencyId))
                    { continue; }
                    dependencies.Add(dependencyId);
                    if (!recursive)
                    { continue; }

                    GetDependencies(dependencyId, dependencies, recursive);
                }
            }
        }


        public bool HasOwner(string dependency, string own)
        {
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            if (!ownMap.Find(dependencyId, out var address))
            { return false; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return false; }

            if (!guid2IntMap.Find(own, out int ownId, out _))
            { return false; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                if (!readSpan.HasInt(0, ownId))
                { return false; }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                if (!relationsSet.Find(ownId, out _))
                { return false; }
            }

            return true;
        }

        public bool HasOwner(string dependency, string own, bool recursive)
        {
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            if (!guid2IntMap.Find(own, out int ownId, out _))
            { return false; }

            return HasOwner(dependencyId, ownId, recursive);
        }

        public bool HasOwner(int dependencyId, int ownId)
        {
            if (!ownMap.Find(dependencyId, out var address))
            { return false; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return false; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                if (!readSpan.HasInt(0, ownId))
                { return false; }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                if (!relationsSet.Find(ownId, out _))
                { return false; }
            }

            return true;
        }

        public bool HasOwner(int dependencyId, int ownId, bool recursive)
        {
            var searched = HashSetPool<int>.Get();
            var has = HasOwnerInternal(searched, dependencyId, ownId, recursive);
            HashSetPool<int>.Return(searched);
            return has;
        }

        internal bool HasOwnerInternal(ICollection<int> searched, int dependencyId, int ownId, bool recursive)
        {
            searched.Add(dependencyId);

            if (!ownMap.Find(dependencyId, out var address))
            { return false; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return false; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                if (readSpan.HasInt(0, ownId))
                { return true; }

                if(!recursive)
                { return false; }

                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var savedOwnId = readSpan.ReadInt(i * 4);
                    if (savedOwnId < 0)
                    { continue; }

                    if (searched.Contains(savedOwnId))
                    { continue; }

                    if (HasOwnerInternal(searched, savedOwnId, ownId, recursive))
                    { return true; }
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                if (relationsSet.Find(ownId, out _))
                { return true; }

                if(!recursive)
                { return false; }

                foreach (var savedOwnId in relationsSet.EveryKey())
                {
                    if (searched.Contains(savedOwnId))
                    { continue; }

                    if (HasOwnerInternal(searched, savedOwnId, ownId, recursive))
                    { return true; }
                }
            }

            return false;
        }


        public void GetOwners(string dependency, ICollection<string> owners)
        {
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return; }

            if (!ownMap.Find(dependencyId, out var address))
            { return; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var ownId = readSpan.ReadInt(i * 4);
                    if (ownId < 0)
                    { continue; }
                    if (!int2GuidMap.Find(ownId, out var guidStruct))
                    { continue; }
                    owners.Add(guidStruct.GetGuid());
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                foreach (var ownId in relationsSet.EveryKey())
                {
                    if (!int2GuidMap.Find(ownId, out var guidStruct))
                    { continue; }
                    owners.Add(guidStruct.GetGuid());
                }
            }
        }

        public void GetOwners(int dependencyId, ICollection<int> owners)
        {
            if (!ownMap.Find(dependencyId, out var address))
            { return; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var ownId = readSpan.ReadInt(i * 4);
                    if (ownId < 0)
                    { continue; }
                    owners.Add(ownId);
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                foreach (var ownId in relationsSet.EveryKey())
                {
                    owners.Add(ownId);
                }
            }
        }

        public void GetOwners(string dependency, ICollection<string> owners, bool recursive)
        {
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return; }

            var ownerKeys = HashSetPool<int>.Get();
            GetOwners(dependencyId, ownerKeys, recursive);

            foreach (var ownerKey in ownerKeys)
            {
                if (!int2GuidMap.Find(ownerKey, out var guidStruct))
                { continue; }
                owners.Add(guidStruct.GetGuid());
            }

            HashSetPool<int>.Return(ownerKeys);
        }


        public void GetOwners(int dependencyId, ICollection<int> owners, bool recursive)
        {
            if (!ownMap.Find(dependencyId, out var address))
            { return; }

            var readSpan = pool.ReadBuffer(address);
            if (!readSpan.Valid())
            { return; }

            if (readSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                for (int i = 0; i < readSpan.Length() / 4; i++)
                {
                    var ownId = readSpan.ReadInt(i * 4);
                    if (ownId < 0)
                    { continue; }

                    if (owners.Contains(ownId))
                    { continue; }
                    owners.Add(ownId);

                    if (!recursive)
                    { continue; }
                    GetOwners(ownId, owners, recursive);
                }
            }
            else
            {
                readSpan.LoadSet(relationsSet, 0);
                foreach (var ownId in relationsSet.EveryKey())
                {
                    if (owners.Contains(ownId))
                    { continue; }
                    owners.Add(ownId);

                    if (!recursive)
                    { continue; }
                    GetOwners(ownId, owners, recursive);
                }
            }
        }

        public bool AddDependencies(int hostId, ICollection<int> dependencies)
        {
            void CreateNewSpan(int hostId)
            {
                if(dependencies.Count() <= 0)
                { return; }
                if (dependencies.Count() <= RAW_SEARCH_COUNT)
                {
                    var writeSpan = pool.CreateBuffer(SHORTKEY_SIZE * dependencies.Count());
                    writeSpan.Memset(byte.MaxValue);
                    dependMap.Add(hostId, writeSpan.Address());

                    int idepend = 0;
                    foreach (var dependency in dependencies)
                    {
                        writeSpan.WriteInt(idepend * SHORTKEY_SIZE, dependency);
                        idepend++;
                    }
                }
                else
                {
                    int dependRawSize = (int)SerializedInt32Int32Set.MemoryWillAllocatedWithCapacity(dependencies.Count());
                    var writeSpan = pool.CreateBuffer(dependRawSize);
                    dependMap.Add(hostId, writeSpan.Address());

                    writeSpan.BuildSet(relationsSet, 0);
                    foreach (var dependency in dependencies)
                    {
                        relationsSet.Add(dependency);
                    }
                }
            }

            if(! dependMap.Find(hostId, out var hostAddress))
            {
                CreateNewSpan(hostId);
                goto SERIALIZEDRELATIONSMAP_AddDependencies;
            }

            int newSize = 0;
            var dependSpan = pool.ReadBuffer(hostAddress);
            if(dependSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                newSize = dependencies.Count() * 4;
                if(newSize <= dependSpan.Length())
                {
                    dependSpan.Memset(byte.MaxValue);

                    int idepend = 0;
                    foreach (var dependency in dependencies)
                    {
                        dependSpan.WriteInt(idepend * SHORTKEY_SIZE, dependency);
                        idepend++;
                    }
                    goto SERIALIZEDRELATIONSMAP_AddDependencies;
                }
            }
            else
            {
                dependSpan.LoadSet(relationsSet, 0);
                newSize = (int)SerializedInt32Int32Set.MemoryWillAllocatedWithCapacity(dependencies.Count());
                if(newSize < dependSpan.Length())
                {
                    dependSpan.BuildSet(relationsSet, 0);
                    foreach (var dependency in dependencies)
                    {
                        relationsSet.Add(dependency);
                    }
                    goto SERIALIZEDRELATIONSMAP_AddDependencies;
                }
            }

            pool.FreeBuffer(hostAddress);
            dependMap.Delete(hostId, out _);
            CreateNewSpan(hostId);

SERIALIZEDRELATIONSMAP_AddDependencies:

            foreach (var dependency in dependencies)
            {
                AddRelation(ownMap, dependency, hostId);
            }

            return true;
        }

        private bool AddRelation(SerializedInt32Int32Map saveMap, int hostId, int targetRelationId)
        {
            SerializedSpan relationSpan;
            if (!saveMap.Find(hostId, out var address))
            {
                relationSpan = pool.CreateBuffer(SHORTKEY_SIZE);
                relationSpan.WriteInt(0, targetRelationId);
                saveMap.Add(hostId, relationSpan.Address());
                return true;
            }

            relationSpan = pool.ReadBuffer(address);
            if (!relationSpan.Valid())
            { return false; }

            if (relationSpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                int oldMaxCount = relationSpan.Length() / 4;
                for (int i = 0; i < oldMaxCount; i++)
                {
                    var relationId = relationSpan.ReadInt(i * 4);
                    if (relationId < 0)
                    {
                        relationSpan.WriteInt(i * 4, targetRelationId);
                        return true;
                    }
                }

                SerializedSpan newSpan;
                if (oldMaxCount + 1 <= RAW_SEARCH_COUNT)
                {
                    newSpan = pool.CreateBuffer((oldMaxCount + 1) * 4);
                    SerializedSpan.BlockCopy(relationSpan, 0, newSpan, 0, relationSpan.Length());
                    newSpan.WriteInt(oldMaxCount * 4, targetRelationId);
                    pool.FreeBuffer(address);
                    saveMap.Add(hostId, newSpan.Address());
                    return true;
                }

                int rawSize = (int)SerializedInt32Int32Set.MemoryWillAllocatedWithCapacity(RAW_SEARCH_COUNT + 1);
                newSpan = pool.CreateBuffer(rawSize);
                newSpan.BuildSet(relationsSet, 0);
                for (int i = 0; i < oldMaxCount; i++)
                {
                    var relationId = relationSpan.ReadInt(i * 4);
                    if (relationId < 0)
                    { continue; }
                    relationsSet.Add(relationId);
                }
                relationsSet.Add(targetRelationId);
                pool.FreeBuffer(address);
                saveMap.Add(hostId, newSpan.Address());
                return true;
            }

            relationSpan.LoadSet(relationsSet, 0);
            if (relationsSet.Count() == relationsSet.Capacity())
            {
                var newSpan = pool.CreateBuffer((int)SerializedInt32Int32Set.MemoryWillAllocatedWithCapacity(relationsSet.Capacity() + relationsSet.Capacity() / 4));
                SerializedSpan.BlockCopy(relationSpan, 0, newSpan, 0, relationSpan.Length());
                pool.FreeBuffer(address);
                saveMap.Add(hostId, newSpan.Address());
                newSpan.ResizeSet(relationsSet, 0);
            }

            relationsSet.Add(targetRelationId);

            return true;
        }

        private bool AddRelation(SerializedInt32Int32Map saveMap, string host, string relation)
        {
            var hostId = GetOrBakeShortKey(host);
            var targetRelationId = GetOrBakeShortKey(relation);
            return AddRelation(saveMap, hostId, targetRelationId);
        }

        //private bool ResetRelationsWith(SerializedInt32Int32Map saveMap, string host, ICollection<string> relations)
        //{
        //    if (relations.Count() == 0)
        //    { return false; }

        //    var hostId = GetOrBakeShortKey(host);
        //    SerializedSpan relationSpan;
        //    int newCount = relations.Count;
        //    int newSize = 0;
        //    if (newCount <= RAW_SEARCH_COUNT)
        //    { newSize = SHORTKEY_SIZE * newCount; }
        //    else
        //    { newSize = (int)SerializedInt32Int32Set.MemoryWillAllocatedWithCapacity(newCount); }

        //    if (saveMap.Find(hostId, out var address))
        //    {
        //        relationSpan = pool.ReadBuffer(address);
        //        if (!relationSpan.Valid())
        //        { return false; }
        //        if (newSize > relationSpan.Length())
        //        {
        //            pool.FreeBuffer(address);
        //            relationSpan = pool.CreateBuffer(newSize);
        //            address = relationSpan.Address();
        //        }
        //    }
        //    else
        //    {
        //        relationSpan = pool.CreateBuffer(newSize);
        //        address = relationSpan.Address();
        //    }

        //    saveMap.Add(hostId, address);

        //    if (newSize <= RAW_SEARCH_DATA_SIZE)
        //    {
        //        int relationIndex = 0;
        //        foreach (var relationGuid in relations)
        //        {
        //            if (!guid2IntMap.Find(relationGuid, out var relationsId, out _))
        //            { relationsId = GetOrBakeShortKey(relationGuid); }
        //            relationSpan.WriteInt(SHORTKEY_SIZE * relationIndex, relationsId);
        //            relationIndex++;
        //        }
        //    }
        //    else
        //    {
        //        relationSpan.BuildSet(relationsSet, 0);
        //        foreach (var relationGuid in relations)
        //        {
        //            if (!guid2IntMap.Find(relationGuid, out var relationsId, out _))
        //            { relationsId = GetOrBakeShortKey(relationGuid); }
        //            relationsSet.Add(relationsId);
        //        }
        //    }

        //    return true;
        //}

        public bool Delete(string host)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }

            DeleteKey(dependMap, ownMap, hostId);
            dependMap.Delete(hostId, out _);

            DeleteKey(ownMap, dependMap, hostId);
            ownMap.Delete(hostId, out _);

            guid2IntMap.Delete(host, out _);
            int2GuidMap.Delete(hostId, out _);

            return true;
        }

        private void DeleteKey(SerializedInt32Int32Map fistMap, SerializedInt32Int32Map secondMap, int key)
        {
            if (fistMap.Find(key, out var firstKeyAddress))
            {
                var firstKeySpan = pool.ReadBuffer(firstKeyAddress);
                if (firstKeySpan.Valid())
                {
                    if (firstKeySpan.Length() < RAW_SEARCH_DATA_SIZE)
                    {
                        for (int i = 0; i < firstKeySpan.Length() / 4; i++)
                        {
                            var firstValue = firstKeySpan.ReadInt(i * 4);
                            if (firstValue < 0)
                            { continue; }

                            if (!secondMap.Find(firstValue, out var secondValueAddress))
                            { continue; }

                            var secondValueSpan = pool.ReadBuffer(secondValueAddress);
                            if (!secondValueSpan.Valid())
                            { continue; }

                            SpanDeleteInt(ref secondValueSpan, relationsSet, key);
                        }
                    }
                    else
                    {
                        firstKeySpan.LoadSet(relationsSet, 0);
                        foreach (var firstValue in relationsSet.EveryKey())
                        {
                            if (!secondMap.Find(firstValue, out var secondValueAddress))
                            { continue; }

                            var secondValueSpan = pool.ReadBuffer(secondValueAddress);
                            if (!secondValueSpan.Valid())
                            { continue; }

                            SpanDeleteInt(ref secondValueSpan, relationsSet, key);
                        }
                    }
                }
            }
        }

        public static void SpanDeleteInt(ref SerializedSpan keySpan, SerializedInt32Int32Set keySet, int key)
        {
            if (keySpan.Length() <= RAW_SEARCH_DATA_SIZE)
            {
                for (int k = 0; k < keySpan.Length() / 4; k++)
                {
                    var found = keySpan.ReadInt(k * 4);
                    if (found != key)
                    { continue; }
                    keySpan.WriteInt(k * 4, -1);
                    break;
                }
            }
            else
            {
                keySpan.LoadSet(keySet, 0);
                keySet.Delete(key);
            }
        }
    }

#else
    #region SIMPLE_VERSION
    public class SerializedRelationsMap
    {
        SerializedGuid2IntMap guid2IntMap;
        SerializedInt2GuidMap int2GuidMap;
        SerializedBucket dependencyBuckets;
        SerializedBucket ownerBuckets;
        int guidIndexer = 0;

        public SerializedRelationsMap()
        {
            guid2IntMap = new SerializedGuid2IntMap();
            int2GuidMap = new SerializedInt2GuidMap();
            dependencyBuckets = new SerializedBucket();
            ownerBuckets = new SerializedBucket();
        }

        public void Init(int capacity)
        {
            capacity = Math.Max(capacity, 4096);

            guidIndexer = 0;
            guid2IntMap.Init(capacity);
            int2GuidMap.Init(capacity);
            dependencyBuckets.Init(capacity * 3);
            ownerBuckets.Init(capacity * 3);
        }

        public byte[] Save()
        {
            int totalSize = 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependencyBuckets.RawLength() + ownerBuckets.RawLength();
            var bytes = new byte[totalSize];

            BitConverter.TryWriteBytes(bytes, guidIndexer);
            guid2IntMap.MoveTo(bytes, 4);
            int2GuidMap.MoveTo(bytes, 4 + guid2IntMap.RawLength());
            dependencyBuckets.MoveTo(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength());
            ownerBuckets.MoveTo(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependencyBuckets.RawLength());

            return bytes;
        }

        public bool Load(byte[] bytes)
        {
            guidIndexer = BitConverter.ToInt32(bytes, 0);

            guid2IntMap.Load(bytes, 4);
            int2GuidMap.Load(bytes, 4 + guid2IntMap.RawLength());
            dependencyBuckets.Load(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength());
            ownerBuckets.Load(bytes, 4 + guid2IntMap.RawLength() + int2GuidMap.RawLength() + dependencyBuckets.RawLength());

            return true;
        }

        public void Prebuild(Dictionary<string, IEnumerable<string>> dependMap)
        {
            GetOrBakeShortKey(dependMap.Keys);

            foreach (var kvp in dependMap)
            {
                var host = kvp.Key;
                var dependencies = kvp.Value;

                guid2IntMap.Find(host, out var hostId, out _);
                foreach (var dependency in dependencies)
                {
                    guid2IntMap.Find(dependency, out var dependId, out _);
                    dependencyBuckets.Add(hostId, dependId);
                }
            }

            foreach (var kvp in dependencyBuckets.EveryKeyValue())
            {
                ownerBuckets.Add(kvp.Value, kvp.Key);
            }
        }

        public List<int> GetOrBakeShortKey(IEnumerable<string> guids)
        {
            var keys = new List<int>();
            foreach (var host in guids)
            {
                if (!guid2IntMap.Find(host, out int value, out _))
                {
                    guidIndexer = guidIndexer + 1;
                    guid2IntMap.Add(host, guidIndexer);
                    int2GuidMap.Add(guidIndexer, host);
                    value = guidIndexer;
                }
                keys.Add(value);
            }
            return keys;
        }

        public int GetOrBakeShortKey(string host)
        {
            if (!guid2IntMap.Find(host, out int value, out _))
            {
                guidIndexer = guidIndexer + 1;
                guid2IntMap.Add(host, guidIndexer);
                int2GuidMap.Add(guidIndexer, host);
                value = guidIndexer;
            }
            return value;
        }


        public bool HasDependency(string host, string dependency)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            if (dependencyBuckets.Find(hostId, dependencyId))
            { return true; }

            return false;
        }

        public bool HasDependency(ReadOnlySpan<byte> host, ReadOnlySpan<byte> dependency)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            if (dependencyBuckets.Find(hostId, dependencyId))
            { return true; }

            return false;
        }

        public bool HasDependency(int hostId, int dependencyId)
        {
            if (dependencyBuckets.Find(hostId, dependencyId))
            { return true; }

            return false;
        }

        public void GetDependencies(string host, ICollection<string> dependencies)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return; }

            foreach (var dependencyId in dependencyBuckets.EveryValueOf(hostId))
            {
                if (!int2GuidMap.Find(dependencyId, out var guidStruct))
                { continue; }
                dependencies.Add(guidStruct.GetGuid());
            }
        }

        public void GetDependencies(int hostId, ICollection<int> dependencies)
        {
            foreach (var dependencyId in dependencyBuckets.EveryValueOf(hostId))
            {
                dependencies.Add(dependencyId);
            }
        }

        public void GetDependencies(string host, ICollection<string> dependencies, bool recursive)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return; }

            var dependKeys = new HashSet<int>();
            GetDependencies(hostId, dependKeys, recursive);
            foreach (var dependKey in dependKeys)
            {
                if (!int2GuidMap.Find(dependKey, out var guidStruct))
                { continue; }
                dependencies.Add(guidStruct.GetGuid());
            }
        }

        public void GetDependencies(int hostId, ICollection<int> dependencies, bool recursive)
        {
            foreach (var dependencyId in dependencyBuckets.EveryValueOf(hostId))
            {
                if (dependencies.Contains(dependencyId))
                { continue; }
                dependencies.Add(dependencyId);
                if (!recursive)
                { continue; }

                GetDependencies(dependencyId, dependencies, recursive);
            }
        }


        public bool HasOwner(string dependency, string host)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            if (ownerBuckets.Find(dependencyId, hostId))
            { return true; }

            return false;
        }

        public bool HasOwner(ReadOnlySpan<byte> host, ReadOnlySpan<byte> dependency)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return false; }

            if (ownerBuckets.Find(dependencyId, hostId))
            { return true; }

            return false;
        }

        public bool HasOwner(int dependencyId, int hostId)
        {
            if (ownerBuckets.Find(dependencyId, hostId))
            { return true; }

            return false;
        }


        public void GetOwners(string dependency, ICollection<string> owners)
        {
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return; }

            foreach (var ownerId in ownerBuckets.EveryValueOf(dependencyId))
            {
                if (!int2GuidMap.Find(ownerId, out var guidStruct))
                { continue; }
                owners.Add(guidStruct.GetGuid());
            }
        }

        public void GetOwners(int dependencyId, ICollection<int> owners)
        {
            foreach (var ownerId in ownerBuckets.EveryValueOf(dependencyId))
            {
                owners.Add(ownerId);
            }
        }

        public void GetOwners(string dependency, ICollection<string> owners, bool recursive)
        {
            if (!guid2IntMap.Find(dependency, out int dependencyId, out _))
            { return; }

            var ownerKeys = new HashSet<int>();
            GetOwners(dependencyId, ownerKeys, recursive);

            foreach (var ownerKey in ownerKeys)
            {
                if (!int2GuidMap.Find(ownerKey, out var guidStruct))
                { continue; }
                owners.Add(guidStruct.GetGuid());
            }
        }


        public void GetOwners(int dependencyId, ICollection<int> owners, bool recursive)
        {
            foreach (var ownerId in ownerBuckets.EveryValueOf(dependencyId))
            {
                if (owners.Contains(ownerId))
                { continue; }
                owners.Add(ownerId);
                if (!recursive)
                { continue; }

                GetOwners(ownerId, owners, recursive);
            }
        }

        public bool AddDependencies(string host, ICollection<string> dependencies)
        {
            var hostId = GetOrBakeShortKey(host);

            foreach (var dependency in dependencies)
            {
                var dependencyId = GetOrBakeShortKey(dependency);
                dependencyBuckets.Add(hostId, dependencyId);
                ownerBuckets.Add(dependencyId, hostId);
            }

            return true;
        }

        public bool Delete(string host)
        {
            if (!guid2IntMap.Find(host, out int hostId, out _))
            { return false; }

            foreach (var dependencyId in dependencyBuckets.EveryValueOf(hostId))
            {
                ownerBuckets.Delete(dependencyId, hostId);
            }

            dependencyBuckets.Delete(hostId);

            guid2IntMap.Delete(host, out _);
            int2GuidMap.Delete(hostId, out _);

            return true;
        }
    }
    #endregion
#endif

    internal static class HashSetPool<TValue>
    {
        static Stack<HashSet<TValue>> list = new Stack<HashSet<TValue>>();
        public static HashSet<TValue> Get()
        {
            HashSet<TValue> container;
            if (list.Count > 0)
            {
                container = list.Pop();
            }
            else
            {
                container = new HashSet<TValue>();
            }

            return container;
        }

        public static void Return(HashSet<TValue> container)
        {
            container.Clear();
            list.Push(container);
        }
    }
}
