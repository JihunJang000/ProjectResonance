using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using R3;

/// <summary>
/// AI演算(UniTask)と 物理移動(FixedUpdate)を分離した最適化された敵コントローラー
/// </summary>
[RequireComponent(typeof(Rigidbody2D))] // RigidBody2D自動追加
public class EnemyController : MonoBehaviour, IDamageable
{
    // Todo: 後で基本Statsは敵別分離する予定。
    // Todo：後でEnemyController -> Model, Controllerで分離する予定。
    
    // Subject -> R3でのActionと類似なもの。
    public Subject<(float damage, Vector3 position)> OnDamaged { get; } = new Subject<(float, Vector3)>();　//Damage関連
    public Subject<float> OnHpRatioChanged { get; } = new Subject<float>(); //HP関連
    
    
    [SerializeField] private float _hp = 30f;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackCooldown = 1.0f;
    [SerializeField] private float _thinkInterval = 0.2f; // AIが思考する間隔

    private float _maxHp;
    
    private Rigidbody2D _rb;
    private CharacterManager _characterManager;
    private Transform _target;
    private float _lastAttackTime = 0f;
    
    
    // 計算済みの移動方向を保存しておく変数
    private Vector2 _cachedMoveDirection = Vector2.zero; 
    
    private CancellationTokenSource _cts;

    [Inject]
    public void Construct(CharacterManager characterManager)
    {
        _characterManager = characterManager;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _maxHp = _hp;
    }

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        // 敵が画面に登場したら、思考ループを非同期で開始
        ThinkLoopAsync(_cts.Token).Forget();
    }

    
    // FixedUpdateでわ重い計算わしないように構成
    private void FixedUpdate()
    {
        // 軽い計算のみ
        _rb.linearVelocity = _cachedMoveDirection * _moveSpeed;
    }
    
    private void OnDisable()
    {
        // 敵が死んだり非活性化されたらループを止める
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        
        _rb.linearVelocity = Vector2.zero;
    }

    private void OnDestroy()
    {
        // R3メモリー解除
        OnDamaged.Dispose();
        OnHpRatioChanged.Dispose();
    }
    
    public void TakeDamage(float damage)
    {
        _hp -= damage;
        // OnNext -> Invokeと似ている関数
        OnDamaged.OnNext((damage, transform.position + Vector3.up * 0.5f));　// 
        OnHpRatioChanged.OnNext(Mathf.Clamp01(_hp / _maxHp));
        if (_hp <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Debug.Log("[Enemy] Enemy Died");
        Destroy(gameObject); // Todo: 後でObjectPoolに変更。
    }
    
    /// <summary>
    /// 0.2秒ごとに1回だけターゲットの方向と距離を計算する非同期関数
    /// </summary>
    private async UniTaskVoid ThinkLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                _target = _characterManager.CurrentPlayerTransform;

                if (_target != null)
                {
                    float distance = Vector2.Distance(transform.position, _target.position);

                    if (distance > _attackRange)
                    {
                        // 追跡状態
                        _cachedMoveDirection = (_target.position - transform.position).normalized;
                    }
                    else
                    {
                        // 攻撃範囲内にいる時わ停止
                        _cachedMoveDirection = Vector2.zero;
                        
                        // Colltimeチェック
                        if (Time.time >= _lastAttackTime + _attackCooldown)
                        {
                            // プレイヤーにダメージ
                            if (_target.TryGetComponent(out IDamageable playerDamageable))
                            {
                                playerDamageable.TakeDamage(_attackDamage);
                                Debug.Log($"[EnemyController] プレイヤーを攻撃 ダメージ: {_attackDamage}");
                                _lastAttackTime = Time.time; 
                            }
                        }
                        
                    }
                }

                // 次の思考まで待機 （最適化用）
                await UniTask.Delay(TimeSpan.FromSeconds(_thinkInterval), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("[EnemyController] ThinkLoopAsync cancelled");
        }
    }
}