//------------------------------------------------------------//
// yanfei 2025.03.05
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using QuickFinder.Text.YAML;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Concurrent;

namespace QuickFinder.Assets
{

    public class AssetInSceneSearcher
    {
        public class ParsingObject
        {
            public long typeId;
            public string typeName;
            public long objectId;

            public long gameObjectId;
            public long prefabInstaceId;
            public long fatherId;
            public List<long> childs;
            public List<long> components;
            public string name;
            public List<PrefabOverrideElement> overrideNames;
            public string guid;
            public string host;
            public long transformParent;//for PrefabInstance
            public long referencedByVariant;
            public VariantPrefabModification variantModification;
            public ParsingAssetTarget overrideSourceAsset;

            public List<ParsingAssetTarget> relatedAssets;
            public int subSceneRelatedIndex = -1;
            public int scriptRelatedIndex = -1;

            public long parsingOrder;
            public string prefabFileName;
            public bool matchSelf = false;
            public bool matchInChilds = false;
            public bool matchAddedVariant = false;
            public bool isRoot = false;
            public List<List<ParsingObject>> childsInPrefab = null;
            public void AddChild(long objectId)
            {
                if (childs == null)
                { childs = new List<long>(); }
                childs.Add(objectId);
            }

            public void AddComponent(long objectId)
            {
                if (components == null)
                { components = new List<long>(); }
                components.Add(objectId);
            }

            public void AddOverrideName(string name_, PrefabOverrideElement fileid)
            {
                if (overrideNames == null)
                { overrideNames = new List<PrefabOverrideElement>(1); }
                overrideNames.Add(fileid);
            }

            public bool TryGetOverrideName(long fileID, out PrefabOverrideElement element)
            {
                if (overrideNames == null)
                {
                    element = default;
                    return false;
                }
                foreach (var ele in overrideNames)
                {
                    if (ele.target.fileID == fileID)
                    {
                        element = ele;
                        return true;
                    }
                }

                element = default;
                return false;
            }

            public void AddRelatedAsset(ParsingAssetTarget target)
            {
                if (relatedAssets == null)
                { relatedAssets = new List<ParsingAssetTarget>(1); }
                relatedAssets.Add(target);
            }

            public void RelapceLatestRelatedAssets(ParsingAssetTarget target)
            {
                relatedAssets[relatedAssets.Count - 1] = target;
            }

            public void TryCreateRelateAssets(long fileId)
            {
                if (relatedAssets == null)
                { 
                    relatedAssets = new List<ParsingAssetTarget>(1); 
                }
                foreach(var asset in relatedAssets)
                {
                    if(asset.fileID == fileId)
                    { return; }
                }
                relatedAssets.Add(new ParsingAssetTarget { fileID = fileId });
            }

            public void TrySetRelatedAssets(long fileId, string guid)
            {
                if (relatedAssets == null)
                {
                    relatedAssets = new List<ParsingAssetTarget>(1);
                }
                for(int i = 0; i < relatedAssets.Count; i++)
                {
                    var asset = relatedAssets[i];
                    if (asset.fileID == fileId)
                    {
                        asset.guid = guid;
                        relatedAssets[i] = asset;
                        return; 
                    }
                }
                relatedAssets.Add(new ParsingAssetTarget { fileID = fileId, guid = guid });
            }

            public bool TryGetRelatedAssets(string guid, out ParsingAssetTarget target)
            {
                if (string.IsNullOrEmpty(guid) || relatedAssets == null)
                { target = default; return false; }
                foreach (var ele in relatedAssets)
                {
                    if (ele.guid == guid)
                    {
                        target = ele;
                        return true;
                    }
                }
                target = default;
                return false;
            }
            public bool TryGetRelatedAssets(int fileID, out ParsingAssetTarget target)
            {
                if (fileID <= 0 || relatedAssets == null)
                { target = default; return false; }
                foreach (var ele in relatedAssets)
                {
                    if (ele.fileID == fileID)
                    {
                        target = ele;
                        return true;
                    }
                }
                target = default;
                return false;
            }

            public string ToShortString()
            {
                return "[" + typeId.ToString() + " " + objectId.ToString() + " " + typeName + "]";
            }

            public string ToAssetString()
            {
                return ObjectString(nameof(typeId), typeId) + ObjectString(nameof(objectId), objectId) + ObjectString(nameof(typeName), typeName) +
                    ObjectString(nameof(matchSelf), matchSelf) + ObjectString(nameof(name), name) + ObjectString(nameof(guid), guid) + ObjectString(nameof(prefabInstaceId), prefabInstaceId) +
                    ObjectString(nameof(gameObjectId), gameObjectId) + ObjectString(nameof(childs), childs) + ObjectString(nameof(components), components);
            }

            private string ObjectString(string pname, string pvalue)
            {
                if (pvalue == null)
                {
                    return string.Format("¡¾{0}: null¡¿", pname);
                }
                return string.Format("¡¾{0}: {1}¡¿", pname, pvalue);
            }

            private string ObjectString(string pname, long pvalue)
            {
                return string.Format("¡¾{0}: {1}¡¿", pname, pvalue);
            }
            private string ObjectString(string pname, bool pvalue)
            {
                return string.Format("¡¾{0}: {1}¡¿", pname, pvalue);
            }

            private string ObjectString(string pname, List<long> pvalue)
            {
                if (pvalue == null)
                {
                    return string.Format("¡¾{0}: null¡¿", pname);
                }
                return string.Format("¡¾{0}: {1}¡¿", pname, string.Join(",", pvalue));
            }

            private string ObjectString(string pname, List<List<long>> pvalue)
            {
                if (pvalue == null)
                {
                    return string.Format("¡¾{0}: null¡¿", pname);
                }
                return string.Format("¡¾{0}: {1}¡¿", pname, string.Join(",", pvalue.Select(x => string.Join("|", string.Join(",", x)))));
            }

            private string ObjectString(string pname, List<List<string>> pvalue)
            {
                if (pvalue == null)
                {
                    return string.Format("¡¾{0}: null¡¿", pname);
                }
                return string.Format("¡¾{0}: {1}¡¿", pname, string.Join(",", pvalue.Select(x => string.Join("|", string.Join(",", x)))));
            }

            //private string ObjectString(string pname, List<List<ParsingObject>> pvalue)
            //{
            //    if (pvalue == null)
            //    {
            //        return string.Format("¡¾{0}: null¡¿", pname);
            //    }
            //    return string.Format("¡¾{0}: {1}¡¿", pname, string.Join(",", pvalue.Select(x => string.Join("|", string.Join(",", x)))));
            //}
        }

        public struct ParsingAssetTarget
        {
            public long fileID;
            public string guid;
            public byte type;

            //public int counter;

            public void Clear() { fileID = 0; guid = null; type = byte.MinValue; }
            public bool IsEmpty() { return fileID == 0 && string.IsNullOrEmpty(guid); }
        }

        public struct PrefabOverrideElement
        {
            public ParsingAssetTarget target;
            public string propertyPath;
            public string propertyValue;

            public void Clear()
            {
                target.Clear();
                propertyPath = null;
                propertyValue = null;
            }
        }

        public class VariantPrefabModification
        {
            //public class AddObjectElement
            //{
            //    public ParsingAssetTarget sourceTarget;
            //    public long addReferenceObjectId;
            //    public int insertIndex;
            //}
            public List<long> removedComponents;
            public List<long> removedGameObjectIds;
            public List<long> addComponentAssetIds;
            public List<long> addComponentReferenceIds;
            public List<int> addComponentInsertIndexs;
            public List<long> addGameObjectAssetIds;
            public List<long> addGameObjectReferenceIds;
            public List<int> addGameObjectInsertIndexs;

            public void AddRemovedComponnetId(long id)
            {
                if (removedComponents == null)
                { removedComponents = new List<long>(1); }
                removedComponents.Add(id);
            }
            public void AddRemovedGameObjectIds(long id)
            {
                if (removedGameObjectIds == null)
                { removedGameObjectIds = new List<long>(1); }
                removedGameObjectIds.Add(id);
            }
            public void AddComponentAssetId(long id)
            {
                if (addComponentAssetIds == null)
                { addComponentAssetIds = new List<long>(1); }
                addComponentAssetIds.Add(id);
            }
            public void AddComponentReferenceId(long id)
            {
                if (addComponentReferenceIds == null)
                { addComponentReferenceIds = new List<long>(1); }
                addComponentReferenceIds.Add(id);
            }
            public void AddComponentInsertIndex(int id)
            {
                if (addComponentInsertIndexs == null)
                { addComponentInsertIndexs = new List<int>(1); }
                addComponentInsertIndexs.Add(id);
            }

            public void AddGameObjectAssetId(long id)
            {
                if (addGameObjectAssetIds == null)
                { addGameObjectAssetIds = new List<long>(1); }
                addGameObjectAssetIds.Add(id);
            }
            public void AddGameObjectReferenceId(long id)
            {
                if (addGameObjectReferenceIds == null)
                { addGameObjectReferenceIds = new List<long>(1); }
                addGameObjectReferenceIds.Add(id);
            }
            public void AddGameObjectInsertIndex(int index)
            {
                if (addGameObjectInsertIndexs == null)
                { addGameObjectInsertIndexs = new List<int>(1); }
                addGameObjectInsertIndexs.Add(index);
            }


            public void Clear()
            {
                addComponentReferenceIds = null; addGameObjectAssetIds = null; addGameObjectReferenceIds = null; addGameObjectInsertIndexs = null;
            }
            public bool Valid()
            {
                return addComponentReferenceIds != null || (addGameObjectAssetIds.Count != 0 && addGameObjectReferenceIds.Count != 0);
            }
        }

        public class OneParsingResult
        {
            private Dictionary<long, ParsingObject> objectTable = new Dictionary<long, ParsingObject>();
            //public List<List<string>> targetHierarchys = null; //null when init, indicate it hasn't been parsing
            private HashSet<ParsingObject> roots = new HashSet<ParsingObject>();
            public string assetPath;
            public string assetGuid;

            public bool TryGetObject(long objectId, out ParsingObject parsingObject)
            {
                return objectTable.TryGetValue(objectId, out parsingObject);
            }

            public void AddObject(long objectId, ParsingObject parsingObject)
            {
                objectTable.Add(objectId, parsingObject);
            }

            private void AddRoot(ParsingObject rootObject)
            {
                if (roots.Contains(rootObject))
                { return; }
                rootObject.isRoot = true;
                roots.Add(rootObject);
            }

            //private void AddHierarchy(List<string> hierarchy)
            //{
            //    if(targetHierarchys == null)
            //    { targetHierarchys = new List<List<string>>(); }
            //    targetHierarchys.Add(hierarchy);
            //}

            public void ParseTargetAssetText(ParsingResults parsingResults, ICollection<string> targetGuids)
            {
                long currentObjectId = -1;
                PrefabOverrideElement prefabOverrideElement = new PrefabOverrideElement();
                ParsingAssetTarget parsingAssetTarget = new ParsingAssetTarget();

                var parser = new QuickFinder.Text.YAML.ManualParser();
                var hostText = System.IO.File.ReadAllText(assetPath);
                var parsingCaller = new ParsingCaller();
                parsingCaller.SearchHierarchy("*");

                void OnPropertyFound(ParsingCaller caller)
                {
                    ParsingObject GetAndBakeObject(long objectId)
                    {
                        if (!TryGetObject(objectId, out var parsingObject))
                        {
                            parsingObject = new ParsingObject();
                            parsingObject.typeId = caller.typeId;
                            parsingObject.objectId = caller.objectId;
                            parsingObject.typeName = caller.FirstHierarchy.ToString();
                            parsingObject.host = assetGuid;
                            parsingObject.parsingOrder = caller.proertyCounter;
                            AddObject(objectId, parsingObject);
                        }

                        return parsingObject;
                    }

                    var parent = caller.ParentHierarchy();

                    if (caller.FirstHierarchy.Equals("Transform") || caller.FirstHierarchy.Equals("RectTransform"))
                    {
                        var parsingObject = GetAndBakeObject(caller.objectId);
                        if (parent.Equals("m_GameObject"))
                        {
                            parsingObject.gameObjectId = caller.propertyValue.ToInteger();
                        }
                        else if (parent.Equals("m_PrefabInstance"))
                        {
                            parsingObject.prefabInstaceId = caller.propertyValue.ToInteger();
                        }
                        else if (parent.Equals("m_CorrespondingSourceObject") && caller.propertyName.Equals("guid"))
                        {
                            parsingObject.guid = caller.propertyValue.ToString();
                        }
                        else if (parent.Equals("m_Father"))
                        {
                            parsingObject.fatherId = caller.propertyValue.ToInteger();
                        }
                        else if (parent.Equals("m_Children"))
                        {
                            parsingObject.AddChild(caller.propertyValue.ToInteger());
                        }
                    }


                    else if (caller.FirstHierarchy.Equals("GameObject"))
                    {
                        var parsingObject = GetAndBakeObject(caller.objectId);
                        if (parent.Equals("component"))
                        {
                            parsingObject.AddComponent(caller.propertyValue.ToInteger());
                        }
                        else if (caller.propertyName.Equals("m_Name"))
                        {
                            parsingObject.name = caller.propertyValue.ToString();
                        }
                        else if (parent.Equals("m_PrefabInstance") && caller.propertyName.Equals("fileID"))//for variant prefab added component on it's hierarachy
                        {
                            parsingObject.prefabInstaceId = caller.propertyValue.ToInteger();
                        }
                        else if (parent.Equals("m_CorrespondingSourceObject") && caller.propertyName.Equals("fileID"))//for variant prefab added component on it's hierarachy
                        {
                            parsingObject.overrideSourceAsset.fileID = caller.propertyValue.ToInteger();
                        }
                        else if (parent.Equals("m_CorrespondingSourceObject") && caller.propertyName.Equals("guid"))//for variant prefab added component on it's hierarachy
                        {
                            parsingObject.overrideSourceAsset.guid = caller.propertyValue.ToString();
                        }
                    }


                    if (caller.FirstHierarchy.Equals("PrefabInstance"))
                    {
                        var parsingObject = GetAndBakeObject(caller.objectId);
                        if (parent.Equals("target") && caller.propertyName.Equals("fileID"))
                        {
                            prefabOverrideElement.target.fileID = caller.propertyValue.ToInteger();
                        }
                        else if (caller.propertyName.Equals("propertyPath") && caller.propertyValue.Equals("m_Name"))
                        {
                            prefabOverrideElement.propertyPath = "m_Name";
                        }
                        else if ((!string.IsNullOrEmpty(prefabOverrideElement.propertyPath)) && caller.propertyName.Equals("value"))
                        {
                            prefabOverrideElement.propertyValue = caller.propertyValue.ToString();
                            parsingObject.AddOverrideName(prefabOverrideElement.propertyValue, prefabOverrideElement);

                            prefabOverrideElement.Clear();
                        }
                        else if (parent.Equals("m_SourcePrefab") && caller.propertyName.Equals("guid"))
                        {
                            parsingObject.guid = caller.propertyValue.ToString();
                            var assetPath = AssetDatabase.GUIDToAssetPath(parsingObject.guid);
                            var prefabName = System.IO.Path.GetFileName(assetPath);
                            prefabName = prefabName.Substring(0, prefabName.IndexOf('.'));
                            parsingObject.prefabFileName = prefabName;
                        }
                        else if (parent.Equals("m_TransformParent"))
                        {
                            parsingObject.transformParent = caller.propertyValue.ToInteger();
                        }
                        else if (caller.CompareHierarchyReverse("m_AddedComponents", 2) && parent.Equals("targetCorrespondingSourceObject") && caller.propertyName.Equals("fileID"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddComponentAssetId(caller.propertyValue.ToInteger());
                        }
                        else if (caller.CompareHierarchyReverse("m_AddedComponents", 2) && parent.Equals("addedObject"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddComponentReferenceId(caller.propertyValue.ToInteger());
                        }
                        else if (parent.Equals("m_AddedGameObjects") && caller.propertyName.Equals("insertIndex"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddComponentInsertIndex((int)caller.propertyValue.ToInteger());
                        }
                        else if (caller.CompareHierarchyReverse("m_AddedGameObjects", 2) && parent.Equals("targetCorrespondingSourceObject") && caller.propertyName.Equals("fileID"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddGameObjectAssetId(caller.propertyValue.ToInteger());
                        }
                        else if (caller.CompareHierarchyReverse("m_AddedGameObjects", 2) && parent.Equals("addedObject"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddGameObjectReferenceId(caller.propertyValue.ToInteger());
                        }
                        else if (parent.Equals("m_AddedGameObjects") && caller.propertyName.Equals("insertIndex"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddGameObjectInsertIndex((int)caller.propertyValue.ToInteger());
                        }

                        else if (parent.Equals("m_RemovedComponents") && caller.propertyName.Equals("fileID"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddRemovedComponnetId((int)caller.propertyValue.ToInteger());
                        }
                        else if (parent.Equals("m_RemovedGameObjects") && caller.propertyName.Equals("fileID"))
                        {
                            if (parsingObject.variantModification == null) { parsingObject.variantModification = new VariantPrefabModification(); }
                            parsingObject.variantModification.AddRemovedGameObjectIds((int)caller.propertyValue.ToInteger());
                        }
                    }


                    else if (caller.FirstHierarchy.Equals("SceneRoots"))
                    {
                        if (parent.Equals("m_Roots"))
                        {
                            var parsingObject = GetAndBakeObject(caller.objectId);
                            parsingObject.AddChild(caller.propertyValue.ToInteger());
                        }
                    }

                    if (caller.typeId != TYPEID_GAMEOBJECT && caller.typeId != TYPEID_PREFABINSTANCE && !IsTransform(caller.typeId))
                    {
                        if (parent.Equals("m_GameObject") && caller.propertyName.Equals("fileID"))
                        {
                            var parsingObject = GetAndBakeObject(caller.objectId);
                            parsingObject.gameObjectId = caller.propertyValue.ToInteger();
                        }
                        else if (caller.isCurlyPair && parent.Valid() && !parent.Equals("target"))
                        {
                            var parsingObject = GetAndBakeObject(caller.objectId);
                            if (caller.propertyName.Equals("fileID"))
                            {
                                parsingAssetTarget.fileID = caller.propertyValue.ToInteger();
                                if (parsingAssetTarget.fileID != 0)
                                {
                                    parsingObject.TryCreateRelateAssets(parsingAssetTarget.fileID);
                                }
                            }
                            else if (caller.propertyName.Equals("guid"))
                            {
                                parsingAssetTarget.guid = caller.propertyValue.ToString();
                                if (parsingAssetTarget.fileID != 0)
                                {
                                    parsingObject.TrySetRelatedAssets(parsingAssetTarget.fileID, parsingAssetTarget.guid);
                                }

                                if (parent.Equals("_SceneAsset") && caller.CompareHierarchyReverse("MonoBehaviour", 2))
                                {
                                    var subScenePath = AssetDatabase.GUIDToAssetPath(parsingAssetTarget.guid);
                                    if(subScenePath.EndsWith(".unity"))
                                    {
                                        parsingObject.subSceneRelatedIndex = parsingObject.relatedAssets.Count - 1;
                                    }
                                }
                                else if(parent.Equals("m_Script") && caller.CompareHierarchyReverse("MonoBehaviour", 2))
                                {
                                    parsingObject.scriptRelatedIndex = parsingObject.relatedAssets.Count - 1;
                                }
                            }
                            //else if (caller.propertyName.Equals("type"))
                            //{
                            //    parsingAssetTarget.type = (byte)caller.propertyValue.ToInteger();
                            //    parsingAssetTarget.Clear();
                            //}
                        }
                    }

                }

                parsingCaller.callback = (c) =>
                {
                    if (currentObjectId != c.objectId)
                    {
                        prefabOverrideElement.Clear();
                        parsingAssetTarget.Clear();
                        currentObjectId = c.objectId;
                    }
                    OnPropertyFound(c);
                };
                parser.ParseYAML(hostText, parsingCaller);
            }

            private void TryLoadOverrideResultOfRelateAssets(ParsingResults parsingResults, ParsingObject componentObject, ICollection<string> targetGuids)
            {
                if (TryGetObject(componentObject.gameObjectId, out var gameObject))
                {
                    if (!gameObject.overrideSourceAsset.IsEmpty())
                    {
                        var parsingObjectPath = AssetDatabase.GUIDToAssetPath(gameObject.overrideSourceAsset.guid);
                        if (parsingObjectPath.EndsWith(".prefab"))
                        {
                            if (!parsingResults.TryGetResult(gameObject.overrideSourceAsset.guid, out var childParsingResult))
                            {
                                childParsingResult = new OneParsingResult { assetGuid = gameObject.overrideSourceAsset.guid, assetPath = parsingObjectPath };
                                parsingResults.AddResult(gameObject.overrideSourceAsset.guid, childParsingResult);
                                childParsingResult.ParseTargetAssetText(parsingResults, targetGuids);
                                childParsingResult.PrepareInfo(parsingResults, targetGuids);
                            }
                        }
                    }
                }
            }

            private void LoadHostVariantAssets(ParsingResults parsingResults, ParsingObject variantPrefab, ICollection<string> targetGuids)
            {
                if (!string.IsNullOrEmpty(variantPrefab.guid))
                {
                    var dependencies = new HashSet<string>();
                    AssetServer.Instance.GetDependencies(variantPrefab.guid, dependencies);
                    dependencies.Add(variantPrefab.guid);
                    foreach (var dependencyGuid in dependencies)
                    {
                        var dependencyPath = AssetDatabase.GUIDToAssetPath(dependencyGuid);
                        if (dependencyPath.EndsWith(".prefab"))
                        {
                            if (!parsingResults.TryGetResult(dependencyGuid, out var childParsingResult))
                            {
                                childParsingResult = new OneParsingResult { assetGuid = dependencyGuid, assetPath = dependencyPath };
                                parsingResults.AddResult(dependencyGuid, childParsingResult);
                                childParsingResult.ParseTargetAssetText(parsingResults, targetGuids);
                                childParsingResult.PrepareInfo(parsingResults, targetGuids);
                            }
                        }
                    }
                }
            }

            private bool TryMarkParsingObject(ParsingResults parsingResults, ParsingObject parsingObject, string targetGuid, ICollection<string> targetGuids)
            {
                if (parsingObject.TryGetRelatedAssets(targetGuid, out var relatedAsset) || targetGuid == parsingObject.guid)
                {
                    parsingObject.matchSelf = true;
                    if (!relatedAsset.IsEmpty())
                    {
                        TryLoadOverrideResultOfRelateAssets(parsingResults, parsingObject, targetGuids);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(parsingObject.guid))
                    { return false; }

                    bool hasDependency = AssetServer.Instance.HasDependency(parsingObject.guid, targetGuid, true);
                    if (hasDependency)
                    {
                        parsingObject.matchInChilds = true;
                    }
                }
                if (parsingObject.matchSelf || parsingObject.matchInChilds)
                {
                    if (string.IsNullOrEmpty(parsingObject.guid))
                    { return false; }
                    if (!parsingResults.TryGetResult(parsingObject.guid, out var childParsingResult))
                    {
                        var parsingObjectPath = AssetDatabase.GUIDToAssetPath(parsingObject.guid);
                        childParsingResult = new OneParsingResult { assetGuid = parsingObject.guid, assetPath = parsingObjectPath };
                        parsingResults.AddResult(parsingObject.guid, childParsingResult);
                        childParsingResult.ParseTargetAssetText(parsingResults, targetGuids);
                        childParsingResult.PrepareInfo(parsingResults, targetGuids);
                    }
                    return true;
                }
                return false;
            }

            public void PrepareInfo(ParsingResults parsingResults, ICollection<string> targetGuids)
            {
                var referencedVariants = new HashSet<long>();
                foreach (var kv in objectTable)
                {
                    var parsingObject = kv.Value;

                    foreach (var targetGuid in targetGuids)
                    {
                        if (string.IsNullOrEmpty(targetGuid))
                        { continue; }

                        if (parsingObject.typeId != TYPEID_GAMEOBJECT && !IsTransform(parsingObject.typeId) && parsingObject.typeId != TYPEID_PREFABINSTANCE)
                        {
                            if (parsingObject.subSceneRelatedIndex >= 0)//
                            {
                                var subSceneGuid = parsingObject.relatedAssets[parsingObject.subSceneRelatedIndex].guid;
                                if (AssetServer.Instance.HasDependency(subSceneGuid, targetGuid, true))
                                {
                                    parsingResults.GetOrCreateSubSceneResult(subSceneGuid, out var parsingResult);
                                }
                            }
                            
                            if (parsingObject.TryGetRelatedAssets(targetGuid, out var relatedAsset) || targetGuid == parsingObject.guid)
                            {
                                parsingObject.matchSelf = true;
                                if (!relatedAsset.IsEmpty())
                                {
                                    TryLoadOverrideResultOfRelateAssets(parsingResults, parsingObject, targetGuids);
                                    LoadHostVariantAssets(parsingResults, parsingObject, targetGuids);
                                }
                            }
                            else
                            {
                                if(parsingObject.relatedAssets != null)
                                {
                                    foreach (var rasset in parsingObject.relatedAssets)
                                    {
                                        if (!string.IsNullOrEmpty(rasset.guid))
                                        {
                                            bool hasDependency = AssetServer.Instance.HasDependency(rasset.guid, targetGuid, true);
                                            if (hasDependency)
                                            {
                                                parsingObject.matchSelf = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (parsingObject.variantModification != null && parsingObject.variantModification.Valid())
                            {
                                if (parsingObject.variantModification.addComponentReferenceIds != null)
                                {
                                    foreach (var referenceObjectId in parsingObject.variantModification.addComponentReferenceIds)
                                    {
                                        if (!TryGetObject(referenceObjectId, out var referenceObject))
                                        { continue; }

                                        if (TryMarkParsingObject(parsingResults, referenceObject, targetGuid, targetGuids))
                                        {
                                            parsingObject.matchAddedVariant = true;
                                            LoadHostVariantAssets(parsingResults, parsingObject, targetGuids);
                                        }
                                    }
                                }

                                if (parsingObject.variantModification.addGameObjectReferenceIds != null)
                                {
                                    foreach (var referenceObjectId in parsingObject.variantModification.addGameObjectReferenceIds)
                                    {
                                        if (!TryGetObject(referenceObjectId, out var referenceObject))
                                        { continue; }
                                        if (string.IsNullOrEmpty(referenceObject.guid))
                                        { continue; }
                                        if (TryMarkParsingObject(parsingResults, referenceObject, targetGuid, targetGuids))
                                        {
                                            parsingObject.matchAddedVariant = true;
                                            LoadHostVariantAssets(parsingResults, parsingObject, targetGuids);
                                        }
                                    }
                                }
                            }

                            TryMarkParsingObject(parsingResults, parsingObject, targetGuid, targetGuids);
                        }
                    }

                    if (parsingObject.typeId == TYPEID_SCENEROOTS)
                    {
                        foreach (var root in parsingObject.childs)
                        {
                            var rootObject = objectTable[root];
                            rootObject.isRoot = true;
                            AddRoot(rootObject);
                        }
                    }
                    else if (IsTransform(parsingObject.typeId))
                    {
                        if (parsingObject.childs != null && parsingObject.fatherId == 0)
                        {
                            AddRoot(parsingObject);
                        }
                        else if (parsingObject.childs == null && parsingObject.fatherId == 0 && parsingObject.prefabInstaceId == 0 && string.IsNullOrEmpty(parsingObject.guid))
                        {
                            AddRoot(parsingObject);
                        }
                        continue;
                    }
                    else if (parsingObject.typeId == TYPEID_PREFABINSTANCE)
                    {
                        if (parsingObject.transformParent != 0)
                        { continue; }

                        if (parsingObject.variantModification != null && parsingObject.variantModification.addGameObjectReferenceIds != null)
                        {
                            foreach (var fileId in parsingObject.variantModification.addGameObjectReferenceIds)
                            {
                                referencedVariants.Add(fileId);
                                if (objectTable.TryGetValue(fileId, out var referencedObject))
                                {
                                    referencedObject.referencedByVariant = parsingObject.objectId;
                                    roots.Remove(referencedObject);
                                }
                            }
                        }

                        if (referencedVariants.Contains(parsingObject.objectId))
                        {
                            continue;
                        }
                        else
                        {
                            AddRoot(parsingObject);
                        }
                    }
                }
            }

            public void BuildHierarchys(ParsingResults parsingResults, List<List<string>> foundHierarchys)
            {
                foreach (var rootObject in roots)
                {
                    var currentPath = new List<string>();
                    var assetInstancePath = new List<ParsingObject>();
                    BuildHierarchyNames(parsingResults, null, rootObject, currentPath, assetInstancePath, foundHierarchys);
                }
            }

            private void BuildHierarchyNames(ParsingResults parsingResults, ParsingObject srcObject, ParsingObject currentObject,
                                             List<string> currentPath, List<ParsingObject> assetInstacePath, List<List<string>> foundHierarchys)
            {
                if (currentObject.typeId == TYPEID_GAMEOBJECT)
                {
                    if (currentObject.components != null)
                    {
                        foreach (var compId in currentObject.components)
                        {
                            if (!TryGetObject(compId, out var component))
                            { continue; }
                            if (IsTransform(component.typeId))
                            { continue; }

                            if (component.matchSelf)
                            {
                                var newHierarchy = currentPath.ToList();
                                var realName = DecideName(parsingResults, assetInstacePath, currentObject);
                                newHierarchy.Add(realName);
                                foundHierarchys.Add(newHierarchy);
                                break;
                            }
                        }
                        foreach (var compId in currentObject.components)
                        {
                            if (!TryGetObject(compId, out var component))
                            { continue; }

                            if (!IsTransform(component.typeId))
                            { continue; }

                            if (srcObject != null && IsTransform(srcObject.typeId)) //already checked
                            {
                                var realName = DecideName(parsingResults, assetInstacePath, currentObject);
                                currentPath.Add(realName);
                                return;
                            }
                            else
                            {
                                using (new PathListRestorer<string, ParsingObject>(currentPath, assetInstacePath))
                                {
                                    var realName = DecideName(parsingResults, assetInstacePath, currentObject);
                                    currentPath.Add(realName);
                                    BuildHierarchyNames(parsingResults, currentObject, component, currentPath, assetInstacePath, foundHierarchys);
                                }
                            }
                        }
                    }

                    if (currentObject.prefabInstaceId != 0 && currentObject.matchSelf)//for variant prefab
                    {
                        if (TryGetObject(currentObject.prefabInstaceId, out var prefabObject))
                        {
                            if (parsingResults.TryGetResult(currentObject.overrideSourceAsset.guid, out var overridePrefabResult))
                            {
                                if (overridePrefabResult.TryGetObject(currentObject.overrideSourceAsset.fileID, out var overrideGameObject))
                                {
                                    var newHierarchy = currentPath.ToList();
                                    newHierarchy.Add(overrideGameObject.name);
                                    foundHierarchys.Add(newHierarchy);
                                }
                            }
                        }
                    }
                }
                else if (IsTransform(currentObject.typeId))
                {
                    if (currentObject.gameObjectId != 0)
                    {
                        if (srcObject != null && srcObject.typeId == TYPEID_GAMEOBJECT)//already checked
                        { return; }

                        if (!TryGetObject(currentObject.gameObjectId, out var gameObject))
                        { return; }

                        BuildHierarchyNames(parsingResults, currentObject, gameObject, currentPath, assetInstacePath, foundHierarchys);
                    }
                    else if (currentObject.prefabInstaceId != 0)
                    {
                        if (!TryGetObject(currentObject.prefabInstaceId, out var prefabInstance))
                        { return; }
                        BuildHierarchyNames(parsingResults, currentObject, prefabInstance, currentPath, assetInstacePath, foundHierarchys);
                    }

                    if (currentObject.childs != null)
                    {
                        foreach (var childId in currentObject.childs)
                        {
                            if (!TryGetObject(childId, out var child))
                            { continue; }

                            using (new PathListRestorer<string, ParsingObject>(currentPath, assetInstacePath))
                            {
                                BuildHierarchyNames(parsingResults, currentObject, child, currentPath, assetInstacePath, foundHierarchys);
                            }
                        }
                    }
                }
                else if (currentObject.typeId == TYPEID_PREFABINSTANCE)
                {
                    if (currentObject.variantModification != null)
                    {
                        if (currentObject.variantModification.addComponentReferenceIds != null)
                        {
                            foreach (var referenceId in currentObject.variantModification.addComponentReferenceIds)
                            {
                                if (!TryGetObject(referenceId, out var referenceObject))
                                { continue; }
                                if (!referenceObject.matchSelf)
                                { continue; }
                                if (!TryGetObject(referenceObject.gameObjectId, out var parsingGameObject))
                                { continue; }

                                parsingGameObject.matchSelf = true;// for variant prefab
                                using (new PathListRestorer<string, ParsingObject>(currentPath, assetInstacePath))
                                {
                                    var realName = DecideName(parsingResults, assetInstacePath, currentObject);
                                    currentPath.Add(realName);
                                    if (parsingResults.TryGetResult(currentObject.guid, out var childResults))
                                    {
                                        var childRootObject = childResults.roots.First();
                                        var childRootFileID = childRootObject.objectId;
                                        if (IsTransform(childRootObject.typeId))
                                        { childRootFileID = childRootObject.gameObjectId; }
                                        //component just on top of currentObject, go on
                                        if (parsingGameObject.overrideSourceAsset.fileID == childRootFileID)
                                        {
                                            foundHierarchys.Add(currentPath.ToList());
                                            continue;
                                        }
                                    }
                                    BuildHierarchyNames(parsingResults, currentObject, parsingGameObject, currentPath, assetInstacePath, foundHierarchys);
                                }
                            }
                        }
                        if (currentObject.variantModification.addGameObjectReferenceIds != null)
                        {
                            foreach (var referenceId in currentObject.variantModification.addGameObjectReferenceIds)
                            {
                                if (!TryGetObject(referenceId, out var referenceObject))
                                { continue; }
                                if (!referenceObject.matchSelf || !referenceObject.matchInChilds)
                                {
                                    using (new PathListRestorer<string, ParsingObject>(currentPath, assetInstacePath))
                                    {
                                        var realName = DecideName(parsingResults, assetInstacePath, currentObject);
                                        currentPath.Add(realName);
                                        BuildHierarchyNames(parsingResults, currentObject, referenceObject, currentPath, assetInstacePath, foundHierarchys);
                                    }
                                }
                            }
                        }
                    }

                    if (currentObject.matchSelf)
                    {
                        var realName = DecideName(parsingResults, assetInstacePath, currentObject);
                        currentPath.Add(realName);
                        var newHierarchy = currentPath.ToList();
                        foundHierarchys.Add(newHierarchy);
                    }

                    if (currentObject.matchInChilds)
                    {
                        if (string.IsNullOrEmpty(currentObject.guid))
                        { return; }
                        if (!parsingResults.TryGetResult(currentObject.guid, out var childResult))
                        { return; }
                        if (childResult.roots.Count == 0)
                        { return; }

                        using (new PathListRestorer<string, ParsingObject>(currentPath, assetInstacePath))
                        {
                            assetInstacePath.Add(currentObject);
                            childResult.BuildHierarchyNames(parsingResults, currentObject, childResult.roots.First(), currentPath, assetInstacePath, foundHierarchys);
                        }
                    }


                }
            }

            private string DecideName(ParsingResults parsingResults, List<ParsingObject> assetInstacePath, ParsingObject currentObject)
            {
                if (IsTransform(currentObject.typeId))
                {
                    var debugMsg = string.Format("unaccepted object type. objectId:{0}  typeId: {1}", currentObject.objectId, currentObject.typeId);
                    UnityEngine.Debug.LogError(debugMsg);
                    return debugMsg;
                }
                string decideName = currentObject.typeId == TYPEID_PREFABINSTANCE ? currentObject.prefabFileName : currentObject.name;
                long fileID = currentObject.objectId;
                if (currentObject.overrideNames != null)
                {
                    if (!string.IsNullOrEmpty(currentObject.guid))
                    {
                        if (parsingResults.TryGetResult(currentObject.guid, out var childResults))
                        {
                            var childRoot = childResults.roots.First();
                            if (childRoot.typeId == TYPEID_PREFABINSTANCE)
                            {
                                if (childRoot.overrideNames != null)
                                {
                                    foreach (var childRootOverrideElement in childRoot.overrideNames)
                                    {
                                        var childRootFileID = (childRootOverrideElement.target.fileID ^ childRoot.objectId) & 0x7fffffffffffffff;
                                        if (currentObject.TryGetOverrideName(childRootFileID, out var overrideElement))
                                        {
                                            decideName = overrideElement.propertyValue;
                                            fileID = (overrideElement.target.fileID ^ fileID) & 0x7fffffffffffffff;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                var childRootFileID = IsTransform(childRoot.typeId) ? childRoot.gameObjectId : childRoot.objectId;
                                if (currentObject.TryGetOverrideName(childRootFileID, out var overrideElement))
                                {
                                    decideName = overrideElement.propertyValue;
                                    fileID = (overrideElement.target.fileID ^ fileID) & 0x7fffffffffffffff;
                                }
                            }

                        }
                    }
                }

                for (int i = assetInstacePath.Count - 1; i >= 0; i--)
                {
                    var parentAssetInstance = assetInstacePath[i];
                    if (parentAssetInstance.TryGetOverrideName(fileID, out var ele))
                    {
                        decideName = ele.propertyValue;
                    }
                    fileID = (parentAssetInstance.objectId ^ fileID) & 0x7fffffffffffffff;
                }
                return decideName;
            }
        }

        public class ParsingResults
        {
            private Dictionary<string, OneParsingResult> assetTable = new Dictionary<string, OneParsingResult>();
            private Dictionary<string, ParsingResults> subSceneResults = new Dictionary<string, ParsingResults>();

            public List<List<string>> hierarchys = new List<List<string>>();

            public readonly string assetGuid;
            public readonly string assetPath;

            public ParsingResults(string path)
            {
                assetPath = path;
                assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public OneParsingResult GetOrCreateResult(string guid, string path)
            {
                if (assetTable.TryGetValue(guid, out var oneResult))
                { return oneResult; }
                oneResult = new OneParsingResult();
                oneResult.assetGuid = guid;
                oneResult.assetPath = path;
                assetTable[guid] = oneResult;
                return oneResult;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetResult(string guid, out OneParsingResult oneResult)
            {
                return assetTable.TryGetValue(guid, out oneResult);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddResult(string guid, OneParsingResult oneResult)
            {
                assetTable[guid] = oneResult;
            }

            public ParsingResults GetOrCreateSubSceneResult(string subSceneGuid, out ParsingResults parsingResult)
            {
                if(!subSceneResults.TryGetValue(subSceneGuid, out parsingResult))
                {
                    var subScenePath = AssetDatabase.GUIDToAssetPath(subSceneGuid);
                    parsingResult = new ParsingResults(subScenePath);
                    subSceneResults.Add(subSceneGuid, parsingResult);
                }
                return parsingResult;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetSubSceneResult(string subSceneGuid, out ParsingResults result)
            {
                return subSceneResults.TryGetValue(subSceneGuid, out result);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddSubSceneResult(string subSceneGuid, ParsingResults result)
            {
                subSceneResults[subSceneGuid] = result;
            }

            public List<string> FindHierarchys(ICollection<string> targetGuids)
            {
                var oneResult = GetOrCreateResult(assetGuid, assetPath);
                oneResult.ParseTargetAssetText(this, targetGuids);
                oneResult.PrepareInfo(this, targetGuids);
                oneResult.BuildHierarchys(this, hierarchys);

                var pathList = hierarchys.Select(x => string.Join(PATH_SPLIT, x)).ToList();

                foreach (var kv in subSceneResults)
                {
                    var subSceneResult = kv.Value;
                    var subHierarchys = subSceneResult.FindHierarchys(targetGuids);
                    if(subHierarchys.Count > 0)
                    {
                        var subScenePathInHostResult = new ParsingResults(assetPath);
                        var subScenePathHostHierarchys = subScenePathInHostResult.FindHierarchys(new HashSet<string> { subSceneResult.assetGuid });
                        foreach(var hostPath in subScenePathHostHierarchys)
                        {
                            foreach(var subPath in subHierarchys)
                            {
                                var combinePath = string.Format("%%%{0}%%%{1}%%%{2}", subSceneResult.assetGuid, string.Join(PATH_SPLIT, hostPath), string.Join(PATH_SPLIT, subPath));
                                pathList.Add(combinePath);
                            }
                        }
                    }
                }

                return pathList;
            }
        }

        public const string AREA_SPLIT = QuickFinder.Engine.Utility.HierarchyUtility.AREA_SPLIT;
        public const string PATH_SPLIT = QuickFinder.Engine.Utility.HierarchyUtility.PATH_SPLIT;

        const long TYPEID_GAMEOBJECT = 1;
        const long TYPEID_TRANSFORM = 4;
        const long TYPEID_RECTTRANSFORM = 224;
        const long TYPEID_PREFABINSTANCE = 1001;
        const long TYPEID_MONOBEHAVIOUR = 114;
        const long TYPEID_SCENEROOTS = 1660057539;
        public static bool IsTransform(long typeid) { return typeid == TYPEID_TRANSFORM || typeid == TYPEID_RECTTRANSFORM; }

        public List<string> FindHierarchys(string hostPath, ICollection<string> targetGuids)
        {
            var parsingResults = new ParsingResults(hostPath);
            var hierarchys = parsingResults.FindHierarchys(targetGuids);

            return hierarchys;
        }
    }

    public struct PathListRestorer<TPath1> : System.IDisposable
    {
        bool disposed;
        List<TPath1> path1;
        int restoreIndex1;

        public PathListRestorer(List<TPath1> path_)
        {
            disposed = false;
            path1 = path_;
            restoreIndex1 = path_ == null ? 0 : path_.Count;
        }

        /// <summary>
        ///  Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        // Protected implementation of Dispose pattern.
        void Dispose(bool disposing)
        {
            if (disposed)
                return;

            // As this is a struct, it could have been initialized using an empty constructor so we
            // need to make sure `cmd` isn't null to avoid a crash. Switching to a class would fix
            // this but will generate garbage on every frame (and this struct is used quite a lot).
            if (disposing)
            {
                if (path1 != null)
                {
                    path1.RemoveRange(restoreIndex1, path1.Count - restoreIndex1);
                }
            }

            disposed = true;
        }
    }

    public struct PathListRestorer<TPath1, TPath2> : System.IDisposable
    {
        bool disposed;
        List<TPath1> path1;
        int restoreIndex1;
        List<TPath2> path2;
        int restoreIndex2;

        public PathListRestorer(List<TPath1> path1_, List<TPath2> path2_)
        {
            disposed = false;
            path1 = path1_;
            restoreIndex1 = path1_ == null ? 0 : path1_.Count;
            path2 = path2_;
            restoreIndex2 = path2_ == null ? 0 : path2_.Count;
        }

        /// <summary>
        ///  Dispose pattern implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        // Protected implementation of Dispose pattern.
        void Dispose(bool disposing)
        {
            if (disposed)
                return;

            // As this is a struct, it could have been initialized using an empty constructor so we
            // need to make sure `cmd` isn't null to avoid a crash. Switching to a class would fix
            // this but will generate garbage on every frame (and this struct is used quite a lot).
            if (disposing)
            {
                if (path1 != null)
                {
                    path1.RemoveRange(restoreIndex1, path1.Count - restoreIndex1);
                }
                if (path2 != null)
                {
                    path2.RemoveRange(restoreIndex2, path2.Count - restoreIndex2);
                }
            }

            disposed = true;
        }
    }
}
