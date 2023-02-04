using System.Collections.Generic;
using Godot;

public abstract class PlantBase : Area2D, ISave {
    public static int Count { get; set; } = 0;
    public List<PlantBase> _next;
    public int ConnectedDirection;
    public int Width { get; private set; }
    public int Id { get; private set; }

    public override void _Ready() {
        Sprite sprite = this.GetNode<Sprite>("Sprite");
        this.Width = (int)(sprite.Texture.GetWidth() * sprite.Scale.x * this.Scale.x);

        this.Id = Count++;
    }

    public virtual StemType GetStemType() {
        return StemType.Stem;
    }

    public virtual Godot.Collections.Dictionary<string, object> Save() {
        return new Godot.Collections.Dictionary<string, object>() {
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
}