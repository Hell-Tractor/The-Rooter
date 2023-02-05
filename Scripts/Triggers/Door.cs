using System.Linq;
using Godot;
using Godot.Collections;
using AI.FSM;
public class Door : Area2D, ITrigger {
    [Export]
    public int RequireCount;
    private int _currentCount = 0;

    public override void _Process(float delta) {
        base._Process(delta);

        if (this._currentCount == RequireCount) {
            if (this.GetOverlappingAreas().OfType<PlayerFSM>().Any()) {
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

    public Dictionary<string, object> Save() {
        return new Dictionary<string, object>() {
            { "Filename", this.Filename },
            { "Parent", this.GetParent().GetPath() },
            { "_currentCount", this._currentCount }
        };
    }
}