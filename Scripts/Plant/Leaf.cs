using System.Diagnostics;
using System.Linq;
using Godot;
using System.Collections.Generic;

public class Leaf : PlantBase, IRoundable, ISave {
    [Export(PropertyHint.Range, "0, 10")]
    public int Delay;
    [Export(PropertyHint.File, "*.tscn")]
    public string StemPath;
    private GrowthCommand _leftCommand;
    private GrowthCommand _rightCommand;
    private GrowthCommand _upCommand;
    private PlantBase self = null;
    private bool _isUpdated = false;

    public override void _Ready() {
        base._Ready();

        _leftCommand = new GrowthCommand(Vector2.Left, Delay);
        _rightCommand = new GrowthCommand(Vector2.Right, Delay);
        _upCommand = new GrowthCommand(Vector2.Up, Delay);
    }

    public void ApplyCommand(int directions) {
        GD.Print("Apply command " + directions);
        _leftCommand.Command = ((directions & BitDirection.Left) != 0);
        _rightCommand.Command = ((directions & BitDirection.Right) != 0);
        _upCommand.Command = ((directions & BitDirection.Up) != 0);
    }

    public override StemType GetStemType() {
        return StemType.Leaf;
    }

    private PlantBase _createLeafAt(Vector2 direction) {
        Vector2 position = this.GlobalPosition + direction * Width;
        // GD.Print("Width: " + Width);
        // GD.Print("Create leaf at " + direction + " " + position);

        // check if there is already a stem at that position
        System.Object overlap = this._checkOverlap(position);
        // GD.Print("Overlap: " + (overlap as Area2D)?.Name ?? "null");
        if (overlap != null)
            return null;

        Leaf leaf = this._getPlantAt(position) as Leaf;
        if (leaf != null && leaf.IsNew) {
            Stem stem = GD.Load<PackedScene>(this.StemPath).Instance<Stem>();
            stem.Position = leaf.Position;
            stem.IsNew = true;
            this.GetParent().AddChild(stem);

            leaf.QueueFree();
            leaf._next[0]._next.Remove(leaf);
            RoundManager.Instance.RemoveRoundable(leaf);
            return stem;
        }

        // create new leaf
        Leaf newLeaf = (Leaf)this.Duplicate(7);
        this.GetParent().AddChild(newLeaf);
        newLeaf.GlobalPosition = position;
        newLeaf._leftCommand = this._leftCommand;
        newLeaf._rightCommand = this._rightCommand;
        newLeaf._upCommand = this._upCommand;
        // GD.Print("New leaf position: " + newLeaf.GlobalPosition);
        newLeaf._next = new List<PlantBase>();
        newLeaf.ConnectedDirection = 0;
        newLeaf.IsNew = true;
        return newLeaf;
    }

    private PlantBase _replaceSelfByStem() {
        Stem stem = GD.Load<PackedScene>(this.StemPath).Instance<Stem>();
        stem.Position = this.Position;
        stem.IsNew = true;
        stem._next = this._next;
        this._next[0]._next.Add(stem);
        stem.ConnectedDirection = this.ConnectedDirection;
        // GD.Print("Init stem connection: " + stem.ConnectedDirection);
        this.GetParent().AddChild(stem);
        this._next[0]._next.Remove(this);
        RoundManager.Instance.RemoveRoundable(this);
        return stem;
    }

    private void _findNearbyStem(PlantBase stem) {
        this._findStemOnDir(Vector2.Left, stem);
        this._findStemOnDir(Vector2.Right, stem);
        this._findStemOnDir(Vector2.Up, stem);
        this._findStemOnDir(Vector2.Down, stem);
        // GD.Print("Connected direction: " + stem.ConnectedDirection);
    }

    private void _findStemOnDir(Vector2 direction, PlantBase stem) {
        Vector2 position = stem.GlobalPosition + direction * Width;

        System.Object overlap = this._checkOverlap(position);
        if (overlap == null) {
            // GD.Print("No overlap at " + direction + position);
            return;
        }

        PlantBase other = overlap as PlantBase;
        // GD.Print("Found plantbase: " + other + " is new: " + other?.IsNew);
        if (other == null || !other.IsNew)
            return;
        // GD.Print("Connected to " + other.GetStemType() + " at " + direction + position);

        stem.ConnectedDirection |= BitDirection.FromVector2(direction);
        other.ConnectedDirection |= BitDirection.FromVector2(-direction);

        if (!stem._next.Contains(other))
            stem._next.Add(other);
        if (!other._next.Contains(stem))
            other._next.Add(stem);
    }

    private static void _connect(PlantBase from, PlantBase to, Vector2 direction) {
        from._next.Add(to);
        to._next.Add(from);
        from.ConnectedDirection |= BitDirection.FromVector2(direction);
        to.ConnectedDirection |= BitDirection.FromVector2(-direction);
    }

    public override void OnRoundFinish() {
        bool left = _leftCommand.Command;
        bool right = _rightCommand.Command;
        bool up = _upCommand.Command;
        // GD.Print("Leaf command: " + left + " " + right + " " + up);

        PlantBase l = null, r = null, u = null;

        if (left)
            left &= (l = _createLeafAt(Vector2.Left)) != null;
        if (right)
            right &= (r = _createLeafAt(Vector2.Right)) != null;
        if (up)
            up &= (u = _createLeafAt(Vector2.Up)) != null;
        this._isUpdated = left || right || up;

        if (this._isUpdated)
            this.self = this._replaceSelfByStem();

        if (l != null)
            _connect(this.self, l, Vector2.Left);
        if (r != null)
            _connect(this.self, r, Vector2.Right);
        if (u != null)
            _connect(this.self, u, Vector2.Up);
    }

    public override void OnRoundLateFinish() {
        if (!this._isUpdated)
            return;
        _findNearbyStem(this.self);
        this.QueueFree();
    }

    private System.Object _checkOverlap(Vector2 position) {
        // use raycast to check if there is a stem at the position
        var result = this.GetWorld2d().DirectSpaceState.IntersectPoint(position, 1, null, this.CollisionLayer, false, true);
        if (result.Count == 0)
            return null;
        return ((Godot.Collections.Dictionary)result[0])["collider"];
    }

    private PlantBase _getPlantAt(Vector2 position) {
        foreach (PlantBase plant in GetTree().GetNodesInGroup("Plant")) {
            if (plant.GlobalPosition.DistanceTo(position) < Width / 2)
                return plant;
        }
        return null;
    }

    public override Dictionary<string, object> Save() {
        Dictionary<string, object> save = base.Save();
        save.Add("Filename", this.Filename);
        save.Add("Parent", this.GetParent().GetPath());
        save.Add("_leftCommand", _leftCommand);
        save.Add("_rightCommand", _rightCommand);
        save.Add("_upCommand", _upCommand);
        return save;
    }

    public override void Load(Dictionary<string, object> data) {
        base.Load(data);

        this._leftCommand = (GrowthCommand)data["_leftCommand"];
        this._rightCommand = (GrowthCommand)data["_rightCommand"];
        this._upCommand = (GrowthCommand)data["_upCommand"];
    }
}