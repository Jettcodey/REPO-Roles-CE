using System;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using R.E.P.O.Roles.patches;
using Repo_Roles;
using UnityEngine;
using Random = System.Random;

namespace R.E.P.O.Roles
{
	public class ClassManager
	{
		private Random rnd = new Random();

		public int chosenRoleId;

		private StrengthManager strManager = new StrengthManager();

		public string chosenRole;

		public int roleAmount = 10;

		public RepoRoles repoRoles = new RepoRoles();

		public ReaperManager rMan;

		public readonly Harmony harmonyPatcher = new Harmony("patches.reporoles.mod");

		public static int stackKills;

		public static bool isTank;

		public static bool isScout;

		private string[] roleNames = new string[11] { "Random", "Runner", "Tank", "Gambler", "Strongman", "Ranged Looter", "Athletic", "Mage", "Reaper", "Scout", "Regular" };

		public ClassManager()
		{
			// The constructor is now empty - we do not save the original values
		}

		public int genGamblerEffectNr()
		{
			return rnd.Next(0, 5);
		}

		public object[] genGamblerEffects()
		{
			string[] array = new string[5] { "You walk faster", "You have more stamina", "You have more health", "You\u00b4re stronger", "You jump higher" };
			string[] array2 = new string[5] { "you walk slower", "you have less stamina", "you have less health", "you\u00b4re weaker", "you don\u00b4t jump as high" };
			int posEffectNr = genGamblerEffectNr();
			int negEffectNr = genGamblerEffectNr();
			while (posEffectNr == negEffectNr)
			{
				posEffectNr = genGamblerEffectNr();
				negEffectNr = genGamblerEffectNr();
			}
			string text = array[posEffectNr];
			string text2 = array2[negEffectNr];
			string text3 = RepoRoles.customRoleDecGambler.Value + " " + text + " but " + text2 + "!";
			return new object[3] { text3, posEffectNr, negEffectNr };
		}

		public void assignRoleFromConfig(PlayerController __instance)
		{
			string roleChoice = RepoRoles.savedRole.Value;
			if (roleChoice == "" || roleChoice == "Random")
			{
				chosenRoleId = rnd.Next(1, roleAmount + 1);
				RepoRoles.Logger.LogInfo((object)"Rolling random role...");
			}
			else
			{
				chosenRoleId = Array.IndexOf<string>(roleNames, roleChoice);
			}
			assignRole(chosenRoleId, __instance);
		}

		private void modifyStrength(string steamID, float newStrength)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				PlayerAvatar.instance.physGrabber.grabStrength = newStrength;
				return;
			}
			PunManager.instance.photonView.RPC("setStrengthRPC", (RpcTarget)0, new object[2] { steamID, newStrength });
		}

		private void setHealth(string steamID, int maxHealth, int health)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				PlayerAvatar.instance.playerHealth.health = health;
				PlayerAvatar.instance.playerHealth.maxHealth = maxHealth;
				return;
			}
			PunManager.instance.photonView.RPC("setHealthRPC", (RpcTarget)0, new object[3] { steamID, maxHealth, health });
		}

		public void setReaperStatus(string steamID, bool isReaper)
		{
			if (!SemiFunc.IsMultiplayer())
			{
				((Component)PlayerAvatar.instance).GetComponent<ReaperManager>().isReaper = isReaper;
			}
			else if ((UnityEngine.Object)(object)PlayerAvatar.instance.photonView != null)
			{
				// only send in run levels
				if (RunManager.instance == null || RunManager.instance.levelCurrent == null) return;
				if (!RunManager.instance.levels.Contains(RunManager.instance.levelCurrent)) return;

				if (PlayerAvatar.instance != null && PlayerAvatar.instance.photonView != null)
				{
					PlayerAvatar.instance.photonView.RPC("setReaperStatusRPC", RpcTarget.All, steamID, isReaper);
#if DEBUG
					RepoRoles.Logger.LogInfo((object)$"[CsMr] setReaperStatus: sent RPC for {steamID}, setTo={isReaper}");
#endif
				}
			}
		}

		public void assignRole(int roleId, PlayerController __instance)
		{
			RepoRoles.Update_ManaRegeneration();
			RepoRoles.Update_ScoutCooldown();

			float currentMoveSpeed = __instance.MoveSpeed;
			float currentSprintSpeed = __instance.SprintSpeed;
			float currentCrouchSpeed = __instance.CrouchSpeed;
			float currentJumpForce = __instance.JumpForce;
			float currentMaxEnergy = __instance.EnergyStart;
			float currentEnergy = __instance.EnergyCurrent;
			float currentGrabStrength = PlayerAvatar.instance.physGrabber.grabStrength;
			float currentGrabRange = PlayerAvatar.instance.physGrabber.grabRange;
			int currentMaxHealth = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
			int currentHealth = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);

			if ((UnityEngine.Object)(object)((Component)PlayerAvatar.instance).GetComponent<ReaperManager>() != null)
			{
				rMan = ((Component)PlayerAvatar.instance).GetComponent<ReaperManager>();
			}
			else
			{
				RepoRoles.Logger.LogError((object)"Failed to get Reaper Manager! Please contact the mod developer about this.");
			}

			string text = SemiFunc.PlayerGetName(PlayerAvatar.instance);
			string key = SemiFunc.PlayerGetSteamID(PlayerAvatar.instance);

			stackKills = 0;

			guiManager.isMage = false;
			isTank = false;
			setReaperStatus(PlayerController.instance.playerSteamID, isReaper: false);
			isScout = false;

			if (!SemiFunc.RunIsLevel() || SemiFunc.RunIsShop())
			{
				return;
			}

			if (!RepoRoles.enableRunner.Value && !RepoRoles.enableTank.Value && !RepoRoles.enableGambler.Value && !RepoRoles.enableStrongman.Value && !RepoRoles.enableRL.Value && !RepoRoles.enableAthletic.Value && !RepoRoles.enableMage.Value && !RepoRoles.enableReaper.Value && !RepoRoles.enableScout.Value && !RepoRoles.enableRegular.Value)
			{
				RepoRoles.Logger.LogError((object)"WARNING! You disabled all roles in the config file. You will not get any roles until you change it back.");
				return;
			}

			if (RepoRoles.showGUIAtStart.Value)
			{
				guiManager.showGUI = true;
			}
			else
			{
				guiManager.showGUI = false;
			}

			harmonyPatcher.PatchAll(typeof(PunManagerPatch));

			if (roleId <= 0)
			{
				RepoRoles.Logger.LogWarning((object)"Unable to find RoleId! Please contact the mod developer.");
				return;
			}

			switch (roleId)
			{
				case 1: // Runner
					{
						if (!RepoRoles.enableRunner.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogInfo((object)("Assigning role " + RepoRoles.customRoleNameRunner.Value + "."));

						__instance.CrouchSpeed = currentCrouchSpeed * 1.5f;
						__instance.MoveSpeed = currentMoveSpeed * 1.5f;
						__instance.SprintSpeed = currentSprintSpeed * 1.5f;
						__instance.EnergyStart = currentMaxEnergy * 1.5f;
						__instance.EnergyCurrent = currentEnergy * 1.5f;

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameRunner.Value;
						RepoRoles.GUIinstance.color = new Color(0.973f, 1f, 0.196f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecRunner.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.973f, 1f, 0.196f);
						break;
					}
				case 2: // Tank
					{
						if (!RepoRoles.enableTank.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogInfo((object)("Assigning role " + RepoRoles.customRoleNameTank.Value + "."));

						setHealth(PlayerController.instance.playerSteamID, currentMaxHealth * 2, currentHealth * 2);
						__instance.CrouchSpeed = currentCrouchSpeed * 0.9f;
						__instance.MoveSpeed = currentMoveSpeed * 0.9f;
						__instance.SprintSpeed = currentSprintSpeed * 0.9f;
						isTank = true;

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameTank.Value;
						RepoRoles.GUIinstance.color = Color.gray;
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecTank.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = Color.gray;
						break;
					}
				case 3: // Gambler
					{
						if (!RepoRoles.enableGambler.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogInfo((object)("Assigning role " + RepoRoles.customRoleNameGambler.Value + "."));

						object[] array = genGamblerEffects();

						if ((int)array[1] == 0)
						{
							__instance.CrouchSpeed = currentCrouchSpeed * 1.3f;
							__instance.MoveSpeed = currentMoveSpeed * 1.3f;
							__instance.SprintSpeed = currentSprintSpeed * 1.3f;
						}
						else if ((int)array[1] == 1)
						{
							__instance.EnergyStart = currentMaxEnergy * 1.8f;
							__instance.EnergyCurrent = currentEnergy * 1.8f;
						}
						else if ((int)array[1] == 2)
						{
							setHealth(PlayerController.instance.playerSteamID, (int)(currentMaxHealth * 1.8), (int)(currentHealth * 1.8));
						}
						else if ((int)array[1] == 3)
						{
							modifyStrength(PlayerController.instance.playerSteamID, currentGrabStrength * 1.3f);
						}
						else if ((int)array[1] == 4)
						{
							__instance.JumpForce = currentJumpForce * 1.5f;
						}

						if ((int)array[2] == 0)
						{
							__instance.CrouchSpeed = currentCrouchSpeed * 0.8f;
							__instance.MoveSpeed = currentMoveSpeed * 0.8f;
							__instance.SprintSpeed = currentSprintSpeed * 0.8f;
						}
						else if ((int)array[2] == 1)
						{
							__instance.EnergyStart = currentMaxEnergy * 0.8f;
							__instance.EnergyCurrent = currentEnergy * 0.8f;
						}
						else if ((int)array[2] == 2)
						{
							setHealth(PlayerController.instance.playerSteamID, (int)(currentMaxHealth * 0.8), (int)(currentHealth * 0.8));
						}
						else if ((int)array[2] == 3)
						{
							modifyStrength(PlayerController.instance.playerSteamID, currentGrabStrength * 0.8f);
						}
						else if ((int)array[2] == 4)
						{
							__instance.JumpForce = currentJumpForce * 0.7f;
						}

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameGambler.Value;
						RepoRoles.GUIinstance.color = new Color(0.576f, 0f, 0.831f);
						RepoRoles.GUIinstance.descColor = new Color(0.576f, 0f, 0.831f);
						RepoRoles.GUIinstance.descText = array[0]?.ToString() + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						break;
					}
				case 4: // Strongman
					{
						if (!RepoRoles.enableStrongman.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameStrongman.Value + "."));

						RepoRoles.Logger.LogInfo((object)("Strength before: " + currentGrabStrength));
						modifyStrength(PlayerController.instance.playerSteamID, currentGrabStrength * 1.5f + 0.5f);
						RepoRoles.Logger.LogInfo((object)("Strength after: " + PhysGrabber.instance.grabStrength));

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameStrongman.Value;
						RepoRoles.GUIinstance.color = new Color(0.761f, 0.055f, 0.055f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecStrongman.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.761f, 0.055f, 0.055f);
						break;
					}
				case 5: // Ranged Looter
					{
						if (!RepoRoles.enableRL.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameRL.Value + "."));

						PlayerAvatar.instance.physGrabber.grabRange = currentGrabRange * 2.5f;
						modifyStrength(PlayerController.instance.playerSteamID, currentGrabStrength * 1.2f);

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameRL.Value;
						RepoRoles.GUIinstance.color = new Color(0.592f, 0.969f, 0.663f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecRL.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.592f, 0.969f, 0.663f);
						break;
					}
				case 6: // Athletic
					{
						if (!RepoRoles.enableAthletic.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameAthletic.Value + "."));

						modifyStrength(PlayerController.instance.playerSteamID, currentGrabStrength * 1.4f);
						__instance.EnergyStart = currentMaxEnergy + 20f;
						__instance.EnergyCurrent = currentEnergy + 20f;
						__instance.JumpForce = currentJumpForce + 3f;

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameAthletic.Value;
						RepoRoles.GUIinstance.color = Color.white;
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecAthletic.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = Color.white;
						break;
					}
				case 7: // Mage
					{
						if (!RepoRoles.enableMage.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}

						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameMage.Value + "."));

						// Set mage flag and initial mana
						guiManager.isMage = true;
						guiManager.aviableMana = 8;
						guiManager.manaTicks = 0;

						// Create UI if it doesn't exist (hidden by default)
						ManaHelper.CreateUI();

						// Update display based on current config
						if (RepoRoles.GUIinstance != null)
						{
							RepoRoles.GUIinstance.UpdateManaDisplay();
						}

						// Adjust health for mage
						if ((double)currentHealth * 0.5 > 0.0)
						{
							setHealth(PlayerController.instance.playerSteamID, (int)(currentMaxHealth * 0.5), (int)(currentHealth * 0.5));
						}
						else
						{
							setHealth(PlayerController.instance.playerSteamID, (int)(currentMaxHealth * 0.5), currentHealth);
						}

						// Set GUI text
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameMage.Value;
						RepoRoles.GUIinstance.color = new Color(0f, 0.384f, 1f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecMage.Value + "\nPress " +
							((object)RepoRoles.showSpellsKey.Value).ToString() + " to see all your spells and press " +
							((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0f, 0.384f, 1f);
						break;
					}
				case 8: // Reaper
					{
						if (!RepoRoles.enableReaper.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameReaper.Value + "."));

						harmonyPatcher.PatchAll(typeof(ReaperPatch));
						setReaperStatus(PlayerController.instance.playerSteamID, isReaper: true);

						setHealth(PlayerController.instance.playerSteamID, (int)(currentMaxHealth * 1.5), (int)(currentHealth * 1.5));

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameReaper.Value;
						RepoRoles.GUIinstance.color = new Color(0.141f, 0.6f, 0.502f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecReaper.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.141f, 0.6f, 0.502f);
						break;
					}
				case 9: // Scout
					{
						if (!RepoRoles.enableScout.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameScout.Value + "."));

						isScout = true;
						__instance.EnergyStart = currentMaxEnergy * 2f;
						__instance.EnergyCurrent = currentEnergy * 2f;
						__instance.sprintRechargeAmount *= 2f;

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameScout.Value;
						RepoRoles.GUIinstance.color = new Color(0.902f, 0.733f, 0.11f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecScout.Value.Replace("[G]", "[" + ((object)RepoRoles.scoutKey.Value).ToString() + "]") + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.902f, 0.733f, 0.11f);
						break;
					}
				case 10: // Regular
					{
						if (!RepoRoles.enableRegular.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameRegular.Value + "."));

						__instance.CrouchSpeed = currentCrouchSpeed;
						__instance.MoveSpeed = currentMoveSpeed;
						__instance.SprintSpeed = currentSprintSpeed;
						__instance.JumpForce = currentJumpForce;
						__instance.EnergyStart = currentMaxEnergy;
						__instance.EnergyCurrent = currentEnergy;

						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameRegular.Value;
						RepoRoles.GUIinstance.color = Color.white;
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecRegular.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = Color.white;
						break;
					}
			}
		}
	}
}