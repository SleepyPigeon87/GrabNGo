using UnityEngine;
using UnityEngine.UI;
using Platformer.Player; 

namespace Platformer.UI
{
    public class StaminaHUD : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Image fillImage;
        [SerializeField] private CanvasGroup canvasGroup; // To fade it out when full

        [Header("References")]
        [SerializeField] private GrappleController grappleController;

        private void Update()
        {
            if (grappleController == null) return;

            // 1. Calculate percentage (0.0 to 1.0)
            float current = grappleController.CurrentStamina;
            float max = grappleController.maxStamina;
            float percent = current / max;

            // 2. Update the radial fill
            fillImage.fillAmount = percent;

            // 3. Optional: Hide bar if stamina is full (cleaner UI)
            if (percent >= 0.99f)
            {
                // Fade out
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime * 5f);
            }
            else
            {
                // Fade in
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.deltaTime * 10f);
            }
        }
    }
}