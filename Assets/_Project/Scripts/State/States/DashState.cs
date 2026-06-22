using UnityEngine;

public class DashState : IState
{
    private PlayerController _player;
    private StateMachine _stateMachine;
    
    private float _dashTimer;
    private Vector2 _dashDirection;

    public DashState(PlayerController player, StateMachine stateMachine)
    {
        _player = player;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        // Debug.Log("[DashState] Enter");
        // 状態進入する時初期化
        _dashTimer = _player.DashDuration;             
        _dashDirection = _player.MoveInput.normalized; 
    }

    public void Update() { }

    public void FixedUpdate()
    {
        _dashTimer -= Time.fixedDeltaTime; 

        if (_dashTimer > 0f)
        {
            _player.Rb.linearVelocity = _dashDirection * _player.DashSpeed; 
        }
        else
        {
            // ダイマが終わると復帰
            if (_player.MoveInput != Vector2.zero)
                _stateMachine.ChangeState(_player.MoveState);
            else
                _stateMachine.ChangeState(_player.IdleState);
        }
    }

    public void Exit()
    {
        // Debug.Log("[DashState] Exit");
        _player.Rb.linearVelocity = Vector2.zero;
    }
}