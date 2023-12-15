using System;
using TMPro;
using UnityEngine;


public class SCRMO_CharacterAnimationController : MonoBehaviour
{
    [SerializeField]
    private SCRMO_CharacterController _characterController;

    [SerializeField] private TextMeshProUGUI _stateText;
    [Header("Keyboard only properties")]
   /* [SerializeField]
    private float _acceleration = 0.1f;*/

  /*  [SerializeField]
    private float _deceleration = 0.5f;*/

    private Animator _animator;

    private int _velocityHash;

  

    private bool _alreadyPlayedDeathAnimation = false;


    [Header("CharacterPunch")] [SerializeField]
    private float _punchSpeedAccelarator;

    private bool _hasAirDashedBeforeTouchingGround = false;

    private bool _hasDescendenBeforeTouchingGround = false;

    //CONSIDERING THAT ALL STATES ALREADY EXIST AT START JUST HOOK UP EACH INDIVIDUAL STATE WITH AN EVENT AND THEN LISTEN TO EACH INDIVDUAL STATE  
    // Start is called before the first frame update
    void Start()
    {

       


        _animator = GetComponent<Animator>();

        _velocityHash = Animator.StringToHash("Velocity");

       
      
        
        PlayPunchingAnimation();
        _characterController.StateChanged += (s, e) =>
        {
            if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Sprinting ||
                _characterController._stateHolder._stateEnum == CharacterStatesEnum.Idle)
            {
                _hasAirDashedBeforeTouchingGround = false;
                _hasDescendenBeforeTouchingGround = false;
            }

            CheckInAir();
            CheckIfGroundPounding();
            
            PlayDescentAnimation();

            
            CheckIfJumping();
            
            //NOT USED ANYMORE IN GROUP PROJECTS
           // CheckIfBackflipFinished();
            
            
            PlayGroundDashAnimation();
            
            PlayInAirDashAnimation();
            
            //NOT USED ANYMORE
            //PlayDashingAnimation();

           // PlayRollingAnimation();

            //PlayInAirBallAnimation();

            PlayFallingAnimation();

            PlayLandingAnimation();
            
            PlayFallingOffPlatformOrLongTimeAnimation();

            PlayDoubleJumpAnimation();

            PlayPreviousStateAnimation();

            //PlaySlidingAnimation();

            CheckIfIdle();

        };

        _characterController.PlayerRespawnedEvent += (s, e) =>
        {
            _animator.ResetTrigger("IsDead");
        };
    }
    private void CheckInAir()
    {
        if(_characterController._stateHolder._stateEnum == CharacterStatesEnum.Idle || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Sprinting || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Landing|| _characterController._stateHolder._stateEnum == CharacterStatesEnum.GroundDash)
        {
            _animator.SetBool("InAir", false);

        }
        else
        {
            _animator.SetBool("InAir", true);

        }
    }

    private void CheckIfGroundPounding()
    {
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.GroundPound)
        {
            _animator.SetBool("IsJumping",false);
            _animator.SetBool("IsFalling",true);
        }
    }

    private void PlayDescentAnimation()
    {
        if (_animator != null)
        {
            if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.GroundPound)
            {
                _animator.SetBool("IsDescending",true);
                _hasDescendenBeforeTouchingGround = true;
            }
            else
            {
                _animator.SetBool("IsDescending",false);
            }
            
        }    
    }
    
    private void PlayInAirDashAnimation()
    {
        if (_animator != null)
        {
            if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.AirDash)
            {

                _hasAirDashedBeforeTouchingGround = true;
                _animator.SetBool("IsAirDashing",true);

            }
            else if(!_hasAirDashedBeforeTouchingGround)
            {
                _animator.SetBool("IsAirDashing",false);
            }
        }    
    }

    private void PlayGroundDashAnimation()
    {
        if (_animator != null)
        {
            if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.GroundDash)
            {
           
                _animator.SetBool("IsGroundDashing",true);

             }
             else
             {
            _animator.SetBool("IsGroundDashing",false);
             }
        }    
    }
    
    private void PlayDoubleJumpAnimation()
    {
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.DoubleJump)
        {
            if (_animator != null)
            {
                _animator.SetTrigger("IsDoubleJumping");
            }
        }
    }

    private void PlayPunchingAnimation()
    {
        _characterController.PunchHandler += (s, e) =>
        {
            _animator.speed += _punchSpeedAccelarator;
            _animator.SetTrigger("IsStartingToPunch");
            if (_animator.GetBool("IsPunch1"))
            {
                _animator.SetBool("IsPunch1",false);
            }
            else
            {
                _animator.SetBool("IsPunch1",true);
            }
        };
    }

    //GROUP PROJECTS NOT USING IT ANYMORE
    /*private void PlayRollingAnimation()
    {
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Rolling)
        {
            _animator.SetTrigger("IsRolling");
        }
    }*/

    private void CheckIfJumping()
    {
        if (_animator != null)
        {
            if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.InitiatingJump ||
                _characterController._stateHolder._stateEnum == CharacterStatesEnum.Jumping ||
                _characterController._stateHolder._stateEnum == CharacterStatesEnum.ApexJumpState)
            {
                _animator.SetBool("IsJumping", true);
            }
            else
            {
                _animator.SetBool("IsJumping", false);

            }
        }

        /*if (_characterController._stateHolder._stateEnum != CharacterStatesEnum.InitiatingJump && _characterController._stateHolder._stateEnum != CharacterStatesEnum.Jumping && _characterController._stateHolder._stateEnum != CharacterStatesEnum.ApexJumpState)
        {
            if (_animator != null)
            {
                _animator.SetBool("IsJumping", false);
            }
        }*/
    }

    private void CheckIfIdle()
    {
        if(_characterController._stateHolder._stateEnum == CharacterStatesEnum.Idle)
        {
            ResetAllAnimationTriggers();
        }
    }

    //NOT USED ANYMORE IN GROUP PROJECTS
   /* private void PlaySlidingAnimation()
    {
        if(_characterController._stateHolder._stateEnum == CharacterStatesEnum.Sliding)
        {
            if (_animator != null)
            {
                _animator.SetTrigger("IsSliding");
            }
        }
       
     }*/

    private void ResetAllAnimationTriggers()
    {
        /*if(_animator != null)
        {
            foreach (var trigger in _animator.parameters)
            {
                if (trigger.type == AnimatorControllerParameterType.Trigger)
                {
                    _animator.ResetTrigger(trigger.name);
                }
            }
        }*/
    }

    //NOT USED ANYMORE SINCE GROUP PROJECTS
    /*private void CheckIfBackflipFinished()
    {
        //Test code from backflip to falling
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.PostBackFlip)
        {
            _animator.SetTrigger("FinishedBackflip");

        }
    }*/

    
    //NOT USED ANYMORE IN GROUP PROJECTS
  /*  private void PlayInAirBallAnimation()
    {
        if(_animator != null)
        {
            if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Diving)
            {
                _animator.SetBool("IsInAirDodgingDive",true);
            }
            else
            {
                _animator.SetBool("IsInAirDodgingDive", false);
            }

            //NOT USED ANYMORE IN GROUP PROJECTS

           *//* if(_characterController._stateHolder._stateEnum == CharacterStatesEnum.BackFlipping)
            {
                _animator.SetBool("IsInAirDodgingBackflip", true);

            }
            else
            {
                _animator.SetBool("IsInAirDodgingBackflip", false);

            }*//*
        }
       
      *//*  if(_characterController._stateHolder is IRoll)
        {
            var rollingstate = _characterController._stateHolder as IRoll;
            rollingstate.Roll += (s, e) =>
            {
                _animator.SetBool("IsInAirDodgingBackflip", true);

            };
            rollingstate.RollStopped += (s, e) =>
            {
                _animator.SetBool("IsInAirDodgingBackflip", false);

            };
        }
*//*
      
    }*/
    private void PlayFallingOffPlatformOrLongTimeAnimation()
    {
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.FallingOffPlatformOrLongTime)
        {
            if(_animator != null)
            {
                _animator.SetTrigger("IsFallingOffPlatformOrLongTime");
            }
        }
    }

    private void PlayLandingAnimation()
    {

        //TODO CHANGE THIS TO MAYBE A ONGROUND TRIGGER
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Landing  || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Idle || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Sprinting|| _characterController._stateHolder._stateEnum == CharacterStatesEnum.GroundPound )
        {
            if (_animator != null)
            {
                _animator.SetTrigger("IsLanding");
            }
        }
    }

    //OLD
    /*private void PlayDashingAnimation()
    {
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Dashing)
        {
            if (_animator != null)
            {
                _animator.SetTrigger("IsDashing");

            }
        }
    }*/
    private void PlayPlayerDeadAnimation(object sender, EventArgs e)
    {
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Dead && _alreadyPlayedDeathAnimation  == false)
        {
            if(_animator != null)
            {
                _alreadyPlayedDeathAnimation = true;
                _animator.SetBool("IsDead",true);
            }
        }
      
    }

    private void PlayFallingAnimation()
    {
        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Falling)
        {
            if (_animator != null)
            {
                if(!_hasDescendenBeforeTouchingGround)
                 _animator.SetBool("IsFalling", true);
            }

        }
        if (_characterController._stateHolder._stateEnum != CharacterStatesEnum.Falling)
        {
            if(_animator != null)
            {
                _animator.SetBool("IsFalling", false);

            }

        }
    }

    private void PlayPreviousStateAnimation()
    {
        //BASICALLY ALL INAIR STATES
        //_isPreviousStateIdle && WAS ALSO ADDED BUT NO CLUE WHAT IT WAS DOING HERE
        //BRO HOW DOES THIS MAKE SENSE TO KNOW IF PREVIOUS STATE WAS IDLE OR NOT WUT???
        //  JUST IN KAAS      if ( (_characterController._stateHolder._stateEnum == CharacterStatesEnum.InitiatingJump || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Jumping || _characterController._stateHolder._stateEnum == CharacterStatesEnum.ApexJumpState || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Falling))

        if (_characterController._stateHolder._stateEnum == CharacterStatesEnum.Idle)
        {
            if (_animator != null)
            {
                _animator.SetBool("WasIdle",true);

               //OLD CODE HOPEFULLY
                /*_animator.SetTrigger("IsPreviousStateIdle");
                _animator.ResetTrigger("IsPreviousStateWalkingOrSprinting");*/
            }
          

        }
        
        //AGAIN WTF ARE YOU DOING
        //        else if ((_characterController._stateHolder._stateEnum == CharacterStatesEnum.Jumping || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Falling || _characterController._stateHolder._stateEnum == CharacterStatesEnum.Sliding))

        else if ( _characterController._stateHolder._stateEnum == CharacterStatesEnum.Sprinting)
        {
            if (_animator != null)
            {
                _animator.SetBool("WasIdle",false);
                
                /*_animator.SetTrigger("IsPreviousStateWalkingOrSprinting");
                _animator.ResetTrigger("IsPreviousStateIdle");*/
            }
        }
    }

   

    private void PlayPlayerGotHitBackwardsKnockbackAnimation(object sender, EventArgs e)
    {
        
            if(_animator!= null)
            {
                _animator.SetTrigger("IsHit");

            }
        
    }

    // Update is called once per frame
    void Update()
    {
       /* //TEST CODE
        if (Input.GetKey(KeyCode.X))
            _animator.SetTrigger("Test");*/


            ChangeAnimationMoveBlendTreeController();

        
        _stateText?.SetText("Animation State: " + _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
    }

    private void ChangeAnimationMoveBlendTreeKeyboard()
    {

        //OLD CODE
      /*  if (_pressedMoveKey && _keyboardVelocity < 1.0f)
        {
            _keyboardVelocity += Time.deltaTime * _acceleration;
        }

        if (!_pressedMoveKey && _keyboardVelocity > 0)
        {
            _keyboardVelocity -= Time.deltaTime * _deceleration;
        }

        if (!_pressedMoveKey && _keyboardVelocity < 0)
        {
            _keyboardVelocity = 0;
        }
        _animator.SetFloat(_velocityHash, _keyboardVelocity);
*/

        float velocity = 0;
        var velocity2D = new Vector2(_characterController.Velocity.x, _characterController.Velocity.z);

        if (_characterController._stateHolder != null && velocity2D.magnitude != 0)
        {

            velocity = velocity2D.magnitude / _characterController.MaxSprintingSpeed;
            velocity = Mathf.Clamp(velocity, 0, 1);
        }
        else
        {
            velocity = 0;
        }

        if(_animator != null)
        {
            _animator.SetFloat(_velocityHash, velocity);

        }
    }

    private void ChangeAnimationMoveBlendTreeController()
    {
     float velocity = 0;
     var velocity2D = new Vector2(_characterController.Velocity.x, _characterController.Velocity.z);

     if (_characterController._stateHolder != null && velocity2D.magnitude != 0)
     {

         velocity = velocity2D.magnitude / _characterController.MaxSprintingSpeed;
         velocity = Mathf.Clamp(velocity, 0, 1);
     }
     else
     {
         velocity = 0;
     }

     if (_animator != null)
        {
            _animator.SetFloat(_velocityHash, velocity);
        }
    }
}
