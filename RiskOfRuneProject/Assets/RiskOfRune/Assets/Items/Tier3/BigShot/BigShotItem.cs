using R2API;
using RiskOfRune;
using RoR2;
using RoR2.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class BigShotItem
{
    public static uint TotalStages = 0;
    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        On.RoR2.CharacterMaster.GiveMoney += CharacterMaster_GiveMoney;
        Stage.onStageStartGlobal += Stage_onStageStartGlobal;
    }

    private static void Stage_onStageStartGlobal(Stage obj)
    {
        if (!NetworkServer.active) return;

        if (obj.sceneDef.cachedName != "bazaar")
        {
            TotalStages += 1;
            //Debug.Log("Total Stages: " + TotalStages);
        }
    }

    private static void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
    {
        if (NetworkServer.active && self.GetBody())
        {
            var sender = self.GetBody();
            var itemCount = self.inventory.GetItemCountEffective(RiskOfRuneContent.bigShot);
            var existing = sender.GetComponent<BigShotBehavior>();

            if (sender.inventory && itemCount > 0)
            {
                //Debug.Log($"Amount | " + amount);
                uint bonus = (uint)Mathf.CeilToInt(amount * 0.3f);
                amount += bonus;
                //Debug.Log($"Adjusted Dealmaker Amount | " + amount);

                if (existing)
                {
                    existing.TotalGoldGained += amount;
                }
            }
        }
        orig(self, amount);
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        var itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.bigShot);
        var existing = sender.GetComponent<BigShotBehavior>();
        if (sender.inventory && itemCount > 0 && !existing)
        {
            existing = sender.gameObject.AddComponent<BigShotBehavior>();
            existing.body = sender;
            existing.stack = itemCount;
            existing.BigShotBuff = RiskOfRuneContent.bigShotBuff;
            existing.projectilePrefab = RiskOfRuneContent.bigShotProjectilePrefab;
        }
        else if (existing && itemCount <= 0) existing.enabled = false;
        else if (existing && itemCount > 0 && !existing.enabled) existing.enabled = true;
        if (existing) existing.stack = itemCount;
    }

    [RequireComponent(typeof(TeamFilter))]
    public class BigShotBehavior : CharacterBody.ItemBehavior
    {
        #region Variables
        public BuffDef BigShotBuff;
        public float reloadTimer;
        public SkillLocator skillLocator;
        public InputBankTest inputBank;

        public int BigShotThreshold = 10;
        public float DmgMult = 77.7f;
        public float StackDmgMult = 22.2f;
        public uint MaxBigShotStacks = 30;
        public float TotalDamageCalc;

        public float TotalGoldGained = 0;
        public int BaseCost = 25;

        private bool TimeForABigShot;

        public GameObject projectilePrefab;
        #endregion
        private void Awake()
        {
            base.enabled = false;
        }
        private void Start()
        {
        }
        private void OnEnable()
        {
            if (body)
            {
                body.onSkillActivatedServer += new Action<GenericSkill>(OnSkillActivated);
                skillLocator = body.GetComponent<SkillLocator>();
                inputBank = body.GetComponent<InputBankTest>();
            }
        }
        private void OnDisable()
        {
            if (body)
            {
                body.onSkillActivatedServer -= new Action<GenericSkill>(OnSkillActivated);
                if (NetworkServer.active)
                {
                    int num = 10000;
                    while (body.HasBuff(BigShotBuff) && num > 0)
                    {
                        num--;
                        body.RemoveBuff(BigShotBuff);
                    }
                }
            }
            inputBank = null;
            skillLocator = null;
        }
        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Normal);
            #region Gold/Buff/Dmg Calcs
            TotalDamageCalc = body.damage * (DmgMult + StackDmgMult * (stack - 1));
            var goldPerStack = Run.instance.GetDifficultyScaledCost(BaseCost);
            var maxGold = goldPerStack * MaxBigShotStacks;

            if (body.GetBuffCount(BigShotBuff) >= MaxBigShotStacks + 1) body.RemoveBuff(BigShotBuff); // Remove buff on higher than alloted stack
            if (TotalGoldGained >= maxGold) TotalGoldGained = maxGold;
            if (TotalGoldGained >= goldPerStack)
            {
                body.AddBuff(BigShotBuff);
                TotalGoldGained -= goldPerStack;
            } // Add buff and remove gold
            if (body.GetBuffCount(BigShotBuff) >= BigShotThreshold && !TimeForABigShot) TimeForABigShot = true; // If buff count is equal to threshold, allow big shot
            #endregion
        }
        private void OnSkillActivated(GenericSkill skill)
        {
            if (!NetworkServer.active) return;

            SkillLocator skillLocator = this.skillLocator;
            if ((skillLocator != null ? skillLocator.primary : null) == skill && TimeForABigShot)
            {
                ShootBigShot();
                //EffectManager.SimpleSoundEffect(BigShotSFX.index, body.corePosition, true);
                for (int i = 0; i < BigShotThreshold; i++)
                {
                    body.RemoveBuff(BigShotBuff);
                }
                TimeForABigShot = false;
            }
        }
        private void ShootBigShot()
        {
            if (NetworkServer.active)
            {
                Ray aimRay = GetAimRay();
                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                {
                    projectilePrefab = projectilePrefab,
                    position = aimRay.origin,
                    rotation = RoR2.Util.QuaternionSafeLookRotation(aimRay.direction),
                    owner = gameObject,
                    damage = TotalDamageCalc,
                    force = 4f,
                    crit = RoR2.Util.CheckRoll(body.crit, body.master),
                    damageColorIndex = DamageColorIndex.Default,
                });
            }
        }
        private Ray GetAimRay()
        {
            return new Ray(body.inputBank.aimOrigin, body.inputBank.aimDirection);
        }
    }
}
}
