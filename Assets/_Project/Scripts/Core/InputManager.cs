using System;
using UnityEngine.InputSystem;
using VContainer.Unity;

/// <summary>
/// 全ての入力を受けて入力Eventを起こすClass
/// </summary>
public class InputManager : ITickable
{
    public event Action<int> OnSwapCharacter;

    public void Tick()
    {
        if (Keyboard.current == null) return;
        // 臨時テストコード
        if (Keyboard.current.digit1Key.wasPressedThisFrame) OnSwapCharacter?.Invoke(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) OnSwapCharacter?.Invoke(1);
        
    }
}