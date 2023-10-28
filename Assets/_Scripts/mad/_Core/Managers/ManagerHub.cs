using UnityEngine;

using WGRF.Bus;

namespace WGRF.Core
{
    [DefaultExecutionOrder(-500)]
    public class ManagerHub : MonoBehaviour
    {
        ///<summary>Manager Hub reference</summary>
        public static ManagerHub S;

        ///<summary>GameEventsHandler reference</summary>
        GameEventsHandler _gameEventsHandler;
        ///<summary>LevelManager reference</summary>
        LevelManager _levelManager;
        ///<summary>Database handler reference</summary>
        Database _database;
        ///<summary>The cursor handler reference</summary>
        CursorHandler _cursorHandler;

        ///<summary>Returns the GameEventsHandler reference</summary>
        public GameEventsHandler GameEventHandler => _gameEventsHandler;
        ///<summary>Returns the LevelManager reference</summary>
        public LevelManager LevelManager => _levelManager;
        ///<summary>Returns the Database handler reference</summary>
        public Database Database => _database;
        ///<summary>Returns the cursor handler reference</summary>
        public CursorHandler CursorHandler => _cursorHandler;


        /*public UI_Manager UIManager { get; private set; }
        public UserHUDHandler HUDHandler { get; private set; }
        public BulletPool BulletPool { get; private set; }
        public AIEntityManager AIEntityManager { get; private set; }
        public WeaponManager WeaponManager { get; private set; }
        public AbilityManager AbilityManager { get; private set; }
        public SkillPointHandle SkillPointHandle { get; private set; }
        public GameSoundsHandler GameSoundsHandler { get; private set; }
        public WeaponSelectionUI WeaponSelectionUIHandler { get; private set; }*/

        //public SaveDataHandler SaveDataHandler { get; private set; }
        //public PlayerEntity PlayerEntity { get; private set; }

        private void Awake()
        {
            if (S != this)
            {
                S = this;
            }

            //============================================
            //To be moved to cursor handler class
            /*QualitySettings.vSyncCount = 1;*/
            //============================================
        }

        void CreateManagers()
        {
            _gameEventsHandler = new GameEventsHandler();
            _database = new Database();
            _cursorHandler = new CursorHandler();
        }

        /// <summary>
        /// Call to set the PlayerEntity field to the passed reference.
        /// Called from PlayerEntity in runtime.
        /// </summary>
        /* public void SetPlayerEntity(PlayerEntity player)
        {
            PlayerEntity = player;
        } */
    }
}