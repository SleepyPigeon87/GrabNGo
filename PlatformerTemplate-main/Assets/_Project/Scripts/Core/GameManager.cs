using UnityEngine;
using System.Collections;
using Platformer.Player;
using Platformer.Core;

namespace Platformer.Core
{   
    public class GameManager : MonoBehaviour
    {
        [Header("settings")]
        public float respawnDelay = 2f;
        private Vector3 currentCheckpointPos;
        private PlayerController player;

         private void Awake()
         {
             ServiceLocator.Register(this);
         }
        private void OnDestroy()
        {
        ServiceLocator.Unregister<GameManager>();
        }

        private void Start()
        {
            // Cache the player so we don't look for it every time we die
            player = FindFirstObjectByType<PlayerController>();

            if (player != null)
            {
                // Set initial spawn to wherever the player starts in the scene
                currentCheckpointPos = player.transform.position;
            }
        }

        public void SetCheckpoint(Vector2 newPos)
        {
            currentCheckpointPos = newPos;
            Debug.Log($"Checkpoint updated to: {newPos}");
        }

        public void RespawnPlayer()
        {
            if (player == null) return;
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            // A. Disable Player (Input & Physics)
            player.enabled = false;
            var rb = player.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; // Prevents falling through world while waiting

            // B. Wait
            yield return new WaitForSeconds(respawnDelay);

            // C. Move & Reset
            player.transform.position = currentCheckpointPos;

            rb.simulated = true;
            player.enabled = true;

            // Optional: Reset Stamina
            var grapple = player.GetComponent<GrappleController>();
            if (grapple != null)
            {
                // Reset stamina to full so they don't spawn tired
                // You might need to make CurrentStamina public set or add a Reset() method
            }
        }
    }
}

