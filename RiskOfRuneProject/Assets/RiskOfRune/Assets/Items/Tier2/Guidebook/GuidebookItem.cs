using R2API;
using RiskOfRune;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GuideBookItem
{
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
        if (!NetworkServer.active) return;

        int itemCount = sender.inventory.GetItemCountEffective(RiskOfRuneContent.guideBook);
        if (sender.inventory && itemCount > 0)
        {
            var existing = sender.GetComponent<GuideBookBehavior>();
            if (!existing && sender.inventory && itemCount > 0)
            {
                existing = sender.gameObject.AddComponent<GuideBookBehavior>();
                existing.body = sender;
                existing.stack = itemCount;
                //Debug.Log("Gave " + sender + " " + ItemName + ".");
            }
            else if (existing && itemCount <= 0) existing.enabled = false;
            else if (existing && itemCount > 0 && !existing.enabled) existing.enabled = true;
            if (existing) existing.stack = itemCount;
        }
    }
}

public class GuideBookBehavior : CharacterBody.ItemBehavior
{
    private int previousStack;

    private void OnEnable()
    {
        ulong num = Run.instance.seed ^ (ulong)((long)Run.instance.stageClearCount);
        UpdateAllMinions(stack);
        MasterSummon.onServerMasterSummonGlobal += OnServerMasterSummonGlobal;
    }

    private void OnDisable()
    {
        MasterSummon.onServerMasterSummonGlobal -= OnServerMasterSummonGlobal;
        UpdateAllMinions(0);
    }

    private void FixedUpdate()
    {
        if (previousStack != stack)
        {
            UpdateAllMinions(stack);
        }
    }

    private void OnServerMasterSummonGlobal(MasterSummon.MasterSummonReport summonReport)
    {
        if (body && body.master && body.master == summonReport.leaderMasterInstance)
        {
            CharacterMaster summonMasterInstance = summonReport.summonMasterInstance;
            if (summonMasterInstance)
            {
                CharacterBody body = summonMasterInstance.GetBody();
                if (body)
                {
                    UpdateMinionInventory(summonMasterInstance.inventory, body.bodyFlags, stack);
                }
            }
        }
    }

    private void UpdateAllMinions(int newStack)
    {
        if (!this.body) return;
        CharacterBody body = this.body;
        if ((body != null) ? body.master : null)
        {
            MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.FindGroup(body.master.netId);
            if (minionGroup != null)
            {
                foreach (MinionOwnership minionOwnership in minionGroup.members)
                {
                    if (minionOwnership)
                    {
                        CharacterMaster component = minionOwnership.GetComponent<CharacterMaster>();
                        if (component && component.inventory)
                        {
                            CharacterBody body2 = component.GetBody();
                            if (body2)
                            {
                                UpdateMinionInventory(component.inventory, body2.bodyFlags, newStack);
                            }
                        }
                    }
                }
                previousStack = newStack;
            }
        }
    }

    private void UpdateMinionInventory(Inventory inventory, CharacterBody.BodyFlags bodyFlags, int newStack)
    {
        if ((inventory && newStack > 0 && (bodyFlags & CharacterBody.BodyFlags.Mechanical) > CharacterBody.BodyFlags.None) || (bodyFlags & CharacterBody.BodyFlags.Devotion) > CharacterBody.BodyFlags.None)
        {
            if ((bodyFlags & CharacterBody.BodyFlags.Void) > CharacterBody.BodyFlags.None) return;

            int itemCount = inventory.GetItemCountPermanent(RiskOfRuneContent.droneHealBoost);
            if (itemCount < stack)
            {
                inventory.GiveItemPermanent(RiskOfRuneContent.droneHealBoost, stack - itemCount);
            }
            else if (itemCount > stack)
            {
                inventory.RemoveItemPermanent(RiskOfRuneContent.droneHealBoost, itemCount - stack);
            }
        }
        else
        {
            inventory.ResetItemPermanent(RiskOfRuneContent.droneHealBoost);
        }
    }
}
