using Godot;
using System.Collections.Generic;

public class FramedAnimatedSprite : AnimatedSprite, ISave {
    private int _frameCount;

    public override void _Ready() {
        this.Frame = 0;
        this._frameCount = this.Frames.GetFrameCount(this.Animation);
    }

    public void NextFrame() {
        this.Frame = (this.Frame + 1) % this._frameCount;
    }

    public Dictionary<string, object> Save() {
        return new Dictionary<string, object>() {
            { "Filename", this.Filename },
            { "Parent", this.GetParent().GetPath() },
            { "Frame", this.Frame }
        };
    }
}