using HarmonyLib;
using Photon.Pun;
using Repo_Roles;
using UnityEngine;

namespace R.E.P.O.Roles;

public class ReaperManager : MonoBehaviour
{
	public bool isReaper;

	public int kills;

	private int enemyDeathTimer = 0;

	public PhotonView photonView;

	private void Start()
	{
		photonView = GetComponent<PhotonView>();
		// for testing purposes, we log if the PhotonView has ReaperManager RPCs registered
		//RepoRoles.Logger.LogWarning($"PhotonView {photonView.ViewID} on {gameObject.name} has ReaperManager RPCs registered.");
	}

	[PunRPC]
	public void setReaperStatusRPC(string steamID, bool isReaper)
	{
		PlayerAvatar val = SemiFunc.PlayerAvatarGetFromSteamID(steamID);
		if (val != null)
		{
			((Component)val).GetComponent<ReaperManager>().isReaper = isReaper;
		}
	}

	private void Update()
	{
		foreach (PlayerAvatar item in SemiFunc.PlayerGetAll())
		{
			if (!((Object)(object)((Component)item).GetComponent<ReaperManager>() != null) || !((Component)item).GetComponent<ReaperManager>().isReaper || (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(item.playerHealth) <= 0)
			{
				continue;
			}
			if (!SemiFunc.IsMultiplayer())
			{
				if (((Component)SemiFunc.PlayerAvatarGetFromSteamID(PlayerController.instance.playerSteamID)).GetComponent<ReaperManager>().kills > 0 && enemyDeathTimer <= 0)
				{
					AccessTools.Field(typeof(PlayerHealth), "maxHealth").SetValue(item.playerHealth, (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(item.playerHealth) + 5);
					if ((int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(item.playerHealth) + 30 > (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(item.playerHealth))
					{
						item.playerHealth.Heal((int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(item.playerHealth) - (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(item.playerHealth), true);
					}
					else
					{
						item.playerHealth.Heal(30, true);
					}
					((Component)item).GetComponent<ReaperManager>().kills = 0;
					enemyDeathTimer = 50;
				}
			}
			else if ((Object)(object)item.photonView != null)
			{
				item.photonView.RPC("giveReaperStatsRPC", (RpcTarget)0, new object[1] { item.steamID });
			}
		}
		if (enemyDeathTimer > 0)
		{
			enemyDeathTimer--;
		}
	}

	// keeps spamming the in the Console/Logs :(
	[PunRPC]
	internal void giveReaperStatsRPC(string steamID)
	{
		PlayerAvatar val = SemiFunc.PlayerAvatarGetFromSteamID(steamID);
		if (val != null && ((Component)SemiFunc.PlayerAvatarGetFromSteamID(PlayerController.instance.playerSteamID)).GetComponent<ReaperManager>().kills > 0 && enemyDeathTimer <= 0)
		{
			AccessTools.Field(typeof(PlayerHealth), "maxHealth").SetValue(val.playerHealth, (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(val.playerHealth) + 5);
			if ((int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(val.playerHealth) + 30 > (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(val.playerHealth))
			{
				val.playerHealth.Heal((int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(val.playerHealth) - (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(val.playerHealth), true);
			}
			else
			{
				val.playerHealth.Heal(30, true);
			}
		((Component)val).GetComponent<ReaperManager>().kills = 0;
			enemyDeathTimer = 50;
		}
	}
}
