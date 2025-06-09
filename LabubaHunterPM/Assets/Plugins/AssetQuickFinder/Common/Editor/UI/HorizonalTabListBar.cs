//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuickFinder.Editor
{
    public class HorizonalTabListBar : VisualElement
    {
        private class ButtonParam
        {
            public int index;
        }

        Action<string, int> action;

        int selectedIndex = -1;
        public int SelectedIndex
        {  get { return selectedIndex; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonNames"></param>
        /// <param name="action_">Action<bool, string, int> action: <isSelected, ButtonName, ButtonIndex></param>
        /// <param name="defalutSelectedIndex"></param>
        public void Build(List<string> buttonNames, Action<string, int> action_)
        {
            action = action_;

            this.Clear();

            this.style.flexDirection = FlexDirection.Row;

            for (int i = 0; i < buttonNames.Count; i++)
            {
                var btn = new Button();
                btn.name = buttonNames[i];
                btn.text = buttonNames[i];
                btn.style.flexGrow = 1f;
                this.Add(btn);

                var pack = new ButtonParam();
                pack.index = i;
                btn.userData = pack;

                btn.clicked += () =>  { OnTabClicked(btn, false); };
            }

        }

        public void SwitchTab(int tabIndex, bool force)
        {
            var btn = this.ElementAt(tabIndex) as Button;
            OnTabClicked(btn, true);
        }

        private void OnTabClicked(Button btn, bool force)
        {
            var btnPack = (ButtonParam)btn.userData;
            if(force || (btnPack.index != selectedIndex))
            {
                MarkButtonName(btnPack);
                action.Invoke(btn.name, btnPack.index);
                selectedIndex = btnPack.index;
            }
        }

        private void MarkButtonName(ButtonParam param)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var button = this.ElementAt(i) as Button;
                if(button == null)
                { continue; }
                button.text = button.name;
                if(param.index == i)
                { button.text = string.Format("¡¾ {0} ¡¿", button.name); }
            }
        }

        private static void SimulateClicking(VisualElement element)
        {
            #region UIELEMENTS SHITS
            //// For RegisterCallback<ClickEvent>
            //using (var clickEvent = ClickEvent.GetPooled())
            //{
            //    clickEvent.target = element;
            //    element.panel.visualTree.SendEvent(clickEvent);
            //}

            //// For clicked += event registration
            //if (element is Button button)
            //{
            //    using (var ev = new NavigationSubmitEvent() { target = button, })
            //    {
            //        button.SendEvent(ev);
            //    }
            //}

            //if (element is Button button)
            //{
            //    using (MouseDownEvent mouseDownEvent = MouseDownEvent.GetPooled(button.worldBound.center, 0, 1, UnityEngine.Vector2.zero))
            //    {
            //        button.SendEvent(mouseDownEvent);
            //    }
            //    using (MouseUpEvent mouseUpEvent = MouseUpEvent.GetPooled(button.worldBound.center, 0, 1, UnityEngine.Vector2.zero))
            //    {
            //        button.SendEvent(mouseUpEvent);
            //    }
            //}
            #endregion

            if (element is Button button)
            {
                using (EventBase mouseDownEvent = MakeMouseEvent(EventType.MouseDown, button.worldBound.center))
                    button.SendEvent(mouseDownEvent);
                using (EventBase mouseUpEvent = MakeMouseEvent(EventType.MouseUp, button.worldBound.center))
                    button.SendEvent(mouseUpEvent);
            }

        }

        public static EventBase MakeMouseEvent(EventType type, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None, int clickCount = 1)
        {
            var evt = new Event() { type = type, mousePosition = position, button = (int)button, modifiers = modifiers, clickCount = clickCount };
            if (type ==EventType.MouseUp)
                return PointerUpEvent.GetPooled(evt);
            else if (type == EventType.MouseDown)
                return PointerDownEvent.GetPooled(evt);
            return null;
        }
    }


}
