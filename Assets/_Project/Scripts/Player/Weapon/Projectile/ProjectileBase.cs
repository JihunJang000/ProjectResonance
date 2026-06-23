using UnityEngine;

/// <summary>
/// ProjectileのBase抽象クラス
/// 現在子クラス：貫通遠距離、敵に到着時範囲爆発魔法プロジェックタイル
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public abstract class ProjectileBase : MonoBehaviour
{
    protected Rigidbody2D Rb;
    protected float Damage;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }
    
    // 初期化
    public virtual void Initialize(Vector2 velocity, float damage)
    {
        Rb.linearVelocity = velocity;
        Damage = damage;
        
        // メモリリーク防止のため、5秒後に自動消滅
        Destroy(gameObject, 5f);
    }

    // Override
    protected abstract void OnTriggerEnter2D(Collider2D collision);
}