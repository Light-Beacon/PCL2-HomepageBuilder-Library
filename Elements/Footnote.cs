namespace PageBuilder.Elements
{
    /// <summary>
    /// 尾注元素
    /// </summary>
    public class FootNote : Line, ITemplateElement, IPGElement
    {
        public new string GetXaml() => Common.templateManager.AssembleXaml(this);
        public DataPairs GetData() => new DataPairs() { { "content", base.GetXaml() } };
        public FootNote(string content)
        {
            Add((Text)content);
        }
    }
}
