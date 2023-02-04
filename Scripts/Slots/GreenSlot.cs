using Godot;

public class GreenSlot : SlotBase {
    protected override void OnLeafEnter(Stem leaf) {
        this._trigger.Trigger();
        leaf.QueueFree();
        this.QueueFree();
    }
}