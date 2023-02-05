using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class BodyClimbState: FSMState
    {
        protected override void init()
        {
            this.StateID = FSMStateID.BodyClimb;
        }
        public override void OnStateEnter(FSMBase fsm)
        {
            base.OnStateEnter(fsm);
            PlayerFSM player = (fsm as PlayerFSM);
            if(player.GetOverlappingAreas().Count >= 1){
			    player.grabPlant = player.GetOverlappingAreas()[0] as PlantBase;
		    }
            else {
                player.grabPlant = null;
            }
        }
        public override void OnStateStay(FSMBase fsm)
        {
            base.OnStateStay(fsm);
            bool isLegal = true;
            Vector2 direction = Vector2.Zero;
            PlayerFSM player = (fsm as PlayerFSM);
            int width = player._width;
            PlantBase grabPlant = player.grabPlant;
            //Vector2 position = player.GlobalPosition;
            if(Input.IsActionJustPressed("move_left")) {
                direction = Vector2.Left;
            }else if(Input.IsActionJustPressed("move_right")) {
                direction = Vector2.Right;
            }else if(Input.IsActionJustPressed("move_up")) {
                direction = Vector2.Up;
            }else if(Input.IsActionJustPressed("move_down")) {
                direction = Vector2.Down;
            }

            bool isPlantedRoot = false;
            grabPlant.GetAllConnectedParts<Root>().ForEach(p => {
                if(p.GetSurroundingNode(Vector2.Zero, 1) != null) {
                    isPlantedRoot = true;
                }
            });
            if(isPlantedRoot) {
                if(direction == Vector2.Down || direction == Vector2.Up && player.GetSurroundingNode(direction * width, 1) == null) {
                    if(direction == Vector2.Up && player.GetSurroundingNode(direction * width, 2) == null) {
                        //play hold
                    }else {
                        //play climb
                    }
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    player.GlobalPosition += direction * width;
                }else if(direction == Vector2.Right || direction == Vector2.Left && player.GetSurroundingNode(direction * width, 1) == null 
                && player.GetSurroundingNode(direction + Vector2.Down * width, 1) == null && player.GetSurroundingNode(direction * width, 2) != null) {
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    player.GlobalPosition += direction * width;
                }else {
                    isLegal = false;
                    return;
                }
            }else {
                List<PlantBase> plants = grabPlant.GetAllConnectedParts();
                if(direction == Vector2.Left || direction == Vector2.Right) {
                    plants.ForEach(p => {
                        if(p.GetSurroundingNode(p.GlobalPosition + direction * width, 2) != null && !plants.Any(q => q == p.GetSurroundingNode(p.GlobalPosition + direction * width, 2))) {
                            isLegal = false;
                            return;
                        }
                    });
                    plants.ForEach(p =>{
                        UndoManager.Instance.Save();
                        RoundManager.Instance.OnRoundFinish();
                        p.GlobalPosition += direction * width;
                    });
                }
            }
        }
    }
}