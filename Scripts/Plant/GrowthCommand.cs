using Godot;
using System.Collections.Generic;

public class GrowthCommand {
    public Vector2 Direction { get; private set; }
    private Queue<bool> _commandQueue;
    public bool Command {
        get { return _commandQueue.Dequeue(); }
        set { _commandQueue.Enqueue(value); }
    }

    public GrowthCommand(Vector2 direction, int delay) {
        Direction = direction;

        _commandQueue = new Queue<bool>(delay + 1);
        for (int i = 0; i < delay; i++) {
            _commandQueue.Enqueue(false);
        }
    }
}