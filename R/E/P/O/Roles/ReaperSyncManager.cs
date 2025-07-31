using Photon.Pun;
using Repo_Roles;
using UnityEngine;

namespace R.E.P.O.Roles;

public class ReaperSyncManager : MonoBehaviourPunCallbacks
{
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		var localAvatar = SemiFunc.PlayerAvatarGetFromSteamID(PlayerController.instance.playerSteamID);
		var rMan = ((Component)localAvatar).GetComponent<ReaperManager>();
		if (rMan != null && rMan.isReaper)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				// did not test this yet.
				RepoRoles.Logger.LogWarning($"Sending late-joiner Reaper status RPC to {newPlayer.NickName}");
				rMan.photonView.RPC("setReaperStatusRPC", newPlayer, PlayerController.instance.playerSteamID, true);
			}
		}
	}
}