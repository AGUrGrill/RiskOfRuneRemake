using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TennaBuckleItem
{
    public static void Init()
    {
        On.RoR2.CharacterMaster.GiveMoney += CharacterMaster_GiveMoney;
    }

    private static void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
    {
        if (!NetworkServer.active) return;

        var body = self.GetBody();

        if (!body) return;

        var itemCount = body.inventory.GetItemCountEffective(RiskOfRuneContent.tennaBuckle);

        if (self.inventory && itemCount > 0)
        {
            amount += (uint)Mathf.CeilToInt(amount * (0.1f + 0.05f * (itemCount - 1)));
        }

        orig(self, amount);
    }
}
