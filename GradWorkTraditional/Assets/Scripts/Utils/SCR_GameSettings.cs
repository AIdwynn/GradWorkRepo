using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class SCR_GameSettings
{
    //CHANGED IT FOR STRAIGHTLINE PROTOTYPE
   // public static float PlayerAirGravity = -6f;

    public static float PlayerAirGravity = -50;
    public static float PlayerJumpGravity = -16f;
    public static float PlayerGroundGravity = -100;
    public static float GroundPoundGravity = -200;
    
    //WILL BE USED LATER FOR DIFFERENCE IN PLAYER SPEED COMPARED TO ENVIRONMENT/BULLETS WILL NEED A TIME MULTIPLIER FOR ENVIRONMENT AS WELL
    public static float CustomTimeMultiplierPlayer = 1f;
    public static float TimeMultiplier { get { return CustomTimeMultiplierPlayer * Time.fixedDeltaTime / Time.timeScale; } }

    public static int WalkableObjectLayers = 7;
    public static int PlayerLayer = 6;
    
    
}
