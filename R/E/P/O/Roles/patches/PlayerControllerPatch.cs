using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace R.E.P.O.Roles.patches
{
	[HarmonyPatch(typeof(PlayerController))]
	public class PlayerControllerPatch
	{
		public static PlayerDeathHead dedHead;
		public static bool grabbedHead;

		[HarmonyPatch("FixedUpdate")]
		[HarmonyPostfix]
		public static void PlayerGrabHeadPatch(PlayerController __instance)
		{
			if (!((Object)(object)__instance != null))
			{
				return;
			}
			if (!__instance.physGrabActive || !((Object)(object)__instance.physGrabObject != null))
			{
				grabbedHead = false;
				return;
			}
			PlayerDeathHead component = __instance.physGrabObject.GetComponent<PlayerDeathHead>();
			if ((Object)(object)component != null && (Object)(object)component.playerAvatar != null)
			{
				dedHead = component;
				grabbedHead = true;
			}
		}
	}
}
