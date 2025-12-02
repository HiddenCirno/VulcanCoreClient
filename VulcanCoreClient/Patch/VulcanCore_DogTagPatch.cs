using System;
using EFT;
using System.Reflection;
using EFT.InventoryLogic;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using SPT.SinglePlayer;
using System.Text.RegularExpressions;


namespace VulcanCore
{
    [HarmonyPatch(typeof(Player), "OnBeenKilledByAggressor")]
    public class VulcanCore_DogTagPatch
    {
        [HarmonyPostfix]
        public static void PostFix(Player __instance, Player aggressor, DamageInfoStruct damageInfo)
        {
            Profile victimProfile = __instance.Profile;
            bool isSavage;
            if (victimProfile == null)
            {
                isSavage = false;
            }
            else
            {
                InfoClass victimInfo = victimProfile.Info;
                EPlayerSide? victimSide = (victimInfo != null) ? new EPlayerSide?(victimInfo.Side) : null;
                EPlayerSide savageSide = EPlayerSide.Savage;
                isSavage = (victimSide.GetValueOrDefault() == savageSide);
            }
            if (!isSavage)
            {
                return;
            }
            //MethodInfo getDogTagItemFromPlayerWhoDied = typeof(SPT.SinglePlayer.Patches.Quests.DogtagPatch).GetMethod("GetDogTagItemFromPlayerWhoDied", BindingFlags.NonPublic | BindingFlags.Instance);
            //MethodInfo updateDogtagItemWithDeathDetails = typeof(SPT.SinglePlayer.Patches.Quests.DogtagPatch).GetMethod("UpdateDogtagItemWithDeathDetails", BindingFlags.NonPublic | BindingFlags.Instance);
            //getDogTagItemFromPlayerWhoDied = 

            Item dogTagItemFromPlayerWhoDied = GetDogTagItemFromPlayerWhoDied(__instance);
            if (dogTagItemFromPlayerWhoDied == null)
            {
                if (__instance.IsYourPlayer)
                {
                    return;
                }
                string str = "DogtagPatch error > DogTag slot item on: ";
                Profile profile2 = __instance.Profile;
                string str2;
                if (profile2 == null)
                {
                    str2 = null;
                }
                else
                {
                    InfoClass info2 = profile2.Info;
                    str2 = ((info2 != null) ? info2.Nickname : null);
                }
                Console.WriteLine(str + str2 + " is null somehow.");
                Debug.LogError(str + str2 + " is null somehow.");
                return;
            }
            else
            {
                DogtagComponent itemComponent = dogTagItemFromPlayerWhoDied.GetItemComponent<DogtagComponent>();
                if (itemComponent == null)
                {
                    Console.WriteLine("DogtagPatch error > DogTagComponent on dog tag slot is null. Something went horrifically wrong!");
                    Debug.LogError("DogtagPatch error > DogTagComponent on dog tag slot is null. Something went horrifically wrong!");
                    return;
                }
                UpdateDogtagItemWithDeathDetails(__instance, aggressor, damageInfo, itemComponent);
                //DogtagPatch.UpdateDogtagItemWithDeathDetails(__instance, aggressor, damageInfo, itemComponent);
                return;
            }
        }
        private static Item GetDogTagItemFromPlayerWhoDied(Player __instance)
        {
            InventoryEquipment equipment = __instance.Equipment;
            if (equipment == null)
            {
                Debug.LogError("DogtagPatch error > Player has no equipment");
                return null;
            }
            Slot slot = equipment.GetSlot(EquipmentSlot.Dogtag);
            if (slot == null)
            {
                Debug.LogError("DogtagPatch error > Player has no dogtag slot");
                return null;
            }
            if (slot == null)
            {
                return null;
            }
            return slot.ContainedItem;
        }

        // Token: 0x060000A0 RID: 160 RVA: 0x00004504 File Offset: 0x00002704
        private static void UpdateDogtagItemWithDeathDetails(Player __instance, Player aggressor, DamageInfoStruct damageInfo, DogtagComponent itemComponent)
        {
            InfoClass info = __instance.Profile.Info;
            itemComponent.AccountId = __instance.Profile.AccountId;
            itemComponent.ProfileId = __instance.Profile.Id;
            //itemComponent.Nickname = info.MainProfileNickname;
            //itemComponent.Side = info.Side;
            itemComponent.KillerName = aggressor.Profile.Info.Nickname;
            itemComponent.Time = DateTime.Now;
            itemComponent.Status = "Killed by";
            itemComponent.KillerAccountId = aggressor.Profile.AccountId;
            itemComponent.KillerProfileId = aggressor.Profile.Id;
            itemComponent.WeaponName = damageInfo.Weapon.ShortName;
            if (__instance.Profile.Info.Experience > 0)
            {
                //itemComponent.Level = info.Level;
            }
        }


    }
}
