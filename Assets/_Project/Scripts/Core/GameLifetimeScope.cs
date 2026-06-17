using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using UnityEngine;
/// <summary>
/// VContainerにClass登録
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    // CharacterManagerが純粋C#Classの為、GameLifetimeScopeで宣言。
    [Header("キャラクター設定")]
    [SerializeField] private List<PlayerController> _characterPrefabs; 
    
    protected override void Configure(IContainerBuilder builder)
    {
        // Lifetime.Singleton -> CharacterManagerをSingleのように使える。
        // WithParameter(_characterPrefabs) -> プレハブをコンストラクタに伝達
        // AsImplementedInterfaces() -> IInitializableのようなInterfaceを使える。
        // AsSelf() -> 名前CharacterManagerでVContainerに登録
        builder.Register<CharacterManager>(Lifetime.Singleton)
            .WithParameter(_characterPrefabs)
            .AsImplementedInterfaces()
            .AsSelf();
        
        Debug.Log("[GameLifetimeScope] マネージャー登録");
    }
}