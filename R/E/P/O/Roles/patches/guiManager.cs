using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Repo_Roles;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace R.E.P.O.Roles.patches
{
    public class guiManager : MonoBehaviour
    {
    	public static bool showGUI = true;

    	public static bool isMage = false;

    	private static bool showSpells = false;

    	private bool isDead = false;

    	public string text;

    	public Color color = Color.white;

    	public string descText;

    	public Color descColor = Color.white;

    	private Texture2D _backgroundTexture;

    	public static Texture2D manaTexture;

    	public static Font customFont;

    	private static AssetBundle fontBundle;

    	public static int aviableMana = 0;

    	public static int manaRegenRate = 0;

    	public int speedTicker = 0;

    	public int manaTicks = 0;

    	public bool speedActive;

    	private float multiSpeed = 1.4f;

    	private float crouchSpeedBefore;

    	private float moveSpeedBefore;

    	private float sprintSpeedBefore;

    	private int overchargeMulti = 1;

    	private bool isOvercharged = false;

    	private int overchargeTicker = 0;

    	private bool jumpActive = false;

    	private float jumpOriginal;

    	private int jumpTicker = 0;

    	private int neededJumpTicker = 1800;

    	private int neededSpeedTicker = 1800;

    	private int manaUsageTicker = 0;

    	private bool showManaUsage = false;

    	private Texture2D textureRunner;

    	private string manaUsageText = "Used x (- y Mana)";

    	private bool fontLoaded = false;

    	private bool RunnerTextureLoaded = false;

    	private int passiveTankRegenTicker = 0;

    	public static AssetBundle LoadAssetBundle(string name)
    	{
    		AssetBundle val = null;
    		string text = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), name);
    		return AssetBundle.LoadFromFile(text);
    	}

    	public static Texture2D LoadBottleTextureFromFile()
    	{
    		AssetBundle val = LoadAssetBundle("REPORoles_assets1");
    		Texture2D result = val.LoadAsset<Texture2D>("emptyBottle.png");
    		val.Unload(false);
    		return result;
    	}

    	public static Texture2D LoadFullBottleTextureFromFile()
    	{
    		AssetBundle val = LoadAssetBundle("REPORoles_assets1");
    		Texture2D result = val.LoadAsset<Texture2D>("filledBottle.png");
    		val.Unload(false);
    		return result;
    	}

    	public static Texture2D LoadTextureByName(string bundleName, string assetName)
    	{
    		AssetBundle val = LoadAssetBundle(bundleName);
    		Texture2D result = val.LoadAsset<Texture2D>(assetName);
    		val.Unload(false);
    		return result;
    	}

    	public static void loadFont()
    	{
    		if ((Object)(object)fontBundle == null)
    		{
    			fontBundle = LoadAssetBundle("REPORoles_assets1");
    			customFont = fontBundle.LoadAsset<Font>("font.ttf");
    		}
    		if ((Object)(object)fontBundle == null)
    		{
    			RepoRoles.Logger.LogError((object)"Missing Assets: Put the assets file next to the dll in your plugins folder. If this does not fix it contact us on our discord (see thunderstore description or README)");
    		}
    		fontBundle.Unload(false);
    	}

    	private void Start()
    	{
    		_backgroundTexture = LoadBottleTextureFromFile();
    		_backgroundTexture.Apply();
    		manaTexture = LoadFullBottleTextureFromFile();
    		manaTexture.Apply();
    	}

    	private void OnGUI()
    	{
    		float num = Screen.width / 2 - 100;
    		float num2 = Screen.height / 2 - 60;
    		if (!fontLoaded)
    		{
    			loadFont();
    			fontLoaded = true;
    		}
    		if (SemiFunc.RunIsLevel() && !SemiFunc.RunIsShop() && showGUI)
    		{
    			GUIStyle val = new GUIStyle();
    			val.fontSize = 60;
    			val.fontStyle = (FontStyle)1;
    			val.richText = true;
    			val.normal.textColor = color;
    			val.alignment = (TextAnchor)4;
    			val.font = customFont;
    			GUIStyle val2 = new GUIStyle(val);
    			val2.normal.textColor = Color.black;
    			val2.alignment = (TextAnchor)4;
    			val2.font = customFont;
    			float num3 = 2f;
    			GUI.Label(new Rect(num - num3, num2, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num + num3, num2, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num, num2 - num3, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num, num2 + num3, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num - num3, num2 - num3, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num - num3, num2 + num3, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num + num3, num2 - num3, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num + num3, num2 + num3, 200f, 50f), this.text, val2);
    			GUI.Label(new Rect(num, num2, 200f, 50f), this.text, val);
    			float num4 = Screen.width / 2 - 100;
    			float num5 = Screen.height / 2 + 40;
    			GUIStyle val3 = new GUIStyle();
    			val3.fontSize = 40;
    			val3.fontStyle = (FontStyle)1;
    			val3.richText = true;
    			val3.normal.textColor = color;
    			val3.alignment = (TextAnchor)4;
    			val3.font = customFont;
    			GUIStyle val4 = new GUIStyle(val3);
    			val4.normal.textColor = Color.black;
    			val4.alignment = (TextAnchor)4;
    			val4.font = customFont;
    			float num6 = 2f;
    			GUI.Label(new Rect(num4 - num6, num5, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4 + num6, num5, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4, num5 - num6, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4, num5 + num6, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4 - num6, num5 - num6, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4 - num6, num5 + num6, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4 + num6, num5 - num6, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4 + num6, num5 + num6, 200f, 50f), descText, val4);
    			GUI.Label(new Rect(num4, num5, 200f, 50f), descText, val3);
    		}
    		if (!isMage || !SemiFunc.RunIsLevel() || SemiFunc.RunIsShop() || (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) <= 0)
    		{
    			return;
    		}
    		GUIStyle val5 = new GUIStyle();
    		val5.alignment = (TextAnchor)1;
    		val5.normal.background = manaTexture;
    		val5.font = customFont;
    		GUIStyle val6 = new GUIStyle();
    		val6.alignment = (TextAnchor)1;
    		val6.normal.background = _backgroundTexture;
    		val6.font = customFont;
    		if (showManaUsage && SemiFunc.RunIsLevel() && !SemiFunc.RunIsShop() && isMage)
    		{
    			int num7 = Screen.width / 2 - 100;
    			int num8 = Screen.height - 125;
    			GUIStyle val7 = new GUIStyle();
    			val7.fontSize = 20;
    			val7.fontStyle = (FontStyle)1;
    			val7.richText = true;
    			val7.normal.textColor = new Color(0f, 0.384f, 1f);
    			val7.alignment = (TextAnchor)4;
    			val7.font = customFont;
    			GUIStyle val8 = new GUIStyle(val7);
    			val8.normal.textColor = Color.black;
    			val8.alignment = (TextAnchor)4;
    			val8.font = customFont;
    			float num9 = 1.5f;
    			GUI.Label(new Rect((float)num7 - num9, (float)num8, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7 + num9, (float)num8, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7, (float)num8 - num9, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7, (float)num8 + num9, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7 - num9, (float)num8 - num9, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7 - num9, (float)num8 + num9, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7 + num9, (float)num8 - num9, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7 + num9, (float)num8 + num9, 200f, 50f), manaUsageText, val8);
    			GUI.Label(new Rect((float)num7, (float)num8, 200f, 50f), manaUsageText, val7);
    		}
    		if (SemiFunc.RunIsLevel() && !SemiFunc.RunIsShop() && isMage && RepoRoles.mageTopManaBool)
    		{
    			if ((Object)(object)ManaHelper.val == null)
    			{
    				ManaHelper.val = GameObject.Find("Energy");
    			}
    			GUI.Box(new Rect(num - 100f, 50f, 50f, 50f), GUIContent.none, val6);
    			GUI.Box(new Rect(num - 50f, 50f, 50f, 50f), GUIContent.none, val6);
    			GUI.Box(new Rect(num, 50f, 50f, 50f), GUIContent.none, val6);
    			GUI.Box(new Rect(num + 50f, 50f, 50f, 50f), GUIContent.none, val6);
    			GUI.Box(new Rect(num + 100f, 50f, 50f, 50f), GUIContent.none, val6);
    			GUI.Box(new Rect(num + 150f, 50f, 50f, 50f), GUIContent.none, val6);
    			GUI.Box(new Rect(num + 200f, 50f, 50f, 50f), GUIContent.none, val6);
    			GUI.Box(new Rect(num + 250f, 50f, 50f, 50f), GUIContent.none, val6);
    			if (aviableMana >= 1)
    			{
    				GUI.Box(new Rect(num - 100f, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    			if (aviableMana >= 2)
    			{
    				GUI.Box(new Rect(num - 50f, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    			if (aviableMana >= 3)
    			{
    				GUI.Box(new Rect(num, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    			if (aviableMana >= 4)
    			{
    				GUI.Box(new Rect(num + 50f, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    			if (aviableMana >= 5)
    			{
    				GUI.Box(new Rect(num + 100f, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    			if (aviableMana >= 6)
    			{
    				GUI.Box(new Rect(num + 150f, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    			if (aviableMana >= 7)
    			{
    				GUI.Box(new Rect(num + 200f, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    			if (aviableMana >= 8)
    			{
    				GUI.Box(new Rect(num + 250f, 50f, 50f, 50f), GUIContent.none, val5);
    			}
    		}
    		if (showSpells && SemiFunc.RunIsLevel() && !SemiFunc.RunIsShop())
    		{
    			GUIStyle val9 = new GUIStyle();
    			val9.fontSize = 40;
    			val9.fontStyle = (FontStyle)1;
    			val9.richText = true;
    			val9.normal.textColor = new Color(0f, 0.384f, 1f);
    			val9.alignment = (TextAnchor)4;
    			val9.font = customFont;
    			GUIStyle val10 = new GUIStyle(val9);
    			val10.normal.textColor = Color.black;
    			val10.alignment = (TextAnchor)4;
    			val10.font = customFont;
    			string text = "<size=65>SPELLS</size>\n[" + ((object)RepoRoles.healKey.Value/*cast due to .constrained prefix*/).ToString() + "] Heal yourself for 5 health (1 Mana)\n\n[" + ((object)RepoRoles.staminaKey.Value/*cast due to .constrained prefix*/).ToString() + "] Regenerate your stamina to full (3 Mana)\n\n[" + ((object)RepoRoles.speedKey.Value/*cast due to .constrained prefix*/).ToString() + "] Become faster for 30 seconds (2 Mana)\n\n[" + ((object)RepoRoles.overchargeKey.Value/*cast due to .constrained prefix*/).ToString() + "] Boost the effectivity of spells used in the next 20 seconds (3 Mana)\n\n[" + ((object)RepoRoles.jumpKey.Value/*cast due to .constrained prefix*/).ToString() + "] Jump higher for 30 seconds (2 Mana)";
    			float num10 = 2f;
    			GUI.Label(new Rect(num - num10, num2, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num + num10, num2, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num, num2 - num10, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num, num2 + num10, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num - num10, num2 - num10, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num - num10, num2 + num10, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num + num10, num2 - num10, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num + num10, num2 + num10, 200f, 50f), text, val10);
    			GUI.Label(new Rect(num, num2, 200f, 50f), text, val9);
    		}
    	}

    	public static void ResetManaUI()
    	{
    		if (ManaHelper.val2 != null)
    		{
    			Object.Destroy(ManaHelper.val2);
    			ManaHelper.val2 = null;
    		}
    		ManaHelper.manaUI = null;
    	}

    	private void updateMana()
    	{
    		if (!isMage)
    			return;

    		if (ManaHelper.val2 == null || ManaHelper.val2.GetComponent<ManaUI>() == null)
    		{
    			RepoRoles.Logger.LogWarning("[Repo Roles] Mana UI missing or destroyed. Recreating...");
    			ManaHelper.CreateUI();
    		}

    		ManaHelper.val2.GetComponent<ManaUI>().SetMana(aviableMana, 8f);
    	}

    	private void Update()
    	{
    		if (SemiFunc.RunIsLevel() && (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) <= 0)
    		{
    			isDead = true;
    		}
    		if (RepoRoles.mageTopManaBool && isMage && (Object)(object)ManaHelper.val != null && (Object)(object)ManaHelper.val2 != null && ManaHelper.val2.activeInHierarchy)
    		{
    			ManaHelper.val2.SetActive(false);
    		}
    		else if (!RepoRoles.mageTopManaBool && isMage && (Object)(object)ManaHelper.val != null && (Object)(object)ManaHelper.val2 != (Object)null && !ManaHelper.val2.activeInHierarchy)
    		{
    			ManaHelper.val2.SetActive(true);
    			((Behaviour)ManaHelper.val2.gameObject.GetComponent<ManaUI>().uiText).enabled = true;
    		}
    		if (!SemiFunc.RunIsLevel())
    		{
    			isMage = false;
    			if (Sender.manager != null && (Object)(object)PlayerAvatar.instance != (Object)null)
    			{
    				Sender.manager.setReaperStatus(PlayerController.instance.playerSteamID, isReaper: false);
    			}
    			ClassManager.isTank = false;
    			isDead = false;
    		}
    		if (!RepoRoles.afterDeathNewRole)
    		{
    			isDead = false;
    		}
    		if (isDead && (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) > 0 && RepoRoles.afterDeathNewRole)
    		{
    			Sender.manager.assignRoleFromConfig(PlayerController.instance);
    			RepoRoles.Logger.LogInfo((object)"Assigning new role on respawn!");
    			isDead = false;
    		}
    		if (Input.GetKeyDown(RepoRoles.toggleKey.Value) && !ChatManager.instance.chatActive)
    		{
    			showGUI = !showGUI;
    		}
    		if ((int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) > 0 && isMage && !ChatManager.instance.chatActive)
    		{
    			if (Input.GetKeyDown(RepoRoles.showSpellsKey.Value))
    			{
    				showSpells = !showSpells;
    			}
    			if (Input.GetKeyDown(RepoRoles.healKey.Value))
    			{
    				if ((int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) + 5 * overchargeMulti < (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth))
    				{
    					if (aviableMana >= 1)
    					{
    						PlayerAvatar.instance.playerHealth.Heal(5 * overchargeMulti, true);
    						aviableMana--;
    						showManaUsage = true;
    						manaUsageTicker = 0;
    						manaUsageText = "Used Heal (-1 Mana)";
    						updateMana();
    					}
    				}
    				else if ((int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) != (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth) && aviableMana >= 1)
    				{
    					PlayerAvatar.instance.playerHealth.Heal((int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth) - (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth), true);
    					aviableMana--;
    					showManaUsage = true;
    					manaUsageTicker = 0;
    					manaUsageText = "Used Heal (-1 Mana)";
    					updateMana();
    				}
    			}
    			if (Input.GetKeyDown(RepoRoles.speedKey.Value) && !speedActive)
    			{
    				crouchSpeedBefore = PlayerController.instance.CrouchSpeed;
    				moveSpeedBefore = PlayerController.instance.MoveSpeed;
    				sprintSpeedBefore = PlayerController.instance.SprintSpeed;
    				if (aviableMana >= 2)
    				{
    					speedTicker = 0;
    					speedActive = true;
    					PlayerController.instance.CrouchSpeed = crouchSpeedBefore * multiSpeed;
    					PlayerController.instance.MoveSpeed = moveSpeedBefore * multiSpeed;
    					PlayerController.instance.SprintSpeed = sprintSpeedBefore * multiSpeed;
    					aviableMana -= 2;
    					showManaUsage = true;
    					manaUsageTicker = 0;
    					manaUsageText = "Used Speed (-2 Mana)";
    					updateMana();
    					if (isOvercharged)
    					{
    						neededSpeedTicker = 3600;
    					}
    					else
    					{
    						neededSpeedTicker = 1800;
    					}
    				}
    			}
    			if (Input.GetKeyDown(RepoRoles.overchargeKey.Value) && !isOvercharged && aviableMana >= 3)
    			{
    				aviableMana -= 3;
    				isOvercharged = true;
    				overchargeMulti = 2;
    				overchargeTicker = 0;
    				showManaUsage = true;
    				manaUsageTicker = 0;
    				manaUsageText = "Used Overcharge (-3 Mana)";
    				updateMana();
    			}
    			if (Input.GetKeyDown(RepoRoles.jumpKey.Value) && !jumpActive)
    			{
    				jumpOriginal = PlayerController.instance.JumpForce;
    				if (aviableMana >= 2)
    				{
    					jumpTicker = 0;
    					jumpActive = true;
    					PlayerController.instance.JumpForce = jumpOriginal + 3f;
    					aviableMana -= 2;
    					showManaUsage = true;
    					manaUsageTicker = 0;
    					manaUsageText = "Used Jump Boost (-2 Mana)";
    					updateMana();
    					if (isOvercharged)
    					{
    						neededJumpTicker = 3600;
    					}
    					else
    					{
    						neededJumpTicker = 1800;
    					}
    				}
    			}
    			if (Input.GetKeyDown(RepoRoles.staminaKey.Value) && PlayerController.instance.EnergyCurrent < PlayerController.instance.EnergyStart && aviableMana >= 3)
    			{
    				PlayerController.instance.EnergyCurrent = PlayerController.instance.EnergyStart;
    				aviableMana -= 3;
    				showManaUsage = true;
    				manaUsageTicker = 0;
    				manaUsageText = "Used Stamina Refill (-3 Mana)";
    				updateMana();
    			}
    		}
    		if (speedActive && speedTicker >= neededSpeedTicker)
    		{
    			PlayerController.instance.CrouchSpeed = crouchSpeedBefore;
    			PlayerController.instance.MoveSpeed = moveSpeedBefore;
    			PlayerController.instance.SprintSpeed = sprintSpeedBefore;
    			speedActive = false;
    		}
    		if (jumpActive && jumpTicker >= neededJumpTicker)
    		{
    			PlayerController.instance.JumpForce = jumpOriginal;
    			jumpActive = false;
    		}
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

    	private void FixedUpdate()
    	{
    		if (passiveTankRegenTicker < 250 && ClassManager.isTank && SemiFunc.RunIsLevel() && !SemiFunc.RunIsShop() && !isDead)
    		{
    			passiveTankRegenTicker++;
    		}
    		if (passiveTankRegenTicker >= 250 && SemiFunc.RunIsLevel() && !SemiFunc.RunIsShop() && ClassManager.isTank)
    		{
    			passiveTankRegenTicker = 0;
    			int num = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth);
    			int num2 = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth);
    			if (num2 < num)
    			{
    				setHealth(PlayerController.instance.playerSteamID, num, num2 + 1);
    			}
    		}
    		if (overchargeTicker >= 1200)
    		{
    			isOvercharged = false;
    			overchargeMulti = 1;
    		}
    		if (isOvercharged)
    		{
    			overchargeTicker++;
    		}
    		manaUsageTicker++;
    		speedTicker++;
    		jumpTicker++;
    		if (manaUsageTicker >= 420)
    		{
    			showManaUsage = false;
    		}
    		if (aviableMana < 8)
    		{
    			manaTicks++;
    		}
    		if (manaTicks >= 1300 - manaRegenRate * 120 && isMage && aviableMana < 8)
    		{
    			aviableMana++;
    			manaTicks = 0;
    			updateMana();
    		}
    	}
    }
}
