using UnityEngine;

/// <summary>
/// Damage Text View
/// </summary>
public class DamageTextView : MonoBehaviour
{
    [Header("UI Prefabs")]
    [SerializeField] private GameObject _floatingTextPrefab;

    public void Spawn(float damage, Vector3 spawnPosition)
    {
        if (_floatingTextPrefab == null) return;
        
        GameObject textObj = Instantiate(_floatingTextPrefab, spawnPosition, Quaternion.identity);
        
        if (textObj.TryGetComponent(out FloatingText floatingText))
        {
            floatingText.Setup(Mathf.RoundToInt(damage));
        }
    }
}