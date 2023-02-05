using System.Collections.Generic;
using Godot;

namespace AI.FSM {

/// <summary>
/// <para>有限状态机</para>
/// <para>子类需要调用父类Start, Update方法，调用并重写setUpFSM方法</para>
/// </summary>
public class FSMBase : Area2D {
    /// <summary>
    /// 初始化状态机，子类需要调用base.setUpFSM()
    /// </summary>
    protected virtual void setUpFSM() {
        _states = new List<FSMState>();
    }
    /// <summary>
    /// 初始化其他组件
    /// </summary>
    protected virtual void init() {
    }
    protected void loadDefaultState() {
        // 加载默认状态
        _currentState = _defaultState = _states.Find(s => s.StateID == defaultStateID);
        _currentState.OnStateEnter(this);
        GD.Print(_currentState);
    }
    public override void _Ready() {
        GD.Print("what");
        init();
        setUpFSM();
        loadDefaultState();
    }
    public override void _Process(float delta) {
        // 更新状态
        GD.Print(_currentState);
        _currentState.Reason(this);
        _currentState.OnStateStay(this);
    }
    /// <summary>
    /// 切换当前状态至目标状态
    /// </summary>
    /// <param name="targetStateID">目标状态ID</param>
    public void changeActiveState(FSMStateID targetStateID) {
        _currentState.OnStateExit(this);
        _currentState = targetStateID == FSMStateID.Default ? _defaultState : _states.Find(s => s.StateID == targetStateID);
        _currentState.OnStateEnter(this);
    }
    public void SetTrigger(FSMTriggerID triggerID) {
        _settledTriggers.Add(triggerID);
    }
    public void ResetTrigger(FSMTriggerID triggerID) {
        _settledTriggers.Remove(triggerID);
    }
    public bool HasTrigger(FSMTriggerID triggerID) {
        return _settledTriggers.Contains(triggerID);
    }

    [Export]
    public FSMStateID defaultStateID;
    protected FSMState _defaultState;
    protected List<FSMState> _states;
    public FSMState _currentState;
    protected SortedSet<FSMTriggerID> _settledTriggers = new SortedSet<FSMTriggerID>();
    public FSMStateID CurrentState {
        get {
            return _currentState.StateID;
        }
    }
}

}