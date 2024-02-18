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
    /// <summary>
    /// The Head Up Display handler of the game (Player specific)
    /// </summary>
    public class UserHUDHandler : MonoBehaviour
    {
        ///<summary>All ability icons</summary>
        [Header("Set in inspector - User Info Panels\n")]
        [SerializeField, Tooltip("All ability icons")] List<Sprite> allAbilityIcons;
        ///<summary>Outer layer of the ability icons</summary>
        [SerializeField, Tooltip("Outer layer of the ability icons")] Image abilityIconOuter;
        ///<summary>Inner layer of the ability icons</summary>
        [SerializeField, Tooltip("Inner layer of the ability icons")] Image abilityIconInner;
        ///<summary>The ability uses text</summary>
        [SerializeField, Tooltip("The ability uses text")] TextMeshProUGUI abilityUses;

        ///<summary>The remaining health slider</summary>
        [Header("Player info")]
        [SerializeField, Tooltip("The remaining health slider")] Slider remainingHealth;

        ///<summary>The weapon name text</summary>
        [Header("Weapon info")]
        [SerializeField, Tooltip("The weapon name text")] TextMeshProUGUI weaponName;
        ///<summary>The remaining bullets slider</summary>
        [SerializeField, Tooltip("The remaining bullets slider")] Slider remainingBullets;

        ///<summary>The score HUD panel</summary>
        [Header("Score UI")]
        [SerializeField, Tooltip("The score HUD panel")] GameObject scorePanel;

        ///<summary>The updates HUD panel</summary>
        [Header("Updates UI")]
        [SerializeField, Tooltip("The updates HUD panel")] GameObject updatesPanel;

        ///<summary>The scoreboard HUD panel</summary>
        [Header("Scoreboard UI")]
        [SerializeField, Tooltip("The scoreboard HUD panel")] GameObject scoreboardPanel;

        ///<summary>The message HUD panel</summary>
        [Header("Message UI")]
        [SerializeField, Tooltip("The message HUD panel")] GameObject messagePanel;

        ///<summary>The pause HUD panel</summary>
        [Header("Pause UI")]
        [SerializeField, Tooltip("The pause HUD panel")] GameObject pausePanel;

        ///<summary>The active global volume</summary>
        Volume postProcessVolume;
        ///<summary>The bloom post process effect</summary>
        Bloom bloom;
        ///<summary>Initial bloom intensity</summary>
        float initialIntensity;
        ///<summary>Is an other panel open right now?</summary>
        bool otherPanelOpen = false;

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
        {
            SetIsOtherPanelOpen(true);
            scorePanel.SetActive(true);
            ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor);
        }

        ///<summary>Opens the update UI</summary>
        public void OpenUpdatesUI()
        {
            SetIsOtherPanelOpen(true);
            updatesPanel.SetActive(true);
            ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor);
        }

        ///<summary>Opens the scoreboard UI</summary>
        public void OpenScoreboardUI()
        {
            SetIsOtherPanelOpen(true);
            scoreboardPanel.SetActive(true);
            ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor);
        }

        ///<summary>Opens the scoreboard UI</summary>
        public void OpenMessageUI(string msg)
        {
            SetIsOtherPanelOpen(true);
            messagePanel.GetComponent<EndMessageController>().SetMessageText(msg);
            messagePanel.SetActive(true);
            ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor);
        }

        ///<summary>Closes the score UI</summary>
        public void CloseScoreUI()
        {
            SetIsOtherPanelOpen(false);
            scorePanel.SetActive(false);
            StartCoroutine(EnableCrosshair());
        }

        ///<summary>Closes the scoreboard UI</summary>
        public void CloseScoreboardUI()
        {
            SetIsOtherPanelOpen(false);
            scoreboardPanel.SetActive(false);
            StartCoroutine(EnableCrosshair());
        }

        ///<summary>Closes the message UI</summary>
        public void CloseMessageUI()
        {
            SetIsOtherPanelOpen(false);
            messagePanel.SetActive(false);
            StartCoroutine(EnableCrosshair());
        }

        ///<summary>Enables the crosshair cursor</summary>
        IEnumerator EnableCrosshair()
        {
            yield return new WaitForSecondsRealtime(1f);
            ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Crosshair);
        }

        ///<summary>Increases then rapidlye decreases the bloom post proccess effect</summary>
        public void BloomOnKill()
        { StartCoroutine(ChangeBloomIntensity()); }

        ///<summary>Increases then rapidlye decreases the bloom post proccess effect</summary>
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

        /// <summary>
        /// If set to true, the pause menu wont be able to toggle on, otherwise the pause panel can be enabled.
        /// </summary>
        public void SetIsOtherPanelOpen(bool value)
        { otherPanelOpen = value; }

        void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            { TogglePauseMenu(); }
        }

        ///<summary>Toggles the state of the pause menu</summary>
        public void TogglePauseMenu()
        {
            if (otherPanelOpen) { return; }

            pausePanel.SetActive(!pausePanel.activeSelf);

            if (!pausePanel.activeInHierarchy)
            {
                ManagerHub.S.GameSoundsHandler.ForceOSTVolume(ManagerHub.S.SettingsHandler.UserSettings.ostVolume);
                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Unpause);
                ManagerHub.S.SetGameState(GameState.Running);
                ManagerHub.S.InternalTime.ChangeTimeScale(1f);
                ManagerHub.S.InternalTime.StartRoomTimer();
            }
            else
            {
                ManagerHub.S.GameSoundsHandler.ForceOSTVolume(0f);
                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Pause);
                ManagerHub.S.SetGameState(GameState.Paused);
                ManagerHub.S.InternalTime.ChangeTimeScale(0.1f);
                ManagerHub.S.InternalTime.StopRoomTimer();
            }
        }

        ///<summary>*Public for Button usage* 
        ///Loads the main menu scene</summary>
        public void LoadMenu()
        {
            ManagerHub.S.SetGameState(GameState.Running);
            ManagerHub.S.InternalTime.ChangeTimeScale(1f);
            ManagerHub.S.GameSoundsHandler.ForceOSTVolume(ManagerHub.S.SettingsHandler.UserSettings.ostVolume);
            ManagerHub.S.RewardSelector.ResetRewards();
            ManagerHub.S.ScoreHandler.ResetTotalScore();
            ManagerHub.S.ScoreHandler.ResetRoomScore();
            ManagerHub.S.InternalTime.ResetRoomTimer();
            ManagerHub.S.AbilityManager.ResetAbilities();
            ManagerHub.S.AIHandler.ResetHandler();
            ManagerHub.S.StageHandler.LoadFromBoot();
        }
    }
}