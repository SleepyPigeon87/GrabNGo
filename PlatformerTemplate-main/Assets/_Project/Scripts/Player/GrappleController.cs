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
        public float winchSpeed = 5f;
        public float MinRopeLength = 1f;
        public float MaxRopeLength = 10f;
        public LayerMask grappleMask; // What can we grab? (Stalactites, Walls)
        [SerializeField] private Transform firePoint;
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
        private GrappleController grappleController;

        public float CurrentStamina { get; private set; }
        public bool IsGrappling => joint.enabled;

        private void Start()
        {
            // Get dependencies
            inputReader = ServiceLocator.Get<InputReader>();
            playerController = GetComponent<PlayerController>();
            grappleController = GetComponent<GrappleController>();
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
                HandleRopeLength();
            }
            else
            {
                UpdateStamina(false);
            } 
        }
            private void HandleRopeLength()
            {
            // Read Up/Down input
            float input = inputReader.MoveInput.y;

                if (Mathf.Abs(input) > 0.1f)
                {
                // Calculate new distance
                // Input > 0 (Up) means Shorten (subtract distance)
                // Input < 0 (Down) means Lengthen (add distance)
                float newDistance = joint.distance - (input * winchSpeed * Time.deltaTime);

                // Clamp it so we don't break physics or go infinite
                joint.distance = Mathf.Clamp(newDistance, MinRopeLength, MaxRopeLength);
                }
            }



        private void TryStartGrapple()
        {
            // 1. Check Stamina first
            if (CurrentStamina <= 0) return;

            // 2. Calculate Direction
            Vector2 direction = GetAimDirection();

            // 3. Raycast (From the new FirePoint, not the center of player)
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, maxGrappleDistance, grappleMask);

            // 4. Handle Result
            if (hit.collider != null)
            {
                // --- HIT! ---
                // Stop any "Miss" animations running from a previous click
                StopAllCoroutines();

                // Physics: Connect the joint
                joint.enabled = true;
                joint.connectedAnchor = hit.point;
                joint.distance = Vector2.Distance(firePoint.position, hit.point);

                // Visuals: Draw the line instantly
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                // --- MISS! (The "Always Fire" visual) ---
                StopAllCoroutines();
                StartCoroutine(GrappleMissEffect(direction));
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
            lineRenderer.SetPosition(0, firePoint.position);
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
                return (worldMousePos - firePoint.position).normalized;
            }
        }

        private System.Collections.IEnumerator GrappleMissEffect(Vector2 direction)
        {
            // 1. Setup
            lineRenderer.enabled = true;
            Vector3 startPos = firePoint.position;
            Vector3 targetPos = startPos + (Vector3)(direction * maxGrappleDistance);
            Vector3 currentTipPos = startPos;

            float shootSpeed = 40f; // Fast shot!

            // 2. Shoot OUT
            while (Vector3.Distance(currentTipPos, targetPos) > 0.1f)
            {
                // Update tip position
                currentTipPos = Vector3.MoveTowards(currentTipPos, targetPos, shootSpeed * Time.deltaTime);

                // Draw the line
                lineRenderer.SetPosition(0, firePoint.position); // Always at hand
                lineRenderer.SetPosition(1, currentTipPos);      // Moving tip

                yield return null; // Wait for next frame
            }

            // 3. Retract BACK (Optional - removes the line quickly)
            // You can comment this loop out if you just want it to vanish instantly
            while (Vector3.Distance(currentTipPos, firePoint.position) > 0.5f)
            {
                // Retract faster than shooting
                currentTipPos = Vector3.MoveTowards(currentTipPos, firePoint.position, shootSpeed * 2f * Time.deltaTime);

                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, currentTipPos);

                yield return null;
            }

            // 4. Cleanup
            lineRenderer.enabled = false;
        }

    }
}
