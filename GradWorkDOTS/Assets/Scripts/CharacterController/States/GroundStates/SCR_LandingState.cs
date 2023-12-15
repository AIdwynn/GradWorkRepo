using StateCharacterController.CharacterStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    internal class SCR_LandingState : SCR_BaseGroundState
    {
        private Tweener tween;
        private float _FullMaxSpeed;
        
        public SCR_LandingState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
            _FullMaxSpeed = _maxSpeed;
        }

        public override void OnStateExit()
        {
            coyoteTimeTweener.Kill();
            tween.Kill();
        }
        public override Vector3 StateSwitch(Vector3 velocity)
        {
            float duration = 0.5f;
            float _finalMaxSpeed = _FullMaxSpeed;
            var velocity2D = new Vector2(velocity.x, velocity.z);
            if (_finalMaxSpeed < velocity2D.magnitude)
                duration = 1.5f;
            _maxSpeed = velocity2D.magnitude;
            tween = DOTween.To(() => _maxSpeed, x => _maxSpeed = x, _finalMaxSpeed, duration)
                .SetUpdate(UpdateType.Fixed)
                .OnComplete(() => ThrowSwitchEvent(CharacterStatesEnum.PreviousGround));

            currentEulerRotation.x = 0;
            FullVelocity = velocity;
            return velocity;
        }
    }

    
}
