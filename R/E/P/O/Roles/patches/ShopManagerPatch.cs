using HarmonyLib;
using R.E.P.O.Roles.patches;
using Repo_Roles;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace R.E.P.O.Roles.patches
{
	[HarmonyPatch(typeof(ShopManager))]
	public class ShopManagerPatch
	{
		[HarmonyPatch("Awake")]
		[HarmonyPostfix]
		public static void reviveListener(ShopManager __instance)
		{
			if (__instance != null && __instance.gameObject.GetComponent<ReviveManager>() == null)
			{
				__instance.gameObject.AddComponent<ReviveManager>();
				//RepoRoles.Logger.LogInfo("Added revive listener");
			}
		}
	}
}
