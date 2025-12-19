using System;
using EFT;
using System.Reflection;
using EFT.InventoryLogic;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using SPT.SinglePlayer;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;


namespace VulcanCore
{
    public class VulcanCore_Utils
    {
        public static string dllPath = Assembly.GetExecutingAssembly().Location;
        public static string pluginDir = Path.GetDirectoryName(dllPath);
        public static class ObjectIdUtils
        {
            // 正则：匹配「」包裹的24位十六进制字符串
            private static readonly Regex ObjectIdRegex = new Regex("「([0-9a-fA-F]{24})」", RegexOptions.Compiled);

            /// <summary>
            /// 判断字符串是否包含至少一个 ObjectId
            /// </summary>
            public static bool ContainsObjectId(string input)
            {
                if (string.IsNullOrEmpty(input)) return false;
                return ObjectIdRegex.IsMatch(input);
            }

            /// <summary>
            /// 提取字符串中第一个 ObjectId，如果没有匹配返回 null
            /// </summary>
            public static string ExtractFirstObjectId(string input)
            {
                if (string.IsNullOrEmpty(input)) return null;

                Match match = ObjectIdRegex.Match(input);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return null;
            }

            /// <summary>
            /// 提取字符串中所有被「」包裹的 24 位哈希 ObjectId，并判断是否包含指定目标
            /// </summary>
            /// <param name="input">输入字符串</param>
            /// <param name="target">要查找的目标 ObjectId</param>
            /// <returns>true 如果目标 ObjectId 在提取结果中，否则 false</returns>
            public static bool ContainsWrappedObjectId(string input, string target)
            {
                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(target))
                    return false;

                MatchCollection matches = ObjectIdRegex.Matches(input);
                foreach (Match match in matches)
                {
                    if (match.Groups[1].Value == target)
                        return true;
                }

                return false;
            }
        }
        public static Sprite SimpleCreateSprite(Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
        }
        public static Sprite SimpleCreateSprite(Texture2D tex, float pixelsPerUnit)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }
        public static Texture2D LoadFromFile(string path, int width = 2, int height = 2)
        {
            string pathes = Path.Combine(pluginDir, path);
            if (!File.Exists(pathes))
            {
                Debug.LogError($"File not found: {pathes}");
                return null;
            }
            byte[] bytes = File.ReadAllBytes(pathes);
            var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            //tex.globalMipMapLimit = 0;
            tex.LoadImage(bytes);
            return tex;
        }
        public static Sprite GetCustomSprite(string id)
        {
            switch (id)
            {
                // --- Ending01 ---
                case "Story_Ending_01_Floor":
                    return VulcanCore_GetSprite_Patch.Story_Ending_01.Floor;

                ///*
                case "Story_Ending_01_Ceiling":
                    return VulcanCore_GetSprite_Patch.Story_Ending_01.Ceiling;

                case "Story_Ending_01_Wall":
                    return VulcanCore_GetSprite_Patch.Story_Ending_01.Wall;

                // --- Ending02 ---
                case "Story_Ending_02_Floor":
                    return VulcanCore_GetSprite_Patch.Story_Ending_02.Floor;

                case "Story_Ending_02_Ceiling":
                    return VulcanCore_GetSprite_Patch.Story_Ending_02.Ceiling;

                case "Story_Ending_02_Wall":
                    return VulcanCore_GetSprite_Patch.Story_Ending_02.Wall;
                    break;

                // --- Ending03 ---
                case "Story_Ending_03_Floor":
                    return VulcanCore_GetSprite_Patch.Story_Ending_03.Floor;

                case "Story_Ending_03_Ceiling":
                    return VulcanCore_GetSprite_Patch.Story_Ending_03.Ceiling;

                case "Story_Ending_03_Wall":
                    return VulcanCore_GetSprite_Patch.Story_Ending_03.Wall;

                // --- Ending04 ---
                case "Story_Ending_04_Floor":
                    return VulcanCore_GetSprite_Patch.Story_Ending_04.Floor;

                case "Story_Ending_04_Ceiling":
                    return VulcanCore_GetSprite_Patch.Story_Ending_04.Ceiling;

                case "Story_Ending_04_Wall":
                    return VulcanCore_GetSprite_Patch.Story_Ending_04.Wall;
                //custom

                case "Custom_01_Floor":
                    return VulcanCore_GetSprite_Patch.Custom_01_Floor;
                //target
                case "Target_Killa_Icon":
                    return VulcanCore_ShootingRangeTarget_Patch.Target_Killa_Icon;

                case "Target_Tagilla_Icon":
                    return VulcanCore_ShootingRangeTarget_Patch.Target_Tagilla_Icon;

                case "Target_Tagilla_Red_Icon":
                    return VulcanCore_ShootingRangeTarget_Patch.Target_Tagilla_Red_Icon;

                case "Target_Knight_Icon":
                    return VulcanCore_ShootingRangeTarget_Patch.Target_Knight_Icon;

                case "Target_Rat_Icon":
                    return VulcanCore_ShootingRangeTarget_Patch.Target_Rat_Icon;

                case "Target_Zryachiy_Icon":
                    return VulcanCore_ShootingRangeTarget_Patch.Target_Zryachiy_Icon;

                case "Target_Zryachiy_Green_Icon":
                    return VulcanCore_ShootingRangeTarget_Patch.Target_Zryachiy_Green_Icon;
            }
            return null;
        }
    }
}
