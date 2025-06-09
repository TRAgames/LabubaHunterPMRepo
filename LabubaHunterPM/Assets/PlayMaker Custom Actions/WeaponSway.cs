using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Character)]
    public class WeaponSway : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to Sway")]
        public FsmOwnerDefault gameObject;

        public float xSwayAmount = 0.05f;

        public float ySwayAmount = 0.025f;
        public float maxXAmount = 0.35f;

        public float maxYAmount = 0.2f;
        public float smooth = 8.0f;
        public bool fixedUpdate;

        private Vector3 vector3;

        GameObject go;

        public override void OnPreprocess()
        {
            if (fixedUpdate) Fsm.HandleFixedUpdate = true;
        }
        public override void OnEnter()
        {
            vector3 = new Vector3(0, 0, 0);
            go = Fsm.GetOwnerDefaultTarget(gameObject);
        }

        public override void OnFixedUpdate()
        {
            var fx = -Input.GetAxis("Mouse X") * xSwayAmount;
            var fy = -Input.GetAxis("Mouse Y") * ySwayAmount;

            if (fx > maxXAmount)
            {
                fx = maxXAmount;
            }
            if (fx < -maxXAmount)
            {
                fx = maxXAmount;
            }
            if (fy > maxXAmount)
            {
                fy = maxYAmount;
            }
            if (fy < -maxXAmount)
            {
                fy = maxYAmount;
            }

            var detection = new Vector3(vector3.x + fx, vector3.y + fy, vector3.z);
            go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, detection, Time.deltaTime * smooth);
        }

    }
}
