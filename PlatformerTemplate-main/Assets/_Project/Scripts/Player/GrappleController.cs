using Platformer.Core;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Platformer.Player
{
    [RequireComponent(typeof(DistanceJoint2D))]
    [RequireComponent(typeof(LineRenderer))]
    public class GrappleController : MonoBehaviour
    {
        [Header("Rubber Arm Settings")]
        [Tooltip("How far the arm can stretch (e.g. 3 player lengths ~ 6 units)")]
        public float maxGrappleDistance = 8f;
        public float climbSpeed = 5f;
        public LayerMask grappleMask; // What can we grab? (Stalactites, Walls)

        [Header("Stamina System")]
        public float maxStamina = 100f;
        public float grappleDrainRate = 15f; // Drain per second while holding
        public float regenRate = 30f; // Regen per second while grounded

        [Header("Visuals")]
        [SerializeField] private LineRenderer lineRenderer;

        // Internal State
        private DistanceJoint2D joint;
        private Rigidbody2D rb;
        private InputReader inputReader;
        private PlayerController playerController;

        public float CurrentStamina { get; private set; }
        public bool IsGrappling => joint.enabled;

        private void Start()
        {
            // Get dependencies
            inputReader = ServiceLocator.Get<InputReader>();
            playerController = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
            joint = GetComponent<DistanceJoint2D>();

            // Initial Setup
            joint.enabled = false;
            lineRenderer.enabled = false;
            CurrentStamina = maxStamina;

            // Configure Joint for "Rubber" feel
            joint.autoConfigureDistance = false;
            joint.maxDistanceOnly = true; // Allows the rope to go slack (important for swings!)
        }

        private void Update()
        {
            // --- NEW: PREVENT GRAPPLE WHILE CLIMBING ---
            // If the player is physically climbing a wall, we shouldn't shoot the arm.
            if (playerController.IsWallClimbing)
            {
                // If we were grappling, stop immediately so we can climb
                if (IsGrappling) StopGrapple();

                // Do not process any more grapple logic this frame
                return;
            }
            // -------------------------------------------

            // 1. Handle Input
            if (inputReader.GrappleHeld && !IsGrappling)
            {
                TryStartGrapple();
            }
            else if (!inputReader.GrappleHeld && IsGrappling)
            {
                StopGrapple();
            }

            // 2. Handle Active Grapple Logic
            if (IsGrappling)
            {
                // We don't need HandleClimbing() here anymore because 
                // Wall Climbing is now handled by PlayerController!
                UpdateStamina(true);
                DrawRope();
            }
            else
            {
                UpdateStamina(false);
            }
        }

        private void TryStartGrapple()
        {
            if (CurrentStamina <= 0) return;

            Vector2 aimDir = GetAimDirection();

            // Raycast to find a grapple point
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDir, maxGrappleDistance, grappleMask);

            if (hit.collider != null)
            {
                StartGrapple(hit.point);
            }
        }

        private void StartGrapple(Vector2 targetPoint)
        {
            joint.enabled = true;
            joint.connectedAnchor = targetPoint;

            // Set the joint distance to the current distance so we don't snap instantly
            joint.distance = Vector2.Distance(transform.position, targetPoint);

            lineRenderer.enabled = true;
        }

        private void StopGrapple()
        {
            joint.enabled = false;
            lineRenderer.enabled = false;
        }

        private void HandleClimbing()
        {
            // Use Vertical Move Input (W/S or Up/Down) to climb
            float climbInput = inputReader.MoveInput.y;

            if (Mathf.Abs(climbInput) > 0.1f)
            {
                // Decrease/Increase joint distance
                joint.distance -= climbInput * climbSpeed * Time.deltaTime;

                // Clamp distance so we don't invert the universe
                joint.distance = Mathf.Clamp(joint.distance, 1f, maxGrappleDistance);
            }
        }

        private void UpdateStamina(bool isGrappling)
        {
            if (isGrappling)
            {
                // Drain stamina
                CurrentStamina -= grappleDrainRate * Time.deltaTime;
                if (CurrentStamina <= 0)
                {
                    CurrentStamina = 0;
                    StopGrapple(); // Forced release!
                }
            }
            else if (playerController.IsGrounded)
            {
                // Regen stamina only when on the ground
                CurrentStamina += regenRate * Time.deltaTime;
                CurrentStamina = Mathf.Min(CurrentStamina, maxStamina);
            }
        }

        private void DrawRope()
        {
            if (!lineRenderer.enabled) return;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, joint.connectedAnchor);
        }

        public void DrainStamina(float amount)
        {
            CurrentStamina -= amount;
            if (CurrentStamina < 0) CurrentStamina = 0;
        }

        private Vector2 GetAimDirection()
        {
            if (inputReader.UsingGamepad)
            {
                // If stick is neutral, default to aim UP
                if (inputReader.AimInput == Vector2.zero) return Vector2.up;
                return inputReader.AimInput;
            }
            else
            {
                // Mouse logic: Convert screen pixel to world position
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                return (worldMousePos - transform.position).normalized;
            }
        }

    }
}
