using HarmonyLib;
using R.E.P.O.Roles.patches;
using System;
using UnityEngine;
using Repo_Roles;
using System.Collections.Generic;
using System.Text;

namespace R.E.P.O.Roles.patches
{
	[HarmonyPatch(typeof(PhysGrabCart))]
	public class PhysGrabCartPatch
	{
		[HarmonyPatch("Update")]
		[HarmonyPostfix]
		public static void displayText()
		{
			if (!SemiFunc.RunIsShop() && PlayerControllerPatch.grabbedHead)
			{
				if (PlayerControllerPatch.dedHead == null || PlayerControllerPatch.dedHead.playerAvatar == null)
					return;

				object targetName = AccessTools.Field(typeof(PlayerAvatar), "playerName")
					.GetValue(PlayerControllerPatch.dedHead.playerAvatar);

				ItemInfoExtraUI.instance.ItemInfoText($"Press {RepoRoles.reviverKey.Value} to revive {targetName}", new Color(0.95f, 0.75f, 0.05f));
			}
		}
	}
}
