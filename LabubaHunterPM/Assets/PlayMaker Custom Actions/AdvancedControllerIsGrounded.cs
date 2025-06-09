// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Tests if a Character Controller on a Game Object was touching the ground during the last move.")]
	public class AdvancedControllerIsGrounded : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject to check.")]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Event to send if touching the ground.")]
		public FsmEvent trueEvent;
		
		[Tooltip("Event to send if not touching the ground.")]
		public FsmEvent falseEvent;
		
		[Tooltip("Store the result in a bool variable.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;
		
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;


		public bool fixedUpdate;
		private CharacterController controller
        {
            get { return cachedComponent; }
        }

        public override void Reset()
		{
			gameObject = null;
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			if (fixedUpdate) Fsm.HandleFixedUpdate = true;
		}


		public override void OnEnter()
		{
			if (!everyFrame && !fixedUpdate)
			{
				DoControllerIsGrounded();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!fixedUpdate)
			{
				DoControllerIsGrounded();
			}
				
		}

		public override void OnFixedUpdate()
		{
			if (fixedUpdate)
			{
				DoControllerIsGrounded();
			}
		}


		void DoControllerIsGrounded()
		{
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject)))
                return;

			var isGrounded = controller.isGrounded;

			storeResult.Value = isGrounded;

			Fsm.Event(isGrounded ? trueEvent : falseEvent);
		}
	}
}
