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
        GD.Print("Add Roundable: " + roundable);
        _roundables.Add(roundable);
    }

    public void RemoveRoundable(IRoundable roundable) {
        GD.Print("Remove Roundable: " + roundable);
        _roundables.Remove(roundable);
    }

    public void Clear() {
        _roundables.Clear();
    }

    public void OnRoundFinish() {
        UndoManager.Instance.Save();
        List<IRoundable> roundables = new List<IRoundable>(_roundables);
        GD.Print("To Round Finish: " + roundables.Count);
        roundables.Sort((a, b) => b.GetRoundPriority() - a.GetRoundPriority());
        roundables.ForEach(roundable => roundable.OnRoundFinish());
        GD.Print("To Round Late Finish: " + roundables.Count);
        roundables.ForEach(roundable => roundable.OnRoundLateFinish());
        GD.Print("To Round Start: " + roundables.Count);
        roundables.ForEach(roundable => roundable.OnRoundStart());
    }
}