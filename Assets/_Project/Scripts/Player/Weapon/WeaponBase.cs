using UnityEngine;

/// <summary>
/// 武器の基本抽象クラス
/// </summary>
public abstract class WeaponBase : MonoBehaviour //MonoBehaviourはWeaponBaseが継承。
{
    //　共通変数があるので抽象クラスで宣言
    public float AttackInterval = 1.0f; 
    public float Damage = 10f;

    // overrideするべき関数達。。
    public abstract void Attack(); 
}