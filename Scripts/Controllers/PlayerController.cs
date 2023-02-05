using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using AI.FSM;

public class PlayerController : Area2D, ISave {
	private int _width;
	private RoundManager roundManager;
	public PlantBase grabPlant;
	private UndoManager undoManager;
	public enum BodyStateType{
		walk,
		climb,
		hold,
		fall,
		dead
	}
	public enum HandStateType{
		vertCrab,
		horiCrab,
		unCrab
	}
	public BodyStateType bodyType {get; private set;} = BodyStateType.walk;
	public HandStateType handType {get; private set;} = HandStateType.unCrab;

	public Dictionary<string, object> Save() {
		return new Dictionary<string, object>(){
			{ "Filename", this.Filename },
            { "Parent", this.GetParent().GetPath() },
			{ "Position", this.Position },
			{ "bodyType", this.bodyType },
			{ "handType", this.handType },
			{ "grabPlant", this.grabPlant?.GetPath() }
		};
	}

	public void Load(Dictionary<string, object> data) {
		GD.Print(this.Position);
		this.Position = (Vector2)data["Position"];
		this.bodyType = (BodyStateType)data["bodyType"];
		this.handType = (HandStateType)data["handType"];
		if(data["grabPlant"] != null)
			this.grabPlant = this.GetNode<PlantBase>(data["grabPlant"].ToString());
		
		GD.Print((Vector2)data["Position"]);
	}

	public override void _Ready() {
		Sprite sprite = this.GetNode<Sprite>("Sprite");
		this._width = Mathf.Abs((int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x));
		this.undoManager = UndoManager.Instance;
		this.roundManager = RoundManager.Instance;
	}

	public override void _Process(float delta) {
		base._Process(delta);

		Timer moveTimer = this.GetNode<Timer>("MoveTimer");
		/*
		if (moveTimer.IsStopped()) {
			// * -------on Round-------
			if(this._processMove()) {
				
				this.roundManager.OnRoundFinish();
			}
			moveTimer.Start();
			// * ---------------------------------------
		
		}*/
		if(this._processMove()) {
			this.roundManager.OnRoundFinish();
		}
		if(Input.IsActionJustPressed("undo")) {
			GD.Print("Z pressed");
			undoManager.Load();
		}
		if(Input.IsActionJustPressed("restart")) {
			this.GetTree().ReloadCurrentScene();
		}
	}

	private bool _processMove() {
		bool isLegal = false;
		int horizontalVelocity = 0;
		int verticalVelocity = 0;
		Vector2 direction = Vector2.Zero;
		Vector2 plantDirection = Vector2.Zero;
		List<PlantBase> plants = new List<PlantBase>();
		if(grabPlant != null)
			plants = grabPlant.GetAllConnectedParts();
		if(Input.IsActionJustPressed("move_left")) {
            direction = Vector2.Left;
			isLegal = true;
        }else if(Input.IsActionJustPressed("move_right")) {
            direction = Vector2.Right;
			isLegal = true;
        }else if(Input.IsActionJustPressed("move_up")) {
            direction = Vector2.Up;
			isLegal = true;
        }else if(Input.IsActionJustPressed("move_down")) {
            direction = Vector2.Down;
			isLegal = true;
        }else if(Input.IsActionJustPressed("grab")) {
			if(grabPlant != null) {
				grabPlant = null;
				isLegal = true;
			}
			if(GetSurroundingNode(Vector2.Zero, 2) != null && grabPlant == null) {
				grabPlant = (PlantBase)GetSurroundingNode(Vector2.Zero, 2);
				isLegal = true;
				GD.Print(grabPlant);
				plants.ForEach(p => GD.Print(p));
			}
			GD.Print("do grab");
		}else if(Input.IsActionJustPressed("grab_left")) {
            plantDirection = Vector2.Left;
			isLegal = true;
        }else if(Input.IsActionJustPressed("grab_right")) {
            plantDirection = Vector2.Right;
			isLegal = true;
        }else if(Input.IsActionJustPressed("grab_up")) {
            plantDirection = Vector2.Up;
			isLegal = true;
        }else if(Input.IsActionJustPressed("grab_down")) {
            plantDirection = Vector2.Down;
			isLegal = true;
        }else {
            return false;
        }
		if(isLegal && direction != Vector2.Zero) {
			if(grabPlant != null) {
				GD.Print("move plants");
				plants.ForEach(p => {
					GD.Print("move", p);
					p.GlobalPosition += direction * _width;
				});
			}
			Position += direction * _width;
		}
		if(isLegal && plantDirection != Vector2.Zero) {
			if(grabPlant != null) {
				GD.Print("move plants");
				plants.ForEach(p => {
					GD.Print("move", p);
					p.GlobalPosition += plantDirection * _width;
				});
			}
		}
		return isLegal;
	}
	public Area2D GetSurroundingNode(Vector2 direction, uint layer) {
            var result = this.GetWorld2d().DirectSpaceState.IntersectRay(GlobalPosition + direction, GlobalPosition + direction, null, layer, false ,true);
            if(result.Count == 0)
                return null;
            else
                return result["collider"] as Area2D;
    }
}
