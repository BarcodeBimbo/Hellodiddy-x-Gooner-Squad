using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using UnityEngine;

namespace HelloDiddy.Mod.UI
{
    public static class NotificationUtils
    {
        private static readonly string IconPath = Path.Combine(
            MelonEnvironment.GameRootDirectory, "Mods", "GoonerSquad", "HelloDiddy.png"
        );

        public static Sprite? LoadNotificationSprite()
        {
            if (!File.Exists(IconPath))
            {
                MelonLogger.Warning($"Notification icon not found at: {IconPath}");
                return null;
            }

            var texture = LoadTexture(IconPath);
            return texture == null
                ? null
                : Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static void CreateNotification(string title, string subtitle, Sprite icon, float duration = 5f, bool playSound = true)
        {
            var container = GameObject.Find("UI/HUD/NotificationContainer");
            if (container == null)
            {
                MelonLogger.Warning("UI container 'NotificationContainer' not found.");
                return;
            }

            var manager = container.GetComponent<Il2CppScheduleOne.UI.NotificationsManager>();
            if (manager == null)
            {
                MelonLogger.Warning("NotificationsManager component missing on container.");
                return;
            }

            manager.SendNotification(title, subtitle, icon, duration, playSound);
        }

        private static Texture2D? LoadTexture(string filePath)
        {
            try
            {
                var data = File.ReadAllBytes(filePath);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!texture.LoadImage(data))
                {
                    MelonLogger.Error($"Texture failed to load: LoadImage returned false for '{filePath}'.");
                    return null;
                }

                return texture;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Exception while loading texture from '{filePath}': {ex.Message}");
                return null;
            }
        }
    }
}
