using System;
using EFT.Interactive;
using EFT.InventoryLogic;
using UnityEngine;
using HarmonyLib;

namespace VulcanCore
{
	[HarmonyPatch(typeof(Door), "BreachSuccessRoll")]
	public class Door_BreachSuccessRoll_Patch
	{
		[HarmonyPrefix]
		public static bool Prefix(Door __instance, Vector3 yourPosition, ref bool __result)
		{
			//__result = true;
			return true;
		}


	}
}
