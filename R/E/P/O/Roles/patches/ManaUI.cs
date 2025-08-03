using Repo_Roles;
using TMPro;
using UnityEngine;

namespace R.E.P.O.Roles.patches
{
	public class ManaUI : SemiUI
	{
		public TextMeshProUGUI textMana;
		public TextMeshProUGUI textManaMax;
		public static ManaUI instance;

		public override void Start()
		{
			base.Start();
			instance = this;

			// Find both UI elements safely
			textMana = GetComponentInChildren<TextMeshProUGUI>();

			var maxObj = transform.Find("EnergyMax");
			if (maxObj != null)
				textManaMax = maxObj.GetComponent<TextMeshProUGUI>();

			// Initial set from guiManager
			if (textMana != null)
				textMana.text = Mathf.Ceil(guiManager.aviableMana).ToString();
		}

		public void SetMana(float mana, float maxMana)
		{
			if (textMana == null || textManaMax == null)
			{
				// Try to recover references if not set
				textMana = GetComponentInChildren<TextMeshProUGUI>();

				var maxObj = transform.Find("EnergyMax");
				if (maxObj != null)
					textManaMax = maxObj.GetComponent<TextMeshProUGUI>();

				if (textMana == null || textManaMax == null)
				{
					RepoRoles.Logger.LogWarning("ManaUI.SetMana called but UI references are null.");
					return;
				}
			}

			textMana.text = Mathf.Ceil(mana).ToString();
			textManaMax.text = $"<b>/</b>{Mathf.Ceil(maxMana)}";
		}
	}
}