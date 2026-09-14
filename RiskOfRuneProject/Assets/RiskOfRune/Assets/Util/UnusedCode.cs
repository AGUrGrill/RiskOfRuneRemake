using System;
using System.Collections.Generic;
using System.Text;

internal class UnusedCode
{
    /*
    // Old Orb Effect
    private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
    {
        orig(self, damageReport);

        var body = damageReport.attackerBody;
        var enemyBody = damageReport.victimBody;
        if (!body || !body.isPlayerControlled) return;
        var itemCount = GetCount(body);

        if (GetCount(body) > 0)
        {
            CoreOrb orb = new CoreOrb();
            orb.target = body.mainHurtBox;
            orb.shieldValue = baseShield + additionalShield * (itemCount-1);
            OrbManager.instance.AddOrb(orb);
        }
    }
        
    public void CreateOrb()
    {
        OrbAPI.AddOrb<CoreOrb>();
    }
        
    public class CoreOrb : Orb
    {
        public float shieldValue;

        public bool scaleOrb = true;

        public float overrideDuration = 0.6f;

        public override void Begin()
        {
            if (target)
            {
                duration = overrideDuration;
                float scale = scaleOrb ? Mathf.Min(shieldValue / target.healthComponent.fullShield, 1f) : 1f;
                EffectData effectData = new EffectData
                {
                    scale = scale,
                    origin = origin,
                    genericFloat = duration
                };
                effectData.SetHurtBoxReference(target);
                EffectManager.SpawnEffect(OrbStorageUtility.Get("Prefabs/Effects/OrbEffects/SquidOrbEffect"), effectData, transmit: true);
            }
        }

        public override void OnArrival()
        {
            if (target)
            {
                var healthComp = target.healthComponent;
                if (healthComp)
                {
                    if (healthComp.shield + shieldValue <= shieldCap)
                        healthComp.shield += shieldValue;

                    // Force some kind of update to show shield maybe
                    healthComp.shield -= 1;
                    healthComp.shield += 1;
                    healthComp.health -= 1;
                    healthComp.health += 1;
                }
            }
        }
    }
        */
}
