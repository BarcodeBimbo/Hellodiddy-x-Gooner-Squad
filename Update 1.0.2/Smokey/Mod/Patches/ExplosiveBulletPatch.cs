using HarmonyLib;
using Il2CppScheduleOne.Combat;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Equipping;
using Il2CppScheduleOne.PlayerScripts;
using System;
using UnityEngine;

namespace HelloDiddy.Mod.Patches
{

    [HarmonyPatch(typeof(Equippable_RangedWeapon), nameof(Equippable_RangedWeapon.Fire))]
    public static class ExplosiveBulletPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Equippable_RangedWeapon __instance)
        {
            if (!MainModState.ExplosiveBulletsEnabled)
                return;

            Vector3 origin = PlayerSingleton<PlayerCamera>.Instance.transform.position +
                             PlayerSingleton<PlayerCamera>.Instance.transform.forward * 0.4f;

            Vector3 direction = PlayerSingleton<PlayerCamera>.Instance.transform.forward;

            RaycastHit[] hits = Physics.SphereCastAll(origin, __instance.RayRadius, direction, __instance.Range,
                NetworkSingleton<CombatManager>.Instance.RangedWeaponLayerMask);

            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
                if (target != null && target != Player.Local.GetComponentInParent<IDamageable>())
                {
                    Vector3 impactPoint = hit.point;

                    ExplosionData explosionData = new ExplosionData(10f, 1000f, 2000f);
                    NetworkSingleton<CombatManager>.Instance.CreateExplosion(impactPoint, explosionData);

                    break;
                }
            }
        }
    }
}
