using UnityEngine;
using Platformer.Core; // Needed for ServiceLocator

namespace Platformer.Core
{
    public class Checkpoint : MonoBehaviour
    {
        [Header("Visuals")]
        [Tooltip("Assign the 'On' version of your object here (e.g. Green Flag)")]
        [SerializeField] private GameObject activeVisuals;

        [Tooltip("Assign the 'Off' version of your object here (e.g. Red Flag)")]
        [SerializeField] private GameObject inactiveVisuals;

        private bool isActivated = false;

        private void Start()
        {
            // Ensure correct visual state at start
            if (activeVisuals != null) activeVisuals.SetActive(false);
            if (inactiveVisuals != null) inactiveVisuals.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // If already on, do nothing
            if (isActivated) return;

            // Check for Player Tag
            if (collision.CompareTag("Player"))
            {
                ActivateCheckpoint();
            }
        }

        private void ActivateCheckpoint()
        {
            isActivated = true;

            // 1. Swap Visuals
            if (inactiveVisuals != null) inactiveVisuals.SetActive(false);
            if (activeVisuals != null) activeVisuals.SetActive(true);

            // 2. Save Position via Service Locator
            // We ask the phonebook (ServiceLocator) for the GameManager
            var gameManager = ServiceLocator.Get<GameManager>();

            if (gameManager != null)
            {
                gameManager.SetCheckpoint(transform.position);
            }
            else
            {
                Debug.LogWarning("Checkpoint could not find GameManager!");
            }
        }
    }
}