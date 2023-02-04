using System.Linq;
using Godot;

public class SlotBase : Area2D, IRoundable {
    [Export]
    public NodePath RoundManagerPath;
    private RoundManager _roundManager;
    [Export]
    public int RoundPriority;
    private Stem _lastStem = null;
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
        Stem leaf = this.GetOverlappingAreas().OfType<Stem>().Where(stem => stem.isLeaf).FirstOrDefault(null);
        if (leaf == _lastStem)
            return;
        if (leaf != null)
            OnLeafEnter(leaf);
        else
            OnLeafExit(_lastStem);
        _lastStem = leaf;
    }

    protected virtual void OnLeafEnter(Stem leaf) {}
    protected virtual void OnLeafExit(Stem leaf) {}
}