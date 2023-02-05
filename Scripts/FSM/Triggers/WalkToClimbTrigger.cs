using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class WalkToClimbTrigger: FSMTrigger
    {
        protected override void init()
        {
            this.TriggerID = FSMTriggerID.WalkToClimb;
        }
        public override bool HandleTrigger(FSMBase fsm)
        {
            Vector2 direction = Vector2.Zero;
            PlayerFSM player = (fsm as PlayerFSM);
            int width = player._width;
            PlantBase grabPlant = player.grabPlant;
            //Vector2 position = player.GlobalPosition;
            if(Input.IsActionJustPressed("move_up")) {
                direction = Vector2.Up;
            }else if(Input.IsActionJustPressed("move_down")) {
                direction = Vector2.Down;
            }else {
                return false;
            }
            if(direction != Vector2.Zero && grabPlant != null) {
                if(player.GetSurroundingNode(direction * width, 1) == null) {
                    if(direction == Vector2.Up && player.GetSurroundingNode(Vector2.Up * width, 2) == null) {
                        //play hold
                    }else {
                        //play climb
                    }
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    //player.GlobalPosition += direction * width;
                    return true;
                }
            }
            return false;
        }
    }
}