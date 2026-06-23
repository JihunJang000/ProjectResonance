using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

/// <summary>
/// AI演算(UniTask)と 物理移動(FixedUpdate)を分離した最適化された敵コントローラー
/// </summary>
[RequireComponent(typeof(Rigidbody2D))] // RigidBody2D自動追加
public class EnemyController : MonoBehaviour
{
    //後で基本Statsは敵別分離する予定。
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _thinkInterval = 0.2f; // AIが思考する間隔

    private Rigidbody2D _rb;
    private CharacterManager _characterManager;
    private Transform _target;
    
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
    }

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        // 敵が画面に登場したら、思考ループを非同期で開始
        ThinkLoopAsync(_cts.Token).Forget();
    }

    private void OnDisable()
    {
        // 敵が死んだり非活性化されたらループを止める
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        
        _rb.linearVelocity = Vector2.zero;
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
                        // 後で攻撃ロジック追加
                    }
                }

                // 次の思考まで待機 （最適化用）
                await UniTask.Delay(TimeSpan.FromSeconds(_thinkInterval), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("[EnemyConroller] ThinkLoopAsync cancelled");
        }
    }

    /// <summary>
    /// FixedUpdateでわ重い計算わしないように構成
    /// </summary>
    private void FixedUpdate()
    {
        // 軽い計算のみ
        _rb.linearVelocity = _cachedMoveDirection * _moveSpeed;
    }
}