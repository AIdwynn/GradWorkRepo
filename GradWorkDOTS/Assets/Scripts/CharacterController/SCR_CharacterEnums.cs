using System;

public enum CharacterStatesEnum
{
    Idle = 0,
    Sprinting = 1,
    Jumping = 2,
    Falling = 3,
    Dead = 4,
    Landing = 5,
    FallingOffPlatformOrLongTime = 6,
    InitiatingJump = 7,
    ApexJumpState = 8,
    Hit = 9,
    GroundPound = 10,
    DoubleJump = 11,
    GroundDash = 12,
    AirDash = 13,
    EndPhaseReturn = 14,
    
    BossHitting,
    Previous,
    PreviousGround,
    PreviousInAir,



}

public enum InputButtons
{
    A, X, B, Y,
    
    
    ACancel, XCancel, BCancel, YCancel,
    LeftTrigger,
    RightTrigger,
    LeftBumper,
    RightBumper,
    LeftStick,
    RightStick,

}

[Flags]
public enum CooldownStatesEnum
{
    Nothing = 0, NoDashing = 1, 
NoAirJumping = 2, BackToSprinting = 4,
SprintDelay = 8, InvisibilityFrames = 16,
DoubleJumpDelay = 32, ParryCooldown = 64, 
IsParrying = 128, IsRecoveringStamina1 = 256,
IsRecoveringStamina2 = 512, IsRecoveringStamina3 = 1024
}
