using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class ChainDashSkill : CharacterSkillBase
{
    [SerializeField] private float _damage = 35f;
    [SerializeField] private float _searchRadius = 6f; // 敵を探す範囲
    [SerializeField] private int _maxChainCount = 5;    // 最大チェイン回数
    [SerializeField] private float _dashDelay = 0.15f;   // 敵と敵の間の移動間隔

    protected override void DoSkillLogic()
    {
        // UniTaskScopeを活用して非同期ダッシュループを実行
        ChainDashAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid ChainDashAsync(System.Threading.CancellationToken token)
    {
        Debug.Log("[KnightChainDashSkill] チェインダッシュ");
        
        Vector3 currentPos = transform.position;
        HashSet<Transform> visitedEnemies = new HashSet<Transform>();

        for (int i = 0; i < _maxChainCount; i++)
        {
            if (token.IsCancellationRequested) return;

            // 周辺の全コライダーを検出
            Collider2D[] hits = Physics2D.OverlapCircleAll(currentPos, _searchRadius);
            
            // まだ攻撃していない最も近い敵を探す
            Transform targetEnemy = hits
                .Where(h => h.GetComponent<IDamageable>() != null     // IDamageableを持っているか
                         && h.transform != transform                  // 自分自身ではないか
                         && !visitedEnemies.Contains(h.transform))    // まだ訪問していないか
                .OrderBy(h => Vector2.Distance(currentPos, h.transform.position)) // 現在位置から近い順に整列
                .FirstOrDefault()?.transform; // 一番最初の敵のTransformを取得（無ければnull）

            // これ以上連鎖する敵がいない場合は終了
            if (targetEnemy == null)
            {
                Debug.Log("[KnightChainDashSkill] Can't find enemy more");
                break;
            }

            // 敵の位置へダッシュ (瞬間移動)
            transform.position = targetEnemy.position;
            currentPos = transform.position;
            visitedEnemies.Add(targetEnemy); // 訪問済みリストに追加

            // ダメージを与える
            if (targetEnemy.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_damage);
                // Debug.Log($"[KnightChainDashSkill] {i + 1}回目のターゲット {targetEnemy.name} を強打");
            }

            // 次のチェインまで少し待機
            await UniTask.Delay(System.TimeSpan.FromSeconds(_dashDelay), cancellationToken: token);
        }
    }
}