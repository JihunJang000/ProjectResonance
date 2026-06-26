using UnityEngine;
using TMPro; 
using R3;
using VContainer;
using System;

/// <summary>
/// インゲームのHUD（ラウンド、タイマー）を制御するクラス
/// </summary>
public class InGameHUDPresenter : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _roundText; // ラウンド表示テキスト
    [SerializeField] private TextMeshProUGUI _timerText; // タイマー表示テキスト

    private RoundManager _roundManager;
    private DisposableBag _disposableBag;

    [Inject]
    public void Construct(RoundManager roundManager)
    {
        _roundManager = roundManager;

        // ラウンド数が変わったらテキストを更新
        _roundManager.CurrentRoundNum
            .Subscribe(round => 
            {
                if (round > 0) _roundText.text = $"Round {round}";
            })
            .AddTo(ref _disposableBag);

        // 残り時間が変わったら 00:00 形式でテキストを更新
        _roundManager.RemainingTime
            .Subscribe(time =>
            {
                // float -> mm:ss 形式で表示
                TimeSpan ts = TimeSpan.FromSeconds(time);
                _timerText.text = ts.ToString(@"mm\:ss"); 
            })
            .AddTo(ref _disposableBag);
    }

    private void OnDestroy()
    {
        _disposableBag.Dispose();
    }
}