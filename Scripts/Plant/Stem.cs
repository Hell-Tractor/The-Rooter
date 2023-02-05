using Godot;
using System.Collections.Generic;

public class Stem : PlantBase, IRoundable {
    public override StemType GetStemType() {
        return StemType.Stem;
    }

    public override Dictionary<string, object> Save() {
        Dictionary<string, object> save = base.Save();
        save.Add("Filename", this.Filename);
        save.Add("Parent", this.GetParent().GetPath());
        return save;
    }
}