using BepInEx.Configuration;
using DeltaruneMod.Elite;
using DeltaruneMod.Items;
using DeltaruneMod.Util;
using R2API;
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
    private List<BuffDef> allBuffs = new List<BuffDef>();
    private List<BuffDef> currBuffs = new List<BuffDef>();
    private List<BuffDef> blacklistedBuffs = new List<BuffDef>();

    public override void Init()
    {
        CreateLang();
        CreateBuff("N.E.O. Armor");
        CreateEquip("Armor of N.E.O.");
        CreateElite();
        AddContent();
        AddRamp();
        AddCrown();
        Hooks();
    }

    public static void Hooks()
    {
        On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
        On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        On.RoR2.CombatDirector.Init += CombatDirector_Init;
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
    }

    private static RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        #region Add Stat Buffs
        if (sender.name.Contains("Brother")) return;
        if (sender.HasBuff(EliteBuff))
        {
            args.armorTotalMult += EliteHealthMult;
            args.healthTotalMult += EliteHealthMult;
            args.damageTotalMult += EliteDamageMult;
        }
        #endregion
    }

    private void CombatDirector_Init(On.RoR2.CombatDirector.orig_Init orig)
    {
        orig();
        if (EliteAPI.VanillaEliteTiers.Length > 2)
        {
            CombatDirector.EliteTierDef targetTier = EliteAPI.VanillaEliteTiers[1];
            List<EliteDef> elites = targetTier.eliteTypes.ToList();
            EliteDefinition.healthBoostCoefficient = EliteHealthMult * 2;
            EliteDefinition.damageBoostCoefficient = EliteDamageMult * 2;
            elites.Add(EliteDefinition);
            targetTier.eliteTypes = elites.ToArray();
        }
        else if (EliteAPI.VanillaEliteTiers.Length > 1)
        {
            CombatDirector.EliteTierDef targetTier = EliteAPI.VanillaEliteTiers[0];
            List<EliteDef> elites = targetTier.eliteTypes.ToList();
            EliteDefinition.healthBoostCoefficient = EliteHealthMult;
            EliteDefinition.damageBoostCoefficient = EliteDamageMult;
            elites.Add(EliteDefinition);
            targetTier.eliteTypes = elites.ToArray();
        }
        //AddElite();
    }

    private void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
    {
        orig(self, buffDef);

        if (buffDef != EliteBuff) return;
        if (self.name.Contains("Brother")) return;

        if (allBuffs.Count <= 0) allBuffs = Helpers.GetBuffs(0);
        if (blacklistedBuffs.Count <= 0) BlacklistBuffs();

        if (self.inventory.GetItemCount(EliteItem) <= 0) self.inventory.GiveItem(EliteItem);
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

    private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
    {
        orig(self, buffDef);

        if (buffDef != EliteBuff) return;
        if (self.name.Contains("Brother")) return;

        if (self.inventory.GetItemCount(EliteItem) > 0 && self.inventory.GetEquipment(self.inventory.activeEquipmentSlot).equipmentDef != EliteEquip) self.inventory.RemoveItem(EliteItem);
        for (int i = 0; i < currBuffs.Count; i++)
        {
            self.RemoveBuff(currBuffs[i]);
        }
        currBuffs.Clear();
    }

    private void BlacklistBuffs()
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

    public void WhitelistedBuffs()
    {

    }

    public override ItemDisplayRuleDict CreateItemDisplayRules()
    {
        ItemDisplayRuleDict itemDisplayRules = new ItemDisplayRuleDict(Array.Empty<ItemDisplayRule>());
        itemDisplayRules.Add("mdlCommandoDualies", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.00058F, 0.33511F, -0.18017F),
                localAngles = new Vector3(326.6749F, 180.4597F, 359.3954F),
                localScale = new Vector3(10F, 10F, 10F)
            }
        });
        itemDisplayRules.Add("mdlHuntress", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0.07349F, 0.14217F, -0.12329F),
                localAngles = new Vector3(6.65519F, 168.3313F, 6.81422F),
                localScale = new Vector3(10F, 10F, 10F)

            }
        });
        itemDisplayRules.Add("mdlToolbot", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Stomach",
                localPos = new Vector3(-0.08547F, 2.30658F, -1.75874F),
                localAngles = new Vector3(358.3483F, 181.1543F, 358.8535F),
                localScale = new Vector3(46.42787F, 46.42787F, 46.42787F)
            }
        });
        itemDisplayRules.Add("mdlEngi", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Stomach",
                localPos = new Vector3(0.00074F, 0.31094F, -0.31571F),
                localAngles = new Vector3(1.20301F, 174.343F, 0.30152F),
                localScale = new Vector3(10F, 10F, 10F)
            }
        });
        itemDisplayRules.Add("mdlMage", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "ClavicleL",
                localPos = new Vector3(-0.32179F, 0.2478F, 0.08986F),
                localAngles = new Vector3(4.08904F, 253.4561F, 123.4712F),
                localScale = new Vector3(6.62753F, 8.03827F, 8.03827F)
            }
        });
        itemDisplayRules.Add("mdlMerc", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0.00153F, 0.18051F, -0.27087F),
                localAngles = new Vector3(358.6591F, 176.6325F, 358.7088F),
                localScale = new Vector3(7.64071F, 6.72569F, 5.55499F)
            }
        });
        itemDisplayRules.Add("mdlLoader", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0.00244F, 0.22424F, -0.28064F),
                localAngles = new Vector3(349.6327F, 181.087F, 359.4709F),
                localScale = new Vector3(10F, 10.25618F, 10F)

            }
        });
        itemDisplayRules.Add("mdlCroco", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.0131F, 0.00966F, 4.53132F),
                localAngles = new Vector3(14.45012F, 8.1635F, 3.24897F),
                localScale = new Vector3(80.6307F, 83.54383F, 83.54383F)
            }
        });
        itemDisplayRules.Add("mdlCaptain", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0.00155F, 0.24961F, -0.22495F),
                localAngles = new Vector3(349.8751F, 187.2755F, 0.6663F),
                localScale = new Vector3(10F, 10F, 10F)

            }
        });
        itemDisplayRules.Add("mdlBandit2", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.00033F, 0.25393F, -0.17588F),
                localAngles = new Vector3(342.7904F, 175.8488F, 11.35585F),
                localScale = new Vector3(10F, 10F, 10F)
            }
        });
        itemDisplayRules.Add("mdlEquipmentDrone", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "HeadCenter",
                localPos = new Vector3(0.0f, 0.0f, 1.09378f),
                localAngles = new Vector3(90f, 0.0f, 0.0f),
                localScale = new Vector3(0.3f, 0.3f, 0.3f)
            }
        });
        itemDisplayRules.Add("mdlWarframeWisp(Clone)", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.00284f, 0.25323f, -0.07018f),
                localAngles = new Vector3(0.0f, 0.0f, 0.0f),
                localScale = new Vector3(0.1f, 0.1f, 0.1f)
            }
        });
        itemDisplayRules.Add("mdlVoidSurvivor", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0.0006F, 0.1003F, -0.22901F),
                localAngles = new Vector3(358.1881F, 186.6165F, 356.1105F),
                localScale = new Vector3(10F, 10F, 10F)
            }
        });
        itemDisplayRules.Add("mdlHeretic", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0.09251f, 0.05643f, -0.01722f),
                localAngles = new Vector3(0.0f, 0.0f, 0.0f),
                localScale = new Vector3(0.1f, 0.1f, 0.1f)
            }
        });
        itemDisplayRules.Add("mdlBeetle", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0.02057F, 0.006F, -0.71707F),
                localAngles = new Vector3(13.48025F, 186.1411F, 1.4122F),
                localScale = new Vector3(28.20171F, 28.20171F, 33.85242F)
            }
        });
        itemDisplayRules.Add("AcidLarva", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "BodyBase",
                localPos = new Vector3(-0.0989F, 3.82093F, -4.37353F),
                localAngles = new Vector3(336.5901F, 188.8735F, 357.3547F),
                localScale = new Vector3(61.60095F, 61.60095F, 61.60095F)

            }
        });
        itemDisplayRules.Add("mdlBeetleGuard", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0.00862F, -0.1821F, 1.84132F),
                localAngles = new Vector3(306.8644F, 358.9843F, 180.0889F),
                localScale = new Vector3(42.46078F, 42.46078F, 42.46078F)
            }
        });
        itemDisplayRules.Add("mdlBeetleQueen", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.06181F, 3.22716F, -0.82205F),
                localAngles = new Vector3(304.1082F, 181.5108F, 358.9578F),
                localScale = new Vector3(70F, 70F, 70F)
            }
        });
        itemDisplayRules.Add("mdlBell", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "ShieldR",
                localPos = new Vector3(-0.44927F, 0.08874F, 0F),
                localAngles = new Vector3(5.42251F, 267.864F, 179.8404F),
                localScale = new Vector3(58.96465F, 58.96465F, 58.96465F)
            }
        });
        itemDisplayRules.Add("mdlBison", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.01604F, 0.30115F, 0.53763F),
                localAngles = new Vector3(345.9094F, 356.8964F, 181.6575F),
                localScale = new Vector3(15.67174F, 15.67174F, 15.67174F)
            }
        });
        itemDisplayRules.Add("mdlBrother", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "chest",
                localPos = new Vector3(0.01628F, 0.31178F, -0.15961F),
                localAngles = new Vector3(345.4924F, 182.5103F, 4.21038F),
                localScale = new Vector3(9.33332F, 9.33332F, 9.33332F)
            }
        });
        itemDisplayRules.Add("mdlClayBoss", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0F, -0.27929F, 1.95548F),
                localAngles = new Vector3(38.76653F, 1.51847F, 359.4286F),
                localScale = new Vector3(18.6769F, 18.95713F, 18.61193F)
            }
        });
        itemDisplayRules.Add("mdlClayBruiser", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0.12396F, 0.21655F, 0.28928F),
                localAngles = new Vector3(324.6672F, 33.4544F, 359.3883F),
                localScale = new Vector3(10.85176F, 10.85176F, 12.97967F)
            }
        });
        itemDisplayRules.Add("mdlClayGrenadier", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0.007F, 0.15244F, 0.00293F),
                localAngles = new Vector3(281.7109F, 189.9032F, 175.2612F),
                localScale = new Vector3(3.23962F, 3.23962F, 3.23962F)
            }
        });
        itemDisplayRules.Add("mdlMagmaWorm", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "HeadCenter",
                localPos = new Vector3(0.12703F, 0.89512F, -0.48463F),
                localAngles = new Vector3(278.4135F, 5.07096F, 354.8828F),
                localScale = new Vector3(54.20102F, 54.20102F, 54.20102F)

            }
        });
        itemDisplayRules.Add("mdlFlyingVermin", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Body",
                localPos = new Vector3(0.06809F, 0.86195F, 0.79218F),
                localAngles = new Vector3(318.7236F, 6.56401F, 1.04516F),
                localScale = new Vector3(28.54833F, 28.54833F, 28.54833F)
            }
        });
        itemDisplayRules.Add("mdlGolem", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0F, 0.55217F, 0.33971F),
                localAngles = new Vector3(3.49989F, 359.9193F, 359.2412F),
                localScale = new Vector3(32.02202F, 32.02202F, 32.02202F)
            }
        });
        itemDisplayRules.Add("mdlGrandparent", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "ShoulderL",
            localPos = new Vector3(-1.98726F, -1.30368F, 3.42324F),
            localAngles = new Vector3(5.7245F, 5.53087F, 257.7782F),
            localScale = new Vector3(70F, 70F, 70F)
            }
        });
        itemDisplayRules.Add("mdlGravekeeper", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Mask",
            localPos = new Vector3(0.05089F, -0.02209F, 1.68318F),
            localAngles = new Vector3(289.1578F, 7.36991F, 171.6874F),
            localScale = new Vector3(50F, 50F, 50F)
            }
        });
        itemDisplayRules.Add("mdlGreaterWisp", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "MaskBase",
            localPos = new Vector3(0.00163F, 0.61108F, 0.72091F),
            localAngles = new Vector3(343.268F, 4.07602F, 358.027F),
            localScale = new Vector3(17.35983F, 17.35983F, 17.35983F)
            }
        });
        itemDisplayRules.Add("mdlGup", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0.00886F, 0.06152F, 0.96569F),
                localAngles = new Vector3(341.9794F, 0F, 0F),
                localScale = new Vector3(16.69731F, 16.69731F, 16.69731F)
            }
        });
        itemDisplayRules.Add("mdlHermitCrab", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Base",
                localPos = new Vector3(-0.37484F, 0.54274F, 0.38064F),
                localAngles = new Vector3(319.7311F, 324.6842F, 357.4469F),
                localScale = new Vector3(23.18918F, 23.18918F, 23.18918F)
            }
        });
        itemDisplayRules.Add("mdlImp", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Neck",
                localPos = new Vector3(0F, 0.00893F, -0.09525F),
                localAngles = new Vector3(310.4503F, 177.8593F, 1.67071F),
                localScale = new Vector3(7.05008F, 7.05008F, 7.05008F)

            }
        });
        itemDisplayRules.Add("mdlImpBoss", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.01053F, 1.43164F, 0.98724F),
                localAngles = new Vector3(316.8173F, 4.54596F, 357.7444F),
                localScale = new Vector3(28.00615F, 28.00615F, 25.84916F)
            }
        });
        itemDisplayRules.Add("mdlJellyfish", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Hull2",
                localPos = new Vector3(0.96473F, -0.06366F, -0.03354F),
                localAngles = new Vector3(18.9179F, 90.34024F, 18.9519F),
                localScale = new Vector3(28.6372F, 28.6372F, 20.74469F)
            }
        });
        itemDisplayRules.Add("mdlLemurian", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.01987F, -0.28066F, 1.65141F),
                localAngles = new Vector3(18.08499F, 359.5553F, 358.3803F),
                localScale = new Vector3(100F, 100F, 100F)
            }
        });
        itemDisplayRules.Add("mdlLemurianBruiser", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(0.19935F, -0.04377F, 2.10265F),
                localAngles = new Vector3(22.09149F, 4.47752F, 2.81782F),
                localScale = new Vector3(71.45258F, 71.45258F, 71.45258F)
            }
        });
        itemDisplayRules.Add("mdlMiniMushroom", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.09003F, -0.8401F, 0F),
                localAngles = new Vector3(28.43619F, 268.6809F, 359.5791F),
                localScale = new Vector3(30F, 30F, 30F)
            }
        });
        itemDisplayRules.Add("mdlMinorConstruct", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "CapTop",
                localPos = new Vector3(0.01175F, 0.41135F, 0.34326F),
                localAngles = new Vector3(331.2021F, 359.9883F, 0.3928F),
                localScale = new Vector3(30F, 30F, 30F)
            }
        });
        itemDisplayRules.Add("mdlNullifier", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.00382F, 1.26069F, -0.29088F),
                localAngles = new Vector3(281.7445F, 354.0934F, 186.2832F),
                localScale = new Vector3(50F, 50F, 50F)

            }
        });
        itemDisplayRules.Add("mdlParent", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-49.97789F, 147.3375F, -0.00828F),
                localAngles = new Vector3(278.9313F, 92.94257F, 356.7719F),
                localScale = new Vector3(1862.46F, 1862.46F, 1862.46F)
            }
        });
        itemDisplayRules.Add("mdlRoboBallBoss", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Center",
                localPos = new Vector3(-0.0022F, -0.00843F, -0.9975F),
                localAngles = new Vector3(359.9314F, 176.8787F, 359.5111F),
                localScale = new Vector3(7.15018F, 7.15018F, 7.15018F)
            }
        });
        itemDisplayRules.Add("mdlRoboBallMini", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Root",
                localPos = new Vector3(-0.0028F, 0.00151F, -0.99744F),
                localAngles = new Vector3(358.952F, 180.8416F, 357.78F),
                localScale = new Vector3(7.72069F, 7.72069F, 7.72069F)
            }
        });
        itemDisplayRules.Add("mdlScav", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Pelvis",
                localPos = new Vector3(0.0841F, 1.59196F, -9.41179F),
                localAngles = new Vector3(355.7691F, 172.1812F, 181.1057F),
                localScale = new Vector3(84.06979F, 84.06979F, 84.06979F)
            }
        });
        itemDisplayRules.Add("mdlTitan", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.22135F, 1.11012F, 2.28249F),
                localAngles = new Vector3(1.24073F, 358.9157F, 0.97727F),
                localScale = new Vector3(66.01145F, 66.01145F, 66.01145F)
            }
        });
        itemDisplayRules.Add("mdlVagrant", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(1.07629F, -0.13497F, 0.07125F),
                localAngles = new Vector3(337.4336F, 76.81135F, 357.3511F),
                localScale = new Vector3(24.93059F, 24.93059F, 24.93059F)

            }
        });
        itemDisplayRules.Add("mdlVermin", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0.00568F, 0.59603F, -0.54056F),
                localAngles = new Vector3(346.7992F, 178.7826F, 180.8927F),
                localScale = new Vector3(21.0526F, 21.0526F, 21.67332F)
            }
        });
        itemDisplayRules.Add("mdlVoidBarnacle", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0.32754F, -0.00013F, 0.0143F),
                localAngles = new Vector3(3.93725F, 93.2476F, 270.9907F),
                localScale = new Vector3(15.36972F, 15.36972F, 15.36972F)
            }
        });
        itemDisplayRules.Add("mdlVoidJailer", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.67372F, -0.00286F, 0.12471F),
                localAngles = new Vector3(359.7408F, 277.3988F, 90.07114F),
                localScale = new Vector3(7.30482F, 7.30482F, 7.30482F)
            }
        });
        itemDisplayRules.Add("mdlVoidMegaCrab", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "BodyBase",
                localPos = new Vector3(-0.0749F, 5.08541F, 5.47022F),
                localAngles = new Vector3(309.5852F, 2.14126F, 0.09454F),
                localScale = new Vector3(86.77286F, 86.77286F, 199.6023F)
            }
        });
        itemDisplayRules.Add("mdlVulture", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.07432F, 1.82829F, -0.62649F),
                localAngles = new Vector3(350.927F, 176.3641F, 182.4292F),
                localScale = new Vector3(64.23273F, 64.23273F, 64.23273F)
            }
        });
        itemDisplayRules.Add("mdlWisp1Mouth", new ItemDisplayRule[1]
        {
            new ItemDisplayRule()
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = this.EliteCrown,
                childName = "Head",
                localPos = new Vector3(0F, 0.41211F, 0.35317F),
                localAngles = new Vector3(292.7783F, 345.231F, 194.4288F),
                localScale = new Vector3(15.90872F, 15.90872F, 25.48447F)
            }
        });
        itemDisplayRules.Add("mdlTreebot", new ItemDisplayRule[]
        {
            new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = EliteCrown,
                childName = "FlowerBase",
                localPos = new Vector3(-0.34876F, -0.13709F, 0.27158F),
                localAngles = new Vector3(1.87692F, 267.5815F, 248.1591F),
                localScale = new Vector3(40.3113F, 40.3113F, 40.3113F)

            }
        });
        itemDisplayRules.Add("mdlChef", new ItemDisplayRule[]
        {
            new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.08292F, -0.40159F, 0.00578F),
                localAngles = new Vector3(301.4835F, 177.4607F, 252.1901F),
                localScale = new Vector3(16.43113F, 14.08382F, 14.08382F)

            }
        });
        itemDisplayRules.Add("mdlSeeker", new ItemDisplayRule[]
        {
            new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = EliteCrown,
                childName = "Chest",
                localPos = new Vector3(-0.08968F, 0.16127F, -0.3987F),
                localAngles = new Vector3(349.7654F, 12.93527F, 20.32264F),
                localScale = new Vector3(18.98264F, 16.87346F, 16.87346F)

            }
        });
        itemDisplayRules.Add("mdlFalseSon", new ItemDisplayRule[]
        {
            new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = EliteCrown,
                childName = "Head",
                localPos = new Vector3(-0.00613F, 0.42402F, 0.07981F),
                localAngles = new Vector3(351.1979F, 260.1143F, 336.6507F),
                localScale = new Vector3(18.54679F, 18.54679F, 18.54679F)

            }
        });
        return itemDisplayRules;
    }
    
}

