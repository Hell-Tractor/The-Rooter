using System.Linq;
using System;
using System.Collections.Generic;
using Godot;

public abstract class PlantBase : Area2D, ISave, IRoundable {
    public static int Count { get; set; } = 0;
    [Export]
    public List<NodePath> PreDefinedNext = new List<NodePath>();
    public List<PlantBase> _next = new List<PlantBase>();
    [Export(PropertyHint.Flags, "Left,Right,Up,Down")]
    public int ConnectedDirection;
    public int Width { get; private set; }
    public int Id { get; private set; }
    [Export]
    public int RoundPriority;
    public bool IsNew = false;

    public override void _Ready() {
        Sprite sprite = this.GetNode<Sprite>("Sprite");
        this.Width = Math.Abs((int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x));

        this.Id = Count++;
        RoundManager.Instance.AddRoundable(this);

        foreach (NodePath path in this.PreDefinedNext) {
            PlantBase next = this.GetNode<PlantBase>(path);
            this._next.Add(next);
        }
    }

    public abstract StemType GetStemType();

    public virtual Dictionary<string, object> Save() {
        return new Dictionary<string, object>() {
            { "ConnectedDirection", this.ConnectedDirection },
            { "_next", this._next }
        };
    }

    public List<PlantBase> GetAllConnectedParts() {
        List<PlantBase> result = new List<PlantBase>();
        List<bool> visited = new List<bool>(Count);
        for (int i = 0; i < Count; i++) {
            visited.Add(false);
        }
        Queue<PlantBase> q = new Queue<PlantBase>();
        visited[this.Id] = true;
        result.Add(this);
        for (q.Enqueue(this); q.Count > 0;) {
            PlantBase u = q.Dequeue();
            GD.Print(u.Name);
            foreach (PlantBase v in u._next) {
                if (!visited[v.Id]) {
                    visited[v.Id] = true;
                    result.Add(v);
                    q.Enqueue(v);
                }
            }
        }
        return result;
    }

    public List<T> GetAllConnectedParts<T>() {
        return this.GetAllConnectedParts().Where(part => part is T).Cast<T>().ToList();
    }
    public Area2D GetSurroundingNode(Vector2 direction, uint layer) {
        Vector2 endPoint = new Vector2(GlobalPosition.x + direction.x + 1, GlobalPosition.y + direction.y + 1);
        var result = this.GetWorld2d().DirectSpaceState.IntersectRay(GlobalPosition + direction, endPoint, null, layer, false ,true);
            if(result.Count == 0)
                return null;
            else
                return result["collider"] as Area2D;
    }
    public virtual void Load(Dictionary<string, object> data) {
        this.ConnectedDirection = (int)data["ConnectedDirection"];
        this._next = (List<PlantBase>)data["_next"];
    }

    public virtual void OnRoundStart() {
        this.IsNew = false;
    }
    public virtual void OnRoundFinish() {}
    public virtual void OnRoundLateFinish() {}
    public int GetRoundPriority() {
        return this.RoundPriority;
    }
}
