using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class ClimbToWalkTrigger: FSMTrigger
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
            Vector2 position = player.GlobalPosition;
            if(Input.IsActionJustPressed("move_left")) {
                direction = Vector2.Left;
            }else if(Input.IsActionJustPressed("move_right")) {
                direction = Vector2.Right;
            }else if(Input.IsActionJustPressed("move_up")) {
                direction = Vector2.Left;
            }else if(Input.IsActionJustPressed("move_down")) {
                direction = Vector2.Right;
            }
            bool isPlantedRoot = false;
            grabPlant.GetAllConnectedParts<Root>().ForEach(p => {
                if(p.GetSurroundingNode(Vector2.Zero, 1) != null) {
                    isPlantedRoot = true;
                }
            });
            if(!isPlantedRoot) return false;
            if(direction == Vector2.Left || direction == Vector2.Right) {
                if(player.GetSurroundingNode(position + direction, 2) == null && player.GetSurroundingNode(position + direction, 1) == null
                && player.GetSurroundingNode(position + direction + Vector2.Down, 1) == null) {
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    player.GlobalPosition += direction * width;
                    return true;
                }else {
                    return false;
                }
            }else if(direction == Vector2.Up) {
                if(player.GetSurroundingNode(position + direction, 1) == null && player.GetSurroundingNode(position, 2) == null) {
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    player.GlobalPosition += direction * width;
                    return true;
                }else {
                    return false;
                }
            }else if(direction == Vector2.Down) {
                if(player.GetSurroundingNode(position + direction * 2, 1) != null || player.GetSurroundingNode(position + direction * 2, 2) != null) {
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    return true;
                }
                if(player.GetSurroundingNode(position + direction * 2, 1) == null && player.GetSurroundingNode(position + direction * 2, 2) == null) {
                    UndoManager.Instance.Save();
                    RoundManager.Instance.OnRoundFinish();
                    player.GlobalPosition += direction * width;
                    return true;
                }else {
                    return false;
                }
            }else {
                return false;
            }
        }
    }
}