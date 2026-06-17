using UnityEngine;

public class MoveState : IState
{
    private PlayerController _player;
    private StateMachine _stateMachine;

    public MoveState(PlayerController player, StateMachine stateMachine)
    {
        _player = player;
        _stateMachine = stateMachine;
    }
    
    public void Enter()
    {
        // Debug.Log("[MoveState] Enter");
    }

    public void FixedUpdate()
    {
        _player.Rb.linearVelocity = _player.MoveInput * _player.MoveSpeed;
    }

    public void Update()
    {
        if (_player.MoveInput == Vector2.zero)
        {
            _stateMachine.ChangeState(_player.IdleState);
        }
    }

    public void Exit()
    {
        // Debug.Log("[MoveState] Exit");
    }
}
