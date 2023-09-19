namespace PageBuilder.Elements
{
    public class Separator : TemplateElement, IPGElement
    {
        public string Content { get; set; }
        public string ContentWithSpace
        {
            get
            {
                string output = "";
                for (int i = 1; i < Content.Length; i++)
                {
                    output += Content[i - 1] + "  ";
                }
                output += Content[Content.Length - 1];
                return output;
            }
        }
        public Separator(string content)
        {
            Content = content;
        }
        public override DataPairs GetData() => new DataPairs()
        {
            { "content",ContentWithSpace}
        };
    }
}