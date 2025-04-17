using HelloDiddy.Mod.Features;
using HelloDiddy.Mod.Functions;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.PlayerScripts;
using MelonLoader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HelloDiddy.Mod
{
    public class MainModStatewip
    {

        public static MainModStatewip Instance { get; private set; }
        public static bool IsMainSceneWIP { get; set; } = false;
        public static float SoilWaterInterval => 0.5f;

        public static MelonPreferences_Entry<KeyCode> MenuKeyPreference;
        public static MelonPreferences_Entry<KeyCode> NoclipKeyPrimary;
        public static MelonPreferences_Entry<KeyCode> NoclipKeySecondary;

        public static MelonPreferences_Entry<bool> AimbotEnabled;
        public static MelonPreferences_Entry<bool> UnfairAimbotEnabled;
        public static MelonPreferences_Entry<float> UnfairAimbotDistance;
        public static MelonPreferences_Entry<bool> SilentAimbotEnabled;
        public static MelonPreferences_Entry<bool> TriggerBotEnabled;
        public static MelonPreferences_Entry<bool> VisibilityCheck;
        public static MelonPreferences_Entry<bool> MagicBulletEnabled;
        public static MelonPreferences_Entry<bool> AimSmoothingEnabled;
        public static MelonPreferences_Entry<float> SmoothFactor;
        public static MelonPreferences_Entry<float> NPCSmoothFactor;
        public static MelonPreferences_Entry<float> AimInaccuracy;
        public static MelonPreferences_Entry<bool> EnablePrediction;
        public static MelonPreferences_Entry<float> BulletSpeed;
        public static MelonPreferences_Entry<float> PredictionFactor;
        public static MelonPreferences_Entry<float> FOV;
        public static MelonPreferences_Entry<bool> DrawFovEnabled;
        public static MelonPreferences_Entry<float> FovLineThickness;
        public static MelonPreferences_Entry<float> AimInaccuracyValuw;
        public static MelonPreferences_Entry<bool> PlayerEspEnabled;
        public static MelonPreferences_Entry<bool> NpcEspEnabled;
        public static MelonPreferences_Entry<bool> ShowNames;
        public static MelonPreferences_Entry<bool> ShowDistance;
        public static MelonPreferences_Entry<bool> BonesESPEnabled;
        public static MelonPreferences_Entry<Color> PlayerBoxColor;
        public static MelonPreferences_Entry<Color> CopBoxColor;
        public static MelonPreferences_Entry<Color> NPCBoxColor;
        public static MelonPreferences_Entry<Color> BonesESPColor;
        public static MelonPreferences_Entry<bool> Use3DBoxes;
        public static MelonPreferences_Entry<float> MaxESPDistance;
        public static MelonPreferences_Entry<int> BoxModeIndex;
        public  MelonPreferences_Entry<bool> AimInaccuracyOverrideEnabled;
        public static MelonPreferences_Entry<bool> GodModeEnabled;
        public static MelonPreferences_Entry<bool> InfiniteEnergy;
        public static MelonPreferences_Entry<bool> InfiniteStamina;
        public static MelonPreferences_Entry<bool> AlwaysUnwanted;
        public MelonPreferences_Entry<bool> NoRecoilEnabled;
        public MelonPreferences_Entry<float> AimInaccuracyValue;
        public MelonPreferences_Entry<bool> NoSpreadEnabled;
        public MelonPreferences_Entry<bool> UnlimitedAmmoEnabled;
        public static MelonPreferences_Entry<bool> HighDamageEnabled;
        public static MelonPreferences_Entry<bool> HighAccuracyEnabled;
        public static MelonPreferences_Entry<float> JumpMultiplier;
        public static MelonPreferences_Entry<float> SpeedMultiplier;
        public static MelonPreferences_Entry<float> DamageMultiplier;
        public static MelonPreferences_Entry<float> FireRateMultiplier;
        public static MelonPreferences_Entry<bool> SuperPunchEnabled;
        public  MelonPreferences_Entry<bool> ExplosiveBulletsEnabled;
        public  MelonPreferences_Entry<bool> ESPRangeOverrideEnabled;
        public  MelonPreferences_Entry<bool> OneShotKillEnabled;
        public  MelonPreferences_Entry<bool> SuperImpactEnabled;
        public  MelonPreferences_Entry<bool> RapidFireEnabled;
        public static  MelonPreferences_Entry<float> SuperPunchDist;

        public static MelonPreferences_Entry<bool> TrashPatchEnabled;
        public static MelonPreferences_Entry<bool> TrashPlusPlusPatch;
        public static MelonPreferences_Entry<bool> DupPatchEnabled;
        public static MelonPreferences_Entry<int> DuplicationMultiplier;
        public static MelonPreferences_Entry<bool> StackPatchEnabled;
        public static MelonPreferences_Entry<int> ModdedStackSize;

        public static MelonPreferences_Entry<bool> AutoGrowEnabled;

        public static MelonPreferences_Entry<float> MoneyAmount;
        public static MelonPreferences_Entry<int> XpAmount;
        public Vector2 scrollPos = Vector2.zero;
        public GameObject canvasObject;
        public CanvasGroup canvasGroup;
        public Texture2D backgroundTexture;
        public Image backgroundImage;
        public bool backgroundLoaded;

        public int SpawnerItemAmount = 1;
        public string SelectedCategory = "Default";
        public Dictionary<string, List<string>> SpawnerItems { get; set; } = new Dictionary<string, List<string>>();
        public bool categoryDropdownExpanded = false;

        public int CurrentTab = 0;
        public string[] TabNames { get; set; } = { "Online", "Self", "Aimbot & ESP", "Spawner", "World", "Misc" };

        public int SelectedPlayerIndex = 0;
        public List<string> TargetPlayers = new List<string>();
        public List<string> TargetNPCs = new List<string>();
        public ESP Esp { get; set; } = new ESP();
        public float soilWaterTimer = 0f;
        public float fadeSpeed = 1f;
        public static bool isSelectingNoclipPrimary = false;
        public static bool isSelectingNoclipSecondary = false;

        public static float lastHealth = -1f;
        public static bool MultiTargetEnabled = false;

        public static bool hasTarget = false;
        public static Material fovMaterial;
        public static int SelectedGrowSeedIndex = 0;
        public static string[] AutoGrowSeeds = { "None", "granddaddypurpleseed", "greencrackseed", "ogkushseed", "sourdieselseed", "cocaseed" };
        public static Dictionary<Pot, bool> readyHarvestNotified = new Dictionary<Pot, bool>();
        public static float ProjectileSpeed = 100f;
        public static Vector3 PlayerPosition = Vector3.zero;
        public static Player LocalPlayer => Player.Local;
        public Vector2 miscTabScroll;
        public Vector2 aimbotEspScrollPos;
        public Vector2 selfTabScroll;
        public Vector2 spawnerTabScroll;
        public Vector2 spawnerItemScrollPos;
        public Vector2 worldTabScroll;
        public Vector2 playerTabScroll;

        public float harvestUpdateTimer = 0f;

        public Vector2 SpawnerCatScrollPos { get; set; } = Vector2.zero;
        public const string itemsJson = @"{ ""categories"": [ { ""category"": ""Product"", ""items"": [ ""granddaddypurple"", ""greencrack"", ""ogkush"", ""sourdiesel"", ""cocaine"", ""meth"" ] }, { ""category"": ""Packaging"", ""items"": [ ""baggie"", ""brick"", ""jar"" ] }, { ""category"": ""Growing"", ""items"": [ ""extralonglifesoil"", ""fertilizer"", ""longlifesoil"", ""pgr"", ""soil"", ""speedgrow"", ""cocaleaf"", ""granddaddypurpleseed"", ""greencrackseed"", ""ogkushseed"", ""sourdieselseed"", ""cocaseed"" ] }, { ""category"": ""Tools"", ""items"": [ ""electrictrimmers"", ""flashlight"", ""managementclipboard"", ""trashbag"", ""trashgrabber"", ""trimmers"", ""wateringcan"", ""bigsprinkler"", ""soilpourer"", ""potsprinkler"", ""baseballbat"", ""fryingpan"", ""m1911"", ""m1911mag"", ""machete"", ""revolvercylinder"", ""revolver"" ] }, { ""category"": ""Furniture"", ""items"": [ ""coffeetable"", ""dumpster"", ""suspensionrack"", ""metalsquaretable"", ""trashcan"", ""bed"", ""smalltrashcan"", ""TV"", ""woodsquaretable"" ] }, { ""category"": ""Lightning"", ""items"": [ ""fullspectrumgrowlight"", ""halogengrowlight"", ""ledgrowlight"" ] }, { ""category"": ""Consumable"", ""items"": [ ""banana"", ""cuke"", ""donut"", ""energydrink"", ""flumedicine"", ""chili"" ] }, { ""category"": ""Equipment"", ""items"": [ ""airpot"", ""plasticpot"", ""growtent"", ""moisturepreservingpot"", ""largestoragerack"", ""mediumstoragerack"", ""smallstoragerack"", ""brickpress"", ""cauldron"", ""chemistrystation"", ""dryingrack"", ""laboven"", ""launderingstation"", ""mixingstation"", ""mixingstationmk"", ""packagingstation"", ""packagingstationmk"" ] }, { ""category"": ""Ingredient"", ""items"": [ ""acid"", ""addy"", ""battery"", ""gasoline"", ""horsesemen"", ""motoroil"", ""mouthwash"", ""paracetamol"", ""viagra"" ] }, { ""category"": ""Decoration"", ""items"": [ ""floorlamp"", ""displaycabinet"", ""filingcabinet"" ] }, { ""category"": ""Clothing"", ""items"": [ ""cargopants"", ""jeans"", ""jorts"", ""longskirt"", ""overalls"", ""skirt"", ""legendsunglasses"", ""rectangleframeglasses"", ""smallroundglasses"", ""speeddealershades"", ""combatboots"", ""dressshoes"", ""flats"", ""sandals"", ""sneakers"", ""fingerlessgloves"", ""gloves"", ""buckethat"", ""cap"", ""chefhat"", ""cowboyhat"", ""flatcap"", ""porkpiehat"", ""saucepan"", ""apron"", ""blazer"", ""collarjacket"", ""tacticalvest"", ""vest"", ""buttonup"", ""rolledbuttonup"", ""flannelshirt"", ""tshirt"", ""vneck"", ""belt"" ] }, { ""category"": ""Vehicle"", ""items"": [ ""cheapskateboard"", ""cruiser"", ""goldenskateboard"", ""lightweightskateboard"", ""skateboard"" ] }, { ""category"": ""Special"", ""items"": [ ""chateaulapeepee"", ""goldbar"", ""oldmanjimmys"", ""cocainebase"", ""iodine"", ""liquidbabyblue"", ""liquidbikercrank"", ""liquidglass"", ""liquidmeth"", ""megabean"", ""phosphorus"", ""babyblue"", ""bikercrank"", ""glass"", ""testweed"" ] } ] }";
        public MainModStatewip() => Instance = this;
    }
        
}
