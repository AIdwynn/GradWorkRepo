using StateCharacterController.CharacterStates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using UnityEngine.Video;

public class SCRMO_CharacterController : MonoBehaviour
{
    [Header("Colliders")] [SerializeField] private CapsuleCollider _standingCollider;
    [SerializeField] private CapsuleCollider _rollingCollider;
    [SerializeField] private CapsuleCollider _slidingCollider;
    [SerializeField] private CapsuleCollider _jumpingCollider;
    public float radius = 0.25f;

    [Header("Balancing Values")] [SerializeField]
    private float _parryTime = 0.2f;

    [SerializeField] private float _parryCooldown = 2f;
    [SerializeField] private float _staminaRecoverspeed = 1.25f;
    public static bool GodMode = false;

    [Header("Terrain Texture Recognition")] [SerializeField]
    private int lightIndex;

    [SerializeField] private int darkIndex;
    public static bool CanAirDash { get; private set; } = true;

    [SerializeField] private int _maxAirMoves = 3;
    private int _currentAirMoves = 3;
    private int _storedStamina = 0;

    public int StoredStamina
    {
        get { return _storedStamina; }

        private set
        {
            if (value > _maxAirMoves)
                _storedStamina = _maxAirMoves;
            else
                _storedStamina = value;
        }
    }

    public int CurrentAirMoves
    {
        get { return _currentAirMoves; }

        private set
        {
            if (value > _currentAirMoves + 1)
            {
              //  Debug.LogError("Value was bigger then current air moves + 1");
                var difference = value - _currentAirMoves;
                for (int i = 0; i < difference; i++)
                {
                    CurrentAirMoves++;
                }

                return;
            }
            else if (value < _currentAirMoves - 1)
            {
               // Debug.LogError("Value was smaller then current air moves - 1");
                var difference = _currentAirMoves - value;
                for (int i = 0; i < difference; i++)
                {
                    CurrentAirMoves--;
                }

                return;
            }
            else
            {
                if (value > _maxAirMoves) return;
                if (value > _currentAirMoves && ScrCharacterState is SCR_BaseInAirState)
                {
                    StoredStamina++;
                    return;
                }

                _currentAirMoves = value;
                if (CurrentAirMoves > 0)
                    CanAirDash = true;
                OnCurrentAirMovesChanged();
            }
        }
    }

    public event EventHandler CurrentAirMovesChangedEvent;


    [Header("State Inputs")] [SerializeField]
    private List<SCR_CharacterStateCreationVariables> _stateInputs = new List<SCR_CharacterStateCreationVariables>();

    private SCR_CharacterBaseState ScrCharacterState
    {
        get { return _stateHolder; }
        set { OnStateChenged(value); }
    }

    private void OnStateChenged(SCR_CharacterBaseState value)
    {
        /*if(_currentStates == _sandCharacterStates)
            Debug.Log("sand");
        else
            Debug.Log("not sand");*/
        if (_stateHolder != null)
        {
            if (_stateHolder != _currentStates[CharacterStatesEnum.Jumping.GetHashCode()]
                && _stateHolder != _currentStates[CharacterStatesEnum.Hit.GetHashCode()])
                _previousState = _stateHolder;

            if (_stateHolder is SCR_BaseGroundState && _stateHolder is not SCR_LandingState &&
                _stateHolder is not SCR_DashingGroundState)
            {
                _previousGroundState = _stateHolder as SCR_BaseGroundState;
                OnPreviousStateChanged(new PreviousStateEventArgs(_previousGroundState._stateEnum));
            }
            else if (_stateHolder is SCR_BaseInAirState)
            {
                if (_stateHolder is SCR_InitiatingJumpState || _stateHolder is SCR_JumpingDoubleState ||
                    _stateHolder is SCR_DashingAirState)
                {
                    _previousInAirState =
                        _currentStates[CharacterStatesEnum.Jumping.GetHashCode()] as SCR_BaseInAirState;
                }
                else
                {
                    _previousInAirState = _stateHolder as SCR_BaseInAirState;
                }

                OnPreviousStateChanged(new PreviousStateEventArgs(_previousInAirState._stateEnum));
            }

            if (_stateHolder is SCR_BaseGroundState && value is SCR_BaseInAirState ||
                _stateHolder is SCR_BaseInAirState && value is SCR_BaseGroundState)
            {
                _stateHolder.OnStateExit();
                _stateHolder = value;
                Velocity = _stateHolder.StateSwitch(Velocity);
                for (int i = 0; i < StoredStamina; i++)
                {
                    CurrentAirMoves++;
                }

                StoredStamina = 0;
            }
            else
            {
                _stateHolder.OnStateExit();
                _stateHolder = value;
                Velocity = _stateHolder.PartialStateSwitch(Velocity);
            }
        }
        else
        {
            //_stateHolder.OnStateExit();
            _stateHolder = value;
            Velocity = _stateHolder.StateSwitch(Rigidbody.velocity);
        }

        SCR_EventHelper.TrySendEvent(StateChanged, this);
        _stateText?.SetText("State: " + _stateHolder._stateEnum);
    }

    private CooldownStatesEnum cooldownState;
    public SCR_CharacterBaseState _stateHolder;
    public List<SCR_CharacterBaseState> _currentStates = new List<SCR_CharacterBaseState>();
    public List<SCR_CharacterBaseState> _characterStates = new List<SCR_CharacterBaseState>();
    public List<SCR_CharacterBaseState> _sandCharacterStates = new List<SCR_CharacterBaseState>();
    public PlayerInput _inputActions;
    public Rigidbody Rigidbody;
    private SCR_CharacterBaseState _previousState;
    private SCR_BaseGroundState _previousGroundState;
    private SCR_BaseInAirState _previousInAirState;


    private Camera _mainCamera;
    [SerializeField] private Animator _animator;

    public Vector3 Velocity = new Vector3();
    private Vector3 _lastSprintingVeloc = new Vector3();
    public float MaxSprintingSpeed;
    private Vector2 _movementInputVector;

    public event EventHandler<DashingEventArgs> Dashing;

    public event EventHandler StateChanged;

    public event EventHandler<PreviousStateEventArgs> PreviousStateChanged;

    public event EventHandler GameOverEvent;

    public event EventHandler OpenOptionsPanelEvent;

    public event EventHandler PlayerConnectPunchEvent;

    public event EventHandler BossPhaseGettingHitEvent;

    public event EventHandler PlayerRespawnedEvent;
    public event EventHandler<CooldownDoneEventArgs> CooldownDoneEvent;

    public event EventHandler PlayerFell;

    public event EventHandler PlayerCollectedHealthEvent;

    public event EventHandler PlayerGiantSpeedBoostEvent;

    [Header("POLISH HELPER FUNCTIONS")] [SerializeField]
    private float _hitStopValue;

    [FormerlySerializedAs("_invisibilityFramesDuration")] [SerializeField]
    public float InvisibilityFramesDuration = 1.5f;

    [FormerlySerializedAs("_cinemachineVirtualCamera")] [SerializeField]
    private CinemachineFreeLook _cinemachineFreeLook;

    [SerializeField] private float _cameraShakeIntensity;

    [SerializeField] private float _cameraShakeTime;

    [SerializeField] private float _controllerRumbleLowFrequency;

    [SerializeField] private float _controllerRumbleHighFrequency;

    [SerializeField] private float _controllerRumbleDuration;

    [Header("Polish boss hit")] [SerializeField]
    private float _cameraShakeBossHitIntensity;

    [SerializeField] private float _cameraShakeBossHitTime;

    [SerializeField] private float _controllerRumbleLowBossHitFrequency;

    [SerializeField] private float _controllerRumbleHighBossHitFrequency;

    [SerializeField] private float _controllerRumbleBossHitDuration;

    [SerializeField] private float _hitStopBossHitValue;

    [SerializeField] private ParticleSystem _bossHitParticleSystem;

    //THIS BELONGS MORE IN A GAMELOOP
    public event EventHandler PlayerDead;

    [Header("BossHittingVariables")] BossKillingInput _bossKillingInput;

    [SerializeField] private Transform BossToHit;
    [SerializeField] private VisualEffect Visualisation;
    [SerializeField] private Transform HitTransform;
    public event EventHandler PunchHandler;

    [Header("Debugging UI")] [SerializeField]
    private TextMeshProUGUI _stateText;

    [SerializeField] private TextMeshProUGUI _velocityText;
    [SerializeField] private TextMeshProUGUI _velocityMagnitudeText;
    [SerializeField] private TextMeshProUGUI _inputText;
    [SerializeField] private TextMeshProUGUI _forwardText;
    public static TextMeshProUGUI ForwardText;

    public List<CinemachineBasicMultiChannelPerlin> _freeLookCameraVirtualCamerasCinemachineBasicMultiChannelPerlins =
        new List<CinemachineBasicMultiChannelPerlin>();

    public bool _isDead = false;

    public bool _isInDescentMode = false;

    #region Start

    void Awake()
    {
        Rigidbody = this.gameObject.GetComponent<Rigidbody>();
        _slidingCollider.enabled = false;
        _rollingCollider.enabled = false;
        _jumpingCollider.enabled = false;

        CurrentAirMoves = _maxAirMoves;
        CanAirDash = true;

        ForwardText = _forwardText;
        _mainCamera = Camera.main;
        /*var cinemachineBrain = _mainCamera.GetComponent<CinemachineBrain>();
 
         StateChanged += (s, e) =>
         {
             ChangeCinemachineBrainCameraUpdateSettings(cinemachineBrain);
         };*/

        SetUpInputStates();

        SetUpPlayerStates();
        SetUpSandPlayerStates();

        _currentStates = _characterStates;
        SwitchState(CharacterStatesEnum.Idle);
        
        
        CooldownDoneEvent += (s, e) =>
        {
            if (e.CooldownState == CooldownStatesEnum.IsRecoveringStamina1 ||
                e.CooldownState == CooldownStatesEnum.IsRecoveringStamina2 ||
                e.CooldownState == CooldownStatesEnum.IsRecoveringStamina3)
            {
                if (CurrentAirMoves < _maxAirMoves)
                    CurrentAirMoves++;
            }
        };

    }

    private void Start()
    {

        StartCoroutine(StaminaResetter());
        
    }

    private void SetUpInputStates()
    {
        PlayerDead += (s, e) => { _isDead = true; };

        _bossKillingInput = new BossKillingInput();
        _bossKillingInput.BossKilling.Disable();
        _bossKillingInput.BossKilling.Kill.performed += _ => SCR_EventHelper.TrySendEvent(PunchHandler, this);

        _inputActions = new PlayerInput();
        _inputActions.PlayerActionMap.Enable();
        _inputActions.MenuActionMap.Enable();
        _inputActions.PlayerActionMap.Jump.performed += _ =>
        {
            //DESCENT CHECK
            if(!_isInDescentMode)
            {
                if (!CheckIfHasCooldown(CooldownStatesEnum.NoAirJumping))
                {
                    ScrCharacterState.ActiveSwitchCheck(InputButtons.A);
                }
            }
           
        };
        _inputActions.PlayerActionMap.Jump.canceled += _ =>
        {
            if (!CheckIfHasCooldown(CooldownStatesEnum.NoAirJumping))
            {
                ScrCharacterState.ActiveSwitchCheck(InputButtons.ACancel);
            }
        };
        _inputActions.PlayerActionMap.Movement.performed += ReadMovementInput;
        _inputActions.PlayerActionMap.Movement.canceled += MovementInputCancel;
        _inputActions.PlayerActionMap.Dash.performed += _ =>
        {
            //DESCENT CHECK
            if (!_isInDescentMode)
            {
                ScrCharacterState.ActiveSwitchCheck(InputButtons.RightTrigger);

            }

        };
        _inputActions.PlayerActionMap.GroundPound.performed += _ => ScrCharacterState.ActiveSwitchCheck(InputButtons.B);
       // _inputActions.PlayerActionMap.Parry.performed += _ => ParryPerformed();
        _inputActions.MenuActionMap.OpenOptionPanel.performed +=
            _ => SCR_EventHelper.TrySendEvent(OpenOptionsPanelEvent, this);
    }

    private void ParryPerformed()
    {
        if (!CheckIfHasCooldown(CooldownStatesEnum.ParryCooldown))
        {
            Cooldown(CooldownStatesEnum.IsParrying, _parryTime);
            Cooldown(CooldownStatesEnum.ParryCooldown, _parryCooldown);
        }
    }

    public void CanHitBoss()
    {
        SCR_EventHelper.TrySendEvent(BossPhaseGettingHitEvent, this);
        _bossKillingInput.BossKilling.Enable();
        _inputActions.PlayerActionMap.Disable();
        SwitchState(CharacterStatesEnum.BossHitting);

        this.transform.position = HitTransform.position;
        var lookAt = new Vector3(HitTransform.transform.position.x, 0, HitTransform.position.z);
        this.transform.LookAt(lookAt);
    }

    //Could cause issues if we want to do something when you press a button while its on cooldown
    private async void Cooldown(CooldownStatesEnum cooldownType, float cooldownTime)
    {
        if (!(CheckIfHasCooldown(cooldownType)))
        {
            cooldownState |= cooldownType;
            await Task.Delay((int)(cooldownTime * 1000));
            if ((CheckIfHasCooldown(cooldownType)))
            {
                cooldownState &= ~cooldownType;
                SCR_EventHelper.TrySendEvent(CooldownDoneEvent, this, new CooldownDoneEventArgs(cooldownType));
            }
        }
    }


    private bool CheckIfHasCooldown(CooldownStatesEnum cooldownType)
    {
        return cooldownState == CooldownStatesEnum.Nothing ? false : (cooldownState & cooldownType) != 0;
    }

    private void SetUpPlayerStates()
    {
        try
        {
            _characterStates = new List<SCR_CharacterBaseState>();
            
            //If unexpected chages happen, check the list and if the order is right and that no states get skipped;
            SCR_CharacterBaseState state = CreateState<SCR_IdleState>(CharacterStatesEnum.Idle);
            _characterStates.Add(state);

            var stateInput = _stateInputs[CharacterStatesEnum.Sprinting.GetHashCode()];
            MaxSprintingSpeed = stateInput.maxSpeed;
            state = CreateState<SCR_SprintingState>(CharacterStatesEnum.Sprinting);
            state.OnSwitch += (s, e) => StoppedSprinting();
            _characterStates.Add(state);

            state = CreateState<SCR_JumpState>(CharacterStatesEnum.Jumping);
            _characterStates.Add(state);

            //Doen't use generic stateCreator since it has an extra variable
            state = CreateState<SCR_FallingState>(CharacterStatesEnum.Falling);
            _characterStates.Add(state);

            state = CreateState<SCR_DeadState>(CharacterStatesEnum.Dead);
            _characterStates.Add(state);

            state = CreateState<SCR_LandingState>(CharacterStatesEnum.Landing);
            _characterStates.Add(state);

            state = CreateState<SCR_FalllingOffPlatformOrReallyLongTime>(CharacterStatesEnum
                .FallingOffPlatformOrLongTime);
            _characterStates.Add(state);

            //Doen't use generic stateCreator since it has an extra variable
            state = CreateState<SCR_InitiatingJumpState>(CharacterStatesEnum.InitiatingJump);
            _characterStates.Add(state);

            state = CreateState<SCR_ApexJumpState>(CharacterStatesEnum.ApexJumpState);
            _characterStates.Add(state);

            state = CreateState<SCR_HitState>(CharacterStatesEnum.Hit);
            state.OnSwitch += (s, e) => Cooldown(CooldownStatesEnum.InvisibilityFrames, InvisibilityFramesDuration);
            _characterStates.Add(state);

            state = CreateState<SCR_GroundPoundState>(CharacterStatesEnum.GroundPound);
            _characterStates.Add(state);

            state = CreateState<SCR_JumpingDoubleState>(CharacterStatesEnum.DoubleJump);
            _characterStates.Add(state);

            state = CreateState<SCR_DashingGroundState>(CharacterStatesEnum.GroundDash);
            state.OnSwitch += (s, e) => Cooldown(CooldownStatesEnum.NoDashing, 0.05f);
            _characterStates.Add(state);

            state = CreateState<SCR_DashingAirState>(CharacterStatesEnum.AirDash);
            _characterStates.Add(state);

            state = CreateState<SCR_EndPhaseReturnState>(CharacterStatesEnum.EndPhaseReturn);
            _characterStates.Add(state);

            state = new SCR_BossHittingState(new SCR_CharacterStateCreationVariables());
            _characterStates.Add(state);


            foreach (var characterState in _characterStates)
            {
                characterState.OnSwitch += (s, e) => SwitchState(e.targetState);
                                if (characterState is IRoll)
                {
                    var rollingState = characterState as IRoll;
                    rollingState.Roll += (s, e) =>
                    {
                        if(_rollingCollider != null) _rollingCollider.enabled = false;
                        if(_standingCollider != null) _standingCollider.enabled = false;
                    };
                    rollingState.RollStopped += (s, e) =>
                    {
                        if (this.isActiveAndEnabled)
                            StartCoroutine(DelayHitBoxChange(_standingCollider, _rollingCollider, 0.5f));
                    };
                }

                if (characterState is ISlide)
                {
                    var slidingState = characterState as ISlide;
                    slidingState.Slide += (s, e) =>
                    {
                        if(_slidingCollider != null)_slidingCollider.enabled = true;
                        if(_standingCollider != null) _standingCollider.enabled = false;
                       
                    };
                    slidingState.SlideStopped += (s, e) =>
                    {
                        StartCoroutine(DelayHitBoxChange(_standingCollider, _slidingCollider, 0.5f));
                    };
                }

                if (characterState is IJump)
                {
                    var jumpState = characterState as IJump;
                    jumpState.Jump += (s, e) =>
                    {
                        if(_jumpingCollider != null)_jumpingCollider.enabled = true;
                        if(_standingCollider != null) _standingCollider.enabled = false;
                    };
                    jumpState.JumpStopped += (s, e) =>
                    {
                        if(_jumpingCollider != null)_jumpingCollider.enabled = false;
                        if(_standingCollider != null) _standingCollider.enabled = true;
                    };
                }

                if (characterState is ICheckTerrain)
                {
                    var terrainCheckingState = characterState as ICheckTerrain;
                    terrainCheckingState.SetIndexes(lightIndex, darkIndex, lightIndex);
                    terrainCheckingState.TerrainChanged += (s, e) =>
                    {
                        ChangeToSandState();
                        //Debug.Log("going to sand");
                    };
                }

                if (characterState is IUseStamina)
                {
                    var staminaUsingState = characterState as IUseStamina;
                    staminaUsingState.StaminaUsed += (s, e) =>
                    {
                        CurrentAirMoves--;
                        if (CurrentAirMoves <= 0)
                            CanAirDash = false;
                        StartStaminaRecoveringCooldown();
                    };
                }
            }
        }
        catch (ArgumentException e)
        {
            Debug.LogError(e);
            throw new ArgumentOutOfRangeException(
                "CharacterStatesEnum count and editor stateInputs count are not equal: argumentoutofrangeexception.");
        }
    }

    public void ChangeToSandState()
    {
        if (_currentStates != _sandCharacterStates)
        {
            try
            {
                var previousStates = _currentStates;
                _currentStates = _sandCharacterStates;
                if(previousStates.Contains(_stateHolder))
                    _stateHolder = _currentStates[previousStates.IndexOf(_stateHolder)];
                if(previousStates.Contains(_previousState))
                    _previousState = _currentStates[previousStates.IndexOf(_previousState)];
                if(previousStates.Contains(_previousGroundState))
                    _previousGroundState = _currentStates[previousStates.IndexOf(_previousGroundState)] as SCR_BaseGroundState;
                if(previousStates.Contains(_previousInAirState))
                    _previousInAirState = _currentStates[previousStates.IndexOf(_previousInAirState)] as SCR_BaseInAirState;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }
    }

    public void ChangeToNormalStates()
    {
        if (_currentStates != _characterStates)
        {
            try
            {
                var previousStates = _currentStates;
                _currentStates = _characterStates;
                if(previousStates.Contains(_stateHolder))
                    _stateHolder = _currentStates[previousStates.IndexOf(_stateHolder)];
                if(previousStates.Contains(_previousState))
                    _previousState = _currentStates[previousStates.IndexOf(_previousState)];
                if(previousStates.Contains(_previousGroundState))
                    _previousGroundState = _currentStates[previousStates.IndexOf(_previousGroundState)] as SCR_BaseGroundState;
                if(previousStates.Contains(_previousInAirState))
                    _previousInAirState = _currentStates[previousStates.IndexOf(_previousInAirState)] as SCR_BaseInAirState;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

        }
    }

    private void StartStaminaRecoveringCooldown()
    {
        if (CheckIfHasCooldown(CooldownStatesEnum.IsRecoveringStamina1))
        {
            if (CheckIfHasCooldown(CooldownStatesEnum.IsRecoveringStamina2))
            {
                if (CheckIfHasCooldown(CooldownStatesEnum.IsRecoveringStamina3))
                    throw new Exception("How did you get here");
                else
                    Cooldown(CooldownStatesEnum.IsRecoveringStamina3, _staminaRecoverspeed);
            }
            else
            {
                Cooldown(CooldownStatesEnum.IsRecoveringStamina2, _staminaRecoverspeed);
            }
        }
        else
        {
            Cooldown(CooldownStatesEnum.IsRecoveringStamina1, _staminaRecoverspeed);
        }
    }

    private void SetUpSandPlayerStates()
    {
        try
        {
            _sandCharacterStates = new List<SCR_CharacterBaseState>();
            
            //If unexpected chages happen, check the list and if the order is right and that no states get skipped;
            SCR_CharacterBaseState state = CreateSandState<SCR_IdleState>(CharacterStatesEnum.Idle);
            _sandCharacterStates.Add(state);

            var stateInput = _stateInputs[CharacterStatesEnum.Sprinting.GetHashCode()];
            MaxSprintingSpeed = stateInput.maxSpeed;
            state = CreateSandState<SCR_SprintingState>(CharacterStatesEnum.Sprinting);
            state.OnSwitch += (s, e) => StoppedSprinting();
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_JumpState>(CharacterStatesEnum.Jumping);
            _sandCharacterStates.Add(state);

            //Doen't use generic stateCreator since it has an extra variable
            state = CreateSandState<SCR_FallingState>(CharacterStatesEnum.Falling);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_DeadState>(CharacterStatesEnum.Dead);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_LandingState>(CharacterStatesEnum.Landing);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_FalllingOffPlatformOrReallyLongTime>(CharacterStatesEnum
                .FallingOffPlatformOrLongTime);
            _sandCharacterStates.Add(state);

            //Doen't use generic stateCreator since it has an extra variable
            state = CreateSandState<SCR_InitiatingJumpState>(CharacterStatesEnum.InitiatingJump);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_ApexJumpState>(CharacterStatesEnum.ApexJumpState);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_HitState>(CharacterStatesEnum.Hit);
            state.OnSwitch += (s, e) => Cooldown(CooldownStatesEnum.InvisibilityFrames, InvisibilityFramesDuration);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_GroundPoundState>(CharacterStatesEnum.GroundPound);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_JumpingDoubleState>(CharacterStatesEnum.DoubleJump);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_DashingGroundState>(CharacterStatesEnum.GroundDash);
            state.OnSwitch += (s, e) => Cooldown(CooldownStatesEnum.NoDashing, 0.05f);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_DashingAirState>(CharacterStatesEnum.AirDash);
            _sandCharacterStates.Add(state);

            state = CreateSandState<SCR_EndPhaseReturnState>(CharacterStatesEnum.EndPhaseReturn);
            _sandCharacterStates.Add(state);

            state = new SCR_BossHittingState(new SCR_CharacterStateCreationVariables());
            _sandCharacterStates.Add(state);


            foreach (var characterState in _sandCharacterStates)
            {
                characterState.OnSwitch += (s, e) => SwitchState(e.targetState);
                if (characterState is IRoll)
                {
                    var rollingState = characterState as IRoll;
                    rollingState.Roll += (s, e) =>
                    {
                        if(_rollingCollider != null) _rollingCollider.enabled = false;
                        if(_standingCollider != null) _standingCollider.enabled = false;
                    };
                    rollingState.RollStopped += (s, e) =>
                    {
                        if (this.isActiveAndEnabled)
                            StartCoroutine(DelayHitBoxChange(_standingCollider, _rollingCollider, 0.5f));
                    };
                }

                if (characterState is ISlide)
                {
                    var slidingState = characterState as ISlide;
                    slidingState.Slide += (s, e) =>
                    {
                        if(_slidingCollider != null)_slidingCollider.enabled = true;
                        if(_standingCollider != null) _standingCollider.enabled = false;
                       
                    };
                    slidingState.SlideStopped += (s, e) =>
                    {
                        StartCoroutine(DelayHitBoxChange(_standingCollider, _slidingCollider, 0.5f));
                    };
                }

                if (characterState is IJump)
                {
                    var jumpState = characterState as IJump;
                    jumpState.Jump += (s, e) =>
                    {
                        if(_jumpingCollider != null)_jumpingCollider.enabled = true;
                        if(_standingCollider != null) _standingCollider.enabled = false;
                    };
                    jumpState.JumpStopped += (s, e) =>
                    {
                        if(_jumpingCollider != null)_jumpingCollider.enabled = false;
                        if(_standingCollider != null) _standingCollider.enabled = true;
                    };
                }

                if (characterState is ICheckTerrain)
                {
                    var terrainCheckingState = characterState as ICheckTerrain;
                    terrainCheckingState.SetIndexes(lightIndex, darkIndex, darkIndex);
                    terrainCheckingState.TerrainChanged += (s, e) =>
                    {
                        ChangeToNormalStates();
                        //Debug.Log("going to normal");
                    };
                }

                if (characterState is IUseStamina)
                {
                    var staminaUsingState = characterState as IUseStamina;
                    staminaUsingState.StaminaUsed += (s, e) =>
                    {
                        CurrentAirMoves--;
                        if (CurrentAirMoves <= 0)
                            CanAirDash = false;
                        StartStaminaRecoveringCooldown();
                    };
                }
            }
        }
        catch (ArgumentException e)
        {
            Debug.LogError(e);
            throw new ArgumentOutOfRangeException(
                "CharacterStatesEnum count and editor stateInputs count are not equal: argumentoutofrangeexception.");
        }
    }

    private T CreateState<T>(CharacterStatesEnum statesEnum) where T : SCR_CharacterBaseState
    {
        var creationVariables = _stateInputs[statesEnum.GetHashCode()];
        return (T)Activator.CreateInstance(typeof(T), creationVariables);
    }

    private T CreateSandState<T>(CharacterStatesEnum statesEnum) where T : SCR_CharacterBaseState
    {
        var creationVariables = _stateInputs[statesEnum.GetHashCode()];
        creationVariables.maxSpeed /= 2;
        creationVariables.acceleration /= 2;
        creationVariables.maxJumpHeight /= 2;
        return (T)Activator.CreateInstance(typeof(T), creationVariables);
    }

    private void StoppedSprinting()
    {
        _lastSprintingVeloc = Velocity;
        Cooldown(CooldownStatesEnum.SprintDelay, 0.5f);
    }

    public void SwitchState(CharacterStatesEnum stateToSwitchTo)
    {
        if (CheckIfHasCooldown(CooldownStatesEnum.BackToSprinting) &&
            (stateToSwitchTo == CharacterStatesEnum.Previous || stateToSwitchTo == CharacterStatesEnum.PreviousGround))
        {
            cooldownState &= ~CooldownStatesEnum.BackToSprinting;
            Velocity = _lastSprintingVeloc;
            ScrCharacterState = _currentStates[CharacterStatesEnum.Sprinting.GetHashCode()];
        }
        else if (stateToSwitchTo == CharacterStatesEnum.Hit &&
                 CheckIfHasCooldown(CooldownStatesEnum.InvisibilityFrames)) return;
        else if (stateToSwitchTo == CharacterStatesEnum.Hit &&
                 CheckIfHasCooldown(CooldownStatesEnum.IsParrying)) return;
        else if (stateToSwitchTo == CharacterStatesEnum.DoubleJump &&
                 CheckIfHasCooldown(CooldownStatesEnum.DoubleJumpDelay)) return;
        else if (stateToSwitchTo == CharacterStatesEnum.GroundDash &&
                 CheckIfHasCooldown(CooldownStatesEnum.NoDashing)) return;
        else
        {
            switch (stateToSwitchTo)
            {
                case CharacterStatesEnum.Previous:
                    ScrCharacterState = _previousState;
                    break;
                case CharacterStatesEnum.PreviousInAir:
                    ScrCharacterState = _previousInAirState;
                    break;
                case CharacterStatesEnum.PreviousGround:
                    ScrCharacterState = _previousGroundState;
                    break;
                default:
                    ScrCharacterState = _currentStates[stateToSwitchTo.GetHashCode()];
                    break;
            }
        }

        if (CheckIfHasCooldown(CooldownStatesEnum.SprintDelay))
        {
            if (stateToSwitchTo == CharacterStatesEnum.InitiatingJump && ScrCharacterState is SCR_SprintingState)
                cooldownState |= CooldownStatesEnum.BackToSprinting;
        }

        if (stateToSwitchTo == CharacterStatesEnum.Landing)
            Cooldown(CooldownStatesEnum.NoAirJumping, 0.01f);
        if (stateToSwitchTo == CharacterStatesEnum.DoubleJump)
            Cooldown(CooldownStatesEnum.DoubleJumpDelay, 0.35f);
    }

    #endregion

    private void ReadMovementInput(CallbackContext callbackContext)
    {
        _movementInputVector = callbackContext.ReadValue<Vector2>();
    }

    private void MovementInputCancel(CallbackContext callbackContext)
    {
        _movementInputVector = Vector2.zero;
    }

    protected void OnPreviousStateChanged(PreviousStateEventArgs eventArgs)
    {
        var handler = PreviousStateChanged;
        handler?.Invoke(this, eventArgs);
    }

    public void OnPlayerGiantSpeedBoost(object source)
    {
        SCR_EventHelper.TrySendEvent(PlayerGiantSpeedBoostEvent, this);
    }

    public void PlayerIsDead()
    {
        SwitchState(CharacterStatesEnum.Dead);
        SCR_EventHelper.TrySendEvent(PlayerDead, this);
    }

    public void OnPlayerCollectedHealth(object source, EventArgs eventArgs)
    {
        SCR_EventHelper.TrySendEvent(PlayerCollectedHealthEvent, this);
    }

    public void GameOver()
    {
        SCR_EventHelper.TrySendEvent(GameOverEvent, this);
    }

    public void StartRestarting()
    {
    }

    public void OnCurrentAirMovesChanged()
    {
        SCR_EventHelper.TrySendEvent(CurrentAirMovesChangedEvent, this);
    }

    private int lastHitFrame = -1;

    public bool PlayerHit(Vector3 position, float numberOfLivesToDecrease)
    {
        if (GodMode) return true;
        // Check if the collision occurred in the current frame
        if (Time.frameCount == lastHitFrame)
            return false;
        if (CheckIfHasCooldown(CooldownStatesEnum.InvisibilityFrames)) return false;

        if (ScrCharacterState.PlayerHit())
        {
            if (CheckIfHasCooldown(CooldownStatesEnum.IsParrying))
            {
                numberOfLivesToDecrease /= 2f;
            }
            var hitstate = ScrCharacterState as SCR_HitState;
            hitstate?.HitFromPosition(position, this.transform.position);

            lastHitFrame = Time.frameCount;

            return true;
        }

        return false;
    }

    public void SpeedBoost(float boostAmount, float boostDuration)
    {
        if (boostTween != null) boostTween.Kill();
        boost = boostAmount;
        boostTween = DOTween.To(() => boost, x => boost = x, 0, boostDuration);
    }

    private float boost = 0;
    private Tween boostTween;

    private void FixedUpdate()
    {
        _inputText?.SetText("Input: " + _movementInputVector.ToString());
        Velocity = ScrCharacterState.VelocityCalculations(Velocity, _movementInputVector, transform,
            _mainCamera.transform);
        var move = new Vector3(Velocity.x, Velocity.y, Velocity.z);
        var direction = new Vector3(move.x, 0, move.z).normalized;
        move += direction * boost;
        this.transform.eulerAngles = SCR_CharacterBaseState.currentEulerRotation;

        Rigidbody.velocity = move * Time.fixedDeltaTime;

        _velocityText?.SetText("Velocity: " + Velocity.ToString());

        _velocityMagnitudeText?.SetText("Velocity Magnitude: " + Velocity.magnitude.ToString());
    }

    private void OnDisable()
    {
        UnSubFromEvents();
    }

    private void UnSubFromEvents()
    {
        SCR_EventHelper.TryUnSubFromEventHandler(ScrCharacterState.OnSwitch);
        SCR_EventHelper.TryUnSubFromEventHandler(CooldownDoneEvent);
        SCR_EventHelper.TryUnSubFromEventHandler(Dashing);
        SCR_EventHelper.TryUnSubFromEventHandler(StateChanged);
        SCR_EventHelper.TryUnSubFromEventHandler(PreviousStateChanged);
        SCR_EventHelper.TryUnSubFromEventHandler(PlayerFell);
        SCR_EventHelper.TryUnSubFromEventHandler(GameOverEvent);
        SCR_EventHelper.TryUnSubFromEventHandler(OpenOptionsPanelEvent);
        SCR_EventHelper.TryUnSubFromEventHandler(PlayerConnectPunchEvent);
        SCR_EventHelper.TryUnSubFromEventHandler(BossPhaseGettingHitEvent);
        SCR_EventHelper.TryUnSubFromEventHandler(PlayerRespawnedEvent);
        SCR_EventHelper.TryUnSubFromEventHandler(PunchHandler);
        SCR_EventHelper.TryUnSubFromEventHandler(PlayerDead);
        SCR_EventHelper.TryUnSubFromEventHandler(PlayerFell);
        SCR_EventHelper.TryUnSubFromEventHandler(PlayerGiantSpeedBoostEvent);
    }

    private IEnumerator DelayHitBoxChange(CapsuleCollider TurnOn, CapsuleCollider TurnOff, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(TurnOn != null)
            TurnOn.enabled = true;
        if(TurnOff != null)
            TurnOff.enabled = false;
    }

    private IEnumerator StaminaResetter()
    {
        var timer = 0;
        while (this.isActiveAndEnabled)
        {
            timer++;
            yield return new WaitForFixedUpdate();
            if (timer >= 30)
            {
                timer = 0;
                var onCooldown = 0;
                if (CheckIfHasCooldown(CooldownStatesEnum.IsRecoveringStamina1))
                    onCooldown++;
                if (CheckIfHasCooldown(CooldownStatesEnum.IsRecoveringStamina2))
                    onCooldown++;
                if (CheckIfHasCooldown(CooldownStatesEnum.IsRecoveringStamina3))
                    onCooldown++;
                var difference = _maxAirMoves - (CurrentAirMoves + onCooldown + StoredStamina);
                if (difference != 0)
                {
                    if (ScrCharacterState is SCR_BaseInAirState)
                        StoredStamina += difference;
                    else
                    {
                        CurrentAirMoves += difference;
                    }
                }
            }
        }
    }
}