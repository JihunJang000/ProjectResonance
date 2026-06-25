using UnityEngine;
using UnityEngine.UI; 

public class EnemyHpView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider _hpSlider;
    
    public void UpdateHpBar(float ratio)
    {
        if (_hpSlider != null)
        {
            _hpSlider.value = ratio;
        }
    }
}