using EFT.Hideout;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using System;
using UnityEngine;
using Newtonsoft.Json;
using SPT.Common.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine.Windows;

namespace VulcanCore
{
    [HarmonyPatch(typeof(HideoutCustomizationIcons), nameof(HideoutCustomizationIcons.GetSprite))]
    public class VulcanCore_GetSprite_Patch
    {
        public static void Postfix(string id, ref Sprite __result)
        {
            var customresult = VulcanCore_Utils.GetCustomSprite(id);
            if (customresult != null)
            {
                __result = customresult;
            }
        }
        public static CustomizationSpriteData Story_Ending_01 = new CustomizationSpriteData();
        public static CustomizationSpriteData Story_Ending_02 = new CustomizationSpriteData();
        public static CustomizationSpriteData Story_Ending_03 = new CustomizationSpriteData();
        public static CustomizationSpriteData Story_Ending_04 = new CustomizationSpriteData();
        public static Sprite Custom_01_Floor;
        public static CustomizationSpriteData CreateStoryCustomizationSpriteData(Texture2D floor, Texture2D ceiling, Texture2D wall)
        {
            return new CustomizationSpriteData
            {
                Floor = VulcanCore_Utils.SimpleCreateSprite(floor),
                Ceiling = VulcanCore_Utils.SimpleCreateSprite(ceiling),
                Wall = VulcanCore_Utils.SimpleCreateSprite(wall),
            };
        }
        public static class StoryCustomizationSprite
        {
            // --- 结局01 ---
            public static Texture2D Story_Ending_01_Floor = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_01_Floor.png");
            public static Texture2D Story_Ending_01_Ceiling = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_01_Ceiling.png");
            public static Texture2D Story_Ending_01_Wall = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_01_Wall.png");
            // --- 结局02 ---
            public static Texture2D Story_Ending_02_Floor = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_02_Floor.png");
            public static Texture2D Story_Ending_02_Ceiling = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_02_Ceiling.png");
            public static Texture2D Story_Ending_02_Wall = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_02_Wall.png");
            // --- 结局03 ---
            public static Texture2D Story_Ending_03_Floor = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_03_Floor.png");
            public static Texture2D Story_Ending_03_Ceiling = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_03_Ceiling.png");
            public static Texture2D Story_Ending_03_Wall = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_03_Wall.png");
            // --- 结局04 ---
            public static Texture2D Story_Ending_04_Floor = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_04_Floor.png");
            public static Texture2D Story_Ending_04_Ceiling = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_04_Ceiling.png");
            public static Texture2D Story_Ending_04_Wall = VulcanCore_Utils.LoadFromFile("hideoutsprite/Story_Ending_04_Wall.png");
        }
    }
    public class CustomizationSpriteData
    {
        public Sprite Floor;
        public Sprite Ceiling;
        public Sprite Wall;
    }
}
