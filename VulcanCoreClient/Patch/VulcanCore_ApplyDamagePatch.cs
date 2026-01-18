using BepInEx.Logging;
using Comfort.Common;
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
                //NotificationManagerClass.DisplayMessageNotification(
                //    $"你承受了{damageInfo.Damage}点{damageInfo.DamageType}伤害",
                //    ENotificationDurationType.Default,
                //    ENotificationIconType.Default,
                //    null
                //);
                if (!(AccessTools.Field(__instance.GetType(), "_healthController").GetValue(__instance) as IHealthController).IsAlive)
                {
                    return false;
                }

                EDamageType damageType = damageInfo.DamageType;
                __instance.LastDamagedBodyPart = bodyPartType;
                IPlayerOwner player = damageInfo.Player;
                Player player2 = ((player != null) ? __instance.GameWorld.GetAlivePlayerByProfileID(player.iPlayer.ProfileId) : null);
                float mutiper = 1f; //100%承伤
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
                        //哦, 还有进化护盾优先
                        //要做吗?
                        //要做吧?
                        //做吗?
                        //整.
                        //不对不对, 应该单独分离一个减伤倍率, 然后根据伤害类型判断, 复合的条件最前, 最终独立计算减伤乘区
                        //对, 对吗?
                        //做一下试试先
                        //主要这护符也不能丢弃....
                        //var 绝对领域 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "绝对领域");
                        //var 锁血 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "死斗锁血不死状态精简版"); //不提供减伤, 这里只是预留
                        //var 全局减伤 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "分子结构强化");
                        //所有减伤按优先级依次取最高
                        if (ProtectByAtomicReinforce(damageType))
                        {
                            var 全局减伤 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "分子结构强化");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 全局减伤?.Value ?? 0f) / 100f); //全局减伤, 优先级最高, 覆盖所有伤害
                        }
                        if (ProtectByATField(damageType))
                        {
                            var 绝对领域 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "绝对领域");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 绝对领域?.Value ?? 0f) / 100f); //刚性盾/绝对领域, 覆盖所有非自身状态(中毒/出血/脱力脱水etc)伤害
                        }
                        //其余
                        if (IsFallDamage(damageType))
                        {
                            var 羽落护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "羽落");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 羽落护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        if (IsFireDamage(damageType))
                        {
                            var 烈焰护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "火焰保护");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 烈焰护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        if (IsProjectileDamage(damageType))
                        {
                            var 弹射物护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "动能护盾");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 弹射物护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        if (IsSniperDamage(damageType))
                        {
                            var 狙击护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "无上神力");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 狙击护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        if (IsBleedingDamage(damageType))
                        {
                            var 鲜血护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "鲜血仪式");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 鲜血护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        if (IsLifeDamage(damageType))
                        {
                            var 生命护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "生命之诗");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 生命护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        if (IsExplosionDamage(damageType))
                        {
                            var 爆破护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "爆破专家");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 爆破护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        if (IsPosionDamage(damageType))
                        {
                            var 毒素护符 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "慈父祝福");
                            mutiper = Math.Min(mutiper, 1f - Math.Max(0f, 毒素护符?.Value ?? 0f) / 100f); //计算减伤并取最高减伤
                        }
                        //完成最终计算
                        float finalDamage = damageInfo.Damage * Math.Max(0f, mutiper); // 使用局部变量来存储最终伤害
                        //Console.WriteLine($"你承受了{finalDamage}点{damageInfo.DamageType}伤害, 减免倍率: ${mutiper}");
                        value = (damageInfo.DidBodyDamage = __instance.ActiveHealthController.ApplyDamage(bodyPartType, finalDamage, damageInfo));
                        /*旧逻辑存档
                        var 爆炸防护 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "爆炸防护");
                        var 绝对领域 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "绝对领域");
                        var 火焰保护 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "火焰保护");
                        if (火焰保护 != null && IsFireDamage(damageType))
                        {
                            float finalDamage = damageInfo.Damage * (Math.Max(1f - 火焰保护.Value.Value / 100f, 0)); // 使用局部变量来存储最终伤害
                            value = (damageInfo.DidBodyDamage = __instance.ActiveHealthController.ApplyDamage(bodyPartType, finalDamage, damageInfo));
                            Console.WriteLine($"伤害防护成功, 减免倍率为{Math.Min(100, 火焰保护.Value.Value)}%, 最终实际承伤为{finalDamage}点, 伤害类型为{damageInfo.DamageType}");
                        }
                        else
                        {
                            value = (damageInfo.DidBodyDamage = __instance.ActiveHealthController.ApplyDamage(bodyPartType, damageInfo.Damage, damageInfo));
                        }
                        */
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
                var nodamage = __instance.IsYourPlayer && mutiper <= 0f;
                if (!nodamage)
                {
                    __instance.ApplyHitDebuff(damageInfo.Damage, damageInfo.StaminaBurnRate * damageInfo.Damage, bodyPartType, damageType);
                }
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
                var 锁血 = GetHighestAmuletWithEffect(__instance.Inventory.AllRealPlayerItems, "死斗锁血不死状态精简版"); //不提供减伤, 这里只是预留
                if (player != null && !__instance.HealthController.IsAlive && Singleton<BotEventHandler>.Instantiated && 锁血 == null)
                {
                    Singleton<BotEventHandler>.Instance.Kill(player.iPlayer, __instance.GetPlayer);
                }
                return false;
            }
        }
        public static bool IsFallDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);
            return type == ECustomDamageType.坠落;
        }
        public static bool IsSniperDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);
            return type == ECustomDamageType.狙击手;
        }
        public static bool IsExplosionDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)(int)damageType;
            ECustomDamageType category = ECustomDamageType.爆炸 | ECustomDamageType.爆炸物 | ECustomDamageType.温压爆炸 | ECustomDamageType.地雷 | ECustomDamageType.迫击炮轰炸;

            return (type & category) != 0;
        }
        public static bool IsFireDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)(int)damageType;
            ECustomDamageType category = ECustomDamageType.火焰 | ECustomDamageType.高温气体 | ECustomDamageType.温压爆炸;

            return (type & category) != 0;
        }
        public static bool IsPosionDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);
            ECustomDamageType category = ECustomDamageType.中毒 | ECustomDamageType.致命毒素;

            return (type & category) != 0;
        }
        public static bool IsLifeDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);
            ECustomDamageType category = ECustomDamageType.小出血 | ECustomDamageType.大出血 | ECustomDamageType.脱水 | ECustomDamageType.力竭 | ECustomDamageType.激素副作用 | ECustomDamageType.药物副作用 | ECustomDamageType.致命毒素 | ECustomDamageType.中毒;

            return (type & category) != 0;
        }
        public static bool IsBleedingDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);
            ECustomDamageType category = ECustomDamageType.小出血 | ECustomDamageType.大出血;

            return (type & category) != 0;
        }
        public static bool IsProjectileDamage(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);
            ECustomDamageType category = ECustomDamageType.子弹 | ECustomDamageType.钝伤 | ECustomDamageType.狙击手;

            return (type & category) != 0;
        }
        public static bool ProtectByATField(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);
            ECustomDamageType category = (ECustomDamageType)(~(ECustomDamageType.激素副作用 | ECustomDamageType.药物副作用 | ECustomDamageType.脱水 | ECustomDamageType.力竭 | ECustomDamageType.小出血 | ECustomDamageType.大出血 | ECustomDamageType.致命毒素 | ECustomDamageType.中毒));

            return (type & category) != 0;
        }
        public static bool ProtectByAtomicReinforce(EDamageType damageType)
        {
            var type = (ECustomDamageType)((int)damageType);

            return type != 0;  // 直接判断
        }
        public static KeyValuePair<string, float>? GetHighestAmuletWithEffect(IEnumerable<Item> inventory, string effectKeyword)
        {
            // 提取所有符合条件的护符
            var amulets = inventory
                .Select(x =>
                {
                    var desc = LocaleManagerClass.LocaleManagerClass.method_4(x.Template._id + " Description");
                    if (desc != null && desc.Contains($"特殊效果: {effectKeyword}"))//if (desc != null && desc.Contains("特殊效果: 护符") && desc.Contains($"护符效果: {effectKeyword}"))
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
    [HarmonyPatch(typeof(ActiveHealthController), "Kill")]
    public static class VulcanCore_AHCKillPatch
    {
        public static bool Prefix(ActiveHealthController __instance, EDamageType damageType)
        {
            if (!__instance.Player.IsYourPlayer)
            {
                return true;
            }
            else
            {
                //Console.WriteLine("你死了!");
                var 锁血 = VulcanCore_PlayerApplyDamageInfoPatch.GetHighestAmuletWithEffect(__instance.Player.Inventory.AllRealPlayerItems, "死斗锁血不死状态精简版");
                if (锁血 == null)
                {
                    //Console.WriteLine("无法找到锁血状态");
                }
                if (__instance.IsAlive && 锁血 == null)
                {
                    __instance.IsAlive = false;
                    __instance.method_35(damageType);
                    if (AccessTools.Field(__instance.GetType(), "DiedEvent") is FieldInfo field)
                    {
                        // 获取事件背后的委托
                        Action<EDamageType> eventDelegate = (Action<EDamageType>)field.GetValue(__instance);
                        if (eventDelegate != null)
                        {                                                                   // 触发事件
                            eventDelegate?.Invoke(damageType);
                        }
                    }
                    //DiedEvent?.Invoke(damageType);
                }
                return false;
            }
        }
    }
    [HarmonyPatch(typeof(ActiveHealthController), "DestroyBodyPart")]
    public static class VulcanCore_AHCDestroyBodyPartPatch
    {
        public static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, EDamageType damageType)
        {
            if (!__instance.Player.IsYourPlayer)
            {
                return true;
            }
            else
            {
                //Console.WriteLine("你死了!");
                var 锁血 = VulcanCore_PlayerApplyDamageInfoPatch.GetHighestAmuletWithEffect(__instance.Player.Inventory.AllRealPlayerItems, "死斗锁血不死状态精简版");
                if (锁血 == null)
                {
                    //Console.WriteLine("无法找到锁血状态");
                }
                if (__instance.IsAlive && 锁血 == null)
                {
                    GClass3009<AHCEffect>.BodyPartState bodyPartState = __instance.Dictionary_0[bodyPart];
                    if (!bodyPartState.IsDestroyed)
                    {
                        bodyPartState.IsDestroyed = true;
                        __instance.method_44(bodyPart, damageType);
                        //BodyPartDestroyedEvent?.Invoke(bodyPart, damageType);
                        if (AccessTools.Field(__instance.GetType(), "BodyPartDestroyedEvent") is FieldInfo field)
                        {
                            // 获取事件背后的委托
                            Action<EBodyPart, EDamageType> eventDelegate = (Action<EBodyPart, EDamageType>)field.GetValue(__instance);
                            if (eventDelegate != null)
                            {                                                                   // 触发事件
                                eventDelegate?.Invoke(bodyPart, damageType);
                            }
                        }
                    }

                    __instance.method_24(bodyPart, damageType);
                    //DiedEvent?.Invoke(damageType);
                }
                return false;
            }
        }
    }
}
    
