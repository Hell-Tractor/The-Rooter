using Godot;
using System;

public class PlayerController : Area2D {
	private int _width;

	public override void _Ready() {
		Sprite sprite = this.GetNode<Sprite>("Sprite");
		this._width = (int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x);
	}

	public override void _Process(float delta) {
		base._Process(delta);

		Timer moveTimer = this.GetNode<Timer>("MoveTimer");
		if (moveTimer.IsStopped()) {
			this._processMove();
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
