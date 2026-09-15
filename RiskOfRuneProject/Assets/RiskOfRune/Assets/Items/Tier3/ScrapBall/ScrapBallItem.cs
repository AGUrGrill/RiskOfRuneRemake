using R2API;
using RiskOfRune;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScrapBallItem
{
    private const float StatMulti = 0.05f;
    private static List<ItemDef> validItems = new List<ItemDef>();
    public static void Init()
    {
        GetValidConsumedItems();
        Hooks();
    }

    public static void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.scrapBall);
        if (itemCount > 0)
        {
            int amountOfBrokenItems = 0;
            foreach (ItemDef item in validItems)
            {
                amountOfBrokenItems += sender.inventory.GetItemCountEffective(item);
            }
            args.damageTotalMult += amountOfBrokenItems * StatMulti;
            args.attackSpeedMultAdd += amountOfBrokenItems * StatMulti;
        }
    }

    private static void GetValidConsumedItems()
    {
        List<string> validRoR2Items = new List<string>();
        validRoR2Items.Add("HealingPotionConsumed");
        validRoR2Items.Add("TonicAffliction");
        validRoR2Items.Add("TeleportOnLowHealthConsumed");
        validRoR2Items.Add("FragileDamageBonusConsumed");
        validRoR2Items.Add("ExtraLifeVoidConsumed");
        validRoR2Items.Add("ExtraLifeConsumed");
        validRoR2Items.Add("LowerPricedChestsConsumed");
        validRoR2Items.Add("RegeneratingScrapConsumed");
        foreach (ItemIndex item in RoR2.ItemCatalog.allItems)
        {
            ItemDef itemDef = RoR2.ItemCatalog.GetItemDef(item);
            foreach (String itemName in validRoR2Items)
            {
                if (itemName.Equals(itemDef.name))
                {
                    validItems.Add(itemDef);
                }
            }
        }
        validItems.Add(RiskOfRuneContent.tvDinnerConsumed);
        validItems.Add(RiskOfRuneContent.execBuffetConsumed);
    }
}
