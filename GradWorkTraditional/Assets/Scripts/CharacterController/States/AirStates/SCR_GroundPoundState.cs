using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_GroundPoundState : SCR_BaseInAirState
    {
        public SCR_GroundPoundState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }


        public override Vector3 StateSwitch(Vector3 velocity)
        {
            gravity = SCR_GameSettings.GroundPoundGravity;
            return velocity;
        }

        public override void ActiveSwitchCheck(InputButtons input)
        {
        }

        public override void OnStateExit()
        {
        }
    }
}