using System.Collections.Generic;
using GC = Godot.Collections;

public class Root : PlantBase {
    public List<Leaf> GetConnectedLeaves() {
        List<Leaf> result = new List<Leaf>();
        List<bool> visited = new List<bool>(Count);
        for (int i = 0; i < Count; i++) {
            visited.Add(false);
        }
        Queue<PlantBase> q = new Queue<PlantBase>();
        visited[this.Id] = true;
        for (q.Enqueue(this); q.Count > 0;) {
            PlantBase u = q.Dequeue();
            foreach (PlantBase v in u._next) {
                if (!visited[v.Id]) {
                    visited[v.Id] = true;
                    if (v.GetStemType() == StemType.Leaf) {
                        result.Add(v as Leaf);
                    } else
                        q.Enqueue(v);
                }
            }
        }
        return result;
    }

    public override GC.Dictionary<string, object> Save() {
        GC.Dictionary<string, object> save = base.Save();
        save.Add("Filename", this.Filename);
        save.Add("Parent", this.GetParent().GetPath());
        return save;
    }
}