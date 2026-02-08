using UnityEngine;
using Platformer.Core;

namespace Platformer.Level
{
    public class Hazard : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                // Kill the player via GameManager service
                var gameManager = ServiceLocator.Get<GameManager>();
                if (gameManager != null)
                {
                    gameManager.RespawnPlayer();
                }
            }
        }
    }
}