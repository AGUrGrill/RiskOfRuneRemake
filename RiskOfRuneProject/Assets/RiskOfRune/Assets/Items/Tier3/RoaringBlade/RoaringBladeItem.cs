using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoaringBladeItem
{
    public static readonly int MaxSwoonStacks = 3;
    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
    }

    private static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
    {
        orig(self, damageInfo, victim);

        try
        {
            var attacker = damageInfo.attacker;
            if (!attacker) return;
            var sender = attacker.GetComponent<CharacterBody>();
            if (!sender) return;
            var victimBody = victim.GetComponent<CharacterBody>();
            if (!victimBody || victimBody.isPlayerControlled) return;

            if (NetworkServer.active && sender && victimBody)
            {
                var existing = victimBody.GetComponent<SwoonDamageTracker>();
                // Add Damage Tracker
                int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.roaringBlade);
                if (!existing && sender.inventory && itemCount > 0)
                {
                    existing = victimBody.gameObject.AddComponent<SwoonDamageTracker>();
                    existing.body = victimBody;
                    existing.stack = itemCount;
                }
                else if (existing && itemCount <= 0) existing.enabled = false;
                else if (existing && itemCount > 0 && !existing.enabled) existing.enabled = true;
                if (existing) existing.stack = itemCount;

                // Add Buff
                if (existing && itemCount > 0
                    && existing.canSwoon && victimBody.GetBuffCount(RiskOfRuneContent.swoonDebuff) <= MaxSwoonStacks)
                {
                    if (RoR2.Util.CheckRoll(50, sender.master))
                    {
                        victimBody.AddBuff(RiskOfRuneContent.swoonDebuff);
                    }
                }

                // Buff Stack 1: set prev health on first time
                if (existing && victimBody.GetBuffCount(RiskOfRuneContent.swoonDebuff) <= 1)
                {
                    existing.prevHealth = victimBody.healthComponent.health;
                }

                // Buff Stack 3: set curr health, do swoon
                if (existing && victimBody.GetBuffCount(RiskOfRuneContent.swoonDebuff) >= MaxSwoonStacks)
                {
                    existing.currHealth = victimBody.healthComponent.health;
                    existing.DoSwoonDamage();
                    for (int i = 0; i <= MaxSwoonStacks; i++)
                    {
                        victimBody.RemoveBuff(RiskOfRuneContent.swoonDebuff);
                    }
                }
            }

        }
        catch { Debug.Log("Please check swoon effect DeltaruneMod"); }
    }

    // Does the damage and effect spawn
    public class SwoonDamageTracker : CharacterBody.ItemBehavior
    {
        public float currHealth;
        public float prevHealth;
        public bool canSwoon;
        private float swoonTimer = 0f;
        private float swoonTimerInterval = 1f;

        private void Start()
        {
            canSwoon = true;
        }
        private void FixedUpdate()
        {
            // If cant swoon start timer to swoon again
            if (!canSwoon)
            {
                swoonTimer -= Time.fixedDeltaTime;
                if (swoonTimer <= 0f)
                {
                    canSwoon = true;
                    swoonTimer = swoonTimerInterval;
                }
            }
        }
        public void DoSwoonDamage()
        {
            if (!NetworkServer.active) return;

            // Deal dmg to target and if its gonna kill, leave at 1 hp (lore kinda + no money/xp otherwise)
            var totalDamageTaken = (prevHealth - currHealth) * (stack + 1);
            var projectedHP = body.healthComponent.health - totalDamageTaken;
            // Disabled cause of gaster issue, back to old system
            /*if (projectedHP <= 0)
            {
                body.healthComponent.health = 1;
            }
            else
            {
                body.healthComponent.health -= totalDamageTaken;
            }*/
            body.healthComponent.health -= totalDamageTaken;

            // Hopefully fix potential of multiple peoples swoon causing the roaring (the health bar to go crazy)
            if (body.healthComponent.health - totalDamageTaken > body.maxHealth)
            {
                ResetSwoonStats();
                return;
            }

            // Spawn effect anddound
            EffectManager.SpawnEffect(RiskOfRuneContent.swoonPrefab, new EffectData { origin = body.transform.position, scale = 1f }, true);
            //EffectManager.SimpleSoundEffect(RiskOfRuneContent.swoonSFX.index, body.corePosition, true);

            Debug.Log("Swooned " + body.name + " for " + totalDamageTaken + "!");
            Debug.Log("Prev HP " + prevHealth + " | Old Curr HP " + currHealth);

            canSwoon = false;
        }
        public void ResetSwoonStats()
        {
            prevHealth = 0;
            currHealth = 0;
        }
    }
}