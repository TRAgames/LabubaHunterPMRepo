
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Device")]
	[Tooltip("")]

	public class GetTouchInput : FsmStateAction
	{

        public FsmFloat lookInputDeadZone;
        public FsmFloat InputX;
		public FsmFloat InputY;
        public FsmBool everyFrame;
       

        int rightFingerId;
		float halfScreenWidth;
		Vector2 lookInput;

		public override void Reset()
		{

			InputX = 0;
			InputY = 0;
			everyFrame = false;
		}

		public override void OnEnter()
		{
            rightFingerId = -1;
            halfScreenWidth = Screen.width / 2;
        }

		public override void OnUpdate()
		{
			if (everyFrame.Value)
			{
                GetValues();

                if (lookInput.sqrMagnitude <= lookInputDeadZone.Value)
                {
                    InputX.Value = 0;
                    InputY.Value = 0;
                }
                else
                {
                    InputX.Value = lookInput.x;
                    InputY.Value = lookInput.y;
                }

            }
		}

       void GetValues()
        {

            for (int i = 0; i < Input.touchCount; i++)
            {

                Touch t = Input.GetTouch(i);

                switch (t.phase)
                {
                    case TouchPhase.Began:

                        if (t.position.x > halfScreenWidth && rightFingerId == -1)
                        {
                            rightFingerId = t.fingerId;
                        }

                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:

                        if (t.fingerId == rightFingerId)
                        {
                            rightFingerId = -1;
                            lookInput = Vector2.zero;
                        }

                        break;
                    case TouchPhase.Moved:


                        if (t.fingerId == rightFingerId)
                        {
                            lookInput = t.deltaPosition * 4 * Time.deltaTime;
                        }

                        break;
                    case TouchPhase.Stationary:

                        if (t.fingerId == rightFingerId)
                        {
                            lookInput = Vector2.zero;
                        }
                        break;
                }
            }
        }

    }
}