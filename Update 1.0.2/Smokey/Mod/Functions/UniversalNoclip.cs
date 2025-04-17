using UnityEngine;
using Il2CppScheduleOne.PlayerScripts;
using HelloDiddy.Mod.UI;

namespace HelloDiddy.Mod.Functions
{
    public static class Noclip
    {
        public static Player LocalPlayer => Player.Local;
        private static readonly float normalSpeed = 10f;
        private static readonly float fastSpeed = 30f;
        private static readonly float slowSpeed = 5f;

        private static Collider? playerCollider;
        private static Rigidbody? playerRb;
        public static bool IsFlying = false;

        public static void Update()
        {
            if (LocalPlayer == null || LocalPlayer.transform == null)
                return;

            var playerTransform = LocalPlayer.transform;

            if (Input.GetKey(MainModState.NoclipKeyPrimary.Value) && Input.GetKeyDown(MainModState.NoclipKeySecondary.Value))
            {
                IsFlying = !IsFlying;

                if (IsFlying)
                    EnableNoclip(playerTransform);
                else
                    DisableNoclip(playerTransform);
            }

            if (IsFlying)
                HandleFlyingMovement(playerTransform);
        }

        private static void EnableNoclip(Transform playerTransform)
        {
            NotificationUtils.CreateNotification("No Clip", "Enabled", NotificationUtils.LoadNotificationSprite());
            if (playerCollider == null)
                playerCollider = playerTransform.GetComponent<Collider>();

            if (playerRb == null)
                playerRb = playerTransform.GetComponent<Rigidbody>();

            if (playerCollider != null)
                playerCollider.enabled = false;

            if (playerRb != null)
            {
                playerRb.useGravity = false;
                playerRb.velocity = Vector3.zero;
                playerRb.isKinematic = true;
            }
        }

        private static void DisableNoclip(Transform playerTransform)
        {
            NotificationUtils.CreateNotification("No Clip", "Disabled", NotificationUtils.LoadNotificationSprite());
            if (playerCollider == null)
                playerCollider = playerTransform.GetComponent<Collider>();

            if (playerRb == null)
                playerRb = playerTransform.GetComponent<Rigidbody>();

            if (playerCollider != null)
                playerCollider.enabled = true;

            if (playerRb != null)
            {
                playerRb.useGravity = true;
                playerRb.isKinematic = false;
            }
        }

        private static void HandleFlyingMovement(Transform playerTransform)
        {
            var direction = Vector3.zero;
            var cameraTransform = Camera.main?.transform;

            if (cameraTransform == null)
                return;

            if (Input.GetKey(KeyCode.W)) direction += cameraTransform.forward;
            if (Input.GetKey(KeyCode.S)) direction -= cameraTransform.forward;
            if (Input.GetKey(KeyCode.A)) direction -= cameraTransform.right;
            if (Input.GetKey(KeyCode.D)) direction += cameraTransform.right;
            if (Input.GetKey(KeyCode.Space)) direction += cameraTransform.up;
            if (Input.GetKey(KeyCode.LeftControl)) direction -= cameraTransform.up;

            float speed = normalSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) speed = fastSpeed;
            if (Input.GetKey(KeyCode.LeftAlt)) speed = slowSpeed;

            playerTransform.position += direction.normalized * speed * Time.deltaTime;
        }
    }
}
