using WGR.Core;

namespace WGR.Gameplay.BattleSystem
{
    /* 
     * The purpose of this static class is to cache the bullet speeds of the game.
     * Each bullet speed gets changed whenever the CurrentSpeed field changes.
     * Primarily used to simulate the ability time changes.
     */
    public static class BulletStatics
    {
        private static float _currentSpeed;
        /// <summary>
        /// When set the value passed gets passed through the 
        /// GameEventHandler.OnBulletSpeedChange(...) event to update every bullet speed.
        /// </summary>
        public static float CurrentSpeed
        {
            get
            {
                return _currentSpeed;
            }
            set
            {
                _currentSpeed = value;

                if (GameManager.S != null)
                {
                    GameManager.S.GameEventHandler.OnBulletSpeedChange(_currentSpeed);
                }
            }
        }

        //Default bullet speed values
        public static float StartingSpeed { get; set; }
        public static float SlowDownSpeed { get; set; }
        public static float StopTimeSpeed = 0f;
    }
}