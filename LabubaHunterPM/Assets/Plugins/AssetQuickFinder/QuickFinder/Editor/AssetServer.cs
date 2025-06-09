//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using QuickFinder.Container;
using QuickFinder.Editor;

namespace QuickFinder.Assets
{
    [InitializeOnLoad]
    public class AssetServer
    {
        public const int VERSION = 1;
        public const float AUTOSAVE_TIME = 30;
        public const float AUTOSAVE_COUNTER = 128;
        public const string DISK_PATH = "Library/SerializedRelationsMap.db";

        SerializedRelationsMap relationsMap;

        private static long autoSaveTick = 0;
        private int motifyCounter;

        private static AssetServer instance;
        public static AssetServer Instance
        {
            get
            {
                if(instance == null)
                { 
                    instance = new AssetServer();
                    instance.Init();
                    autoSaveTick = System.DateTime.Now.Ticks;
                    UnityEditor.EditorApplication.quitting += instance.OnQuit;
                    UnityEditor.EditorApplication.update += instance.AutoSave;
                }
                return instance;
            }
        }

        ~AssetServer()
        {
            if(instance != null)
            {
                autoSaveTick = 0;
                UnityEditor.EditorApplication.update -= instance.AutoSave;
                UnityEditor.EditorApplication.quitting -= instance.OnQuit;
            }

        }

        static AssetServer()
        {
            Instance.motifyCounter = 0;
        }

        public void Init(bool forceRebuild = false)
        {
            if((!forceRebuild) && (relationsMap != null))
            {  return; }
            
            if (forceRebuild || !System.IO.File.Exists(DISK_PATH) || (PlayerPrefs.GetInt("QUICKFINDER_ASSETSERVER_VERSION", 0) != VERSION))
            {
                Prebuild();
                Save();
                PlayerPrefs.SetInt("QUICKFINDER_ASSETSERVER_VERSION", VERSION);
            }
            else
            {
                var bytes = System.IO.File.ReadAllBytes(DISK_PATH);
                relationsMap = new SerializedRelationsMap();
                relationsMap.Load(bytes);
            }
        }

        public void Prebuild()
        {
            relationsMap = new SerializedRelationsMap();

            var paths = AssetDatabase.GetAllAssetPaths();
            relationsMap.Init(paths.Length + paths.Length / 4);

            var dependMap = new Dictionary<string, IEnumerable<string>>();
            int handledCount = 0;
            int stepCount = paths.Length / 100 + 1;
            foreach (var path in paths)
            {
                var hostGuid = AssetDatabase.AssetPathToGUID(path);
                var dependencies = AssetDatabase.GetDependencies(path, false);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    dependencies[i] = AssetDatabase.AssetPathToGUID(dependencies[i]);
                }
                dependMap[hostGuid] = dependencies;

                if (handledCount % stepCount == 0)
                {
                    EditorUtility.DisplayProgressBar("asset database building", "asset database building, it will take a few seconds depend on your hadware", (float)handledCount / paths.Length);
                }
                handledCount++;
            }
            EditorUtility.ClearProgressBar();

            relationsMap.Prebuild(dependMap);
        }

        public void Rebuild()
        {
            Init(true);
        }

        public void Save()
        {
            if (relationsMap == null)
            { return; }
            var bytes = relationsMap?.Save();

            System.IO.File.WriteAllBytes(DISK_PATH, bytes);
        }

        public void OnQuit()
        {
            Save();
        }

        public void AutoSave()
        {
            var currentTick = System.DateTime.Now.Ticks;
            var passTime = (currentTick - autoSaveTick) / 10000000f;
            if(passTime >= AUTOSAVE_TIME && motifyCounter > 0)
            {
                autoSaveTick = currentTick;
                motifyCounter = 0;
                Save();
                return;
            }

            if(motifyCounter >= AUTOSAVE_COUNTER)
            {
                autoSaveTick = currentTick;
                motifyCounter = 0;
                Save();
                return;
            }
        }

        public bool HasDependency(string host, string dependency)
        {
            return relationsMap.HasDependency(host, dependency);
        }

        public bool HasDependency(string host, string dependency, bool recursive)
        {
            return relationsMap.HasDependency(host, dependency, recursive);
        }

        public void GetDependencies(string hostPath, ICollection<string> dependencies)
        {
            if (FileEditorUtility.IsFile(hostPath))
            {
                var hostGuid = AssetDatabase.AssetPathToGUID(hostPath);
                GetDependenciesByGuid(hostGuid, dependencies);
            }
            else
            {
                var allHosts = FileEditorUtility.GetAssetsByUnity(new string[] { hostPath });
                foreach (var host in allHosts)
                {
                    var hostGuid = AssetDatabase.AssetPathToGUID(host);
                    GetDependenciesByGuid(hostGuid, dependencies);
                }
            }
        }

        public void GetDependencies(string hostPath, ICollection<string> dependencies, bool recursive)
        {
            if (FileEditorUtility.IsFile(hostPath))
            {
                var hostGuid = AssetDatabase.AssetPathToGUID(hostPath);
                GetDependenciesByGuid(hostGuid, dependencies, recursive);
            }
            else
            {
                var allHosts = FileEditorUtility.GetAssetsByUnity(new string[] { hostPath });
                foreach (var host in allHosts)
                {
                    var hostGuid = AssetDatabase.AssetPathToGUID(host);
                    GetDependenciesByGuid(hostGuid, dependencies, recursive);
                }
            }
        }

        public void GetDependencies(List<string> hostPaths, ICollection<string> dependencies, bool recursive)
        {
            for (int i = 0; i < hostPaths.Count; i++)
            {
                GetDependencies(hostPaths[i], dependencies, recursive);
            }
        }

        public void GetDependenciesByGuid(string hostGuid, ICollection<string> dependencies, bool recursive = false)
        {
            if (!recursive)
                relationsMap.GetDependencies(hostGuid, dependencies);
            else
            {
                relationsMap.GetDependencies(hostGuid, dependencies, recursive);
            }
        }

        public bool HasOwner(string dependency, string own)
        {
            return relationsMap.HasOwner(dependency, own);
        }

        public bool HasOwner(string dependency, string own, bool recursive)
        {
            return relationsMap.HasOwner(dependency, own, recursive);
        }

        public void GetOwners(string dependencyPath, ICollection<string> owners, bool recursive)
        {
            if(FileEditorUtility.IsFile(dependencyPath))
            {
                var dependencyGuid = AssetDatabase.AssetPathToGUID(dependencyPath);
                GetOwnersByGuid(dependencyGuid, owners, recursive);
            }
            else
            {
                var dependencies = FileEditorUtility.GetAssetsByUnity(new string[] { dependencyPath });
                foreach(var dependency in dependencies)
                {
                    var dependencyGuid = AssetDatabase.AssetPathToGUID(dependency);
                    GetOwnersByGuid(dependencyGuid, owners, recursive);
                }
            }
        }

        public void GetOwners(List<string> dependencyPaths, ICollection<string> owners, bool recursive)
        {
            for (int i = 0; i < dependencyPaths.Count; i++)
            {
                GetOwners(dependencyPaths[i], owners, recursive);
            }
        }

        public void GetOwnersByGuid(string dependencyGuid, ICollection<string> owners, bool recursive = false)
        {
            if (!recursive)
                relationsMap.GetOwners(dependencyGuid, owners);
            else
                relationsMap.GetOwners(dependencyGuid, owners, true);
        }

        public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var asset in deletedAssets)
            {
                motifyCounter++;
                var deleteGuid = AssetDatabase.AssetPathToGUID(asset);
                relationsMap.Delete(deleteGuid);
            }

            var importGuids = importedAssets.Select(x=> AssetDatabase.AssetPathToGUID(x)).ToList();
            var imortShortKeys = relationsMap.GetOrBakeShortKey(importGuids);

            foreach (var asset in importedAssets)
            {
                motifyCounter++;
                var importGuid = AssetDatabase.AssetPathToGUID(asset);
                var dependencies = AssetDatabase.GetDependencies(asset, false).Select(x => relationsMap.GetOrBakeShortKey(AssetDatabase.AssetPathToGUID(x))).ToHashSet();
                var importId = relationsMap.GetOrBakeShortKey(importGuid);
                relationsMap.AddDependencies(importId, dependencies);
            }
        }
    }

    public class AssetInfoPostProcessor : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AssetServer.Instance.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
        }
    }
}

