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

		private float origMoveSpeed;

		private float origSprintSpeed;

		private float origCrouchSpeed;

		private float speedMultiplier;

		private int origPlayerHealth;

		private int origMaxPlayerHealth;

		private float origJumpForce;

		private float origMaxEnergy;

		private float origGrabStrength;

		private float origGrabRange;

		public static int stackKills;

		public static bool isTank;

		public static bool isScout;

		private string[] roleNames = new string[11] { "Random", "Runner", "Tank", "Gambler", "Strongman", "Ranged Looter", "Athletic", "Mage", "Reaper", "Scout", "Regular" };

		public ClassManager()
		{
			origMoveSpeed = PlayerController.instance.MoveSpeed;
			origSprintSpeed = PlayerController.instance.SprintSpeed;
			origCrouchSpeed = PlayerController.instance.CrouchSpeed;
			speedMultiplier = 1.5f;
			origJumpForce = PlayerController.instance.JumpForce;
			origPlayerHealth = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
			origMaxPlayerHealth = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
			origMaxEnergy = PlayerController.instance.EnergyStart;
			origGrabStrength = PhysGrabber.instance.grabStrength;
			origGrabRange = PhysGrabber.instance.grabRange;
		}

		public int genGamblerEffectNr()
		{
			return rnd.Next(0, 5);
		}

		public object[] genGamblerEffects()
		{
			string[] array = new string[5] { "You walk faster", "You have more stamina", "You have more health", "You\u00b4re stronger", "You jump higher" };
			string[] array2 = new string[5] { "you walk slower", "you have less stamina", "you have less health", "you\u00b4re weaker", "you don\u00b4t jump as high" };
			int num = genGamblerEffectNr();
			int num2 = genGamblerEffectNr();
			while (num == num2)
			{
				num = genGamblerEffectNr();
				num2 = genGamblerEffectNr();
			}
			string text = array[num];
			string text2 = array2[num2];
			string text3 = RepoRoles.customRoleDecGambler.Value + " " + text + " but " + text2 + "!";
			return new object[3] { text3, num, num2 };
		}

		public void assignRoleFromConfig(PlayerController __instance)
		{
			string value = RepoRoles.savedRole.Value;
			if (value == "" || value == "Random")
			{
				chosenRoleId = rnd.Next(1, roleAmount + 1);
				RepoRoles.Logger.LogInfo((object)"Rolling random role...");
			}
			else
			{
				chosenRoleId = Array.IndexOf<string>(roleNames, value);
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
				PlayerAvatar.instance.photonView.RPC("setReaperStatusRPC", (RpcTarget)0, new object[2] { steamID, isReaper });
			}
		}

		public void assignRole(int roleId, PlayerController __instance)
		{
			RepoRoles.Update_ManaRegeneration();
			RepoRoles.Update_ScoutCooldown();
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
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			if (StatsManager.instance.playerUpgradeHealth.TryGetValue(key, out var value))
			{
				num = value;
			}
			if (StatsManager.instance.playerUpgradeSpeed.TryGetValue(key, out var value2))
			{
				num2 = value2;
			}
			if (StatsManager.instance.playerUpgradeStamina.TryGetValue(key, out var value3))
			{
				num3 = value3;
			}
			if (StatsManager.instance.playerUpgradeStrength.TryGetValue(key, out var value4))
			{
				num4 = value4;
			}
			if (StatsManager.instance.playerUpgradeRange.TryGetValue(key, out var value5))
			{
				num5 = value5;
			}
			stackKills = 0;
			__instance.CrouchSpeed = origCrouchSpeed;
			__instance.MoveSpeed = origMoveSpeed;
			__instance.SprintSpeed = origSprintSpeed + (float)num2 * 1f;
			__instance.JumpForce = origJumpForce;
			__instance.EnergyStart = origMaxEnergy + (float)num3 * 10f;
			__instance.EnergyCurrent = origMaxEnergy + (float)num3 * 10f;
			PlayerAvatar.instance.physGrabber.grabRange = origGrabRange + 1f * (float)num5;
			modifyStrength(PlayerController.instance.playerSteamID, origGrabStrength + (float)num4 * 0.2f);
			int num6 = origMaxPlayerHealth + num * 20;
			int health = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
			if ((int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) > num6)
			{
				setHealth(PlayerController.instance.playerSteamID, num6, num6);
			}
			else
			{
				setHealth(PlayerController.instance.playerSteamID, num6, health);
			}
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
				case 1:
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
						__instance.CrouchSpeed = origCrouchSpeed * speedMultiplier;
						__instance.MoveSpeed = origMoveSpeed * speedMultiplier;
						__instance.SprintSpeed = origSprintSpeed * speedMultiplier + (float)num2 * 1f;
						__instance.EnergyStart = (origMaxEnergy + (float)num3 * 10f) * 1.2f;
						__instance.EnergyCurrent = (origMaxEnergy + (float)num3 * 10f) * 1.2f;
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameRunner.Value;
						RepoRoles.GUIinstance.color = new Color(0.973f, 1f, 0.196f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecRunner.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.973f, 1f, 0.196f);
						break;
					}
				case 2:
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
						int num15 = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
						int num16 = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
						setHealth(PlayerController.instance.playerSteamID, num15 * 2, num16 * 2);
						__instance.CrouchSpeed = origCrouchSpeed * 0.9f;
						__instance.MoveSpeed = origMoveSpeed * 0.9f;
						__instance.SprintSpeed = (origSprintSpeed + (float)num2 * 1f) * 0.9f;
						isTank = true;
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameTank.Value;
						RepoRoles.GUIinstance.color = Color.gray;
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecTank.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = Color.gray;
						break;
					}
				case 3:
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
							__instance.CrouchSpeed = origCrouchSpeed * 1.3f;
							__instance.MoveSpeed = origMoveSpeed * 1.3f;
							__instance.SprintSpeed = origSprintSpeed + (float)num2 * 1.3f;
						}
						else if ((int)array[1] == 1)
						{
							__instance.EnergyStart = (origMaxEnergy + (float)num3 * 10f) * 1.8f;
							__instance.EnergyCurrent = (origMaxEnergy + (float)num3 * 10f) * 1.8f;
						}
						else if ((int)array[1] == 2)
						{
							int num7 = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
							int num8 = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
							setHealth(PlayerController.instance.playerSteamID, (int)((double)num7 * 1.8), (int)((double)num8 * 1.8));
						}
						else if ((int)array[1] == 3)
						{
							modifyStrength(PlayerController.instance.playerSteamID, (origGrabStrength + (float)num4 * 0.2f) * 1.3f);
						}
						else if ((int)array[1] == 4)
						{
							__instance.JumpForce *= 1.5f;
						}
						if ((int)array[2] == 0)
						{
							__instance.CrouchSpeed = origCrouchSpeed;
							__instance.MoveSpeed = origMoveSpeed;
							__instance.SprintSpeed = origSprintSpeed + (float)num2 * 0.8f;
						}
						else if ((int)array[2] == 1)
						{
							__instance.EnergyStart = (origMaxEnergy + (float)num3 * 10f) * 0.8f;
							__instance.EnergyCurrent = (origMaxEnergy + (float)num3 * 10f) * 0.8f;
						}
						else if ((int)array[2] == 2)
						{
							int num9 = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
							int num10 = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
							setHealth(PlayerController.instance.playerSteamID, (int)((double)num9 * 0.8), (int)((double)num10 * 0.8));
						}
						else if ((int)array[2] == 3)
						{
							modifyStrength(PlayerController.instance.playerSteamID, (origGrabStrength + (float)num4 * 0.2f) * 0.8f);
						}
						else if ((int)array[2] == 4)
						{
							__instance.JumpForce *= 0.7f;
						}
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameGambler.Value;
						RepoRoles.GUIinstance.color = new Color(0.576f, 0f, 0.831f);
						RepoRoles.GUIinstance.descColor = new Color(0.576f, 0f, 0.831f);
						RepoRoles.GUIinstance.descText = array[0]?.ToString() + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						break;
					}
				case 4:
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
						RepoRoles.Logger.LogInfo((object)("Strength before: " + PhysGrabber.instance.grabStrength));
						modifyStrength(PlayerController.instance.playerSteamID, (origGrabStrength + (float)num4 * 0.2f) * 1.5f + 0.5f);
						RepoRoles.Logger.LogInfo((object)("Strength after: " + PhysGrabber.instance.grabStrength));
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameStrongman.Value;
						RepoRoles.GUIinstance.color = new Color(0.761f, 0.055f, 0.055f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecStrongman.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.761f, 0.055f, 0.055f);
						break;
					}
				case 5:
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
						PhysGrabber.instance.grabRange = (origGrabRange + (float)num5 * 1f) * 2.5f;
						modifyStrength(PlayerController.instance.playerSteamID, (origGrabStrength + (float)num4 * 0.2f) * 1.2f);
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameRL.Value;
						RepoRoles.GUIinstance.color = new Color(0.592f, 0.969f, 0.663f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecRL.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.592f, 0.969f, 0.663f);
						break;
					}
				case 6:
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
						modifyStrength(PlayerController.instance.playerSteamID, (origGrabStrength + (float)num4 * 0.2f) * 1.3f);
						__instance.EnergyStart = origMaxEnergy + (float)num3 * 10f + 20f;
						__instance.EnergyCurrent = origMaxEnergy + (float)num3 * 10f + 20f;
						__instance.JumpForce = origJumpForce + 3f;
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameAthletic.Value;
						RepoRoles.GUIinstance.color = Color.white;
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecAthletic.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = Color.white;
						break;
					}
				case 7:
					{
						if (!RepoRoles.enableMage.Value)
						{
							assignRole(rnd.Next(1, roleAmount + 1), PlayerController.instance);
							RepoRoles.Logger.LogInfo((object)"You got assigned a new random role because this one was disabled.");
							break;
						}
						guiManager.ResetManaUI();
						RepoRoles.Logger.LogInfo("Resetting Mana UI.");
						RepoRoles.Logger.LogMessage((object)("Assigning role " + RepoRoles.customRoleNameMage.Value + "."));
						guiManager.isMage = true;
						guiManager.aviableMana = 8;
						ManaHelper.CreateUI();
						guiManager.manaTicks = 0;
						int num13 = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
						int num14 = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
						if ((double)num14 * 0.5 > 0.0)
						{
							setHealth(PlayerController.instance.playerSteamID, (int)((double)num13 * 0.5), (int)((double)num14 * 0.5));
						}
						else
						{
							setHealth(PlayerController.instance.playerSteamID, (int)((double)num13 * 0.5), num14);
						}
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameMage.Value;
						RepoRoles.GUIinstance.color = new Color(0f, 0.384f, 1f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecMage.Value + "\nPress " + ((object)RepoRoles.showSpellsKey.Value/*cast due to .constrained prefix*/).ToString() + " to see all your spells and press " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0f, 0.384f, 1f);
						break;
					}
				case 8:
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
						int num11 = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
						int num12 = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
						setHealth(PlayerController.instance.playerSteamID, (int)((double)num11 * 1.5), (int)((double)num12 * 1.5));
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameReaper.Value;
						RepoRoles.GUIinstance.color = new Color(0.141f, 0.6f, 0.502f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecReaper.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.141f, 0.6f, 0.502f);
						break;
					}
				case 9:
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
						__instance.EnergyStart = (origMaxEnergy + (float)num3 * 10f) * 2f;
						__instance.EnergyCurrent = (origMaxEnergy + (float)num3 * 10f) * 2f;
						__instance.sprintRechargeAmount *= 2f;
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameScout.Value;
						RepoRoles.GUIinstance.color = new Color(0.902f, 0.733f, 0.11f);
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecScout.Value.Replace("[G]", "[" + ((object)RepoRoles.scoutKey.Value/*cast due to .constrained prefix*/).ToString() + "]") + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = new Color(0.902f, 0.733f, 0.11f);
						break;
					}
				case 10:
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
						__instance.CrouchSpeed = origCrouchSpeed;
						__instance.MoveSpeed = origMoveSpeed;
						__instance.SprintSpeed = origSprintSpeed + (float)num2 * 1f;
						__instance.JumpForce = origJumpForce;
						__instance.EnergyStart = origMaxEnergy + (float)num3 * 10f;
						__instance.EnergyCurrent = origMaxEnergy + (float)num3 * 10f;
						RepoRoles.GUIinstance.text = RepoRoles.customRoleNameRegular.Value;
						RepoRoles.GUIinstance.color = Color.white;
						RepoRoles.GUIinstance.descText = RepoRoles.customRoleDecRegular.Value + "\nPress " + ((object)RepoRoles.toggleKey.Value/*cast due to .constrained prefix*/).ToString() + " to continue";
						RepoRoles.GUIinstance.descColor = Color.white;
						break;
					}
				}
			}
		}
	}
