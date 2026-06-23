using UnityEngine;

/// <summary>
/// 近距離武器。周辺に範囲ダメージ。
/// </summary>
public class MeleeWeapon : WeaponBase
{
    [Header("Weapon Settings")]
    [SerializeField] private float _attackRadius = 2.5f; 
    [SerializeField] private LayerMask _enemyLayer;      // InspectorでEnemy Layer設定

    private readonly Collider2D[] _hitColliders = new Collider2D[100];
    
    public override void Attack()
    {
        // OverlapCircleNonAlloc -> 新しい配列を生成せず、_hitCollidersに結果を上書きする
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, _attackRadius, _hitColliders, _enemyLayer);

        // 探知された敵がない場合は終了
        if (hitCount == 0) return;

        // 近距離敵にダメージ
        for (int i = 0; i < hitCount; i++) //.Length使用禁止。
        {
            if (_hitColliders[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Damage); // 継承したWeaponBaseのIDamageable
            }
        }
        
        Debug.Log($"[MeleeWeapon] 周辺 {hitCount}　体の敵を攻撃");
    }

    // Scene Check用
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}