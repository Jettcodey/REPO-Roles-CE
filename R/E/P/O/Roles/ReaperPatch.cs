using HarmonyLib;
using Repo_Roles;
using Photon.Pun;
using UnityEngine;

namespace R.E.P.O.Roles
{
	[HarmonyPatch(typeof(EnemyHealth))]
	internal static class ReaperPatch
	{
		[HarmonyPatch("Death")]
		[HarmonyPrefix]
		public static void PrefixMethod()
		{
			var avatar = SemiFunc.PlayerAvatarGetFromSteamID(PlayerController.instance.playerSteamID);
			if (avatar == null) return;

			var rMan = avatar.GetComponent<ReaperManager>();
			if (rMan == null || !rMan.isReaper) return;

			rMan.kills++;

			if (SemiFunc.IsMultiplayer())
			{
				rMan.SendRPC("giveReaperStatsRPC", RpcTarget.All, avatar.steamID);
			}
			else
			{
				rMan.ApplyReaperStats(avatar);
			}
		}
	}
}