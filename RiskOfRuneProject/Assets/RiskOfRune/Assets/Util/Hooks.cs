using DeltaruneMod.Elites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class Hooks
{
    int numOfEnemiesSpawned = 0;
    public Hooks()
    {
        //On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
    }

    private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, RoR2.CharacterMaster self, RoR2.CharacterBody body)
    {
        orig(self, body);

        // Simulate NEO Elite
        if (DeltaruneMod.DeltarunePlugin.neoEliteDisable.Value) return;
        if (DeltarunePlugin.allEnemiesNEO.Value)
        {
            if (body.isPlayerControlled || body.isRemoteOp || body.IsDrone) return;

            numOfEnemiesSpawned++;
            body.inventory.SetEquipmentIndex(NeoElite.instance.EliteEquip.equipmentIndex, true);
        }    
    }
}

