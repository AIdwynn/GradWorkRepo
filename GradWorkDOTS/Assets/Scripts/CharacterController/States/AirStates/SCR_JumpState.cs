using System;
using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_JumpState : SCR_BaseInAirState, IJump
    {
        private Tweener tween;
        private float _FullMaxSpeed;
        public SCR_JumpState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
            gravity = SCR_GameSettings.PlayerJumpGravity;
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
            tween = DOTween.To(() => _maxSpeed, x => _maxSpeed = x, _finalMaxSpeed, duration).SetUpdate(UpdateType.Fixed);
            
            return velocity;
        }

        protected override void PassiveSwitchCheck(Vector2 Input, Vector3 velocity, Transform transform)
        {
            if (velocity.y < 0)
                ThrowSwitchEvent(CharacterStatesEnum.ApexJumpState);
        }

        public override void OnStateExit()
        {
            tween.Complete();
            SCR_EventHelper.TrySendEvent(JumpStopped, this);
        }

        public event EventHandler Jump;
        public event EventHandler JumpStopped;
    }
}