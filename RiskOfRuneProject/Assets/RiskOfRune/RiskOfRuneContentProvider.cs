using RoR2.ContentManagement;
using UnityEngine;
using RoR2;
using System.Collections;
namespace RiskOfRune
{
    public class RiskOfRuneContent : IContentPackProvider
    {
        public string identifier => RiskOfRuneMain.GUID;

        public static ReadOnlyContentPack readOnlyContentPack => new ReadOnlyContentPack(RiskOfRuneContentPack);
        internal static ContentPack RiskOfRuneContentPack { get; } = new ContentPack();

        public static ItemDef tennaBuckle;
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
            var exapansionDef = bundle.LoadAsset<RoR2.ExpansionManagement.ExpansionDef>("RiskOfRuneExpansion");

            RiskOfRuneContentPack.itemDefs.Add(new ItemDef[] { tennaBuckle});
            RiskOfRuneContentPack.expansionDefs.Add(new RoR2.ExpansionManagement.ExpansionDef[] { exapansionDef });
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
    }
}
