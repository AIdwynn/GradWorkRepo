using System;
using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public abstract class SCR_BaseJumpingState : SCR_BaseInAirState, IJump
    {
        protected float _jumpHeight;
        protected float _jumpTime = 0.25f;
        protected float _jumpVelocity;
        protected float _jumpTimeCounter = 0;
        private Tweener tween;
        private float _FullMaxSpeed;

        public SCR_BaseJumpingState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
            _jumpHeight = creationVariables.maxJumpHeight;
            _jumpVelocity = _jumpHeight / _jumpTime;
            _FullMaxSpeed = _maxSpeed;
        }
    
        public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform CameraTransform)
        {
            _jumpTimeCounter += Time.fixedDeltaTime;
            velocity = base.VelocityCalculations(velocity, input, playerTransform, CameraTransform);
            velocity.y = _jumpVelocity;
            return velocity;
        }
        
        public override void ActiveSwitchCheck(InputButtons input)
        {
            if (input == InputButtons.RightTrigger&& SCRMO_CharacterController.CanAirDash)
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
        public override void OnStateExit()
        {
            _jumpTimeCounter = 0;
            tween.Complete();
            SCR_EventHelper.TrySendEvent(JumpStopped, this);
        }
        public override Vector3 StateSwitch(Vector3 velocity)
        {
            SCR_EventHelper.TrySendEvent(Jump, this);
            
            float duration = 0.5f;
            float _finalMaxSpeed = _FullMaxSpeed;
            var velocity2D = new Vector2(velocity.x, velocity.z);
            if (_finalMaxSpeed < velocity2D.magnitude)
                duration = 1.5f;
            _maxSpeed = velocity2D.magnitude;
            tween = DOTween.To(() => _maxSpeed, x => _maxSpeed = x, _finalMaxSpeed, duration).SetUpdate(UpdateType.Fixed);
            
            return velocity;
        }
        protected override void PassiveSwitchCheck(Vector2 Input, Vector3 velocity, Transform transform)
        {
            if(_jumpTimeCounter >= _jumpTime)
                ThrowSwitchEvent(CharacterStatesEnum.ApexJumpState);
        }
        
        public event EventHandler Jump;
        public event EventHandler JumpStopped;
    }
}