using HarmonyLib;
using Photon.Pun;
using Repo_Roles;
using System.Reflection;
using UnityEngine;

namespace R.E.P.O.Roles.patches
{
	public class ReviveManager : MonoBehaviour
	{
		public void Update()
		{
			if (ClassManager.isReviver && Input.GetKeyDown(RepoRoles.reviverKey.Value))
			{
				//RepoRoles.Logger.LogInfo("Revive key pressed");

				if (PlayerControllerPatch.grabbedHead && PlayerControllerPatch.dedHead != null)
				{
					PlayerDeathHead grabHead = PlayerControllerPatch.dedHead;

					if (grabHead != null)
					{
						FieldInfo inExtractionField = AccessTools.Field(typeof(PlayerDeathHead), "inExtractionPoint");
						if (!(bool)inExtractionField.GetValue(grabHead))
						{
							inExtractionField.SetValue(grabHead, true);
							grabHead.Revive();
							RepoRoles.Logger.LogInfo($"Revived Player: {SemiFunc.PlayerGetName(grabHead.playerAvatar)}");
						}
					}
				}
			}
		}
	}
}
