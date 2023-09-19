using System.Collections.Generic;
using System.Linq;

namespace PageBuilder.Elements
{
    /// <summary>
    /// 项目列表项目元素
    /// </summary>
    public class BulletsLine : IPGElement, LineElement, ITemplateElement, IBulletsElement
    {
        public Line Line { get; set; }
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public BulletsLine(Line line)
        {
            this.Line = line;
        }
        public BulletsLine()
        {
            Line = new Line();
        }
        public DataPairs GetData() => new DataPairs() { { "content", Line.GetXaml() } };
        public void Join(string code) => Line.Add(new Text(code));
    }

    /// <summary>
    /// 项目列表元素
    /// </summary>
    public class BulletsList : List<IBulletsElement>, IPGElement, ITemplateElement, IBulletsElement
    {
        public string GetAllLines()
        {
            string xaml = string.Empty;
            foreach (var t in this)
                xaml += t.GetXaml() + "\n";
            return xaml;
        }
        public DataPairs GetData() => new DataPairs() { { "content", GetAllLines() } };
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public void Join(string code)
        {
            if (code[0] == '*' || code[0] == '\t')
            {
                code = code.Remove(0, 1);
            }
            else if (code[..4] == "    ")
            {
                code = code[4..];
            }
            else
                return;
            if (code[0] == '*' || code[0] == '\t' || (code.Length > 4 && code[..4] == "    "))
            {
                if (!(Count != 0 && this.Last() is BulletsList))
                    Add(new BulletsList());
            }
            else
            {
                if (code[0] == ' ')
                    Add(new BulletsLine());
            }
            this[^1].Join(code);
        }
    }
}