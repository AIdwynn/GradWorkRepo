using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    internal class SCR_FalllingOffPlatformOrReallyLongTime : SCR_BaseInAirState
    {
        public SCR_FalllingOffPlatformOrReallyLongTime(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override void OnStateExit()
        {
        }
        
        public override Vector3 StateSwitch(Vector3 velocity)
        {
            _landingDistance = 1;
            _StateToSwitchTo = CharacterStatesEnum.Landing;
            return velocity;
        }
    }
}
