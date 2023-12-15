
using System;
using UnityEngine;

namespace StateCharacterController.CharacterStates
{
    public class SCR_JumpingDoubleState : SCR_BaseJumpingState, IUseStamina
    {
        public SCR_JumpingDoubleState(SCR_CharacterStateCreationVariables creationVariables) : base(creationVariables)
        {
        }

        public override Vector3 StateSwitch(Vector3 velocity)
        {
            SCR_EventHelper.TrySendEvent(StaminaUsed, this);
            return base.StateSwitch(velocity);
        }

        public event EventHandler StaminaUsed;
    }
}

