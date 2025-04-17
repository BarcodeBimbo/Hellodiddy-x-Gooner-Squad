using HelloDiddy.Mod;
using Il2CppScheduleOne.NPCs;
using UnityEngine;

public enum Stance
{
    Standing,
    Crouching,
    Prone
}

public static class TargetUtils
{
    private static readonly string[] BonePriorityPaths = new string[]
    {
        "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head",
        "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck",
        "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2",
        "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1",
        "Armature/mixamorig:Hips/mixamorig:Spine",
        "Armature/mixamorig:Hips"
    };

    public static Vector3 GetDynamicTargetPosition(Transform target)
    {
        if (target == null)
            return Vector3.zero;

        Transform bestBone = FindDynamicBone(target);
        if (bestBone != null)
        {
            Vector3 bestBonePos = bestBone.position;
            return MainModState.EnablePrediction
                ? PredictTargetPosition(target, bestBonePos)
                : bestBonePos;
        }

        Vector3 rootPos = target.position;
        float heightOffset = GetHeightOffset(target);
        Vector3 finalPos = rootPos + Vector3.up * heightOffset;

        return MainModState.EnablePrediction
            ? PredictTargetPosition(target, finalPos)
            : finalPos;
    }

    private static Transform FindDynamicBone(Transform target)
    {
        if (target == null)
            return null;

        Stance stance = DetectStance(target);
        string[] bonesToSearch = stance switch
        {
            Stance.Standing => new string[]
            {
                "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head",
                "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck"
            },
            Stance.Crouching => new string[]
            {
                "Armature/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2"
            },
            Stance.Prone => new string[]
            {
                "Armature/mixamorig:Hips"
            },
            _ => BonePriorityPaths
        };

        foreach (string path in bonesToSearch)
        {
            Transform bone = target.Find(path);
            if (bone != null)
                return bone;
        }

        return target;
    }

    private static float GetHeightOffset(Transform target)
    {
        return DetectStance(target) switch
        {
            Stance.Prone => 0.0f,
            Stance.Crouching => 0.9f,
            _ => 1.6f,
        };
    }

    public static Stance DetectStance(Transform target)
    {
        if (target == null)
            return Stance.Standing;

        Transform hips = target.Find("Armature/mixamorig:Hips") ?? target;
        Vector3 livePos = hips.position;

        float rayDistance = 10f;
        float groundHeight = 0f;
        RaycastHit hit;

        int groundLayerIndex = LayerMask.NameToLayer("Ground");
        int groundLayerMask = groundLayerIndex >= 0 ? LayerMask.GetMask("Ground") : Physics.DefaultRaycastLayers;

        if (Physics.Raycast(livePos, Vector3.down, out hit, rayDistance, groundLayerMask))
        {
            if (hit.collider != null && hit.collider.transform.IsChildOf(target))
            {
                Vector3 offsetOrigin = livePos + Vector3.up * 0.5f;
                if (Physics.Raycast(offsetOrigin, Vector3.down, out RaycastHit offsetHit, rayDistance, groundLayerMask))
                    groundHeight = offsetHit.point.y;
            }
            else
            {
                groundHeight = hit.point.y;
            }
        }

        float relativeHeight = livePos.y - groundHeight;

        if (relativeHeight >= 3.0f)
            return Stance.Standing;
        else if (relativeHeight >= 1.0f)
            return Stance.Crouching;
        else
            return Stance.Prone;
    }

    public static Vector3 PredictTargetPosition(Transform target, Vector3 currentPosition)
    {
        if (target == null)
            return currentPosition;

        Rigidbody rb = target.GetComponent<Rigidbody>();
        Vector3 velocity = rb != null ? rb.velocity : Vector3.zero;

        float projectileSpeed = MainModState.BulletSpeed > 0 ? MainModState.BulletSpeed : 100f;
        Vector3 toTarget = currentPosition - MainModState.LocalPlayer.transform.position;
        float distance = toTarget.magnitude;
        float timeToTarget = distance / projectileSpeed;

        return currentPosition + velocity * timeToTarget * MainModState.PredictionFactor;
    }
}
