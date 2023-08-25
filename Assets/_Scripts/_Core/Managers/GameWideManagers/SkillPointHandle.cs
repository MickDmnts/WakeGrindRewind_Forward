using UnityEngine;

namespace WGR.Core.Managers
{
    public class SkillPointHandle : MonoBehaviour
    {
        //Dynamically changed
        int availableSkillPoints = 0;

        private void Start()
        {
            GameManager.S.GameEventHandler.onSaveOverwrite += OnNewGameSave;
            GameManager.S.GameEventHandler.onLevelPassed += DecidePointIncrementValue;

            if (GameManager.S.SaveDataHandler.HasSavedBefore())
            {
                SetSkillPoints(GameManager.S.SaveDataHandler.GetSaveFileInfo().RemainingSkillPoints);
            }
        }

        void OnNewGameSave()
        {
            SetSkillPoints(0);
        }

        /// <summary>
        /// Called when the player WINS a level to decide how many skill points he shall receive
        /// based on the level number.
        /// </summary>
        /// <param name="scene">The level won</param>
        void DecidePointIncrementValue(GameScenes scene)
        {
            switch (scene)
            {
                case GameScenes.Level1:
                    IncrementSkillPoints(1);
                    break;
                case GameScenes.Level2:
                    IncrementSkillPoints(2);
                    break;
                case GameScenes.Level3:
                    IncrementSkillPoints(3);
                    break;
                case GameScenes.Level4:
                    IncrementSkillPoints(4);
                    break;
                case GameScenes.Level5:
                    IncrementSkillPoints(5);
                    break;
            }
        }

        /// <summary>
        /// Call to increment availableSkillPoints by the passed value.
        /// </summary>
        /// <param name="value"></param>
        void IncrementSkillPoints(int value)
        {
            availableSkillPoints += value;
        }

        /// <summary>
        /// Call to get the availableSkillPoints count.
        /// </summary>
        /// <returns></returns>
        public int RemainingSkillPoints()
        {
            return availableSkillPoints;
        }

        /// <summary>
        /// Call to Decrease the skill point value by the passed value.
        /// Called from each player ability when they succesfully update.
        /// </summary>
        /// <param name="value"></param>
        public void DecreaseSkillPoints(int value)
        {
            availableSkillPoints -= value;
        }

        /// <summary>
        /// Call to set the availableSkillPoints to the passed value.
        /// To-be-used with json loading.
        /// </summary>
        public void SetSkillPoints(int value)
        {
            availableSkillPoints = value;
        }

        private void OnDestroy()
        {
            GameManager.S.GameEventHandler.onSaveOverwrite -= OnNewGameSave;
            GameManager.S.GameEventHandler.onLevelPassed -= DecidePointIncrementValue;
        }
    }
}