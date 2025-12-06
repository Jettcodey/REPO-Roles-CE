using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Repo_Roles;
using UnityEngine;

namespace R.E.P.O.Roles.patches
{
	[HarmonyPatch(typeof(ShopManager))]
	public static class ShopManagerPatch
	{
		[HarmonyPatch(nameof(ShopManager.GetAllItemsFromStatsManager))]
		[HarmonyPostfix]
		private static void GetAllItemsFromStatsManagerPostfix(ShopManager __instance)
		{
			try
			{
				// Only run on master client in multiplayer
				if (SemiFunc.IsMultiplayer() && !SemiFunc.IsMasterClientOrSingleplayer())
				{
					return;
				}

				RepoRoles.Logger.LogInfo($"ShopManagerPatch: Filtering upgrades (Master: {SemiFunc.IsMasterClientOrSingleplayer()})");

				// Get all Upgrades that are not ours.
				List<Item> allValidUpgrades = GetAllValidUpgrades(__instance);

				// Replace disabled upgrades in potentialItemUpgrades.
				FilterAndMaintainUpgradeCount(__instance.potentialItemUpgrades, allValidUpgrades);

				// Remove disabled upgrades from potentialItems.
				__instance.potentialItems.RemoveAll(item =>
					item != null &&
					item.itemName != null &&
					IsDisabledUpgrade(item));
			}
			catch (System.Exception e)
			{
				RepoRoles.Logger.LogError($"Error in ShopManager patch: {e}");
			}
		}

		private static List<Item> GetAllValidUpgrades(ShopManager shopManager)
		{
			List<Item> validUpgrades = new List<Item>();

			// Check potential items for Upgrades.
			foreach (var item in shopManager.potentialItems.Concat(shopManager.potentialItemUpgrades))
			{
				if (item == null || item.itemName == null) continue;

				if (item.itemType != SemiFunc.itemType.item_upgrade) continue;

				if (IsDisabledUpgrade(item)) continue;

				validUpgrades.Add(item);
			}

			return validUpgrades.Distinct().ToList(); // Try removing duplicates.
		}

		private static void FilterAndMaintainUpgradeCount(List<Item> upgradeList, List<Item> validUpgradesPool)
		{
			if (upgradeList == null) return;

			List<Item> filteredList = new List<Item>();

			foreach (var item in upgradeList)
			{
				if (item == null || item.itemName == null)
				{
					filteredList.Add(item);
					continue;
				}

				// Check if disabled Upgrade.
				if (IsDisabledUpgrade(item))
				{
					// Replace with random Upgrade.
					if (validUpgradesPool.Count > 0)
					{
						int randomIndex = Random.Range(0, validUpgradesPool.Count);
						filteredList.Add(validUpgradesPool[randomIndex]);
						RepoRoles.Logger.LogInfo($"Replaced {item.itemName} with {validUpgradesPool[randomIndex].itemName}");
					}

				}
				else
				{
					filteredList.Add(item);
				}
			}

			// Update the list.
			upgradeList.Clear();
			upgradeList.AddRange(filteredList);
		}

		private static bool IsDisabledUpgrade(Item item)
		{
			if (item == null || item.itemName == null) return false;

			bool isMageUpgrade = item.itemName.Contains("Mana Regeneration Upgrade");
			bool isScoutUpgrade = item.itemName.Contains("Scout Cooldown Upgrade");

			if (!isMageUpgrade && !isScoutUpgrade) return false;

			// Check if role upgrades are completely disabled.
			if (!RepoRoles.enableRoleUpgrades.Value)
			{
				return true;
			}

			// Check specific roles.
			if (isMageUpgrade && !RepoRoles.enableMage.Value)
			{
				return true;
			}

			if (isScoutUpgrade && !RepoRoles.enableScout.Value)
			{
				return true;
			}

			return false;
		}
	}
}