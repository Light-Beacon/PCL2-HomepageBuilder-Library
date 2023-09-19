using System.Collections.Generic;

namespace PageBuilder.Elements
{
    /// <summary>
    /// 卡片类
    /// </summary>
    public class Card : ITemplateElement
    {
        List<IPGElement> contents;
        public bool CanSwap { get; set; }
        public bool? IsSwaped { get; set; }
        public string Title { get; set; }
        public void Add(IPGElement element) => contents.Add(element);
        public void Add(List<IPGElement> elements) => contents.AddRange(elements);
        public void Remove(IPGElement element) => contents.Remove(element);
        string GetPartXaml(List<IPGElement> list)
        {
            string xaml = string.Empty;
            foreach (var ele in list)
            {
                xaml += ele.GetXaml() + "\n";
            }
            return xaml;
        }
        string BodyXaml => GetPartXaml(contents);
        public DataPairs GetData() => new DataPairs()
        {
            { "title",Text.Trans(Title)},
            { "body", BodyXaml },
            { "canswap",CanSwap.ToString() },
            { "extra", IsSwaped == null ? "" : $"IsSwaped=\"{IsSwaped}\"" }
        };
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public Card(string title, bool canSwap = true, bool? isSwaped = true)
        {
            contents = new List<IPGElement>();
            Title = title;
            CanSwap = canSwap;
            IsSwaped = isSwaped;
            //让swap nullable
        }
        public Card(Title title, List<IPGElement> contents, bool canSwap = true, bool isSwaped = true)
        {
            contents = new List<IPGElement>();
            Title = Title;
            this.contents = contents;
            CanSwap = canSwap;
            IsSwaped = isSwaped;
        }
    }
}
