using UnityEngine;

public class ShotgunSpreadSkill : CharacterSkillBase
{
    [SerializeField] private GameObject _arrowPrefab; // 弾プレハブ
    [SerializeField] private float _damage = 20f;     // 1発あたりのダメージ
    [SerializeField] private float _projectileSpeed = 15f;
    
    // 5つの扇形の角度
    private readonly float[] _angles = { -30f, -15f, 0f, 15f, 30f };

    protected override void DoSkillLogic()
    {
        Debug.Log("[ShotgunSpreadSkill] ShotgunSpreadSkill 射撃");
        
        Vector2 lookDir = Vector2.right; // デフォルトの向き（右）
        
        // GameObjectにいるPlayerController参照
        if (TryGetComponent(out PlayerController player))
        {
            // キャラクターが移動中の場合、移動入力を基準
            if (player.MoveInput != Vector2.zero)
            {
                lookDir = player.MoveInput.normalized;
            }
            else
            {
                // 停止中の場合、キャラクターの反転状態(localScale)から向きを推測
                lookDir = transform.localScale.x < 0 ? Vector2.left : Vector2.right;
            }
        }

        // 決定されたベクトルから基本角度を計算
        float baseAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        foreach (float angleOffset in _angles)
        {
            // 基準角度に扇形のオフセットを足して最終的な回転値を計算
            float finalAngle = baseAngle + angleOffset;
            Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);

            // 弾を生成
            GameObject arrow = Instantiate(_arrowPrefab, transform.position, rotation);
            
            // 生成された弾が飛んでいく方向ベクトルを計算
            Vector2 dir = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));
            
            if (arrow.TryGetComponent(out ProjectileBase projectile))
            {
                projectile.Initialize(dir * _projectileSpeed, _damage);
            }
        }
    }
}