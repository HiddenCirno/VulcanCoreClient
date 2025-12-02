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
    [HarmonyPatch(typeof(SkillEffect), "GetStringValue")]
    public class VulcanCore_AttitudeDisplayPatch
    {
        [HarmonyPrefix]
        public static bool GetStringValue_Prefix(SkillEffect __instance, string postfix, ref string __result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // 1. 处理 Delay
            if (__instance.Delay > 1f)
            {
                stringBuilder.Append(string.Format("{0} {1}{2}",
                    "Del.".Localized(null),
                    __instance.Delay,
                    "sec".Localized(null)));
            }

            // 2. 修改后的 Duration 逻辑（核心修改点）
            if (__instance.Duration > 0f)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" / ");
                }

                // 判断是否超过7200秒
                if (__instance.Duration >= 7200f)
                {
                    stringBuilder.Append("Infinity".Localized(null)); // 直接显示 Infinity，不附加 "Dur." 和 "sec"
                }
                else
                {
                    stringBuilder.Append(string.Format("{0} {1}{2}",
                        "Dur.".Localized(null),
                        __instance.Duration,
                        "sec".Localized(null)));
                }
            }

            // 3. 处理 Cost
            if (__instance.Cost > 0)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" / ");
                }
                stringBuilder.Append(__instance.Cost.ToString() + " HP");
            }

            // 4. 附加 postfix（如果存在）
            if (!string.IsNullOrEmpty(postfix))
            {
                stringBuilder.Append(postfix);
            }

            __result = stringBuilder.ToString();
            return false; // 跳过原始方法
        }


    }
}
