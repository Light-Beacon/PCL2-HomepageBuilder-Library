namespace PageBuilder.Elements
{
    public class L_Loading : TemplateElement, LineElement
    {
        public string Content { get; set; }
        public override DataPairs GetData() => new DataPairs()
        {
            { "content",Content }
        };
        public L_Loading(string content)
        {
            Content = content;
        }
    }
    public class L_Hint : TemplateElement, LineElement
    {
        public string Content { get; set; }
        public bool Warn { get; set; }
        public override DataPairs GetData() => new DataPairs()
        {
            { "content",Content },
            { "warn",Warn.ToString() }
        };
        public L_Hint(string content, bool isWarn = true)
        {
            Content = content;
            Warn = isWarn;
        }
    }
    public class L_Button : TemplateElement, LineElement
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string Tip { get; set; }
        public override DataPairs GetData() => new DataPairs()
        {
            { "text",Text },
            { "type",Type },
            { "data",Data },
            { "tip",Tip }
        };
        public L_Button(string text, string type, string data = "", string tip = "")
        {
            Text = text;
            Type = type;
            Data = data;
            Tip = tip;
        }
    }
}
