using UnityEngine;
using UnityEngine.UI;
using WGRF.BattleSystem;
using WGRF.Core;
using WGRF.Player;

namespace WGRF.UI
{
    public class UpdatesPanelController : MonoBehaviour
    {
        [SerializeField] Image weaponSprite;
        [SerializeField] Image abilitySprite;

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
        { ManagerHub.S.InternalTime.ChangeTimeScale(1f); }

        public void UpdateWeapon()
        {
            Weapon temp = ManagerHub.S.RewardSelector.GetWeaponUpdate();
            if (!temp) return;

            ManagerHub.S.PlayerController.Access<PlayerAttack>("pAttack").SetWeaponInfo(temp);
            gameObject.SetActive(false);
        }

        public void UpdateAbility()
        {
            ManagerHub.S.AbilityManager.UpdateAbilityTier((AbilityType)nextAbilityIdx);
            gameObject.SetActive(false);
        }

        public void UpdatePlayerStat()
        {
            int rnd = Random.Range(0, 20);

            if (rnd >= 0 && rnd <= 10)
            { ManagerHub.S.RewardSelector.PlayerHealthUpdateReward(); }
            else
            { ManagerHub.S.RewardSelector.AbilityUsesReward(); }

            gameObject.SetActive(false);
        }

        public void BadassUpdate()
        {
            ManagerHub.S.ScoreHandler.IncreaseTotalScoreBy(100);
            gameObject.SetActive(false);
        }
    }
}