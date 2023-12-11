using System;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCRMO_SpeedBoostGiver : MonoBehaviour
    {
        [SerializeField] int _speedBoost = 200;
        [SerializeField] private float _speedBoostDuration = 1f;

        private void Start()
        {
            
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == SCR_GameSettings.PlayerLayer)
            {
            }
        }
    }
}