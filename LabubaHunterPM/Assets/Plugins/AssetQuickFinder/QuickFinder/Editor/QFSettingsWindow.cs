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

namespace QuickFinder.Editor
{
    public class QFSettingsWindow : VisualElement, INaviTabWindow
    {
        VisualElement mainView;


        ShortcutKeyTextField findAssetOwnerField;
        ShortcutKeyTextField findSceneObjectsWithAssetField;

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

                QuickFinderSettings.UnserializeKey(QuickFinderSettings.FindAssetOwnersKeyString, (int)KeyCode.LeftBracket);
                QuickFinderSettings.ReadKey(QuickFinderSettings.FindAssetOwnersKeyString, out _, out var key2);
                findAssetOwnerField = new ShortcutKeyTextField(QuickFinderSettings.FindAssetOwnersKeyString, KeyCode.None, key2);
                findAssetOwnerField.DisableKey1(true);
                findAssetOwnerField.onKey2Changed = (x) => 
                {
                    QuickFinderSettings.SetKey2(QuickFinderSettings.FindAssetOwnersKeyString, x);
                };
                this.Add(findAssetOwnerField);

                QuickFinderSettings.UnserializeKey(QuickFinderSettings.FindSceneObjectWithAssetsKeyString, (int)KeyCode.RightBracket);
                QuickFinderSettings.ReadKey(QuickFinderSettings.FindSceneObjectWithAssetsKeyString, out _, out key2);
                findSceneObjectsWithAssetField = new ShortcutKeyTextField(QuickFinderSettings.FindSceneObjectWithAssetsKeyString, KeyCode.None, key2);
                findSceneObjectsWithAssetField.DisableKey1(true);
                findSceneObjectsWithAssetField.onKey2Changed = (x) =>
                {
                    QuickFinderSettings.SetKey2(QuickFinderSettings.FindSceneObjectWithAssetsKeyString, x);
                };
                this.Add(findSceneObjectsWithAssetField);
            }
        }

        public VisualElement GetRootView()
        {
            return this;
        }
        #endregion
    }

    public class ShortcutKeyTextField : VisualElement
    {
        public KeyCode combindKey1 { get; private set; }
        public KeyCode combindKey2 { get; private set; }

        Label titleLabel;
        TextField keyField1;
        TextField keyField2;

        public System.Action<KeyCode> onKey1Changed;
        public System.Action<KeyCode> onKey2Changed;

        public ShortcutKeyTextField(string title, KeyCode combindKey1, KeyCode combindKey2)
        {
            this.style.flexDirection = FlexDirection.Row;
            titleLabel = new Label();
            titleLabel.text = title;
            titleLabel.style.minWidth = 200;
            this.Add(titleLabel);

            keyField1 = new TextField();
            keyField1.style.width = 100;
            keyField1.maxLength = 1;
            keyField1.SetValueWithoutNotify(KeyCode2String(combindKey1));
            keyField1.RegisterValueChangedCallback(OnKeyField1Changed);
            this.Add(keyField1);

            Label combindLabel = new Label(" + ");
            this.Add(combindLabel);

            keyField2 = new TextField();
            keyField2.style.width = 100;
            keyField2.maxLength = 1;
            keyField2.SetValueWithoutNotify(KeyCode2String(combindKey2));
            keyField2.RegisterValueChangedCallback(OnKeyField2Changed);
            this.Add(keyField2);
        }

        private string KeyCode2String(KeyCode key)
        {
            var skey = "";
            if(key == KeyCode.LeftAlt || key == KeyCode.RightAlt)
            { skey = "alt"; }
            else if(key == KeyCode.LeftControl || key == KeyCode.RightControl)
            { skey = "control"; }
            else if(key == KeyCode.LeftShift || key == KeyCode.RightShift)
            { skey = "shift"; }
            else
            {
                skey = char.ConvertFromUtf32((int)key);
            }
            return skey.ToLower();
        }
        private bool String2KeyCode(string skey, out KeyCode key)
        {
            skey = skey.Trim().ToLower();
            if (skey == "alt")
            { key = KeyCode.LeftAlt; }
            else if(skey == "control")
            { key = KeyCode.LeftControl; }
            else if(skey == "shift")
            { key = KeyCode.LeftShift; }
            else if(skey.Length == 1 && skey[0] > 0 && skey[0] < 127)
            {
                key = (KeyCode)skey[0];
            }
            else
            {
                key = KeyCode.None;
            }
            return key != KeyCode.None;
        }

        private void OnKeyField1Changed(ChangeEvent<string> skey)
        {
            if (skey.newValue.Length <= 0)
            { return; }

            char newKey = skey.newValue[0];
            if ((int)newKey < 0 || newKey > 127)
            {
                EditorUtility.DisplayDialog("", "illegal.", "OK");
                return;
            }
            combindKey1 = (KeyCode)newKey;
            onKey1Changed?.Invoke(combindKey1);
        }
        private void OnKeyField2Changed(ChangeEvent<string> skey)
        {
            if (skey.newValue.Length <= 0)
            { return; }
            char newKey = skey.newValue[0];
            if ((int)newKey < 0 || newKey > 127)
            {
                EditorUtility.DisplayDialog("", "illegal.", "OK");
                return;
            }
            combindKey2 = (KeyCode)newKey;
            onKey2Changed?.Invoke(combindKey2);
        }

        public void DisableKey1(bool disable)
        {
            keyField1.SetEnabled(!disable);
        }
        public void DisableKey2(bool disable)
        {
            keyField2.SetEnabled(!disable);
        }
    }

    public static class QuickFinderSettings
    {
        public const string FindAssetOwnersKeyString = "Find Asset Owners Shortcut Key";
        public const string FindSceneObjectWithAssetsKeyString = "Find Scene Object With Asset Key";

        private static Dictionary<string, int> shortcutMap = new Dictionary<string, int>();
        static QuickFinderSettings()
        {
            shortcutMap[FindAssetOwnersKeyString] =             PlayerPrefs.GetInt(FindAssetOwnersKeyString,            (int)KeyCode.LeftBracket);
            shortcutMap[FindSceneObjectWithAssetsKeyString] =   PlayerPrefs.GetInt(FindSceneObjectWithAssetsKeyString,  (int)KeyCode.RightBracket);
        }

        public static void UnserializeKey(string saveKey, int defaltValue)
        {
            defaltValue = (int)MapKey((KeyCode)defaltValue);
            var saveValue = PlayerPrefs.GetInt(saveKey, defaltValue);
            saveValue = (int)MapKey((KeyCode)saveValue);
            shortcutMap[saveKey] = saveValue;
        }
        public static void SerializeKey(string saveKey, int saveValue)
        {
            saveValue = (int)MapKey((KeyCode)saveValue);
            shortcutMap[saveKey] = saveValue;
            PlayerPrefs.SetInt(saveKey, saveValue);
        }
        public static void ReadKey(string readKey, out KeyCode key1, out KeyCode key2)
        {
            var saveValue = shortcutMap[readKey];
            key1 = (KeyCode)(saveValue >> 16);
            key2 = (KeyCode)((saveValue << 16) >> 16);
            key1 = MapKey(key1);
            key2 = MapKey(key2);
        }
        public static void SetKey(string saveKey, KeyCode key1, KeyCode key2)
        {
            key1 = MapKey(key1); key2 = MapKey(key2);
            var saveValue = (int)key1 << 16 + (int)key2;
            SerializeKey(saveKey, saveValue);
        }
        public static void SetKey1(string readKey, KeyCode key1)
        {
            key1 = MapKey(key1);
            var saveValue = shortcutMap[readKey];
            saveValue = (saveValue & 0x0000FFFF) + ((int)key1 << 16);
            SerializeKey(readKey, saveValue);
        }
        public static void SetKey2(string readKey, KeyCode key2)
        {
            key2 = MapKey(key2);
            var saveValue = shortcutMap[readKey];
            saveValue = ((saveValue >> 16) << 16) | (int)key2;
            SerializeKey(readKey, saveValue);
        }
        public static bool IsKey1(string readKey, KeyCode key1)
        {
            key1 = MapKey(key1);
            ReadKey(readKey, out var oldKey1, out _);
            return key1 == oldKey1;
        }
        public static bool IsKey2(string readKey, KeyCode key2)
        {
            key2 = MapKey(key2);
            ReadKey(readKey, out _, out var oldKey2);
            return key2 == oldKey2;
        }
        public static KeyCode MapKey(KeyCode rawkey)
        {
            if(rawkey == KeyCode.LeftAlt || rawkey == KeyCode.RightAlt)
            { return KeyCode.LeftAlt; }
            else if(rawkey == KeyCode.LeftControl || rawkey == KeyCode.RightControl)
            { return KeyCode.LeftControl; }
            else if(rawkey == KeyCode.LeftShift || rawkey == KeyCode.RightShift)
            { return KeyCode.LeftShift; }
            else { return rawkey; }
        }
    }
}


