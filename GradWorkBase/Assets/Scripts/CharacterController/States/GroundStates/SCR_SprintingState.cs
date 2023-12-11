using UnityEngine;
using DG.Tweening;

namespace StateCharacterController.CharacterStates
{
    public class SCR_SprintingState : SCR_BaseGroundState
    {
        private Tweener tween;
        private float _FullMaxSpeed;
        public SCR_SprintingState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
            _FullMaxSpeed = _maxSpeed;
        }

        public override void OnStateExit()
        {
            coyoteTimeTweener.Kill();
            tween.Complete();
        }

        public override Vector3 StateSwitch(Vector3 velocity)
        {
            float duration = 0.5f;
            float _finalMaxSpeed = _FullMaxSpeed;
            var velocity2D = new Vector2(velocity.x, velocity.z);
            if (_finalMaxSpeed < velocity2D.magnitude)
                duration = 1.5f;
            _maxSpeed = velocity2D.magnitude;
            tween = DOTween.To(() => _maxSpeed, x => _maxSpeed = x, _finalMaxSpeed, duration).SetUpdate(UpdateType.Fixed);
            FullVelocity = velocity;
            return velocity;
        }

        protected override void PassiveSwitchCheck(Vector2 input, Vector3 velocity, Transform transform)
        {
            var velocity2D = new Vector2(velocity.x, velocity.z);
            var curDir3 = transform.forward;
            var curDir2 = new Vector2(curDir3.x, curDir3.z).normalized;
            var inpDir = input.normalized;

            //var tolerance = 0.8f;

            if (velocity2D.magnitude *input.magnitude < 0.05)
                ThrowSwitchEvent(CharacterStatesEnum.Idle);
            /*else if (velocity.magnitude * input.magnitude < (_maxSpeed * 3/4))
                ThrowSwitchEvent(CharacterStatesEnum.Walking);
            else if (!CheckDirection(curDir2, inpDir, tolerance))
                ThrowSwitchEvent(CharacterStatesEnum.Walking);*/
            base.PassiveSwitchCheck(input, velocity, transform);
        }
    }
}