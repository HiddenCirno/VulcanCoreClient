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
using System.Collections.Generic;
using static BackendDummyClass.GClass2321;


namespace VulcanCore
{
    [HarmonyPatch(typeof(KeycardDoor), "UnlockOperation")]
    public class VulcanCore_UnlockOperation_KeyCard
    {
        [HarmonyPrefix]
        public static bool Prefix(KeycardDoor __instance, KeyComponent key, Player player, WorldInteractiveObject wio, ref GStruct156<KeyInteractionResultClass> __result)
        {
            // 1. 检查玩家是否可以交互
            Error canInteract = player.MovementContext.CanInteract;
            if (canInteract != null)
            {
                __result = canInteract;
                return false;
            }

            // 2. 检查技能要求
            if (wio.HasSkillRequirement &&
                player.Skills.TryGetSkill(wio.SkillRequirement, out SkillClass skillClass) &&
                wio.SkillMinLevelRequirement < skillClass.Level)
            {
                __result = new LanguageExtend($"{wio.SkillRequirement} low level");
                return false;
            }

            // 3. 通过反射获取私有 _additionalKeys
            var additionalKeysField = typeof(KeycardDoor).GetField("_additionalKeys", BindingFlags.NonPublic | BindingFlags.Instance);
            HashSet<string> additionalKeys = additionalKeysField?.GetValue(__instance) as HashSet<string>;


            // 4. 判断钥匙是否匹配
            bool flag = key.Template.KeyId == __instance.KeyId ||
                        (additionalKeys != null && additionalKeys.Contains(key.Template.KeyId))
                        || VulcanCore_Utils.ObjectIdUtils.ExtractFirstObjectId(LocaleManagerClass.LocaleManagerClass.method_4(key.Template.KeyId + " Description")) == "A0A0A0A0FDFFFF000A0A0A0A"
                        || VulcanCore_Utils.ObjectIdUtils.ContainsWrappedObjectId(LocaleManagerClass.LocaleManagerClass.method_4(key.Template.KeyId + " Description"), __instance.KeyId);

            if (!flag)
            {
                __result = new KeyInteractionResultClass(key, null, false);
                return false;
            }

            // 5. 增加钥匙使用次数
            key.NumberOfUsages++;
            NotificationManagerClass.DisplayMessageNotification(
                    $"使用了{LocaleManagerClass.LocaleManagerClass.method_4(key.Template.KeyId.ToString() + " Name")}",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Default,
                    null
                );
            ItemTransactionManagerResult gstruct = default;

            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage &&
                key.Template.MaximumNumberOfUsage > 0)
            {
                gstruct = InteractionsHandlerClass.Discard(
                    key.Item,
                    (TraderControllerClass)key.Item.Parent.GetOwner(),
                    false
                );

                if (gstruct.Failed)
                {
                    __result = gstruct.Error;
                    return false;
                }

                NotificationManagerClass.DisplayMessageNotification(
                    $"{LocaleManagerClass.LocaleManagerClass.method_4(key.Template.KeyId.ToString() + " Name")}的次数已耗尽",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Alert,
                    null
                );
            }

            // 6. 返回解锁结果
            __result = new KeyInteractionResultClass(key, gstruct.Value, true);
            return false;
        }


    }
}
