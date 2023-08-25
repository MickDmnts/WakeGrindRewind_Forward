using UnityEngine;

using WGR.Entities.Player;
using WGR.UI;

namespace WGR.Core.Managers
{
    /* [CLASS DOCUMENTATION]
     * 
     * A SINGLETON IS PRESENT IN THIS CLASS SO IT IS ACCESIBLE FROM EVERY SCRIPT.
     * 
     * Every variable in this class stores a manager reference.
     * 
     * [Class flow]
     *  1. This class is created dynamically from the GameManager in runtime.
     * 
     * [Must Know]
     * This class file acts as central hub for different system managers.
     * 
     * 1. GameEventHandler, UIManager and EnemyEntityManager are CREATED in runtime from this script.
     * 2. LevelManager, BulletPool, WeaponManager, AbilityManager, SkillPointHandle
     *      are found in runtime
     * 
     */

    [DefaultExecutionOrder(25)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager S;

        #region MANAGERS
        public GameEventsHandler GameEventHandler { get; private set; }
        public LevelManager LevelManager { get; private set; }
        public UI_Manager UIManager { get; private set; }
        public UserHUDHandler HUDHandler { get; private set; }
        public BulletPool BulletPool { get; private set; }
        public AIEntityManager AIEntityManager { get; private set; }
        public WeaponManager WeaponManager { get; private set; }
        public AbilityManager AbilityManager { get; private set; }
        public SkillPointHandle SkillPointHandle { get; private set; }
        public GameSoundsHandler GameSoundsHandler { get; private set; }
        public WeaponSelectionUI WeaponSelectionUIHandler { get; private set; }
        #endregion

        public SaveDataHandler SaveDataHandler { get; private set; }
        public PlayerEntity PlayerEntity { get; private set; }

        int playerDeaths = 0;
        public int PlayerDeaths
        {
            get
            {
                return playerDeaths;
            }
            set
            {
                playerDeaths = value;
            }
        }

        private void Awake()
        {
            if (S != this)
            {
                S = this;
            }

            QualitySettings.vSyncCount = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            CacheManagers();
        }

        void CacheManagers()
        {
            SaveDataHandler = FindObjectOfType<SaveDataHandler>();

            GameEventHandler = new GameEventsHandler();
            UIManager = gameObject.AddComponent<UI_Manager>();
            AIEntityManager = gameObject.AddComponent<AIEntityManager>();

            LevelManager = FindObjectOfType<LevelManager>();
            BulletPool = FindObjectOfType<BulletPool>();
            WeaponManager = FindObjectOfType<WeaponManager>();
            AbilityManager = FindObjectOfType<AbilityManager>();
            SkillPointHandle = FindObjectOfType<SkillPointHandle>();
            GameSoundsHandler = FindObjectOfType<GameSoundsHandler>();

        }

        private void Start()
        {
            if (GameEventHandler != null)
            {
                GameEventHandler.onSceneChanged += InitiateAutosave;
                GameEventHandler.onSaveOverwrite += ResetPlayerDeaths;
            }

            //Get the saved player deaths.
            SaveDataInfo tempData = SaveDataHandler.GetSaveFileInfo();
            if (tempData != null)
            {
                playerDeaths = tempData.PlayerDeaths;
            }
        }

        /// <summary>
        /// <para>*Subscribed to GameEventHandler.onSceneChanged*</para>
        /// Called whenever a scene loads to save the game
        /// </summary>
        /// <param name="scene"></param>
        void InitiateAutosave(GameScenes scene)
        {
            if (scene.Equals(GameScenes.PlayerHub))
            {
                SaveDataHandler.SaveGame();
            }
        }

        /// <summary>
        /// <para>*Subscribed to GameEventHandler.onSaveOverwrite*</para>
        /// Call to set player deaths to 0.
        /// </summary>
        void ResetPlayerDeaths()
        {
            playerDeaths = 0;
        }

        /// <summary>
        /// Call to set the PlayerEntity field to the passed reference.
        /// Called from PlayerEntity in runtime.
        /// </summary>
        public void SetPlayerEntity(PlayerEntity player)
        {
            PlayerEntity = player;
        }

        /// <summary>
        /// Call to set the UserHUDHandler field to the passed reference.
        /// Called from UserHUDHandler in runtime.
        /// </summary>
        public void SetHUDHandler(UserHUDHandler hudHandler)
        {
            HUDHandler = hudHandler;
        }

        /// <summary>
        /// Call to set the WeaponSelectionUI field to the passed reference.
        /// Called from WeaponSelectionUI in runtime.
        /// </summary>
        public void SetWeaponSelectionUIHandler(WeaponSelectionUI weaponSelectionUI)
        {
            WeaponSelectionUIHandler = weaponSelectionUI;
        }

        private void OnDestroy()
        {
            if (GameEventHandler != null)
            {
                GameEventHandler.onSceneChanged -= InitiateAutosave;
                GameEventHandler.onSaveOverwrite -= ResetPlayerDeaths;
            }
        }
    }
}