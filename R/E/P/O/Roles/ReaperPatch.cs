using HarmonyLib;
using Repo_Roles;
using Photon.Pun;
using UnityEngine;

namespace R.E.P.O.Roles;

[HarmonyPatch(typeof(EnemyHealth))]
internal static class ReaperPatch
{
	private static ReaperManager rMan;

	[HarmonyPatch("Death")]
	[HarmonyPrefix]
	public static void PrefixMethod()
	{
		if ((Object)(object)((Component)SemiFunc.PlayerAvatarGetFromSteamID(PlayerController.instance.playerSteamID)).GetComponent<ReaperManager>() != (Object)null)
		{
			rMan = ((Component)SemiFunc.PlayerAvatarGetFromSteamID(PlayerController.instance.playerSteamID)).GetComponent<ReaperManager>();
		}
		else
		{
			RepoRoles.Logger.LogError((object)"Failed to get Reaper Manager! Please contact the mod developer about this.");
		}
		if (rMan.isReaper && (Object)(object)rMan != (Object)null)
		{
			rMan.kills++;
		}
		else
		{
			RepoRoles.Logger.LogError((object)"Unable to find ReaperManager in PlayerAvatar.instance. Please report this to the mod author.");
		}
	}
}
