using System.Collections.Generic;
using HarmonyLib;

namespace R.E.P.O.Roles.patches;

[HarmonyPatch(typeof(StatsManager), "Start")]
internal static class StatsManagerPatch
{
	private static void Prefix(StatsManager __instance)
	{
		__instance.dictionaryOfDictionaries.Add("playerUpgradeManaRegeneration", new Dictionary<string, int>());
		__instance.dictionaryOfDictionaries.Add("playerUpgradeScoutCooldownReduction", new Dictionary<string, int>());
	}
}
