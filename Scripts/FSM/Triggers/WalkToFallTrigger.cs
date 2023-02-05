using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class WalkToFallTrigger: FSMTrigger
    {
        protected override void init()
        {
            this.TriggerID = FSMTriggerID.WalkToFall;
        }
        public override bool HandleTrigger(FSMBase fsm)
        {
            Vector2 direction = Vector2.Zero;
            PlayerFSM player = (fsm as PlayerFSM);
            int width = player._width;
            PlantBase grabPlant = player.grabPlant;
            //Vector2 position = player.GlobalPosition;
            if(player.GetSurroundingNode(Vector2.Down * 2 * width, 2) == null) {
                return true;
            }else {
                return false;
            }
        }
    }
}