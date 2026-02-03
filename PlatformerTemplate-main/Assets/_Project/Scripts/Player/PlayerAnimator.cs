using UnityEngine;
using Platformer.Core;

namespace Platformer.Player
{
    /*
     * ============================================================================
     * PLAYER ANIMATOR
     * ============================================================================
     *
     * This script connects gameplay state to the Animator Controller.
     *
     * HOW IT WORKS:
     *
     *   PlayerController (gameplay state) --> PlayerAnimator --> Animator Controller
     *          |                                    |
     *          v                                    v
     *   "I'm grounded, moving at 5 m/s"    "Set IsGrounded=true, Speed=5"
     *
     * The Animator Controller uses these parameters to determine which animation
     * to play via transitions you set up in the Animator window.
     *
     * WHY CACHE PARAMETER HASHES?
     *
     * Animator.SetFloat("Speed", value) looks up "Speed" by string every call.
     * Animator.SetFloat(speedHash, value) uses a cached integer ID - much faster.
     *
     * This optimization matters because we call these every frame.
     *
     * ANIMATOR PARAMETERS:
     *
     * This script expects the following parameters in your Animator Controller:
     *
     *   | Name             | Type  | Description                           |
     *   |------------------|-------|---------------------------------------|
     *   | HorizontalSpeed  | Float | Absolute horizontal velocity          |
     *   | VerticalSpeed    | Float | Vertical velocity (+ up, - down)      |
     *   | IsGrounded       | Bool  | True when touching ground             |
     *   | IsJumping        | Bool  | True during jump rise                 |
     *   | IsFalling        | Bool  | True when falling                     |
     *
     * Add more parameters as you add mechanics (IsDashing, IsWallSliding, etc.)
     *
     * ============================================================================
     */

    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        /*
         * ------------------------------------------------------------------------
         * CACHED REFERENCES
         * ------------------------------------------------------------------------
         */

        private Animator animator;
        private PlayerController playerController;

        /*
         * ------------------------------------------------------------------------
         * PARAMETER HASHES
         * ------------------------------------------------------------------------
         *
         * These are cached IDs for animator parameters.
         * Using Animator.StringToHash("Name") once is faster than passing
         * string literals every frame.
         */

        private static readonly int HorizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
        private static readonly int IsFallingHash = Animator.StringToHash("IsFalling");

        /*
         * ------------------------------------------------------------------------
         * UNITY LIFECYCLE
         * ------------------------------------------------------------------------
         */

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Get PlayerController - could be on this object or a parent
            playerController = GetComponentInParent<PlayerController>();

            if (playerController == null)
            {
                Debug.LogError("[PlayerAnimator] PlayerController not found! " +
                               "PlayerAnimator must be on the same GameObject as PlayerController " +
                               "or on a child object.", this);
            }
        }

        /*
         * ------------------------------------------------------------------------
         * UPDATE ANIMATOR PARAMETERS
         * ------------------------------------------------------------------------
         *
         * We update in LateUpdate so all gameplay logic has finished.
         * This ensures we're animating the final state, not an intermediate one.
         */

        private void LateUpdate()
        {
            if (playerController == null || animator == null) return;

            UpdateMovementParameters();
            UpdateStateParameters();
        }

        /// <summary>
        /// Updates speed-related animator parameters.
        /// </summary>
        private void UpdateMovementParameters()
        {
            // HorizontalSpeed: Absolute value so animations don't care about direction
            // (Sprite flipping handles facing direction)
            float horizontalSpeed = Mathf.Abs(playerController.HorizontalSpeed);
            animator.SetFloat(HorizontalSpeedHash, horizontalSpeed);

            // VerticalSpeed: Signed value so we can distinguish rising vs falling
            animator.SetFloat(VerticalSpeedHash, playerController.VerticalSpeed);
        }

        /// <summary>
        /// Updates state-related animator parameters.
        /// </summary>
        private void UpdateStateParameters()
        {
            // Ground state
            animator.SetBool(IsGroundedHash, playerController.IsGrounded);

            // Jump/Fall states (derived from velocity and ground state)
            bool isRising = playerController.VerticalSpeed > 0.1f;
            bool isFalling = playerController.VerticalSpeed < -0.1f && !playerController.IsGrounded;

            animator.SetBool(IsJumpingHash, isRising);
            animator.SetBool(IsFallingHash, isFalling);
        }

        /*
         * ------------------------------------------------------------------------
         * SPRITE FLIPPING
         * ------------------------------------------------------------------------
         *
         * When moving left, flip the sprite horizontally.
         * We do this here rather than in PlayerController because it's purely visual.
         */

        private void Update()
        {
            if (playerController == null) return;

            UpdateFacingDirection();
        }

        private void UpdateFacingDirection()
        {
            float horizontalSpeed = playerController.HorizontalSpeed;

            // Only flip if we have significant horizontal movement
            if (Mathf.Abs(horizontalSpeed) > 0.1f)
            {
                // Flip by scaling X negative (preserves collider orientation)
                Vector3 scale = transform.localScale;
                scale.x = horizontalSpeed > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }

        /*
         * ------------------------------------------------------------------------
         * EXTENDING FOR NEW MECHANICS
         * ------------------------------------------------------------------------
         *
         * When you add a new mechanic (Dash, Wall Slide, etc.), follow this pattern:
         *
         * 1. Add a new hash:
         *    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");
         *
         * 2. Get a reference to the mechanic controller:
         *    private DashController dashController;
         *    (get it in Start())
         *
         * 3. Update the parameter in LateUpdate:
         *    animator.SetBool(IsDashingHash, dashController.IsDashing);
         *
         * 4. Add the parameter to your Animator Controller in Unity.
         *
         * 5. Set up transitions that use the new parameter.
         */
    }
}
