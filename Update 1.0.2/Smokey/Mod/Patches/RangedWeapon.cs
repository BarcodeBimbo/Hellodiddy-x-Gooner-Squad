using HarmonyLib;
using Il2CppScheduleOne.Equipping;
using UnityEngine;

namespace HelloDiddy.Mod.Patches
{
    [HarmonyPatch]
    public static class UltimateAimbotPatches
    {
        private static float defaultImpactForce = 10f;
        private static float defaultFireCooldown = 1f;
        private static float defaultDamage = 30f;
        private static float defaultAccuracy = 1f;
        private static float defaultRange = 100f;
        private static int defaultAmmo = 0;
        private static float defaultAimDuration = 0.05f;

        [HarmonyPatch(typeof(Equippable_RangedWeapon), nameof(Equippable_RangedWeapon.UpdateInput))]
        [HarmonyPostfix]
        public static void WeaponUpdateInputPatch(Equippable_RangedWeapon __instance)
        {
            if (__instance == null || MainModState.Instance == null)
                return;

            ApplyWeaponBuffs(__instance);

            if (MainModState.TriggerBotEnabled && Input.GetMouseButton(1))
            {
                Transform target = Aimbot.lockedTarget ?? Aimbot.FindBestTarget();
                if (target != null && __instance.CanFire())
                {
                    Vector3 aimPos = TargetUtils.GetDynamicTargetPosition(target);
                    Vector3 screenPos = Aimbot.mainCamera.WorldToScreenPoint(aimPos);

                    if (screenPos.z > 0f)
                    {
                        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                        float distToCrosshair = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), screenCenter);

                        if (distToCrosshair < 30f && (!MainModState.VisibilityCheck || AimbotVisible(target, aimPos)))
                        {
                            __instance.Fire();
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Equippable_RangedWeapon), nameof(Equippable_RangedWeapon.Fire))]
        [HarmonyPrefix]
        public static void SilentAimPatch(Equippable_RangedWeapon __instance)
        {
            if (__instance == null || !Input.GetMouseButton(0))
                return;

            Transform target = Aimbot.lockedTarget ?? Aimbot.FindBestTarget();
            if (target == null) return;

            Vector3 aimPoint = TargetUtils.GetDynamicTargetPosition(target);

            if (MainModState.MagicBulletEnabled || MainModState.SilentAimbotEnabled)
            {
                __instance.transform.rotation = Quaternion.LookRotation((aimPoint - __instance.transform.position).normalized);
            }

            if (Aimbot.CurrentPriority == Aimbot.PriorityType.Distance)
            {
                Vector3 directionToTarget = (aimPoint - __instance.transform.position).normalized;
                SmoothRotateTowards(__instance.transform, directionToTarget);
            }
        }

        [HarmonyPatch(typeof(Equippable_RangedWeapon), nameof(Equippable_RangedWeapon.GetSpread))]
        [HarmonyPostfix]
        public static void WeaponGetSpreadPatch(ref float __result)
        {
            if (MainModState.Instance?.NoSpreadEnabled == true)
                __result = 0f;
        }

        private static void ApplyWeaponBuffs(Equippable_RangedWeapon weapon)
        {
            var instance = MainModState.Instance;
            if (instance == null) return;

            if (MainModState.IsMainScene)
            {
                defaultImpactForce = weapon.ImpactForce;
                defaultFireCooldown = weapon.FireCooldown;
                defaultDamage = weapon.Damage;
                defaultAccuracy = weapon._Accuracy_k__BackingField;
                defaultRange = weapon.Range;
                defaultAmmo = weapon.weaponItem.Value;
                defaultAimDuration = weapon.AimDuration;
            }

            weapon.ImpactForce = instance.SuperImpactEnabled ? 9999f : defaultImpactForce;
            weapon.FireCooldown = instance.RapidFireEnabled ? 0f : defaultFireCooldown;
            weapon.Damage = instance.OneShotKillEnabled ? 9999f : defaultDamage;
            weapon._Accuracy_k__BackingField = instance.AimInaccuracyOverrideEnabled ? instance.AimInaccuracyValue : defaultAccuracy;
            weapon.Range = instance.ESPRangeOverrideEnabled ? MainModState.BulletSpeed : defaultRange;
            weapon.weaponItem.Value = instance.UnlimitedAmmoEnabled ? weapon.MagazineSize + 93 : defaultAmmo;

            weapon.AccuracyChangeDuration = 0.1f;
            weapon.aimVelocity = 300f;
            weapon.Aim = 1f;
            weapon.AimDuration = 0.02f;
        }

        private static bool AimbotVisible(Transform target, Vector3 position)
        {
            if (target == null || Aimbot.mainCamera == null) return false;

            Vector3 origin = Aimbot.mainCamera.transform.position;
            Vector3 direction = (position - origin).normalized;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, 500f))
            {
                return hit.transform == target || hit.transform.IsChildOf(target);
            }

            return false;
        }

        private static void SmoothRotateTowards(Transform transform, Vector3 direction)
        {
            if (transform == null) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotateSpeed = Mathf.Max(300f, MainModState.SmoothFactor * 10f);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }
    }
}
