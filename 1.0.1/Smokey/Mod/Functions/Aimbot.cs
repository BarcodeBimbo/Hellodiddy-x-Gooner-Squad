using Il2CppScheduleOne.NPCs;
using Smokey.Mod.Features;
using UnityEngine;
using System;
using System.Collections.Generic;
using Smokey.Mod.UI;

namespace Smokey.Mod.Functions
{
    public static class Aimbot
    {
        // Toggle settings.
        public static bool AimbotEnabled = false;          // Sticky aimbot mode.
        public static bool SilentAimbotEnabled = false;      // Silent aimbot mode.
        // Removed AutoShoot and Cursor locking.
        public static bool VisibilityCheck = false;          // Require visibility for target.
        public static bool DrawFovEnabled = true;            // Draw the FOV circle.
        public static bool MultiTargetEnabled = false;       // When true, handle multiple targets simultaneously.

        // Configurable parameters.
        public static float FOV = 90f;                        // Field-of-view (in screen pixels) for target acquisition.
        public static float SmoothFactor = 80f;               // Smoothing factor for players.
        public static float NPCSmoothFactor = 70f;            // Smoothing factor for NPCs.
        public static bool EnablePrediction = true;           // Enable target velocity prediction.
        public static float BulletSpeed = 50f;                // Bullet speed (for travel time calculation).
        public static float PredictionFactor = 0.5f;          // Base prediction scale factor.
        public static bool AimSmoothingEnabled = false;       // Set to false so we snap instantly.
        public static float AimInaccuracy = 0f;               // 0 = perfect aim.

        // Priority mode enum includes FOV mode.
        public enum PriorityType { ClosestToCrosshair, LowestHealth, FOV, Off }
        public static PriorityType CurrentPriority = PriorityType.ClosestToCrosshair;

        private static Transform currentTarget;

        public static void CyclePriority()
        {
            if (CurrentPriority == PriorityType.ClosestToCrosshair)
                CurrentPriority = PriorityType.LowestHealth;
            else if (CurrentPriority == PriorityType.LowestHealth)
                CurrentPriority = PriorityType.FOV;
            else if (CurrentPriority == PriorityType.FOV)
                CurrentPriority = PriorityType.Off;
            else
                CurrentPriority = PriorityType.ClosestToCrosshair;
        }

        public static void HandleAimbot()
        {
            // Do not run aimbot when UI is visible.
            if (Settings.showUI || !MainMod.IsMainScene || Features.Features.LocalPlayer == null)
            {
                AimbotVisual.SetLockState(false);
                return;
            }

            // Toggle aimbot activation while the right mouse button is held.
            if ((AimbotEnabled || SilentAimbotEnabled) && Input.GetMouseButton(1))
            {
                // Process targets.
                if (MultiTargetEnabled)
                {
                    List<Transform> validTargets = GetValidTargets();
                    if (validTargets.Count > 0)
                    {
                        foreach (Transform target in validTargets)
                        {
                            ProcessAndFireAtTarget(target);
                        }
                        AimbotVisual.SetLockState(true);
                    }
                    else
                    {
                        AimbotVisual.SetLockState(false);
                    }
                }
                else
                {
                    Transform bestTarget = FindBestTarget();
                    if (bestTarget != null)
                    {
                        ProcessAndFireAtTarget(bestTarget);
                        AimbotVisual.SetLockState(true);
                    }
                    else
                    {
                        AimbotVisual.SetLockState(false);
                    }
                }
            }
            else
            {
                AimbotVisual.SetLockState(false);
            }
        }

        // Process a target: calculate predicted position and adjust the camera.
        private static void ProcessAndFireAtTarget(Transform target)
        {
            Vector3 targetPos = target.position;
            bool isNPCTarget = false;
            NPC targetNPC = null;

            // Check if target is an NPC.
            foreach (var npc in Features.Features.GetNPCS())
            {
                if (npc != null && npc.transform == target)
                {
                    if (!string.IsNullOrEmpty(npc.name) &&
                        string.Equals(npc.name, "Uncle Nelson", StringComparison.OrdinalIgnoreCase))
                        return;
                    isNPCTarget = true;
                    targetNPC = npc;
                    break;
                }
            }

            if (!isNPCTarget)
            {
                var capsule = target.GetComponent<CapsuleCollider>();
                if (capsule != null)
                    targetPos += Vector3.up * (capsule.height * 0.9f);
                else
                    targetPos += Vector3.up * 1.6f;
            }
            else if (targetNPC != null && targetNPC.movement != null && targetNPC.movement.capsuleCollider != null)
            {
                float colliderCenterY = targetNPC.movement.capsuleCollider.center.y;
                float colliderHeight = targetNPC.movement.capsuleCollider.height;
                float computedHeadHeight = colliderCenterY + (colliderHeight / 2f);
                targetPos = targetNPC.movement.FootPosition + new Vector3(0, computedHeadHeight, 0);

                // Incorporate additional NPC properties into prediction.
                float npcMultiplier = 1f;
                int stance = Convert.ToInt32(targetNPC.movement.Stance); // e.g., 0=standing, 1=crouching, 2=laying down.
                if (stance == 1)
                    npcMultiplier *= 0.8f;
                else if (stance == 2)
                    npcMultiplier *= 0.6f;

                npcMultiplier += targetNPC.movement.stumbleDirection.magnitude * 0.5f;
                npcMultiplier += targetNPC.movement.RunSpeed / 10f; // Increase multiplier with higher run speed.

                targetPos = GetPredictedTargetPosition(target, isNPCTarget, targetPos, npcMultiplier);
            }
            else
            {
                targetPos += Vector3.up * 1.6f;
            }

            if (SilentAimbotEnabled)
                SilentAimAt(targetPos, isNPCTarget);
            else if (AimbotEnabled)
                AimAt(targetPos, isNPCTarget, target);
        }

        // Single-target mode: find the best player target based on FOV and priority.
        private static Transform FindBestTarget()
        {
            Camera cam = Camera.main;
            if (cam == null)
                return null;

            float bestScore = float.MinValue;
            Transform bestTarget = null;

            foreach (var player in Features.Features.GetPlayers())
            {
                if (player == null || player.IsLocalPlayer || player.transform == null ||
                    player.Health == null || player.Health.CurrentHealth <= 0)
                    continue;
                if (VisibilityCheck && (player.Visibility == null || !player.VisualState))
                    continue;

                Vector3 headPos = player.transform.position + Vector3.up * 1.6f;
                Vector3 screenPos = cam.WorldToScreenPoint(headPos);
                if (screenPos.z < 0)
                    continue;

                float screenDist = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2),
                                                     new Vector2(screenPos.x, Screen.height - screenPos.y));

                float score = 0f;
                if (CurrentPriority == PriorityType.FOV)
                    score = (1f / (screenDist + 0.001f)) * 100f;
                else if (CurrentPriority == PriorityType.ClosestToCrosshair)
                    score = 1f / (screenDist + 0.001f);
                else if (CurrentPriority == PriorityType.LowestHealth)
                    score = 1f / (player.Health.CurrentHealth + 0.001f);
                else
                    score = (1f / (screenDist + 0.001f)) +
                            (1f / (Vector3.Distance(Features.Features.LocalPlayer.transform.position, player.transform.position) + 1f));

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = player.transform;
                }
            }

            return bestTarget;
        }

        // Multi-target mode: return all valid targets inside the FOV circle.
        private static List<Transform> GetValidTargets()
        {
            Camera cam = Camera.main;
            List<Transform> targets = new List<Transform>();
            if (cam == null)
                return targets;

            foreach (var player in Features.Features.GetPlayers())
            {
                if (player == null || player.IsLocalPlayer || player.transform == null ||
                    player.Health == null || player.Health.CurrentHealth <= 0)
                    continue;
                if (VisibilityCheck && (player.Visibility == null || !player.VisualState))
                    continue;

                Vector3 headPos = player.transform.position + Vector3.up * 1.6f;
                Vector3 screenPos = cam.WorldToScreenPoint(headPos);
                if (screenPos.z < 0)
                    continue;
                float screenDist = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2),
                                                     new Vector2(screenPos.x, Screen.height - screenPos.y));
                if (screenDist > FOV)
                    continue;

                targets.Add(player.transform);
            }

            foreach (var npc in Features.Features.GetNPCS())
            {
                if (npc == null || npc.transform == null ||
                    !npc.IsConscious || npc.Health == null || npc.Health.Health <= 0)
                    continue;
                if (!string.IsNullOrEmpty(npc.name) &&
                    string.Equals(npc.name, "Uncle Nelson", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (VisibilityCheck && !npc.isVisible)
                    continue;

                Vector3 targetOffset;
                if (npc.movement != null && npc.movement.capsuleCollider != null)
                {
                    float colliderCenterY = npc.movement.capsuleCollider.center.y;
                    float colliderHeight = npc.movement.capsuleCollider.height;
                    float computedHeadHeight = colliderCenterY + (colliderHeight / 2f);
                    targetOffset = npc.movement.FootPosition + new Vector3(0, computedHeadHeight, 0);
                }
                else
                {
                    targetOffset = npc.transform.position + Vector3.up * 1.6f;
                }
                Vector3 screenPos = cam.WorldToScreenPoint(targetOffset);
                if (screenPos.z < 0)
                    continue;
                float screenDist = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2),
                                                     new Vector2(screenPos.x, Screen.height - screenPos.y));
                if (screenDist > FOV)
                    continue;

                targets.Add(npc.transform);
            }
            return targets;
        }

        /// <summary>
        /// Calculates the predicted target position.
        /// This overload accepts an extra multiplier (for NPC-specific prediction adjustments).
        /// </summary>
        private static Vector3 GetPredictedTargetPosition(Transform bestTarget, bool isNPCTarget, Vector3 originalPos, float extraMultiplier = 1f)
        {
            if (!EnablePrediction)
                return originalPos;

            Vector3 shooterPos = Features.Features.LocalPlayer.transform.position;
            float distance = Vector3.Distance(shooterPos, originalPos);
            float travelTime = distance / BulletSpeed;

            Vector3 targetVelocity = Vector3.zero;
            bool foundVelocity = false;
            foreach (var player in Features.Features.GetPlayers())
            {
                if (player != null && player.transform == bestTarget)
                {
                    var velCalc = player.VelocityCalculator;
                    if (velCalc != null)
                    {
                        targetVelocity = velCalc.GetAverageVelocity();
                        foundVelocity = true;
                    }
                    break;
                }
            }
            if (!foundVelocity)
            {
                foreach (var npc in Features.Features.GetNPCS())
                {
                    if (npc != null && npc.transform == bestTarget)
                    {
                        if (npc.movement != null && npc.movement.velocityCalculator != null)
                        {
                            targetVelocity = npc.movement.velocityCalculator.GetAverageVelocity();
                            foundVelocity = true;
                        }
                        break;
                    }
                }
            }

            float dynamicPredictionFactor = PredictionFactor;
            float targetSpeed = targetVelocity.magnitude;
            if (targetSpeed > 0.1f)
                dynamicPredictionFactor += targetSpeed * 0.1f;
            if (distance > 50f)
                dynamicPredictionFactor += (distance - 50f) * 0.01f;
            Vector3 localVelocity = Vector3.zero;
            if (Features.Features.LocalPlayer != null && Features.Features.LocalPlayer.VelocityCalculator != null)
                localVelocity = Features.Features.LocalPlayer.VelocityCalculator.GetAverageVelocity();
            float localSpeed = localVelocity.magnitude;
            if (localSpeed > 0.1f)
                dynamicPredictionFactor += localSpeed * 0.05f;

            Vector3 predictedPos = originalPos + targetVelocity * travelTime * dynamicPredictionFactor * extraMultiplier;
            float verticalDiff = shooterPos.y - originalPos.y;
            if (Mathf.Abs(verticalDiff) > 1f)
                predictedPos.y += verticalDiff * 0.1f;

            return predictedPos;
        }

        /// <summary>
        /// Smoothly rotates the camera toward the target (single-target mode).
        /// When the Priority is FOV, the aim snaps instantly.
        /// Now, instead of moving/locking the mouse, the camera's rotation is simply set (locked) to aim at the predicted head.
        /// </summary>
        private static void AimAt(Vector3 targetWorldPosition, bool isNPCTarget, Transform targetTransform)
        {
            Camera cam = Camera.main;
            if (cam == null)
                return;

            Vector3 directionToTarget = (targetWorldPosition - cam.transform.position).normalized;
            if (AimInaccuracy > 0f)
            {
                float randomYaw = UnityEngine.Random.Range(-AimInaccuracy, AimInaccuracy);
                float randomPitch = UnityEngine.Random.Range(-AimInaccuracy, AimInaccuracy);
                Quaternion randomOffset = Quaternion.Euler(randomPitch, randomYaw, 0);
                directionToTarget = randomOffset * directionToTarget;
            }

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            if (CurrentPriority == PriorityType.FOV)
            {
                // In FOV mode, instantly snap the camera rotation.
                cam.transform.rotation = targetRotation;
                return;
            }

            // For other priority modes, we could use a smoothing factor.
            // However, since you want the camera to be locked to the target's head,
            // we set the rotation directly.
            cam.transform.rotation = targetRotation;
        }

        /// <summary>
        /// For silent aim (and multi-target mode), temporarily sets the camera rotation, fires a shot, and then reverts.
        /// In this version, we no longer move or lock the mouse; only the camera rotation is set.
        /// </summary>
        private static void SilentAimAt(Vector3 targetWorldPosition, bool isNPCTarget)
        {
            Camera cam = Camera.main;
            if (cam == null)
                return;

            Quaternion originalRotation = cam.transform.rotation;
            Vector3 directionToTarget = (targetWorldPosition - cam.transform.position).normalized;
            if (AimInaccuracy > 0f)
            {
                float randomYaw = UnityEngine.Random.Range(-AimInaccuracy, AimInaccuracy);
                float randomPitch = UnityEngine.Random.Range(-AimInaccuracy, AimInaccuracy);
                Quaternion randomOffset = Quaternion.Euler(randomPitch, randomYaw, 0);
                directionToTarget = randomOffset * directionToTarget;
            }
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            cam.transform.rotation = targetRotation;
            cam.transform.rotation = originalRotation;
        }
    }
}
