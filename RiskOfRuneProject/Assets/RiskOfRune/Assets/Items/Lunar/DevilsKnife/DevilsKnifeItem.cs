using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DevilsKnifeItem
{
    public static List<BuffDef> buffs = new List<BuffDef>();
    public static void Init()
    {
        Hooks();
    }

    public static void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
    }

    private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        // If no buffs in list, populate
        if (buffs.Count <= 0) buffs = Helpers.GetBuffs(99);

        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.devilsKnife);
        var chaosInflicted = sender.GetComponent<WorldRevolvingEffect>();
        if (itemCount > 0)
        {
            if (!chaosInflicted)
            {
                chaosInflicted = sender.gameObject.AddComponent<WorldRevolvingEffect>();
                chaosInflicted.itemStacks = itemCount;
                chaosInflicted.body = sender;
                chaosInflicted.enabled = true;
            }
            else if (chaosInflicted) chaosInflicted.itemStacks = itemCount;
            else if (!chaosInflicted.enabled) chaosInflicted.enabled = true;
        }
        else if (chaosInflicted && itemCount <= 0) chaosInflicted.enabled = false;
    }

    // Handles buff applications
    public class WorldRevolvingEffect : MonoBehaviour
    {
        private float timer = 0f;
        private float maxTime = 10f;
        public CharacterBody body;
        public int itemStacks = 0;

        private void Awake()
        {
            base.enabled = false;
        }
        private void OnEnable()
        {

        }
        // Timer for chaos (buff application)
        private void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0f)
            {
                ChaosChaos();
                timer = maxTime;
            }
        }
        // Find random buff user doesnt have and give it
        //
        // SEEKER ISSUE - When sojourn causes seeker to disappear, maybe make timer per person?
        public void ChaosChaos()
        {
            BuffDef randomBuff;
            while (true)
            {
                randomBuff = buffs[UnityEngine.Random.Range(0, buffs.Count)];
                if (!body.HasBuff(randomBuff)) break;
                Debug.Log(body.name + " already has buff " + randomBuff.name + "! Cycling to next buff...");
            }
            body.AddTimedBuff(randomBuff, 5 + (itemStacks * 5));
            Debug.Log("Added random buff:" + randomBuff.name + " to " + body.name);
        }
        private void OnDisable()
        {

        }
    }
}
