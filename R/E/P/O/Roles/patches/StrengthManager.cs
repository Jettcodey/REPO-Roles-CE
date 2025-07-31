using Photon.Pun;
using UnityEngine;

namespace R.E.P.O.Roles.patches;

public class StrengthManager : MonoBehaviour
{
	internal PhotonView photonView;

	private void Start()
	{
		photonView = ((Component)this).GetComponent<PhotonView>();
	}

	[PunRPC]
	internal void setStrengthRPC(string steamID, float newStrength)
	{
		PlayerAvatar val = SemiFunc.PlayerAvatarGetFromSteamID(steamID);
		if (val != null)
		{
			val.physGrabber.grabStrength = newStrength;
		}
	}
}