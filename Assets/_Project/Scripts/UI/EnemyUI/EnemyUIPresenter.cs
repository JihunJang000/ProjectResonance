using UnityEngine;
using R3;

/// <summary>
/// Enemy関連UIPresenter -> 1個存在。 Model, Viewは役割ごとに存在（多数存在）
/// </summary>
public class EnemyUIPresenter : MonoBehaviour
{
    [Header("Target Model")]
    [SerializeField] private EnemyController _enemyController; // Prototype以後Model + Controllerに分離

    [Header("Target Views")]
    [SerializeField] private DamageTextView _damageTextView;
    [SerializeField] private EnemyHpView _enemyHpView;
    
    private void Awake()
    {
        if (_enemyController == null)
        {
            if (_enemyController == null) _enemyController = GetComponent<EnemyController>();
            if (_damageTextView == null) _damageTextView = GetComponent<DamageTextView>();
            if (_enemyHpView == null) _enemyHpView = GetComponent<EnemyHpView>();
        }
    }

    private void Start()
    {
        if (_enemyController != null)
        {
            // Subscribe -> 購読
            // RegisterTo -> 破壊される時、購読解除。
            _enemyController.OnDamaged
                .Subscribe(data => _damageTextView.Spawn(data.damage, data.position))　
                .RegisterTo(destroyCancellationToken); 
            
            _enemyController.OnHpRatioChanged
                .Subscribe(ratio => _enemyHpView.UpdateHpBar(ratio))
                .RegisterTo(destroyCancellationToken);
        }
        
        
    }
    
}