using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace R.E.P.O.Roles.patches
{
	[HarmonyPatch(typeof(StatsManager))]
	internal static class StatsManagerPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("Start")]
		private static void StatsPrefix(StatsManager __instance)
		{
			__instance.dictionaryOfDictionaries.Add("playerUpgradeManaRegeneration", new Dictionary<string, int>());
			__instance.dictionaryOfDictionaries.Add("playerUpgradeScoutCooldownReduction", new Dictionary<string, int>());
		}
	}
}
