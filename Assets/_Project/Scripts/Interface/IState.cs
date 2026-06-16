using UnityEngine;

/// <summary>
/// FSM使用
/// </summary>
public interface IState
{
    void Enter();
    void FixedUpdate();
    void Update();
    void Exit();
    
}
