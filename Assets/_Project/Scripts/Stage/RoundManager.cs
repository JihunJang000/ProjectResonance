using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

// Round状態 Enum宣言
public enum RoundState
{
    None, Ready, Round1, Round2, Round3, Victory, GameOver
}

/// <summary>
/// ゲームの進行時間管理、Round管理
/// </summary>
public class RoundManager : IInitializable, IDisposable
{
    // R3で変数の変更事項をUIに反映
    private readonly ReactiveProperty<RoundState> _currentState = new(RoundState.None);
    private readonly ReactiveProperty<int> _currentRoundNum = new(0);
    private readonly ReactiveProperty<float> _remainingTime = new(0f);

    // プロパティー ->　ReadOnlyで外部から読むことしかできないように
    public ReadOnlyReactiveProperty<RoundState> CurrentState => _currentState;
    public ReadOnlyReactiveProperty<int> CurrentRoundNum => _currentRoundNum;
    public ReadOnlyReactiveProperty<float> RemainingTime => _remainingTime;

    // CancellationTokenSource -> 作業cancel switch
    private CancellationTokenSource _cts;

    public void Initialize()
    {
        _cts = new CancellationTokenSource();
        // 非同期関数.Forget() -> 非同期関数開始し、バックグラウンドで実行。結果を待たず次の行え移動。
        StartGameTimelineAsync(_cts.Token).Forget(); 
    }

    public void Dispose()
    {
        // UniTask取り消し。
        _cts?.Cancel();
        _cts?.Dispose();
        
        // R3取り消し。
        _currentState.Dispose();
        _currentRoundNum.Dispose();
        _remainingTime.Dispose();
    }

    /// <summary>
    /// 1分ごとにラウンド転換。
    /// </summary>
    private async UniTaskVoid StartGameTimelineAsync(CancellationToken token)
    {
        try
        {
            // 準備(3秒待機)
            _currentState.Value = RoundState.Ready; // .ValueでR3変数の値変化を知らせる.(.Value = R3のみ関連)
            Debug.Log("[RoundManager] ゲーム準備中");
            // 非同期3秒。cancellationTokenもあげて非同期する時, 物体破壊有無を認識
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);

            // ラウンド１
            _currentState.Value = RoundState.Round1;
            _currentRoundNum.Value = 1;
            Debug.Log("[RoundManager] Round 1 Start "); 
            await CountDownAsync(60f, token);

            // ラウンド２
            _currentState.Value = RoundState.Round2;
            _currentRoundNum.Value = 2;
            Debug.Log("[RoundManager] Round 2 Start"); 
            await CountDownAsync(60f, token);

            // ラウンド３
            _currentState.Value = RoundState.Round3;
            _currentRoundNum.Value = 3;
            Debug.Log("[RoundManager] Round 3 Start");
            await CountDownAsync(60f, token);

            // 生存 -> 勝利
            _currentState.Value = RoundState.Victory;
            Time.timeScale = 0f;
            Debug.Log("[RoundManager] Stage Clear!");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[RoundManager] Time Management Function Quit");
        }
    }

    /// <summary>
    /// 指定した時間を1秒ずつける関数。
    /// </summary>
    private async UniTask CountDownAsync(float duration, CancellationToken token)
    {
        _remainingTime.Value = duration;

        while (_remainingTime.Value > 0)
        {
            // プレイヤ死亡する時。Return.
            if (_currentState.Value == RoundState.GameOver) return;

            // 1秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
            
            // R3でUIに反映
            _remainingTime.Value -= 1f;
        }
    }

    /// <summary>
    /// Game Over Trigger. 
    /// </summary>
    public void TriggerGameOver()
    {
        if (_currentState.Value == RoundState.Victory || _currentState.Value == RoundState.GameOver) return;
        
        _currentState.Value = RoundState.GameOver;
        Time.timeScale = 0f;
        
        Debug.Log("[RoundManager] Game Over!");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; // 時間を元に戻す
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 現在のシーンを再読み込み
    }
}