using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedHorrorFPS
{
    public class TouchpadFPSLook : MonoBehaviour
    {
        public float horizontalRotationSpeed = 250.0f;
        public float verticalRotationSpeed = 150.0f;
        public float rotationDampening = 0.75f;
        public float minVerticalAngle = -60.0f;
        public float maxVerticalAngle = 60.0f;

        public float delta;

        private float h, v;
        private Vector3 newPosition;
        private Quaternion newRotationH, newRotationV, smoothRotation;
        //private Transform cameraTransform;

        public Transform transformH;
        public Transform transformV;
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            h = this.transform.eulerAngles.x + delta;
        }


        void LateUpdate()
        {

            h += Touchpad.Instance.HorizontalValue * horizontalRotationSpeed * Time.deltaTime * PlayerPrefs.GetFloat("Sensitivity", 0.2f) * PlayerPrefs.GetFloat("SensitivityC", 0.5f);
            v -= Touchpad.Instance.VerticalValue * verticalRotationSpeed * Time.deltaTime * PlayerPrefs.GetFloat("Sensitivity", 0.2f) * PlayerPrefs.GetFloat("SensitivityC", 0.5f);

            h = ClampAngle(h, -360.0f, 360.0f);
            v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

            newRotationH = Quaternion.Euler(0, h, 0.0f);

            transformH.rotation = newRotationH;

            newRotationV = Quaternion.Euler(v, 0, 0.0f);
            
            transformV.localRotation = newRotationV;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;

            if (angle > 360)
                angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }

        private float TimeSignature(float speed)
        {
            return 1.0f / (1.0f + 80.0f * Mathf.Exp(-speed * 0.02f));
        }
    }
}