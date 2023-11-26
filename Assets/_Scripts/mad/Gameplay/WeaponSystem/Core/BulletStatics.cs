using WGRF.Core;

namespace WGRF.BattleSystem
{
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

                if (ManagerHub.S != null)
                {
                    ManagerHub.S.GameEventHandler.OnBulletSpeedChange(_currentSpeed);
                }
            }
        }

        //Default bullet speed values
        public static float StartingSpeed { get; set; }
        public static float SlowDownSpeed { get; set; }
        public static float StopTimeSpeed = 0f;
    }
}