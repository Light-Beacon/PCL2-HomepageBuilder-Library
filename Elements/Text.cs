namespace PageBuilder.Elements
{
    /// <summary>
    /// 页面文字元素
    /// </summary>
    public class Text : ITextElement, InlineElement
    {
        protected string text;
        public virtual string GetXaml() => Trans(text);
        public string GetText() => text;
        public Text(string content) => text = content;
        public static implicit operator Text(string text) => new Text(text);
        public static implicit operator string(Text text) => text.GetText();

        public static string Trans(string input)
        {
            input = input.Replace("&", "&amp;");
            input = input.Replace("<", "&lt;");
            input = input.Replace(">", "&gt;");
            input = input.Replace("'", "&apos;");
            input = input.Replace("\"", "&quot;");
            return input;
        }
    }
}
