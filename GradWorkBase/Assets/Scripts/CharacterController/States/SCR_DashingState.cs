using System;
using DG.Tweening;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_DashingGroundState : SCR_BaseGroundState, ISlide, IUseStamina
    {
        private float _dashBoost = 2000;
        private float _boost = 0;
        private float _dashTime = 0.1f;
        
        private Tween _dashTween;
        private Vector3 _inputForward;
        private bool firstFrame = true;
        
        public SCR_DashingGroundState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        protected override Vector3 CalculateVelocityWithForward(Vector3 velocity, Vector2 Input, Transform CharacterController)
        {
            if (firstFrame)
            {     
                _inputForward = new Vector3(Input.x, 0, Input.y).normalized;
                firstFrame = false;
            }

            //velocity =  base.CalculateVelocityWithForward(velocity, Input, CharacterController);
            return velocity + (2*(_inputForward * _boost));
        }

        public override void ActiveSwitchCheck(InputButtons input)
        {
            
        }

        public override Vector3 StateSwitch(Vector3 velocity)
        {
            SCR_EventHelper.TrySendEvent(Slide, this);
            SCR_EventHelper.TrySendEvent(StaminaUsed, this);
            
            _boost = _dashBoost;
            _dashTween = DOTween.To(() => _boost, x => _boost = x, _boost/10, _dashTime).SetEase(Ease.OutQuad, 5, 0)
                .SetUpdate(UpdateType.Fixed).OnComplete(() => ThrowSwitchEvent(CharacterStatesEnum.PreviousGround));
            coyoteTimeTweener.Kill();

            return velocity;
        }

        public override void OnStateExit()
        {
            SCR_EventHelper.TrySendEvent(SlideStopped, this);
            firstFrame = true;
            
            _dashTween.Kill();
        }

        public event EventHandler Slide;
        public event EventHandler SlideStopped;
        public event EventHandler StaminaUsed;
    }
    
    public class SCR_DashingAirState : SCR_BaseInAirState, IRoll, IUseStamina
    {
        private float _dashBoost = 2000;
        private float _boost = 0;
        private float _dashTime = 0.1f;
        
        private Tween _dashTween;
        private Vector3 _inputForward;
        private bool firstFrame = true;
        
        public SCR_DashingAirState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }
        protected override Vector3 CalculateVelocityWithForward(Vector3 velocity, Vector2 Input, Transform CharacterController)
        {
            if (firstFrame)
            {     
                _inputForward = new Vector3(Input.x, 0, Input.y).normalized;
                firstFrame = false;
            }
            //velocity =  base.CalculateVelocityWithForward(velocity, Input, CharacterController);
            return velocity + (2*(_inputForward * _boost));
        }

        public override void ActiveSwitchCheck(InputButtons input)
        {
            
        }
        public override Vector3 StateSwitch(Vector3 velocity)
        {
            SCR_EventHelper.TrySendEvent(Roll, this);
            SCR_EventHelper.TrySendEvent(StaminaUsed, this);
            
            _boost = _dashBoost;
            _dashTween = DOTween.To(() => _boost, x => _boost = x, _boost/10, _dashTime).SetEase(Ease.OutQuad, 5, 0)
                .SetUpdate(UpdateType.Fixed).OnComplete(() => ThrowSwitchEvent(CharacterStatesEnum.ApexJumpState));

            gravity = 0;
            velocity.y = 0;
            
            return velocity;
        }

        public override void OnStateExit()
        {
            SCR_EventHelper.TrySendEvent(RollStopped, this);
            firstFrame = true;
            
            _dashTween.Kill();
        }


        public event EventHandler Roll;
        public event EventHandler RollStopped;
        public event EventHandler StaminaUsed;
    }
}
