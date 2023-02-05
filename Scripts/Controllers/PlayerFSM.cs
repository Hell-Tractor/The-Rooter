using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace AI.FSM
{
    public class PlayerFSM: FSMBase, ISave
    {
        public int _width;
        public PlantBase grabPlant = null;
        public override void _Ready() {
            base._Ready();          
		    Sprite sprite = this.GetNode<Sprite>("Sprite");
		    this._width = Mathf.Abs((int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x));
	    }
        public override void _Process(float delta)
        {
            base._Process(delta);/*
            if(this.GetOverlappingAreas().Count >= 1){
			    grabPlant = this.GetOverlappingAreas()[0] as PlantBase;
		    }*/
            if(Input.IsActionJustPressed("undo")) {
                UndoManager.Instance.Load();
            }
            if(Input.IsActionJustPressed("restart")) {
                this.GetTree().ReloadCurrentScene();
            }
        }
        public Dictionary<string, object> Save() {
		    return new Dictionary<string, object>(){
			    { "Filename", this.Filename },
                { "Parent", this.GetParent().GetPath() },
			    { "GlobalPosition", this.GlobalPosition },
                { "grabPlant", this.grabPlant },
                { "_currentState", this._currentState}
		    };
        }
        public void Load(Dictionary<string, object> data) {
		    this.GlobalPosition = (Vector2)data["GlobalPosition"];
            this._currentState = (FSMState)data["_currentState"];
            if(data["grabPlant"] != null)
                this.grabPlant = (PlantBase)data["grabPlant"];
	    }
        public Area2D GetSurroundingNode(Vector2 direction, uint layer) {
            Vector2 endPoint = new Vector2(GlobalPosition.x + direction.x + 1, GlobalPosition.y + direction.y + 1);
            var result = this.GetWorld2d().DirectSpaceState.IntersectRay(GlobalPosition + direction, endPoint, null, layer, false ,true);
            if(result.Count == 0)
                return null;
            else
                return result["collider"] as Area2D;
        }

        protected override void setUpFSM()
        {
            base.setUpFSM();

            BodyWalkState walkState = new BodyWalkState();
            walkState.AddMap(FSMTriggerID.WalkToClimb, FSMStateID.BodyClimb);
            walkState.AddMap(FSMTriggerID.WalkToFall, FSMStateID.BodyFall);
            _states.Add(walkState);

            BodyClimbState climbState = new BodyClimbState();
            climbState.AddMap(FSMTriggerID.ClimbToWalk, FSMStateID.BodyWalk);
            _states.Add(climbState);

            BodyFallState fallState = new BodyFallState();
            fallState.AddMap(FSMTriggerID.FallToWalk, FSMStateID.BodyWalk);
            fallState.AddMap(FSMTriggerID.FallToDeath, FSMStateID.BodyDeath);
            _states.Add(fallState);

            BodyDeathState deathState = new BodyDeathState();
            _states.Add(deathState);
        }
    }
}