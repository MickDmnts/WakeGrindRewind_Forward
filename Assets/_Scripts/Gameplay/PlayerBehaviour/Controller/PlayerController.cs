using System.Collections;
using UnityEngine;
using WGR.Core.Managers;

namespace WGR.Entities.Player
{
    /* [CLASS DOCUMENTATION]
     * 
     * The purpose of this script is to handle the player movement, dashing and rotation to face the mouse.
     * 
     * [Must know]
     * 1. The controller gets activated/deactivated based on the onPlayerStateChange event value.
     * 
     */
    public class PlayerController : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] float speed;
        [SerializeField] float dashSpeed = 30f;
        [SerializeField] TrailRenderer[] dashLines;

        #region PRIVATE_VARIABLES
        bool controllerIsActive = false;

        Rigidbody playerRB;
        RigidbodyConstraints defaultConstraints;
        Vector3 mousePos;

        Vector3 moveDir;
        float horizontalInput;
        float verticalInput;

        //Dash specific
        bool isDashing;
        Vector3 dashDir;
        [SerializeField]
        LayerMask layerMask;
        Vector3 lastMoveDir;
        #endregion

        private void Awake()
        {
            EntrySetup();

            SetDashTrailEmision(false);
        }

        /// <summary>
        /// Call to setup the player controller on first load.
        /// </summary>
        void EntrySetup()
        {
            playerRB = GetComponent<Rigidbody>();
            defaultConstraints = playerRB.constraints;
        }

        private void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange += SetControllerIsActive;

                GameManager.S.GameEventHandler.onPlayerRewind += SetControllerIsActive;
            }
        }

        /// <summary>
        /// *Subscribed to the onPlayerStateChange and onPlayerRewind event.
        /// Call to change the rigidbody constraints based on the passed value.
        /// <para>If the passed value is true, then the starting RB costraints are used,
        /// otherwise the RB freezes every constraint.</para>
        /// </summary>
        void SetControllerIsActive(bool value)
        {
            controllerIsActive = value;

            if (controllerIsActive)
            {
                playerRB.constraints = defaultConstraints;
            }
            else
            {
                playerRB.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        void Update()
        {
            //Dont execute if the controller is inactive.
            if (!controllerIsActive) return;

            //Get the mouse position.
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ApplyRotationBasedOnMousePos();

            //Get the user input.
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            //Get the Dash input
            bool isTryingToDash = Input.GetKeyDown(KeyCode.LeftShift);

            //Enables the dash sequence and dash trails once.
            if (isTryingToDash && !isDashing)
            {
                isDashing = true;
                SetDashTrailEmision(true);
            }
        }

        void FixedUpdate()
        {
            //Dont execute if the controller is inactive.
            if (!controllerIsActive) return;

            //If the user pressed LSHIFT
            if (isDashing)
            {
                Dash();
            }

            //Use the user input and move
            if (!isDashing)
            {
                ApplyMovementVelocity(horizontalInput, verticalInput);
            }
        }

        /// <summary>
        /// Call to dash towards:
        /// <para>The mouse position if the user inputs are 0.</para>
        /// <para>The user input direction.</para>
        /// </summary>
        void Dash()
        {
            dashDir = Vector3.zero;

            //In case we DON'T move
            if (horizontalInput == 0f && verticalInput == 0f)
            {
                Vector3 dashToMouseDir = (mousePos - transform.position);
                dashDir.Set(dashToMouseDir.x, 0f, dashToMouseDir.z);
                lastMoveDir = dashDir;
            }
            else //in case we DO move
            {
                dashDir.Set(horizontalInput, 0, verticalInput);
                lastMoveDir = dashDir;
            }

            //The dash.
            playerRB.AddForce(lastMoveDir.normalized * dashSpeed * Time.deltaTime, ForceMode.Impulse);

            StartCoroutine(DashRefresh());
        }

        /// <summary>
        /// Call to set isDashing to false and trail emision to false after 0.2f seconds.
        /// </summary>
        IEnumerator DashRefresh()
        {
            yield return new WaitForSeconds(0.2f);
            isDashing = false;
            SetDashTrailEmision(false);
            yield return null;
        }

        /// <summary>
        /// Call to move the player RB towards the user input.
        /// <para>Calls ControlIsWalkingAnimation(...) to set the isWalking animation of the player
        /// based on the RB velocity value.</para>
        /// </summary>
        /// <param name="xInput"></param>
        /// <param name="zInput"></param>
        void ApplyMovementVelocity(float xInput, float zInput)
        {
            moveDir.Set(xInput, 0, zInput);

            playerRB.velocity = moveDir.normalized * speed;

            ControlIsWalkingAnimation(playerRB.velocity);
        }

        /// <summary>
        /// Call to set the isWalking animation state value based on the passed value.
        /// <para>If the value is of Vector3 zero then the walking animation is set to false,
        /// otherwise to true.</para>
        /// </summary>
        /// <param name="playerVelocity"></param>
        void ControlIsWalkingAnimation(Vector3 playerVelocity)
        {
            if (playerVelocity != Vector3.zero)
            {
                GameManager.S.PlayerEntity.PlayerAnimations.SetWalkAnimationState(true);
            }
            else
            {
                GameManager.S.PlayerEntity.PlayerAnimations.SetWalkAnimationState(false);
            }
        }

        /// <summary>
        /// Call to rotate the player RB to face the mouse in world position.
        /// </summary>
        void ApplyRotationBasedOnMousePos()
        {
            Vector3 lookDirection = playerRB.position - mousePos;

            float angle = Mathf.Atan2(lookDirection.z, lookDirection.x) * Mathf.Rad2Deg + 90f;
            playerRB.rotation = Quaternion.Euler(0, -angle, 0);
        }

        /// <summary>
        /// Call to set the emit state of every dash trail to the passed value.
        /// </summary>
        void SetDashTrailEmision(bool state)
        {
            foreach (TrailRenderer trail in dashLines)
            {
                trail.emitting = state;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange -= SetControllerIsActive;

                GameManager.S.GameEventHandler.onPlayerRewind -= SetControllerIsActive;
            }
        }
    }
}