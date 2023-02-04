using Godot;

public class Leaf : PlantBase, IRoundable {
    [Export(PropertyHint.Range, "0, 10")]
    public int Delay;
    [Export]
    public NodePath RoundManagerPath;
    [Export]
    public int RoundPriority;
    private RoundManager _roundManager;
    private GrowthCommand _leftCommand;
    private GrowthCommand _rightCommand;
    private GrowthCommand _upCommand;
    private PlantBase self = null;

    public override void _Ready() {
        base._Ready();

        _roundManager = this.GetNode<RoundManager>(RoundManagerPath);
        _roundManager.AddRoundable(this);

        _leftCommand = new GrowthCommand(Vector2.Left, Delay);
        _rightCommand = new GrowthCommand(Vector2.Right, Delay);
        _upCommand = new GrowthCommand(Vector2.Up, Delay);
    }

    public void ApplyCommand(int directions) {
        if ((directions & BitDirection.Left) != 0)
            _leftCommand.Command = true;
        if ((directions & BitDirection.Right) != 0)
            _rightCommand.Command = true;
        if ((directions & BitDirection.Up) != 0)
            _upCommand.Command = true;
    }

    public override StemType GetStemType() {
        return StemType.Leaf;
    }

    private Leaf _createLeafAt(Vector2 direction, PlantBase from) {
        Vector2 position = this.Position + direction * Width;

        // check if there is already a stem at that position
        System.Object overlap = this._checkOverlap(position);
        if (overlap != null)
            return null;

        // create new leaf
        Leaf newLeaf = (Leaf)this.Duplicate();
        newLeaf.Position = position;
        newLeaf.RoundPriority = this.RoundPriority;
        newLeaf.ConnectedDirection = BitDirection.FromVector2(-direction);
        this.GetParent().AddChild(newLeaf);
        return newLeaf;
    }

    private PlantBase _replaceSelfByStem() {
        PlantBase stem = new PlantBase();
        stem.Position = this.Position;
        this.GetParent().AddChild(stem);
        return stem;
    }

    private void _findNearbyStem(PlantBase stem) {
        this._findStemOnDir(Vector2.Left, stem);
        this._findStemOnDir(Vector2.Right, stem);
        this._findStemOnDir(Vector2.Up, stem);
        this._findStemOnDir(Vector2.Down, stem);
    }

    private void _findStemOnDir(Vector2 direction, PlantBase stem) {
        Vector2 position = this.Position + direction * Width;

        System.Object overlap = this._checkOverlap(position);
        if (overlap == null)
            return;

        Stem other = overlap as Stem;
        if (other == null || !other.IsNew)
            return;

        stem.ConnectedDirection |= BitDirection.FromVector2(direction);
        other.ConnectedDirection |= BitDirection.FromVector2(-direction);

        if (!stem._next.Contains(other))
            stem._next.Add(other);
        if (!other._next.Contains(stem))
            other._next.Add(stem);
    }

    public void OnRoundFinish() {
        bool left = _leftCommand.Command;
        bool right = _rightCommand.Command;
        bool up = _upCommand.Command;

        if (left)
            _createLeafAt(Vector2.Left, this);
        if (right)
            _createLeafAt(Vector2.Right, this);
        if (up)
            _createLeafAt(Vector2.Up, this);

        this.self = this._replaceSelfByStem();
    }

    public void OnRoundLateFinish() {
        _findNearbyStem(this.self);
        this.QueueFree();
    }

    public int GetRoundPriority() {
        return this.RoundPriority;
    }

    private System.Object _checkOverlap(Vector2 position) {
        // use raycast to check if there is a stem at the position
        var result = this.GetWorld2d().DirectSpaceState.IntersectRay(position, position, null, this.CollisionLayer, false, true);
        if (result.Count == 0)
            return null;
        return result["collider"];
    }

    public void OnRoundStart() {}
}