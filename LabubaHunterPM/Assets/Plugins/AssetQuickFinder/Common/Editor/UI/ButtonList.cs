
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
    public class ButtonList : VisualElement
    {
        private Action<string, int> action;

        public void Build(string title, List<string> buttonNames, Action<string, int> callback, List<string> tooltips = null,  FlexDirection direction = FlexDirection.Row)
        {
            this.Clear();

            action = callback;

            this.style.flexDirection = direction;

            var titleLabel = new Label();
            titleLabel.text = title;
            this.Add(titleLabel);


            for (int i = 0; i < buttonNames.Count; i++)
            {
                var button = new Button();
                var buttonIndex = i;
                var buttonName = buttonNames[buttonIndex];
                button.text = buttonName;
                if(tooltips != null)
                {
                    button.tooltip = tooltips[buttonIndex];
                }
                button.clicked += () => { action.Invoke(buttonName, buttonIndex); };
                //buttons.Add(button);

                this.Add(button);
            }
        }
    }
}


