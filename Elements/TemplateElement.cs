namespace PageBuilder.Elements
{
    public abstract class TemplateElement : IPGElement, ITemplateElement
    {
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public abstract DataPairs GetData();
    }
}