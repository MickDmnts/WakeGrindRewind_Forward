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
        ///<summary>The weapon name</summary>
        [Tooltip("The weapon name")] public string WeaponName;
        ///<summary>The weapon category</summary>
        [Tooltip("The weapon category")] public WeaponCategory WeaponCategory;
        ///<summary>The weapon type</summary>
        [Tooltip("The weapon type")] public WeaponType WeaponType;
        ///<summary>The weapon sprite</summary>
        [Tooltip("The weapon sprite")] public Sprite WeaponSprite;
        ///<summary>The weapon ammunition sprite</summary>
        [Tooltip("The weapon ammunition sprite")] public Sprite weaponAmmoSprite;
        ///<summary>The weapon in-world equiped sprite</summary>
        [Tooltip("The weapon in-world equiped sprite")] public Sprite weaponEquipedSprite;
        ///<summary>The interval between the shots of the weapon</summary>
        [Tooltip("The interval between the shots of the weapon")] public float IntervalBetweenShots;
        ///<summary>The default magazine size</summary>
        [Tooltip("The default magazine size")] public int DefaultMagazine;
        ///<summary>Max bullet spread</summary>
        [Tooltip("Max bullet spread")] public float MaxBulletSpread;
        ///<summary>Base damage</summary>
        [Tooltip("Base damage")] public int Damage = 20;

        ///<summary>AI: Minimum shoot distance for this weapon</summary>
        [Tooltip("AI: Minimum shoot distance for this weapon")] public float MinShootDistance;
    }
}