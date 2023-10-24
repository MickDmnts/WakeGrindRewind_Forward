using UnityEngine;

namespace WGRF.Core
{
    [DefaultExecutionOrder(-500)]
    public class ManagerHub : MonoBehaviour
    {
        ///<summary>Manager Hub reference</summary>
        public static ManagerHub S;

        #region MANAGERS
        public GameEventsHandler GameEventHandler { get; private set; }
        public LevelManager LevelManager { get; private set; }
        /*public UI_Manager UIManager { get; private set; }
        public UserHUDHandler HUDHandler { get; private set; }
        public BulletPool BulletPool { get; private set; }
        public AIEntityManager AIEntityManager { get; private set; }
        public WeaponManager WeaponManager { get; private set; }
        public AbilityManager AbilityManager { get; private set; }
        public SkillPointHandle SkillPointHandle { get; private set; }
        public GameSoundsHandler GameSoundsHandler { get; private set; }
        public WeaponSelectionUI WeaponSelectionUIHandler { get; private set; }*/
        #endregion

        //public SaveDataHandler SaveDataHandler { get; private set; }
        //public PlayerEntity PlayerEntity { get; private set; }

        private void Awake()
        {
            if (S != this)
            {
                S = this;
            }

            QualitySettings.vSyncCount = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
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