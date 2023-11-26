using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using WGRF.Core;

namespace WGRF.Entities.Player
{
    public class PlayerController : CoreBehaviour
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

        private PlayerInput input = null;
        public PlayerInput Input => input;
        Vector2 movementVector = Vector2.zero;

        //Dash specific
        bool isDashing;
        bool isTryingToDash;
        Vector3 dashDir;
        [SerializeField]
        LayerMask layerMask;
        Vector3 lastMoveDir;
        #endregion

        protected override void PreAwake()
        {
            //SetController here
            SetController(transform.GetComponent<Controller>());

            playerRB = GetComponent<Rigidbody>();
            defaultConstraints = playerRB.constraints;

            SetControllerIsActive(true);

            SetDashTrailEmision(false);

            input = new PlayerInput();
        }

        private void OnEnable()
        {
            input.Enable();
            input.Player.Movement.performed += OnMovementPerformed;
            input.Player.Movement.performed += OnMovementCanceled;
            input.Player.LookAt.performed += OnCursorMoved;
            input.Player.Dash.performed += OnDashPerformed;
        }

        private void OnDisable()
        {
            input.Disable();
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.performed -= OnMovementCanceled;
            input.Player.LookAt.performed -= OnCursorMoved;
            input.Player.Dash.performed -= OnDashPerformed;
        }

        private void Start()
        {
            //ManagerHub.S.PlayerEntity.onPlayerStateChange += SetControllerIsActive;
        }

        /// <summary>
        /// 
        /// </summary>
        void SetControllerIsActive(bool value)
        { controllerIsActive = value; }

        void Update()
        {
            //Dont execute if the controller is inactive.
            if (!controllerIsActive) return;

            //Get the mouse position.
            //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            ApplyRotationBasedOnMousePos();

            //Get the Dash input
            //bool isTryingToDash = Input.GetKeyDown(KeyCode.LeftShift);

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
                ApplyMovementVelocity();
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
            if (movementVector.Equals(Vector2.zero))
            {
                Vector3 dashToMouseDir = (mousePos - transform.position);
                dashDir.Set(dashToMouseDir.x, 0f, dashToMouseDir.z);
                lastMoveDir = dashDir;
            }
            else //in case we DO move
            {
                dashDir.Set(movementVector.x, 0, movementVector.y);
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
        void ApplyMovementVelocity()
        {
            playerRB.velocity = movementVector * speed;
            ControlIsWalkingAnimation(playerRB.velocity);
        }

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            movementVector = value.ReadValue<Vector2>();
        }

        private void OnMovementCanceled(InputAction.CallbackContext value)
        {
            movementVector = Vector2.zero;
        }

        private void OnCursorMoved(InputAction.CallbackContext value)
        {
            mousePos = new Vector3(value.ReadValue<Vector2>().x, 0, value.ReadValue<Vector2>().y);
        }

        private void OnDashPerformed(InputAction.CallbackContext value)
        {
            isTryingToDash = value.ReadValue<bool>();
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
                Controller.Access<PlayerAnimations>("playerAnimations").SetWalkAnimationState(true);
            }
            else
            {
                Controller.Access<PlayerAnimations>("playerAnimations").SetWalkAnimationState(false);
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

        protected override void PreDestroy()
        {
            //ManagerHub.S.PlayerEntity.onPlayerStateChange -= SetControllerIsActive;
        }
    }
}