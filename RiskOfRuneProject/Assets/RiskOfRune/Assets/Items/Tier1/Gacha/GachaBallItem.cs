using R2API;
using RewiredConsts;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GachaBallItem
{
    public static List<ItemDef> gachaItems = new List<ItemDef>();

    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
    }

    private static void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
    {
        orig(self, body);

        int itemCount = self.inventory.GetItemCountEffective(RiskOfRuneContent.gachaBall);
        if (gachaItems.Count <= 0) GetGachaItems();

        // Global hook to always delete all gacha items on stage start
        try
        {
            foreach (var item in gachaItems)
            {
                var gachaItemCount = self.inventory.GetItemCountEffective(item);
                for (int i = 0; i < gachaItemCount; i++)
                {
                    self.inventory.RemoveItemPermanent(item);
                    Debug.Log("Removed " + item.name + " from " + self.name);
                }
            }
        }
        catch
        {
            Debug.Log("Error removing gacha items.");
        }

        // On stage start give random items
        if (itemCount > 0)
        {
            for (int i = 0; i < itemCount; i++)
            {
                var ranGachaItem = gachaItems[UnityEngine.Random.Range(0, gachaItems.Count)];
                self.inventory.GiveItemPermanent(ranGachaItem);
                Debug.Log("Gave " + self.name + " " + ranGachaItem.name);
            }
        }
    }

    private static void GetGachaItems()
    {
        gachaItems.Add(RiskOfRuneContent.tvDinner);
        gachaItems.Add(RiskOfRuneContent.execBuffet);
        gachaItems.Add(RiskOfRuneContent.goldenIdol);
        gachaItems.Add(RiskOfRuneContent.blueRibbion);
        gachaItems.Add(RiskOfRuneContent.gingerGuard);
    }
    private class GachaBallTimer : MonoBehaviour
    {
        // Temp items last 80sec, 90sec with substandard dup

        readonly float timerInterval = 90f;
        float timer = 0f;

        public int stackCount = 0;
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
        // Timer
        private void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                GAMBLE();
                timer = timerInterval;
            }
        }
        // Add buff to increase speed
        private void GAMBLE()
        {
            for (int i = 0; i < stackCount; i++)
            {
                var ranGachaItem = gachaItems[UnityEngine.Random.Range(0, gachaItems.Count)];
                player.inventory.GiveItemTemp(ranGachaItem.itemIndex);
                Debug.Log("Gave " + player.name + " " + ranGachaItem.name);
            }
        }
    }

}
    