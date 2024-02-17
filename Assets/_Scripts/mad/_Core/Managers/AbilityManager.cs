using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using WGRF.Abilities;

namespace WGRF.Core
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

    [DefaultExecutionOrder(100)]
    public class AbilityManager : CoreBehaviour
    {
        ///<summary>The ability icons in order of the AbilityTypes enumeration</summary>
        [Header("Set ability Icons")]
        [SerializeField, Tooltip("The ability icons in order of the AbilityTypes enumeration")]
        List<Sprite> abilityIcons;

        ///<summary>The player abilities</summary>
        List<Ability> abilities;
        ///<summary>The currently active ability</summary>
        Ability activeAbility;
        ///<summary>The current ability uses left</summary>
        int abilitiesPerRoom = 2;
        ///<summary>The total ability uses</summary>
        int totalAbilitiesPerRoom = -1;
        ///<summary>True if the player activated an ability in this frame</summary>
        bool activatedAnAbility;
        ///<summary>Active ability index can be used from the UI to visualize the currently selected ability.</summary>
        int activeAbilityIndex = -1;

        ///<summary>When set to true abilities are shown and enabled, if false then abilities are neither drawn or can be used.</summary>
        public bool AbilitiesCanActivate { get; set; }
        ///<summary>If true the player can cycle through his abilities</summary>
        public bool AbilitiesCanCycle { get; private set; }
        ///<summary>The current ability uses left.</summary>
        public int AbilitiesPerRoom => abilitiesPerRoom;
        ///<summary>The total ability uses</summary>
        public int TotalAbilitiesPerRoom => totalAbilitiesPerRoom;

        protected override void PreAwake()
        {
            CreateAbilities(ref abilities);
            totalAbilitiesPerRoom = abilitiesPerRoom;

            AbilitiesCanActivate = true;
            AbilitiesCanCycle = true;
        }

        /// <summary>
        /// Call to create all the player abilities and set their state based on the loaded JSON info.
        /// </summary>
        /// <param name="abilityList">The ability list to modify</param>
        void CreateAbilities(ref List<Ability> abilityList)
        {
            //Create the Rewinder for the rewind ability
            Rewinder abilityRewind = gameObject.AddComponent<Rewinder>();

            //Create the fresh abilities
            abilityList = new List<Ability>()
            {
                new SlowDownTime("Slow Down Time", 1, abilityIcons[0], true),
                new RewindTime("Rewind Time", 0, abilityIcons[1], false, abilityRewind),
                new StopTime("Stop Time", 0, abilityIcons[2], false),
            };
        }

        /// <summary>
        /// Call to cycle through all the abilities and reset their ability uses.
        /// </summary>
        public void EnableAbilities()
        {
            foreach (Ability ability in abilities)
            {
                ability.ResetAbilityUses();
            }
        }

        private void Start()
        {
            StartAbilities();
            activeAbilityIndex = 0;
            activeAbility = abilities[activeAbilityIndex];
        }

        /// <summary>
        /// Call to invoke every Start() method of each ability present in the Abilities list.
        /// </summary>
        void StartAbilities()
        {
            foreach (Ability ability in abilities)
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
            AbilitiesCanCycle = true;
        }

        private void Update()
        {
            //Prevents the player from using his abilities in MainMenu + PlayerHub scenes.
            if (!AbilitiesCanActivate || ManagerHub.S.GameState == GameState.Paused) return;

            if (AbilitiesCanCycle)
            {
                if (Keyboard.current.tabKey.wasPressedThisFrame && !activatedAnAbility)
                {
                    CycleActiveAbility();
                }
            }

            if (Keyboard.current.rKey.wasPressedThisFrame && !activatedAnAbility)
            {
                activatedAnAbility = activeAbility.TryActivate();
                if (activatedAnAbility)
                { AbilitiesCanCycle = false; }
            }
        }

        /// <summary>
        /// Call to increment the ActiveAbilityIndex by 1.
        /// <para>ActiveAbilityIndex is set to 0 when it goes past the Abilities.Count.</para>
        /// <para>Also sets the activeAbility to the Abilities[ActiveAbilityIndex]</para>
        /// </summary>
        void CycleActiveAbility()
        {
            activeAbilityIndex++;

            activeAbilityIndex = activeAbilityIndex % abilities.Count;
            activeAbility = abilities[activeAbilityIndex];

            //UI UPDATES
            ManagerHub.S.HUDHandler.ChangeSelectedAbilityIcon(activeAbility);

            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.VHSSlideIn);
        }

        private void FixedUpdate()
        {
            if (ManagerHub.S.GameState == GameState.Paused) return;

            //If the player activated any ability...
            if (activatedAnAbility)
            {
                //... call the UpdateAbilityTick(...) of the SELECTED ability
                activeAbility.UpdateAbilityTick();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The ability type you want to update.</param>
        public void UpdateAbilityTier(AbilityType type)
        {
            switch (type)
            {
                case AbilityType.Slowtime:
                    abilities[0].UpgradeAbility();
                    break;
                case AbilityType.RewindTime:
                    abilities[1].UpgradeAbility();
                    break;
                case AbilityType.StopTime:
                    abilities[2].UpgradeAbility();
                    break;
            }

            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.VHSOpen);
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
                    tempTier = abilities[0].AbilityTier;
                    break;
                case AbilityType.RewindTime:
                    tempTier = abilities[1].AbilityTier;
                    break;
                case AbilityType.StopTime:
                    tempTier = abilities[2].AbilityTier;
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
                    tempStr = abilities[0].AbilityDescription;
                    break;
                case AbilityType.RewindTime:
                    tempStr = abilities[1].AbilityDescription;
                    break;
                case AbilityType.StopTime:
                    tempStr = abilities[2].AbilityDescription;
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
        { return activeAbilityIndex; }

        ///<summary>Decreases the ability usages per room by 1</summary>
        public void DecreaseAbilityUses()
        {
            abilitiesPerRoom = Math.Max(0, --abilitiesPerRoom);
            ManagerHub.S.HUDHandler.SetAbilityUses(abilitiesPerRoom);
        }

        ///<summary>Increases the ability uses per room by 1</summary>
        public int IncreaseTotalAbilityUsesPerRoom()
        {
            totalAbilitiesPerRoom = abilitiesPerRoom += 1;
            ManagerHub.S.HUDHandler.SetAbilityUses(totalAbilitiesPerRoom);
            return totalAbilitiesPerRoom;
        }
        
        /// <summary>
        /// Returns the next ability sprite by index
        /// </summary>
        /// <param name="idx">The ability index</param>
        public Sprite NextAbilitySprite(int idx) => abilities[Math.Clamp(idx, 0, abilities.Count - 1)].AbilitySprite;
        
        /// <summary>
        /// Resets the ability uses
        /// </summary>
        public void ResetAbilities()
        {
            for (int i = 0; i < abilities.Count; i++)
            { abilities[i].ResetAbilityUses(); }
        }
    }
}