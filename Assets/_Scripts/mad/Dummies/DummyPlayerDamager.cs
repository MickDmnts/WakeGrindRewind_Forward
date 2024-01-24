#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;
using WGRF.Entities.Player;

public class DummyPlayerDamager : MonoBehaviour
{
    public PlayerEntity playerEntity;
    
    void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            playerEntity.AttackInteraction();
        }
    }
}
#endif