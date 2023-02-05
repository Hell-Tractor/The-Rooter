using Godot;

public class Obstacle : Area2D, ITrigger {
    [Export]
    public int RequireCount;
    private int _currentCount = 0;

    public void Trigger() {
        if (++_currentCount == RequireCount)
            this.QueueFree();
    }

    public void UnTrigger() {
        _currentCount--;
    }
}