using UnityEngine;
using TMPro; 
using R3;    
using VContainer;

public class InGameTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    
    private RoundManager _roundManager;

    // CompositeDisposable -> 購読解除する為に購読を集めるList
    private readonly CompositeDisposable _disposables = new(); 
    
    // VContainerでSingleton RoundManager注入
    [Inject]
    public void Construct(RoundManager roundManager)
    {
        _roundManager = roundManager;
    }

    private void Start()
    {
        // R3購読
        _roundManager.RemainingTime
            .Subscribe(time => 
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                _timerText.text = $"{minutes:00}:{seconds:00}"; // UI テキスト更新
            })
            .AddTo(_disposables); //　AddTo() -> 購読をComposteDisposable・Listに追加
        
        // ラウンドState購読
        _roundManager.CurrentState
            .Subscribe(state => 
            {
                // 変わる度にテキスト変更
                if (state == RoundState.Ready) 
                    _timerText.text = "Ready";
                else if (state == RoundState.Victory) 
                    _timerText.text = "Clear!";
                else if (state == RoundState.GameOver) 
                    _timerText.text = "Game Over!";
            })
            .AddTo(_disposables);
    }
}