using Ilumisoft.RadarSystem.UI;
using UnityEngine;

namespace Ilumisoft.RadarSystem
{
    [AddComponentMenu("Radar System/Locatable")]
    public class Locatable : LocatableComponent
    {
        [SerializeField]
        protected LocatableIconComponent iconPrefab;
        protected LocatableIconComponent icon;

        [SerializeField, Tooltip("Determines whether the locatable will be hidden or will stay visible when being out of the radar radius")]
        private bool clampOnRadar = false;

        public override bool ClampOnRadar { get => clampOnRadar; set => clampOnRadar = value; }

        public override LocatableIconComponent CreateIcon()
        {
            icon = Instantiate(iconPrefab);
            return icon;
        }

        public void Death()
        {
            icon.SetColorRed();
        }
    }
}