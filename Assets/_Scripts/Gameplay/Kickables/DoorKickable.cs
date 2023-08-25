using UnityEngine;
using WGR.AI.Entities;
using WGR.Core.Managers;
using WGR.Interactions;

namespace WGR.Entities
{
    /* [CLASS DOCUMENTATION] 
     * 
     * [Variable Specifics]
     * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
     * Dynamically changed: These variables are changed throughout the game.
     * 
     * [Implements] KickableEntity.cs
     */
    public class DoorKickable : KickableEntity
    {
        /// <summary>
        /// The layers the door can exist on.
        /// </summary>
        enum DoorLayers
        {
            Collidable = 14,
            NonCollidable = 2,
        }

        [Header("Set door values in inspector")]
        [SerializeField] bool isLocked;
        [SerializeField] bool neverUnlock;

        //Private variables
        Rigidbody doorRB;
        SpriteRenderer doorRenderer;

        private void Awake()
        {
            doorRB = GetComponent<Rigidbody>();
            doorRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            SetDoorLayer(DoorLayers.Collidable);

            if (isLocked)
            {
                MakeUnmovable();
            }
        }

        /// <summary>
        /// Call to set the door rigidbody to kinematic.
        /// </summary>
        void MakeUnmovable()
        {
            doorRB.isKinematic = true;
        }

        /// <summary>
        /// Call to push the door gameObject to the opposite of the incoming direction and set kicked and canStun to true.
        /// <para>If neverUnlock is set to true or the door is in the NonCollidable DoorLayer nothing happens.</para>
        /// </summary>
        public override void SimulateKnockback(Vector3 incomingDir)
        {
            if (gameObject.layer.Equals(DoorLayers.NonCollidable)) return;

            if (isLocked && !neverUnlock)
            {
                External_UnlockDoor();
            }

            if (!neverUnlock | !isLocked)
            {
                doorRB.gameObject.layer = 2;

                oppositeDir = (transform.position - incomingDir).normalized;

                doorRB.velocity = oppositeDir * kickForce;

                kicked = true;
                canStun = true;

                if (GameManager.S != null)
                {
                    GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.DoorKick);
                }
            }
        }

        protected void Update()
        {
            if (!kicked | !stunOnCollision) return;

            //Deactivates the door stun ability after a short interval.
            if (canStun)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f)
                {
                    canStun = false;

                    SetDoorLayer(DoorLayers.NonCollidable);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!neverUnlock) PlayDoorSoundOnEntityCollision(collision);

            //Call the enemy stun interaction if the door got recently kicked.
            if (canStun)
            {
                IKickable kickable = collision.gameObject.GetComponent<IKickable>();
                if (kickable != null)
                {
                    kickable.SimulateKnockback(transform.position);
                }

                IInteractable interaction = (IInteractable)collision.gameObject.GetComponent<AIEntity>();
                if (interaction != null)
                {
                    interaction.StunInteraction();
                }
            }
        }

        /// <summary>
        /// Call to check if the collision is a player or an enemy and is yes play the door open sound.
        /// </summary>
        /// <param name="collision"></param>
        void PlayDoorSoundOnEntityCollision(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
            {
                if (GameManager.S != null)
                    GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.DoorOpen);
            }
        }

        /// <summary>
        /// Call to set the door gameObject layer to the passed DoorLayer layer.
        /// <para>If the passed layer is the NonCollidable layer then the door is moved to the Entities sortingLayer
        /// so its below every entity.</para>
        /// </summary>
        void SetDoorLayer(DoorLayers layer)
        {
            gameObject.layer = (int)layer;

            if (layer.Equals(DoorLayers.NonCollidable))
            {
                if (doorRenderer != null)
                {
                    doorRenderer.sortingLayerName = "Entities";
                    doorRenderer.sortingOrder = -10;

                    Color doorColor = doorRenderer.color;
                    doorColor.a = 0.5f;

                    doorRenderer.color = doorColor;
                }
            }
        }

        /// <summary>
        /// Call to make the door gameObject interactable and open-able.
        /// </summary>
        public void External_UnlockDoor()
        {
            neverUnlock = false;
            isLocked = false;
            doorRB.isKinematic = false;
        }
    }
}