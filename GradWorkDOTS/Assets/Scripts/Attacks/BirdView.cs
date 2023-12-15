using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdView : MonoBehaviour
{
    public event EventHandler HitEvent;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
            SCR_EventHelper.TrySendEvent(HitEvent, this);
    }
}
