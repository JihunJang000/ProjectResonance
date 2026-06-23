using UnityEngine;

/// <summary>
/// 遠距離攻撃： 貫通矢。
/// </summary>
public class PiercingProjectile : ProjectileBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(Damage);
            
            // Debug.Log($"[PiercingProjectile] ダメージ: {Damage}");
        }
    }
}