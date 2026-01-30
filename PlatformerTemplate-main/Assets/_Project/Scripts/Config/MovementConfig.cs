using UnityEngine;

namespace Platformer.Config
{
    /*
     * ============================================================================
     * MOVEMENT CONFIGURATION
     * ============================================================================
     *
     * All the numbers that control how movement FEELS.
     *
     * GAME FEEL BASICS:
     * The difference between "tight" and "floaty" controls is mostly in these
     * values. Same code, different config = completely different feel.
     *
     * TUNING WORKFLOW:
     * 1. Start with defaults
     * 2. Play for 30 seconds
     * 3. Identify what feels wrong ("too slow", "can't stop", "too floaty")
     * 4. Adjust ONE value
     * 5. Repeat
     *
     * Don't change multiple values at once. You won't know what fixed it.
     *
     * CREATE AN INSTANCE:
     *   Right-click in Project window > Create > Platformer > Movement Config
     *
     * ============================================================================
     */

    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Platformer/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        /*
         * ------------------------------------------------------------------------
         * HORIZONTAL MOVEMENT
         * ------------------------------------------------------------------------
         *
         * maxSpeed: How fast the player CAN move (units per second)
         *   Low (4-6): Slow, methodical, precision platformer
         *   Medium (8-10): Standard platformer feel
         *   High (12+): Fast, action-focused
         *
         * acceleration: How fast the player REACHES max speed
         *   Low (20-30): Slow buildup, feels heavy/weighty
         *   Medium (40-60): Responsive but not instant
         *   High (80+): Near-instant, very snappy
         *
         * deceleration: How fast the player STOPS when releasing input
         *   Low (20-30): Slides, momentum-based (ice physics territory)
         *   Medium (40-60): Stops fairly quickly
         *   High (80+): Stops almost instantly, very tight control
         *
         * RELATIONSHIP:
         * If acceleration > deceleration: Player feels like they're on ice
         * If deceleration > acceleration: Player stops faster than they start
         * If acceleration == deceleration: Symmetric feel (common choice)
         */

        [Header("Horizontal Movement")]
        [Tooltip("Maximum horizontal speed in units per second.")]
        [Range(1f, 20f)]
        public float maxSpeed = 8f;

        [Tooltip("How quickly the player reaches max speed. Higher = snappier.")]
        [Range(10f, 200f)]
        public float acceleration = 60f;

        [Tooltip("How quickly the player stops when not pressing movement. Higher = tighter stops.")]
        [Range(10f, 200f)]
        public float deceleration = 60f;

        /*
         * ------------------------------------------------------------------------
         * TURN AROUND BOOST
         * ------------------------------------------------------------------------
         *
         * When the player is moving right and presses left, what happens?
         *
         * Without boost: Player decelerates to zero, then accelerates left.
         *   Feels sluggish, like wading through mud.
         *
         * With boost: Player decelerates FASTER, snapping to the new direction.
         *   Feels responsive, like the character is athletic and nimble.
         *
         * This multiplier is applied to deceleration when input opposes velocity.
         *   1.0 = no boost (same as normal deceleration)
         *   1.5 = 50% faster turnaround
         *   2.0 = twice as fast turnaround
         *
         * Celeste uses aggressive turn boost. Mario uses moderate.
         */

        [Header("Turn Around")]
        [Tooltip("Multiplier on deceleration when changing direction. Higher = snappier turns.")]
        [Range(1f, 3f)]
        public float turnAroundMultiplier = 1.5f;

        /*
         * ------------------------------------------------------------------------
         * AIR CONTROL
         * ------------------------------------------------------------------------
         *
         * How much can the player steer while airborne?
         *
         * 0.0 = No air control. Jump trajectory is fixed. (Realistic but frustrating)
         * 0.5 = Half control. Can adjust but committed to general direction.
         * 1.0 = Full control. Air feels same as ground. (Common in platformers)
         *
         * Most platformers use 0.8-1.0 because it feels good, even if unrealistic.
         * Lower values are used for "commitment" mechanics (you must aim your jump).
         */

        [Header("Air Control")]
        [Tooltip("How much horizontal control in the air. 0 = none, 1 = full ground control.")]
        [Range(0f, 1f)]
        public float airControlMultiplier = 0.85f;

        /*
         * ------------------------------------------------------------------------
         * JUMP PARAMETERS
         * ------------------------------------------------------------------------
         *
         * jumpForce: Initial upward velocity when jumping
         *   This determines jump HEIGHT, but gravity affects it too.
         *   Tuning tip: Set gravity first, then adjust jumpForce to get desired height.
         *
         * jumpCutMultiplier: What happens when player releases jump early?
         *   When player releases jump button while rising, we multiply upward
         *   velocity by this value. Lower = more height control.
         *   0.5 = releasing early cuts jump to ~half height
         *   1.0 = no variable jump height (holding doesn't matter)
         *
         * coyoteTime: Grace period after leaving a platform
         *   Player walks off edge, has this many seconds to still jump.
         *   Makes platforming feel forgiving without being visible.
         *   0.1-0.15s is standard. 0 = no forgiveness (hard mode).
         */

        [Header("Jumping")]
        [Tooltip("Upward velocity applied when jumping.")]
        [Range(5f, 25f)]
        public float jumpForce = 14f;

        [Tooltip("Velocity multiplier when releasing jump early. Lower = more height control.")]
        [Range(0.1f, 1f)]
        public float jumpCutMultiplier = 0.5f;

        [Tooltip("Seconds after leaving ground where jump still works.")]
        [Range(0f, 0.3f)]
        public float coyoteTime = 0.12f;

        /*
         * ------------------------------------------------------------------------
         * GRAVITY SCALING
         * ------------------------------------------------------------------------
         *
         * Unity's Rigidbody2D has gravity, but one gravity value for the whole
         * jump feels wrong. Players expect:
         *   - Rising: Floaty apex (hang time at top of jump)
         *   - Falling: Fast descent (get back to ground quickly)
         *
         * We scale gravity differently based on vertical velocity:
         *
         * gravityScale: Base gravity multiplier (1 = Unity default)
         * fallGravityMultiplier: Extra gravity when falling
         * apexGravityMultiplier: Reduced gravity near apex (optional advanced feel)
         *
         * COMMON SETUP:
         *   gravityScale = 1
         *   fallGravityMultiplier = 1.5-2.0 (fall faster than rise)
         *
         * This creates the "game feel" gravity that players expect but can't
         * articulate. Real physics would use 1.0 for both, but it feels floaty.
         */

        [Header("Gravity")]
        [Tooltip("Base gravity scale. 1 = Unity default. Higher = heavier.")]
        [Range(0.5f, 3f)]
        public float gravityScale = 1f;

        [Tooltip("Gravity multiplier when falling. Higher = faster fall.")]
        [Range(1f, 3f)]
        public float fallGravityMultiplier = 1.6f;

        /*
         * ------------------------------------------------------------------------
         * GROUND DETECTION
         * ------------------------------------------------------------------------
         *
         * How do we know if the player is on the ground?
         *
         * We cast a short ray (or box) downward from the player's feet.
         * If it hits something on the Ground layer, we're grounded.
         *
         * groundCheckDistance: How far down to check
         *   Too short: Player "ungrounds" on tiny bumps
         *   Too long: Player can jump while clearly in the air
         *   Sweet spot: Just past the collider's edge (0.05-0.1)
         *
         * groundLayer: Which physics layers count as ground
         *   Always use layers, not tags. Layers are faster and cleaner.
         */

        [Header("Ground Detection")]
        [Tooltip("How far below the player to check for ground.")]
        [Range(0.01f, 0.5f)]
        public float groundCheckDistance = 0.1f;

        [Tooltip("Which layers count as ground. Set this in the Inspector!")]
        public LayerMask groundLayer;
    }
}
