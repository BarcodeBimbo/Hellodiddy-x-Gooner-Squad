using UnityEngine;
using UnityEngine.UI;

namespace Smokey
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        public GameObject notificationPanel;  // Panel to show notifications
        public Text notificationText;          // Text component for showing notification

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void ShowNotification(string message, float duration = 3f)
        {
            notificationPanel.SetActive(true);
            notificationText.text = message;

            // Hide notification after the specified duration
            Invoke("HideNotification", duration);
        }

        public void ShowKillNotification(string message)
        {
            ShowNotification(message, 5f);  // Kill notifications stay a bit longer
        }

        private void HideNotification()
        {
            notificationPanel.SetActive(false);
        }

        public void NotifyToggle(string modName, bool isEnabled)
        {
            if (isEnabled)
                ShowNotification($"{modName} Enabled", 2f);
            else
                ShowNotification($"{modName} Disabled", 2f);
        }

        // Example: Notify when a Silent Aimbot is toggled
        public void NotifytAimbotToggle(bool isEnabled)
        {
            if (isEnabled)
                ShowNotification("Aimbot Enabled", 2f);
            else
                ShowNotification("Aimbot Disabled", 2f);
        }

        public void NotifySilentAimbotToggle(bool isEnabled)
        {
            if (isEnabled)
                ShowNotification("Silent Aimbot Enabled", 2f);
            else
                ShowNotification("Silent Aimbot Disabled", 2f);
        }

        // Example: Notify the user of a kill event
        public void NotifyKill(string killerName, string victimName)
        {
            ShowKillNotification($"{killerName} killed {victimName}!");
        }
    }
}
