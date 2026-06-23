using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

/// <summary>
/// Player戦闘担当
/// Playerの体力、ひげき、一般攻撃、SKill管理。
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour, IDamageable
{
    [Header("Player Stats")]
    [SerializeField] private float _maxHp = 100f;
    private float _currentHp;

    [Header("Default Weapon Prefab")]
    [SerializeField] private WeaponBase _startingWeaponPrefab; //武器原本データ
    private WeaponBase _equippedWeapon; // 実際装備した武器。
    
    // Todo: 後でSkill, Ult追加予定。

    private RoundManager _roundManager;
    private CancellationTokenSource _combatCts;

    [Inject]
    public void Construct(RoundManager roundManager)
    {
        _roundManager = roundManager;
    }

    private void Awake()
    {
        _currentHp = _maxHp;
    }

    private void Start()
    {
        // 基本武器装着
        if (_startingWeaponPrefab != null)
        {
            EquipWeapon(_startingWeaponPrefab);
        }
    }
    
    private void OnEnable()
    {
        _combatCts = new CancellationTokenSource();
        AutoAttackLoopAsync(_combatCts.Token).Forget();
    }

    private void OnDisable()
    {
        _combatCts?.Cancel();
        _combatCts?.Dispose();
        _combatCts = null;
    }

    /// <summary>
    /// N秒周期で発動する自動一般攻撃。（UniTaskしよう）
    /// </summary>
    private async UniTaskVoid AutoAttackLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (_roundManager.CurrentState.CurrentValue == RoundState.GameOver) return;

                if (_equippedWeapon != null)
                {
                    // 抽象クラスWeaponBaseのAttack関数
                    _equippedWeapon.Attack();

                    // 攻撃周期分だけ待機。
                    await UniTask.Delay(TimeSpan.FromSeconds(_equippedWeapon.AttackInterval), cancellationToken: token);
                }
                else
                {
                    // 武器がないばあい
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
                }
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    //武器装着、交代。（Playerと武器の位置同期化。）
    public void EquipWeapon(WeaponBase weaponPrefab)
    {
        //既存武器破壊。
        if (_equippedWeapon != null)
        {
            Destroy(_equippedWeapon.gameObject);
        }

        // 武器をPlayerの子に。
        WeaponBase newWeapon = Instantiate(weaponPrefab, transform);
        
        // 位置一致させる。
        newWeapon.transform.localPosition = Vector3.zero;
        
        _equippedWeapon = newWeapon;
        
        Debug.Log($"[PlayerCombat] {_equippedWeapon.name} Weapon Equipped");
    }
    
    /// <summary>
    /// ひげきされた場合
    /// </summary>
    public void TakeDamage(float damage)
    {
        _currentHp -= damage;
        // Debug.Log($"[Player] Damage: {damage}, CurrentHP: {_currentHp}");

        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[Player] Player Died");
        _roundManager.TriggerGameOver(); 
        gameObject.SetActive(false);
    }
    
}



