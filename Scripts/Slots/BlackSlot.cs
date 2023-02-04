public class BlackSlot : SlotBase {
    protected override void OnLeafEnter(Leaf leaf) {
        leaf.QueueFree();
    }
}