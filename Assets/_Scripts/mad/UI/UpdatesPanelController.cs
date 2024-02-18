using UnityEngine;
using UnityEngine.UI;
using WGRF.BattleSystem;
using WGRF.Core;
using WGRF.Player;

namespace WGRF.UI
{
    /// <summary>
    /// Responsible for displaying the updates in the UI panel
    /// </summary>
    public class UpdatesPanelController : MonoBehaviour
    {
        ///<summary>The weapon update sprite</summary>
        [SerializeField, Tooltip("The weapon update sprite")] Image weaponSprite;
        ///<summary>The ability update sprite</summary>
        [SerializeField, Tooltip("The ability update sprite")] Image abilitySprite;

        ///<summary>Next upgradeable ability</summary>
        int nextAbilityIdx = -1;

        void OnEnable()
        {
            ManagerHub.S.InternalTime.ChangeTimeScale(0f);

            weaponSprite.sprite = ManagerHub.S.RewardSelector.GetNextWeaponSprite();

            nextAbilityIdx++;
            nextAbilityIdx = nextAbilityIdx % 3;
            abilitySprite.sprite = ManagerHub.S.AbilityManager.NextAbilitySprite(nextAbilityIdx);
        }

        void OnDisable()
        {
            ManagerHub.S.InternalTime.ChangeTimeScale(1f);
            ManagerHub.S.HUDHandler.SetIsOtherPanelOpen(false);
        }

        ///<summary>Call to start the weapon update sequence</summary>
        public void UpdateWeapon()
        {
            Weapon temp = ManagerHub.S.RewardSelector.GetWeaponUpdate();
            if (!temp) return;

            ManagerHub.S.PlayerController.Access<PlayerAttack>("pAttack").SetWeaponInfo(temp);
            gameObject.SetActive(false);
        }

        ///<summary>Call to start the ability update sequence</summary>
        public void UpdateAbility()
        {
            ManagerHub.S.AbilityManager.UpdateAbilityTier((AbilityType)nextAbilityIdx);
            gameObject.SetActive(false);
        }

        ///<summary>Call to start the player stat update sequence</summary>
        public void UpdatePlayerStat()
        {
            int rnd = Random.Range(0, 20);

            if (rnd >= 0 && rnd <= 10)
            { ManagerHub.S.RewardSelector.PlayerHealthUpdateReward(); }
            else
            { ManagerHub.S.RewardSelector.AbilityUsesReward(); }

            gameObject.SetActive(false);
        }

        ///<summary>BADASS SUMMARY</summary>
        public void BadassUpdate()
        {
            ManagerHub.S.ScoreHandler.IncreaseTotalScoreBy(100);
            gameObject.SetActive(false);
        }
    }
}