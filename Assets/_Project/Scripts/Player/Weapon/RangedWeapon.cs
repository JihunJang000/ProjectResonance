using UnityEngine;


/// <summary>
/// 遠距離武器：　貫通するProjectileを近い敵に自動発射。
/// </summary>
public class RangedWeapon : WeaponBase //Base抽象クラス1個+Interface達継承
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject _projectilePrefab; //Projectile 登録
    [SerializeField] private float _projectileSpeed = 12f;
    [SerializeField] private float _targetRange = 8f;
    [SerializeField] private LayerMask _enemyLayer;
    
    private PlayerController _playerController; //キャラ方向を得る為に。(今は使わない） 
    
    // GCを残さないように、あらかじめ配列宣言。NonAlloc方式で敵を探知。
    // 最大100。GC管理用。Profiling. 
    private readonly Collider2D[] _hitColliders = new Collider2D[100]; 
    public override void Attack()
    {
        // NonAlloc -> 関数が新しい配列を返却せず、あらかじめ宣言した配列にいれる。探知された敵の数だけ返却。
        // _hitColliders.Length使用禁止(NonAllocの特性がある為hitCount使用）
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, _targetRange, _hitColliders, _enemyLayer);

        // 敵ない時攻撃いない。
        if (hitCount == 0) return;

        Collider2D closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        // 近い敵距離計算、敵保存。
        for (int i = 0; i < hitCount; i++)
        {
            float distance = Vector2.Distance(transform.position, _hitColliders[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = _hitColliders[i];
            }
        }

        // Projectileを近い敵に発射。
        if (closestEnemy != null)
        {
            Vector2 fireDirection = ((Vector2)closestEnemy.transform.position - (Vector2)transform.position).normalized;
            
            GameObject projectileObj = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            if (projectileObj.TryGetComponent(out ProjectileBase projectile))
            {
                projectile.Initialize(fireDirection * _projectileSpeed, Damage);
            }
        }
    }
}