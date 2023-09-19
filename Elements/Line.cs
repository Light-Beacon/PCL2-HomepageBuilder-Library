using System.Collections.Generic;

namespace PageBuilder.Elements
{
    /// <summary>
    /// 行元素
    /// </summary>
    public class Line : List<InlineElement>, IPGElement, LineElement
    {
        public string GetXaml()
        {
            string xaml = string.Empty;
            foreach (InlineElement t in this)
            {
                xaml += t.GetXaml();
            }
            return xaml;
        }
        public Line(string text) => Add(new PGTextBlock(text));
        public Line() : base() { }
    }
}
