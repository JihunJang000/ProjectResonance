using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///  playerの動きを new InputSystemを使って表現
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.3f;
    
    private PlayerInputActions _inputActions;　//　自動的に作られたInputAction情報を持っているクラス 
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    
    private bool _isDashing;
    private float _dashTimer;
    private Vector2 _dashDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // 인풋 액션 객체 생성 및 이벤트 구독 (콜백 방식)
        _inputActions = new PlayerInputActions();
        
        _inputActions.Player.Move.performed += context => _moveInput = context.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += context => _moveInput = Vector2.zero;
        
        _inputActions.Player.Dash.started += context => Dash();
    }

    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            _dashTimer -= Time.fixedDeltaTime; 

            //タイマーが残った場合
            if (_dashTimer > 0f)
            {
                _rb.linearVelocity = _dashDirection * _dashSpeed;　
            }
            // 残ってない場合
            else
            {
                _isDashing = false;
                _rb.linearVelocity = Vector2.zero;
            }
            
            return; 
        }
        
        _rb.linearVelocity = _moveInput * _moveSpeed;
    }

    private void Dash()
    {
        if (_isDashing || _moveInput == Vector2.zero) return;

        Debug.Log("[PlayerController.Dash] Dash");
        
        _isDashing = true;                      // ダッシュ状態ON
        _dashTimer = _dashDuration;             // タイマー初期化
        _dashDirection = _moveInput.normalized; // 方向固定
    }
}