namespace PageBuilder.Elements
{
    /// <summary>
    /// 带链接的文字元素
    /// </summary>
    public class LinkText : Text, ITemplateElement
    {
        string link;
        public string Link => link;
        public override string GetXaml() => Common.templateManager.AssembleXaml(this);
        public LinkText(string content, string uri) : base(content) => link = uri;
        public DataPairs GetData() => new DataPairs() { { "link", link }, { "text", text } };
    }
}