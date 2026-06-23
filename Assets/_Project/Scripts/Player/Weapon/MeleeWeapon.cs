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
    
    private ContactFilter2D _contactFilter;
    
    private void Awake()
    {
        //　敵のColliderのisTriggerがOnされても攻撃するように設定。
        _contactFilter = new ContactFilter2D();
        _contactFilter.useLayerMask = true;
        _contactFilter.SetLayerMask(_enemyLayer);
        _contactFilter.useTriggers = true;
    }
    
    public override void Attack()
    {
        
        Debug.Log("[Weapon] Attack() 実行");
        
        // OverlapCircle -> 新しい配列を生成せず、_hitCollidersに結果を上書きする
        int hitCount = Physics2D.OverlapCircle(transform.position, _attackRadius, _contactFilter, _hitColliders);
        
        Debug.Log($"hitCount: {hitCount}");
        // 探知された敵がない場合は終了
        if (hitCount == 0) return;

        // 近距離敵にダメージ
        for (int i = 0; i < hitCount; i++) //.Length使用禁止。
        {
            if (_hitColliders[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Damage); // 継承したWeaponBaseのIDamageable
                Debug.Log($"Damage: {Damage}");
            }
            else
            {
                Debug.Log("IDamageable is null");
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