using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Il2CppScheduleOne.NPCs;
using MelonLoader;
using HelloDiddy.Mod.Functions;
using HelloDiddy.Mod;
using HelloDiddy.Mod.UI;
public static class Aimbot
{
    public enum PriorityType { ClosestToCrosshair, LowestHealth, FOV, Distance }
    public static PriorityType CurrentPriority = PriorityType.ClosestToCrosshair;

    public static Transform? lockedTarget;
    public static GameObject? playerObj;
    public static GameObject? cameraContainer;
    public static Camera? mainCamera;

    public static void HandleAimbot()
    {
        CacheReferences();

        if (Settings.showUI || !MainModState.IsMainScene || MainModState.LocalPlayer == null)
        {
            lockedTarget = null;
            AimbotVisual.SetLockState(false);
            return;
        }

        if (!MainModState.AimbotEnabled || !Input.GetMouseButton(1))
        {
            lockedTarget = null;
            AimbotVisual.SetLockState(false);
            return;
        }

        if (lockedTarget == null || !IsTargetValid(lockedTarget))
            lockedTarget = FindBestTarget();

        if (lockedTarget != null)
        {
            Vector3 aimPoint = TargetUtils.GetDynamicTargetPosition(lockedTarget);
            PredictiveAim(aimPoint);
            AimbotVisual.SetLockState(true);
        }
        else
        {
            AimbotVisual.SetLockState(false);
        }
    }
    private static void PredictiveAim(Vector3 targetPosition)
    {
        if (mainCamera == null || playerObj == null || cameraContainer == null)
            return;

        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 aimDirection = (targetPosition - cameraPos).normalized;
        if (aimDirection == Vector3.zero || float.IsNaN(aimDirection.x) || float.IsNaN(aimDirection.y) || float.IsNaN(aimDirection.z))
            return;

        Vector3 flatDirection = new Vector3(aimDirection.x, 0f, aimDirection.z).normalized;
        if (flatDirection != Vector3.zero)
        {
            Quaternion yawRotation = Quaternion.LookRotation(flatDirection);
            playerObj.transform.rotation = Quaternion.RotateTowards(
                playerObj.transform.rotation,
                yawRotation,
                MainModState.SmoothFactor * 100f * Time.deltaTime
            );
        }

        float pitch = -Mathf.Asin(Mathf.Clamp(aimDirection.y, -0.99f, 0.99f)) * Mathf.Rad2Deg;
        Vector3 currentEuler = cameraContainer.transform.localEulerAngles;
        Vector3 targetEuler = new Vector3(pitch, currentEuler.y, currentEuler.z);

        cameraContainer.transform.localEulerAngles = Vector3.MoveTowards(
            currentEuler,
            targetEuler,
            MainModState.SmoothFactor * 100f * Time.deltaTime
        );
    }

    private static void CacheReferences()
    {
        if (playerObj == null)
            playerObj = GameObject.Find("Player_Local");

        if (cameraContainer == null)
            cameraContainer = GameObject.Find("Player_Local/CameraContainer");

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public static Transform FindBestTarget()
    {
        try
        {
            if (mainCamera == null || playerObj == null)
                return null;

            Vector3 camPos = mainCamera.transform.position;
            Vector3 camForward = mainCamera.transform.forward;
            var targets = new List<(Transform transform, float distance, float angle, float health)>();
            if (MainModState.TargetNPCs && NPCManager.NPCRegistry != null)
            {
                foreach (var npc in NPCManager.NPCRegistry)
                {
                    if (npc == null || npc.Health == null || npc.Health.Health <= 0f || !npc.IsConscious)
                        continue;

                    if (npc.fullName == "Uncle Nelson" || npc.IsInVehicle || npc.isInBuilding)
                        continue;

                    var bone = npc.Avatar?.MiddleSpineRB;
                    if (bone == null)
                        continue;

                    Vector3 pos = bone.position;
                    Vector3 toTarget = (pos - camPos);
                    float angle = Vector3.Angle(camForward, toTarget);
                    float dist = toTarget.magnitude;

                    if (angle <= MainModState.FOV && dist <= MainModState.MaxESPDistance)
                        targets.Add((bone.transform, dist, angle, npc.Health.Health));
                }
            }
            if (MainModState.TargetPlayers && Il2CppScheduleOne.PlayerScripts.Player.PlayerList != null)
            {
                foreach (var player in Il2CppScheduleOne.PlayerScripts.Player.PlayerList)
                {
                    if (player == null || player.IsLocalPlayer || player.Health?.CurrentHealth <= 0f)
                        continue;

                    var bone = player.Avatar?.MiddleSpineRB;
                    if (bone == null)
                        continue;

                    Vector3 pos = bone.position;
                    Vector3 toTarget = (pos - camPos);
                    float angle = Vector3.Angle(camForward, toTarget);
                    float dist = toTarget.magnitude;

                    if (angle <= MainModState.FOV && dist <= MainModState.MaxESPDistance)
                        targets.Add((bone.transform, dist, angle, player.Health.CurrentHealth));
                }
            }

            if (targets.Count == 0)
                return null;

            switch (CurrentPriority)
            {
                case PriorityType.Distance:
                    return targets.OrderBy(t => t.distance).First().transform;
                case PriorityType.FOV:
                    return targets.OrderBy(t => t.angle).First().transform;
                case PriorityType.LowestHealth:
                    return targets.OrderBy(t => t.health).First().transform;
                case PriorityType.ClosestToCrosshair:
                default:
                    return targets
                        .OrderBy(t => t.angle)
                        .ThenBy(t => t.distance)
                        .First().transform;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[Aimbot] Exception in FindBestTarget: " + ex);
            return null;
        }
    }

    private static bool IsTargetValid(Transform target)
    {
        if (target == null) return false;

        foreach (var player in Il2CppScheduleOne.PlayerScripts.Player.PlayerList)
            if (player != null && player.transform == target)
                return player.Health.CurrentHealth > 0;

        foreach (var npc in NPCManager.NPCRegistry)
            if (npc != null && npc.transform == target)
                return npc.Health.Health > 0;

        return false;
    }

    public static void CyclePriority()
    {
        CurrentPriority = (PriorityType)(((int)CurrentPriority + 1) % Enum.GetValues(typeof(PriorityType)).Length);
        MelonLogger.Msg($"[Aimbot] Priority changed to: {CurrentPriority}");
    }
}