using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class BodyFallState: FSMState
    {
        public int fallHeightCount = 0;
        protected override void init()
        {
            this.StateID = FSMStateID.BodyFall;
        }
        public override void OnStateStay(FSMBase fsm)
        {
            base.OnStateStay(fsm);
            bool isLegal = true;
            //Vector2 direction = Vector2.Zero;
            PlayerFSM player = (fsm as PlayerFSM);
            int width = player._width;
            PlantBase grabPlant = player.grabPlant;
            bool isPlantedRoot = false;
            grabPlant?.GetAllConnectedParts<Root>().ForEach(p => {
                if(p.GetSurroundingNode(Vector2.Zero, 1) != null) {
                    isPlantedRoot = true;
                }
            });
            if(grabPlant == null) {
                while(player.GetSurroundingNode(Vector2.Down * width * 2, 1) == null && player.GetSurroundingNode(Vector2.Down * width * 2, 2) == null) {
                    fallHeightCount++ ;
                    GD.Print(fallHeightCount);
                    GD.Print(player.GlobalPosition);
                    player.GlobalPosition += Vector2.Down * width;
                    GD.Print(player.GlobalPosition);
                }
                player.GlobalPosition += Vector2.Down * width * fallHeightCount;
            }else {
                List<Root> rootPlants = grabPlant.GetAllConnectedParts<Root>();
                List<PlantBase> plants = grabPlant.GetAllConnectedParts();
                rootPlants.ForEach(p => plants.Remove(p));
                
                if(player.GetSurroundingNode(Vector2.Down * width * 2, 1) == null && player.GetSurroundingNode(Vector2.Down * width * 2, 2) == null) {
                    plants.ForEach(p => {
                        if(p.GetSurroundingNode(Vector2.Down * width, 1) == null && p.GetSurroundingNode(Vector2.Down * width, 2) == null
                        || (p.GetSurroundingNode(Vector2.Down * width, 2) != null && (plants.Any(q => q == p.GetSurroundingNode(Vector2.Down * width, 2)) 
                        || rootPlants.Any(q => q == p.GetSurroundingNode(Vector2.Down * width, 2))))) {
                            fallHeightCount++ ;
                            p.GlobalPosition += Vector2.Down * width;
                        }else {
                            isLegal = false;
                        }
                    });
                }else {
                    return;
                }
            }
        }
    }
}