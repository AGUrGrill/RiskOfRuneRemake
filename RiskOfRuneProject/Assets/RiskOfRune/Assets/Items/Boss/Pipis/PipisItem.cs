using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PipisItem
{
    private static readonly float baseMulti = 0.05f;
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
        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.pipis);

        if (itemCount > 0)
        {
            var multi = baseMulti * itemCount;
            args.armorAdd += multi;
            args.healthMultAdd += multi;
            args.moveSpeedMultAdd += multi;
            args.regenMultAdd += multi;
            args.attackSpeedMultAdd += multi;
            args.damageMultAdd += multi;
            args.critDamageMultAdd += multi;
            args.critAdd += multi;
        }
    }
}