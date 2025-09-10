using HarmonyLib;
using Repo_Roles;
using Unity.VisualScripting;
using UnityEngine;

namespace R.E.P.O.Roles
{
	[HarmonyPatch(typeof(PlayerAvatar))]
	internal static class PlayerAvatarPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch("Awake")]
		private static void Awake_Postfix(PlayerAvatar __instance)
		{
			// Ensure each avatar has a ReaperManager
			if (!__instance.gameObject.GetComponent<ReaperManager>())
			{
				__instance.gameObject.AddComponent<ReaperManager>();
				RepoRoles.Logger.LogInfo((object)$"[PrArPch]: Added ReaperManager to avatar steamID={__instance.steamID} pv={(__instance.photonView != null ? __instance.photonView.ViewID.ToString() : "null")}");
			}
			else
			{
				RepoRoles.Logger.LogInfo((object)$"[PrArPch]: avatar already has ReaperManager steamID={__instance.steamID} pv={(__instance.photonView != null ? __instance.photonView.ViewID.ToString() : "null")}");
			}

			// Make sure event listener exists on every client
			ReaperEventListener.Ensure();
		}
	}
}