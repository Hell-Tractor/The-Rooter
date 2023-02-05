using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class FallToDeathTrigger: FSMTrigger
    {
        protected override void init()
        {
            this.TriggerID = FSMTriggerID.FallToDeath;
        }
        public override bool HandleTrigger(FSMBase fsm)
        {
            PlayerFSM player = (fsm as PlayerFSM);
            if(player._currentState is BodyFallState) {
                BodyFallState fallBody = player._currentState as BodyFallState;
                if(fallBody.fallHeightCount >= 4) {
                    fallBody.fallHeightCount = 0;
                    GD.Print("dead");
                    return true;
                }
            }
            return false;
        }
    }
}