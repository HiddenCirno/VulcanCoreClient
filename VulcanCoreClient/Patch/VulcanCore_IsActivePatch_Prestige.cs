using System;
using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;

namespace VulcanCore
{
    [HarmonyPatch]
    public class VulcanCore_IsActivePatch_Prestige
    {
        private static MethodBase TargetMethod()
        {
            Type typeFromHandle = typeof(PrestigeTransferItemsState);
            Type typeFromHandle2 = typeof(GClass3455.ISelectionContext);
            InterfaceMapping interfaceMap = typeFromHandle.GetInterfaceMap(typeFromHandle2);
            for (int i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
            {
                bool active = interfaceMap.InterfaceMethods[i].Name == "IsActive";
                if (active)
                {
                    return interfaceMap.TargetMethods[i];
                }
            }
            throw new Exception("Could not find " + typeFromHandle2.FullName + ".IsActive implementation on " + typeFromHandle.FullName);
        }

        private static bool Prefix(PrestigeTransferItemsState __instance, ItemContextAbstractClass context, ref string tooltip, ref bool __result)
        {
            if (!VulcanCoreClient.SkipContainerCheck.Value)
            {
                if (!__instance.StashConfig.Filters.Contains(context.Item.Template))
                {
                    tooltip = "UI/Trader/TransferLocked";
                    __result = false;
                    return false;
                }

                if (context.Item is CompoundItem { IsEmpty: false })
                {
                    tooltip = "UI/Prestige/NonEmptyContainerError";
                    __result = false;
                    return false;
                }
            }
            tooltip = null;
            __result = true;
            return false;
        }
    }
}
