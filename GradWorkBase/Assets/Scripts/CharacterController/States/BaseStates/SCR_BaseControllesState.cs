using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public abstract class SCR_BaseControllesState : SCR_CharacterBaseState
    {
        protected SCR_BaseControllesState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform cameraTransform)
        {
            return Vector3.zero;
        }
        public override Vector3 PartialStateSwitch(Vector3 velocity)
        {
            return StateSwitch(velocity);
        }

        public  override void ActiveSwitchCheck(InputButtons input)
        {

        }

        protected override void PassiveSwitchCheck(Vector2 Input, Vector3 velocity, Transform transform)
        {

        }
    }
}
