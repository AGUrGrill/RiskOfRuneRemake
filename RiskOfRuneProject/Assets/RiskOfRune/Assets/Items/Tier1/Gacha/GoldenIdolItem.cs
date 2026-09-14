using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GoldenIdolItem
{
    public static readonly float xpGainMult = 0.2f;

    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        On.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
    }

    private static void DeathRewards_OnKilledServer(On.RoR2.DeathRewards.orig_OnKilledServer orig, DeathRewards self, DamageReport damageReport)
    {
        var player = damageReport.attackerBody;
        var xp = self.expReward;

        int itemCount = player.inventory.GetItemCountEffective(RiskOfRuneContent.goldenIdol);
        if (itemCount > 0)
        {
            //Debug.Log("XP Bonus Added: " + xp);
            var mult = xpGainMult * itemCount;
            uint bonus = (uint)Mathf.CeilToInt(xp * mult);
            self.expReward += bonus;
            //Debug.Log(" -> " + self.expReward);
        }

        orig(self, damageReport);
    }

}
    