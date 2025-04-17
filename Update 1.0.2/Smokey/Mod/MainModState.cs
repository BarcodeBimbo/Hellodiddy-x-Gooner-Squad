using HelloDiddy.Mod.Features;
using HelloDiddy.Mod.Functions;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;

namespace HelloDiddy.Mod
{
    public class MainModState
    {

        //this will be cleaned and a config system will remove most of this this looks like junk 
        public static MainModState Instance { get; private set; }
        public static bool IsMainScene { get; set; } = false;
        public static float SuperPunchDist { get; set; } = 1f;
        public static float SelectedTime { get; set; } = 1f;
        public static float SoilWaterInterval => 0.5f;
        public static float lastHealth = -1f;
        public const float GodModeHealthValue = float.MaxValue;
        public static Player LocalPlayer => Player.Local;
        public static MelonPreferences_Entry<KeyCode> MenuKeyPreference;
        public static bool isSelectingMenuKey = false;
        public static bool Use3DBoxes = true;
        public static bool BonesESPEnabled = false;
        public static bool ShowNames = true;
        public static bool ShowDistance = true;
        public static Color PlayerBoxColor = new Color(0.6f, 0.4f, 1.0f);
        public static Color CopBoxColor = new Color(0.3f, 0.6f, 1.0f);
        public static Color NPCBoxColor = new Color(1.0f, 0.8f, 0.2f);
        public static float MaxESPDistance = 75f;
        public static Color BonesESPColor = Color.cyan;
        public static bool SuperPunchEnabled = false;
        public static bool UnfairAimbotEnabled = false;
        public static float UnfairAimbotDistance = 50f;
        public static bool AimbotEnabled = false;
        public static bool SilentAimbotEnabled = false;
        public static bool VisibilityCheck = false;
        public static bool DrawFovEnabled = true;
        public static bool MultiTargetEnabled = false;
        public static bool AimSmoothingEnabled = false;
        public static bool MagicBulletEnabled = false;
        public static bool IsAimbotSuppressed = false;
        public static bool BulletCorrectionEnabled = true;
        public static float SmoothFactor = 80f;
        public static float NPCSmoothFactor = 70f;
        public static float AimInaccuracy = 0f;
        public static bool EnablePrediction = true;
        public static float BulletSpeed = 50f;
        public static float PredictionFactor = 0.5f;
        public static float FOV = 30f;
        public static bool TriggerBotEnabled = false;
        public static bool LegitModeEnabled = false;
        public static bool hasTarget = false;
        public static Material fovMaterial;
        public static float FovLineThickness = 0.5f;
        public static bool AutoGrowEnabled = false;
        public static int SelectedGrowSeedIndex = 0;
        public static string[] AutoGrowSeeds = { "None", "granddaddypurpleseed", "greencrackseed", "ogkushseed", "sourdieselseed", "cocaseed" };

        public static Dictionary<Pot, bool> readyHarvestNotified = new Dictionary<Pot, bool>();
        public static float ProjectileSpeed = 100f;
        public static Vector3 PlayerPosition = Vector3.zero;


        public static bool TargetPlayers = true;
        public static bool TargetNPCs = true;
        public static float AimbotMaxDistance = 250f;
        public static bool ExplosiveBulletsEnabled = false;
        public bool GodModeEnabled { get; set; }
        public bool AlwaysUnwanted { get; set; }
        public bool UnlimitedAmmoEnabled { get; set; }
        public bool InfiniteStamina { get; set; }
        public bool InfiniteEnergy { get; set; }
        public bool NoRecoilEnabled { get; set; }
        public bool NoSpreadEnabled { get; set; }
        public bool RapidFireEnabled { get; set; }
        public bool HighDamageEnabled { get; set; }
        public bool HighAccuracyEnabled { get; set; }
        public bool SuperImpactEnabled { get; set; }
        public bool OneShotKillEnabled { get; set; }
        public bool isDoneMixing { get; set; }
        public bool AimInaccuracyOverrideEnabled { get; set; }
        public bool ESPRangeOverrideEnabled { get; set; }
        public bool PlayerEspEnabled { get; set; }
        public bool NpcEspEnabled { get; set; }
        public bool TrashPatchEnabled { get; set; }
        public bool TrashPlusPlusPatch { get; set; }
        public bool DupPatchEnabled { get; set; }
        public bool StackPatchEnabled { get; set; }
        public bool AutoSoilAndWaterEnabled { get; set; }
        public float JumpMultiplier { get; set; } = 1f;
        public float SpeedMultiplier { get; set; } = 1f;
        public float DamageMultiplier { get; set; } = 10f;
        public float DesiredAccuracy { get; set; } = 9999f;
        public float AimInaccuracyValue { get; set; } = 50f;
        public float ESPRangeValue { get; set; } = 500f;
        public float FireRateMultiplier { get; set; } = 2f;
        public float FadeSpeed { get; set; } = 5f;
        public Dictionary<string, List<string>> SpawnerItems { get; set; } = new Dictionary<string, List<string>>();
        public Vector2 SpawnerCatScrollPos { get; set; } = Vector2.zero;
        public string SelectedCategory { get; set; } = "";
        public int SpawnerItemAmount { get; set; } = 1;
        public int SelectedPlayerIndex { get; set; } = -1;
        public string[] TabNames { get; set; } = { "Online", "Self", "Aimbot & ESP", "Spawner", "World", "Misc" };
        public int CurrentTab { get; set; } = 0;
        public int BoxModeIndex { get; set; } = 1;
        public float MoneyAmount { get; set; } = 100f;
        public int XpAmount { get; set; } = 100;
        public int DuplicationMultiplier { get; set; } = 2;
        public int ModdedStackSize { get; set; } = 20;
        public ESP Esp { get; set; } = new ESP();
        public Texture2D backgroundTexture;
        public bool backgroundLoaded = false;
        public bool categoryDropdownExpanded = false;
        public Vector2 spawnerItemScrollPos = Vector2.zero;
        public Vector2 scrollPos = Vector2.zero;
        public Vector2 playerTabScroll = Vector2.zero;
        public Vector2 selfTabScroll = Vector2.zero;
        public Vector2 aimbotEspScrollPos = Vector2.zero;
        public Vector2 worldTabScroll = Vector2.zero;
        public Vector2 miscTabScroll = Vector2.zero;
        public Vector2 spawnerTabScroll = Vector2.zero;
        public static MelonPreferences_Entry<KeyCode> NoclipKeyPrimary;
        public static MelonPreferences_Entry<KeyCode> NoclipKeySecondary;
        public static bool isSelectingNoclipPrimary = false;
        public static bool isSelectingNoclipSecondary = false;
        public GameObject canvasObject;
        public UnityEngine.UI.Image backgroundImage;
        public CanvasGroup canvasGroup;
        public float fadeSpeed = 5f;
        public float harvestUpdateTimer = 0f;
        public float soilWaterTimer = 0f;
        public float lobbyCapacity = 4f;

        public const string itemsJson = @"{ ""categories"": [ { ""category"": ""Product"", ""items"": [ ""granddaddypurple"", ""greencrack"", ""ogkush"", ""sourdiesel"", ""cocaine"", ""meth"" ] }, { ""category"": ""Packaging"", ""items"": [ ""baggie"", ""brick"", ""jar"" ] }, { ""category"": ""Growing"", ""items"": [ ""extralonglifesoil"", ""fertilizer"", ""longlifesoil"", ""pgr"", ""soil"", ""speedgrow"", ""cocaleaf"", ""granddaddypurpleseed"", ""greencrackseed"", ""ogkushseed"", ""sourdieselseed"", ""cocaseed"" ] }, { ""category"": ""Tools"", ""items"": [ ""electrictrimmers"", ""flashlight"", ""managementclipboard"", ""trashbag"", ""trashgrabber"", ""trimmers"", ""wateringcan"", ""bigsprinkler"", ""soilpourer"", ""potsprinkler"", ""baseballbat"", ""fryingpan"", ""m1911"", ""m1911mag"", ""machete"", ""revolvercylinder"", ""revolver"" ] }, { ""category"": ""Furniture"", ""items"": [ ""coffeetable"", ""dumpster"", ""suspensionrack"", ""metalsquaretable"", ""trashcan"", ""bed"", ""smalltrashcan"", ""TV"", ""woodsquaretable"" ] }, { ""category"": ""Lightning"", ""items"": [ ""fullspectrumgrowlight"", ""halogengrowlight"", ""ledgrowlight"" ] }, { ""category"": ""Consumable"", ""items"": [ ""banana"", ""cuke"", ""donut"", ""energydrink"", ""flumedicine"", ""chili"" ] }, { ""category"": ""Equipment"", ""items"": [ ""airpot"", ""plasticpot"", ""growtent"", ""moisturepreservingpot"", ""largestoragerack"", ""mediumstoragerack"", ""smallstoragerack"", ""brickpress"", ""cauldron"", ""chemistrystation"", ""dryingrack"", ""laboven"", ""launderingstation"", ""mixingstation"", ""mixingstationmk"", ""packagingstation"", ""packagingstationmk"" ] }, { ""category"": ""Ingredient"", ""items"": [ ""acid"", ""addy"", ""battery"", ""gasoline"", ""horsesemen"", ""motoroil"", ""mouthwash"", ""paracetamol"", ""viagra"" ] }, { ""category"": ""Decoration"", ""items"": [ ""floorlamp"", ""displaycabinet"", ""filingcabinet"" ] }, { ""category"": ""Clothing"", ""items"": [ ""cargopants"", ""jeans"", ""jorts"", ""longskirt"", ""overalls"", ""skirt"", ""legendsunglasses"", ""rectangleframeglasses"", ""smallroundglasses"", ""speeddealershades"", ""combatboots"", ""dressshoes"", ""flats"", ""sandals"", ""sneakers"", ""fingerlessgloves"", ""gloves"", ""buckethat"", ""cap"", ""chefhat"", ""cowboyhat"", ""flatcap"", ""porkpiehat"", ""saucepan"", ""apron"", ""blazer"", ""collarjacket"", ""tacticalvest"", ""vest"", ""buttonup"", ""rolledbuttonup"", ""flannelshirt"", ""tshirt"", ""vneck"", ""belt"" ] }, { ""category"": ""Vehicle"", ""items"": [ ""cheapskateboard"", ""cruiser"", ""goldenskateboard"", ""lightweightskateboard"", ""skateboard"" ] }, { ""category"": ""Special"", ""items"": [ ""chateaulapeepee"", ""goldbar"", ""oldmanjimmys"", ""cocainebase"", ""iodine"", ""liquidbabyblue"", ""liquidbikercrank"", ""liquidglass"", ""liquidmeth"", ""megabean"", ""phosphorus"", ""babyblue"", ""bikercrank"", ""glass"", ""testweed"" ] } ] }";

        public MainModState() => Instance = this;
    }
}
