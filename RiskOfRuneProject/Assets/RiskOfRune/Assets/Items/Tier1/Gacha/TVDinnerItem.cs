using R2API;
using RewiredConsts;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TVDinnerItem
{
    public static readonly float healAmount = 100f;

    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        On.RoR2.CharacterBody.OnTakeDamageServer += CharacterBody_OnTakeDamageServer;
    }

    private static void CharacterBody_OnTakeDamageServer(On.RoR2.CharacterBody.orig_OnTakeDamageServer orig, CharacterBody self, DamageReport damageReport)
    {
        orig(self, damageReport);

        var playerHealth = self.healthComponent.health;
        var playerMaxHealth = self.maxHealth;
        var healthCalculation = playerHealth + healAmount;
        var wontOverheal = healthCalculation <= playerMaxHealth;
        var activationThreshold = playerMaxHealth * 0.4;
        int itemCount = self.inventory.GetItemCountEffective(RiskOfRuneContent.tvDinner);

        if (itemCount > 0 && playerHealth <= activationThreshold)
        {
            //Debug.Log("Stats:" +
            //    "\n" + playerHealth +
            //    "\n" + playerMaxHealth +
            //    "\n" + healthCalculation +
            //    "\n" + wontOverheal +
            //    "\n" + activationThreshold);
            if (wontOverheal)
            {
                self.healthComponent.health += healAmount;
            }
            else
            {
                self.healthComponent.health = playerMaxHealth;
            }
            self.AddTimedBuff(RoR2Content.Buffs.Immune, 0.5f);
            self.inventory.RemoveItemPermanent(RiskOfRuneContent.tvDinner);
            self.inventory.GiveItemPermanent(RiskOfRuneContent.tvDinnerConsumed);
            CharacterMasterNotificationQueue.SendTransformNotification(self.master, RiskOfRuneContent.tvDinner.itemIndex, RiskOfRuneContent.tvDinnerConsumed.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
            Debug.Log(self.name + " healed!");
        }
    }

}
    