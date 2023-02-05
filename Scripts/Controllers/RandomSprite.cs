using System.Collections.Generic;
using Godot;

public class RandomSprite : AnimatedSprite, ISave {
    public void Load(Dictionary<string, object> data) {
        this.Frame = (int)data["Frame"];
    }

    public Dictionary<string, object> Save() {
        return new Dictionary<string, object>() {
            { "Filename", this.Filename },
            { "Parent", this.GetParent().GetPath() },
            { "Frame", this.Frame }
        };
    }

    public override void _Ready() {
        this.Frame = (int)GD.RandRange(0, this.Frames.GetFrameCount(this.Animation) - 1e-6);
    }
}