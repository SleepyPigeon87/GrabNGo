using UnityEngine;
using UnityEngine.InputSystem;
using Platformer.Config;
using Platformer.Core;

namespace Platformer.Player
{
    /*
     * ============================================================================
     * INPUT READER
     * ============================================================================
     *
     * This script is the BRIDGE between hardware (keyboard, controller) and
     * game logic (PlayerController). It handles:
     *
     *   1. Reading raw input from Unity's Input System
     *   2. Processing analog values (deadzones, normalization)
     *   3. Tracking device type (keyboard vs controller)
     *   4. Buffering inputs for forgiving gameplay
     *
     * NOTE: This class is named "InputReader" (not "PlayerInput") to avoid
     * collision with Unity's built-in PlayerInput component from the Input System.
     *
     * WHY SEPARATE INPUT FROM CONTROLLER?
     *
     * Separation of concerns. PlayerController handles physics and movement.
     * InputReader handles the messy reality of hardware. This means:
     *
     *   - You can test PlayerController with fake input
     *   - Input bugs don't break movement code
     *   - Controller-specific features stay contained here
     *   - Easier to add input replay/recording later
     *
     * UNITY INPUT SYSTEM BASICS
     *
     * The new Input System uses these concepts:
     *
     *   InputActionAsset: A file containing all your input bindings
     *   ActionMap: A group of actions (e.g., "Player", "UI", "Vehicle")
     *   Action: A single input like "Move" or "Jump"
     *   Binding: A physical input mapped to an action (e.g., "Space" -> "Jump")
     *
     * We reference actions by name: inputActions.FindAction("Move")
     *
     * IMPORTANT: You must create the InputActionAsset and assign it in the
     * Inspector. See the setup guide for step-by-step instructions.
     *
     * ============================================================================
     */

    public class InputReader : MonoBehaviour
    {
        /*
         * ------------------------------------------------------------------------
         * CONFIGURATION
         * ------------------------------------------------------------------------
         */

        [Header("Input Asset")]
        [Tooltip("The InputActionAsset containing your Player action map. " +
                 "Create one via: Right-click > Create > Input Actions")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Config")]
        [Tooltip("Input configuration ScriptableObject for deadzones and buffering.")]
        [SerializeField] private InputConfig config;

        /*
         * ------------------------------------------------------------------------
         * INPUT STATE (Read by PlayerController)
         * ------------------------------------------------------------------------
         *
         * These are the "processed" values that PlayerController reads.
         * They're already cleaned up (deadzone applied, buffering handled).
         */

        /// <summary>
        /// Processed movement input. X is horizontal (-1 to 1), Y is vertical.
        /// Deadzone has already been applied. Safe to use directly.
        /// </summary>
        public Vector2 MoveInput { get; private set; }

        /// <summary>
        /// True if a jump input is buffered and waiting to be consumed.
        /// Call ConsumeJumpBuffer() when you execute the jump.
        /// </summary>
        public bool JumpBuffered => jumpBufferTimer > 0f;

        /// <summary>
        /// True while the jump button is held down.
        /// Used for variable jump height (release early = lower jump).
        /// </summary>
        public bool JumpHeld { get; private set; }

        /// <summary>
        /// True if currently using a gamepad. False if keyboard/mouse.
        /// Useful for showing correct button prompts.
        /// </summary>
        public bool UsingGamepad { get; private set; }
        public bool GrappleHeld { get; private set; }
        public Vector2 AimInput { get; private set; }
        /*
         * ------------------------------------------------------------------------
         * INTERNAL STATE
         * ------------------------------------------------------------------------
         */

        // Input System action references
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction grappleAction;
        private InputAction aimAction;  
        // Input buffering timers
        private float jumpBufferTimer;

        // Raw input before processing (for debugging)
        private Vector2 rawMoveInput;

        /*
         * ------------------------------------------------------------------------
         * UNITY LIFECYCLE
         * ------------------------------------------------------------------------
         */

        private void Awake()
        {
            // Register with ServiceLocator so other scripts can access us
            ServiceLocator.Register<InputReader>(this);

            // Find our input actions
            SetupInputActions();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<InputReader>();
            CleanupInputActions();
        }

        private void OnEnable()
        {
            EnableInputActions();
        }

        private void OnDisable()
        {
            DisableInputActions();
        }

        private void Update()
        {
            // Read and process movement input every frame
            ProcessMoveInput();
            ProcessAimInput();

            // Tick down input buffers
            UpdateBufferTimers();
        }

        /*
         * ------------------------------------------------------------------------
         * INPUT SETUP
         * ------------------------------------------------------------------------
         *
         * Unity's Input System requires you to:
         *   1. Find the action references
         *   2. Enable them when active
         *   3. Subscribe to button events
         *   4. Disable and unsubscribe when done
         *
         * Missing any step = input doesn't work or memory leaks.
         */

        private void SetupInputActions()
        {
            if (inputActions == null)
            {
                Debug.LogError("[InputReader] InputActionAsset not assigned! " +
                               "Drag your Input Actions asset into the Inspector.", this);
                return;
            }

            // Find actions by name in the "Player" action map
            // These names must match what you defined in the InputActionAsset
            var playerMap = inputActions.FindActionMap("Player");

            if (playerMap == null)
            {
                Debug.LogError("[InputReader] 'Player' action map not found in InputActionAsset! " +
                               "Make sure you have an action map named 'Player'.", this);
                return;
            }

            moveAction = playerMap.FindAction("Move");
            jumpAction = playerMap.FindAction("Jump");
            grappleAction = playerMap.FindAction("Grapple");
            aimAction = playerMap.FindAction("Aim");

            if (moveAction == null)
                Debug.LogError("[InputReader] 'Move' action not found in Player map!", this);
            if (jumpAction == null)
                Debug.LogError("[InputReader] 'Jump' action not found in Player map!", this);
        }

        private void EnableInputActions()
        {
            if (moveAction == null) return;

            // Enable actions so they receive input
            moveAction.Enable();
            jumpAction.Enable();
            if (grappleAction != null)grappleAction.Enable();
            if (aimAction != null)  aimAction.Enable();
            // Subscribe to jump button events
            // "performed" = button pressed, "canceled" = button released
            jumpAction.performed += OnJumpPerformed;
            jumpAction.canceled += OnJumpCanceled;
            if (grappleAction != null)
            {
                grappleAction.performed += OnGrapplePerformed;
                grappleAction.canceled += OnGrappleCanceled;
            }
            // Track which device is being used
            InputSystem.onActionChange += OnActionChange;
        }

        private void DisableInputActions()
        {
            if (moveAction == null) return;

            // Unsubscribe from events (prevents memory leaks)
            jumpAction.performed -= OnJumpPerformed;
            jumpAction.canceled -= OnJumpCanceled;
            if (grappleAction != null)
            {
                grappleAction.performed -= OnGrapplePerformed;
                grappleAction.canceled -= OnGrappleCanceled;
            }
            InputSystem.onActionChange -= OnActionChange;

            // Disable actions
            moveAction.Disable();
            jumpAction.Disable();
            if (grappleAction != null) grappleAction.Disable();
            if (aimAction != null) aimAction.Disable();
        }

        private void CleanupInputActions()
        {
            // Final cleanup on destroy
            DisableInputActions();
        }

        /*
         * ------------------------------------------------------------------------
         * MOVEMENT INPUT PROCESSING
         * ------------------------------------------------------------------------
         *
         * This is where raw stick/keyboard input becomes usable game input.
         *
         * FRAME TIMING NOTE:
         * We read input in Update(), not FixedUpdate(). Why?
         *
         * FixedUpdate runs at a fixed rate (default 50 times/second).
         * Update runs every frame (could be 60, 144, etc.).
         *
         * If you read input in FixedUpdate:
         *   - At 144fps, input is read every ~3 frames (input feels laggy)
         *   - Button presses can be missed between FixedUpdate calls
         *
         * Read input in Update, USE input in FixedUpdate for physics.
         * PlayerController will read our processed MoveInput in its FixedUpdate.
         */

        private void ProcessMoveInput()
        {
            if (moveAction == null) return;

            // Read raw input from the action
            rawMoveInput = moveAction.ReadValue<Vector2>();

            // Apply deadzone and store result
            MoveInput = ApplyDeadzone(rawMoveInput);
        }
        // --- NEW: Process Aiming ---
        private void ProcessAimInput()
        {
            if (aimAction == null) return;

            Vector2 raw = aimAction.ReadValue<Vector2>();

            // If using Mouse, we need the raw screen position. 
            // If using Controller, we need the direction vector.
            if (UsingGamepad)
            {
                AimInput = ApplyDeadzone(raw);
            }
            else
            {
                // Pass raw mouse coordinates; logic will handle conversion later
                AimInput = raw;
            }
        }
        /*
         * ------------------------------------------------------------------------
         * DEADZONE PROCESSING
         * ------------------------------------------------------------------------
         *
         * CIRCULAR DEADZONE EXPLAINED:
         *
         * 1. Calculate the magnitude (length) of the stick vector
         * 2. If magnitude < deadzone, return zero (stick is in dead center)
         * 3. Otherwise, rescale the input so the usable range is 0-1
         *
         * The rescaling step is important! Without it:
         *   - Deadzone of 0.15 means minimum input is 0.15
         *   - Player can't do slow, precise movements
         *
         * With rescaling:
         *   - Input at edge of deadzone becomes 0
         *   - Input at full extension becomes 1
         *   - Full range of movement is preserved
         *
         * VISUAL:
         *   Raw input 0.15 (at deadzone edge) -> Processed 0.0
         *   Raw input 0.50 (middle) -> Processed ~0.41
         *   Raw input 1.00 (full tilt) -> Processed 1.0
         */

        private Vector2 ApplyDeadzone(Vector2 input)
        {
            float magnitude = input.magnitude;
            float deadzone = config != null ? config.deadzone : 0.15f;

            // Inside deadzone = no input
            if (magnitude < deadzone)
            {
                return Vector2.zero;
            }

            // Rescale so deadzone edge = 0, full tilt = 1
            // Formula: (magnitude - deadzone) / (1 - deadzone)
            float rescaledMagnitude = (magnitude - deadzone) / (1f - deadzone);

            // Clamp to 0-1 range and apply to direction
            rescaledMagnitude = Mathf.Clamp01(rescaledMagnitude);
            return input.normalized * rescaledMagnitude;
        }

        /*
         * ------------------------------------------------------------------------
         * JUMP INPUT & BUFFERING
         * ------------------------------------------------------------------------
         *
         * BUTTON EVENTS:
         *   "performed" fires when button is pressed
         *   "canceled" fires when button is released
         *
         * We don't act on the jump immediately here. Instead, we start a
         * buffer timer. PlayerController checks JumpBuffered and calls
         * ConsumeJumpBuffer() when it executes the jump.
         *
         * This separation means PlayerController owns the "can I jump?" logic
         * (grounded check, coyote time, etc.) while we own the input timing.
         */

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpHeld = true;

            // Start the buffer timer
            float bufferDuration = config != null ? config.jumpBufferDuration : 0.1f;
            jumpBufferTimer = bufferDuration;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            JumpHeld = false;
        }
        // --- NEW: Grapple Callbacks ---
        private void OnGrapplePerformed(InputAction.CallbackContext ctx) => GrappleHeld = true;
        private void OnGrappleCanceled(InputAction.CallbackContext ctx) => GrappleHeld = false;

        /// <summary>
        /// Call this when you execute a jump to clear the buffer.
        /// Prevents the same input from triggering multiple jumps.
        /// </summary>
        public void ConsumeJumpBuffer()
        {
            jumpBufferTimer = 0f;
        }

        private void UpdateBufferTimers()
        {
           
            // Count down buffer timers
            if (jumpBufferTimer > 0f)
            {
                jumpBufferTimer -= Time.deltaTime;
            }
        }

        /*
         * ------------------------------------------------------------------------
         * DEVICE DETECTION
         * ------------------------------------------------------------------------
         *
         * WHY TRACK THIS?
         *
         * 1. Button prompts: Show "Press A" for controller, "Press Space" for keyboard
         * 2. Sensitivity: Some games adjust aim sensitivity per device
         * 3. Analytics: Know how players are playing your game
         *
         * We detect device changes by watching which device triggers actions.
         * When player switches from keyboard to controller (or vice versa),
         * UsingGamepad updates automatically.
         */

        private void OnActionChange(object obj, InputActionChange change)
        {
            // We only care about actions being performed
            if (change != InputActionChange.ActionPerformed) return;

            var action = obj as InputAction;
            if (action == null) return;

            // Check if the device that triggered this action is a gamepad
            var device = action.activeControl?.device;
            if (device != null)
            {
                bool wasUsingGamepad = UsingGamepad;
                UsingGamepad = device is Gamepad;

                // Log device switches (helpful for debugging)
                if (wasUsingGamepad != UsingGamepad)
                {
                    Debug.Log($"[InputReader] Switched to {(UsingGamepad ? "Gamepad" : "Keyboard")}");
                }
            }
        }

        /*
         * ------------------------------------------------------------------------
         * DEBUG VISUALIZATION
         * ------------------------------------------------------------------------
         *
         * Draw input state in Scene view for debugging.
         * Useful for verifying deadzone is working correctly.
         */

        private void OnDrawGizmosSelected()
        {
            // Draw raw input (red) vs processed input (green)
            Vector3 pos = transform.position + Vector3.up * 2f;

            // Raw input
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + new Vector3(rawMoveInput.x, rawMoveInput.y, 0));

            // Processed input
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + new Vector3(MoveInput.x, MoveInput.y, 0));

            // Deadzone circle
            Gizmos.color = Color.yellow;
            float deadzone = config != null ? config.deadzone : 0.15f;
            DrawGizmoCircle(pos, deadzone, 16);
        }

        private void DrawGizmoCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 nextPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}
