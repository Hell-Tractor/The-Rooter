using Godot;
using System.Collections.Generic;

public class RoundManager : Node {
    private List<IRoundable> _roundables = new List<IRoundable>();

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
        _roundables.Sort((a, b) => b.GetRoundPriority() - a.GetRoundPriority());
        _roundables.ForEach(roundable => roundable.OnRoundFinish());
        _roundables.ForEach(roundable => roundable.OnRoundLateFinish());
        _roundables.ForEach(roundable => roundable.OnRoundStart());
    }
}