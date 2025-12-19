using EFT;
using EFT.Hideout;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;
using Newtonsoft.Json;
using SPT.Common.Http;
using System;
using Comfort.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

namespace VulcanCore
{
    [HarmonyPatch(typeof(ShootingRangeTargetResourceManager), nameof(ShootingRangeTargetResourceManager.method_2))]
    public class VulcanCore_ShootingRangeTarget_Patch
    {
        public static bool Prefix(ShootingRangeTargetResourceManager __instance, ResourceKey resourceKey, EHideoutCustomizationType customizationType)
        {
            if (customizationType == EHideoutCustomizationType.ShootingRangeMark)
            {
                string text = resourceKey.ToAssetName();
                if (text != null && __instance.Icons.ShootingRangeMarkTextures.TryGetValue(text, out var value))
                {
                    __instance.HideoutCustomizationItemsInstaller_0.SetPaperTargetTexture(value);
                }
                else if (text != null && GetCustomShootingRangeTargetTexture(text) != null)
                {
                    __instance.HideoutCustomizationItemsInstaller_0.SetPaperTargetTexture(GetCustomShootingRangeTargetTexture(text));
                }
                else
                {
                    Debug.LogError($"Can't find texture for paper target with id: {resourceKey}");
                }
            }
            else
            {
                StashItemModel stashItemModel = GClass1857.InstantiateAsset<StashItemModel>(Singleton<IEasyAssets>.Instance, resourceKey);
                if (stashItemModel != null)
                {
                    __instance.HideoutCustomizationItemsInstaller_0.SetItem(customizationType, stashItemModel);
                }
                else
                {
                    Debug.LogError($"Could not instantiate prefab with path: {resourceKey}");
                }
            }
            return false;
        }
        public static Texture2D Target_Killa = VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Killa.png"); 
        public static Texture2D Target_Tagilla = VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Tagilla.png");
        public static Texture2D Target_Tagilla_Red = VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Tagilla_Red.png");
        public static Texture2D Target_Knight = VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Knight.png");
        public static Texture2D Target_Rat = VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Rat.png");
        public static Texture2D Target_Zryachiy = VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Zryachiy.png");
        public static Texture2D Target_Zryachiy_Green = VulcanCore_Utils.LoadFromFile("vulcan_resource/Target_Zryachiy_Green.png"); 
        public static Sprite Target_Killa_Icon;
        public static Sprite Target_Tagilla_Icon;
        public static Sprite Target_Tagilla_Red_Icon;
        public static Sprite Target_Knight_Icon;
        public static Sprite Target_Rat_Icon;
        public static Sprite Target_Zryachiy_Icon;
        public static Sprite Target_Zryachiy_Green_Icon;
        public static Texture2D GetCustomShootingRangeTargetTexture(string resourcekey)
        {
            switch (resourcekey)
            {
                case "Target_Killa":
                    return Target_Killa;
                case "Target_Tagilla":
                    return Target_Tagilla;
                case "Target_Tagilla_Red":
                    return Target_Tagilla_Red;
                case "Target_Knight":
                    return Target_Knight;
                case "Target_Rat":
                    return Target_Rat;
                case "Target_Zryachiy":
                    return Target_Zryachiy;
                case "Target_Zryachiy_Green":
                    return Target_Zryachiy_Green;
            }
            return null;
        }
    }
}
