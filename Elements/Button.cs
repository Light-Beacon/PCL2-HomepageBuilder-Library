namespace PageBuilder.Elements
{
    public class Button : TemplateElement, InlineElement
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public Button(string title, string imageUrl)
        {
            Title = title;
            ImageUrl = imageUrl;
        }
        public override DataPairs GetData() => new DataPairs()
        {
            { "title",Title },
            { "imgUrl",ImageUrl }
        };
    }
}
