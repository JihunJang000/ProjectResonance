using UnityEngine;
using System.Linq;

public class SingleTargetSnipeSkill : CharacterSkillBase
{
    [SerializeField] private GameObject _arrowPrefab;   // 弾プレハブ
    [SerializeField] private float _damage = 80f;       // 単体ダメージ
    [SerializeField] private float _projectileSpeed = 20f;
    [SerializeField] private float _searchRadius = 10f; // ターゲット検索範囲

    protected override void DoSkillLogic()
    {
        // まだ攻撃していない近い敵探し
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _searchRadius);
        Transform closestEnemy = hits
            .Where(h => h.GetComponent<IDamageable>() != null && h.transform != transform)
            .OrderBy(h => Vector2.Distance(transform.position, h.transform.position))
            .FirstOrDefault()?.transform;

        // 発射方向決定
        Vector2 shootDirection = Vector2.right; 
        if (closestEnemy != null)
        {
            shootDirection = (closestEnemy.position - transform.position).normalized;
        }

        // 弾を生成して角度を合わせる
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        GameObject arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        
        
        if (arrow.TryGetComponent(out ProjectileBase projectile))
        {
            projectile.Initialize(shootDirection * _projectileSpeed, _damage);
        }
        
        Debug.Log($"[SingleTargetSnipeSkill] 単体狙撃 Damage: {_damage}");
    }
}