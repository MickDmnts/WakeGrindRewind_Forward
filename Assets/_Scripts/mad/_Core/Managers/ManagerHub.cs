using UnityEngine;

using WGRF.Internal;

namespace WGRF.Core
{
    /// <summary>
    /// In the ManagerHub every handler instance is created and cached.
    /// </summary>
    [DefaultExecutionOrder(-500)]
    public class ManagerHub : MonoBehaviour
    {
        ///<summary>Manager Hub reference</summary>
        public static ManagerHub S;

        ///<summary>GameEventsHandler reference</summary>
        GameEventsHandler _gameEventsHandler;
        ///<summary>The globals reference contains usefull game-wide paths and variables</summary>
        AppInternal _globals;
        ///<summary>LevelManager reference</summary>
        LevelManager _levelManager;
        ///<summary>Database handler reference</summary>
        Database _database;
        ///<summary>The settings handler reference</summary>
        SettingsHandler _settingsHandler;
        ///<summary>The cursor handler reference</summary>
        CursorHandler _cursorHandler;
        ///<summary>The WGRF audio handler reference</summary>
        GameSoundsHandler _gameSoundsHandler;

        ///<summary>Returns the GameEventsHandler reference</summary>
        public GameEventsHandler GameEventHandler => _gameEventsHandler;
        ///<summary>Returns the globals reference containing game-wide paths and variables</summary>
        public AppInternal Globals => _globals;
        ///<summary>Returns the LevelManager reference</summary>
        public LevelManager LevelManager => _levelManager;
        ///<summary>Returns the Database handler reference</summary>
        public Database Database => _database;
        ///<summary>Returns the settings handler reference</summary>
        public SettingsHandler SettingsHandler => _settingsHandler;
        ///<summary>Returns the cursor handler reference</summary>
        public CursorHandler CursorHandler => _cursorHandler;
        ///<summary>Returns the WGRF audio handler reference</summary>
        public GameSoundsHandler GameSoundsHandler => _gameSoundsHandler;

        /*public UI_Manager UIManager { get; private set; }
        public UserHUDHandler HUDHandler { get; private set; }
        public BulletPool BulletPool { get; private set; }
        public AIEntityManager AIEntityManager { get; private set; }
        public WeaponManager WeaponManager { get; private set; }
        public AbilityManager AbilityManager { get; private set; }
        public SkillPointHandle SkillPointHandle { get; private set; }
        public WeaponSelectionUI WeaponSelectionUIHandler { get; private set; }*/

        //public PlayerEntity PlayerEntity { get; private set; }

        private void Awake()
        {
            if (S != this)
            {
                S = this;
            }

            CreateManagers();
        }

        void CreateManagers()
        {
            _gameEventsHandler = new GameEventsHandler();
            _globals = new AppInternal();
            _database = new Database();
            _settingsHandler = new SettingsHandler();
            _cursorHandler = new CursorHandler();
            _gameSoundsHandler = GetComponent<GameSoundsHandler>();
        }
    }
}