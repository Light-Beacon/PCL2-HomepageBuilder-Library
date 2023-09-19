namespace PageBuilder.Elements
{
    /// <summary>
    /// 图片元素
    /// </summary>
    public class Image : IPGElement, ITemplateElement
    {
        private string imageSource_;
        protected string ImageSource { get => imageSource_; set => imageSource_ = Common.CurrentProject.imageMap.TryMapString(value); }
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public DataPairs GetData() => new DataPairs() { { "source", ImageSource } };
        public Image(string imageSource)
        {
            this.ImageSource = imageSource;
        }
    }
    /// <summary>
    /// 带副标题的图片元素
    /// </summary>
    public class ImageWithTitle : Image, IPGElement, ITemplateElement
    {
        public string Title { get; set; }
        public ImageWithTitle(string imageSource, string title) : base(imageSource)
        {
            Title = title;
        }
        public new DataPairs GetData() => new DataPairs() { { "title", Title }, { "source", ImageSource } };
        public new string GetXaml() => Common.templateManager.AssembleXaml(this);
    }
    /// <summary>
    /// 头图元素
    /// </summary>
    public class HeaderImage : Image, IPGElement, ITemplateElement
    {
        public string Title { get; set; }
        public string Latestbdrname { get; set; }
        public HeaderImage(string title, string imageSource,string latestbdrname = "") : base(imageSource)
        {
            this.Title = title;
            this.Latestbdrname = latestbdrname;
        }
        public new DataPairs GetData() => new DataPairs() { { "title", Title }, { "source", ImageSource },
            { "deco", Latestbdrname.Length > 0 ? $"x:Name = \"latest_{Latestbdrname}\"":""}};
    }
    public class HeaderImage2 : HeaderImage
    {
        public HeaderImage2(string title, string imageSource, string latestbdrname = "") : base(title,imageSource,latestbdrname)
        {
            
        }
    }
}