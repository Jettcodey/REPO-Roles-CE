using HarmonyLib;
using R.E.P.O.Roles.patches;
using R.E.P.O.Roles;
using UnityEngine;

namespace Repo_Roles;

public class ScoutMarker : MonoBehaviour
{
	private int cooldownTicker = 0;

	private int activeTicker = 250;

	private bool isActive = false;

	private bool onCooldown = true;

	public static int reductionUpgrades;

	private void OnGUI()
	{
		if (!SemiFunc.RunIsLevel() || SemiFunc.RunIsShop() || PlayerAvatar.instance.playerHealth.health <= 0)
		{
			return;
		}
		GUIStyle val = new GUIStyle(GUI.skin.label)
		{
			alignment = (TextAnchor)4,
			fontSize = 22
		};
		val.normal.textColor = new Color(0.902f, 0.733f, 0.11f);
		if ((Object)(object)guiManager.customFont != null)
		{
			val.font = guiManager.customFont;
		}
		if (EnemyDirector.instance.enemiesSpawned != null && ClassManager.isScout && isActive)
		{
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				alignment = (TextAnchor)4,
				fontSize = 22
			};
			val2.normal.textColor = Color.red;
			if ((Object)(object)guiManager.customFont != null)
			{
				val2.font = guiManager.customFont;
			}
			foreach (EnemyParent item in EnemyDirector.instance.enemiesSpawned)
			{
				if ((Object)(object)item == null || !item.EnableObject.activeInHierarchy)
				{
					continue;
				}
				Enemy val3 = (Enemy)AccessTools.Field(typeof(EnemyParent), "Enemy").GetValue(item);
				if ((Object)(object)val3 != null && (Object)(object)Camera.main != null)
				{
					Vector3 val4 = Camera.main.WorldToViewportPoint(val3.CenterTransform.position);
					if (!(val4.z < 0f))
					{
						string enemyName = item.enemyName;
						string text = $"{enemyName} [{val4.z:F0}m]";
						Vector2 val5 = val2.CalcSize(new GUIContent(text));
						float num = val4.x * (float)Screen.width - val5.x / 2f;
						float num2 = (float)Screen.height - val4.y * (float)Screen.height - val5.y / 2f;
						GUI.Label(new Rect(num, num2, val5.x, val5.y), text, val2);
					}
				}
			}
			if (isActive && !onCooldown)
			{
				int num3 = Screen.width / 2 - 100;
				int num4 = Screen.height - 125;
				string text2 = (activeTicker / 50).ToString();
				string text3 = "Ability runs out in: " + text2 + "s";
				GUI.Label(new Rect((float)num3, (float)num4, 200f, 50f), text3, val);
			}
		}
		if (!isActive && ClassManager.isScout && onCooldown)
		{
			int num5 = Screen.width / 2 - 100;
			int num6 = Screen.height - 125;
			string text4 = (cooldownTicker / 50).ToString();
			val.normal.textColor = new Color(0.851f, 0.667f, 0.098f);
			string text5 = "Ability is ready in: " + text4 + "s";
			GUI.Label(new Rect((float)num5, (float)num6, 200f, 60f), text5, val);
		}
		else if (!isActive && ClassManager.isScout && cooldownTicker <= 0)
		{
			int num7 = Screen.width / 2 - 100;
			int num8 = Screen.height - 125;
			string text6 = (cooldownTicker / 50).ToString();
			val.normal.textColor = Color.green;
			string text7 = "Ability is ready!\nPress [" + ((object)RepoRoles.scoutKey.Value/*cast due to .constrained prefix*/).ToString() + "] to activate";
			GUI.Label(new Rect((float)num7, (float)num8, 200f, 60f), text7, val);
		}
	}

	private void FixedUpdate()
	{
		if (!ClassManager.isScout)
		{
			return;
		}
		if (isActive)
		{
			activeTicker--;
		}
		if (activeTicker <= 0)
		{
			isActive = false;
			activeTicker = 250;
			onCooldown = true;
			if (reductionUpgrades <= 5)
			{
				cooldownTicker = 2000 - reductionUpgrades * 250;
			}
			else
			{
				cooldownTicker = 750;
			}
		}
		if (cooldownTicker <= 0)
		{
			onCooldown = false;
		}
		if (cooldownTicker >= 0 && onCooldown)
		{
			cooldownTicker--;
		}
	}

	private void Update()
	{
		if (ClassManager.isScout && SemiFunc.RunIsLevel() && !SemiFunc.RunIsShop() && !ChatManager.instance.chatActive && !onCooldown && Input.GetKeyDown(RepoRoles.scoutKey.Value))
		{
			isActive = true;
		}
	}
}
