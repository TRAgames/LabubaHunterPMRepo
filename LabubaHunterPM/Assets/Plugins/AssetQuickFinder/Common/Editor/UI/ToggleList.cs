//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace QuickFinder.Editor
{
    public class ToggleList : VisualElement
    {
        private List<Toggle> toggles = new List<Toggle>();
        private Action<string, int, bool> action;

        public void Build(string title, List<string> toggleNames, Action<string, int, bool> callback, List<string> toolTips = null,  FlexDirection direction = FlexDirection.Row)
        {
            this.Clear();
            toggles?.Clear();

            this.style.flexDirection = direction;

            action = callback;

            var titleLabel = new Label();
            titleLabel.text = title;
            this.Add(titleLabel);


            for (int i = 0; i < toggleNames.Count; i++)
            {
                var toggle = new Toggle();
                toggles.Add(toggle);
                var toggleIndex = i;
                var toggleName = toggleNames[toggleIndex];
                toggle.text = toggleName;
                if(toolTips != null)
                {
                    toggle.tooltip = toolTips[toggleIndex];
                }
                toggle.RegisterValueChangedCallback((evt)=> { action?.Invoke(toggleName, toggleIndex, evt.newValue); });

                this.Add(toggle);
            }
        }

        public void SetOption(int toggleIndex, bool selected, bool withoutNotify = false)
        {
            if(!withoutNotify)
            {
                toggles[toggleIndex].value = selected;
            }
            else
            {
                toggles[toggleIndex].SetValueWithoutNotify(selected);
            }
        }

        public bool IsOptionSelected(int toggleIndex)
        {
            return toggles[toggleIndex].value;
        }
    }

}
