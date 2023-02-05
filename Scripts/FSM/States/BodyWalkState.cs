using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace AI.FSM
{
    public class BodyWalkState: FSMState
    {
        
        protected override void init()
        {
            this.StateID = FSMStateID.BodyWalk;
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
                direction = Vector2.Left;
            }else if(Input.IsActionJustPressed("move_down")) {
                direction = Vector2.Right;
            }
            if(direction == Vector2.Left || direction == Vector2.Right) {
                if(grabPlant == null) {
                    if(player.GetSurroundingNode(direction * width, 1) == null) {
                    //with no plant move on horizon
                        if(player.GetSurroundingNode((direction + Vector2.Up) * width, 1) == null && player.GetSurroundingNode(Vector2.Up * width, 1) == null) {
                            UndoManager.Instance.Save();
                            RoundManager.Instance.OnRoundFinish();
                            player.GlobalPosition += (direction + Vector2.Up) * width;
                        }else {
                            isLegal = false;
                            return;
                        }   
                    }else {
                        UndoManager.Instance.Save();
                        RoundManager.Instance.OnRoundFinish();
                        player.GlobalPosition += direction * width;
                    }
                }
                else {
                    List<PlantBase> plants = grabPlant.GetAllConnectedParts();
                    plants.ForEach(p => {
                        if((p.GetSurroundingNode(direction * width, 2) != null && !plants.Any(q => q==p.GetSurroundingNode(direction * width, 2))) 
                        || p.GetSurroundingNode(direction * width, 1) != null) {
                            isLegal = false;
                            return;
                        }
                    });
                    if(isLegal == true) {
                        UndoManager.Instance.Save();
                        RoundManager.Instance.OnRoundFinish();
                        plants.ForEach(p => {p.GlobalPosition += direction * width;});
                    }
                        
                    else {
                        plants.ForEach(p => {
                            if((p.GetSurroundingNode((direction + Vector2.Up) * width, 2) != null && !plants.Any(q => q==p.GetSurroundingNode((direction + Vector2.Up) * width, 2)))|| p.GetSurroundingNode((direction + Vector2.Up) * width, 1) != null
                            || (p.GetSurroundingNode(Vector2.Up * width, 2) != null && !plants.Any(q => q==p.GetSurroundingNode(Vector2.Up * width, 2))) || p.GetSurroundingNode(Vector2.Up * width, 1) != null) {
                                isLegal = false;
                                return;
                            }
                        });
                        UndoManager.Instance.Save();
                        RoundManager.Instance.OnRoundFinish();
                        plants.ForEach(p => {p.GlobalPosition += (Vector2.Up + direction) * width;});
                    }
                }
            }else if(direction == Vector2.Up && grabPlant != null) {
                if(player.GetSurroundingNode(direction * width, 1) == null && player.GetSurroundingNode(direction * width, 2) == null) {
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    player.GlobalPosition += direction * width;
                }else {
                    isLegal = false;
                    return;
                }
            }

        }
    }
}