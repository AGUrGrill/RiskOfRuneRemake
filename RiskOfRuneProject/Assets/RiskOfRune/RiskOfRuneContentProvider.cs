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

        public static BuffDef jackBuff;
        public static BuffDef sanguineFrostbite;


        public static AssetBundle bundle;

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

            jackBuff = bundle.LoadAsset<BuffDef>("JackBuff");
            var exapansionDef = bundle.LoadAsset<ExpansionDef>("RiskOfRuneExpansion");

            AddItems();

            RiskOfRuneContentPack.itemDefs.Add(new ItemDef[] { tennaBuckle, jackKeyNOff, lancerCard, blueRibbion, execBuffet, execBuffetConsumed, 
                devilsKnife, goldenIdol, tvDinner, tvDinnerConsumed, gachaBall, gingerGuard, thornRing, pipis, mrPipis, commRing });
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
                PickupDropTable.RegenerateAll(run);
            };
        }
    }
}
