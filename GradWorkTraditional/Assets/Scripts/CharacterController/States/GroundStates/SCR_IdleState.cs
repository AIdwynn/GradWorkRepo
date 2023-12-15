using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_IdleState : SCR_BaseGroundState
    {
        public SCR_IdleState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override void ActiveSwitchCheck(InputButtons input)
        {
            if (input == InputButtons.A)
            {
                ThrowSwitchEvent(CharacterStatesEnum.InitiatingJump);
            }
        }
        public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform CameraTransform)
        {
            currentInput = input;
            velocity = Vector3.zero;
            //velocity.y = SCR_GameSettings.PlayerGroundGravity;

            TurningCharacter(playerTransform, input);
            PassiveSwitchCheck(input, velocity, playerTransform);
            return velocity;
        }
        public override Vector3 StateSwitch(Vector3 velocity)
        {
            FullVelocity = Vector3.zero;
            return new Vector3(0,0,0);
        }

        protected override void PassiveSwitchCheck(Vector2 input, Vector3 velocity, Transform transform)
        {
            if (input.magnitude > 0.1)
                ThrowSwitchEvent(CharacterStatesEnum.Sprinting);
            base.PassiveSwitchCheck(input, velocity, transform);
        }

        public override void OnStateExit()
        {
            coyoteTimeTweener.Kill();
        }
    }
}