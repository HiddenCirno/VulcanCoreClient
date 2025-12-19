
global using BuffEffect = GClass3019.GClass3044.GClass3045;
global using ItemManager = GClass3380;
global using ItemTransactionManager = GClass3408;
global using ItemTransactionManagerResult = GStruct154<GClass3408>;
global using LanguageExtend = GClass1522;
global using ShootingRangeTargetResourceManager = GClass2421;
global using SkillEffect = GClass1443;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using SPT.Common.Http;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace VulcanCore
{
    [BepInPlugin("com.hiddenhiragi.vulcancore", "VulcanCoreClient", "1.0.0")]
    public class VulcanCoreClient : BaseUnityPlugin
    {
        public static string dllPath = Assembly.GetExecutingAssembly().Location;
        public static string pluginDir = Path.GetDirectoryName(dllPath);
        public void Awake()
        {
            AssetBundle myBundle = AssetBundle.LoadFromFile(Path.Combine(pluginDir, "vulcan_resource/vulcan_layout.pack"));
            var myPrefab = myBundle.LoadAllAssets<GameObject>();
            foreach (var prefab in myPrefab)
            {
                var gridView = prefab.GetComponent<ContainedGridsView>();
                if (gridView != null)
                {
                    Console.WriteLine($"loading gridview");
                    var key = $"UI/Rig Layouts/{prefab.name}";
                    if (!CacheResourcesPopAbstractClass.Dictionary_0.ContainsKey(key))
                    {
                        Console.WriteLine($"gridview added successful: {key}");
                        //Console.WriteLine($"Test: {gridView.GridViews}");
                        CacheResourcesPopAbstractClass.Dictionary_0.Add(key, gridView);
                    }
                    
                }
            }
            var upgradeslot = VulcanCore_Utils.SimpleCreateSprite(VulcanCore_Utils.LoadFromFile("vulcan_resource/upgrade_slot.png", 1, 1), 100);
            var plugins = VulcanCore_Utils.SimpleCreateSprite(VulcanCore_Utils.LoadFromFile("vulcan_resource/plugins.png", 1, 1), 100);
            var pluginsadv = VulcanCore_Utils.SimpleCreateSprite(VulcanCore_Utils.LoadFromFile("vulcan_resource/pluginsadv.png", 1, 1), 100);
            CacheResourcesPopAbstractClass.Dictionary_0.Add($"Slots/mod_upgrade", upgradeslot);
            CacheResourcesPopAbstractClass.Dictionary_0.Add($"Slots/mod_plugins", plugins);
            CacheResourcesPopAbstractClass.Dictionary_0.Add($"Slots/mod_pluginsadv", pluginsadv);
            for (var i = 0; i < 20; i++)
            {
                CacheResourcesPopAbstractClass.Dictionary_0.Add($"Slots/mod_upgrade_00{i}", upgradeslot);
                CacheResourcesPopAbstractClass.Dictionary_0.Add($"Slots/mod_plugins_00{i}", plugins);
                CacheResourcesPopAbstractClass.Dictionary_0.Add($"Slots/mod_pluginsadv_00{i}", pluginsadv);
            }
            ResourceKeyManagerAbstractClass.Dictionary_0.TryAdd("Black_Division_1", "assets/content/audio/phrases/black_division_1_voice.bundle");
            ResourceKeyManagerAbstractClass.Dictionary_0.TryAdd("Black_Division_2", "assets/content/audio/phrases/black_division_2_voice.bundle");
            //ResourceKeyManagerAbstractClass.Dictionary_0["Bear_2_Eng"] = "assets/content/audio/phrases/black_division_1_voice.bundle";
            //Console.WriteLine($"{ResourceKeyManagerAbstractClass.Dictionary_0["Bear_2_Eng"]}");
            //Console.WriteLine($"{ResourceKeyManagerAbstractClass.Dictionary_0["Black_Division_1"]}");
            var harmony = new Harmony("com.hiddenhiragi.vulcancore");
            //harmony.PatchAll(typeof(VulcanCore_DogTagPatch).Assembly);
            harmony.PatchAll();
            returnClientConnect("11.13.2001-11.23.2019");
            callServerWhenGameStart("11.13.2001-11.23.2019");
            LoadAllSprites();
            VulcanCore_ShootingRangeTarget_Patch.Target_Killa_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Killa_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Tagilla_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Tagilla_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Tagilla_Red_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Tagilla_Red_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Knight_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Knight_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Rat_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Rat_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Zryachiy_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Zryachiy_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Zryachiy_Green_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Zryachiy_Green_Icon.png"));
            VulcanCore_GetSprite_Patch.Custom_01_Floor = VulcanCore_Utils.SimpleCreateSprite(VulcanCore_Utils.LoadFromFile("vulcan_resource/Custom_01_Floor.png"));
            Debug.Log("VulcanCore: Now Awake.");
        }
        private static string returnClientConnect(string request)
        {
            return RequestHandler.PostJson("/VulcanCoreClient/InitFix", JsonConvert.SerializeObject(new FixRequest(request)));
        }
        private static string callServerWhenGameStart(string request)
        {
            return RequestHandler.PostJson("/VulcanCoreClient/ClientStartCall", JsonConvert.SerializeObject(new FixRequest(request)));
        }
        private void LoadAllSprites()
        {
            VulcanCore_GetSprite_Patch.Story_Ending_01.Floor =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_01_Floor);

            VulcanCore_GetSprite_Patch.Story_Ending_01.Ceiling =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_01_Ceiling);

            VulcanCore_GetSprite_Patch.Story_Ending_01.Wall =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_01_Wall);
            VulcanCore_GetSprite_Patch.Story_Ending_02.Floor =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_02_Floor);

            VulcanCore_GetSprite_Patch.Story_Ending_02.Ceiling =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_02_Ceiling);

            VulcanCore_GetSprite_Patch.Story_Ending_02.Wall =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_02_Wall);
            VulcanCore_GetSprite_Patch.Story_Ending_03.Floor =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_03_Floor);

            VulcanCore_GetSprite_Patch.Story_Ending_03.Ceiling =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_03_Ceiling);

            VulcanCore_GetSprite_Patch.Story_Ending_03.Wall =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_03_Wall);
            VulcanCore_GetSprite_Patch.Story_Ending_04.Floor =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_04_Floor);

            VulcanCore_GetSprite_Patch.Story_Ending_04.Ceiling =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_04_Ceiling);

            VulcanCore_GetSprite_Patch.Story_Ending_04.Wall =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_GetSprite_Patch.StoryCustomizationSprite.Story_Ending_04_Wall);

        }

        private static bool loaded = false;
    }
    public class FixRequest
    {
        public FixRequest(string request)
        {
            this.request = request;
        }
        public string request;
    }

}
