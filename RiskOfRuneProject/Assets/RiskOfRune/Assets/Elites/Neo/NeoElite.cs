using R2API;
using RiskOfRune;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

public class NeoElite
{
    private static int maxBuffs = 2;
    private static List<BuffDef> allBuffs = new List<BuffDef>();
    private static List<BuffDef> currBuffs = new List<BuffDef>();
    private static List<BuffDef> blacklistedBuffs = new List<BuffDef>();

    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        //On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
        //On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
    }

    private static void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
    {
        orig(self, buffDef);

        if (buffDef != RiskOfRuneContent.neoEliteBuff) return;
        if (self.name.Contains("Brother")) return;

        if (allBuffs.Count <= 0) allBuffs = Helpers.GetBuffs(0);
        if (blacklistedBuffs.Count <= 0) BlacklistBuffs();

        for (int i = 0; i < maxBuffs; i++)
        {
            BuffDef ranBuff = allBuffs[UnityEngine.Random.Range(0, allBuffs.Count)];
            //Debug.Log("Picked buff: " + ranBuff);
            foreach (BuffDef buff in blacklistedBuffs)
            {
                //Debug.Log("Blacklisted buffs: " + buff);
                // Fallback buff for problematic buffs
                if (ranBuff == buff)
                {
                    Debug.Log("Changed buff to: " + ranBuff);
                    ranBuff = RoR2Content.Buffs.TonicBuff;
                }
            }
            currBuffs.Add(ranBuff);
            self.AddBuff(ranBuff);
        }
    }

    private static void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
    {
        orig(self, buffDef);

        if (buffDef != RiskOfRuneContent.neoEliteBuff) return;
        if (self.name.Contains("Brother")) return;

        if (self.inventory.GetEquipment(self.inventory.activeEquipmentSlot).equipmentDef != RiskOfRuneContent.neoEquipment) 
            self.inventory.RemoveEquipment(RiskOfRuneContent.neoEquipment.equipmentIndex);
        for (int i = 0; i < currBuffs.Count; i++)
        {
            self.RemoveBuff(currBuffs[i]);
        }
        currBuffs.Clear();
    }

    private static void BlacklistBuffs()
    {
        blacklistedBuffs.Add(RoR2Content.Buffs.Immune);
        blacklistedBuffs.Add(DLC2Content.Buffs.HiddenRejectAllDamage);
        blacklistedBuffs.Add(RoR2Content.Buffs.HiddenInvincibility);
        blacklistedBuffs.Add(DLC2Content.Buffs.DisableAllSkills);
        blacklistedBuffs.Add(RoR2Content.Buffs.Intangible);
        blacklistedBuffs.Add(RoR2Content.Buffs.LunarShell);
        blacklistedBuffs.Add(DLC2Content.Buffs.SoulSurge);
        blacklistedBuffs.Add(RoR2Content.Buffs.ElephantArmorBoost);
        blacklistedBuffs.Add(DLC2Content.Buffs.KnockUpHitEnemies);
        blacklistedBuffs.Add(DLC2Content.Buffs.KnockUpHitEnemiesJuggleCount);
        blacklistedBuffs.Add(DLC2Content.Buffs.KnockBackUnavailable);
        blacklistedBuffs.Add(DLC2Content.Buffs.KnockBackActiveWindow);
        blacklistedBuffs.Add(DLC2Content.Buffs.SeekerAnimaBuff);
        blacklistedBuffs.Add(DLC1Content.Buffs.BearVoidCooldown);
        blacklistedBuffs.Add(DLC2Content.Buffs.StunAndPierceBuff);
        blacklistedBuffs.Add(RoR2Content.Buffs.CrocoRegen);
        blacklistedBuffs.Add(DLC2Content.Buffs.HealAndReviveRegenBuff);
        blacklistedBuffs.Add(DLC2Content.Buffs.AurelioniteBlessing);
        blacklistedBuffs.Add(DLC2Content.Buffs.EliteBeadCorruption);
        blacklistedBuffs.Add(RoR2Content.Buffs.EngiShield);
    }

    public static void WhitelistedBuffs()
    {

    }
}

