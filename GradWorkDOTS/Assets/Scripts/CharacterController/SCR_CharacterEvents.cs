using System;
using UnityEngine;

public class DashingEventArgs : EventArgs
{
    public Vector2 Input;
    public Transform CameraTransform;

    public DashingEventArgs(PlayerInput inputActions, Transform cameraTransform)
    {
        Input = inputActions.PlayerActionMap.Movement.ReadValue<Vector2>();
        CameraTransform = cameraTransform;
    }
}

public class PreviousStateEventArgs : EventArgs
{
    public CharacterStatesEnum State;

    public PreviousStateEventArgs(CharacterStatesEnum state)
    {
        State = state;
    }
}

public class CooldownDoneEventArgs : EventArgs
{
    public CooldownStatesEnum CooldownState;
    
    public CooldownDoneEventArgs(CooldownStatesEnum cooldownState)
    {
        CooldownState = cooldownState;
    }
}

public class EventSwitchArgs : EventArgs
{
    public CharacterStatesEnum targetState;

    public EventSwitchArgs(CharacterStatesEnum targetState)
    {
        this.targetState = targetState;
    }
}