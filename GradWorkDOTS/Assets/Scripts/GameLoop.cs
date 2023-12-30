using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop
{
    public static GameLoop Instance;
    public SCRMO_CharacterController player;

    public GameLoop()
    {
        Instance = this;
        player = Object.FindObjectOfType<SCRMO_CharacterController>();
    }
}
