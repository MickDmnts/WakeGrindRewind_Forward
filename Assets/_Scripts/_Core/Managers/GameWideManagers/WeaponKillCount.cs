using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using WGR.Core;

namespace WGR.Gameplay.BattleSystem
{
    /// <summary>
    /// This struct is used to store weapon kill counts and weapon types tied together.
    /// </summary>
    [System.Serializable]
    public struct WeaponUnlockData
    {
        public WeaponUnlockData(WeaponType type)
        {
            this.WeaponType = type;
            this.KillCount = 0;
            this.IsUnlocked = false;
        }

        public WeaponType WeaponType;
        public int KillCount;
        public bool IsUnlocked;
    }

    /* [CLASS DOCUMENTATION]
    * 
    * Private variables: These values change in runtime.
    * 
    * [Class flow]
    * 1. When the game loads the weaponCountPairs dictionary gets created with default weapon values.
    * 
    * [Must know]
    * 1. Kills are externally increased from the player attacking.
    * 2. The weapon selection screen buttons check if the weapon is unlocked before updating their image to a colored one.
    * 3. The weapon kills and unlocked states get automatically updated from the save data file when the game loads.
    * 4. When the user starts a new save the weapon kills and unlocked states are set back to default values.
    * 
    */
    public class WeaponKillCount : MonoBehaviour
    {
        //Private variable
        Dictionary<WeaponType, WeaponUnlockData> weaponCountPairs = new Dictionary<WeaponType, WeaponUnlockData>();

        private void Awake()
        {
            InitializeWeaponData();
        }

        /// <summary>
        /// Call to create a WeaponType, WeaponUnlockData pair based on the WeaponManager.GetWeaponSOsList indexing 
        /// and then add it to the weaponCountPairs dictionary.
        /// <para>Bypasses Unarmed WeaponCategory weapons.</para>
        /// </summary>
        void InitializeWeaponData()
        {
            foreach (Weapon weapon in GameManager.S.WeaponManager.GetWeaponSOsList())
            {
                if (weapon.WeaponCategory.Equals(WeaponCategory.Unarmed)) continue;

                weaponCountPairs.Add(weapon.WeaponType, new WeaponUnlockData(weapon.WeaponType));
            }
        }

        private void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSaveOverwrite += OnNewGameSave;
            }

            UpdateDataFromJSON();
        }

        /// <summary>
        /// Call to update each weapon pair from the weaponCountPairs dictionary with the saved data values.
        /// <para>If there was no save, set values to default, else updates from the save data file.</para>
        /// </summary>
        void UpdateDataFromJSON()
        {
            for (int i = 0; i < weaponCountPairs.Count; i++)
            {
                //Cache the currently inspected dictionary element.
                WeaponUnlockData tempWeaponData = weaponCountPairs.ElementAt(i).Value;
                WeaponType tempType = tempWeaponData.WeaponType;

                //In case there was a save file.
                if (GameManager.S.SaveDataHandler.HasSavedBefore())
                {
                    if (GameManager.S != null)
                    {
                        tempWeaponData.KillCount = GameManager.S.SaveDataHandler.GetSaveFileInfo().GunKillsInOrder[i];
                    }

                    CheckForUnlock(ref tempWeaponData);
                }
                else //In case there was no save file 
                {
                    tempWeaponData.KillCount = 0;

                    CheckForUnlock(ref tempWeaponData);
                }

                //Add the pair back to its dictionary position.
                weaponCountPairs[tempType] = tempWeaponData;
            }
        }

        /// <summary>
        /// Call to check if the weapon data passed has a kill count greater than 10, if yes
        /// sets its IsUnlocked value to true.
        /// </summary>
        /// <param name="weaponData"></param>
        void CheckForUnlock(ref WeaponUnlockData weaponData)
        {
            if (weaponData.KillCount >= 10)
            {
                weaponData.IsUnlocked = true;
            }
        }

        /// <summary>
        /// *Subscribed to GameEventHandler.onSaveOverwrite event*
        /// <para>Call to set every dictionary weapon pair to default values EXCEPT the 0th one
        /// which gets set to 10 kills and isUnlocked to true. (Bat weapon)</para>
        /// </summary>
        void OnNewGameSave()
        {
            for (int i = 0; i < weaponCountPairs.Count; i++)
            {
                WeaponUnlockData tempWeaponData = weaponCountPairs.ElementAt(i).Value;
                WeaponType tempType = tempWeaponData.WeaponType;

                //Set the bat to be unlocked.
                if (i == 0)
                {
                    tempWeaponData.KillCount = 10;
                    tempWeaponData.IsUnlocked = true;
                }
                else //set every other weapon to locked 
                {
                    tempWeaponData.KillCount = 0;
                    tempWeaponData.IsUnlocked = false;
                }

                //Update the weapon
                CheckForUnlock(ref tempWeaponData);

                //Add the pair back to its dictionary position.
                weaponCountPairs[tempType] = tempWeaponData;
            }
        }

        /// <summary>
        /// Call to add a kill to the passed weapon type.
        /// <para>Weapon types are unique per weapon.</para>
        /// </summary>
        public void AddKillToWeapon(WeaponType weaponType)
        {
            //We don't count the abilities test room kills so early return.
            if (GameManager.S.LevelManager.FocusedScene == GameScenes.AbilitiesTutorial) return;

            //Double check that the weapon exists in the dictionary before trying to access it.
            if (weaponCountPairs.ContainsKey(weaponType) && !weaponType.Equals(WeaponType.Punch))
            {
                //Increase its kill count.
                WeaponUnlockData tempData = weaponCountPairs[weaponType];
                tempData.KillCount++;

                //Update its new isUnlocked state.
                CheckForUnlock(ref tempData);

                //Add the pair back to its dictionary position.
                weaponCountPairs[weaponType] = tempData;
            }
        }

        /// <summary>
        /// Call to check if the passed weapon type is unlocked in the dictionary.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if the passed weapon is unlocked, false if otherwise.</returns>
        public bool IsWeaponUnlocked(WeaponType type)
        {
            if (weaponCountPairs.ContainsKey(type))
            {
                return weaponCountPairs[type].IsUnlocked;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Call to get the weapon pair dictionary.
        /// </summary>
        public Dictionary<WeaponType, WeaponUnlockData> GetWeaponUnlockDataDict() => weaponCountPairs;

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSaveOverwrite -= OnNewGameSave;
            }
        }
    }
}