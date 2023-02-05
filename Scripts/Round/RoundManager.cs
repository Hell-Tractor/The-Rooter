using Godot;
using System.Collections.Generic;

public class RoundManager : Node {
    private List<IRoundable> _roundables = new List<IRoundable>();
    public static RoundManager Instance { get; private set; } = null;

    public override void _EnterTree() {
        if (Instance != null)
            throw new System.Exception("Duplicate RoundManager Exists.");
        Instance = this;
    }

    public void AddRoundable(IRoundable roundable) {
        _roundables.Add(roundable);
    }

    public void RemoveRoundable(IRoundable roundable) {
        _roundables.Remove(roundable);
    }

    public void Clear() {
        _roundables.Clear();
    }

    public void OnRoundFinish() {
        List<IRoundable> roundables = new List<IRoundable>(_roundables);
        roundables.Sort((a, b) => b.GetRoundPriority() - a.GetRoundPriority());
        roundables.ForEach(roundable => roundable.OnRoundFinish());
        roundables.ForEach(roundable => roundable.OnRoundLateFinish());
        roundables.ForEach(roundable => roundable.OnRoundStart());
    }
}