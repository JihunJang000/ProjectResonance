/// <summary>
/// Stateを管理する役割
/// </summary>
public class StateMachine
{
    public IState CurrentState { get; private set; } //現在のState格納

    // FSM初期State実行
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    // State入れ替え
    public void ChangeState(IState newState)
    {
        CurrentState?.Exit();
        
        CurrentState = newState;
        CurrentState.Enter();
    }

    //　Monobehaviourを継承したPlayerControllerで実行
    public void Update()
    {
        CurrentState?.Update();
    }

    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}