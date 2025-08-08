using Photon.Pun;
using UnityEngine;

namespace R.E.P.O.Roles.patches
{
	public class HealthManager : MonoBehaviour
	{
		internal PhotonView photonView;

		private void Start()
		{
			photonView = ((Component)this).GetComponent<PhotonView>();
		}

		[PunRPC]
		internal void setHealthRPC(string steamID, int maxHealth, int health)
		{
			PlayerAvatar val = SemiFunc.PlayerAvatarGetFromSteamID(steamID);
			if (val != null)
			{
				val.playerHealth.maxHealth = maxHealth;
				val.playerHealth.health = health;
			}
		}
	}
}
