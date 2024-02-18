using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using WGRF.Core;

namespace WGRF.Player
{
    /// <summary>
    /// The player input controller
    /// </summary>
    public class PlayerController : CoreBehaviour
    {
        ///<summary>The player speed</summary>
        [Header("Set in inspector")]
        [SerializeField, Tooltip("The player speed")] float speed;
        ///<summary>The player dash speed</summary>
        [SerializeField, Tooltip("The player dash speed")] float dashSpeed = 30f;
        ///<summary>The player dash lines</summary>
        [SerializeField, Tooltip("The player dash lines")] TrailRenderer[] dashLines;

        ///<summary>Is the controller active?</summary>
        bool controllerIsActive = false;
        ///<summary>The player rigidbody</summary>
        Rigidbody playerRB;
        ///<summary>The mouse position</summary>
        Vector3 mousePos;
        ///<summary>The player input instance</summary>
        PlayerInput playerInput = null;
        ///<summary>The player movement direction</summary>
        Vector3 movementVector = Vector3.zero;
        ///<summary>Is the player dashing?</summary>
        bool isDashing;
        ///<summary>The dash direction</summary>
        Vector3 dashDir;
        ///<summary>The cached move direction of the player</summary>
        Vector3 lastMoveDir;

        ///<summary>Returns the player inputs instance</summary>
        public PlayerInput PlayerInput => playerInput;

        protected override void PreAwake()
        {
            SetController(transform.root.GetComponent<Controller>());

            playerRB = GetComponent<Rigidbody>();

            SetControllerIsActive(true);

            SetDashTrailEmision(false);

            playerInput = new PlayerInput();
            SetInputEvents();
        }

        ///<summary>Registers the movement input events</summary>
        void SetInputEvents()
        {
            playerInput.Player.Movement.performed += OnMovementPerformed;
            playerInput.Player.LookAt.performed += OnCursorMoved;
            playerInput.Player.Dash.performed += OnDashPerformed;
        }

        private void OnEnable()
        { playerInput.Enable(); }

        private void OnDisable()
        { playerInput.Disable(); }

        /// <summary>
        /// Sets the controller active state
        /// </summary>
        /// <param name="value">The new state</param>
        void SetControllerIsActive(bool value)
        { controllerIsActive = value; }

        void FixedUpdate()
        {
            //Dont execute if the controller is inactive.
            if (!controllerIsActive) return;

            //If the user pressed LSHIFT
            if (isDashing)
            {
                Dash();
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

            if (movementVector.Equals(Vector2.zero))
            {
                Vector3 dashToMouseDir = mousePos - transform.position;
                dashDir.Set(dashToMouseDir.x, 0f, dashToMouseDir.z);
                lastMoveDir = dashDir;
            }
            else
            {
                dashDir.Set(movementVector.x, 0, movementVector.y);
                lastMoveDir = dashDir;
            }

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

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            movementVector = new Vector3(value.ReadValue<Vector2>().x, 0, value.ReadValue<Vector2>().y);
            if (!isDashing)
            { ApplyMovementVelocity(); }
        }

        /// <summary>
        /// Call to move the player RB towards the user input.
        /// </summary>
        void ApplyMovementVelocity()
        {
            playerRB.velocity = movementVector * speed;
            ControlIsWalkingAnimation(playerRB.velocity);
        }

        private void OnCursorMoved(InputAction.CallbackContext value)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000, 9))
            {
                Vector3 dir = hit.point - transform.position;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }

        private void OnDashPerformed(InputAction.CallbackContext value)
        {
            isDashing = true;
            SetDashTrailEmision(true);
        }

        /// <summary>
        /// Call to set the isWalking animation state value based on the passed value.
        /// <para>If the value is of Vector3 zero then the walking animation is set to false,
        /// otherwise to true.</para>
        /// </summary>
        void ControlIsWalkingAnimation(Vector3 playerVelocity)
        {
            if (playerVelocity != Vector3.zero)
            { Controller.Access<PlayerAnimations>("pAnimations").SetWalkAnimationState(true); }
            else
            { Controller.Access<PlayerAnimations>("pAnimations").SetWalkAnimationState(false); }
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

        protected override void PreDestroy()
        {
            playerInput.Player.Movement.performed -= OnMovementPerformed;
            playerInput.Player.LookAt.performed -= OnCursorMoved;
            playerInput.Player.Dash.performed -= OnDashPerformed;
        }
    }
}