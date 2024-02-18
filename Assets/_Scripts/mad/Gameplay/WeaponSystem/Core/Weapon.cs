using UnityEngine;

namespace WGRF.BattleSystem
{
    /// <summary>
    /// Every weapon category in the game
    /// </summary>
    public enum WeaponCategory
    {
        Unarmed = 0,
        Melee = 1,
        Ranged = 2,
    }

    /// <summary>
    /// Every weapon type in the game
    /// </summary>
    public enum WeaponType
    {
        Knife = 0,
        BaseballBat = 1,
        Punch = 2,

        Pistol = 3,
        SemiAutomatic = 4,
        Shotgun = 5,

        _NaN = -1,
        Throwable = -2,
    }

    /// <summary>
    /// A template for weapon creation scriptable objects.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponAsset", menuName = "WeaponSystem/Weapon", order = 1)]
    public class Weapon : ScriptableObject
    {
        public string WeaponName;
        public WeaponCategory WeaponCategory;
        public WeaponType WeaponType;
        public Sprite WeaponSprite;
        public Sprite weaponGrayedSprite;
        public Sprite weaponAmmoSprite;
        public Sprite weaponEquipedSprite;
        public float IntervalBetweenShots;
        public int DefaultMagazine;
        public float MaxBulletSpread;
        public int Damage = 20;

        //AI-specific
        public float MinShootDistance;
    }
}