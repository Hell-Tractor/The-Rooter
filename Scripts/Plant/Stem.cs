using Godot;

public class Stem : PlantBase, IRoundable {
    [Export]
    public int RoundPriority;
    public bool IsNew = true;

    public int GetRoundPriority() {
        return this.RoundPriority;
    }

    public void OnRoundFinish() {}

    public void OnRoundLateFinish() {}

    public void OnRoundStart() {
        this.IsNew = false;
    }
}