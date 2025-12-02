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
    [HarmonyPatch(typeof(WorldInteractiveObject), "UnlockOperation")]
    public class VulcanCore_UnlockOperation_World
    {
        [HarmonyPrefix]
        public static bool Prefix(
        WorldInteractiveObject __instance,
        KeyComponent key,
        Player player,
        WorldInteractiveObject wio,
        ref GStruct156<KeyInteractionResultClass> __result)
        {
            // === 1. 玩家是否能交互 ===
            Error canInteract = player.MovementContext.CanInteract;
            if (canInteract != null)
            {
                __result = canInteract;
                return false; // 跳过原始
            }


            // === 2. 技能等级要求 ===
            SkillClass skillClass;
            if (wio.HasSkillRequirement
                && player.Skills.TryGetSkill(wio.SkillRequirement, out skillClass)
                && wio.SkillMinLevelRequirement < skillClass.Level)
            {
                __result = new LanguageExtend($"{wio.SkillRequirement} low level");
                return false;
            }

            /*
            // === 3. 钥匙匹配检查 ===
            if (!(key.Template.KeyId == __instance.KeyId))
            {
                __result = new GClass3854("Key doesn't match");
                return false;
            }
            */

            // === 4. 使用钥匙 ===
            ItemTransactionManagerResult gstruct = default(ItemTransactionManagerResult);
            key.NumberOfUsages++;
            NotificationManagerClass.DisplayMessageNotification(
                    $"使用了{LocaleManagerClass.LocaleManagerClass.method_4(key.Template.KeyId.ToString() + " Name")}",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Default,
                    null
                );

            // 钥匙耐久耗尽 → 丢弃
            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage
                && key.Template.MaximumNumberOfUsage > 0)
            {
                gstruct = InteractionsHandlerClass.Discard(
                    key.Item,
                    (TraderControllerClass)key.Item.Parent.GetOwner(),
                    false);

                if (gstruct.Failed)
                {
                    __result = gstruct.Error;
                    return false;
                }

                // 🔔 在这里加提示：钥匙用尽
                //SendClientNotification(player, key);
                NotificationManagerClass.DisplayMessageNotification(
                    $"{LocaleManagerClass.LocaleManagerClass.method_4(key.Template.KeyId.ToString() + " Name")}的次数已耗尽",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Alert,
                    null
                );
            }

            // === 5. 返回成功 ===
            __result = new KeyInteractionResultClass(key, gstruct.Value, true);
            return false; // 跳过原始方法
        }


    }
}
