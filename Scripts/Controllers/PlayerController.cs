using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class PlayerController : Area2D, ISave {
	private int _width;
	private RoundManager roundManager;
	public PlantBase crabPlant;
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
			{ "crabPlant", this.crabPlant }
		};
	}

	public void Load(Dictionary<string, object> data) {
		this.Position = (Vector2)data["Position"];
		this.bodyType = (BodyStateType)data["bodyType"];
		this.handType = (HandStateType)data["handType"];
		this.crabPlant = (PlantBase)data["crabPlant"];
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
		if (moveTimer.IsStopped()) {
			// * -------请将一回合内的检测全部放在这里-------
			if(this._processMove()) {
				this.roundManager.OnRoundFinish();
			}
			moveTimer.Start();
			// * ---------------------------------------
			
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
		List<PlantBase> plants = new List<PlantBase>();
		Func<Vector2, uint, Area2D, int> getObstacle = (direction, layer, objectArea) => { //layer 1: earth layer 2: plant
			return objectArea.GetWorld2d().DirectSpaceState.IntersectRay(Position + direction * _width, Position + direction * _width, null, layer, false ,true).Count;
		};
		Func<Vector2, uint, Area2D, PlantBase> getPlantSet = (direction, layer, objectArea) => { //layer 1: earth layer 2: plant
			return objectArea.GetWorld2d().DirectSpaceState.IntersectRay(Position + direction * _width, Position + direction * _width, null, layer, false ,true)[0] as PlantBase;
		};
		Func<Vector2, uint, List<PlantBase>, int> getCrabObstacle = (direction, layer, mplants) => { //layer 1: earth layer 2: plant
			int obstacleCount = 0;
			mplants.ForEach(p => {
				if(p.GetWorld2d().DirectSpaceState.IntersectRay(p.Position + direction * _width, p.Position + direction * _width, null, layer, false ,true).Count >= 0){
					obstacleCount += 1;
					return;
				}
			});
			return obstacleCount;
		};
		Action<List<PlantBase>, Vector2> movePlant = (mplants, direction) => {
			mplants.ForEach(p => p.Position += direction * _width);
		};

		if(this.GetOverlappingAreas().Count >= 1){
			crabPlant = this.GetOverlappingAreas()[0] as PlantBase;
			plants = crabPlant.GetAllConnectedParts();
		}
			

		if (Input.IsActionPressed("move_right") && getObstacle(Vector2.Right, 1, this) == 0) {
			if(handType == HandStateType.unCrab){
				if(getObstacle(Vector2.Right, 2, this) > 0)
					bodyType = BodyStateType.climb;
				horizontalVelocity += _width;
				isLegal = true;
			}
			else if(getCrabObstacle(Vector2.Right, 1, plants) == 0 && getCrabObstacle(Vector2.Right, 2, plants) == 0){
				horizontalVelocity += _width;
				isLegal = true;
				movePlant(plants, Vector2.Right);
			}
			if(isLegal == true){
				bodyType = BodyStateType.walk;
			}
		}
		if (Input.IsActionPressed("move_right") && (getObstacle(Vector2.Right, 1, this) > 0 || getObstacle(Vector2.Right, 2, this) > 0) &&
			getObstacle(Vector2.Up, 1, this) == 0 && getObstacle(Vector2.Up, 2, this) == 0 && getObstacle(Vector2.Up + Vector2.Right, 1, this) == 0 && 
			getObstacle(Vector2.Up + Vector2.Right, 2, this) == 0) {
			horizontalVelocity += _width;
			verticalVelocity += _width;
		}
		if (Input.IsActionPressed("move_left") && (getObstacle(Vector2.Left, 1, this) > 0 || getObstacle(Vector2.Left, 2, this) > 0) &&
			getObstacle(Vector2.Up, 1, this) == 0 && getObstacle(Vector2.Up, 2, this) == 0 && getObstacle(Vector2.Up + Vector2.Left, 1, this) == 0 && 
			getObstacle(Vector2.Up + Vector2.Left, 2, this) == 0) {
			horizontalVelocity -= _width;
			verticalVelocity += _width;
		}
		if (Input.IsActionPressed("move_left") && getObstacle(Vector2.Left, 1, this) == 0) {
			if(handType == HandStateType.unCrab){
				if(getObstacle(Vector2.Left, 2, this) > 0)
					bodyType = BodyStateType.climb;
				horizontalVelocity -= _width;
				isLegal = true;
			}
			else if(getCrabObstacle(Vector2.Left, 1, plants) == 0 && getCrabObstacle(Vector2.Left, 2, plants) == 0){
				horizontalVelocity -= _width;
				isLegal = true;
				movePlant(plants, Vector2.Left);
			}
			if(isLegal == true){
				bodyType = BodyStateType.walk;
			}
		}
		if(Input.IsActionPressed("move_up") && getObstacle(Vector2.Up, 1, this) == 0){
			if(handType == HandStateType.unCrab && getObstacle(Vector2.Up, 1, this) == 0){
				verticalVelocity += _width;
				isLegal = true;
			}
			if(isLegal == true){
				if(bodyType == BodyStateType.climb && getObstacle(Vector2.Up, 2, this) == 0)
					bodyType = BodyStateType.hold;
				else if(bodyType == BodyStateType.hold)
					bodyType = BodyStateType.walk;
			}
		}

		if(Input.IsActionPressed("move_down") && getObstacle(Vector2.Down * 2, 1, this) == 0) {
			if(handType == HandStateType.unCrab && getObstacle(Vector2.Down * 2, 1, this) == 0){
				verticalVelocity -= _width;
				isLegal = true;
			}
			if(isLegal == true){
				if(bodyType == BodyStateType.climb && getObstacle(Vector2.Down, 2, this) == 0)
					bodyType = BodyStateType.walk;
				else if(bodyType == BodyStateType.hold)
					bodyType = BodyStateType.climb;
			}
		}

		if(Input.IsActionPressed("crab") && getObstacle(Vector2.Zero, 1, this) > 0) {
			if(handType == HandStateType.unCrab){
				if(crabPlant.ConnectedDirection == 1 || crabPlant.ConnectedDirection == 2 || crabPlant.ConnectedDirection == 3)
					handType = HandStateType.horiCrab;
				else
					handType = HandStateType.vertCrab;
				isLegal = true;
			}else {
				bool bumpFlag = false;
				while(!bumpFlag) {
					if(getObstacle(Vector2.Down, 1, plants.OfType<Root>().ToList()[0]) > 0){
						plants.ForEach(p => p.Position += Vector2.Down);
						break;
					}
					if(getCrabObstacle(Vector2.Down, 1, plants) == 0 && getCrabObstacle(Vector2.Down, 2, plants) == 0)
						plants.ForEach(p => p.Position += Vector2.Down);
					else
						plants.ForEach(p => p.QueueFree());
				}
				isLegal = true;
			}
		}

		if(Input.IsActionPressed("crab_left") && (crabPlant.ConnectedDirection == 1 || crabPlant.ConnectedDirection == 2 || crabPlant.ConnectedDirection == 3)) {
			if(getCrabObstacle(Vector2.Left, 1, plants) == 0 && getCrabObstacle(Vector2.Left, 2, plants) == 0 && crabPlant._next.Any(p => p == getPlantSet(Vector2.Right, 2, crabPlant as Area2D))) {
				plants.ForEach(p => p.Position += Vector2.Left);
				isLegal = true;
			}
				
		}

		if(Input.IsActionPressed("crab_right") && (crabPlant.ConnectedDirection == 1 || crabPlant.ConnectedDirection == 2 || crabPlant.ConnectedDirection == 3)) {
			if(getCrabObstacle(Vector2.Right, 1, plants) == 0 && getCrabObstacle(Vector2.Right, 2, plants) == 0 && crabPlant._next.Any(p => p == getPlantSet(Vector2.Left, 2, crabPlant as Area2D))) {
				plants.ForEach(p => p.Position += Vector2.Right);
				isLegal = true;
			}	
		}

		if(Input.IsActionPressed("crab_up") && !(crabPlant.ConnectedDirection == 1 || crabPlant.ConnectedDirection == 2 || crabPlant.ConnectedDirection == 3)) {
			if(getCrabObstacle(Vector2.Up, 1, plants) == 0 && getCrabObstacle(Vector2.Up, 2, plants) == 0 && crabPlant._next.Any(p => p == getPlantSet(Vector2.Down, 2, crabPlant as Area2D))) {
				plants.ForEach(p => p.Position += Vector2.Up);
				isLegal = true;
			}	
		}

		if(Input.IsActionPressed("crab_down") && !(crabPlant.ConnectedDirection == 1 || crabPlant.ConnectedDirection == 2 || crabPlant.ConnectedDirection == 3)) {
			if(getCrabObstacle(Vector2.Down, 1, plants) == 0 && getCrabObstacle(Vector2.Down, 2, plants.Where(p => p.GetStemType() != StemType.Root).ToList()) == 0 && crabPlant._next.Any(p => p == getPlantSet(Vector2.Up, 2, crabPlant as Area2D))) {
				plants.ForEach(p => p.Position += Vector2.Up);
				isLegal = true;
			}	
		}
		
		if(getObstacle(Vector2.Up, 2, this) == 0 && bodyType == BodyStateType.climb)
			bodyType = BodyStateType.hold;
		if(bodyType != BodyStateType.climb && getObstacle(Vector2.Down, 1, this) == 0 && getObstacle(Vector2.Down, 2, this) == 0) {
			bodyType = BodyStateType.fall;
			int highCount = 0;
			while(getObstacle(Vector2.Down * highCount, 1, this) == 0 && getObstacle(Vector2.Down * highCount, 2, this) == 0) {
				highCount ++;
			}
			if(highCount > 3)
				bodyType = BodyStateType.dead;
			verticalVelocity -= highCount * _width;
		}
			

		
		Position += new Vector2(horizontalVelocity, verticalVelocity);
		if(bodyType != BodyStateType.walk && handType != HandStateType.unCrab)
			Console.WriteLine("wrong statetype");
		return isLegal;
	}
}
