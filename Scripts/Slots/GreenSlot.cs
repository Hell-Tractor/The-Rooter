using Godot;

public class GreenSlot : SlotBase {
    protected override void OnLeafEnter(Leaf leaf) {
        this._trigger.Trigger();
        leaf.QueueFree();
        this.QueueFree();
    }
}