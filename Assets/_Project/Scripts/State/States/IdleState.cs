using UnityEngine;

public class IdleState : IState
{
    private PlayerController _player;
    private StateMachine _stateMachine;
    
    public IdleState(PlayerController player, StateMachine stateMachine)
    {
        _player = player;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _player.Rb.linearVelocity = Vector2.zero;
        Debug.Log("[IdleState] Enter");
    }

    public void Update()
    {
        // 方向キー入力さえたらMoveStateで入れ替え
        if (_player.MoveInput != Vector2.zero)
        {
            _stateMachine.ChangeState(_player.MoveState);
        }
    }

    public void FixedUpdate() { }

    public void Exit()
    {
        Debug.Log("[IdleState] Exit");
    }
}