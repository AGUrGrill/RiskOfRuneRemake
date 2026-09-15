using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThornRingItem
{
    public static bool healthAmputated = false;
    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        if (!NetworkServer.active) return;


        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.thornRing);

        if (sender.inventory && itemCount > 0)
        {
            float curseHPReduction = 0.3f + (0.1f * (itemCount - 1));

            // Cap HP reduction cause, well ya
            if (curseHPReduction >= 0.7f) curseHPReduction = 0.7f;

            // Force HP mult to be curse amount
            args.healthTotalMult *= (1 - curseHPReduction);

            // Convert comm ring to thorn ring if applicable
            int commRingItemCount = sender.inventory.GetItemCountPermanent(RiskOfRuneContent.commRing);
            if (commRingItemCount > 0)
            {
                for (int i = 0; i < commRingItemCount; i++)
                {
                    sender.inventory.RemoveItemPermanent(RiskOfRuneContent.commRing);
                    sender.inventory.GiveItemPermanent(RiskOfRuneContent.thornRing);
                }
            }
        }
    }

    private static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
    {
        orig(self, damageInfo, victim);

        if (!NetworkServer.active) return;

        var attacker = damageInfo.attacker;
        if (!attacker) return;
        var attackerBody = attacker.GetComponent<CharacterBody>();
        if (!attackerBody || !attackerBody.isPlayerControlled) return;
        var victimBody = victim.GetComponent<CharacterBody>();
        if (!victimBody || victimBody.isPlayerControlled) return;

        int itemCount = attackerBody.inventory.GetItemCountEffective(RiskOfRuneContent.thornRing);

        // Add debuff to enemy
        if (victimBody.name == "BrotherBody(Clone)" || victimBody.name == "ITBrotherBody(Clone)" || victimBody.name == "BrotherHurtBody(Clone)" || victimBody.name == "BrotherGlassBody(Clone)") return;
        if (attackerBody.isPlayerControlled && itemCount > 0)
        {
            for (int i = 0; i < itemCount; i++)
            {
                victimBody.AddBuff(RiskOfRuneContent.sanguineFrostbiteDebuff);
            }
        }
    }
}
