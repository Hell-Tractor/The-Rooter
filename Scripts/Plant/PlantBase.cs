using System.Collections.Generic;
using Godot;

public class PlantBase : Area2D {
    public static int Count { get; private set; } = 0;
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
}
