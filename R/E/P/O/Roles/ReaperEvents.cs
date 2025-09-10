using ExitGames.Client.Photon;
using Photon.Pun;
using Repo_Roles;
using Photon.Realtime;

namespace R.E.P.O.Roles
{
	internal static class ReaperEvents
	{
		// please dont conflict with other mods
		public const byte EV_APPLY_REAPER_BUFFS = 150;
		public const byte EV_REAPER_STATUS_CHANGE = 151;
		public const byte EV_REQUEST_REAPER_BUFFS = 152;

		public static void RaiseApplyBuffs(string killerSteamID, string[] reaperSteamIDs)
		{
			if (reaperSteamIDs == null || reaperSteamIDs.Length == 0) return;
			if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
			{
#if DEBUG
				RepoRoles.Logger.LogWarning($"[RpEs] RaiseApplyBuffs: Not in room - cannot raise event (killer='{killerSteamID}')");
#endif
				return;
			}

			object[] payload = new object[] { killerSteamID ?? string.Empty, reaperSteamIDs };
			var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
			var send = new SendOptions { Reliability = true };
			PhotonNetwork.RaiseEvent(EV_APPLY_REAPER_BUFFS, payload, options, send);
#if DEBUG
			RepoRoles.Logger.LogInfo($"[RpEs] Master: Raised EV_APPLY_REAPER_BUFFS for {reaperSteamIDs.Length} reapers (killer='{killerSteamID ?? "unknown"}')");
#endif
		}

		public static void RaiseReaperStatusChange(string steamID, bool isReaper)
		{
			if (string.IsNullOrEmpty(steamID)) return;
			if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
			{
				RepoRoles.Logger.LogWarning($"[RpEs] RaiseReaperStatusChange: Not in room - skipping for {steamID} -> {isReaper}");
				return;
			}

			object[] payload = new object[] { steamID, isReaper };
			var options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
			var send = new SendOptions { Reliability = true };
			PhotonNetwork.RaiseEvent(EV_REAPER_STATUS_CHANGE, payload, options, send);
#if DEBUG
			RepoRoles.Logger.LogInfo($"[RpEs] Raised EV_REAPER_STATUS_CHANGE for {steamID} -> {isReaper}");
#endif
		}

		public static void RaiseRequestApplyBuffs(string killerSteamID)
		{
			if (string.IsNullOrEmpty(killerSteamID)) killerSteamID = string.Empty;
			if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
			{
				RepoRoles.Logger.LogWarning($"[RpEs] RaiseRequestApplyBuffs: Not in room - skipping request for killer={killerSteamID}");
				return;
			}

			object[] payload = new object[] { killerSteamID };
			var options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
			var send = new SendOptions { Reliability = true };
			PhotonNetwork.RaiseEvent(EV_REQUEST_REAPER_BUFFS, payload, options, send);
#if DEBUG
			RepoRoles.Logger.LogInfo($"[RpEs] Raised EV_REQUEST_REAPER_BUFFS for killer={killerSteamID}");
#endif
		}
	}
}