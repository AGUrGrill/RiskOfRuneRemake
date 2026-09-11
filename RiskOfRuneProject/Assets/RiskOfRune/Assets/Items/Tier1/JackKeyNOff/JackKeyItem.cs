using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackKeyItem
{
    private static readonly float multi = 0.05f;
    private static readonly float baseMulti = 0.05f;
    public static void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
    }

    private static void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
    {
        orig(self);

        var itemCount = self.inventory.GetItemCountEffective(RiskOfRuneContent.jackKeyNOff);
        var timer = self.GetComponent<JackNOffTimer>();
        if (itemCount > 0 && !timer)
        {
            timer = self.gameObject.AddComponent<JackNOffTimer>();
            timer.player = self;
            timer.enabled = true;
        }
        else if (itemCount <= 0 && timer)
        {
            timer.enabled = false;
        }
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        var itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.jackKeyNOff);
        if (itemCount > 0 && sender.HasBuff(RiskOfRuneContent.jackBuff))
        {
            var buffCount = sender.GetBuffCount(RiskOfRuneContent.jackBuff);
            var modifiedItemCount = itemCount - 1;
            var totalSpeedMult = buffCount * (baseMulti + (modifiedItemCount * multi));
            args.moveSpeedMultAdd += totalSpeedMult;
        }
    }

    public static void Init()
    {
        Hooks();
    }

    private class JackNOffTimer : MonoBehaviour
    {
        readonly float timerInterval = 30f;
        float timer = 0f;

        public CharacterBody player;

        private void Awake()
        {
            base.enabled = false;
        }
        private void OnEnable()
        {
            if (!player)
            {
                Debug.Log("Player not found! Destroying...");
                Destroy(this);
            }

            timer = timerInterval;
        }
        // Jack Key N. Off Timer
        private void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                YourTakingTooLong();
                timer = timerInterval;
            }
        }
        // Add buff to increase speed
        private void YourTakingTooLong()
        {
            Debug.Log("Adding speed buff!");
            player.AddBuff(RiskOfRuneContent.jackBuff);
        }
    }
}

