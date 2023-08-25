using UnityEngine;
using WGR.BattleSystem;

namespace WGR.AI.Entities.Hostile.Boss
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: These values must be from the inspector
     * Private Variables: These values change in runtime.
     * 
     * [Must know]
     * 0. The attached script caches the Rigidbody of the ROOT transform.
     * 
     * 1. This script is attached in the boss entity and enables his ability to dodge bullets.
     * 2. The boss gets pushed to the opposite of the incoming projectile direction.
     * 
     */
    public class BulletDetector : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] float maxRadius;
        [SerializeField] float evadeCooldown;
        [HideInInspector] public bool IsActive;

        #region PRIVATE_VARIABLES
        Rigidbody enemyRB;
        bool isInRadius = false;
        bool evaded = false;
        Vector3 incomingProjectileDirection = Vector3.zero;

        float evadeCDCache;
        #endregion

        private void Start()
        {
            EntrySetup();
        }

        /// <summary>
        /// Call to set the default behaviour of the bullet detector on scene load.
        /// </summary>
        void EntrySetup()
        {
            evadeCDCache = evadeCooldown;
            enemyRB = transform.root.GetComponent<Rigidbody>();
            IsActive = true;
        }

        private void Update()
        {
            //Dont update when the detector is inactive
            if (!IsActive) return;

            //Starts the cooldown
            if (evaded)
            {
                evadeCooldown -= Time.deltaTime;
                if (evadeCooldown <= 0)
                {
                    //Resets the bullet detector.
                    evaded = false;
                    evadeCooldown = evadeCDCache;
                }

                //prevents the IsInRadious from updating.
                return;
            }

            //Bullet detection
            isInRadius = IsInRadius(transform.parent.transform, maxRadius, out incomingProjectileDirection);
            if (isInRadius)
            {
                PushInOppositeDir(incomingProjectileDirection);
                evaded = true;
            }
        }

        /// <summary>
        /// Call to chek if any projectile gameObject is inside the passed maxRadius value around the attached gameObject.
        /// </summary>
        /// <param name="directionOut">Where should the incoming projectile direction get stored?</param>
        /// <returns>True, if a projectile is inside the radius and nothing's masking it.
        /// False, if not.</returns>
        bool IsInRadius(Transform parentAI, float maxRadius, out Vector3 directionOut)
        {
            directionOut = Vector3.zero;

            Collider[] overlaps = new Collider[10];
            int count = Physics.OverlapSphereNonAlloc(parentAI.position, maxRadius, overlaps);

            //Check for projectile collisions.
            for (int i = 0; i < count; i++)
            {
                if (overlaps[i].Equals(null)) continue;

                //If the gameObject is a bullet
                if (overlaps[i].gameObject.layer == (int)ProjectileLayers.PlayerProjectile)
                {
                    //And nothing is masking it.
                    if (Physics.Linecast(transform.parent.transform.position, overlaps[i].transform.position))
                    {
                        Debug.DrawLine(transform.parent.transform.position, overlaps[i].transform.position);

                        //update the out value.
                        directionOut = overlaps[i].transform.position - transform.parent.transform.position;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Call to push the attached gameObject to the opposite of the passed Vector3 value.
        /// </summary>
        void PushInOppositeDir(Vector3 incomingDirection)
        {
            if (Vector3.Dot(-transform.parent.transform.TransformDirection(Vector3.left), incomingDirection) < 0)
            {
                enemyRB.velocity = transform.parent.transform.right * 50f;
            }
            else
            {
                enemyRB.velocity = -transform.parent.transform.right * 50f;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!IsActive) return;

            //Draw the detection radius around the enemy
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxRadius);

            if (!isInRadius)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            //Draw the middle black ray
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
        }
#endif
    }
}