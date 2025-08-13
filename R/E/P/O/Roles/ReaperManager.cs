using HarmonyLib;
using Photon.Pun;
using Repo_Roles;
using UnityEngine;

namespace R.E.P.O.Roles
{
	public class ReaperManager : MonoBehaviour
	{
		public bool isReaper;
		public int kills;
		public PhotonView photonView;

		private void Start()
		{
			photonView = GetComponent<PhotonView>();
		}

		private void OnDestroy()
		{
			if (photonView != null && photonView.IsMine && photonView.ViewID != 0)
			{
				PhotonNetwork.RemoveRPCs(photonView);
			}
			photonView = null;
		}

		[PunRPC]
		internal void setReaperStatusRPC(string steamID, bool isReaper)
		{
			var avatar = SemiFunc.PlayerAvatarGetFromSteamID(steamID);
			if (avatar != null)
			{
				avatar.GetComponent<ReaperManager>().isReaper = isReaper;
			}
		}

		[PunRPC]
		internal void giveReaperStatsRPC(string steamID)
		{
			var avatar = SemiFunc.PlayerAvatarGetFromSteamID(steamID);
			if (avatar != null)
			{
				ApplyReaperStats(avatar);
			}
		}

		internal void ApplyReaperStats(PlayerAvatar avatar)
		{
			var ph = avatar.playerHealth;
			int maxHealth = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(ph) + 5;
			AccessTools.Field(typeof(PlayerHealth), "maxHealth").SetValue(ph, maxHealth);

			int health = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(ph);
			if (health + 30 > maxHealth)
			{
				avatar.playerHealth.Heal(maxHealth - health);
			}
			else
			{
				avatar.playerHealth.Heal(30);
			}
		}
	}
}