#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;
using WGRF.BattleSystem;
using WGRF.Core;
using WGRF.Gameplay.BattleSystem;

public class DummyRewardsTester : MonoBehaviour
{   
    public bool updateWeapon;

    // Update is called once per frame
    void Update()
    {
        if (updateWeapon)
        {
            Weapon temp = ManagerHub.S.RewardSelector.GetWeaponUpdate();
            ManagerHub.S.PlayerController.Access<PlayerAttack>("pAttack").SetWeaponInfo(temp);
            ManagerHub.S.PlayerController.Access<PlayerAttack>("pAttack").SetCurrentRoomWeapon(temp);
            updateWeapon = false;
        }

        if (Keyboard.current.rKey.isPressed)
        {ManagerHub.S.RewardSelector.ResetRewards();}
    }
}
#endif