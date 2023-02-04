using Godot;
using System.Collections.Generic;

public class RoundManager : Node {
    private List<IRoundable> _roundables = new List<IRoundable>();
	private UndoManager undoManager;
    public static RoundManager Instance { get; private set; } = null;
    public override void _Ready()
    {
        base._Ready();
        if (Instance != null)
            throw new System.Exception("Duplicate RoundManager Exists.");
        Instance = this;
        this.undoManager = UndoManager.Instance;
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
        undoManager.Save();
        _roundables.Sort((a, b) => b.GetRoundPriority() - a.GetRoundPriority());
        _roundables.ForEach(roundable => roundable.OnRoundFinish());
        _roundables.ForEach(roundable => roundable.OnRoundLateFinish());
        _roundables.ForEach(roundable => roundable.OnRoundStart());
    }
}