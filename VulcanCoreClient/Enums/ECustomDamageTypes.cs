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
    [Flags]
    public enum ECustomDamageType
    {
        未知 = 1,
        坠落 = 2,
        爆炸 = 4,
        铁丝网 = 8,
        火焰 = 0x10,
        爆炸物 = 0x20, //疑似榴弹类型
        冲击 = 0x40, //Impact. 这啥
        生存 = 0x80, //Existence, 你又是啥玩意儿??
        药物副作用 = 0x100, //Medicine....这啥玩意儿?
        子弹 = 0x200,
        近战 = 0x400,
        地雷 = 0x800,
        狙击手 = 0x1000,
        钝伤 = 0x2000,
        小出血 = 0x4000,
        大出血 = 0x8000,
        脱水 = 0x10000,
        力竭 = 0x20000, //这玩意掉血吗??
        辐射暴露 = 0x40000,
        激素副作用 = 0x80000,
        中毒 = 0x100000,
        致命毒素 = 0x200000,
        BTR碾压 = 0x400000,
        迫击炮轰炸 = 0x800000,
        高温气体 = 0x1000000,
        温压爆炸 = 0x2000000,
        环境伤害 = 0x4000000
    }
}
