

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

/// <summary>
/// VContainerを利用してキャラクターをSpawnし、Tagシステムを管理。
/// </summary>
public class CharacterManager : IInitializable, ITickable
{
    // IInitailizable -> 継承してInitialize関数を純粋C#ClassでStart()関数のように使用可能
    // ITickable -> Tick()使用可能。Tick()わUnityのUpdate()と同じ機能

    private readonly IObjectResolver _resolver;　// VContainerからの権限
    private readonly List<PlayerController> _prefabs;
    
    // 実際生成されたキャラクターList
    private List<PlayerController> _spawnedCharacters = new List<PlayerController>();
    private int _currentIndex = 0;

    // GameLifetimeScopeのWithParameterで自動的に注入
    public CharacterManager(List<PlayerController> prefabs, IObjectResolver resolver)
    {
        _prefabs = prefabs;
        _resolver = resolver;
    }
    
    // = Start()
    public void Initialize()
    {
        // キャラクター生成
        foreach (var prefab in _prefabs)
        {
            // VContainer専用Instantiate(キャラクター内のScript内にいる[Inject]に注入可能。）
            var instance = _resolver.Instantiate(prefab, Vector2.zero, Quaternion.identity);
            instance.gameObject.SetActive(false);
            _spawnedCharacters.Add(instance);
        }
        
        if (_spawnedCharacters.Count > 0)
        {
            _spawnedCharacters[0].gameObject.SetActive(true);
            Debug.Log("[CharacterManager.Initialize] 初期基本キャラクターSpawn: Index = 0");
        }
    }

    // = Update()
    public void Tick()
    {
        // テスト
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwapCharacter(0); 
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwapCharacter(1); 
    }

    private void SwapCharacter(int index)
    {
        if (index < 0 || index >= _spawnedCharacters.Count || index == _currentIndex) return;

        // 出ているキャラクターの位置貯蔵。
        Vector2 currentPos = _spawnedCharacters[_currentIndex].transform.position;
        
        // 現在のキャラーOff.
        _spawnedCharacters[_currentIndex].gameObject.SetActive(false);

        // 該当するキャラを召喚。
        _currentIndex = index;
        _spawnedCharacters[_currentIndex].transform.position = currentPos;
        _spawnedCharacters[_currentIndex].gameObject.SetActive(true);

        Debug.Log($"[CharacterManager.SwapCharacter] キャラクター召喚. Index: {index}");
    }
}