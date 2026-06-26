using System;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// RoundManager(Model)とGameResultView(View)を仲介するPresenter
/// </summary>
public class GameResultPresenter : MonoBehaviour
{
    [Header("Target View")]
    [SerializeField] private GameResultView _view;
    private RoundManager _roundManager; //SingletonはInject
    
    private DisposableBag _disposableBag; // R3購読解除用

    [Inject]
    public void Construct(RoundManager roundManager)
    {
        _roundManager = roundManager;
    }
    
    private void Start()
    {
        _view.SetupUI();

        // 状態監視
        _roundManager.CurrentState
            .Subscribe(state =>
            {
                if (state == RoundState.GameOver) _view.ShowGameOver();
                else if (state == RoundState.Victory) _view.ShowVictory();
            })
            .AddTo(ref _disposableBag);

        // ボタンイベント
        _view.RestartButtonGameOver.onClick.AddListener(() => _roundManager.RestartGame());
        _view.RestartButtonVictory.onClick.AddListener(() => _roundManager.RestartGame());
    }

    private void OnDestroy()
    {
        _disposableBag.Dispose();
        _view.RestartButtonGameOver.onClick.RemoveAllListeners();
        _view.RestartButtonVictory.onClick.RemoveAllListeners();
    }
}