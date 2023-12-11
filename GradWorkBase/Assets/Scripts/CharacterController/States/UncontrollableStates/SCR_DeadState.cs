using UnityEditor;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_DeadState : SCR_BaseControllesState
    {
        public SCR_DeadState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform cameraTransform)
        {

            velocity.y = SCR_GameSettings.PlayerGroundGravity;
            return velocity;
        }
        
        public override void OnStateExit()
        {
        }

        public override void ActiveSwitchCheck(InputButtons input)
        {
        }

        public override Vector3 StateSwitch(Vector3 velocity)
        {
            return Vector3.zero;
        }

        protected override void PassiveSwitchCheck(Vector2 Input, Vector3 velocity, Transform transform)
        {
         
        }

        public override bool PlayerHit()
        {
            return false;
        }
    }
}