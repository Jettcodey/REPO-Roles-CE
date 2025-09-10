using HarmonyLib;
using Photon.Pun;
using Repo_Roles;
using System.Collections.Generic;
using UnityEngine;

namespace R.E.P.O.Roles
{
	[HarmonyPatch(typeof(EnemyHealth))]
	internal static class EnemyHealthPatch
	{
		private static readonly Dictionary<int, float> processed = new Dictionary<int, float>();
		private const float processedExpiry = 3f;

		// Multiplayer lethal damage detection
		[HarmonyPrefix]
		[HarmonyPatch("Hurt")]
		public static void Hurt_Prefix(EnemyHealth __instance, int _damage, Vector3 _hurtDirection)
		{
			if (!SemiFunc.IsMultiplayer()) return;

			// cleanup expired entries
			if (processed.Count > 0)
			{
				float now = Time.time;
				var keys = new List<int>(processed.Keys);
				foreach (var k in keys)
				{
					if (now - processed[k] > processedExpiry)
						processed.Remove(k);
				}
			}

			if (__instance == null) return;
			if (__instance.dead) return;

			int current = __instance.healthCurrent;
			int result = current - _damage;
			if (result > 0) return; // not lethal

			var pv = __instance.GetComponent<PhotonView>();
			int pvId = pv != null ? pv.ViewID : 0;
			if (pvId != 0 && processed.ContainsKey(pvId)) return;
			if (pvId != 0) processed[pvId] = Time.time;

			string killerSteam = PlayerController.instance != null ? PlayerController.instance.playerSteamID : string.Empty;

			if (PhotonNetwork.IsMasterClient)
			{
				var reapers = ReaperEventListener.GetMasterReapers();
#if DEBUG
				RepoRoles.Logger.LogInfo((object)$"[EnHlPch] Master detected lethal hit by {killerSteam} pv={pvId}");
#endif
				if (reapers != null && reapers.Length > 0)
					ReaperEvents.RaiseApplyBuffs(killerSteam, reapers);
			}
			else
			{
#if DEBUG
				RepoRoles.Logger.LogInfo((object)$"[EnHlPch] Client detected lethal hit by {killerSteam} pv={pvId}, requesting master");
#endif
				ReaperEvents.RaiseRequestApplyBuffs(killerSteam);
			}
		}

		// Singleplayer enemy death detection
		[HarmonyPostfix]
		[HarmonyPatch("DeathImpulseRPC")]
		public static void DeathImpulseRPC_Postfix(EnemyHealth __instance)
		{
			if (SemiFunc.IsMultiplayer()) return;

			if (__instance == null || !__instance.dead) return;

			string killerSteam = PlayerController.instance != null ? PlayerController.instance.playerSteamID : string.Empty;
			RepoRoles.Logger.LogInfo((object)$"[EnHlPch] Singleplayer enemy death detected by {killerSteam}");

			foreach (var avatar in SemiFunc.PlayerGetAll())
			{
				if (avatar == null) continue;
				var rm = avatar.GetComponent<ReaperManager>();
				if (rm != null && rm.isReaper)
				{
					rm.ApplyReaperStats(avatar);
					rm.kills = 0;
					rm.enemyDeathTimer = 50;
					RepoRoles.Logger.LogInfo((object)$"[EnHlPch] Applied singleplayer buff to {avatar.steamID}");
				}
			}
		}
	}
}