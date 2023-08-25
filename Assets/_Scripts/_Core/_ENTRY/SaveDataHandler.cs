using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WGR.BattleSystem;

/* [CLASS DOCUMENTATION]
* 
*  Private Variables: These values are changed throughout the game.
* 
* [Class flow]
*  1. This class runs exactly before the main menu scene loads and checks for a present save data file in the users Documents folder.
*/

namespace WGR.Core.Managers
{
    [DefaultExecutionOrder(10)]
    public class SaveDataHandler : MonoBehaviour
    {
        #region PRIVATE_VARIABLES
        string savePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/WGR_Saves";
        string saveFileName = "/save.wgr";

        SaveDataInfo saveData;
        bool hasSave;
        #endregion

        private void Awake()
        {
            CheckForSaveFile();
        }

        /// <summary>
        /// Call to check if there is a save file present in the user's documents folder.
        /// <para>If the file exists, calls LoadSaveFile() and sets hasSave to true.</para>
        /// <para>If there is not, calls CreateSaveFolder() and sets hasSave to false.</para>
        /// </summary>
        void CheckForSaveFile()
        {
            if (IsSavePresent())
            {
                saveData = LoadSaveFile();
                hasSave = true;
            }
            else
            {
                CreateSaveFolder();
                hasSave = false;
            }
        }

        /// <summary>
        /// Checks if there is a save file in the savePath folder.
        /// </summary>
        /// <returns>Returns true if saveFileName exists in the savePath folder, false otherwise.</returns>
        bool IsSavePresent()
        {
            //First check if the folder exists...
            if (Directory.Exists(savePath))
            {
                //...and then check for the save file.
                if (File.Exists(savePath + saveFileName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Call to load all the values from the json save file.
        /// </summary>
        /// <returns>The loaded SaveDataInfo file.</returns>
        SaveDataInfo LoadSaveFile()
        {
            string jsonSave = File.ReadAllText(savePath + saveFileName);
            SaveDataInfo saveFile = JsonUtility.FromJson<SaveDataInfo>(jsonSave);

            return saveFile;
        }

        /// <summary>
        /// Call to create the save file directory in the users documents folder.
        /// </summary>
        void CreateSaveFolder()
        {
            DirectoryInfo info = Directory.CreateDirectory(savePath);
            info.Attributes = FileAttributes.Hidden;
        }

        /// <summary>
        /// Call to read the appropriate in-game values, populate the SaveDataInfo class and then write 
        /// the json string into the save data file.
        /// </summary>
        public void SaveGame()
        {
            PopulateDataFile(out saveData);

            string jsonFile = WriteClassToJson(saveData);

            WriteJsonToFile(jsonFile);
        }

        /// <summary>
        /// Call to write the ability tiers,
        /// kills per gun and remaining skill points
        /// into the SaveDataInfo dataContainer class.
        /// </summary>
        /// <param name="dataContainer">SaveDataInfo to initialize and populate.</param>
        void PopulateDataFile(out SaveDataInfo dataContainer)
        {
            dataContainer = new SaveDataInfo();

            if (GameManager.S != null)
            {
                //Cache the player skill points.
                int skillPoints = GameManager.S.SkillPointHandle.RemainingSkillPoints();

                //Cache the each ability tier.
                int[] abilityTiers = new int[3]
                {
                GameManager.S.AbilityManager.GetAbilityCurrentTier(AbilityType.Slowtime),
                GameManager.S.AbilityManager.GetAbilityCurrentTier(AbilityType.RewindTime),
                GameManager.S.AbilityManager.GetAbilityCurrentTier(AbilityType.StopTime),
                };

                //Cache the weapon kills
                Dictionary<WeaponType, WeaponUnlockData> weaponData = GameManager.S.WeaponManager.WeaponKillCount.GetWeaponUnlockDataDict();
                int[] gunKillCounts = new int[weaponData.Count];

                for (int i = 0; i < gunKillCounts.Length; i++)
                {
                    gunKillCounts[i] = weaponData.ElementAt(i).Value.KillCount;
                }

                //Populate each SaveDataInfo field with the appropriate values.
                dataContainer.RemainingSkillPoints = skillPoints;
                dataContainer.AbilityTiersInOrder = abilityTiers;
                dataContainer.GunKillsInOrder = gunKillCounts;

                //Populate the player deaths field.
                dataContainer.PlayerDeaths = GameManager.S.PlayerDeaths;
            }
            else
                Utils.MissingComponent("GameManager", this);
        }

        /// <summary>
        /// Call to parse the passed SaveDataInfo instance to a json String.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The parsed json string.</returns>
        string WriteClassToJson(SaveDataInfo file)
        {
            return JsonUtility.ToJson(file);
        }

        /// <summary>
        /// Call to write the passed json string into the save data file in the users documents.
        /// </summary>
        /// <param name="jsonString">The json string to write in the save file.</param>
        void WriteJsonToFile(string jsonString)
        {
            File.WriteAllText(savePath + saveFileName, jsonString);
        }

        /// <summary>
        /// Returns the hasSaved value.
        /// </summary>
        public bool HasSavedBefore()
        {
            return hasSave;
        }

        /// <summary>
        /// Call to delete the save data file in the users documents.
        /// <para>Invokes GameEventHandler.OnSaveOvewrite() and sets saveData field to null.</para>
        /// </summary>
        public void OverwriteSaveFile()
        {
            File.Delete(savePath + saveFileName);

            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.OnSaveOvewrite();
            }

            saveData = null;
        }

        /// <summary>
        /// Call to get the currently saveData value.
        /// </summary>
        /// <returns></returns>
        public SaveDataInfo GetSaveFileInfo()
        {
            return saveData;
        }
    }
}