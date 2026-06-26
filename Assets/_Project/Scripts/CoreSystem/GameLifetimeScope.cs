using System.Collections.Generic;
using Unity.Cinemachine;
using VContainer;
using VContainer.Unity;
using UnityEngine;

/// <summary>
/// VContainerにClass登録
/// 後でRootLifetimeScope, InGameScope, UIScopeに分離する予定。
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    // CharacterManagerが純粋C#Classの為、GameLifetimeScopeで宣言。
    //　Todo: 後でSOに変更する予定。
    [SerializeField] private List<PlayerController> _characterPrefabs; 
    [SerializeField] private CinemachineCamera _virtualCamera;
    
    [SerializeField] private StageSpawnDataSO _stageSpawnDataSO; // Todo: 後でStage複数使う為にListに変更
    
    protected override void Configure(IContainerBuilder builder)
    {
        // Lifetime.Singleton -> CharacterManagerをSingleのように使える。
        // WithParameter(_characterPrefabs) -> プレハブをコンストラクタに伝達
        // AsImplementedInterfaces() -> IInitializableのようなInterfaceを使える。
        // AsSelf() -> 名前CharacterManagerでVContainerに登録
        builder.Register<CharacterManager>(Lifetime.Singleton)
            .WithParameter(_characterPrefabs)
            .WithParameter(_virtualCamera)
            .AsImplementedInterfaces()
            .AsSelf();

        builder.Register<EnemySpawnManager>(Lifetime.Singleton)
            .WithParameter(_stageSpawnDataSO)
            .AsImplementedInterfaces()
            .AsSelf();
        
        builder.Register<RoundManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        
        builder.Register<InputManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        // UI関連
        builder.RegisterComponentInHierarchy<GameResultPresenter>();
        builder.RegisterComponentInHierarchy<InGameHUDPresenter>();
        
        //Build完了前に強制注入
        builder.RegisterBuildCallback(resolver => resolver.Resolve<GameResultPresenter>());
        
        Debug.Log("[GameLifetimeScope] マネージャー登録");
    }
}