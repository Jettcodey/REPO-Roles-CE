using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace R.E.P.O.Roles;

[HarmonyPatch(typeof(PlayerAvatar))]
internal static class PlayerAvatarPatch
{
	[HarmonyPostfix]
	[HarmonyPatch("Awake")]
	private static void Awake_Postfix(PlayerAvatar __instance)
	{
		if (!__instance.gameObject.GetComponent<ReaperManager>())
		{
			__instance.gameObject.AddComponent<ReaperManager>();
		}
	}
}
