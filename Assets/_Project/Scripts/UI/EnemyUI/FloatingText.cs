using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMesh; // Text多数存在する可能性がある為Inspectorで登録。
    [SerializeField] private float _moveSpeed = 1.5f;    // 上に上がるスピード
    [SerializeField] private float _duration = 0.5f;     

    public void Setup(int damage)
    {
        _textMesh.text = damage.ToString();
        AnimateAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid AnimateAsync(System.Threading.CancellationToken token)
    {
        float elapsed = 0f;
        Color originalColor = _textMesh.color;
        
        // 左右ランダムOffset
        Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0, 0);
        transform.position += randomOffset;

        while (elapsed < _duration)
        {
            if (token.IsCancellationRequested) return;

            elapsed += Time.deltaTime;
            
            // 上に移動
            transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);

            // Fade Out
            float alpha = Mathf.Lerp(1f, 0f, elapsed / _duration);
            _textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        
        Destroy(gameObject);
    }
}