using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Repo_Roles;
using System.Collections.Generic;
using UnityEngine;

namespace R.E.P.O.Roles
{
	internal class ReaperEventListener : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		private static bool _created;
		private static readonly HashSet<string> masterReapers = new HashSet<string>();

		public static void Ensure()
		{
			if (_created) return;
			_created = true;
			var go = new GameObject("RepoRoles_ReaperEventListener");
			Object.DontDestroyOnLoad(go);
			go.AddComponent<ReaperEventListener>();
			RepoRoles.Logger.LogInfo("ReaperEventListener created!");
		}

		public static string[] GetMasterReapers()
		{
			lock (masterReapers)
			{
				var arr = new string[masterReapers.Count];
				masterReapers.CopyTo(arr);
				return arr;
			}
		}

		private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
		private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

		public override void OnJoinedRoom()
		{
			lock (masterReapers) masterReapers.Clear();
#if DEBUG
			RepoRoles.Logger.LogInfo("[RpEsLr] OnJoinedRoom: cleared master registry");
#endif
		}

		public override void OnLeftRoom()
		{
			lock (masterReapers) masterReapers.Clear();
#if DEBUG
			RepoRoles.Logger.LogInfo("[RpEsLr] OnLeftRoom: cleared master registry");
#endif
		}

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			lock (masterReapers) masterReapers.Clear();
#if DEBUG
			RepoRoles.Logger.LogInfo("[RpEsLr] OnMasterClientSwitched: cleared master registry (will repopulate by status events)");
#endif
		}

		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code == ReaperEvents.EV_REAPER_STATUS_CHANGE)
			{
				var data = photonEvent.CustomData as object[];
				if (data == null || data.Length < 2) return;
				string steam = data[0] as string ?? string.Empty;
				bool isReaper = (bool)data[1];

				if (PhotonNetwork.IsMasterClient)
				{
					lock (masterReapers)
					{
						if (isReaper)
						{
							if (masterReapers.Add(steam))
								RepoRoles.Logger.LogInfo($"[RpEsLr] Master: Registered reaper {steam}");
						}
						else
						{
							if (masterReapers.Remove(steam))
								RepoRoles.Logger.LogInfo($"[RpEsLr] Master: Unregistered reaper {steam}");
						}
					}
				}
				return;
			}

			if (photonEvent.Code == ReaperEvents.EV_REQUEST_REAPER_BUFFS)
			{
				if (!PhotonNetwork.IsMasterClient) return;
				var data = photonEvent.CustomData as object[];
				string killer = data != null && data.Length > 0 ? data[0] as string ?? string.Empty : string.Empty;

				string[] reapers;
				lock (masterReapers)
				{
					reapers = new string[masterReapers.Count];
					masterReapers.CopyTo(reapers);
				}

				RepoRoles.Logger.LogInfo($"[RpEsLr] Master: received EV_REQUEST_REAPER_BUFFS for killer={killer} reapersCount={reapers.Length}");
				if (reapers.Length > 0)
				{
					ReaperEvents.RaiseApplyBuffs(killer, reapers);
				}
				else
				{
					RepoRoles.Logger.LogInfo("[RpEsLr] Master: no reapers registered - nothing to broadcast");
				}
				return;
			}

			if (photonEvent.Code == ReaperEvents.EV_APPLY_REAPER_BUFFS)
			{
				try
				{
					var data = photonEvent.CustomData as object[];
					string killer = data?[0] as string ?? string.Empty;
					string[] reapers = data?[1] as string[] ?? new string[0];
#if DEBUG
					RepoRoles.Logger.LogInfo($"[RpEsLr] giveReaperStatsRPC: received via event from killer={killer} reapersCount={reapers.Length}");
#endif
					foreach (var rSteam in reapers)
					{
						var avatar = SemiFunc.PlayerAvatarGetFromSteamID(rSteam);
						if (avatar == null)
						{
							RepoRoles.Logger.LogWarning($"[RpEsLr] giveReaperStatsRPC: cannot find avatar for steamID={rSteam} on this client");
							continue;
						}
						var rm = avatar.GetComponent<ReaperManager>();
						if (rm == null)
						{
							RepoRoles.Logger.LogWarning($"[RpEsLr] giveReaperStatsRPC: avatar {rSteam} has no ReaperManager on this client");
							continue;
						}

						int health = (int)HarmonyLib.AccessTools.Field(typeof(PlayerHealth), "health").GetValue(avatar.playerHealth);
						if (health <= 0)
						{
							RepoRoles.Logger.LogInfo($"[RpEsLr] giveReaperStatsRPC: avatar {rSteam} is dead (health={health}) - skipping");
							continue;
						}
						if (rm.enemyDeathTimer > 0)
						{
							RepoRoles.Logger.LogInfo($"[RpEsLr] giveReaperStatsRPC: avatar {rSteam} in cooldown (enemyDeathTimer={rm.enemyDeathTimer}) - skipping");
							continue;
						}

						int maxBefore = (int)HarmonyLib.AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(avatar.playerHealth);
						int healthBefore = health;
						int maxAfter = maxBefore + 5;
						HarmonyLib.AccessTools.Field(typeof(PlayerHealth), "maxHealth").SetValue(avatar.playerHealth, maxAfter);

						if (healthBefore + 30 > maxAfter)
							avatar.playerHealth.Heal(maxAfter - healthBefore);
						else
							avatar.playerHealth.Heal(30);

						rm.kills = 0;
						rm.enemyDeathTimer = 50;
#if DEBUG
						RepoRoles.Logger.LogInfo(
							$"[RpEsLr] giveReaperStatsRPC: Applied buff to {rSteam} (health {healthBefore} -> {(int)HarmonyLib.AccessTools.Field(typeof(PlayerHealth), "health").GetValue(avatar.playerHealth)}, max {maxBefore} -> {maxAfter})");
#endif
					}
				}
				catch (System.Exception ex)
				{
					RepoRoles.Logger.LogError($"[RpEsLr]: Event handler exception: {ex}");
				}
			}
		}
	}
}