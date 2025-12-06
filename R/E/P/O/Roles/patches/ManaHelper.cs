using Repo_Roles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace R.E.P.O.Roles.patches
{
	public class ManaHelper : MonoBehaviour
	{
		public static GameObject val;
		public static GameObject val2;
		public static ManaUI manaUI;

		public static void CreateUI()
		{
			// Only create if it doesn't exist
			if (val2 != null) return;

			// Find the base Energy UI
			val = GameObject.Find("Energy");
			if (val == null)
			{
				RepoRoles.Logger.LogError("Could not find base 'Energy' UI. Mana UI cannot be created.");
				return;
			}

			// Clone the Energy UI
			val2 = GameObject.Instantiate(val, val.transform.parent);
			if (val2 == null)
			{
				RepoRoles.Logger.LogError("Failed to instantiate Mana UI.");
				return;
			}

			val2.name = "Mana";
			val2.transform.localPosition -= new Vector3(0f, 33f, 0f);

			// Remove problematic components
			RemoveProblematicComponents(val2);

			// Color the text blue
			foreach (var tmp in val2.GetComponentsInChildren<TextMeshProUGUI>(true))
			{
				if (tmp != null)
					tmp.color = new Color(0f, 0.384f, 1f);
			}

			// Set mana icon
			SetManaIcon(val2);

			// Add our simple ManaUI component
			manaUI = val2.AddComponent<ManaUI>();

			// Initially hide it - we'll show it when needed
			val2.SetActive(false);

			RepoRoles.Logger.LogInfo("Mana UI created successfully");
		}

		private static void RemoveProblematicComponents(GameObject uiObject)
		{
			// Remove EnergyUI if present
			var energyUI = uiObject.GetComponent<EnergyUI>();
			if (energyUI != null)
				GameObject.DestroyImmediate(energyUI);

			// Remove any SemiUI components
			var semiUI = uiObject.GetComponent<SemiUI>();
			if (semiUI != null)
				GameObject.DestroyImmediate(semiUI);
		}

		private static void SetManaIcon(GameObject uiObject)
		{
			Texture2D manaTexture = guiManager.manaTexture;
			if (manaTexture == null) return;

			var sprite = Sprite.Create(manaTexture,
				new Rect(0, 0, manaTexture.width, manaTexture.height),
				new Vector2(0.5f, 0.5f));

			var image = uiObject.GetComponentInChildren<Image>(true);
			if (image != null)
			{
				image.sprite = sprite;
				image.color = Color.white;
			}
		}

		public static void ShowUI(bool show)
		{
			if (val2 != null)
			{
				val2.SetActive(show);
			}
		}

		public static void UpdateManaValue()
		{
			if (manaUI != null && guiManager.isMage)
			{
				manaUI.SetMana(guiManager.aviableMana, 8f);
			}
		}
	}
}