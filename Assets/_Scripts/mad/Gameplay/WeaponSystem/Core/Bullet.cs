using UnityEngine;
using WGRF.Core;
using WGRF.Interactions;

namespace WGRF.BattleSystem
{
    /// <summary>
    /// The layers every bullet gameObject can exist on.
    /// </summary>
    public enum ProjectileLayers
    {
        PlayerProjectile = 12,
        EnemyProjectile = 13,
    }

    /// <summary>
    /// The individual bullet handler for projectiles
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        ///<summary>The addressable blood impact gfx path</summary>
        [Header("Set in inspector")]
        [SerializeField, Tooltip("The addressable blood impact gfx path")] string bloodImpactFX_Path;
        ///<summary>The initial bullet speed</summary>
        [SerializeField, Tooltip("The initial bullet speed")] float bulletSpeed;
        ///<summary>The bullet damage</summary>
        [SerializeField, Tooltip("The bullet damage")] int damage = 5;

        private void Update()
        { transform.position += transform.forward * bulletSpeed * Time.deltaTime; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out IInteractable interaction))
            {
                interaction.AttackInteraction(damage);

                if (ManagerHub.S.SettingsHandler.UserSettings.goreVFX)
                {
                    //Spawn the blood impact FX when the bullet hits an entity
                    GameObject spawnedFX = Instantiate(UnityAssets.Load(bloodImpactFX_Path, false));
                    spawnedFX.transform.position = other.transform.position;
                    spawnedFX.transform.rotation = Quaternion.identity;
                    spawnedFX.transform.rotation = transform.rotation * Quaternion.Euler(-90f, 0f, 0f);
                }
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Call to set THIS bullet instances' bullet type.
        /// <para>Bullet type of:</para>
        /// <para>Enemy: Collides with the player and not with AIEntities.</para>
        /// <para>Player: Collides with the AIEntities and not the player.</para>
        /// </summary>
        public void SetBulletType(BulletType type)
        { transform.root.gameObject.layer = (int)type; }

        /// <summary>
        /// Resets bullet variables when the bullet gets enabled.
        /// </summary>
        private void OnEnable()
        { Destroy(gameObject, 3f); }
    }
}