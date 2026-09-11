using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class BlueRibbonItem
{
    public static readonly float BlueRibbionHealingMult = 0.05f;
    public static readonly float BlueRibbionAttackSpeedMult = 0.05f;

    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
    {
        var itemCount = body.inventory.GetItemCountEffective(RiskOfRuneContent.tennaBuckle);
        if (itemCount > 0)
        {
            args.healthTotalMult += BlueRibbionHealingMult * itemCount;
            args.attackSpeedMultAdd += BlueRibbionAttackSpeedMult * itemCount;
        }
    }

}
    