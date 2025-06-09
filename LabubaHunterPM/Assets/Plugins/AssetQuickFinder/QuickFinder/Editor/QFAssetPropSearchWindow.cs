//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using QuickFinder.Assets;
using QuickFinder.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fei.Assets.QuickFinder
{
    public class QFAssetPropSearchWindow : VisualElement, INaviTabWindow
    {
        VisualElement mainView;

        public void Show(System.Object args)
        {
            if (mainView == null)
            {
                mainView = new VisualElement();
                mainView.style.flexGrow = 1f;
                mainView.style.flexShrink = 0f;
                mainView.style.flexDirection = FlexDirection.Column;
                this.Add(mainView);

            }
        }

        public VisualElement GetRootView()
        {
            return this;
        }
    }

    public static class ImporterReflections
    {
        static HashSet<Type> importerTypes = new HashSet<Type>();

        static ImporterReflections()
        {
            Add<TextureImporter>();
            //Add<ShaderImporter>();
            Add<ModelImporter>();
            Add<AudioImporter>();
            Add<IHVImageFormatImporter>();
            Add<SpeedTreeImporter>();
            Add<VideoClipImporter>();
            Add<TrueTypeFontImporter>();
        }

        public static void Add<TImporter>() where TImporter : AssetImporter
        {
            importerTypes.Add(typeof(TImporter));
        }
    }
}
