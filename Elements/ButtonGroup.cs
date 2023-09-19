using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageBuilder.Elements
{
    public class ButtonGroup : TemplateElement, LineElement
    {
        List<Button> Buttons { get; set; }
        public string Content
        {
            get
            {
                string result = "";
                foreach (var button in Buttons)
                {
                    result += button.GetXaml();
                }
                return result;
            }
        }
        public string Type { get; set; }
        public ButtonGroup(List<Button> buttons) => Buttons = buttons;
        public ButtonGroup(string type)
        {
            Type = type;
            Buttons = new List<Button>();
        }
        public void Add(string name, string url)
        {
            Buttons.Add(new Button(name, url));
        }
        public override DataPairs GetData() => new DataPairs()
        {
            { "content",Content},
            { "type",Type}
        };
    }
}
