using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RiskOfRune
{
    public class RiskOfRuneContent : IContentPackProvider
    {
        public string identifier => RiskOfRuneMain.GUID;

        public static ReadOnlyContentPack readOnlyContentPack => new ReadOnlyContentPack(RiskOfRuneContentPack);
        internal static ContentPack RiskOfRuneContentPack { get; } = new ContentPack();

        public static ItemDef tennaBuckle;
        public static ItemDef jackKeyNOff;
        public static ItemDef lancerCard;
        public static ItemDef blueRibbion;
        public static ItemDef execBuffet;
        public static ItemDef execBuffetConsumed;
        public static ItemDef gingerGuard;
        public static ItemDef goldenIdol;
        public static ItemDef tvDinner;
        public static ItemDef tvDinnerConsumed;
        public static ItemDef gachaBall;
        public static ItemDef devilsKnife;
        public static ItemDef thornRing;
        public static ItemDef commRing;
        public static ItemDef pipis;
        public static ItemDef mrPipis;
        public static ItemDef guideBook;
        public static ItemDef bigShot;
        public static ItemDef susieAxe;
        public static ItemDef gasterMask;
        public static ItemDef roaringBlade;
        public static ItemDef scrapBall;
        public static ItemDef droneHealBoost;

        public static EquipmentDef neoEquipment;

        public static BuffDef jackBuff;
        public static BuffDef sanguineFrostbiteDebuff;
        public static BuffDef neoEliteBuff;
        public static BuffDef susieAxeBuff;
        public static BuffDef bigShotBuff;
        public static BuffDef swoonDebuff;
        public static BuffDef gasterCorruptionDebuff;

        public static EliteDef neoElite;
        public static AssetBundle bundle;

        public static GameObject susieAxeProjectilePrefab;
        public static GameObject bigShotProjectilePrefab;
        public static GameObject swoonPrefab;
        public static GameObject gasterCorruptionPrefab;

        public static EffectDef swoonEffect;
        public static EffectDef gasterCorruptionEffect;

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            var asyncOperation = AssetBundle.LoadFromFileAsync(RiskOfRuneMain.assetBundleDir);
            while(!asyncOperation.isDone)
            {
                args.ReportProgress(asyncOperation.progress);
                yield return null;
            }

            bundle = asyncOperation.assetBundle;
            tennaBuckle = bundle.LoadAsset<ItemDef>("TennaBuckle");
            jackKeyNOff = bundle.LoadAsset<ItemDef>("JackKey");
            lancerCard = bundle.LoadAsset<ItemDef>("LancerCard");
            blueRibbion = bundle.LoadAsset<ItemDef>("BlueRibbon");
            execBuffet = bundle.LoadAsset<ItemDef>("ExecBuffet");
            execBuffetConsumed = bundle.LoadAsset<ItemDef>("ExecBuffetConsumed");
            gingerGuard = bundle.LoadAsset<ItemDef>("GingerGuard");
            goldenIdol = bundle.LoadAsset<ItemDef>("GoldenIdol");
            tvDinner = bundle.LoadAsset<ItemDef>("TVDinner");
            tvDinnerConsumed = bundle.LoadAsset<ItemDef>("TVDinnerConsumed");
            gachaBall = bundle.LoadAsset<ItemDef>("GachaBall");
            devilsKnife = bundle.LoadAsset<ItemDef>("DevilsKnife");
            thornRing = bundle.LoadAsset<ItemDef>("ThornRing");
            commRing = bundle.LoadAsset<ItemDef>("CommemorativeRing");
            pipis = bundle.LoadAsset<ItemDef>("Pipis");
            mrPipis = bundle.LoadAsset<ItemDef>("MrPipis");
            scrapBall = bundle.LoadAsset<ItemDef>("ScrapBall");
            roaringBlade = bundle.LoadAsset<ItemDef>("RoaringBlade");
            gasterMask = bundle.LoadAsset<ItemDef>("GasterMask");
            bigShot = bundle.LoadAsset<ItemDef>("BigShot");
            guideBook = bundle.LoadAsset<ItemDef>("GuideBook");
            susieAxe = bundle.LoadAsset<ItemDef>("SusieAxe");

            neoEquipment = bundle.LoadAsset<EquipmentDef>("NeoEliteEquipment");

            jackBuff = bundle.LoadAsset<BuffDef>("JackKeyBuff");
            sanguineFrostbiteDebuff = bundle.LoadAsset<BuffDef>("SanguineFrostbiteDebuff");
            neoEliteBuff = bundle.LoadAsset<BuffDef>("NeoEliteBuff");
            bigShotBuff = bundle.LoadAsset<BuffDef>("BigShotBuff");
            swoonDebuff = bundle.LoadAsset<BuffDef>("RoaringBladeDebuff");
            gasterCorruptionDebuff = bundle.LoadAsset<BuffDef>("GasterCorruptionDebuff");

            neoElite = bundle.LoadAsset<EliteDef>("NeoElite");

            susieAxeProjectilePrefab = bundle.LoadAsset<GameObject>("SusieAxeProjectile");
            bigShotProjectilePrefab = bundle.LoadAsset<GameObject>("BigShotProjectile");
            swoonPrefab = bundle.LoadAsset<GameObject>("SwoonPrefab");
            gasterCorruptionPrefab = bundle.LoadAsset<GameObject>("GasterCorruptionPrefab");

            swoonEffect = new EffectDef(swoonPrefab);
            gasterCorruptionEffect = new EffectDef(gasterCorruptionPrefab);

            var exapansionDef = bundle.LoadAsset<ExpansionDef>("RiskOfRuneExpansion");

            AddItems();

            RiskOfRuneContentPack.itemDefs.Add(new ItemDef[] { 
                tennaBuckle, jackKeyNOff, lancerCard, blueRibbion, execBuffet, execBuffetConsumed, 
                devilsKnife, goldenIdol, tvDinner, tvDinnerConsumed, gachaBall, gingerGuard, thornRing, 
                pipis, mrPipis, commRing, scrapBall, roaringBlade, gasterMask, bigShot, guideBook, susieAxe
            });
            RiskOfRuneContentPack.buffDefs.Add(new BuffDef[] { 
                jackBuff, sanguineFrostbiteDebuff, neoEliteBuff, bigShotBuff, swoonDebuff, gasterCorruptionDebuff 
            });
            RiskOfRuneContentPack.eliteDefs.Add(new EliteDef[] { neoElite });
            RiskOfRuneContentPack.equipmentDefs.Add(new EquipmentDef[] { neoEquipment });
            RiskOfRuneContentPack.projectilePrefabs.Add(new GameObject[] { 
                susieAxeProjectilePrefab, bigShotProjectilePrefab 
            });
            RiskOfRuneContentPack.networkedObjectPrefabs.Add(new GameObject[] { swoonPrefab, gasterCorruptionPrefab });
            RiskOfRuneContentPack.effectDefs.Add(new EffectDef[] { swoonEffect, gasterCorruptionEffect });
            RiskOfRuneContentPack.expansionDefs.Add(new ExpansionDef[] { exapansionDef });

            RemoveFromLootPool();
        }
        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(RiskOfRuneContentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }
        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
        private void AddSelf(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }
        internal RiskOfRuneContent()
        {
            ContentManager.collectContentPackProviders += AddSelf;
        }
        private void AddItems()
        {
            TennaBuckleItem.Init();
            JackKeyItem.Init();
            LancerCardItem.Init();
            BlueRibbonItem.Init();
            ExecBuffetItem.Init();
            GingerGuardItem.Init();
            GoldenIdolItem.Init();
            GachaBallItem.Init();
            TVDinnerItem.Init();
            ThornRingItem.Init();
            DevilsKnifeItem.Init();
            PipisItem.Init();
            MrPipisItem.Init();
            GuideBookItem.Init();
            BigShotItem.Init();
            SusieAxeItem.Init();
            RoaringBladeItem.Init();
            GasterMaskItem.Init();
            ScrapBallItem.Init();

            NeoElite.Init();
        }

        public void RemoveFromLootPool()
        {
            Run.onRunStartGlobal += (run) =>
            {
                if (run == null) return;

                if (execBuffet != null) run.availableItems.Remove(execBuffet.itemIndex);
                if (execBuffetConsumed != null) run.availableItems.Remove(execBuffetConsumed.itemIndex);
                if (blueRibbion != null) run.availableItems.Remove(blueRibbion.itemIndex);
                if (gingerGuard != null) run.availableItems.Remove(gingerGuard.itemIndex);
                if (goldenIdol != null) run.availableItems.Remove(goldenIdol.itemIndex);
                if (tvDinner != null) run.availableItems.Remove(tvDinner.itemIndex);
                if (tvDinnerConsumed != null) run.availableItems.Remove(tvDinnerConsumed.itemIndex);
                if (thornRing != null) run.availableItems.Remove(thornRing.itemIndex);
                if (commRing != null) run.availableItems.Remove(commRing.itemIndex);
                //PickupDropTable.RegenerateAll(run);
                run.BuildDropTable();
            };
        }
    }
}
