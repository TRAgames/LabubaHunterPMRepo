//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickFinder.Editor
{
    public class AssetField : FieldTemplateUIItem<string>
    {
        private Image icon;
        private TextField pathField;
        private ButtonList externalButtonBar;
        public Action<string, int> onSelected;
        public const string PATH_SPLIT = QuickFinder.Engine.Utility.HierarchyUtility.PATH_SPLIT;
        public const string AREA_SPLIT = QuickFinder.Engine.Utility.HierarchyUtility.AREA_SPLIT;

        public bool isSceneObject { get; set; } = false;

        private string _value;
        public override string Value 
        { 
            get { return _value; }
            set 
            {  
                if(value == _value)
                { return; }
                _value = value;
                Refresh(_value);
            }
        }

        private bool showFileNameOnly = false;
        public bool ShowFileNameOnly 
        {
            get { return showFileNameOnly; } 
            set 
            { 
                if(showFileNameOnly == value)
                { return; }
                showFileNameOnly = value;
                _ShowFileNameOnly(value);
            } 
        }

        private void _ShowFileNameOnly(bool showFileNameOnly_)
        {
            if(!showFileNameOnly_)
            {
                if (pathField != null && _value != null)
                {
                    QuickFinder.Engine.Utility.HierarchyUtility.ParsePath(_value, out var subSceneGuid, out var hostPath, out var subPath);
                    string relacePath;
                    if (string.IsNullOrEmpty(subSceneGuid))
                    {
                        relacePath = _value.Replace(PATH_SPLIT, "/");
                    }
                    else
                    {
                        var subScenePath = AssetDatabase.GUIDToAssetPath(subSceneGuid);
                        subScenePath = System.IO.Path.GetFileNameWithoutExtension(subScenePath);
                        var finalFileIndex = hostPath.LastIndexOf(PATH_SPLIT);
                        if(finalFileIndex >= 0)
                        {
                            hostPath = hostPath.Substring(0, finalFileIndex + PATH_SPLIT.Length) + "¡¾" + subScenePath + "¡¿"; ;
                        }
                        else
                        {

                            hostPath = "¡¾" + subScenePath + "¡¿";
                        }
                        relacePath = string.Format("{0}/{1}", hostPath.Replace(PATH_SPLIT, "/"), subPath.Replace(PATH_SPLIT, "/"));
                    }
                    pathField.value = relacePath;
                    //var splitIndex = relacePath.LastIndexOf("/");
                    //if (splitIndex < 0)
                    //{ return; }

                    //pathField.value = relacePath.Substring(splitIndex + 1);
                }
            }
            else
            {
                if (pathField != null && _value != null)
                {
                    QuickFinder.Engine.Utility.HierarchyUtility.ParsePath(_value, out var subSceneGuid, out var hostPath, out var subPath);
                    var finalFileIndex = subPath.LastIndexOf(PATH_SPLIT);
                    if (finalFileIndex >= 0)
                    { pathField.value = System.IO.Path.GetFileNameWithoutExtension(subPath.Substring(finalFileIndex + PATH_SPLIT.Length)); }
                    else
                    { pathField.value = System.IO.Path.GetFileNameWithoutExtension(subPath); }
                }

            }
        }

        public AssetField()
        {
            this.style.flexDirection = FlexDirection.Row;

            icon = new Image
            {
                scaleMode = ScaleMode.ScaleAndCrop,
                pickingMode = PickingMode.Ignore
            };
            icon.style.flexShrink = 0f;
            this.Add(icon);

            externalButtonBar = new ButtonList();
            externalButtonBar.style.flexShrink = 0f;
            this.Add(externalButtonBar);

            pathField = new TextField();
            pathField.style.flexGrow = 1f;
            this.Add(pathField);
#if UNITY_6000_0_OR_NEWER
            pathField.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
#else
            pathField.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
#endif

            SetIconSize(22);
        }

        protected override void Refresh(string  assetPath)
        {
            _value = assetPath;
            if(!isSceneObject)
            {
                var iconName = UnityIconUtility.GetBuildInIconNameByFileNameWithExt(assetPath);
                icon.image = EditorGUIUtility.IconContent(iconName).image;
            }
            else
            {
                icon.image = EditorGUIUtility.IconContent("GameObject Icon").image;
            }
            pathField.value = assetPath;
            pathField.isReadOnly = true;
#if UNITY_6000_0_OR_NEWER
            pathField.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
#else
            pathField.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
#endif

            _ShowFileNameOnly(showFileNameOnly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonNames"></param>
        /// <param name="onExternlButtonClick"></param>
        public void BuildExternalButtonBar(List<string> buttonNames, Action<string, int, string, int> onExternlButtonClick, List<string> tooltips = null)
        {
            externalButtonBar.Build("", buttonNames, (btnNmae, btnIndex) => 
            {
                onExternlButtonClick?.Invoke(btnNmae, btnIndex, Value, Index);
            }, tooltips);
        }

        private void OnExternalButtonBarClicked(string btnName, int btnIndex)
        {

        }

        public void SetIcon(string iconName)
        {
            icon.image = EditorGUIUtility.IconContent(iconName).image;
        }

        public void SetIconSize(int size)
        {
            icon.style.width = size;
            icon.style.height = size;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            onSelected?.Invoke(_value, Index);
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            onSelected?.Invoke(_value, Index);
        }
    }

    public static class UnityIconUtility
    {
        private static readonly Dictionary<string, string> ext2IconMap;
        static UnityIconUtility()
        {
            ext2IconMap = new Dictionary<string, string>
            {
                {"default",  "d_DefaultAsset Icon" },

                //{"asset", "ScriptableObject Icon" },

                {"controller",  "AnimatorController Icon" },
                {"overrideController",  "AnimatorOverrideController Icon" },
                {"mask", "AvatarMask Icon" },
                {"anim", "AnimatorState Icon" },
                {"timeline", "TimelineAsset Icon" },
                {"signal", "d_SignalAsset Icon" },

                {"lighting", "LightmapParameters Icon" },
                {"flare", "LensFlare Icon" },

                {"renderTexture", "RenderTexture Icon" },
                {"spriteatlas", "d_SpriteAtlas Icon" },
                {"sprite", "d_Sprite Icon" },
                {"png", "Texture Icon" },
                {"tga", "Texture Icon" },
                {"jpg", "Texture Icon" },
                {"bmp", "Texture Icon" },

                {"material", "d_Material Icon" },
                {"mat", "d_Material Icon" },

                {"shader", "d_Shader Icon" },
                {"shadervariants", "d_ShaderVariantCollection Icon" },
                {"compute", "d_ComputeShader Icon" },
                {"raytrace", "d_RayTracingShader Icon" },

                {"preset", "Preset Icon" },

                {"mixer", "d_AudioMixerController Icon" },

                {"wav", "d_AudioSource Icon" },
                {"mp3", "d_AudioSource Icon" },
                {"ogg", "d_AudioSource Icon" },
                {"flac", "d_AudioSource Icon" },
                {"aac", "d_AudioSource Icon" },

                {"unity", "d_SceneAsset Icon" },
                {"scenetemplate", "d_SelectionListTemplate Icon" },

                {"fbx", "PrefabModel Icon" },
                {"FBX", "PrefabModel Icon" },

                {"prefab", "d_Prefab Icon" },

                {"txt", "Text Icon" },
                {"md", "Text Icon" },
                {"json", "Text Icon" },

                {"terrain", "d_Terrain Icon" },
                {"brush", "GridBrush Icon" },
                {"terrainlayer", "SortingGroup Icon" },
                //{"exr", "ReflectionProbe Icon" },

                {"physicMaterial", "PhysicMaterial Icon" },
                {"physicMaterial2D", "PhysicsMaterial2D Icon" },

                {"fontsettings", "Font Icon" },

                {"cs", "cs Script Icon" },

                {"folder", "Folder Icon" },

                {"guiskin", "GUISkin Icon" },
            };
        }

        public static string GetBuildinIconNameByExt(string  ext)
        {
            if(ext2IconMap.TryGetValue(ext, out var iconName))
                return iconName;
            return null;
        }

        private static Regex lightingDataRegex = new Regex(@".*lightdingData.*\.asset", RegexOptions.IgnoreCase);
        private static Regex terrainDataRegex = new Regex(@".*terrainData.*\.asset", RegexOptions.IgnoreCase);
        public static string GetBuildInIconNameByFileNameWithExt(string fileName)
        {
            var dotIndex = fileName.LastIndexOf('.');
            if(dotIndex == -1)
            { 
                if(System.IO.Directory.Exists(fileName))
                {
                    return GetBuildinIconNameByExt("folder");
                }
            }

            var ext = fileName.Substring(dotIndex + 1);
            var iconName = GetBuildinIconNameByExt(ext);
            if (iconName != null)
            { return iconName; }

            if(lightingDataRegex.IsMatch(fileName))
            { return "LightingDataAsset Icon"; }

            if (lightingDataRegex.IsMatch(fileName))
            { return "TerrainData Icon"; }

            if (fileName.EndsWith(".asset"))
            {
                return "ScriptableObject Icon";
            }

            if(fileName.EndsWith(".exr"))
            {
                if(fileName.Contains("Reflection"))
                { return "Cubemap Icon"; }
                else { return "Texture Icon"; }
            }

            return GetBuildinIconNameByExt("default");
        }
    }
}


