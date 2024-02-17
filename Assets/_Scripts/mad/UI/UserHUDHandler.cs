using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

using WGRF.Abilities;
using WGRF.BattleSystem;
using WGRF.Core;
using WGRF.Player;

namespace WGRF.UI
{
    public class UserHUDHandler : MonoBehaviour
    {
        [Header("Set in inspector - User Info Panels\n")]
        [SerializeField] List<Sprite> allAbilityIcons;
        [SerializeField] Image abilityIconOuter;
        [SerializeField] Image abilityIconInner;
        [SerializeField] TextMeshProUGUI abilityUses;

        [Header("Player info")]
        [SerializeField] Slider remainingHealth;

        [Header("Weapon info")]
        [SerializeField] TextMeshProUGUI weaponName;
        [SerializeField] Slider remainingBullets;

        [Header("Score UI")]
        [SerializeField] GameObject scorePanel;

        [Header("Updates UI")]
        [SerializeField] GameObject updatesPanel;

        [Header("Scoreboard UI")]
        [SerializeField] GameObject scoreboardPanel;

        [Header("Message UI")]
        [SerializeField] GameObject messagePanel;

        [Header("Pause UI")]
        [SerializeField] GameObject pausePanel;

        Volume postProcessVolume;
        Bloom bloom;
        float initialIntensity;

        void Awake()
        {
            ManagerHub.S.AttackHudHandler(this);
            updatesPanel.SetActive(false);
            CloseScoreUI();
        }

        void Start()
        {
            postProcessVolume = ManagerHub.S.PostProcessVolume;
            postProcessVolume.sharedProfile.TryGet<Bloom>(out bloom);
            initialIntensity = bloom.intensity.value;
        }

        /// <summary>
        /// Call to change all the sprites of the selectedAbilityIcons list to the passed abilityIcon sprite.
        /// </summary>
        /// <param name="ability">The selected ability icon.</param>
        public void ChangeSelectedAbilityIcon(Ability ability)
        {
            this.abilityIconInner.sprite = ability.AbilitySprite;
            this.abilityIconOuter.color = ability.IsUnlocked ? Color.white : Color.gray;
        }

        /// <summary>
        /// Sets the ability uses left text
        /// </summary>
        public void SetAbilityUses(int uses)
        { abilityUses.SetText(uses.ToString()); }

        /// <summary>
        /// Sets the player health slider initial values.
        /// </summary>
        public void SetPlayerHealthInfo(PlayerEntity entity)
        {
            remainingHealth.minValue = 0;
            remainingHealth.maxValue = entity.MaxHealth;

            SetPlayerHealth(entity.MaxHealth);
        }

        /// <summary>
        /// Sets the player health slider value 
        /// </summary>
        public void SetPlayerHealth(int value)
        { remainingHealth.value = value; }

        /// <summary>
        /// Updates the UI weapon information based on the passed weapon
        /// </summary>
        public void SetWeaponSliderInfo(Weapon weapon)
        {
            weaponName.SetText(weapon.WeaponName);

            remainingBullets.minValue = 0;
            remainingBullets.maxValue = weapon.DefaultMagazine;

            ChangeBulletsLeft(weapon.DefaultMagazine);
        }

        /// <summary>
        /// Call to set the remainingBullets text to the passed value.
        /// </summary>
        public void ChangeBulletsLeft(int bulletsLeft)
        { remainingBullets.value = bulletsLeft; }

        ///<summary>Opens the score UI</summary>
        public void OpenScoreUI()
        { scorePanel.SetActive(true); }

        ///<summary>Closes the score UI</summary>
        public void CloseScoreUI()
        { scorePanel.SetActive(false); }

        ///<summary>Opens the update UI</summary>
        public void OpenUpdatesUI()
        { updatesPanel.SetActive(true); }

        ///<summary>Opens the scoreboard UI</summary>
        public void OpenScoreboardUI()
        { scoreboardPanel.SetActive(true); }

        ///<summary>Opens the scoreboard UI</summary>
        public void OpenMessageUI(string msg)
        {
            messagePanel.GetComponent<EndMessageController>().SetMessageText(msg);
            messagePanel.SetActive(true);
        }

        ///<summary>Closes the scoreboard UI</summary>
        public void CloseScoreboardUI()
        { scoreboardPanel.SetActive(false); }

        ///<summary>Closes the scoreboard UI</summary>
        public void CloseMessageUI()
        { messagePanel.SetActive(false); }

        public void BloomOnKill()
        { StartCoroutine(ChangeBloomIntensity()); }

        IEnumerator ChangeBloomIntensity()
        {
            float elapsedTime = 0f;
            while (elapsedTime < 0.15f)
            {
                elapsedTime += Time.deltaTime;
                bloom.intensity.value = Mathf.Lerp(initialIntensity, initialIntensity * 5f, elapsedTime / 0.25f);
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < 0.15f)
            {
                elapsedTime += Time.deltaTime;
                bloom.intensity.value = Mathf.Lerp(initialIntensity * 5f, initialIntensity, elapsedTime / 0.25f);
                yield return null;
            }

            bloom.intensity.value = initialIntensity;
        }

        void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            { TogglePauseMenu(); }
        }

        ///<summary>Toggles the state of the pause menu</summary>
        public void TogglePauseMenu()
        {
            pausePanel.SetActive(!pausePanel.activeSelf);

            if (!pausePanel.activeInHierarchy)
            {
                ManagerHub.S.GameSoundsHandler.ForceOSTVolume(1f);
                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Unpause);
                ManagerHub.S.SetGameState(GameState.Running);
                ManagerHub.S.InternalTime.ChangeTimeScale(1f);
            }
            else
            {
                ManagerHub.S.GameSoundsHandler.ForceOSTVolume(0f);
                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pause);
                ManagerHub.S.SetGameState(GameState.Paused);
                ManagerHub.S.InternalTime.ChangeTimeScale(0.1f);
            }
        }

        public void LoadMenu()
        {
            ManagerHub.S.SetGameState(GameState.Running);
            ManagerHub.S.InternalTime.ChangeTimeScale(1f);

            ManagerHub.S.GameSoundsHandler.ForceOSTVolume(1f);
            ManagerHub.S.StageHandler.LoadFromBoot();
            ManagerHub.S.RewardSelector.ResetRewards();
            ManagerHub.S.ScoreHandler.ResetTotalScore();
            ManagerHub.S.ScoreHandler.ResetRoomScore();
            ManagerHub.S.InternalTime.ResetRoomTimer();
            ManagerHub.S.AbilityManager.ResetAbilities();
        }
    }
}