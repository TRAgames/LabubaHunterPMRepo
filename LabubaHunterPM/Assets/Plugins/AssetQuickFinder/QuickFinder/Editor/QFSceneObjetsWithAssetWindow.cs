//------------------------------------------------------------//
// yanfei 2025.03.05
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using QuickFinder.Assets;
using QuickFinder.Editor;
using QuickFinder.Engine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using QuickFinder.Assets;

namespace QuickFinder.Editor
{
    public class QFSceneObjetsWithAssetWindow : VisualElement, INaviTabWindow
    {
        public class Args
        {
            public List<string> assetList;
            public List<string> resultList;
            public string stagePath;
        }

        WindowArgHistory history = new WindowArgHistory();

        const string AREA_SPLIT = AssetInSceneSearcher.AREA_SPLIT;
        const string PATH_SPLIT = AssetInSceneSearcher.PATH_SPLIT;

        VisualElement mainView;
        AssetTemplateListView inputListView;
        FieldTemplateListView<string, AssetField> resultListView;

        #region INaviTabWindow
        public void Show(System.Object obj)
        {
            if (mainView == null)
            {
                mainView = new VisualElement();
                mainView.style.flexGrow = 1f;
                mainView.style.flexShrink = 0f;
                mainView.style.flexDirection = FlexDirection.Column;
                this.Add(mainView);

                inputListView = new AssetTemplateListView();
                inputListView.style.maxHeight = 420;
                mainView.Add(inputListView);

                var spaceLabel = new Label();
                spaceLabel.text = " ";
                mainView.Add(spaceLabel);

                resultListView = new FieldTemplateListView<string, AssetField>();
                this.Add(resultListView);
            }

            var arg = (obj as Args);
            if(arg == null)
            {
                arg = history.ToLatest() as Args;
                if(arg == null)
                {
                    if(Selection.assetGUIDs.Length <= 0)
                    {
                        inputListView.Build("searching assets:", new List<string> { }, null, null);
                        return;
                    }
                    
                    arg = new Args { assetList = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList() };
                    history.Enqueue(arg);
                }
            }
            else
            {
                if(arg != history.Latest())
                    history.Enqueue(arg);
            }

            inputListView.Build("searching assets:", arg.assetList, null, null);

            FindAndSaveTargetAssetHierarchy(arg);
            resultListView.Build("scene objects with assets found", MakeSceneObjectField, arg.resultList);
        }

        public VisualElement GetRootView()
        {
            return this;
        }
        #endregion

        private void OnAssetExternalButtonClicked(string btnName, int btnIndex, string value, int valueIndex)
        {
            if (btnName == "R")
            {

            }
        }

        private void FindAndSaveTargetAssetHierarchy(Args arg)
        {
            var hostPath = GetCurrentSceneOrStagePath();
            arg.stagePath = hostPath;
            arg.resultList = new List<string>();
            var hostGuid = AssetDatabase.AssetPathToGUID(hostPath);
            var assetList = FileEditorUtility.GetAssetsInList(arg.assetList);
            assetList = assetList.Select(x => AssetDatabase.AssetPathToGUID(x)).ToList();
            var searchAssets = assetList.TakeWhile(x => AssetServer.Instance.HasDependency(hostGuid, x, true)).ToList();
            if (searchAssets.Count == 0)
            { return; }
            var sceneObjectSearcher = new AssetInSceneSearcher();
            var targetHierarchys = sceneObjectSearcher.FindHierarchys(hostPath, searchAssets);
            if (targetHierarchys.Count == 0)
            { return; }
            var hierarchys = targetHierarchys.ToList();
            arg.resultList = hierarchys;

            return;
        }

        AssetField MakeSceneObjectField()
        {
            var assetField = new AssetField();
            assetField.SetIconSize(22);
            assetField.onSelected = OnSceneObjectSelected;
            assetField.isSceneObject = true;
            return assetField;
        }

        public static string GetCurrentSceneOrStagePath()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            string hostPath;
            if (prefabStage != null)
            {
                hostPath = prefabStage.assetPath;
            }
            else
            {
                var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                hostPath = scene.path;
            }
            return hostPath;
        }

        private void OnSceneObjectSelected(string hierarchyPath, int listIndex)
        {
            var arg = history.Current<Args>();
            var currentHostPath = GetCurrentSceneOrStagePath();
            if (arg.stagePath != currentHostPath)
            {
                if (EditorUtility.DisplayDialog("", "scene has changed, refresh curent data.", "OK"))
                {
                    Show(arg);
                    return;
                }
            }

            string findPath;
            QuickFinder.Engine.Utility.HierarchyUtility.ParsePath(hierarchyPath, out var subSceneGuid, out var hostPath, out var subPath);
            if (string.IsNullOrEmpty(subSceneGuid))
            {
                findPath = subPath;
            }
            else
            {
                findPath = subPath;  //string.Format("{0}&&&{1}", hostPath, subPath);
            }

            if (string.IsNullOrEmpty(subSceneGuid))
            {
                var allHierarchyPathList = history.Current<Args>().resultList.Where(x => !x.StartsWith(AREA_SPLIT)).ToList();
                int sameIndex = -1;
                for (int i = 0; i <= listIndex; i++)
                {
                    if (allHierarchyPathList[i] == hierarchyPath)
                    { sameIndex++; }
                }
                if (sameIndex < 0)
                { return; }

                var hierarchys = findPath.Split(PATH_SPLIT, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                if (hierarchys == null || hierarchys.Count == 0)
                { return; }

                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    var targets = new List<Transform>();
                    HierarchyUtility.FindTransforms(prefabStage.prefabContentsRoot.transform, hierarchys, targets);
                    if (targets.Count <= sameIndex)
                    {
                        Debug.LogError(hierarchyPath + ":  some of transform hierarchy missing. choose the first one");
                        if (targets.Count > 0)
                        { EditorGUIUtility.PingObject(targets[0]); }
                        return;
                    }

                    EditorGUIUtility.PingObject(targets[sameIndex]);
                }
                else
                {
                    var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                    var roots = scene.GetRootGameObjects();
                    var targets = new List<Transform>();
                    for (int i = 0; i < scene.rootCount; i++)
                    {
                        HierarchyUtility.FindTransforms(roots[i].transform, hierarchys, targets);
                    }
                    if (targets.Count <= sameIndex)
                    {
                        Debug.LogError(hierarchyPath + ":  some of transform hierarchy missing. choose the first one");
                        if (targets.Count > 0)
                        { EditorGUIUtility.PingObject(targets[0]); }
                        return;
                    }

                    EditorGUIUtility.PingObject(targets[sameIndex]);
                }
            }
            else
            {
                var subScenePathHead = AREA_SPLIT + subSceneGuid;
                var allHierarchyPathList = history.Current<Args>().resultList.Where(x => x.StartsWith(subScenePathHead)).ToList();
                int sameIndex = -1;
                for (int i = 0; i <= listIndex; i++)
                {
                    if (allHierarchyPathList[i] == hierarchyPath)
                    { sameIndex++; }
                }
                if (sameIndex < 0)
                { return; }

                var targetSubScenePath = AssetDatabase.GUIDToAssetPath(subSceneGuid);
                UnityEngine.SceneManagement.Scene scene = default;
                bool foundSubScene = false;
                for (int iscene = 0; iscene < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; iscene++)
                {
                    scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(iscene);

                    if (targetSubScenePath != scene.path)
                    { continue; }

                    foundSubScene = true;
                    break;
                }

                if (!foundSubScene)
                {
                    var sceneName = System.IO.Path.GetFileNameWithoutExtension(targetSubScenePath);
                    EditorUtility.DisplayDialog("", string.Format("load the subscene¡¾{0}¡¿ first please.", sceneName), "OK");
                    return;
                }

                var hierarchys = findPath.Split(PATH_SPLIT, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                if (hierarchys == null || hierarchys.Count == 0)
                { return; }

                var roots = scene.GetRootGameObjects();
                var targets = new List<Transform>();
                for (int i = 0; i < scene.rootCount; i++)
                {
                    HierarchyUtility.FindTransforms(roots[i].transform, hierarchys, targets);
                }
                if (targets.Count <= sameIndex)
                {
                    Debug.LogError(hierarchyPath + ":  some of transform hierarchy missing. choose the first one");
                    if (targets.Count > 0)
                    { EditorGUIUtility.PingObject(targets[0]); }
                    return;
                }

                EditorGUIUtility.PingObject(targets[sameIndex]);
            }

        }
    }
}

