using UnityEngine;

namespace Platformer.Config
{
    /*
     * ============================================================================
     * INPUT CONFIGURATION
     * ============================================================================
     *
     * This ScriptableObject holds all the tunable values for input handling.
     * Separating config from code means:
     *   - Designers can tune without touching code
     *   - You can create multiple presets (responsive, floaty, accessible)
     *   - Values are saved in an asset file, not lost when scripts recompile
     *
     * CREATE AN INSTANCE:
     *   Right-click in Project window > Create > Platformer > Input Config
     *
     * ============================================================================
     */

    [CreateAssetMenu(fileName = "InputConfig", menuName = "Platformer/Input Config")]
    public class InputConfig : ScriptableObject
    {
        /*
         * ------------------------------------------------------------------------
         * DEADZONE SETTINGS
         * ------------------------------------------------------------------------
         *
         * WHY DEADZONES MATTER:
         * Analog sticks are physical hardware. They drift, they don't center
         * perfectly, and tiny movements happen even when the player isn't touching
         * the stick. Without a deadzone, your character might slowly drift or
         * vibrate in place.
         *
         * DEADZONE SHAPES:
         *
         * CIRCULAR (recommended for most games):
         *   Creates a circular "ignore zone" in the center of stick travel.
         *   If stick magnitude < deadzone, input = zero.
         *   Feels natural because diagonal movements work consistently.
         *
         *       +---+---+---+
         *       |   | ^ |   |
         *       +---+   +---+
         *       | <   O   > |   <-- O is the deadzone circle
         *       +---+   +---+
         *       |   | v |   |
         *       +---+---+---+
         *
         * AXIAL (per-axis deadzone):
         *   Applies deadzone separately to X and Y axes.
         *   Can feel "sticky" on diagonals because X and Y snap independently.
         *   Sometimes preferred for games needing precise cardinal directions.
         *
         * We use CIRCULAR in this template.
         */

        [Header("Deadzone")]
        [Tooltip("Stick values below this are treated as zero. Prevents drift. " +
                 "0.1-0.2 is typical. Higher = more deadzone, less sensitivity.")]
        [Range(0.05f, 0.4f)]
        public float deadzone = 0.15f;

        /*
         * ------------------------------------------------------------------------
         * INPUT BUFFERING
         * ------------------------------------------------------------------------
         *
         * THE PROBLEM:
         * Player presses jump 2 frames before landing. Without buffering, the
         * input is lost because they weren't grounded when they pressed it.
         * Player thinks: "I pressed jump! This game is broken!"
         *
         * THE SOLUTION:
         * Buffer inputs for a short window. If the player presses jump and THEN
         * becomes grounded within the buffer window, execute the jump.
         *
         * BUFFER WINDOW TUNING:
         *   Too short (< 0.05s): Inputs still feel dropped
         *   Good (0.1-0.15s): Responsive, forgiving, still requires timing
         *   Too long (> 0.2s): Game plays itself, removes skill expression
         *
         * This is one of the "invisible" systems that makes controls feel good.
         * Players never notice it working, but they WILL notice it missing.
         */

        [Header("Input Buffering")]
        [Tooltip("How long to remember a jump press. If the player lands within " +
                 "this window after pressing jump, the jump executes. 0.1s is a good start.")]
        [Range(0f, 0.3f)]
        public float jumpBufferDuration = 0.1f;

        [Tooltip("How long to remember a dash press.")]
        [Range(0f, 0.3f)]
        public float dashBufferDuration = 0.08f;

        /*
         * ------------------------------------------------------------------------
         * KEYBOARD VS CONTROLLER
         * ------------------------------------------------------------------------
         *
         * THE CHALLENGE:
         * Keyboard input is DIGITAL: 0 or 1, nothing in between.
         * Controller stick is ANALOG: smooth range from 0 to 1.
         *
         * If you treat keyboard the same as "stick fully pushed," the character
         * instantly moves at max speed. This can feel too snappy or too slow
         * depending on your acceleration settings.
         *
         * APPROACH:
         * We convert keyboard input to a "virtual analog" value. This lets
         * acceleration curves work consistently across input devices.
         *
         * keyboardAnalogValue = 1.0 means keyboard acts like stick fully pushed.
         * Lower values make keyboard feel like a partial stick push.
         * Most games use 1.0 and tune acceleration to feel good on both.
         */

        [Header("Keyboard Settings")]
        [Tooltip("What analog value keyboard input maps to. 1.0 = full speed. " +
                 "Lower values make keyboard movement slower/softer.")]
        [Range(0.5f, 1f)]
        public float keyboardAnalogValue = 1f;
    }
}
