using Photon.Pun;
using Repo_Roles;
using UnityEngine;

namespace R.E.P.O.Roles.patches
{
	public class EngineerManager : MonoBehaviourPun
	{
		private float sacrificedEnergy = 0f;
		private float regenTimer = 0f;

		private void Update()
		{
			if (!ClassManager.isEngineer) return;

			if (!SemiFunc.RunIsLevel() || SemiFunc.RunIsShop() || PlayerAvatar.instance == null)
				return;

			if (PlayerAvatar.instance.playerHealth.health <= 0)
			{
				if (sacrificedEnergy > 0f)
				{
					PlayerController.instance.EnergyStart += sacrificedEnergy;
					sacrificedEnergy = 0f;
					regenTimer = 0f;
				}

				return;
			}

			if (sacrificedEnergy > 0f)
			{
				regenTimer += Time.deltaTime;
				if (regenTimer >= 40f)
				{
					PlayerController.instance.EnergyStart += 5f;
					PlayerController.instance.EnergyCurrent += 5f;

					sacrificedEnergy -= 5f;
					regenTimer = 0f;
				}
			}
			else
			{
				regenTimer = 0f;
			}

			if (Input.GetKeyDown(RepoRoles.chargeItemKey.Value))
			{
				if (PlayerController.instance.EnergyStart < 5f) return;

				if (PhysGrabber.instance != null && PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject != null)
				{
					ItemBattery battery = PhysGrabber.instance.grabbedPhysGrabObject.GetComponentInChildren<ItemBattery>();

					if (battery != null && battery.batteryLife < 100f)
					{
						PlayerController.instance.EnergyStart -= 5f;
						sacrificedEnergy += 5f; 
						PlayerController.instance.EnergyCurrent = Mathf.Max(0f, PlayerController.instance.EnergyCurrent - 5f);

						int totalBars = Mathf.Max(1, battery.batteryBars);
						float oneBarCharge = 100f / totalBars;

						battery.batteryLife = Mathf.Min(battery.batteryLife + oneBarCharge, 100f);
					}
				}
			}
		}
	}
}