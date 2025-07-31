using System.Collections.Generic;
using Repo_Roles;
using UnityEngine;

namespace R.E.P.O.Roles.patches;

public class Upgrader : MonoBehaviour
{
	private ItemToggle _itemToggle;

	private void Start()
	{
		_itemToggle = ((Component)this).GetComponent<ItemToggle>();
	}

	public static void UpdateStat(int amount, string steamId, string stat)
	{
		Dictionary<string, int> dictionary = StatsManager.instance.dictionaryOfDictionaries[stat];
		if (!dictionary.ContainsKey(steamId))
		{
			dictionary[steamId] = 0;
		}
		dictionary[steamId] += amount;
	}

	public static int GetStat(string steamId, string upgradeName)
	{
		Dictionary<string, int> dictionary = StatsManager.instance.dictionaryOfDictionaries[upgradeName];
		int value;
		return dictionary.TryGetValue(steamId, out value) ? value : 0;
	}

	public void upgrade()
	{
		string steamId = SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(_itemToggle.playerTogglePhotonID));
		RepoRoles.Logger.LogInfo((object)("Your Mana Regen before: " + GetStat(steamId, "playerUpgradeManaRegeneration")));
		UpdateStat(1, steamId, "playerUpgradeManaRegeneration");
		RepoRoles.Update_ManaRegeneration();
		RepoRoles.Logger.LogInfo((object)("Your Mana Regen after: " + GetStat(steamId, "playerUpgradeManaRegeneration")));
	}

	public void upgradeScoutCooldown()
	{
		string steamId = SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarGetFromPhotonID(_itemToggle.playerTogglePhotonID));
		RepoRoles.Logger.LogInfo((object)("Your Scout Cooldown Upgrades before: " + GetStat(steamId, "playerUpgradeScoutCooldownReduction")));
		UpdateStat(1, steamId, "playerUpgradeScoutCooldownReduction");
		RepoRoles.Update_ScoutCooldown();
		RepoRoles.Logger.LogInfo((object)("Your Scout Cooldown Upgrades after: " + GetStat(steamId, "playerUpgradeScoutCooldownReduction")));
	}
}