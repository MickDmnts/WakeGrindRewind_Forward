using System.Collections;
using UnityEngine;

using WGRF.Abilities;
using WGRF.Core;

namespace WGRF.Player
{
    [DefaultExecutionOrder(150)]
    public class PlayerEntity : Entity, IRewindable
    {
        ///<summary>The blood decal addressable path of the player</summary>
        [Header("Decal on death")]
        [SerializeField, Tooltip("The blood decal addressable path of the player")]
        string bloodDecalPath;

        ///<summary>Is the player active?</summary>
        bool isActive = false;
        ///<summary>Is the player dead?</summary>
        bool isDead;

        ///<summary>Returns the is dead state of the player</summary>
        public bool IsDead => isDead;
        ///<summary>Returns the is active state of the player</summary>
        public bool IsActive => isActive;

        protected override void PreAwake()
        {
            SetController(transform.root.GetComponent<Controller>());
            ManagerHub.S.AttachPlayerController(Controller);
        }

        private void Start()
        {
            isActive = true;
            ManagerHub.S.HUDHandler.SetPlayerHealthInfo(this);
        }

        /// <summary>
        /// Call to initiate the attack interaction of the player.
        /// <para>If the player is marked dead then return.</para>
        /// <para>If there are rewind ability uses and the player dies then a rewind sequence gets initiated.</para>
        /// <para>If there are not, then the player dies and gets back to the player hub.</para>
        /// </summary>
        public override void AttackInteraction(int damage)
        {
            //Prevents the player from dying from multiple shots when he's already dead.
            if (entityLife <= 0) return;

            entityLife -= damage;

            ManagerHub.S.HUDHandler.SetPlayerHealth(entityLife);

            //Should check for rewind availability here
            if (entityLife <= 0)
            {
                isDead = true;
                isActive = false;

                UnityAssets.LoadAsync(bloodDecalPath, false, (decal) =>
                {
                    GameObject temp = Instantiate(decal);
                    temp.transform.position = transform.position;
                    temp.transform.rotation = temp.transform.rotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
                });

                StartCoroutine(DeathSequence());
            }
        }

        /// <summary>
        /// Called when the player really dies and moves him back into the player hub after 2 seconds.
        /// </summary>
        /// <returns></returns>
        IEnumerator DeathSequence()
        {
            Controller.Access<PlayerAnimations>("pAnimations").PlayDeathAnimation();
            isActive = false;
            ManagerHub.S.HUDHandler.OpenMessageUI("You are dead!");

            yield return new WaitForSeconds(2f);

            transform.rotation = Quaternion.Euler(Vector3.up);

            yield return null;
        }

        /// <summary>
        /// Subscribed to the onPlayerSpawn event to move the player on the currently loaded scenes' spawn point
        /// when it loads.
        /// </summary>
        void MoveToSpawnPoint(Vector3 spawnPoint)
        { transform.position = spawnPoint; }

        /// <summary>
        /// Call to get the current position of the player.
        /// </summary>
        public Vector3 GetPosition()
        { return transform.position; }

        /// <summary>
        /// Call to set the current position of the player.
        /// <para>If the player is marked dead, then return.</para>
        /// </summary>
        /// <param name="newPos"></param>
        public void SetPosition(Vector3 newPos)
        {
            if (IsDead) return;

            transform.position = newPos;
        }

        /// <summary>
        /// *ANIMATION EVENT*
        /// <para>Called when the wake up animation finishes playing to reactivate the player controls.</para>
        /// </summary>
        public void EnableUserControlOfPlayer()
        {
            //Change after implemented the CoreBehaviour and Controller
            isDead = false;
            isActive = true;

            Controller.Access<PlayerAnimations>("pAnimations").Animator.SetBool("isDead", false);
            Controller.Access<PlayerAnimations>("pAnimations").Animator.SetBool("isWakingUp", false);
        }

        /// <summary>
        /// Increases the player's health value by the passed value 
        /// </summary>
        /// <param name="value">Value to increase the health value by</param>
        /// <returns>The increased player's health</returns>
        public int IncreaseHealthBy(int value)
        {
            maxLife += value;
            entityLife = maxLife;
            return entityLife;
        }
    }
}