using System.Linq;
using Godot;

public class Door : Area2D, ITrigger {
    [Export]
    public bool IsOpen = false;

    public override void _Process(float delta) {
        base._Process(delta);

        if (this.IsOpen) {
            if (this.GetOverlappingAreas().OfType<PlayerController>().Any()) {
                // * stage clear
            }
        }
    }

    public void Trigger() {
        this.IsOpen = true;
    }

    public void UnTrigger() {
        this.IsOpen = false;
    }
}