using System.Linq;
using Godot;

public class Earth : Area2D, IRoundable {
    [Export(PropertyHint.Flags, "Left,Right,Up")]
    public int GrowthCommand;
    [Export]
    public int RoundPriority;
    [Export]
    public bool IsInfinity = false;
    [Export(PropertyHint.Range, "0, 100")]
    public int Fertility;
    [Export]
	public NodePath RoundManagerPath;

    public override void _Ready() {
        // Add this stem to the round manager
        RoundManager manager = this.GetNode<RoundManager>(RoundManagerPath);
        manager.AddRoundable(this);
    }

    public int GetRoundPriority() {
        return this.RoundPriority;
    }

    public void OnRoundFinish() {
        if (!this.IsInfinity && this.Fertility == 0)
            return;
        // get the stem that is overlapping this earth
        Stem root = this.GetOverlappingAreas().OfType<Stem>().Where(stem => stem.isRoot).FirstOrDefault(null);
        if (root != null) {
            // if there is a stem, pass the growth command to it
            root.Command |= this.GrowthCommand;
            if (!this.IsInfinity) {
                // if this earth is not infinite, decrease fertility
                this.Fertility--;
            }
        }
    }
}