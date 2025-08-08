using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MenuLib;
using MenuLib.MonoBehaviors;
using REPOLib;
using REPOLib.Modules;
using R.E.P.O.Roles;
using R.E.P.O.Roles.patches;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MenuLib.MenuAPI;
using static MenuLib.MonoBehaviors.REPOSlider;

namespace Repo_Roles
{
	[BepInPlugin("R3Labs.Repo_Roles.Classic", "REPO Roles Classic", "2.1.1")]
	[BepInDependency("REPOLib", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("MenuLib", BepInDependency.DependencyFlags.SoftDependency)]
	public class RepoRoles : BaseUnityPlugin
	{
		public static RepoRoles Instance { get; private set; }
		internal static new ManualLogSource Logger { get; private set; }

		public readonly Harmony harmony = new Harmony("Repo_Roles");

		public static Font newFont;

		public static guiManager GUIinstance;

		public static ConfigEntry<KeyCode> toggleKey;

		public static ConfigEntry<KeyCode> showSpellsKey;

		public static ConfigEntry<KeyCode> healKey;

		public static ConfigEntry<KeyCode> speedKey;

		public static ConfigEntry<KeyCode> overchargeKey;

		public static ConfigEntry<KeyCode> jumpKey;

		public static ConfigEntry<KeyCode> staminaKey;

		public static ConfigEntry<KeyCode> scoutKey;

		public static ConfigEntry<string> customRoleNameRunner;

		public static ConfigEntry<string> customRoleNameTank;

		public static ConfigEntry<string> customRoleNameGambler;

		public static ConfigEntry<string> customRoleNameStrongman;

		public static ConfigEntry<string> customRoleNameRL;

		public static ConfigEntry<string> customRoleNameAthletic;

		public static ConfigEntry<string> customRoleNameMage;

		public static ConfigEntry<string> customRoleNameReaper;

		public static ConfigEntry<string> customRoleNameScout;

		public static ConfigEntry<string> customRoleNameRegular;

		public static ConfigEntry<string> customRoleDecRunner;

		public static ConfigEntry<string> customRoleDecTank;

		public static ConfigEntry<string> customRoleDecGambler;

		public static ConfigEntry<string> customRoleDecStrongman;

		public static ConfigEntry<string> customRoleDecRL;

		public static ConfigEntry<string> customRoleDecAthletic;

		public static ConfigEntry<string> customRoleDecMage;

		public static ConfigEntry<string> customRoleDecReaper;

		public static ConfigEntry<string> customRoleDecScout;

		public static ConfigEntry<string> customRoleDecRegular;

		public static ConfigEntry<string> savedRole;

		public static ConfigEntry<bool> assignRoleAfterRevive;

		public static ConfigEntry<bool> mageTopManaConf;

		public static ConfigEntry<bool> showGUIAtStart;

		public ConfigDefinition showGUIAtStartDef = new ConfigDefinition("GUI", "Show GUI when being assigned a role");

		public ConfigDefinition mageTopManaDef = new ConfigDefinition("Role", "Place where mana is shown");

		public ConfigDefinition def = new ConfigDefinition("GUI", "GUI Toggle Key");

		public ConfigDefinition scoutButtonDef = new ConfigDefinition("GUI", "Activate Scout Ability");

		public ConfigDefinition customRoleNameRunnerDef = new ConfigDefinition("Role Names", "Runner Name");

		public ConfigDefinition customRoleNameTankDef = new ConfigDefinition("Role Names", "Tank Name");

		public ConfigDefinition customRoleNameGamblerDef = new ConfigDefinition("Role Names", "Gambler Name");

		public ConfigDefinition customRoleNameStrongmanDef = new ConfigDefinition("Role Names", "Strongman Name");

		public ConfigDefinition customRoleNameRLDef = new ConfigDefinition("Role Names", "Ranged Looter Name");

		public ConfigDefinition customRoleNameAthleticDef = new ConfigDefinition("Role Names", "Athletic Name");

		public ConfigDefinition customRoleNameMageDef = new ConfigDefinition("Role Names", "Mage Name");

		public ConfigDefinition customRoleNameReaperDef = new ConfigDefinition("Role Names", "Reaper Name");

		public ConfigDefinition customRoleNameScoutDef = new ConfigDefinition("Role Names", "Scout Name");

		public ConfigDefinition customRoleNameRegularDef = new ConfigDefinition("Role Names", "Regular Name");

		public ConfigDefinition customRoleDesRunnerDef = new ConfigDefinition("Role Descriptions", "Runner Description");

		public ConfigDefinition customRoleDesTankDef = new ConfigDefinition("Role Descriptions", "Tank Description");

		public ConfigDefinition customRoleDesGamblerDef = new ConfigDefinition("Role Descriptions", "Gambler Description");

		public ConfigDefinition customRoleDesStrongmanDef = new ConfigDefinition("Role Descriptions", "Strongman Description");

		public ConfigDefinition customRoleDesRLDef = new ConfigDefinition("Role Descriptions", "Ranged Looter Description");

		public ConfigDefinition customRoleDesAthleticDef = new ConfigDefinition("Role Descriptions", "Athletic Description");

		public ConfigDefinition customRoleDesMageDef = new ConfigDefinition("Role Descriptions", "Mage Description");

		public ConfigDefinition customRoleDesReaperDef = new ConfigDefinition("Role Descriptions", "Reaper Description");

		public ConfigDefinition customRoleDesScoutDef = new ConfigDefinition("Role Descriptions", "Scout Description");

		public ConfigDefinition customRoleDesRegularDef = new ConfigDefinition("Role Descriptions", "Regular Description");

		public ConfigDefinition showSpellsDef = new ConfigDefinition("Mage", "Show Spells");

		public ConfigDefinition healDef = new ConfigDefinition("Mage", "Healing Spell");

		public ConfigDefinition speedDef = new ConfigDefinition("Mage", "Speed Spell");

		public ConfigDefinition overchargeDef = new ConfigDefinition("Mage", "Overcharge Spell");

		public ConfigDefinition jumpDef = new ConfigDefinition("Mage", "Jump Boost Spell");

		public ConfigDefinition staminaDef = new ConfigDefinition("Mage", "Stamina Refill Spell");

		public ConfigDefinition selectedRoleDef = new ConfigDefinition("Role", "Your selected role");

		public ConfigDefinition afterDeathRoleDef = new ConfigDefinition("Role", "Reaasign roles after being revived");

		public static ConfigEntry<bool> enableRunner;

		public static ConfigEntry<bool> enableTank;

		public static ConfigEntry<bool> enableGambler;

		public static ConfigEntry<bool> enableStrongman;

		public static ConfigEntry<bool> enableRL;

		public static ConfigEntry<bool> enableAthletic;

		public static ConfigEntry<bool> enableMage;

		public static ConfigEntry<bool> enableReaper;

		public static ConfigEntry<bool> enableScout;

		public static ConfigEntry<bool> enableRegular;

		public ConfigDefinition enableRunnerDef = new ConfigDefinition("Role", "Enable Runner");

		public ConfigDefinition enableTankDef = new ConfigDefinition("Role", "Enable Tank");

		public ConfigDefinition enableGamblerDef = new ConfigDefinition("Role", "Enable Gambler");

		public ConfigDefinition enableStrongmanDef = new ConfigDefinition("Role", "Enable Strongman");

		public ConfigDefinition enableRLDef = new ConfigDefinition("Role", "Enable Ranged Looter");

		public ConfigDefinition enableAthleticDef = new ConfigDefinition("Role", "Enable Athletic");

		public ConfigDefinition enableMageDef = new ConfigDefinition("Role", "Enable Mage");

		public ConfigDefinition enableReaperDef = new ConfigDefinition("Role", "Enable Reaper");

		public ConfigDefinition enableScoutDef = new ConfigDefinition("Role", "Enable Scout");

		public ConfigDefinition enableRegularDef = new ConfigDefinition("Role", "Enable Regular");

		private REPOSlider slider;

		private REPOPopupPage configPage;

		private REPOButton buttonOpen;

		private REPOButton buttonOpenLobby;

		private REPOButton buttonClose;

		private REPOToggle afterDeathRoleToggle;

		private REPOToggle mageTopMana;

		public static bool afterDeathNewRole = false;

		public static bool mageTopManaBool;

		public static string chosenRoleName = "Random";

		public void sliderConf(string a)
		{
			savedRole.Value = a;
			chosenRoleName = a;
		}

		public void aDRC(bool a)
		{
			assignRoleAfterRevive.Value = a;
			afterDeathNewRole = a;
		}

		public void mageManaFunc(bool a)
		{
			mageTopManaConf.Value = a;
			mageTopManaBool = a;
		}

		public static string getPath()
		{
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "REPORoles_assets2");
		}

		private void Awake()
		{
			Instance = this;
			Logger = base.Logger;
			savedRole = Config.Bind(selectedRoleDef, "Random", null);
			assignRoleAfterRevive = Config.Bind(afterDeathRoleDef, false, null);
			mageTopManaConf = Config.Bind(mageTopManaDef, true, null);
			mageTopManaBool = mageTopManaConf.Value;
			showGUIAtStart = Config.Bind(showGUIAtStartDef, true, null);
			enableRunner = Config.Bind(enableRunnerDef, true, null);
			enableTank = Config.Bind(enableTankDef, true, null);
			enableGambler = Config.Bind(enableGamblerDef, true, null);
			enableStrongman = Config.Bind(enableStrongmanDef, true, null);
			enableRL = Config.Bind(enableRLDef, true, null);
			enableAthletic = Config.Bind(enableAthleticDef, true, null);
			enableMage = Config.Bind(enableMageDef, true, null);
			enableReaper = Config.Bind(enableReaperDef, true, null);
			enableScout = Config.Bind(enableScoutDef, true, null);
			enableRegular = Config.Bind(enableRegularDef, true, null);
			Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded successfully.");
			MenuAPI.AddElementToSettingsMenu((BuilderDelegate)delegate (Transform parent)
			{
				if ((UnityEngine.Object)(object)configPage == null)
				{
					configPage = MenuAPI.CreateREPOPopupPage("REPO Roles", 0, true, true, 1.5f);
				}
				buttonOpen = MenuAPI.CreateREPOButton("REPO Roles Config", (Action)delegate
				{
					configPage.OpenPage(false);
				}, parent, new Vector2(500f, 10f));
				configPage.AddElementToScrollView(delegate (Transform scrollView)
				{
					if ((UnityEngine.Object)(object)slider == null)
					{
						slider = MenuAPI.CreateREPOSlider("REPO Roles", "Choose your role", (Action<string>)delegate (string s)
						{
							sliderConf(s);
						}, scrollView, new string[11] { "Random", "Runner", "Tank", "Gambler", "Strongman", "Ranged Looter", "Athletic", "Mage", "Reaper", "Scout", "Regular" }, savedRole.Value, new Vector2(0f, 0f), "", "", (BarBehavior)0);
					}
					return ((REPOElement)slider).rectTransform;
				}, 0f, 0f);

				configPage.AddElementToScrollView(delegate (Transform scrollView)
				{
					if ((UnityEngine.Object)(object)afterDeathRoleToggle == null)
					{
						afterDeathRoleToggle = MenuAPI.CreateREPOToggle("Reassign role on respawn", (Action<bool>)delegate (bool b)
						{
							aDRC(b);
						}, scrollView, new Vector2(0f, 0f), "ON", "OFF", assignRoleAfterRevive.Value);
					}
					return ((REPOElement)afterDeathRoleToggle).rectTransform;
				}, 0f, 0f);

				configPage.AddElementToScrollView(delegate (Transform scrollView)
				{
					if ((UnityEngine.Object)(object)mageTopMana == null)
					{
						mageTopMana = MenuAPI.CreateREPOToggle("Mage Mana Position", (Action<bool>)delegate (bool b)
						{
							mageManaFunc(b);
						}, scrollView, new Vector2(0f, 0f), "TOP", "LEFT", mageTopManaConf.Value);
					}
					return ((REPOElement)mageTopMana).rectTransform;
				}, 0f, 0f);

				configPage.AddElement((BuilderDelegate)delegate (Transform val)
				{
					if ((UnityEngine.Object)(object)buttonClose == null)
					{
						buttonClose = MenuAPI.CreateREPOButton("Back", (Action)delegate
						{
							configPage.ClosePage(false);
						}, val, Vector2.zero);
					}
				});
			});
			MenuAPI.AddElementToLobbyMenu((BuilderDelegate)delegate(Transform parent)
			{
				buttonOpenLobby = MenuAPI.CreateREPOButton("REPO Roles Config", (Action)delegate
				{
					configPage.OpenPage(false);
				}, parent, new Vector2(500f, 500f));
			});
			if ((UnityEngine.Object)(object)Instance == null)
			{
				Instance = this;
			}
			harmony.PatchAll(typeof(RepoRoles));
			harmony.PatchAll(typeof(Sender));
			Logger.LogInfo("Init mod");
			SceneManager.sceneLoaded += delegate
			{
				if (GUIinstance == null)
				{
					GameObject val = new GameObject();
					GUIinstance = val.AddComponent<guiManager>();
					GUIinstance.text = "";
					GUIinstance.color = Color.cyan;
					UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)(object)val);
				}
			};
			toggleKey = Config.Bind(def, (KeyCode)114, null);
			showSpellsKey = Config.Bind(showSpellsDef, (KeyCode)109, null);
			healKey = Config.Bind(healDef, (KeyCode)104, null);
			speedKey = Config.Bind(speedDef, (KeyCode)106, null);
			overchargeKey = Config.Bind(overchargeDef, (KeyCode)111, null);
			jumpKey = Config.Bind(jumpDef, (KeyCode)107, null);
			staminaKey = Config.Bind(staminaDef, (KeyCode)110, null);
			customRoleNameRunner = Config.Bind(customRoleNameRunnerDef, "Runner", null);
			customRoleNameTank = Config.Bind(customRoleNameTankDef, "Tank", null);
			customRoleNameGambler = Config.Bind(customRoleNameGamblerDef, "Gambler", null);
			customRoleNameStrongman = Config.Bind(customRoleNameStrongmanDef, "Strongman", null);
			customRoleNameRL = Config.Bind(customRoleNameRLDef, "Ranged Looter", null);
			customRoleNameAthletic = Config.Bind(customRoleNameAthleticDef, "Athletic", null);
			customRoleNameMage = Config.Bind(customRoleNameMageDef, "Mage", null);
			customRoleNameReaper = Config.Bind(customRoleNameReaperDef, "Reaper", null);
			customRoleNameScout = Config.Bind(customRoleNameScoutDef, "Scout", null);
			customRoleNameRegular = Config.Bind(customRoleNameRegularDef, "Regular", null);
			customRoleDecRunner = Config.Bind(customRoleDesRunnerDef, "You have more stamina and run much faster than everyone else!", null);
			customRoleDecTank = Config.Bind(customRoleDesTankDef, "You walk slower but your hp is doubled!", null);
			customRoleDecGambler = Config.Bind(customRoleDesGamblerDef, "You rolled random effects:", null);
			customRoleDecStrongman = Config.Bind(customRoleDesStrongmanDef, "You\u00b4re incredibly strong!", null);
			customRoleDecRL = Config.Bind(customRoleDesRLDef, "You can reach objects from far away!", null);
			customRoleDecAthletic = Config.Bind(customRoleDesAthleticDef, "You have more stamina, strength and can jump higher", null);
			customRoleDecMage = Config.Bind(customRoleDesMageDef, "You are able to use your mana to become incredibly strong!", null);
			customRoleDecReaper = Config.Bind(customRoleDesReaperDef, "For each enemy you and your friends kill, you become stronger!", null);
			customRoleDecScout = Config.Bind(customRoleDesScoutDef, "Your stamina is more efficient and by pressing [G] you can see all enemies around you.", null);
			customRoleDecRegular = Config.Bind(customRoleDesRegularDef, "You are just a regular Semibot. Nothing special.", null);
			scoutKey = Config.Bind(scoutButtonDef, (KeyCode)103, null);
			harmony.PatchAll(typeof(PunManagerPatch));
			harmony.PatchAll(typeof(PlayerAvatarPatch));
			harmony.PatchAll(typeof(StatsManagerPatch));
			BundleLoader.LoadBundle(getPath(), delegate(AssetBundle assetBundle)
			{
				Item val = assetBundle.LoadAsset<Item>("Mana Regeneration Upgrade");
				Items.RegisterItem(val);
				Item val2 = assetBundle.LoadAsset<Item>("Scout Cooldown Upgrade");
				Items.RegisterItem(val2);
			}, false);
		}

		public static void Update_ManaRegeneration()
		{
			if (LevelGenerator.Instance.Generated && !SemiFunc.MenuLevel())
			{
				int stat = Upgrader.GetStat(PlayerAvatar.instance.steamID, "playerUpgradeManaRegeneration");
				guiManager.manaRegenRate = Upgrader.GetStat(PlayerAvatar.instance.steamID, "playerUpgradeManaRegeneration");
			}
		}

		public static void Update_ScoutCooldown()
		{
			if (LevelGenerator.Instance.Generated && !SemiFunc.MenuLevel())
			{
				int stat = Upgrader.GetStat(PlayerAvatar.instance.steamID, "playerUpgradeScoutCooldownReduction");
				ScoutMarker.reductionUpgrades = Upgrader.GetStat(PlayerAvatar.instance.steamID, "playerUpgradeScoutCooldownReduction");
			}
		}
	}
}
