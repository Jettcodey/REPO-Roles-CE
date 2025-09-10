using HarmonyLib;
using Repo_Roles;
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
			var killerAvatar = SemiFunc.PlayerAvatarGetFromSteamID(PlayerController.instance.playerSteamID);
			if (killerAvatar == null) return;

			var killerRM = killerAvatar.GetComponent<ReaperManager>();
			if (killerRM != null)
			{
				killerRM.kills++;
			}

			// Call OnEnemyKilled on the local player's ReaperManager (handles client->master request or master broadcast)
			if (killerRM != null)
			{
				killerRM.OnEnemyKilled(killerAvatar);
			}
			else
			{
				ReaperEvents.RaiseRequestApplyBuffs(killerAvatar.steamID);
#if DEBUG
				RepoRoles.Logger.LogWarning((object)$"[RpPch] PrefixMethod: killer had no ReaperManager, sent request for killer={killerAvatar.steamID}");
#endif
			}
		}
	}
}