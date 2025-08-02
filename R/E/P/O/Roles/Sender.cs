using BepInEx.Logging;
using Repo_Roles;
using UnityEngine;
using HarmonyLib;

namespace R.E.P.O.Roles
{
    [HarmonyPatch(typeof(TruckScreenText))]
    internal static class Sender
    {
    	private static bool hasRanOnce;

    	public static ClassManager manager = new ClassManager();

    	[HarmonyPatch("ArrowPointAtGoalLogic")]
    	[HarmonyPrefix]
    	private static void PrefixMethod()
    	{
    		if (!hasRanOnce)
    		{
    			manager.assignRoleFromConfig(PlayerController.instance);
    			RepoRoles.Logger.LogInfo((object)"Successfully rolled role!");
    			hasRanOnce = true;
    		}
    	}

    	[HarmonyPatch("Start")]
    	[HarmonyPrefix]
    	private static void StartPrefix()
    	{
    		hasRanOnce = false;
    	}
    }
}
