using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SCR_CharacterStateCreationVariables
{
    public CharacterStatesEnum statesEnum;
    public float acceleration = 0;
    public float movingFriction = 0;
    public float stoppingFriction = 0;
    public float maxSpeed = 0;
    public float rotateSpeed = 0;
    public float transitionToReallyLongFallTime = 0;
    public float maxJumpHeight = 0;
}
