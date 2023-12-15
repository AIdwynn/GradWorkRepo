using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    internal class SCR_HitState : SCR_BaseControllesState
    {
        private float _hitForce = 1500;
        //CURRENTLY MADE IT SO HIT = ANIMATION TIME 
        //OG TIME WAS 2.07F
        private float _pushBackTime = 1f;
        private Vector2 _velocity;
        private Tweener tweem;
        public SCR_HitState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform cameraTransform)
        {
            velocity = new Vector3(_velocity.x, SCR_GameSettings.GroundPoundGravity * 3, _velocity.y);
            var velocity2D = new Vector2(velocity.x, velocity.z);
            TurningCharacter(playerTransform, -velocity2D.normalized);
            return velocity;
        }

        public override void OnStateExit()
        {
            tweem.Complete();
        }

        public override Vector3 StateSwitch(Vector3 velocity)
        {
            return velocity;
        }

        public void HitFromPosition(Vector3 HitPosition, Vector3 MyPosition)
        {
            //I want the direction that points away from the position the second point is the players position
            Vector3 direction = ( MyPosition - HitPosition).normalized;
            if(HitPosition.y > MyPosition.y)
            {
                direction.y = 0;
                direction.Normalize();
            }
            _velocity = direction * _hitForce;

            tweem = DOTween.To(() => _velocity, x => _velocity = x, Vector2.zero, _pushBackTime).SetEase(Ease.OutSine)
                .SetUpdate(UpdateType.Fixed).OnComplete(() => ThrowSwitchEvent(CharacterStatesEnum.Idle));
        }

        public override bool PlayerHit() { return false; }
    }
}
