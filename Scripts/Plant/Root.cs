using System.Linq;
using System.Collections.Generic;

public class Root : PlantBase {
    public List<Leaf> GetConnectedLeaves() {
        return this.GetAllConnectedParts().Where(part => part.GetStemType() == StemType.Leaf).Cast<Leaf>().ToList();
    }

    public override StemType GetStemType() {
        return StemType.Root;
    }

    public override Dictionary<string, object> Save() {
        Dictionary<string, object> save = base.Save();
        save.Add("Filename", this.Filename);
        save.Add("Parent", this.GetParent().GetPath());
        return save;
    }
}