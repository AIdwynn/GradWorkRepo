using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public abstract class SCR_BaseInAirState : SCR_CharacterBaseState
    {
        protected float _landingDistance = 0.4f;
        protected CharacterStatesEnum _StateToSwitchTo = CharacterStatesEnum.Landing;
        protected float gravity = SCR_GameSettings.PlayerAirGravity;
        protected SCR_BaseInAirState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform CameraTransform)
        {
            velocity.y += gravity;

            if(velocity.y < -15000)
                velocity.y = -15000;
            
            return base.VelocityCalculations(velocity, input, playerTransform, CameraTransform);
        }

        public override Vector3 PartialStateSwitch(Vector3 velocity)
        {
            return StateSwitch(velocity);
        }

        public override void ActiveSwitchCheck(InputButtons input)
        {
            if (input == InputButtons.RightTrigger && SCRMO_CharacterController.CanAirDash)
            {
                    ThrowSwitchEvent(CharacterStatesEnum.AirDash);
            }
            else if (input == InputButtons.B)
            {
                ThrowSwitchEvent(CharacterStatesEnum.GroundPound);
            }
            else if (input == InputButtons.A && SCRMO_CharacterController.CanAirDash)
            {
                ThrowSwitchEvent(CharacterStatesEnum.DoubleJump);
            }
        }
        protected override void PassiveSwitchCheck(Vector2 Input, Vector3 velocity, Transform transform)
        {
            CheckDistanceToGround(transform);
        }
        private void CheckDistanceToGround(Transform transform)
        {
            var rayDistance = 100;
            var layermask = 1 << SCR_GameSettings.WalkableObjectLayers;
            if (!Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out var hit, rayDistance,
                    layermask)) return;
            Debug.DrawRay(transform.position, Vector3.down * (_landingDistance), Color.red);
            if (hit.distance < _landingDistance + 0.5f)
            {
                ThrowSwitchEvent(_StateToSwitchTo);
            }
        }


    }
}
