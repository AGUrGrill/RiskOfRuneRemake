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

        public static BuffDef jackBuff;
        

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
            var exapansionDef = bundle.LoadAsset<ExpansionDef>("RiskOfRuneExpansion");

            AddItems();

            RiskOfRuneContentPack.itemDefs.Add(new ItemDef[] { tennaBuckle, jackKeyNOff, lancerCard, blueRibbion, execBuffet, execBuffetConsumed });
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

                PickupDropTable.RegenerateAll(run);
            };
        }
    }
}
