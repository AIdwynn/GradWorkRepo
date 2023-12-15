using System.Collections;
using System.Collections.Generic;
using StateCharacterController.CharacterStates;
using UnityEngine;

public class SCR_EndPhaseReturnState : SCR_BaseControllesState
{
    private bool firstTime = true;
    private float timer = 0;
    public SCR_EndPhaseReturnState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
    {
    }

    public override Vector3 VelocityCalculations(Vector3 velocity, Vector2 input, Transform playerTransform, Transform cameraTransform)
    {
        timer += Time.fixedDeltaTime;
        if (timer > _acceleration)
        {
            ThrowSwitchEvent(CharacterStatesEnum.Idle);
        }
        velocity.y += SCR_GameSettings.PlayerAirGravity;
        return velocity;
    }

    public override Vector3 StateSwitch(Vector3 velocity)
    {
        if (firstTime)
        {
            firstTime = false;
            return Vector3.zero;
        }

        var positionNoY = new Vector3(0, 0, 0);
        positionNoY.y = 0;
        velocity.y = 0;
        
        var direction = (velocity - positionNoY).normalized;
        var distance = Vector3.Distance(velocity, positionNoY);
        
        var returnVelocity = (distance/_acceleration) * direction;
        returnVelocity.y = - ((SCR_GameSettings.PlayerAirGravity*_acceleration)/2);
        
        return returnVelocity/Time.fixedDeltaTime;
    }

    public override void OnStateExit()
    {
        timer = 0;
        firstTime = true;
    }
}
