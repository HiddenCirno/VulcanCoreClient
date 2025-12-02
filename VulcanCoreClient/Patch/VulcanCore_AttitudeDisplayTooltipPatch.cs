using System;
using EFT;
using System.Reflection;
using EFT.InventoryLogic;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using SPT.SinglePlayer;
using System.Text;


namespace VulcanCore
{
    [HarmonyPatch(typeof(SkillEffect), "GetFullStringValue")]
    public class VulcanCore_AttitudeDisplayTooltipPatch
    {
        [HarmonyPrefix]
        public static bool GetFullStringValue_Prefix(SkillEffect __instance, string displayName, ref string __result)
        {
            // 保持原有的空值检查逻辑
            if (__instance.Delay.IsZero() && __instance.Duration.IsZero() && __instance.Cost == 0)
            {
                __result = string.Empty;
                return false;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(displayName.Localized(null));

            // 延迟显示保持不变
            if (__instance.Delay > 1f)
            {
                stringBuilder.Append(string.Format("\n{0} {1}{2}",
                    "Delay".Localized(null),
                    __instance.Delay,
                    "sec".Localized(null)));
            }

            // 修改后的持续时间显示逻辑
            if (__instance.Duration > 0f)
            {
                string durationValue = __instance.Duration >= 7200f
                    ? "Infinity".Localized(null)
                    : __instance.Duration.ToString() + "sec".Localized(null);

                stringBuilder.Append(string.Format("\n{0} {1}",
                    "Duration".Localized(null),
                    durationValue));
            }

            // 消耗显示保持不变
            if (__instance.Cost > 0)
            {
                stringBuilder.Append("\n" + __instance.Cost.ToString() + " HP");
            }

            __result = stringBuilder.ToString();
            return false; // 跳过原始方法
        }


    }
}
