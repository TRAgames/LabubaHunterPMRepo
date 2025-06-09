//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using QuickFinder.Editor;
using QuickFinder.Assets;

namespace QuickFinder.Editor
{
    public class QFMainWindow : EditorWindow
    {
        NaviPathHelper tabPathHelper;
        private NaviVerticalPad navigateView;
        private VisualElement tabWindowArea;
        private Dictionary<string, INaviTabWindow> tabWindows = new Dictionary<string, INaviTabWindow>();

        private void Init()
        {
            if(navigateView != null)
            { return; }

            tabPathHelper = new NaviPathHelper();

            CreateTabWindowPath(tabPathHelper);

            rootVisualElement.style.flexDirection = FlexDirection.Row;

            var padBox = new Box();
            padBox.style.flexShrink = 0;
            padBox.style.maxWidth = 200;
            rootVisualElement.Add(padBox);
            navigateView = new NaviVerticalPad(tabPathHelper, OnTabCanged);
            padBox.Add(navigateView);

            tabWindowArea = new VisualElement();
            tabWindowArea.style.flexGrow = 1f;
            tabWindowArea.style.flexShrink = 0f;
            rootVisualElement.Add(tabWindowArea);
        }

        protected void CreateTabWindowPath(NaviPathHelper tabPathHelper)
        {
            tabPathHelper.Add<QFRelationsWindow>("Relations");
            tabPathHelper.Add<QFSceneObjetsWithAssetWindow>("SceneObjetsWithAsset");
            //tabPathHelper.Add<QFAssetPropSearchWindow>("AssetPropSearch");
            tabPathHelper.Add<QFSettingsWindow>("Settings");
        }

        public void OnGUI()
        {
            if (this.titleContent.text != "QFMainWindow")
            {
                Close();
            }
            else if(rootVisualElement.childCount == 0)
            {
                QFMainWindow.ShowMe("Relations", Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList());
            }
        }

        private void OnTabCanged(string path)
        {
            Show(path, null);
        }

        private INaviTabWindow CreateTabWindow(Type windowType)
        {
            var window = System.Activator.CreateInstance(windowType) as INaviTabWindow;

            return window;
        }

        private void Show(string path, System.Object arg)
        {
            Show();
            Init();
            var windowType = tabPathHelper.GetWindow(path);

            tabWindowArea.Clear();
            if (!tabWindows.TryGetValue(path, out var oneWindow))
            {
                oneWindow = CreateTabWindow(windowType);
                tabWindows.Add(path, oneWindow);
            }
            var oneWindowView = oneWindow.GetRootView();
            if(oneWindowView.parent != null)
            { 
                oneWindowView.parent.Remove(oneWindowView); 
            }
            tabWindowArea.Add(oneWindowView);

            oneWindow.Show(arg);

            navigateView.SetSelectionWithoutNotify(path);
        }

        private void Show<TWindow>(System.Object arg) where TWindow : INaviTabWindow
        {
            var path = tabPathHelper.GetPath(typeof(TWindow));
            if(path == null)
            { return; }
            Show(path, arg);
        }

        public static void ShowMe(string path, System.Object arg)
        {
            var window = GetWindow<QFMainWindow>("QFMainWindow");
            window.Show(path, arg);
        }

    }

    [InitializeOnLoad]
    public class QFMainWindowShortcut
    {
        static long shortcutKeyPressedTick = 0;
        static QFMainWindowShortcut()
        {
            EditorApplication.projectWindowItemOnGUI += OnShortcutKey;
        }

        [MenuItem("Assets/[Finder] Find Asset Owner", priority = 0)]
        public static void OpenRelationsWindow()
        {
            List<string> seletedAssets;
            if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length == 0)
            {
                seletedAssets = new List<string> { "Assets" };
            }
            else
            {
                seletedAssets = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
            }

            QFMainWindow.ShowMe("Relations", new QFRelationsWindow.Args { assetList = seletedAssets });
        }

        [MenuItem("Assets/[Finder] Find Asset In Scene", priority = 0)]
        public static void OpenSceneObjectsWithAssetWindow()
        {
            if (Selection.assetGUIDs == null || Selection.assetGUIDs.Length == 0)
            {
                return;
            }

            QFMainWindow.ShowMe("SceneObjetsWithAsset", new QFSceneObjetsWithAssetWindow.Args { assetList = Selection.assetGUIDs.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList() });
        }

        private static void OnShortcutKey(String guid, Rect rect)
        {
            var currentTick = DateTime.Now.Ticks;
            if(currentTick - shortcutKeyPressedTick > 2000000)
            {
                shortcutKeyPressedTick = currentTick;
                Event e = Event.current;
                if (QuickFinderSettings.IsKey2(QuickFinderSettings.FindAssetOwnersKeyString, e.keyCode))
                {
                    OpenSceneObjectsWithAssetWindow();
                }
                else if (QuickFinderSettings.IsKey2(QuickFinderSettings.FindSceneObjectWithAssetsKeyString, e.keyCode))
                {
                    OpenRelationsWindow();
                }
            }
        }
    }
}





