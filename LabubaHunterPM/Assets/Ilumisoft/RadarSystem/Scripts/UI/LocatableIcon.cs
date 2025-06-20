﻿using UnityEngine;
using UnityEngine.UI;

namespace Ilumisoft.RadarSystem.UI
{
    /// <summary>
    /// Concrete component for the icon being visible on the radar of a locatable
    /// </summary>
    [AddComponentMenu("Radar System/UI/Locatable Icon")]
    [RequireComponent(typeof(CanvasGroup))]
    public class LocatableIcon : LocatableIconComponent
    {
        protected CanvasGroup CanvasGroup { get; set; }
        [SerializeField] private Image _image;

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            //_image = GetComponent<Image>();
        }

        public override void SetVisible(bool visibility)
        {
            CanvasGroup.alpha = visibility ? 1.0f : 0.0f;
        }

        public override void SetColorRed()
        {
            _image.color = Color.red;
        }
    }
}