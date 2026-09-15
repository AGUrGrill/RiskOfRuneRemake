using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.CharacterAI;

public class GasterMaskItem
{
    private static int CorruptConversionTime;
    private const int CorruptionBaseTime = 10;
    private const int CorruptionMultTime = 10;
    private static Dictionary<CharacterBody, float> lastTimeCloneSpawned = new Dictionary<CharacterBody, float>();
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
        //try
        //{

        // Null check vitals
        var attacker = damageInfo.attacker;
        if (!attacker) return;
        var attackerBody = attacker.GetComponent<CharacterBody>();
        if (!attackerBody) return;
        var victimBody = victim.GetComponent<CharacterBody>();
        if (!victimBody || victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical) || victimBody.isPlayerControlled) return;

        // if meets conditions, apply effect
        int itemCount = attackerBody.inventory.GetItemCountEffective(RiskOfRuneContent.gasterMask);
        if (NetworkServer.active && itemCount > 0)
        {
            var thresholdHP = 0.1f;

            // Add buff
            if (!victimBody.HasBuff(RiskOfRuneContent.gasterCorruptionDebuff)) 
                victimBody.AddBuff(RiskOfRuneContent.gasterCorruptionDebuff);

            // Convert if hp is low enough
            if (victimBody.healthComponent.health <= victimBody.maxHealth * thresholdHP)
            {
                float time = Time.time; // Make sure clone dosent spawn twice
                if (lastTimeCloneSpawned.TryGetValue(attackerBody, out float lastTime))
                {
                    if (time - lastTime < 0.05f) return;
                }
                lastTimeCloneSpawned[attackerBody] = time;

                // Get this guy outta here!!
                if (victimBody.name != "VoidInfestorBody(Clone)" && victimBody.name != "VoidInfestorBody"
                    && victimBody.master.name != "VoidInfestorMaster(Clone)" && victimBody.master.name != "VoidInfestorMaster")
                {
                    ConvertEnemy(victimBody, attackerBody);
                }
            }
        }
        //}
        //catch { Debug.Log("Please check gaster effect DeltaruneMod"); }
    }

    private static void ConvertEnemy(CharacterBody target, CharacterBody owner)
    {
        int itemCount = owner.inventory.GetItemCountEffective(RiskOfRuneContent.gasterMask);

        // Add check to know if target is marked for conversion already
        if (target.GetComponent<ThingyMaBobber>()) return;
        target.gameObject.AddComponent<ThingyMaBobber>();

        // Destroy old target and set models to not release on death
        var targetCopy = target;
        var modelLocator = target.modelLocator;
        if (modelLocator) modelLocator.dontReleaseModelOnDeath = true;
        target.healthComponent.Suicide();

        // Setup target copy
        targetCopy.rigidbody.velocity = Vector3.zero;
        targetCopy.master.teamIndex = TeamIndex.Void;
        targetCopy.teamComponent.teamIndex = TeamIndex.Void;
        //targetCopy.inventory.SetEquipmentIndex(DLC1Content.Elites.Void.eliteEquipmentDef.equipmentIndex, true);
        //targetCopy.AddBuff(DLC1Content.Buffs.EliteVoid);

        var ai = targetCopy.master.GetComponent<BaseAI>();
        if (ai)
        {
            ai.enemyAttention = 0f;
            ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();
        }

        // Create ally
        CorruptConversionTime = CorruptionBaseTime + (itemCount - 1) * CorruptionMultTime;
        CharacterBody voidAlly = RoR2.Util.TryToCreateGhost(targetCopy, owner, CorruptConversionTime);
        voidAlly.AddBuff(DLC1Content.Buffs.EliteVoid);

        // Stop corpse from spawning
        var voidAllyModelLocator = voidAlly.modelLocator;
        if (voidAllyModelLocator) voidAllyModelLocator.dontReleaseModelOnDeath = true;

        // Spawn effect on enemy
        EffectData effectData = new EffectData
        {
            scale = 1f
        };
        effectData.SetNetworkedObjectReference(voidAlly.gameObject);

        EffectManager.SpawnEffect(RiskOfRuneContent.gasterCorruptionPrefab, effectData, true);

        // Destroy old target copy
        targetCopy.healthComponent.Suicide();
    }

    public void SetupVoidItemConversion()
    {
        ItemDef happiestMask = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/GhostOnKill/GhostOnKill.asset").WaitForCompletion();

        var provider = ScriptableObject.CreateInstance<ItemRelationshipProvider>();
        provider.name = "HappiestMaskToMysterMansMaskConversion";
        provider.relationshipType = Addressables.LoadAssetAsync<ItemRelationshipType>("RoR2/DLC1/Common/ContagiousItem.asset").WaitForCompletion();
        var pair = new ItemDef.Pair
        {
            itemDef1 = happiestMask,
            itemDef2 = RiskOfRuneContent.gasterMask
        };
        provider.relationships = new ItemDef.Pair[] { pair };
        ContentAddition.AddItemRelationshipProvider(provider);
    }

    // Just makes sure the target isnt converted twice by tagging it (hopefully)
    public class ThingyMaBobber : MonoBehaviour
    {
        void Start()
        {
            //Debug.Log("bleh :P");
        }
    }
}
