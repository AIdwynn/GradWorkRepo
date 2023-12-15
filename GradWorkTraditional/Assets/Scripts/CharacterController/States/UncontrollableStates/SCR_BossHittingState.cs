using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_BossHittingState : SCR_BaseControllesState
    {
        public SCR_BossHittingState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override Vector3 StateSwitch(Vector3 velocity)
        {
            return Vector3.zero;
        }

        public override void OnStateExit()
        {
            
        }
    }
}