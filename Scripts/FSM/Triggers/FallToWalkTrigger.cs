using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class FallToWalkTrigger: FSMTrigger
    {
        protected override void init()
        {
            this.TriggerID = FSMTriggerID.FallToWalk;
        }
        public override bool HandleTrigger(FSMBase fsm)
        {
            PlayerFSM player = (fsm as PlayerFSM);
            if(player._currentState is BodyFallState) {
                BodyFallState fallBody = player._currentState as BodyFallState;
                if(fallBody.fallHeightCount < 4) {
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    player.GlobalPosition += Vector2.Down * player._width * fallBody.fallHeightCount;
                    return true;
                }
            }
            return false;
        }
    }
}