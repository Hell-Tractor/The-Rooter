public class YellowSlot : SlotBase {
    protected override void OnLeafEnter(Stem leaf) {
        this._trigger.Trigger();
    }

    protected override void OnLeafExit(Stem leaf) {
        this._trigger.UnTrigger();
    }
}