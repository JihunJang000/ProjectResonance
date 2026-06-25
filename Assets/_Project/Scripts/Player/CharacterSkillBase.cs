using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using System;

// CharacterのSkillが継承するSkill Base抽象クラス
public abstract class CharacterSkillBase : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] protected float _cooldown = 5f;
    
    public ReactiveProperty<bool> IsReady { get; } = new ReactiveProperty<bool>(true);

    public void ExecuteSkill()
    {
        if (!IsReady.Value) return; 
        
        IsReady.Value = false; 
        DoSkillLogic();        
        
        StartCooldownAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    // 格Skillごとに実装
    protected abstract void DoSkillLogic();

    private async UniTaskVoid StartCooldownAsync(System.Threading.CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_cooldown), cancellationToken: token);
            IsReady.Value = true; 
            Debug.Log($"[Skill] Skill On");
        }
        catch (OperationCanceledException) { }
    }
}