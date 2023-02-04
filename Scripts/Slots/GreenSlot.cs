using Godot;
using System.Collections.Generic;

public class GreenSlot : SlotBase, ISave {
    protected override void OnLeafEnter(Leaf leaf) {
        this._trigger.Trigger();
        leaf.QueueFree();
        this.QueueFree();
    }

    public Dictionary<string, object> Save() {
        return new Dictionary<string, object>() {
            { "Filename", this.Filename },
            { "Parent", this.GetParent().GetPath() }
        };
    }
}