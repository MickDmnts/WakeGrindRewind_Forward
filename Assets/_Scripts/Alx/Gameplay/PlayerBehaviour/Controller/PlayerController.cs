using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using WGRF.Core;

namespace WGRF.Player
{
    public class PlayerController : CoreBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] float speed;
        [SerializeField] float dashSpeed = 30f;
        [SerializeField] TrailRenderer[] dashLines;
        [SerializeField] LayerMask layerMask;

        bool controllerIsActive = false;

        Rigidbody playerRB;
        Vector3 mousePos;

        private PlayerInput playerInput = null;
        Vector3 movementVector = Vector3.zero;

        //Dash specific
        bool isDashing;
        Vector3 dashDir;
        Vector3 lastMoveDir;

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

        void SetInputEvents()
        {
            playerInput.Player.Movement.performed += OnMovementPerformed;
            playerInput.Player.LookAt.performed += OnCursorMoved;
            playerInput.Player.Dash.performed += OnDashPerformed;
        }

        private void OnEnable()
        {playerInput.Enable();}

        private void OnDisable()
        {playerInput.Disable();}

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
            Vector3 lookDirection = playerRB.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(lookDirection.z, lookDirection.x) * Mathf.Rad2Deg + 90f;
            playerRB.rotation = Quaternion.Euler(0, -angle, 0);
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
            {Controller.Access<PlayerAnimations>("pAnimations").SetWalkAnimationState(true);}
            else
            {Controller.Access<PlayerAnimations>("pAnimations").SetWalkAnimationState(false);}
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