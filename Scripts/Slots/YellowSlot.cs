public class YellowSlot : SlotBase {
    protected override void OnLeafEnter(Leaf leaf) {
        this._trigger?.Trigger();
    }

    protected override void OnLeafExit(Leaf leaf) {
        this._trigger?.UnTrigger();
    }
}