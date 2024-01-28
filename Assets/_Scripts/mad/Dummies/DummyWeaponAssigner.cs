#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;
using WGRF.BattleSystem;
using WGRF.Core;
using WGRF.Player;

public class DummyWeaponAssigner : MonoBehaviour
{
    public Weapon weapon;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.isPressed)
        { ManagerHub.S.PlayerController.Access<PlayerAttack>("pAttack").SetWeaponInfo(weapon); }
    }
}
#endif