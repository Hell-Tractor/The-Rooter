using Godot;
using System;

public class PlayerController : Area2D {
	private int _width;
	private RoundManager roundManager;
	[Export]
	public NodePath RoundManagerPath;

	public override void _Ready() {
		Sprite sprite = this.GetNode<Sprite>("Sprite");
		this._width = (int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x);

		roundManager = this.GetNode<RoundManager>(RoundManagerPath);
	}

	public override void _Process(float delta) {
		base._Process(delta);

		Timer moveTimer = this.GetNode<Timer>("MoveTimer");
		if (moveTimer.IsStopped()) {
			// * -------请将一回合内的检测全部放在这里-------
			this._processMove();

			// * ---------------------------------------
			this.roundManager.OnRoundFinish();
			moveTimer.Start();
		}
	}

	private void _processMove() {
		int velocity = 0;
		if (Input.IsActionPressed("move_right")) {
			velocity += _width;
		}
		if (Input.IsActionPressed("move_left")) {
			velocity -= _width;
		}
		Position += new Vector2(velocity, 0);
	}
}
