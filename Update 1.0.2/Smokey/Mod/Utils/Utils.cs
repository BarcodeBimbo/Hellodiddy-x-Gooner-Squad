using MelonLoader;
using System;
using System.Collections.Generic;

namespace HelloDiddy.Mod.Utils
{
    public class Utils
    {
        public static Dictionary<string, List<string>> LoadItemsFromJson()
        {
            Dictionary<string, List<string>> itemTree = new Dictionary<string, List<string>>();
            try
            {
                string[] categories = MainModState.itemsJson.Split(new string[] { "\"category\":" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cat in categories)
                {
                    if (!cat.Contains("\"items\":"))
                        continue;
                    string[] parts = cat.Split('"');
                    if (parts.Length < 2)
                    {
                        MelonLogger.Error("Failed to parse category name.");
                        continue;
                    }
                    string categoryName = parts[1];
                    string[] itemParts = cat.Split(new string[] { "\"items\": [" }, StringSplitOptions.None);
                    if (itemParts.Length < 2)
                    {
                        MelonLogger.Error("Failed to find item list for category " + categoryName);
                        continue;
                    }
                    itemParts[1] = itemParts[1].Replace("]", "").Replace("}", "");
                    string itemsPart = itemParts[1];
                    List<string> items = new List<string>();
                    foreach (string item in itemsPart.Split(new char[] { '"', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimmedItem = item.Trim();
                        if (trimmedItem == "[" || trimmedItem == "]" || trimmedItem == "{" || trimmedItem == "}")
                            continue;
                        if (!string.IsNullOrWhiteSpace(trimmedItem))
                        {
                            items.Add(trimmedItem);
                        }
                    }
                    itemTree[categoryName] = items;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Error parsing embedded JSON: " + ex);
            }
            return itemTree;
        }

        public static bool CheckMainScene(string sceneName, bool unloading)
        {
            if (sceneName == "Main" && unloading)
                return false;
            else if (sceneName == "Main" && !unloading)
                return true;
            return false;
        }

        public class HelloDiddyxGooner
        {
            private bool _state;

            public HelloDiddyxGooner(bool initialState = false) => _state = initialState;      
            public bool SetState(bool newState)
            {
                if (newState != _state)
                {
                    _state = newState;
                    return true;
                }
                return false;
            }

            public bool GetState() => _state;
        }
    }
}
