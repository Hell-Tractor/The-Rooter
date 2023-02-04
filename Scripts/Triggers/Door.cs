using System.Linq;
using Godot;

public class Door : Area2D, ITrigger {
    [Export]
    public int RequireCount;
    private int _currentCount = 0;

    public override void _Process(float delta) {
        base._Process(delta);

        if (this._currentCount == RequireCount) {
            if (this.GetOverlappingAreas().OfType<PlayerController>().Any()) {
                // * stage clear
            }
        }
    }

    public void Trigger() {
        this._currentCount++;
    }

    public void UnTrigger() {
        this._currentCount--;
    }
}