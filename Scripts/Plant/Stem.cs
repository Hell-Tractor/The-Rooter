using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Stem : Area2D, IRoundable {
    private List<Stem> _next = new List<Stem>();
    public int Command = 0;
    public int ConnectedDirections = 0;
    private int _width;
    [Export]
    public int RoundPriority;
    [Export]
    public bool isRoot;
    public bool isLeaf { get { return this._next.Any(); }}
    [Export]
	public NodePath RoundManagerPath;

    public override void _Ready() {
        Sprite sprite = this.GetNode<Sprite>("Sprite");
        this._width = (int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x);

        // Add this stem to the round manager
        RoundManager manager = this.GetNode<RoundManager>(RoundManagerPath);
        manager.AddRoundable(this);
    }

    public void OnRoundFinish() {
        if (_next.Any()) {
            // If there are any next stems, pass the command to them
            _next.ForEach(next => next.Command |= Command);
        } else {
            for (int i = 1; i <= 4; i <<= 1) {
                if ((this.Command & i) != 0) {
                    // calculate the position of the new stem
                    Vector2 position = this._getNextStemPosition(i);

                    // check if there is already a stem at that position
                    System.Object overlap = this._checkOverlap(position);
                    if (overlap != null) {
                        Stem oldStem = overlap as Stem;
                        if (oldStem != null)
                            oldStem.ConnectedDirections |= (int)this._getOppositeDirection(i);
                        continue;
                    }

                    // If the command is set, grow in that direction
                    Stem next = new Stem();
                    next.Position = position;
                    next.RoundPriority = this.RoundPriority - 1;
                    next.ConnectedDirections = (int)this._getOppositeDirection(i);
                    this.GetParent().AddChild(next);
                    _next.Add(next);
                }
            }
        }
        Command = 0;
    }

    public int GetRoundPriority() {
        return this.RoundPriority;
    }

    private Vector2 _getNextStemPosition(int direction) {
        Vector2 position = this.Position;
        switch (direction) {
            case (int)Directions.Left:
                position -= new Vector2(_width, 0);
                break;
            case (int)Directions.Right:
                position += new Vector2(_width, 0);
                break;
            case (int)Directions.Up:
                position -= new Vector2(0, _width);
                break;
        }
        return position;
    }

    private Directions _getOppositeDirection(int direction) {
        switch (direction) {
            case (int)Directions.Left:
                return Directions.Right;
            case (int)Directions.Right:
                return Directions.Left;
            case (int)Directions.Up:
                return Directions.Down;
            case (int)Directions.Down:
                return Directions.Up;
        }
        throw new ArgumentException("Invalid direction");
    }

    private System.Object _checkOverlap(Vector2 position) {
        // use raycast to check if there is a stem at the position
        var result = this.GetWorld2d().DirectSpaceState.IntersectRay(position, position, null, this.CollisionLayer, false, true);
        if (result.Count == 0)
            return null;
        return result["collider"];
    }
}
