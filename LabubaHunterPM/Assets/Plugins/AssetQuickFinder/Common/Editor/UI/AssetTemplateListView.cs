//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using QuickFinder.Assets;
using QuickFinder.Editor;
using QuickFinder.Engine.Utility;
using QuickFinder.Text.YAML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static QuickFinder.Editor.AssetTemplateListView;

namespace QuickFinder.Editor
{
    public class AssetTemplateListView : VisualElement
    {
        ToggleList advancedOptionBar;
        FieldTemplateListView<string, AssetField> listView;

        List<string> paths = new List<string>();

        private Action<string, int, string, int> onAssetItemExternalButtonClicked;
        private List<string> assetExternButtonList;

        public AssetTemplateListView()
        {
            advancedOptionBar = new ToggleList();
            advancedOptionBar.style.flexShrink = 0f;
            advancedOptionBar.Build("", new List<string> { "show file name only", "sort by extension", "sort by file name only" }, OnAdvancedOptionClicked, null);
            advancedOptionBar.SetOption(0, false, true);

            listView = new FieldTemplateListView<string, AssetField>(advancedOptionBar);
            this.Add(listView);
        }

        public void Build(string title, List<string> elements)
        {
            this.paths.Clear();
            this.paths.AddRange(elements);
            //paths.Sort(FileEditorUtility.CompareExtensionThenFolderThenName);
            SortPathList();
            listView.Build(title, MakeAssetFieldItem, this.paths);
        }

        public void Build(string title, List<string> elements, Action<string, int, string, int> itemExternalButtonCallback, List<string> assetExternButtonList)
        {
            this.onAssetItemExternalButtonClicked = itemExternalButtonCallback;
            this.assetExternButtonList = assetExternButtonList;

            this.paths.Clear();
            this.paths.AddRange(elements);
            //paths.Sort(FileEditorUtility.CompareExtensionThenFolderThenName);
            SortPathList();
            listView.Build(title, MakeAssetFieldItem, this.paths);
        }

        private AssetField MakeAssetFieldItem()
        {
            var assetField = new AssetField();
            assetField.SetIconSize(22);
            assetField.ShowFileNameOnly = advancedOptionBar.IsOptionSelected(0);
            assetField.onSelected = OnAssetItemSelected;
            if(onAssetItemExternalButtonClicked != null)
            {
                assetField.BuildExternalButtonBar(assetExternButtonList, onAssetItemExternalButtonClicked, assetExternButtonList);
            }
            return assetField;
        }

        private void SortPathList()
        {
            if (advancedOptionBar.IsOptionSelected(1))
            {
                if (advancedOptionBar.IsOptionSelected(2))
                    paths.Sort(FileEditorUtility.CompareExtensionThenName);
                else
                    paths.Sort(FileEditorUtility.CompareExtensionThenFolderThenName);
            }
            else
            {
                if (advancedOptionBar.IsOptionSelected(2))
                    paths.Sort(FileEditorUtility.CompareNameWithoutExtension);
                else
                    paths.Sort();
            }
        }

        private void OnAssetItemSelected(string value_, int index)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(value_);
            if (asset == null)
            { return; }

            EditorGUIUtility.PingObject(asset);
        }

        private void OnAdvancedOptionClicked(string optionName, int btnIndex, bool selected)
        {
            if (btnIndex == 0)
            {
                listView.Refresh(paths);
                return;
            }

            SortPathList();
            listView.Refresh(paths);
        }
    }

}
