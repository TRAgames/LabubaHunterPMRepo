//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace QuickFinder.Editor
{
    /// <summary>
    /// a tag selector. Add new tag on left side, show tag list on right side. Click tags to remove it.
    /// </summary>
    public class TagsButtonSelector : Box
    {
        public TagsButtonSelector(string listButtonName, List<string> tagList)
        {
            this.style.flexDirection = FlexDirection.Row;

            m_nameButton = new Button();
            m_nameButton.text = listButtonName;
            m_nameButton.clicked += OnListNameClick;
            this.Add(m_nameButton);

            m_tagField = new TextField();
            this.Add(m_tagField);
            m_tagField.style.width = 240;

            m_addButton = new Button();
            m_addButton.text = "+";
            this.Add(m_addButton);
            m_addButton.clicked += () =>
            {
                if (string.IsNullOrEmpty(m_tagField.value) || m_tagList.Contains(m_tagField.value))
                    return;

                if (onAddTagButtonClick != null && !onAddTagButtonClick.Invoke(m_tagField.value))
                    return;
                m_tagField.SetValueWithoutNotify(m_tagField.value.Trim());
                m_tagList.Add(m_tagField.value);
                RefreshList(m_tagList);
            };

            RefreshList(tagList);
        }

        public void RefreshList(List<string> tagList)
        {
            m_tagList = tagList;
            if (m_listArea != null)
            {
                this.Remove(m_listArea);
                m_listArea = null;
            }
            m_listArea = new Box();
            m_listArea.style.flexDirection = FlexDirection.Row;
            m_listArea.style.flexWrap = Wrap.Wrap;
            this.Add(m_listArea);
            for (int i = 0; i < m_tagList.Count; i++)
            {
                var tagButton = new Button();
                tagButton.text = m_tagList[i];
                tagButton.clicked += () =>
                {
                    if (EditorUtility.DisplayDialog("", $"Do you want to remove¡¾{tagButton.text}¡¿ tag?", "CONFIRM", "CANCEL"))
                    {
                        var allow = onRemoveButtonClick != null ? onRemoveButtonClick(tagButton.text) : true;
                        if (allow)
                        {
                            if (m_tagList.Remove(tagButton.text))
                            { m_listArea.Remove(tagButton); }
                        }
                    }
                };
                m_listArea.Add(tagButton);
            }
        }

        void OnListNameClick()
        {
            onListNameButtonClick?.Invoke();
        }

        public List<string> CopyTagList()
        {
            return m_tagList.Select(x => x).ToList();
        }

        private Button m_nameButton;
        private TextField m_tagField;
        private Button m_addButton;
        private Box m_listArea;

        private List<string> m_tagList = new List<string>();


        public Action onListNameButtonClick;

        public Func<string, bool> onAddTagButtonClick;
        public Func<string, bool> onRemoveButtonClick;
    }
}
