using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LancerCardItem
{
    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
    }

    private static void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
    {
        orig(self, body);

        if (!NetworkServer.active || body == null || body.inventory == null || RiskOfRuneContent.lancerCard == null) return;

        string currentScene = SceneCatalog.GetSceneDefForCurrentScene()?.cachedName;
        if (currentScene == "bazaar") return;

        int itemCount = body.inventory.GetItemCountEffective(RiskOfRuneContent.lancerCard);
        if (itemCount > 0)
        {
            for (int i = 0; i < itemCount; i++)
            {
                if (i == 0 || i % 2 == 0)
                {
                    body.AddBuff(DLC2Content.Buffs.FreeUnlocks.buffIndex);
                }
                Debug.Log($"Added lancer unlock effect to {body.name}");
            }
        }
    }
}