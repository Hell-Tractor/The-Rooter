using System.Linq;
using System;
using System.Collections.Generic;
using Godot;

public abstract class PlantBase : Area2D, ISave {
    public static int Count { get; set; } = 0;
    [Export]
    public List<PlantBase> _next;
    public int ConnectedDirection;
    public int Width { get; private set; }
    public int Id { get; private set; }

    public override void _Ready() {
        Sprite sprite = this.GetNode<Sprite>("Sprite");
        this.Width = Math.Abs((int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x));

        this.Id = Count++;
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
        for (q.Enqueue(this); q.Count > 0;) {
            PlantBase u = q.Dequeue();
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
        var result = this.GetWorld2d().DirectSpaceState.IntersectRay(GlobalPosition + direction, GlobalPosition + direction, null, layer, false ,true);
            if(result.Count == 0)
                return null;
            else
                return result["collider"] as Area2D;
    } 
    public virtual void Load(Dictionary<string, object> data) {
        this.ConnectedDirection = (int)data["ConnectedDirection"];
        this._next = (List<PlantBase>)data["_next"];
    }
}
