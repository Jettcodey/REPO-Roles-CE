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
			// Clean up previous UI
			if (val2 != null)
			{
				Object.Destroy(val2);
				val2 = null;
				manaUI = null;
			}

			// Find the base Energy UI again
			val = GameObject.Find("Energy");
			if (val == null)
			{
				RepoRoles.Logger.LogError("[Repo Roles] Could not find base 'Energy' UI. Mana UI cannot be created.");
				return;
			}

			// Clone and modify UI
			val2 = Object.Instantiate(val, val.transform.parent);
			if (val2 == null)
			{
				RepoRoles.Logger.LogError("[Repo Roles] Failed to instantiate Mana UI.");
				return;
			}

			val2.name = "Mana";
			val2.transform.localPosition -= new Vector3(0f, 33f, 0f);

			// Remove EnergyUI if present
			var energyUI = val2.GetComponent<EnergyUI>();
			if (energyUI != null)
				Object.DestroyImmediate(energyUI);

			// Color the text
			foreach (var tmp in val2.GetComponentsInChildren<TextMeshProUGUI>(true))
			{
				if (tmp != null)
					tmp.color = new Color(0f, 0.384f, 1f); // Blue tone
			}

			// Set mana icon
			Texture2D manaTexture = guiManager.manaTexture;
			if (manaTexture != null)
			{
				var sprite = Sprite.Create(manaTexture, new Rect(0, 0, manaTexture.width, manaTexture.height), new Vector2(0.5f, 0.5f));
				var image = val2.GetComponentInChildren<Image>(true);
				if (image != null)
				{
					image.sprite = sprite;
					image.color = Color.white;
				}
				else
				{
					RepoRoles.Logger.LogWarning("[Repo Roles] Mana UI Image component not found.");
				}
			}
			else
			{
				RepoRoles.Logger.LogWarning("[Repo Roles] guiManager.manaTexture is null!");
			}

			// Add ManaUI
			manaUI = val2.AddComponent<ManaUI>();
			manaUI.SetMana(8f, 8f);
		}
	}
}