//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickFinder.Editor
{
    public abstract class FieldTemplateUIItem<TValue> : VisualElement
    {
        private int index;

        public int Index { get { return index; } }

        public abstract TValue Value { get; set; }

        public void Build(TValue obj, int index)
        {
            this.index = index;
            Refresh(obj);
        }

        protected abstract void Refresh(TValue obj);
    }

    public class FieldTemplateListView<TItem, TUIItem> : VisualElement where TUIItem : FieldTemplateUIItem<TItem>, new()
    {
        private Label titleLabel;

        private VisualElement externalOptionBar;

        private ListView listView;

        private List<TItem> elements = new List<TItem>();

        private Func<VisualElement> itemCreator;

        private bool created;

        public List<TItem> ItemSource { get { return listView.itemsSource as List<TItem>; } }

        public FieldTemplateListView(VisualElement externalOptionBar_ = null)
        { 
            if(externalOptionBar_ != null)
            {
                if(externalOptionBar_.parent != null)
                { externalOptionBar_.parent.Remove(externalOptionBar_); }
                this.externalOptionBar = externalOptionBar_;
            }
        }

        private void CreateView()
        {
            titleLabel = new Label();
            this.Add(titleLabel);

            if (externalOptionBar != null)
            {
                this.Add(externalOptionBar);
            }

            listView = new ListView();
            listView.selectionType = SelectionType.Single;
            listView.makeItem = MakeItem;
            listView.bindItem = BindItem;
            this.Add(listView);
        }

        public void Build(string title, Func<TUIItem> itemCreator, List<TItem> elements)
        {
            this.itemCreator = itemCreator;

            if (!created)
            {
                CreateView();
                created = true;
            }

            titleLabel.text = title;
            Refresh(elements);
        }
        
        public void Refresh(List<TItem> elements)
        {
            if (!created)
            {
                CreateView();
                created = true;
            }

            this.elements.Clear();
            this.elements.AddRange(elements);

            listView.itemsSource = this.elements;

#if UNITY_2021_2_OR_NEWER
            listView.Rebuild();
#else
            listView.Refresh();
#endif
        }

        private VisualElement MakeItem()
        {
            return itemCreator.Invoke();
        }

        private void BindItem(VisualElement element, int index)
        {
            var item = element as FieldTemplateUIItem<TItem>;
            item?.Build(elements[index], index);
        }
    }
}

