namespace PageBuilder.Elements
{
    public class Title : LineElement, ITemplateElement
    {
        int level;
        public string Content { get; set; }
        public DataPairs GetData() => new DataPairs()
        {
            { "level",level.ToString() },
            { "content",Content }
        };
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public Title(int level, string content)
        {
            this.level = level;
            this.Content = content;
        }
    }
}
