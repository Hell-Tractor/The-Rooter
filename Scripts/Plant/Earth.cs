using System.Linq;
using Godot;
using System.Collections.Generic;

public class Earth : Area2D, IRoundable, ISave {
    [Export(PropertyHint.Flags, "Left,Right,Up")]
    public int GrowthCommand;
    [Export]
    public int RoundPriority;
    [Export]
    public bool IsInfinity = false;
    [Export(PropertyHint.Range, "0, 100")]
    public int Fertility;

    public override void _Ready() {
        // Add this stem to the round manager
        RoundManager.Instance.AddRoundable(this);
    }

    public int GetRoundPriority() {
        return this.RoundPriority;
    }

    public void OnRoundFinish() {
        if (!this.IsInfinity && this.Fertility == 0)
            return;
        // get the stem that is overlapping this earth
        Root root = this.GetOverlappingAreas().OfType<Root>().FirstOrDefault(null);
        if (root != null) {
            // if there is a stem, pass the growth command to it
            root.GetConnectedLeaves().ForEach(leaf => leaf.ApplyCommand(this.GrowthCommand));
            if (!this.IsInfinity) {
                // if this earth is not infinite, decrease fertility
                this.Fertility--;
            }
        }
    }

    public void OnRoundLateFinish() {}
    public void OnRoundStart() {}

    public Dictionary<string, object> Save() {
        return new Dictionary<string, object>() {
            { "Filename", this.Filename },
            { "Parent", this.GetParent().GetPath() },
            { "Fertility", this.Fertility }
        };
    }

    public void Load(Dictionary<string, object> data) {
        this.Fertility = (int)data["Fertility"];
    }
}