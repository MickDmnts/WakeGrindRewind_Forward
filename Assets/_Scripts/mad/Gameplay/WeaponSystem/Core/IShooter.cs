namespace WGRF.BattleSystem
{
    /// <summary>
    /// Interface containing the necessary shooting mechanism methods.
    /// </summary>
    public interface IShooter
    {
        /// <summary>
        /// Call to set the required Shooter fields based on the passed Weapon Scriptable Object.
        /// </summary>
        void SetWeaponInfo(Weapon weapon);

        void Shoot();
    }
}