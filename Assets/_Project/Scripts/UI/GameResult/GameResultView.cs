using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リザルト画面のUI要素を保持・制御するViewクラス
/// </summary>
public class GameResultView : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _victoryPanel;

    [Header("Buttons")]
    [SerializeField] private Button _restartButtonGameOver;
    [SerializeField] private Button _restartButtonVictory;
    
    public Button RestartButtonGameOver => _restartButtonGameOver;
    public Button RestartButtonVictory => _restartButtonVictory;

    /// <summary>
    /// 初期状態のUIセットアップ
    /// </summary>
    public void SetupUI()
    {
        _gameOverPanel.SetActive(false);
        _victoryPanel.SetActive(false);
    }

    public void ShowGameOver() => _gameOverPanel.SetActive(true);
    public void ShowVictory() => _victoryPanel.SetActive(true);
}