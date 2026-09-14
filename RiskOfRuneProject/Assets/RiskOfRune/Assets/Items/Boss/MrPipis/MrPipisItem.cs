using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MrPipisItem
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
        if (!NetworkServer.active || !sender.inventory) return;

        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.mrPipis);
        var existing = sender.GetComponent<MrPipisTracker>();
        if (itemCount > 0)
        {
            if (!existing)
            {
                existing = sender.gameObject.AddComponent<MrPipisTracker>();
                existing.body = sender;
                existing.enabled = true;
            }
            if (!existing.enabled) existing.enabled = true;
        }
        else if (itemCount <= 0 && existing) existing.enabled = false;
    }

    public class MrPipisTracker : MonoBehaviour
    {
        List<BuffDef> allAffixes = Helpers.GetBuffs(2);
        public CharacterBody body;

        private void Awake()
        {
            base.enabled = false;
        }
        private void OnEnable()
        {

        }
        private void FixedUpdate()
        {
            foreach (var affix in allAffixes)
            {
                if (!body.HasBuff(affix) && affix != DLC2Content.Buffs.AurelioniteBlessing && affix != DLC2Content.Buffs.EliteAurelionite) body.AddBuff(affix);
            }
        }

        private void OnDisable()
        {
            foreach (var affix in allAffixes)
            {
                if (body.HasBuff(affix)) body.RemoveBuff(affix);
            }
        }
    }
}