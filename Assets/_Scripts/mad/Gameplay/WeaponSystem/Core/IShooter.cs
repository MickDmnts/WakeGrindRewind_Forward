namespace WGRF.BattleSystem
{
    /* 
     * Interface containing the necessary shooting mechanism methods. 
     */
    public interface IShooter
    {
        /// <summary>
        /// Call to set the required Shooter fields based on the passed Weapon Scriptable Object.
        /// <para><paramref name="cachedBulletCount"/>Must be set only in the PlayerWeapon.</para>
        /// </summary>
        void SetWeaponInfo(Weapon weapon, int cachedBulletCount = -1);

        void Shoot();
    }
}