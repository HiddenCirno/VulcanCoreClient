using System;
using EFT;
using System.Reflection;
using EFT.InventoryLogic;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using SPT.SinglePlayer;
using System.Text;
using System.Linq;
using EFT.Interactive;
using Diz.LanguageExtensions;
using EFT.Communications;
using static UnityEngine.TouchScreenKeyboard;


namespace VulcanCore
{
    [HarmonyPatch(typeof(PlayerOwner), "GetKey")]
    public class VulcanCore_PlayerOwner_GetKey
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerOwner __instance, WorldInteractiveObject worldInteractiveObject, ref KeyComponent __result)
        {
            // 获取所有符合条件的仿制钥匙
            var fakekeys = ItemManager.GetItemComponentsInChildren<KeyComponent>(
                __instance.Player.InventoryController.Inventory.Equipment,
                onlyMerged: false
            ).Where(x =>
            {
                string description = LocaleManagerClass.LocaleManagerClass.method_4(x.Template.KeyId + " Description");
                if (VulcanCore_Utils.ObjectIdUtils.ContainsObjectId(description))
                {
                    string targetId = VulcanCore_Utils.ObjectIdUtils.ExtractFirstObjectId(description);
                    if (targetId == "A0A0A0A0FDFFFF000A0A0A0A" ||
                        VulcanCore_Utils.ObjectIdUtils.ContainsWrappedObjectId(description, worldInteractiveObject.KeyId))
                        return true;
                }
                return false;
            });

            // 选择最优仿制钥匙
            var bestFakeKey = fakekeys
                .OrderByDescending(x => x.Template.MaximumNumberOfUsage == 0) // 无限耐久排最前
                .ThenBy(x =>
                {
                    // 计算真实耐久
                    if (x.Template.MaximumNumberOfUsage == 0)
                        return int.MaxValue; // 无限耐久单独优先，不参与耐久比较
                    return x.Template.MaximumNumberOfUsage - x.NumberOfUsages;
                })
                .FirstOrDefault();

            // 普通钥匙匹配
            var normalkey = ItemManager.GetItemComponentsInChildren<KeyComponent>(
                __instance.Player.InventoryController.Inventory.Equipment,
                onlyMerged: false
            ).FirstOrDefault(x => x.Template.KeyId == worldInteractiveObject.KeyId);

            // 结果优先使用仿制钥匙
            __result = bestFakeKey ?? normalkey;

            return false; // 跳过原始方法
        }
    }
}
