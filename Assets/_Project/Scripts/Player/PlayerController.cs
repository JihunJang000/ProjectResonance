using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

/// <summary>
/// Player移動担当
///  playerの動きを new InputSystemを使って表現
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f; 
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.3f;
    
    [Header("Skills")]
    [SerializeField] private CharacterSkillBase _mainSkill;     // Q Skill
    [SerializeField] private CharacterSkillBase _ultimateSkill; // E Skill
    
    private PlayerInputActions _inputActions;　//　自動的に作られたInputAction情報を持っているクラス 
    private InputManager _inputManager;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    
    //プロパティー
    public StateMachine StateMachine { get; private set; }
    
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public DashState DashState { get; private set; }
    public Rigidbody2D Rb => _rb; // get専用
    public Vector2 MoveInput => _moveInput;
    public float MoveSpeed => _moveSpeed;
    public float DashSpeed => _dashSpeed;
    public float DashDuration => _dashDuration;
    
    
    [Inject]
    public void Construct(InputManager inputManager)
    {
        _inputManager = inputManager;
    }
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // InputActionからInput情報を得る為に宣言
        _inputActions = new PlayerInputActions();
        
        _inputActions.Player.Move.performed += context => _moveInput = context.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += context => _moveInput = Vector2.zero;
        
        _inputActions.Player.Dash.started += context => Dash();

        StateMachine = new StateMachine();
        
        // State再利用用
        IdleState = new IdleState(this, StateMachine);
        MoveState = new MoveState(this, StateMachine);
        DashState = new DashState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(this.IdleState); 
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        
        if (_inputManager != null)
        {
            _inputManager.OnMainSkillPressed += ExecuteMainSkill;
            _inputManager.OnUltimatePressed += ExecuteUltimateSkill;
        }
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        
        if (_inputManager != null)
        {
            _inputManager.OnMainSkillPressed -= ExecuteMainSkill;
            _inputManager.OnUltimatePressed -= ExecuteUltimateSkill;
        }
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    void Update()
    {
        StateMachine.Update();
    }

    private void Dash()
    {
        //　動かない場合と既にダッシュしている場合 Return.
        if (this._moveInput == Vector2.zero || StateMachine.CurrentState == DashState) return;
        
        StateMachine.ChangeState(DashState);
    }
    
    private void ExecuteMainSkill()
    {
        if (_mainSkill != null) _mainSkill.ExecuteSkill();
    }

    private void ExecuteUltimateSkill()
    {
        if (_ultimateSkill != null) _ultimateSkill.ExecuteSkill();
    }
}