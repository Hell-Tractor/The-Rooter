public class BlackSlot : SlotBase {
    protected override void OnLeafEnter(Stem leaf) {
        leaf.QueueFree();
    }
}