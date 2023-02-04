using System.Linq;
using Godot;

public class SlotBase : Area2D, IRoundable {
    [Export]
    public NodePath RoundManagerPath;
    private RoundManager _roundManager;
    [Export]
    public int RoundPriority;
    private Leaf _lastLeaf = null;
    [Export]
    public NodePath TriggerPath;
    protected ITrigger _trigger;

    public override void _Ready() {
        _roundManager = this.GetNode<RoundManager>(RoundManagerPath);
        _roundManager.AddRoundable(this);

        _trigger = this.GetNode<ITrigger>(TriggerPath);
    }

    public int GetRoundPriority() {
        return RoundPriority;
    }

    public void OnRoundFinish() {
        Leaf leaf = this.GetOverlappingAreas().OfType<Leaf>().FirstOrDefault(null);
        if (leaf == _lastLeaf)
            return;
        if (leaf != null)
            OnLeafEnter(leaf);
        else
            OnLeafExit(_lastLeaf);
        _lastLeaf = leaf;
    }

    protected virtual void OnLeafEnter(Leaf leaf) {}
    protected virtual void OnLeafExit(Leaf leaf) {}

    public void OnRoundLateFinish() {}
    public void OnRoundStart() {}
}