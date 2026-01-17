using BepInEx.Logging;
using Diz.LanguageExtensions;
using EFT;
using EFT.Ballistics;
using EFT.Communications;
using EFT.HealthSystem;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.SinglePlayer;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Comfort.Common;
using UnityEngine;
using static UnityEngine.TouchScreenKeyboard;


namespace VulcanCore
{
    /*RecieveDamage存在问题, 现已弃用
    [HarmonyPatch(typeof(Player), "ReceiveDamage")]
    public class VulcanCore_RecieveDamagePatch
    {
        //尼基塔你真是死了妈了你妈了个b这个方法压根不对
        //Recieve压根没阻拦伤害
        //司马东西
        [HarmonyPrefix]
        public static bool Prefix(Player __instance, float damage, EBodyPart part, EDamageType type, float absorbed, MaterialType special)
        {
            if (!__instance.IsYourPlayer)
            {
                return true;
            }
            else
            {
                /*提取所有护符, 已有完整方法, 仅做留档
                var amuletsDictionary = __instance.InventoryController.Inventory.AllRealPlayerItems
                    .Select(x =>
                    {
                        var desc = LocaleManagerClass.LocaleManagerClass.method_4(x.Template._id + " Description");
                        if (desc != null && desc.Contains("特殊效果: 护符"))
                        {
                            return new KeyValuePair<string, string>(x.Template._id, desc); // 返回 Template._id 和描述
                        }
                        return default(KeyValuePair<string, string>); // 不符合条件返回默认值
                    })
                    .Where(kv => kv.Key != null) // 去掉默认值（空的 KeyValuePair）
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
                
                //反射获取原委托
                FieldInfo field = typeof(Player).GetField("OnDamageReceived", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    Console.WriteLine($"受到{type}伤害");
                    Console.WriteLine("事件提取成功");
                    /*提取爆炸护符, 已有完整方法, 仅做留档
                    var antiexp = amuletsDictionary
                        .Where(kvp => kvp.Value.Contains("护符效果: 爆炸防护"))
                        .Select(kvp =>
                        {
                            // 提取 "伤害减免: num%" 中的数值部分
                            var match = System.Text.RegularExpressions.Regex.Match(kvp.Value, @"伤害减免: (\d+)%");
                            if (match.Success)
                            {
                                var reductionValue = float.Parse(match.Groups[1].Value);  // 获取数值部分
                                return new { kvp.Key, ReductionValue = reductionValue };  // 创建匿名对象，包含 Key 和减免值
                            }
                            return null;
                        })
                        .Where(item => item != null)  // 过滤掉无效项
                        .OrderByDescending(item => item.ReductionValue)  // 根据减免值从高到低排序
                        .FirstOrDefault();  // 获取排序后的第一个元素
                    
                    var antiexp = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "爆炸防护");
                    if (antiexp == null)
                    {
                        Console.WriteLine("警告: 未找到护符");
                    }
                    else
                    {
                        Console.WriteLine("护符查找成功");
                    }
                    // 获取事件背后的委托
                    Player.GDelegate65 eventDelegate = (Player.GDelegate65)field.GetValue(__instance);
                    if (eventDelegate != null)
                    {
                        Console.WriteLine("事件委托提取成功");
                        if (!IsExplosionDamage(type))
                        {
                            Console.WriteLine($"警告: 伤害类型和预期不符, 输入的类型为{type}");
                        }
                        if (antiexp != null && IsExplosionDamage(type))
                        {
                            float finalDamage = damage * (Math.Max(1f - antiexp.Value.Value / 100f, 0)); // 使用局部变量来存储最终伤害
                            // 触发事件
                            eventDelegate.Invoke(finalDamage, part, type, absorbed, special);
                            Console.WriteLine($"爆炸防护成功, 最终实际受到伤害: {finalDamage}");
                        }
                        else
                        {
                            eventDelegate.Invoke(damage, part, type, absorbed, special);
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public static bool IsExplosionDamage(EDamageType damageType)
        {
            // 自定义一个伤害类型组合，例如 Explosion, ThermobaricExplosion, Landmine 和 GrenadeFragment
            EDamageType category = EDamageType.Explosion | EDamageType.ThermobaricExplosion | EDamageType.Landmine | EDamageType.GrenadeFragment | EDamageType.Artillery;

            // 判断传入的 damageType 是否包含上述任意一个伤害类型
            return (damageType & category) != 0;
        }
        public static KeyValuePair<string, float>? GetHighestAmuletWithEffect(IEnumerable<Item> inventory, string effectKeyword)
        {
            // 提取所有符合条件的护符
            var amulets = inventory
                .Select(x =>
                {
                    var desc = LocaleManagerClass.LocaleManagerClass.method_4(x.Template._id + " Description");
                    if (desc != null && desc.Contains("特殊效果: 护符") && desc.Contains($"护符效果: {effectKeyword}"))
                    {
                        return new KeyValuePair<string, string>(x.Template._id, desc); // 返回 Template._id 和描述
                    }
                    return default(KeyValuePair<string, string>); // 不符合条件返回默认值
                })
                .Where(kv => kv.Key != null) // 去掉默认值（空的 KeyValuePair）
                .ToList();

            // 根据 "伤害减免: num%" 提取数值并排序
            var highestAmulet = amulets
                .Select(kvp =>
                {
                    var match = System.Text.RegularExpressions.Regex.Match(kvp.Value, @"伤害减免: (\d+)%");
                    if (match.Success)
                    {
                        var reductionValue = float.Parse(match.Groups[1].Value);  // 获取数值部分
                        return new { kvp.Key, ReductionValue = reductionValue };  // 创建匿名对象，包含 Key 和减免值
                    }
                    return null;
                })
                .Where(item => item != null)  // 过滤掉无效项
                .OrderByDescending(item => item.ReductionValue)  // 根据减免值从高到低排序
                .FirstOrDefault();  // 获取排序后的第一个元素

            // 如果有符合条件的护符，返回对应的 Key 和减免值
            if (highestAmulet != null)
            {
                return new KeyValuePair<string, float>(highestAmulet.Key, highestAmulet.ReductionValue);
            }

            // 如果没有找到符合条件的护符，返回 null
            return null;
        }
    }
    */
    [HarmonyPatch(typeof(Player), "ApplyDamageInfo")]
    public class VulcanCore_PlayerApplyDamageInfoPatch
    {
        public static bool Prefix(Player __instance, DamageInfoStruct damageInfo, EBodyPart bodyPartType, EBodyPartColliderType colliderType, float absorbed)
        {
            if (!__instance.IsYourPlayer)
            {
                return true;
            }
            else
            {
                NotificationManagerClass.DisplayMessageNotification(
                    $"你承受了{damageInfo.Damage}点{damageInfo.DamageType}伤害",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Default,
                    null
                );
                if (!(AccessTools.Field(__instance.GetType(), "_healthController").GetValue(__instance) as IHealthController).IsAlive)
                {
                    return false;
                }

                EDamageType damageType = damageInfo.DamageType;
                __instance.LastDamagedBodyPart = bodyPartType;
                IPlayerOwner player = damageInfo.Player;
                Player player2 = ((player != null) ? __instance.GameWorld.GetAlivePlayerByProfileID(player.iPlayer.ProfileId) : null);
                if (__instance.ActiveHealthController != null)
                {
                    __instance.ActiveHealthController.DoWoundRelapse(damageInfo.Damage, bodyPartType);
                    //__instance.LastAggressor = player?.iPlayer;
                    //__instance.LastDamageInfo = damageInfo;
                    //__instance.LastBodyPart = bodyPartType;
                    //死了妈的保护声明
                    AccessTools.Field(__instance.GetType(), "LastAggressor").SetValue(__instance, player?.iPlayer);
                    AccessTools.Field(__instance.GetType(), "LastDamageInfo").SetValue(__instance, damageInfo);
                    AccessTools.Field(__instance.GetType(), "LastBodyPart").SetValue(__instance, bodyPartType);
                    damageInfo.BleedBlock = __instance.method_95(colliderType);
                    float value = 0f;
                    if (__instance.IsYourPlayer)
                    {
                        //护符和伤害类型逻辑得重新构建, 单类取最高, 多类优先级要重构
                        //服务端数据好做....该做这部分了
                        //鉴于SPT的护符机制缺失, 考虑给邪教徒护符加个毒素防护
                        //目前应该是全局优先, 全局内检测其他伤害分支然后取最高, 但是爆炸防护和火焰要不要改....似乎存在重叠
                        //所以尼基塔nmgb为什么要把氧气罐做成高温气体呢?
                        //真的搞不明白
                        //哦, push了先
                        var antiexp = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "爆炸防护");
                        if (antiexp != null && IsExplosionDamage(damageType))
                        {
                            float finalDamage = damageInfo.Damage * (Math.Max(1f - antiexp.Value.Value / 100f, 0)); // 使用局部变量来存储最终伤害
                            value = (damageInfo.DidBodyDamage = __instance.ActiveHealthController.ApplyDamage(bodyPartType, finalDamage, damageInfo));
                            //Console.WriteLine($"伤害防护成功, 减免倍率为{Math.Min(100, antiexp.Value.Value)}%, 最终实际承伤为{finalDamage}点, 伤害类型为{damageInfo.DamageType}");
                        }
                        else
                        {
                            value = (damageInfo.DidBodyDamage = __instance.ActiveHealthController.ApplyDamage(bodyPartType, damageInfo.Damage, damageInfo));
                        }
                    }
                    else
                    {
                        value = (damageInfo.DidBodyDamage = __instance.ActiveHealthController.ApplyDamage(bodyPartType, damageInfo.Damage, damageInfo));
                    }
                    __instance.ActiveHealthController.BluntContusion(bodyPartType, absorbed);
                    if (GClass855.Positive(value) && __instance.ActiveHealthController.TryApplySideEffects(damageInfo, bodyPartType, out var sideEffectComponent) && player2 != null)
                    {
                        player2.OnSideEffectApplied(sideEffectComponent);
                    }
                }
                else
                {
                    damageInfo.DidBodyDamage = 0f;
                }

                player2?.Loyalty.MarkAsAggressor(__instance);
                __instance.ManageAggressor(damageInfo, bodyPartType, colliderType);
                __instance.ApplyHitDebuff(damageInfo.Damage, damageInfo.StaminaBurnRate * damageInfo.Damage, bodyPartType, damageType);
                if (!GClass3051.IsWeaponInduced(damageType))
                {
                    __instance.ReceiveDamage(damageInfo.Damage, bodyPartType, damageType, 0f, MaterialType.None);
                }

                //__instance.BeingHitAction?.Invoke(damageInfo, bodyPartType, 0f);
                var beingHitAction = AccessTools.Field(__instance.GetType(), "BeingHitAction").GetValue(__instance) as Action<DamageInfoStruct, EBodyPart, float>;
                beingHitAction?.Invoke(damageInfo, bodyPartType, 0f);
                if (Singleton<BotEventHandler>.Instantiated)
                {
                    Singleton<BotEventHandler>.Instance.BeingHitAction(damageInfo, __instance);
                }

                if (player != null && !__instance.HealthController.IsAlive && Singleton<BotEventHandler>.Instantiated)
                {
                    Singleton<BotEventHandler>.Instance.Kill(player.iPlayer, __instance.GetPlayer);
                }
                return false;
            }
        }
        public static bool IsExplosionDamage(EDamageType damageType)
        {
            // 自定义一个伤害类型组合，例如 Explosion, ThermobaricExplosion, Landmine 和 GrenadeFragment
            EDamageType category = EDamageType.Explosion | EDamageType.ThermobaricExplosion | EDamageType.Landmine | EDamageType.GrenadeFragment | EDamageType.Artillery;

            // 判断传入的 damageType 是否包含上述任意一个伤害类型
            return (damageType & category) != 0;
        }
        public static KeyValuePair<string, float>? GetHighestAmuletWithEffect(IEnumerable<Item> inventory, string effectKeyword)
        {
            // 提取所有符合条件的护符
            var amulets = inventory
                .Select(x =>
                {
                    var desc = LocaleManagerClass.LocaleManagerClass.method_4(x.Template._id + " Description");
                    if (desc != null && desc.Contains("特殊效果: 护符") && desc.Contains($"护符效果: {effectKeyword}"))
                    {
                        return new KeyValuePair<string, string>(x.Template._id, desc); // 返回 Template._id 和描述
                    }
                    return default(KeyValuePair<string, string>); // 不符合条件返回默认值
                })
                .Where(kv => kv.Key != null) // 去掉默认值（空的 KeyValuePair）
                .ToList();

            // 根据 "伤害减免: num%" 提取数值并排序
            var highestAmulet = amulets
                .Select(kvp =>
                {
                    var match = System.Text.RegularExpressions.Regex.Match(kvp.Value, @"伤害减免: (\d+)%");
                    if (match.Success)
                    {
                        var reductionValue = float.Parse(match.Groups[1].Value);  // 获取数值部分
                        return new { kvp.Key, ReductionValue = reductionValue };  // 创建匿名对象，包含 Key 和减免值
                    }
                    return null;
                })
                .Where(item => item != null)  // 过滤掉无效项
                .OrderByDescending(item => item.ReductionValue)  // 根据减免值从高到低排序
                .FirstOrDefault();  // 获取排序后的第一个元素

            // 如果有符合条件的护符，返回对应的 Key 和减免值
            if (highestAmulet != null)
            {
                return new KeyValuePair<string, float>(highestAmulet.Key, highestAmulet.ReductionValue);
            }

            // 如果没有找到符合条件的护符，返回 null
            return null;
        }
    }
}
