using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

/// <summary>
/// VContainerを利用してキャラクターをSpawnし、Tagシステムを管理。
/// </summary>
public class CharacterManager : IInitializable, IDisposable
{
    // IInitailizable -> 継承してInitialize関数を純粋C#ClassでStart()関数のように使用可能
    // ITickable -> Tick()使用可能。Tick()わUnityのUpdate()と同じ機能
    // IDisposable -> 購読解除用
    private IObjectResolver _resolver;　// VContainerからの権限
    private List<PlayerController> _prefabs;
    private InputManager _inputManager;
    // 実際生成されたキャラクターList
    private List<PlayerController> _spawnedCharacters = new List<PlayerController>();
    private CinemachineCamera _virtualCamera;
    private int _currentIndex = 0;
    
    // GameLifetimeScopeのWithParameterから自動的に注入
    public CharacterManager(List<PlayerController> prefabs, IObjectResolver resolver, InputManager inputManager, CinemachineCamera virtualCamera)
    {
        _prefabs = prefabs;
        _resolver = resolver;
        _inputManager = inputManager;
        _virtualCamera =  virtualCamera;
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
            UpdateCameraTarget(_spawnedCharacters[0].transform); // カメラ設定
            Debug.Log("[CharacterManager.Initialize] 初期基本キャラクターSpawn: Index = 0");
        }
        
        _inputManager.OnSwapCharacter += SwapCharacter;
    }
    
    public void Dispose()
    {
        // 購読解除
        if (_inputManager != null)
        {
            _inputManager.OnSwapCharacter -= SwapCharacter;
        }
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

        // カメラ設定
        UpdateCameraTarget(_spawnedCharacters[_currentIndex].transform);
        
        Debug.Log($"[CharacterManager.SwapCharacter] キャラクター召喚. Index: {index}");
    }

    private void UpdateCameraTarget(Transform target)
    {
        _virtualCamera.Follow = target;
        
    }
}