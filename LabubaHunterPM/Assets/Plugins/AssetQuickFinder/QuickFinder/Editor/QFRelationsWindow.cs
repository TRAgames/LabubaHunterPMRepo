//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using QuickFinder.Assets;
using QuickFinder.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    public class QFRelationsWindow : VisualElement, INaviTabWindow
    {
        public class Args
        {
            public List<string> assetList;
            public int relationType = 0; // 0: owner 1 dependency
        }
        private VisualElement mainView;
        private ButtonList historyArrowBar;
        private FieldTemplateListView<string, AssetField> inputListView;
        private HorizonalTabListBar resultTabBar;
        private Button advanceOptionsFolder;
        private Box advanceOptionsBox;
        private ToggleList searchOptionsBar;
        private AssetTemplateListView resultListView;
        private Label resultInfoLabel;


        WindowArgHistory history = new WindowArgHistory();
        private int selectedResultTabIndex = 0;

        #region INaviTabWindow
        public void Show(System.Object arg)
        {
            var passed = arg as Args;
            if (passed == null)
            {
                passed = history.Current<Args>();
                if (passed != null)
                { passed.relationType = selectedResultTabIndex; }
            }
            if (passed == null)
            {
                var assetList = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
                passed = new Args { assetList = assetList, relationType = selectedResultTabIndex };
            }

            if(history.Current<Args>() == null)
            {
                history.Enqueue(passed);
            }
            if(!passed.assetList.SequenceEqual(history.Current<Args>().assetList))
            {
                history.Enqueue(passed);
            }
            selectedResultTabIndex = history.Current<Args>().relationType;
            Build(passed.assetList);
        }

        public VisualElement GetRootView()
        {
            return this;
        }
        #endregion

        void Build(List<string> paths)
        {
            if (mainView == null)
            {
                mainView = new VisualElement();
                this.Add(mainView);
                mainView.style.flexShrink = 0f;
                mainView.style.flexDirection = FlexDirection.Column;

                Button rebuildButton = new Button();
                rebuildButton.text = "Refresh Database";
                rebuildButton.tooltip = "rebuild asset database, it will take a few seconds depend on your project and hardware.";
                rebuildButton.clicked += RebuildDatabase;
                mainView.Add(rebuildButton);

                Button importButton = new Button();
                importButton.text = "search for clipboard";
                importButton.tooltip = "import selected assets from system clipboard";
                importButton.clicked += ImportAssetsFromClipboard;
                mainView.Add(importButton);

                var inputArea = new VisualElement();
                inputArea.style.flexDirection = FlexDirection.Row;
                inputArea.style.flexShrink = 0f;
                this.Add(inputArea);

                historyArrowBar = new ButtonList();
                historyArrowBar.style.flexShrink = 0f;
                historyArrowBar.Build("", new List<string> { "<","X", ">" }, OnHistoryBarClicked);
                inputArea.Add(historyArrowBar);

                inputListView = new FieldTemplateListView<string, AssetField>();
                inputListView.style.maxHeight = 512;
                inputArea.Add(inputListView);

                resultTabBar = new HorizonalTabListBar();
                resultTabBar.style.flexShrink = 0f;
                this.Add(resultTabBar);

                //advanceOptionsFolder = new Button();
                //advanceOptionsFolder.style.flexShrink = 0f;
                //advanceOptionsFolder.text = "> click me to unfold advance options";
                //advanceOptionsFolder.clicked += OnAdvanceOptionsFoldClicked;
                //this.Add(advanceOptionsFolder);

                //advanceOptionsBox = new Box();
                //advanceOptionsBox.style.flexShrink = 0f;
                //this.Add(advanceOptionsBox);

                searchOptionsBar = new ToggleList();
                searchOptionsBar.style.flexShrink = 0f;
                searchOptionsBar.Build("", new List<string> { "include indirect reference", /*"include self"*/ }, OnSearchOptionsBarClick, new List<string> { "search as deep as possible: refeencies of reference, owners of owner", /*"add self to the result"*/ });
                searchOptionsBar.SetOption(0, true, true);
                this.Add(searchOptionsBar);

                //ShowElements(advanceOptionsBox, false);

                resultListView = new AssetTemplateListView();
                this.Add(resultListView);

                Box resultInfoBox = new Box();
                resultInfoBox.style.flexShrink = 0f;
                resultInfoBox.style.height = 22;
                this.Add(resultInfoBox);

                resultInfoLabel = new Label();
                resultInfoLabel.style.borderTopWidth = 4;
                resultInfoLabel.style.unityTextAlign = TextAnchor.LowerLeft;
                resultInfoLabel.style.color = new Color(0.2f, 0.8f, 0.1f);
                resultInfoBox.Add(resultInfoLabel);
            }

            inputListView.Build("selected assets", MakeAssetFieldItem, paths);

            resultTabBar.Build(new List<string> { "My Owners", "My Dependencies" }, OnHorizonalTabSwitched);//0 owners; 1 dependencies
            resultTabBar.SwitchTab(selectedResultTabIndex, true);
        }

        private AssetField MakeAssetFieldItem()
        {
            var assetField = new AssetField();
            assetField.SetIconSize(22);
            return assetField;
        }

        private void RebuildDatabase()
        {
            AssetServer.Instance.Rebuild();
        }

        private void ImportAssetsFromClipboard()
        {
            var paths = new HashSet<string>();
            var text = EditorGUIUtility.systemCopyBuffer.Replace('\\', '/');
            Regex guidRegex = new Regex(@"[0-9a-fA-F]{32}");
            var matches = guidRegex.Matches(text);
            foreach (Match match in matches)
            {
                var t = AssetDatabase.GUIDToAssetPath(match.Value);
                if(t != null)
                { 
                    if(System.IO.File.Exists(t) || System.IO.Directory.Exists(t))
                        paths.Add(t); 
                }
            }

            var pathRegex = new Regex(@"Assets/([^<>:""\/\\|?*\r\n]*/)*[^<>:""\/\\|?*\r\n]*(\.[^<>:""\/\\|?*\r\n]*)?");
            var lines = text.Split("\n");
            foreach (var line in lines)
            {
                matches = pathRegex.Matches(line);
                foreach (Match match in matches)
                {
                    if(System.IO.File.Exists (match.Value) || System.IO.Directory.Exists(match.Value))
                        paths.Add(match.Value);
                }
            }

            var arg = new Args { assetList = paths.ToList(), relationType = selectedResultTabIndex };
            history.Enqueue(arg);
            Show(arg);
        }

        private void OnHistoryBarClicked(string btnName, int btnIndex)
        {
            if(btnName == "<")
            {
                var selectedHistory = history.Backward<Args>();
                Build(selectedHistory.assetList);
            }
            else if(btnName == "X")
            {
                if(history.DeleteCurrent<Args>(out var currenHistory))
                {
                    Build(currenHistory.assetList);
                }
            }
            else if(btnName == ">")
            {
                var selectedHistory = history.Forward<Args>();
                Build(selectedHistory.assetList);
            }
        }

        private void OnHorizonalTabSwitched(string tabName, int tabIndex)
        {
            selectedResultTabIndex = tabIndex;
            string ownerOrDependency;
            List<string> resultList;
            if (tabIndex == 0)
            {
                ownerOrDependency = "Owners";
                var owners = new HashSet<string>();
                AssetServer.Instance.GetOwners(inputListView.ItemSource, owners, searchOptionsBar.IsOptionSelected(0));
                resultList = owners.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
            }
            else
            {
                ownerOrDependency = "Depnendencies";
                var dependencies = new HashSet<string>();
                AssetServer.Instance.GetDependencies(inputListView.ItemSource, dependencies, searchOptionsBar.IsOptionSelected(0));
                resultList = dependencies.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
            }
            resultListView.Build(ownerOrDependency, resultList, OnAssetExternalButtonClicked, new List<string> { "P", ">" });
            resultInfoLabel.text = string.Format(" {0} {1} found.    include indirect reference: {2}.", resultList.Count, ownerOrDependency, searchOptionsBar.IsOptionSelected(0));
        }

        private void OnAssetExternalButtonClicked(string btnName, int btnIndex, string value, int valueIndex)
        {
            if (btnName == ">")
            {
                if (value.EndsWith(".unity"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(value);
                }
                else
                {
                    var asset = AssetDatabase.LoadAllAssetsAtPath(value);
                    if (asset != null)
                    {
                        AssetDatabase.OpenAsset(asset);
                    }
                }
            }
            else if (btnName == "P")
            {
                var targetGuid = AssetDatabase.AssetPathToGUID(value);
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
                var hostGuid = AssetDatabase.AssetPathToGUID(hostPath);
                if (!AssetServer.Instance.HasDependency(hostGuid, targetGuid, true))
                { return; }

                var passingArgs = new QFSceneObjetsWithAssetWindow.Args { assetList = new List<string> { value } };
                QFMainWindow.ShowMe("SceneObjetsWithAsset", passingArgs);
            }
        }

        private void OnAdvanceOptionsFoldClicked()
        {
            if (advanceOptionsBox.style.visibility == Visibility.Visible)
            {
                ShowElements(advanceOptionsBox, false);
            }
            else
            {
                ShowElements(advanceOptionsBox, true);
            }
        }

        public void OnSearchOptionsBarClick(string btnName, int btnIndex, bool selected)
        {
            if (btnIndex == 0)
            {
                Show(null);
            }
        }

        private void ShowElements(VisualElement element, bool show)
        {
            element.style.visibility = show ? Visibility.Visible : Visibility.Hidden;
            foreach (var child in element.Children())
            {
                child.style.visibility = show ? Visibility.Visible : Visibility.Hidden;
            }

            element.style.height = show ? Length.Auto() : 0;
        }
    }
}
