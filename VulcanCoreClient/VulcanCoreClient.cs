
global using SkillEffect = GClass1443;
global using BuffEffect = GClass3019.GClass3044.GClass3045;
global using ItemManager = GClass3380;
global using LanguageExtend = GClass1522;
global using ShootingRangeTargetResourceManager = GClass2421;
global using ItemTransactionManager = GClass3408;
global using ItemTransactionManagerResult = GStruct154<GClass3408>;
using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using SPT.Common.Http;
using UnityEngine;

namespace VulcanCore
{
    [BepInPlugin("com.hiddenhiragi.vulcancore", "VulcanCoreClient", "1.0.0")]
    public class VulcanCoreClient : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("com.hiddenhiragi.vulcancore");
            //harmony.PatchAll(typeof(VulcanCore_DogTagPatch).Assembly);
            harmony.PatchAll();
            returnClientConnect("11.13.2001-11.23.2019");
            LoadAllSprites();
            VulcanCore_ShootingRangeTarget_Patch.Target_Killa_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("hideoutsprite/Target_Killa_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Tagilla_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("hideoutsprite/Target_Tagilla_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Tagilla_Red_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("hideoutsprite/Target_Tagilla_Red_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Knight_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("hideoutsprite/Target_Knight_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Rat_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("hideoutsprite/Target_Rat_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Zryachiy_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("hideoutsprite/Target_Zryachiy_Icon.png"));

            VulcanCore_ShootingRangeTarget_Patch.Target_Zryachiy_Green_Icon =
                VulcanCore_Utils.SimpleCreateSprite(
                    VulcanCore_Utils.LoadFromFile("hideoutsprite/Target_Zryachiy_Green_Icon.png"));
            VulcanCore_GetSprite_Patch.Custom_01_Floor = VulcanCore_Utils.SimpleCreateSprite(VulcanCore_Utils.LoadFromFile("hideoutsprite/Custom_01_Floor.png"));
            Debug.Log("VulcanCore: Now Awake.");
        }
        private static string returnClientConnect(string request)
        {
            return RequestHandler.PostJson("/VulcanCoreClient/InitFix", JsonConvert.SerializeObject(new FixRequest(request)));
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
