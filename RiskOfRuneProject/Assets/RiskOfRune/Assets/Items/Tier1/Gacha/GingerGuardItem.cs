using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GingerGuardItem
{
    public static readonly float GingerGuardArmorMult = 0.05f;

    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.gingerGuard);
        if (itemCount > 0)
        {
            args.armorTotalMult += GingerGuardArmorMult * itemCount;
        }
    }

}
    