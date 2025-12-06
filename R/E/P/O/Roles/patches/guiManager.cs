using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Repo_Roles;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using System.Collections;

namespace R.E.P.O.Roles.patches
{
	public class guiManager : MonoBehaviour
	{
		private bool lastManaPosition = true;

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

		public static int manaTicks = 0;

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
				// tighter body (single newlines, no extra blank lines)
				string headerText = "SPELLS";
				string bodyText =
					"[" + RepoRoles.healKey.Value.ToString() + "] Heal yourself for 5 health (1 Mana)\n"
					+ "[" + RepoRoles.staminaKey.Value.ToString() + "] Regenerate your stamina to full (3 Mana)\n"
					+ "[" + RepoRoles.speedKey.Value.ToString() + "] Become faster for 30 seconds (2 Mana)\n"
					+ "[" + RepoRoles.overchargeKey.Value.ToString() + "] Boost the effectivity of spells used in the next 20 seconds (3 Mana)\n"
					+ "[" + RepoRoles.jumpKey.Value.ToString() + "] Jump higher for 30 seconds (2 Mana)";

				// center anchors
				float centerX = Screen.width * 0.5f;
				float centerY = Screen.height * 0.5f;

				// width available to wrap the body
				float areaWidth = Mathf.Clamp(Screen.width * 0.6f, 260f, 1200f);

				// base styles
				GUIStyle headerStyle = new GUIStyle();
				headerStyle.richText = true;
				headerStyle.wordWrap = true;
				headerStyle.alignment = TextAnchor.MiddleCenter;
				headerStyle.font = customFont;
				headerStyle.fontStyle = FontStyle.Bold;

				GUIStyle bodyStyle = new GUIStyle(headerStyle); // share common settings
				bodyStyle.wordWrap = true;

				GUIStyle outlineHeader = new GUIStyle(headerStyle);
				outlineHeader.normal.textColor = Color.black;
				GUIStyle outlineBody = new GUIStyle(bodyStyle);
				outlineBody.normal.textColor = Color.black;

				// helper to find font size that keeps rendered height below a limit
				int GetFittingFontSize(string s, GUIStyle style, float width, int maxSize, int minSize, float maxAllowedHeight)
				{
					for (int size = maxSize; size >= minSize; size--)
					{
						style.fontSize = size;
						float needed = style.CalcHeight(new GUIContent(s), width);
						if (needed <= maxAllowedHeight)
							return size;
					}
					return minSize;
				}

				// dynamic max font based on screen size (keeps scaling on very large screens)
				int maxHeader = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.09f), 24, 120); // ~9% of height
				int maxBody = Mathf.Clamp(Mathf.RoundToInt(Screen.height * 0.045f), 14, 72);  // ~4.5% of height
				int minHeader = 16;
				int minBody = 12;

				// limit: don't allow a single label to be taller than screen minus margin
				float maxAllowedLabelHeight = Screen.height - 60f;

				// find header size then body size. We allow header to be big; body must fit in remaining space
				// First pick header size that fits (but header can be up to maxHeader)
				int headerSize = GetFittingFontSize(headerText, headerStyle, areaWidth, maxHeader, minHeader, maxAllowedLabelHeight);
				headerSize = Mathf.Max(minHeader, headerSize + 4); // keep your +4 bump
				headerStyle.fontSize = headerSize;
				outlineHeader.fontSize = headerSize;

				// compute header rendered height
				float headerHeight = headerStyle.CalcHeight(new GUIContent(headerText), areaWidth);

				// Remaining vertical space for body (allow some margin)
				float remainingForBody = Mathf.Max(60f, Screen.height - headerHeight - 80f);

				// pick body size to fit inside remaining space
				int bodySize = GetFittingFontSize(bodyText, bodyStyle, areaWidth, maxBody, minBody, remainingForBody);
				bodySize = Mathf.Max(minBody, bodySize + 4); // same +4 bump
				bodyStyle.fontSize = bodySize;
				outlineBody.fontSize = bodySize;

				// compute body rendered height
				float bodyHeight = bodyStyle.CalcHeight(new GUIContent(bodyText), areaWidth);

				// small gap between header and body. keep it proportional and small so spacing isn't huge
				float gap = Mathf.Clamp(Mathf.RoundToInt(headerSize * 0.25f), 4, 14);

				// total block height and rect centered on screen
				float totalHeight = headerHeight + gap + bodyHeight;
				Rect blockRect = new Rect(centerX - areaWidth * 0.5f, centerY - totalHeight * 0.5f, areaWidth, totalHeight);

				// positions for header and body inside the block
				Rect headerRect = new Rect(blockRect.x, blockRect.y, blockRect.width, headerHeight);
				Rect bodyRect = new Rect(blockRect.x, blockRect.y + headerHeight + gap, blockRect.width, bodyHeight);

				// draw outline / shadow for header and body (offset in 8 directions)
				float offset = Mathf.Clamp(Screen.height * 0.0025f, 1f, 3f); // scale shadow a bit with resolution
																			 // header outline
				GUI.Label(new Rect(headerRect.x - offset, headerRect.y, headerRect.width, headerRect.height), headerText, outlineHeader);
				GUI.Label(new Rect(headerRect.x + offset, headerRect.y, headerRect.width, headerRect.height), headerText, outlineHeader);
				GUI.Label(new Rect(headerRect.x, headerRect.y - offset, headerRect.width, headerRect.height), headerText, outlineHeader);
				GUI.Label(new Rect(headerRect.x, headerRect.y + offset, headerRect.width, headerRect.height), headerText, outlineHeader);
				// body outline (do same offsets)
				GUI.Label(new Rect(bodyRect.x - offset, bodyRect.y, bodyRect.width, bodyRect.height), bodyText, outlineBody);
				GUI.Label(new Rect(bodyRect.x + offset, bodyRect.y, bodyRect.width, bodyRect.height), bodyText, outlineBody);
				GUI.Label(new Rect(bodyRect.x, bodyRect.y - offset, bodyRect.width, bodyRect.height), bodyText, outlineBody);
				GUI.Label(new Rect(bodyRect.x, bodyRect.y + offset, bodyRect.width, bodyRect.height), bodyText, outlineBody);

				// main colored labels
				GUIStyle headerColor = new GUIStyle(headerStyle);
				headerColor.normal.textColor = new Color(0f, 0.384f, 1f);
				GUIStyle bodyColor = new GUIStyle(bodyStyle);
				bodyColor.normal.textColor = new Color(0f, 0.384f, 1f);

				GUI.Label(headerRect, headerText, headerColor);
				GUI.Label(bodyRect, bodyText, bodyColor);
			}
		}

		public void InitializeMageDisplay()
		{
			if (!isMage) return;

			// Clean up any existing UI
			if (ManaHelper.val2 != null)
			{
				GameObject.Destroy(ManaHelper.val2);
				ManaHelper.val2 = null;
				ManaHelper.manaUI = null;
			}

			// Reset last position to force update
			lastManaPosition = !RepoRoles.mageTopManaBool;

			// Update display based on config
			UpdateManaDisplay();

			// Set initial mana value
			if (!RepoRoles.mageTopManaBool && ManaHelper.manaUI != null)
			{
				ManaHelper.manaUI.SetMana(aviableMana, 8f);
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

		public void UpdateManaDisplay()
		{
			if (!isMage) return;

			// Ensure UI exists
			if (ManaHelper.val2 == null)
			{
				ManaHelper.CreateUI();
			}

			// Show/hide based on config
			if (RepoRoles.mageTopManaBool)
			{
				// TOP display: Hide custom UI (OnGUI will draw)
				ManaHelper.ShowUI(false);
			}
			else
			{
				// LEFT display: Show custom UI
				ManaHelper.ShowUI(true);
				ManaHelper.UpdateManaValue();
			}
		}

		private void updateMana()
		{
			if (!isMage) return;

			// Update mana value in the appropriate display
			if (RepoRoles.mageTopManaBool)
			{
				// TOP display updates automatically in OnGUI
			}
			else
			{
				// LEFT display: Update the custom UI
				ManaHelper.UpdateManaValue();
			}
		}

		private void Update()
		{
			// Check if mana position config has changed and update display
			if (isMage && lastManaPosition != RepoRoles.mageTopManaBool)
			{
				UpdateManaDisplay();
				lastManaPosition = RepoRoles.mageTopManaBool;
			}
			if (SemiFunc.RunIsLevel() && (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) <= 0)
			{
				isDead = true;
			}
			if (!SemiFunc.RunIsLevel())
			{
				isMage = false;
				if (Sender.manager != null && (Object)(object)PlayerAvatar.instance != null)
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
							UpdateManaDisplay();
						}
					}
					else if ((int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth) != (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth) && aviableMana >= 1)
					{
						PlayerAvatar.instance.playerHealth.Heal((int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(PlayerAvatar.instance.playerHealth) - (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(PlayerAvatar.instance.playerHealth), true);
						aviableMana--;
						showManaUsage = true;
						manaUsageTicker = 0;
						manaUsageText = "Used Heal (-1 Mana)";
						UpdateManaDisplay();
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
						UpdateManaDisplay();
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
					UpdateManaDisplay();
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
						UpdateManaDisplay();
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
					UpdateManaDisplay();
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
			if (!isMage)
			{
				// Clean up UI when no longer a mage
				if (ManaHelper.val2 != null)
				{
					GameObject.Destroy(ManaHelper.val2);
					ManaHelper.val2 = null;
					ManaHelper.manaUI = null;
				}
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
				UpdateManaDisplay();
			}
		}
	}
}