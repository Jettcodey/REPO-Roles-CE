using HarmonyLib;
using R.E.P.O.Roles;
using R.E.P.O.Roles.patches;
using Unity.VisualScripting;
using UnityEngine;

namespace Repo_Roles
{
	[HarmonyPatch(typeof(PunManager))]
	internal static class PunManagerPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch("Start")]
		private static void Start_Postfix(PunManager __instance)
		{
			if ((Object)(object)__instance.GetComponent<StrengthManager>() == null)
			{
				((Object)(object)__instance).AddComponent<StrengthManager>();
			}
			if ((Object)(object)__instance.GetComponent<HealthManager>() == null)
			{
				((Object)(object)__instance).AddComponent<HealthManager>();
			}
			if ((Object)(object)__instance.GetComponent<ScoutMarker>() == null)
			{
				((Object)(object)__instance).AddComponent<ScoutMarker>();
			}
		}
	}
}
