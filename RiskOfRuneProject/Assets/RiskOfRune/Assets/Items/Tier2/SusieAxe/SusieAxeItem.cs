using R2API;
using RiskOfRune;
using RoR2;
using RoR2.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System;

public class SusieAxeItem
{
    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients; ;
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.susieAxe);
        var existing = sender.GetComponent<PrimarySkillSusieAxeBehavior>();
        if (!existing && sender.inventory && itemCount > 0)
        {
            existing = sender.gameObject.AddComponent<PrimarySkillSusieAxeBehavior>();
            existing.body = sender;
            existing.stack = itemCount;
            existing.projectilePrefab = RiskOfRuneContent.susieAxeProjectilePrefab;
            existing.SusieAxeBuff = RiskOfRuneContent.susieAxeBuff;
        }
        else if (existing && itemCount <= 0) existing.enabled = false;
        else if (existing && itemCount > 0 && !existing.enabled) existing.enabled = true;
        if (existing) existing.stack = itemCount;
    }


    [RequireComponent(typeof(TeamFilter))]
    public class PrimarySkillSusieAxeBehavior : CharacterBody.ItemBehavior
    {
        public const int numShurikensBase = 2;
        public float damageCoefficientBase = 6f;
        public float damageCoefficientPerStack = 2f;
        public const float force = 4f;
        public float reloadTime = 5f;
        public float reloadTimer;
        public float healPercent = 0.05f;

        public SkillLocator skillLocator;
        public GameObject projectilePrefab;
        public GameObject susieProjPrefab;
        public BuffDef SusieAxeBuff;
        public InputBankTest inputBank;

        private void Awake()
        {
            enabled = false;
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
                    while (body.HasBuff(SusieAxeBuff) && num > 0)
                    {
                        num--;
                        body.RemoveBuff(SusieAxeBuff);
                    }
                }
            }
            inputBank = null;
            skillLocator = null;
        }
        private void OnSkillActivated(GenericSkill skill)
        {
            if (!NetworkServer.active) return;

            SkillLocator skillLocator = this.skillLocator;
            if (((skillLocator != null ? skillLocator.primary : null) == skill || (skillLocator != null ? skillLocator.secondary : null) == skill) && body.GetBuffCount(SusieAxeBuff) > 0)
            {
                FireSusieAxe();
                //RoR2.Util.PlaySound("Play_rude_buster", gameObject);
                //RpcPlaySusieSound();
                //EffectManager.SpawnEffect(SusieAxeEffectPrefab, new EffectData { origin = transform.position, scale = 1f }, true);
                //EffectManager.SimpleSoundEffect(RudeBusterSFX.index, body.corePosition, true);
                body.RemoveBuff(SusieAxeBuff);

                float healCalc = body.maxHealth * healPercent;
                //Debug.Log(body.maxHealth + " | Heal: " + healCalc);
                if ((body.healthComponent.health + healCalc) > body.maxHealth)
                    body.healthComponent.health = body.maxHealth;
                else
                    body.healthComponent.health += healCalc;
            }
        }
        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            int numOfShurikens = numShurikensBase;
            if (body.GetBuffCount(SusieAxeBuff) < numOfShurikens)
            {
                float reloadNum = reloadTime;
                reloadTimer += Time.fixedDeltaTime;
                while (reloadTimer > reloadNum && body.GetBuffCount(SusieAxeBuff) < numOfShurikens)
                {
                    body.AddBuff(SusieAxeBuff);
                    reloadTimer -= reloadNum;
                }
            }
        }

        private void FireSusieAxe()
        {
            Ray aimRay = GetAimRay();
            float dmgCalc = body.damage * (damageCoefficientBase + (damageCoefficientPerStack * (stack - 1)));
            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
            {
                projectilePrefab = projectilePrefab,
                position = aimRay.origin,
                rotation = RoR2.Util.QuaternionSafeLookRotation(aimRay.direction),
                owner = gameObject,
                damage = dmgCalc,
                force = force,
                crit = RoR2.Util.CheckRoll(body.crit, body.master),
                damageColorIndex = DamageColorIndex.Item,
                comboNumber = 3,
            });
        }
        private Ray GetAimRay()
        {
            return new Ray(body.inputBank.aimOrigin, body.inputBank.aimDirection);
        }
    }
}
