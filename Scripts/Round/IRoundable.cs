public interface IRoundable {
    // called when the round starts
    void OnRoundStart();
    // called when the round ends
    void OnRoundFinish();
    // called after all objects have been notified of the round ending
    void OnRoundLateFinish();
    int GetRoundPriority();
}