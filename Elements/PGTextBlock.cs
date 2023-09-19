namespace PageBuilder.Elements
{
    public class PGTextBlock : Text, ITemplateElement, LineElement, InlineElement
    {
        public DataPairs GetData() => new DataPairs() { { "content", text } };
        public override string GetXaml() => Common.templateManager.AssembleXaml(this);
        public PGTextBlock(string content) : base(content) { }
    }
}
