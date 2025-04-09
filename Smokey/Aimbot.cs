using UnityEngine;

namespace Smokey
{
    public static class Aimbot
    {
        public static bool AimbotEnabled = false; // Sticky aimbot (left-click)
        public static bool SilentAimbotEnabled = false; // Silent aimbot (right-click)
        public static bool AutoShoot = false; // Auto-shoot after lock-on
        public static bool VisibilityCheck = false; // Visibility check toggle
        public static bool DrawFovEnabled = true; // Draw FOV circle toggle

        public static float FOV = 90f; // FOV radius for normal aimbot

        public enum PriorityType { ClosestToCrosshair, LowestHealth, Off }
        public static PriorityType CurrentPriority = PriorityType.ClosestToCrosshair; // Priority system (closest to crosshair or lowest health)

        private static Transform currentTarget = null; // Store current target for lock-on

        public static void CyclePriority()
        {
            // Toggle through priority types (ClosestToCrosshair, LowestHealth, Off)
            if (CurrentPriority == PriorityType.ClosestToCrosshair)
                CurrentPriority = PriorityType.LowestHealth;
            else if (CurrentPriority == PriorityType.LowestHealth)
                CurrentPriority = PriorityType.Off;
            else
                CurrentPriority = PriorityType.ClosestToCrosshair;
        }

        public static void HandleAimbot()
        {
            if (!MainMod.IsMainScene || Features.LocalPlayer == null)
            {
                AimbotVisual.SetLockState(false);
                return;
            }

            Transform bestTarget = null;

            // Sticky Aimbot (left-click)
            if (AimbotEnabled && Input.GetMouseButton(0)) // Left-click activated sticky aimbot
            {
                bestTarget = FindBestTarget();
                if (bestTarget != null)
                {
                    AimAt(bestTarget.position + Vector3.up * 1.6f); // Head position
                    AimbotVisual.SetLockState(true);

                    if (AutoShoot)
                    {
                        Features.LocalPlayer.Punch(); // Auto-punch after lock-on
                    }
                }
                else
                {
                    AimbotVisual.SetLockState(false);
                }
            }

            // Silent Aimbot (right-click)
            if (SilentAimbotEnabled && Input.GetMouseButton(1)) // Right-click activated silent aimbot
            {
                bestTarget = FindBestTarget();
                if (bestTarget != null)
                {
                    AimAt(bestTarget.position + Vector3.up * 1.6f); // Head position
                    AimbotVisual.SetLockState(true);

                    if (AutoShoot)
                    {
                        Features.LocalPlayer.Punch(); // Auto-punch after lock-on
                    }
                }
                else
                {
                    AimbotVisual.SetLockState(false);
                }
            }
        }

        private static Transform FindBestTarget()
        {
            Camera cam = Camera.main;
            if (cam == null) return null;

            Transform bestTarget = null;
            float bestMetric = float.MaxValue;

            // Normal Aimbot Target Lock (Sticky Aimbot when priority is used)
            foreach (var player in Features.GetPlayers())
            {
                if (player == null || player.IsLocalPlayer || player.transform == null || player.Health == null || player.Health.CurrentHealth <= 0)
                    continue;

                if (VisibilityCheck && (player.Visibility == null || !player.VisualState)) // Visibility check
                    continue;

                Vector3 screenPos = cam.WorldToScreenPoint(player.transform.position + Vector3.up * 1.6f); // Head position
                if (screenPos.z < 0) continue;

                float dist = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(screenPos.x, Screen.height - screenPos.y));
                if (dist > FOV) continue;

                // If Priority is ClosestToCrosshair, lock on to closest player
                float metric = CurrentPriority == PriorityType.ClosestToCrosshair ? dist : player.Health.CurrentHealth;

                if (metric < bestMetric)
                {
                    bestMetric = metric;
                    bestTarget = player.transform;
                }
            }

            // Silent Aimbot Target Lock (Similar logic applied to NPCs)
            if (SilentAimbotEnabled)
            {
                foreach (var npc in Features.GetNPCS())
                {
                    if (npc == null || npc.transform == null || !npc.IsConscious || npc.Health == null || npc.Health.Health <= 0)
                        continue;

                    if (VisibilityCheck && !npc.isVisible) // Visibility check for NPCs
                        continue;

                    Vector3 screenPos = cam.WorldToScreenPoint(npc.transform.position + Vector3.up * 1.6f); // Head position
                    if (screenPos.z < 0) continue;

                    float dist = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(screenPos.x, Screen.height - screenPos.y));
                    if (dist > FOV) continue;

                    float metric = CurrentPriority == PriorityType.ClosestToCrosshair ? dist : npc.Health.Health;

                    if (metric < bestMetric)
                    {
                        bestMetric = metric;
                        bestTarget = npc.transform;
                    }
                }
            }

            return bestTarget;
        }

        private static void AimAt(Vector3 worldPosition)
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 dir = (worldPosition - cam.transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            cam.transform.rotation = lookRot; // No smoothing for sticky lock-on
        }
    }
}
