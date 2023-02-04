using Godot;

public class FramedAnimatedSprite : AnimatedSprite {
    private int _frameCount;

    public override void _Ready() {
        this.Frame = 0;
        this._frameCount = this.Frames.GetFrameCount(this.Animation);
    }

    public void NextFrame() {
        this.Frame = (this.Frame + 1) % this._frameCount;
    }
}