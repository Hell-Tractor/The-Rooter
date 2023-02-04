using System.Linq;
using System.Collections.Generic;
using GC = Godot.Collections;

public class Root : PlantBase {
    public List<Leaf> GetConnectedLeaves() {
        return this.GetAllConnectedParts().Where(part => part.GetStemType() == StemType.Leaf).Cast<Leaf>().ToList();
    }

    public override StemType GetStemType() {
        return StemType.Root;
    }

    public override GC.Dictionary<string, object> Save() {
        GC.Dictionary<string, object> save = base.Save();
        save.Add("Filename", this.Filename);
        save.Add("Parent", this.GetParent().GetPath());
        return save;
    }
}