using System.Collections.Generic;
using UnityEngine;
using WGR.Abilities;

namespace WGR.Core.Managers
{
    /// <summary>
    /// All the available abilities the player has.
    /// </summary>
    public enum AbilityType
    {
        Slowtime,
        RewindTime,
        StopTime,
    }

    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * Inspector variables: This variable must be set from the inspector.
     * Dynamically changed: Every class variable is changed throughout the game.
     * 
     * [Must Know]
     * 1. The ability manager creates all the player abilities on game startup.
     * 2. Each AbilityTier and IsUnlocked are set from the JSON save file IF there is any on GAME STARTUP.
     */
    [DefaultExecutionOrder(100)]
    public class AbilityManager : MonoBehaviour
    {
        [Header("Set ability Icons")]
        [SerializeField] List<Sprite> abilityIcons;

        List<Ability> Abilities;

        /// <summary>
        /// When set to true abilities are shown and enabled, if false then abilities are neither drawn or can be used.
        /// </summary>
        public bool AbilitiesCanActivate { get; set; }

        public bool AbilitiesCanCycle { get; private set; }

        /// <summary>
        /// Active ability index can be used from the UI to visualize the currently selected ability.
        /// </summary>
        public int ActiveAbilityIndex { get; private set; }
        Ability activeAbility;

        //Used as ability re-enabling preventor while another or the same ability is used.
        bool activatedAnAbility;

        private void Awake()
        {
            //we can load the jSon here to get each ability tier + isActive for each ability
            CreateAbilities(ref Abilities);

            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSaveOverwrite += OnNewGameSave;
                GameManager.S.GameEventHandler.onSceneChanged += SetAbilitiesCanActivate;
            }
        }

        void OnNewGameSave()
        {
            foreach (Ability ability in Abilities)
            {
                ability.AbilityInfoFullReset();

                if (ability.Compare(ability, Abilities[0]))
                {
                    ability.IsUnlocked = true;
                    ability.AbilityTier = 1;
                }

                ability.UpdateStatsPerTier();
            }
        }

        /// <summary>
        /// Call to create all the player abilities and set their state based on the loaded JSON info.
        /// </summary>
        /// <param name="abilityList">The ability list to modify</param>
        void CreateAbilities(ref List<Ability> abilityList)
        {
            if (GameManager.S.SaveDataHandler.HasSavedBefore())
            {
                //Cache each ability tier separately
                int slowTier, rewTier, stopTier;
                slowTier = GameManager.S.SaveDataHandler.GetSaveFileInfo().AbilityTiersInOrder[0];
                rewTier = GameManager.S.SaveDataHandler.GetSaveFileInfo().AbilityTiersInOrder[1];
                stopTier = GameManager.S.SaveDataHandler.GetSaveFileInfo().AbilityTiersInOrder[2];

                //Determine if the abilities are locked based on their tier rank
                bool rewUnlocked = rewTier > 0 ? true : false;
                bool stopUnlocked = stopTier > 0 ? true : false;

                //Create the Rewinder for the rewind ability
                Rewinder abilityRewind = gameObject.AddComponent<Rewinder>();

                //Create the LOADED abilities
                abilityList = new List<Ability>()
                {
                    new SlowDownTime("Slow Down Time", slowTier, abilityIcons[0], true),
                    new RewindTime("Rewind Time", rewTier, abilityIcons[1], rewUnlocked, abilityRewind),
                    new StopTime("Stop Time", stopTier, abilityIcons[2], stopUnlocked),
                };
            }
            else
            {
                //Create the Rewinder for the rewind ability
                Rewinder abilityRewind = gameObject.AddComponent<Rewinder>();

                //Create the fresh abilities
                abilityList = new List<Ability>()
                {
                    new SlowDownTime("Slow Down Time",  1,abilityIcons[0], true),
                    new RewindTime("Rewind Time",  0,abilityIcons[1], false, abilityRewind),
                    new StopTime("Stop Time", 0,abilityIcons[2], false),
                };
            }
        }

        /// <summary>
        /// Activates OR deactivates the ability use based on the current scene.
        /// <para>Abilities are always deactivated in the Player Hub scene.</para>
        /// </summary>
        void SetAbilitiesCanActivate(GameScenes activeScene)
        {
            EnableAbilities();

            if (activeScene == GameScenes.PlayerHub || activeScene == GameScenes.NewGameIntro)
            {
                AbilitiesCanActivate = false;
                AbilitiesCanCycle = false;
                return;
            }
            else
            {
                AbilitiesCanActivate = true;
                AbilitiesCanCycle = true;
            }

            //So abilities can activate on scene entry
            activatedAnAbility = false;
        }

        /// <summary>
        /// Call to cycle through all the abilities and reset their ability uses.
        /// </summary>
        public void EnableAbilities()
        {
            foreach (Ability ability in Abilities)
            {
                ability.ResetAbilityUses();
                CycleActiveAbility();
            }
        }

        private void Start()
        {
            StartAbilities();
        }

        /// <summary>
        /// Call to invoke every Start() method of each ability present in the Abilities list.
        /// </summary>
        void StartAbilities()
        {
            foreach (Ability ability in Abilities)
            {
                ability.Start(EnableAbilitySelection);
            }
        }

        /// <summary>
        /// Gets called when the ability behaviour of the selected ability has finished.
        /// <para>Called through an Action callback.</para>
        /// </summary>
        void EnableAbilitySelection()
        {
            activatedAnAbility = false;
        }

        private void Update()
        {
            //Prevents the player from using his abilities in MainMenu + PlayerHub scenes.
            if (!AbilitiesCanActivate || GameManager.S.UIManager.IsPaused) return;

            if (AbilitiesCanCycle)
            {
                if (Input.GetKeyDown(KeyCode.Tab) && !activatedAnAbility)
                {
                    CycleActiveAbility();
                }
            }

            if (Input.GetKeyDown(KeyCode.R) && !activatedAnAbility)
            {
                activatedAnAbility = activeAbility.TryActivate();
            }
        }

        /// <summary>
        /// Call to increment the ActiveAbilityIndex by 1.
        /// <para>ActiveAbilityIndex is set to 0 when it goes past the Abilities.Count.</para>
        /// <para>Also sets the activeAbility to the Abilities[ActiveAbilityIndex]</para>
        /// </summary>
        void CycleActiveAbility()
        {
            ActiveAbilityIndex++;

            if (ActiveAbilityIndex >= Abilities.Count)
            {
                ActiveAbilityIndex = 0;
            }

            activeAbility = Abilities[ActiveAbilityIndex];

            //UI UPDATES
            if (GameManager.S != null)
            {
                GameManager.S.HUDHandler.ChangeSelectedAbilityIcon(activeAbility.AbilitySprite, ActiveAbilityIndex, activeAbility.IsUnlocked);
                GameManager.S.HUDHandler.UpdateRemainingTimeIcon(0, activeAbility.ActiveTime);
                GameManager.S.HUDHandler.UpdateRemainingUsesIcon(activeAbility.UsesPerLevel, activeAbility.GetCachedUses());

                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSSlideIn);
            }
        }

        private void FixedUpdate()
        {
            if (GameManager.S.UIManager.IsPaused) return;

            //If the player activated any ability...
            if (activatedAnAbility)
            {
                //... call the UpdateAbilityTick(...) of the SELECTED ability
                activeAbility.UpdateAbilityTick();
            }
        }

        /// <summary>
        /// This method is added as a listener to the Ability update button in the PlayerHub.
        /// </summary>
        /// <param name="type">The ability type you want to update.</param>
        public void UpdateAbilityTier(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Slowtime:
                    Abilities[0].UpgradeAbility();
                    break;
                case AbilityType.RewindTime:
                    Abilities[1].UpgradeAbility();
                    break;
                case AbilityType.StopTime:
                    Abilities[2].UpgradeAbility();
                    break;
            }

            if (GameManager.S != null)
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.VHSOpen);
        }

        /// <summary>
        /// This method is used from the UI to display the skill points needed for each 
        /// tier update.
        /// </summary>
        /// <param name="type">The ability you want to display.</param>
        /// <returns>The points for tier update.</returns>
        public int GetPointsNeeded(AbilityType type)
        {
            int points = 0;

            switch (type)
            {
                case AbilityType.Slowtime:
                    points = Abilities[0].GetPointsForUpgrade();
                    break;
                case AbilityType.RewindTime:
                    points = Abilities[1].GetPointsForUpgrade();
                    break;
                case AbilityType.StopTime:
                    points = Abilities[2].GetPointsForUpgrade();
                    break;
            }

            return points;
        }

        /// <summary>
        /// This method is used from the HandleAbilityPanel script to update the PlayerHub
        /// ability panel texts.
        /// </summary>
        /// <param name="type">The ability you want to display.</param>
        /// <returns>The passed ability tier number.</returns>
        public int GetAbilityCurrentTier(AbilityType type)
        {
            int tempTier = 0;

            switch (type)
            {
                case AbilityType.Slowtime:
                    tempTier = Abilities[0].AbilityTier;
                    break;
                case AbilityType.RewindTime:
                    tempTier = Abilities[1].AbilityTier;
                    break;
                case AbilityType.StopTime:
                    tempTier = Abilities[2].AbilityTier;
                    break;
            }

            return tempTier;
        }

        public string GetAbilityDescription(AbilityType type)
        {
            string tempStr = "";

            switch (type)
            {
                case AbilityType.Slowtime:
                    tempStr = Abilities[0].AbilityDescription;
                    break;
                case AbilityType.RewindTime:
                    tempStr = Abilities[1].AbilityDescription;
                    break;
                case AbilityType.StopTime:
                    tempStr = Abilities[2].AbilityDescription;
                    break;
                default:
                    break;
            }

            return tempStr;
        }

        /// <summary>
        /// Call to get the current selected ability index.
        /// </summary>
        public int GetCurrentSelectedAbility()
        {
            return ActiveAbilityIndex;
        }

        /// <summary>
        /// Call to check if the rewind ability UsesPerLevel is greater than 0.
        /// <para>If yes, then return true, false otherwise.</para>
        /// </summary>
        /// <param name="subtractOneUse">If set to true and the above evaluation results to true too, then 
        /// subtract one use from the rewind ability UsesPerLevel value.</param>
        public bool CanRewind(bool subtractOneUse)
        {
            if (Abilities[1].UsesPerLevel > 0)
            {
                if (subtractOneUse)
                {
                    Abilities[1].UsesPerLevel--;
                }

                return true;
            }

            return false;
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSaveOverwrite -= OnNewGameSave;
                GameManager.S.GameEventHandler.onSceneChanged -= SetAbilitiesCanActivate;
            }
        }
    }
}