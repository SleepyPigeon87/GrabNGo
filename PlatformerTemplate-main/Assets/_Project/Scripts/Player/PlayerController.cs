using UnityEngine;
using Platformer.Config;
using Platformer.Core;

namespace Platformer.Player
{
    /*
     * ============================================================================
     * PLAYER CONTROLLER
     * ============================================================================
     *
     * This is the CORE of your platformer. It handles:
     *
     *   - Horizontal movement (acceleration, deceleration, air control)
     *   - Jumping (variable height, coyote time, gravity scaling)
     *   - Ground detection (raycast-based)
     *
     * ARCHITECTURE:
     *
     *   InputReader (reads hardware) --> PlayerController (physics/movement)
     *                                          |
     *                                          v
     *                                    MovementConfig (tunable values)
     *
     * PlayerController does NOT read input directly. It asks InputReader for
     * the processed input values. This separation means:
     *
     *   - Input handling is isolated (device quirks stay in InputReader)
     *   - Movement logic can be tested with fake input
     *   - Same movement code works regardless of input device
     *
     * PHYSICS APPROACH:
     *
     * We use Rigidbody2D for physics but control velocity directly rather than
     * using AddForce. This gives us precise control over the feel:
     *
     *   AddForce approach: "Apply 10 Newtons of force, let physics figure it out"
     *   Direct velocity: "The player should be moving at exactly 8 units/second"
     *
     * Direct velocity control is standard for tight platformers (Celeste, Hollow
     * Knight, Dead Cells all do this).
     *
     * ============================================================================
     */

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        /*
         * ------------------------------------------------------------------------
         * CONFIGURATION
         * ------------------------------------------------------------------------
         */

        [Header("Config")]
        [Tooltip("Movement configuration ScriptableObject. Create via: " +
                 "Right-click > Create > Platformer > Movement Config")]
        [SerializeField] private MovementConfig config;

        [Header("Ground Check")]
        [Tooltip("Transform at the player's feet for ground detection. " +
                 "Create an empty child GameObject at the bottom of the player.")]
        [SerializeField] private Transform groundCheckPoint;

        /*
         * ------------------------------------------------------------------------
         * RUNTIME STATE
         * ------------------------------------------------------------------------
         *
         * These values change every frame based on physics and input.
         * Exposed as properties so other systems can read them (UI, animations).
         */

        /// <summary>
        /// True if the player is touching ground.
        /// </summary>
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// Current horizontal velocity.
        /// </summary>
        public float HorizontalSpeed => rb.linearVelocity.x;

        /// <summary>
        /// Current vertical velocity. Positive = rising, negative = falling.
        /// </summary>
        public float VerticalSpeed => rb.linearVelocity.y;

        /*
         * ------------------------------------------------------------------------
         * INTERNAL STATE
         * ------------------------------------------------------------------------
         */

        // Component references (cached for performance)
        private Rigidbody2D rb;
        private InputReader inputReader;

        // Coyote time tracking
        private float lastGroundedTime;
        private bool hasJumpedSinceGrounded;

        // Jump state
        private bool isJumping;

        /*
         * ------------------------------------------------------------------------
         * UNITY LIFECYCLE
         * ------------------------------------------------------------------------
         */

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            // Configure Rigidbody2D for platformer physics
            rb.freezeRotation = true;  // Don't let physics rotate the player
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // Better collision at high speeds
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;  // Smooth visual movement
        }

        private void Start()
        {
            // Get InputReader from ServiceLocator
            inputReader = ServiceLocator.Get<InputReader>();

            if (inputReader == null)
            {
                Debug.LogError("[PlayerController] InputReader not found! " +
                               "Make sure InputReader is in the scene and initializes before PlayerController.", this);
            }

            if (config == null)
            {
                Debug.LogError("[PlayerController] MovementConfig not assigned! " +
                               "Create a Movement Config asset and assign it in the Inspector.", this);
            }

            if (groundCheckPoint == null)
            {
                Debug.LogWarning("[PlayerController] Ground Check Point not assigned! " +
                                 "Create an empty GameObject at the player's feet and assign it.", this);
            }
        }

        /*
         * ------------------------------------------------------------------------
         * PHYSICS UPDATE
         * ------------------------------------------------------------------------
         *
         * CRITICAL: All physics/movement code goes in FixedUpdate, not Update.
         *
         * FixedUpdate runs at a fixed rate (default 50/second) regardless of
         * framerate. Physics calculations need this consistency because:
         *
         *   - Forces applied in Update would be stronger at lower framerates
         *   - Collision detection timing would be inconsistent
         *   - Physics behavior would change based on computer speed
         *
         * Rule of thumb:
         *   Input reading -> Update (maximum responsiveness)
         *   Physics/movement -> FixedUpdate (consistent behavior)
         */

        private void FixedUpdate()
        {
            if (config == null || inputReader == null) return;

            // Order matters! Ground check first, then movement, then jump
            UpdateGroundedState();
            UpdateHorizontalMovement();
            UpdateJump();
            UpdateGravityScale();
        }

        /*
         * ------------------------------------------------------------------------
         * GROUND DETECTION
         * ------------------------------------------------------------------------
         *
         * HOW IT WORKS:
         * We cast a short ray downward from the player's feet. If it hits
         * something on the Ground layer, we're grounded.
         *
         * WHY RAYCAST (not collision events)?
         *   - More control over what counts as "ground"
         *   - Can detect slopes and get surface normal
         *   - Works regardless of collider shape
         *   - OnCollisionEnter/Exit can be unreliable on edges
         *
         * COYOTE TIME:
         * We track when the player was last grounded. If they walk off a ledge,
         * they have a brief window (coyoteTime) where they can still jump.
         * This makes platforming feel forgiving.
         *
         * Why "coyote time"? Named after cartoon coyotes who don't fall until
         * they look down. The player hasn't "looked down" yet, so they can jump.
         */

        private void UpdateGroundedState()
        {
            bool wasGrounded = IsGrounded;

            // Perform ground check
            IsGrounded = CheckGrounded();

            // Track coyote time
            if (IsGrounded)
            {
                lastGroundedTime = Time.time;

                // Reset jump state when landing
                if (!wasGrounded)
                {
                    hasJumpedSinceGrounded = false;
                    isJumping = false;
                }
            }
        }

        private bool CheckGrounded()
        {
            if (groundCheckPoint == null) return false;

            // Cast a ray downward from the ground check point
            RaycastHit2D hit = Physics2D.Raycast(
                groundCheckPoint.position,
                Vector2.down,
                config.groundCheckDistance,
                config.groundLayer
            );

            return hit.collider != null;
        }

        /// <summary>
        /// True if the player can jump (grounded or within coyote time).
        /// </summary>
        private bool CanJump()
        {
            // Already used the jump since last grounded
            if (hasJumpedSinceGrounded) return false;

            // Currently grounded
            if (IsGrounded) return true;

            // Within coyote time window
            float timeSinceGrounded = Time.time - lastGroundedTime;
            if (timeSinceGrounded <= config.coyoteTime) return true;

            return false;
        }

        /*
         * ------------------------------------------------------------------------
         * HORIZONTAL MOVEMENT
         * ------------------------------------------------------------------------
         *
         * THE FORMULA:
         *
         * This is acceleration-based movement. Instead of instantly setting
         * velocity to maxSpeed * input, we gradually change velocity over time.
         *
         *   targetVelocity = input * maxSpeed
         *   currentVelocity moves toward targetVelocity by (acceleration * deltaTime)
         *
         * ACCELERATION vs DECELERATION:
         * When input matches velocity direction: use acceleration
         * When no input: use deceleration
         * When input opposes velocity: use deceleration * turnAroundMultiplier
         *
         * This gives three distinct "feels":
         *   - Starting to move: acceleration
         *   - Stopping: deceleration
         *   - Changing direction: faster deceleration (snappy turnaround)
         *
         * AIR CONTROL:
         * In the air, we multiply acceleration by airControlMultiplier.
         * This makes air movement less responsive than ground movement,
         * which most players expect (even if they can't articulate it).
         */

        private void UpdateHorizontalMovement()
        {
            // Get input from InputReader (already processed with deadzone)
            float inputX = inputReader.MoveInput.x;

            // Calculate target velocity
            float targetVelocity = inputX * config.maxSpeed;

            // Get current velocity
            float currentVelocity = rb.linearVelocity.x;

            // Determine which rate to use
            float rate = CalculateMoveRate(inputX, currentVelocity);

            // Apply air control multiplier if airborne
            if (!IsGrounded)
            {
                rate *= config.airControlMultiplier;
            }

            // Move toward target velocity
            float newVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);

            // Apply to rigidbody (preserve Y velocity)
            rb.linearVelocity = new Vector2(newVelocity, rb.linearVelocity.y);
        }

        private float CalculateMoveRate(float input, float currentVelocity)
        {
            // No input = decelerate
            if (Mathf.Abs(input) < 0.01f)
            {
                return config.deceleration;
            }

            // Check if input opposes current velocity (turning around)
            bool isTurningAround = (input > 0 && currentVelocity < -0.1f) ||
                                   (input < 0 && currentVelocity > 0.1f);

            if (isTurningAround)
            {
                // Turning around = faster deceleration
                return config.deceleration * config.turnAroundMultiplier;
            }

            // Normal acceleration
            return config.acceleration;
        }

        /*
         * ------------------------------------------------------------------------
         * JUMPING
         * ------------------------------------------------------------------------
         *
         * VARIABLE JUMP HEIGHT:
         *
         * When the player releases jump while rising, we cut their upward velocity.
         * This means:
         *   - Tap jump = short hop
         *   - Hold jump = full jump
         *
         * This is a standard platformer feature. It gives players fine control
         * over jump height, which is essential for precision platforming.
         *
         * IMPLEMENTATION:
         * 1. On jump press: Set Y velocity to jumpForce (if CanJump())
         * 2. While rising AND jump not held: Multiply Y velocity by jumpCutMultiplier
         *
         * The "while rising" check is important - we only cut the jump on the
         * way UP. Cutting velocity while falling would feel wrong.
         */

        private void UpdateJump()
        {
            // Check for buffered jump input
            if (inputReader.JumpBuffered && CanJump())
            {
                ExecuteJump();
                inputReader.ConsumeJumpBuffer();
                return;
            }

            // Variable jump height: cut velocity if jump released while rising
            if (isJumping && !inputReader.JumpHeld && rb.linearVelocity.y > 0)
            {
                // Cut the upward velocity
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * config.jumpCutMultiplier);
                isJumping = false;  // Only apply cut once
            }
        }

        private void ExecuteJump()
        {
            // Set vertical velocity directly (not AddForce)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, config.jumpForce);

            // Track jump state
            isJumping = true;
            hasJumpedSinceGrounded = true;
        }

        /*
         * ------------------------------------------------------------------------
         * GRAVITY SCALING
         * ------------------------------------------------------------------------
         *
         * THE PROBLEM:
         * Realistic gravity makes jumps feel "floaty". Players expect to fall
         * faster than they rise - it feels more responsive and satisfying.
         *
         * THE SOLUTION:
         * Scale gravity based on whether we're rising or falling:
         *   - Rising (velocity.y > 0): Normal gravity
         *   - Falling (velocity.y < 0): Increased gravity
         *
         * This creates the snappy, responsive jump that players expect from
         * platformers, even though it's physically unrealistic.
         *
         * NUMBERS:
         * gravityScale = 1.0, fallGravityMultiplier = 1.6 means:
         *   - Rising: 1.0x gravity
         *   - Falling: 1.6x gravity (60% faster fall)
         */

        private void UpdateGravityScale()
        {
            if (rb.linearVelocity.y < 0)
            {
                // Falling - apply increased gravity
                rb.gravityScale = config.gravityScale * config.fallGravityMultiplier;
            }
            else
            {
                // Rising or grounded - normal gravity
                rb.gravityScale = config.gravityScale;
            }
        }

        /*
         * ------------------------------------------------------------------------
         * DEBUG VISUALIZATION
         * ------------------------------------------------------------------------
         */

        private void OnDrawGizmosSelected()
        {
            if (groundCheckPoint == null || config == null) return;

            // Draw ground check ray
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(
                groundCheckPoint.position,
                groundCheckPoint.position + Vector3.down * config.groundCheckDistance
            );

            // Draw ground check point
            Gizmos.DrawWireSphere(groundCheckPoint.position, 0.05f);
        }
    }
}
