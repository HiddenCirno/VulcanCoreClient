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
    [HarmonyPatch(typeof(BuffEffect), "GetStringValue")]
    public class VulcanCore_BuffDisplayPatch
    {
        [HarmonyPrefix]
        public static bool GetStringValue_Prefix(BuffEffect __instance, ref string __result)
        {
            string str = __instance.BuffColoredStringValue();
            bool flag = __instance.Value.IsZero();
            StringBuilder stringBuilder = new StringBuilder();

            // 处理 Chance
            if (__instance.Chance < 1f)
            {
                stringBuilder.Append(string.Format("{0} {1}%",
                    "UI/ItemAttribute/Chance".Localized(null),
                    Math.Round((double)(__instance.Chance * 100f))));
            }

            // 处理 Delay
            if (__instance.Delay > 1f)
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" / ");
                stringBuilder.Append(string.Format("{0} {1}{2}",
                    "Del.".Localized(null),
                    __instance.Delay,
                    "sec".Localized(null)));
            }

            // 修改后的 Duration 逻辑
            if (__instance.Duration > 0f)
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" / ");

                // 核心修改点：判断是否超过7200秒
                if (__instance.Duration < 7200f)
                {
                    stringBuilder.Append(string.Format("{0} {1}{2}",
                        "Dur.".Localized(null),
                        __instance.Duration,
                        "sec".Localized(null)));
                }
                else
                {
                    // 显示 Infinity（需确保本地化支持）
                    stringBuilder.Append(string.Format("{0}",
                        "Infinity".Localized(null)));
                }

                // 附加效果值（如果非零）
                if (!flag)
                {
                    stringBuilder.Append(" (" + str + ")");
                }
            }
            else if (!flag) // 处理 Duration <=0 但有效果值的情况
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" / ");
                stringBuilder.Append(" " + str);
            }

            __result = stringBuilder.ToString();
            return false; // 跳过原始方法
        }


    }
}
