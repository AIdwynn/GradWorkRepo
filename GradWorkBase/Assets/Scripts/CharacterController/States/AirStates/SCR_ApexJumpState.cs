using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    internal class SCR_ApexJumpState : SCR_BaseInAirState, IJump
    {
        private float _apexTime = 0.075f;
        private Tween _tween;

        private Tweener momentumTween;
        private float _FullMaxSpeed;
        public SCR_ApexJumpState(SCR_CharacterStateCreationVariables creationVariables) : base( creationVariables)
        {
            gravity = 0;
            _FullMaxSpeed = _maxSpeed;
        }

        public override void OnStateExit()
        {
            _tween.Kill();
            momentumTween.Complete();
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
            momentumTween = DOTween.To(() => _maxSpeed, x => _maxSpeed = x, _finalMaxSpeed, duration);
            
            velocity.y = 0;

            var _fallingDelay = 0;
            _tween = DOTween.To(() => _fallingDelay, x => x = _fallingDelay, 1, _apexTime)
                .SetUpdate(UpdateType.Fixed).OnComplete(() =>
            {
                ThrowSwitchEvent(CharacterStatesEnum.Falling);
            });

            return velocity;
        }

        public event EventHandler Jump;
        public event EventHandler JumpStopped;
    }
}
