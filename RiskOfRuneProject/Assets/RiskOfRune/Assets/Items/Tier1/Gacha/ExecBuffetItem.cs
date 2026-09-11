using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecBuffetItem
{
    private static readonly float healAmount = 100f;
    public static void Hooks()
    {
        On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
    }

    private static void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody body, DamageReport damageReport)
    {
        orig(body, damageReport);

        int itemCount = body.inventory.GetItemCountEffective(RiskOfRuneContent.lancerCard);
        if (itemCount <= 0) return;

        var holderHealth = body.healthComponent.health;
        var holderMaxHealth = body.maxHealth;
        var activationThreshold = holderMaxHealth * 0.4;

        if (holderHealth <= activationThreshold)
        {
            foreach (var characterBody in CharacterBody.readOnlyInstancesList)
            {
                if (characterBody.isPlayerControlled)
                {
                    var playerHealth = characterBody.healthComponent.health;
                    var playerMaxHealth = characterBody.maxHealth;
                    var healthCalculation = playerHealth + healAmount;
                    var wontOverheal = healthCalculation <= playerMaxHealth;
                    if (wontOverheal)
                    {
                        characterBody.healthComponent.health += healAmount;
                    }
                    else
                    {
                        characterBody.healthComponent.health = playerMaxHealth;
                    }
                    characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.Immune, 0.5f);
                    Debug.Log(characterBody.name + " healed!");
                }
                Debug.Log(characterBody + " found.");
            }
            body.inventory.RemoveItemPermanent(RiskOfRuneContent.execBuffet);
            body.inventory.GiveItemPermanent(RiskOfRuneContent.execBuffetConsumed);
            CharacterMasterNotificationQueue.SendTransformNotification(body.master, RiskOfRuneContent.execBuffet.itemIndex, RiskOfRuneContent.execBuffetConsumed.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
        }
    }

    public static void Init()
    {
        Hooks();
    }
}
