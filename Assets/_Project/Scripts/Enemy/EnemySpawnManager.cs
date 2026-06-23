using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

/// <summary>
/// ラウンド状態を購読してEnemy生成するManager
/// </summary>
public class EnemySpawnManager : IInitializable, IDisposable
{
    private readonly RoundManager _roundManager;
    private readonly IObjectResolver _resolver; //Instantiate用
    private readonly StageSpawnDataSO _stageData; 
    
    private readonly CompositeDisposable _disposables = new();
    private CancellationTokenSource _spawnCts;

    
    public EnemySpawnManager(RoundManager roundManager, IObjectResolver resolver, StageSpawnDataSO stageData)
    {
        _roundManager = roundManager;
        _resolver = resolver;
        _stageData = stageData;
    }

    public void Initialize()
    {
        // R3変数CurrentStateに購読
        _roundManager.CurrentState
            .Subscribe(HandleRoundStateChanged)
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
        CancelSpawning();
    }

    // ラウンドが変わる度にSpawn速度変更
    private void HandleRoundStateChanged(RoundState state)
    {
        CancelSpawning(); 

        // ラウンド別StageSpawnDataSO変更, Spawn時間変更.
        if (state == RoundState.Round1)
        {
            _spawnCts = new CancellationTokenSource();
            SpawnLoopAsync(_stageData.Round1Enemies, 2f, _spawnCts.Token).Forget();
        }
        else if (state == RoundState.Round2)
        {
            _spawnCts = new CancellationTokenSource();
            SpawnLoopAsync(_stageData.Round2Enemies, 1.5f, _spawnCts.Token).Forget();
        }
        else if (state == RoundState.Round3)
        {
            _spawnCts = new CancellationTokenSource();
            SpawnLoopAsync(_stageData.Round3Enemies, 1f, _spawnCts.Token).Forget();
        }
    }

    //　以前ラウンドUniTask作業キャンセル
    private void CancelSpawning()
    {
        _spawnCts?.Cancel();
        _spawnCts?.Dispose();
        _spawnCts = null;
    }

    /// <summary>
    /// 指定された敵をIntervalごとに召喚。
    /// </summary>
    private async UniTaskVoid SpawnLoopAsync(List<EnemySpawnData> enemyList, float intervalSeconds, CancellationToken token)
    {
        try 
        {
            while (!token.IsCancellationRequested)
            {
                // Game Over -> 即座に終止。
                if (_roundManager.CurrentState.CurrentValue == RoundState.GameOver) return;

                SpawnRandomEnemy(enemyList);
                await UniTask.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    /// <summary>
    /// EnemySpawnDataからの重み基盤で敵一人を選ぶ一般的な関数
    /// </summary>
    private void SpawnRandomEnemy(List<EnemySpawnData> enemyList)
    {
        if (enemyList == null || enemyList.Count == 0) return;

        // 重み総合計算
        int totalWeight = 0;
        foreach (var data in enemyList)
        {
            totalWeight += data.SpawnWeight;
        }
            

        // 一人選択
        int randomValue = Random.Range(0, totalWeight);
        
        EnemyController selectedPrefab = null;
        int currentWeight = 0;

        
        foreach (var data in enemyList)
        {
            currentWeight += data.SpawnWeight;
            if (randomValue < currentWeight)
            {
                selectedPrefab = data.EnemyPrefab;
                break;
            }
        }
        
        if (selectedPrefab != null)
        {
            // 臨時位置（後で修正）
            Vector2 spawnPos = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            _resolver.Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        }
    }
}