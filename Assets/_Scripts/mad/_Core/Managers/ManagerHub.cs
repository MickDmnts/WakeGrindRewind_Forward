using UnityEngine;
using WGRF.BattleSystem;
using WGRF.Core.Managers;
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
        ///<summary>Database handler reference</summary>
        Database _database;
        ///<summary>The settings handler reference</summary>
        SettingsHandler _settingsHandler;
        ///<summary>The cursor handler reference</summary>
        CursorHandler _cursorHandler;
        ///<summary>The WGRF audio handler reference</summary>
        GameSoundsHandler _gameSoundsHandler;
        ///<summary>The stage handler reference</summary>
        StageHandler _stageHandler;
        ///<summary>The time scale handler of WGRF</summary>
        InternalTime _internalTime;
        ///<summary>Reference to the player controller</summary>
        Controller _playerController;
        ///<summary>Reference to the bullet pool of the game</summary>
        BulletPool _bulletPool;
        ///<summary>Reference to the weapon manager</summary>
        WeaponManager _weaponManager;
        

        ///<summary>Returns the GameEventsHandler reference</summary>
        public GameEventsHandler GameEventHandler => _gameEventsHandler;
        ///<summary>Returns the globals reference containing game-wide paths and variables</summary>
        public AppInternal Globals => _globals;
        ///<summary>Returns the Database handler reference</summary>
        public Database Database => _database;
        ///<summary>Returns the settings handler reference</summary>
        public SettingsHandler SettingsHandler => _settingsHandler;
        ///<summary>Returns the cursor handler reference</summary>
        public CursorHandler CursorHandler => _cursorHandler;
        ///<summary>Returns the WGRF audio handler reference</summary>
        public GameSoundsHandler GameSoundsHandler => _gameSoundsHandler;
        ///<summary>Returns the reference to the Stage Handler of WGRF</summary>
        public StageHandler StageHandler => _stageHandler;
        ///<summary>Returns the reference to the Internal Time of WGRF</summary>
        public InternalTime InternalTime => _internalTime;
        ///<summary>Returns the reference to the player Controller</summary>
        public Controller PlayerController => _playerController;
        ///<summary>Returns the reference to the bullet pool</summary>
        public BulletPool BulletPool => _bulletPool;
        ///<summary>Returns the reference to the Weapon manager</summary>
        public WeaponManager WeaponManager => _weaponManager;

        /*public UI_Manager UIManager { get; private set; }
        public UserHUDHandler HUDHandler { get; private set; }
        public AIEntityManager AIEntityManager { get; private set; }
        public AbilityManager AbilityManager { get; private set; }
        public SkillPointHandle SkillPointHandle { get; private set; }
        public WeaponSelectionUI WeaponSelectionUIHandler { get; private set; }*/

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
            _stageHandler = new StageHandler();
            _internalTime = new InternalTime(this);
        }

        /// <summary>
        /// * PLAYER USE ONLY *
        /// Attaches the player's Controller reference to the ManagerHub.
        /// </summary>
        /// <param name="controller">The player's controller</param>
        public void AttachPlayerController(Controller controller)
        { _playerController = controller; }
    }
}