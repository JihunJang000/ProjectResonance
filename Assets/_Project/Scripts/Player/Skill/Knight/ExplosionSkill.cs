using UnityEngine;

/// <summary>
/// 周辺敵に範囲ダメージ
/// </summary>
public class ExplosionSkill : CharacterSkillBase
{
    [SerializeField] private float _damage = 50f;
    [SerializeField] private float _explosionRadius = 3f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _vfxPrefab;
    
    protected override void DoSkillLogic()
    {
        Debug.Log("[ExplosionSkill] Skill発動");
        
        if (_vfxPrefab != null)
        {
            GameObject vfx = Instantiate(_vfxPrefab, transform.position, Quaternion.identity);
            
            vfx.transform.localScale = new Vector3(_explosionRadius * 2, _explosionRadius * 2, 1f);
            
            // プロトタイプ用：1.5秒後に削除（後でObjectPoolに変更またはパーティクルの自動削除機能を利用）
            Destroy(vfx, 0.5f); 
        }
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable enemy))
            {
                enemy.TakeDamage(_damage);
                // Debug.Log($"[ExplosionSkill] Enemy: {hit.name} Damage: {_damage}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}