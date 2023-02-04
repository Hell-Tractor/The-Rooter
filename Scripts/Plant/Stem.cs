using Godot;
using Godot.Collections;

public class Stem : PlantBase, IRoundable {
    [Export]
    public int RoundPriority;
    public bool IsNew = false;

    public int GetRoundPriority() {
        return this.RoundPriority;
    }

    public override StemType GetStemType() {
        return StemType.Stem;
    }

    public void OnRoundFinish() {}

    public void OnRoundLateFinish() {}

    public void OnRoundStart() {
        this.IsNew = false;
    }

    public override Dictionary<string, object> Save() {
        Dictionary<string, object> save = base.Save();
        save.Add("Filename", this.Filename);
        save.Add("Parent", this.GetParent().GetPath());
        return save;
    }
}