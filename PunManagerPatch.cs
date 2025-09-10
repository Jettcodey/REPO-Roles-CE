using HarmonyLib;
using R.E.P.O.Roles;
using Photon.Pun;
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
			if (((Object)__instance).GetComponent<StrengthManager>() == null)
				((Object)__instance).AddComponent<StrengthManager>();

			if (((Object)__instance).GetComponent<HealthManager>() == null)
				((Object)__instance).AddComponent<HealthManager>();

			if (((Object)__instance).GetComponent<ScoutMarker>() == null)
				((Object)__instance).AddComponent<ScoutMarker>();
		}
	}
}