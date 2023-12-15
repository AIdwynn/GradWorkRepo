using System;
using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_FallingState : SCR_BaseInAirState, IJump
    {
        private float _originalTransitionToReallyLongFallTime;

        private Tweener tween;
        private float _FullMaxSpeed;
        public SCR_FallingState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
            _originalTransitionToReallyLongFallTime = creationVariables.transitionToReallyLongFallTime;
            _transitionToReallyLongFallTime = creationVariables.transitionToReallyLongFallTime;
            _FullMaxSpeed = _maxSpeed;
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
            tween = DOTween.To(() => _maxSpeed, x => _maxSpeed = x, _finalMaxSpeed, duration);
            
            velocity.y = 0;
            return velocity;
        }

        public override Vector3 PartialStateSwitch(Vector3 velocity)
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
            _transitionToReallyLongFallTime -= Time.deltaTime;

              if (_transitionToReallyLongFallTime <= 0)
              {
                  ThrowSwitchEvent(CharacterStatesEnum.FallingOffPlatformOrLongTime);
              }

            base.PassiveSwitchCheck(Input, velocity, transform);

        }

        public override void OnStateExit()
        {
            _transitionToReallyLongFallTime = _originalTransitionToReallyLongFallTime;
            tween.Complete();
            
            SCR_EventHelper.TrySendEvent(JumpStopped, this);
        }

        public event EventHandler Jump;
        public event EventHandler JumpStopped;
    }
}