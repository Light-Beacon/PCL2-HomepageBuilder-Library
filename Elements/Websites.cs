#nullable enable
namespace PageBuilder.Elements
{
    /// <summary>
    /// 三家原文链接
    /// </summary>
    public class Websites : ITemplateElement
    {
        string mcbbsUri;
        string officalUri;
        string wikiUri;
        const string DISABLE_SUFFIX = "IsEnabled=\"false\"";
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public Websites(string? wikiUri, string? mcbbsUri, string? officalUri)
        {
            this.mcbbsUri = mcbbsUri??"";
            this.officalUri = officalUri??"";
            this.wikiUri = wikiUri??"";
        }
        public DataPairs GetData() => new DataPairs()
        {
            {"mcbbslink",mcbbsUri},
            {"officallink",officalUri},
            {"wikilink",wikiUri},
            {"EnableWikiSuffix",wikiUri.Length == 0?DISABLE_SUFFIX:""},
            {"EnableBBSSuffix",mcbbsUri.Length == 0?DISABLE_SUFFIX:""},
            {"EnableOFLSuffix",officalUri.Length == 0?DISABLE_SUFFIX:""}
        };
    }
}