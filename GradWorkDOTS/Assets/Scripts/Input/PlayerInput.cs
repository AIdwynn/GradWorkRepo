//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Scripts/Input/PlayerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""PlayerActionMap"",
            ""id"": ""4ecc1cc0-2939-407b-a9f7-d9d631f14340"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""f4d60751-457e-4e12-b6f0-137d570924d6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""73006b99-ac17-4fbf-8071-16a196f2ba74"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""28e54964-72c1-4adf-9d62-a78e92830f2d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CameraRotate"",
                    ""type"": ""Value"",
                    ""id"": ""26d6a1ea-709f-4ef9-ad4d-54efeb79ad77"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""GroundPound"",
                    ""type"": ""Button"",
                    ""id"": ""48e56949-3d23-4eeb-8d1f-17e5e9bae98c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Parry"",
                    ""type"": ""Button"",
                    ""id"": ""71c62044-04ef-434d-914f-f23586c2de2a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b3ac5c54-935f-4844-bdb9-25b2e63c39d7"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""KeyBoard"",
                    ""id"": ""b0058598-e6f2-40ea-b0ac-e68e9aad21b9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b2f586ce-8985-4095-89a7-8da60bee6593"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""K&M"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4690cfa3-c626-4488-86e7-ca92907cc714"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""K&M"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bad2804b-e73b-4987-9b5d-ab3eea4a2d8b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""K&M"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""52595d6f-b700-4b39-ad3e-e625f4c9033b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""K&M"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""KeyBoard"",
                    ""id"": ""79b2ec84-dee5-4240-b2ec-22b2de500853"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d2f880d6-e5e2-48b4-a609-a2baf5040f69"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4eeffcb4-b1d3-4649-95f4-8667b12e8d05"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2bbb7938-02dd-496e-a30a-eb45c787050c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""15b9f421-d609-4107-9cd4-c7618c94d223"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""72c5f2e1-dee8-43a7-8853-3357233eae93"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60a67d4b-e0d0-44a7-a38e-2450ffb2a076"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""K&M"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6fbc9d05-3543-4e7a-bae5-c6ee543c8156"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f22e3aa5-111d-43c4-8cc1-d00007a7e5c2"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""K&M"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f3d2cd5f-5374-40c0-845d-655a7e7d9a16"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Controller"",
                    ""action"": ""CameraRotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""80941ce7-a870-4512-b119-8cd373083c43"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""GroundPound"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35d69a14-2d61-483e-ae91-c621189f5d5f"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""K&M"",
                    ""action"": ""GroundPound"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23a984a7-fe36-4def-9613-fe486246c361"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Parry"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MenuActionMap"",
            ""id"": ""0e4fab8c-663c-4e97-b686-00562c6e07fb"",
            ""actions"": [
                {
                    ""name"": ""OpenOptionPanel"",
                    ""type"": ""Button"",
                    ""id"": ""a33055f1-0214-4763-8944-06ce034dbdaf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""14f49aaf-7b0e-47de-8020-31580360ce08"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""OpenOptionPanel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""K&M"",
            ""bindingGroup"": ""K&M"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerActionMap
        m_PlayerActionMap = asset.FindActionMap("PlayerActionMap", throwIfNotFound: true);
        m_PlayerActionMap_Movement = m_PlayerActionMap.FindAction("Movement", throwIfNotFound: true);
        m_PlayerActionMap_Jump = m_PlayerActionMap.FindAction("Jump", throwIfNotFound: true);
        m_PlayerActionMap_Dash = m_PlayerActionMap.FindAction("Dash", throwIfNotFound: true);
        m_PlayerActionMap_CameraRotate = m_PlayerActionMap.FindAction("CameraRotate", throwIfNotFound: true);
        m_PlayerActionMap_GroundPound = m_PlayerActionMap.FindAction("GroundPound", throwIfNotFound: true);
        m_PlayerActionMap_Parry = m_PlayerActionMap.FindAction("Parry", throwIfNotFound: true);
        // MenuActionMap
        m_MenuActionMap = asset.FindActionMap("MenuActionMap", throwIfNotFound: true);
        m_MenuActionMap_OpenOptionPanel = m_MenuActionMap.FindAction("OpenOptionPanel", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerActionMap
    private readonly InputActionMap m_PlayerActionMap;
    private List<IPlayerActionMapActions> m_PlayerActionMapActionsCallbackInterfaces = new List<IPlayerActionMapActions>();
    private readonly InputAction m_PlayerActionMap_Movement;
    private readonly InputAction m_PlayerActionMap_Jump;
    private readonly InputAction m_PlayerActionMap_Dash;
    private readonly InputAction m_PlayerActionMap_CameraRotate;
    private readonly InputAction m_PlayerActionMap_GroundPound;
    private readonly InputAction m_PlayerActionMap_Parry;
    public struct PlayerActionMapActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActionMapActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PlayerActionMap_Movement;
        public InputAction @Jump => m_Wrapper.m_PlayerActionMap_Jump;
        public InputAction @Dash => m_Wrapper.m_PlayerActionMap_Dash;
        public InputAction @CameraRotate => m_Wrapper.m_PlayerActionMap_CameraRotate;
        public InputAction @GroundPound => m_Wrapper.m_PlayerActionMap_GroundPound;
        public InputAction @Parry => m_Wrapper.m_PlayerActionMap_Parry;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionMapActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActionMapActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @CameraRotate.started += instance.OnCameraRotate;
            @CameraRotate.performed += instance.OnCameraRotate;
            @CameraRotate.canceled += instance.OnCameraRotate;
            @GroundPound.started += instance.OnGroundPound;
            @GroundPound.performed += instance.OnGroundPound;
            @GroundPound.canceled += instance.OnGroundPound;
            @Parry.started += instance.OnParry;
            @Parry.performed += instance.OnParry;
            @Parry.canceled += instance.OnParry;
        }

        private void UnregisterCallbacks(IPlayerActionMapActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @CameraRotate.started -= instance.OnCameraRotate;
            @CameraRotate.performed -= instance.OnCameraRotate;
            @CameraRotate.canceled -= instance.OnCameraRotate;
            @GroundPound.started -= instance.OnGroundPound;
            @GroundPound.performed -= instance.OnGroundPound;
            @GroundPound.canceled -= instance.OnGroundPound;
            @Parry.started -= instance.OnParry;
            @Parry.performed -= instance.OnParry;
            @Parry.canceled -= instance.OnParry;
        }

        public void RemoveCallbacks(IPlayerActionMapActions instance)
        {
            if (m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActionMapActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActionMapActions @PlayerActionMap => new PlayerActionMapActions(this);

    // MenuActionMap
    private readonly InputActionMap m_MenuActionMap;
    private List<IMenuActionMapActions> m_MenuActionMapActionsCallbackInterfaces = new List<IMenuActionMapActions>();
    private readonly InputAction m_MenuActionMap_OpenOptionPanel;
    public struct MenuActionMapActions
    {
        private @PlayerInput m_Wrapper;
        public MenuActionMapActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @OpenOptionPanel => m_Wrapper.m_MenuActionMap_OpenOptionPanel;
        public InputActionMap Get() { return m_Wrapper.m_MenuActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActionMapActions set) { return set.Get(); }
        public void AddCallbacks(IMenuActionMapActions instance)
        {
            if (instance == null || m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Add(instance);
            @OpenOptionPanel.started += instance.OnOpenOptionPanel;
            @OpenOptionPanel.performed += instance.OnOpenOptionPanel;
            @OpenOptionPanel.canceled += instance.OnOpenOptionPanel;
        }

        private void UnregisterCallbacks(IMenuActionMapActions instance)
        {
            @OpenOptionPanel.started -= instance.OnOpenOptionPanel;
            @OpenOptionPanel.performed -= instance.OnOpenOptionPanel;
            @OpenOptionPanel.canceled -= instance.OnOpenOptionPanel;
        }

        public void RemoveCallbacks(IMenuActionMapActions instance)
        {
            if (m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMenuActionMapActions instance)
        {
            foreach (var item in m_Wrapper.m_MenuActionMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MenuActionMapActions @MenuActionMap => new MenuActionMapActions(this);
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    private int m_KMSchemeIndex = -1;
    public InputControlScheme KMScheme
    {
        get
        {
            if (m_KMSchemeIndex == -1) m_KMSchemeIndex = asset.FindControlSchemeIndex("K&M");
            return asset.controlSchemes[m_KMSchemeIndex];
        }
    }
    public interface IPlayerActionMapActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnCameraRotate(InputAction.CallbackContext context);
        void OnGroundPound(InputAction.CallbackContext context);
        void OnParry(InputAction.CallbackContext context);
    }
    public interface IMenuActionMapActions
    {
        void OnOpenOptionPanel(InputAction.CallbackContext context);
    }
}
