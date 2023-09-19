using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageBuilder.Elements
{
    internal class LatestVersionAnimation: ITemplateElement
    {
        public string randomname { get; set; }
        public string GetXaml() => Common.templateManager.AssembleXaml(this);
        public DataPairs GetData() => new DataPairs()
        {
            { "randomname",randomname }
        };
        public LatestVersionAnimation(string randomname_)
        {
            randomname = randomname_;
        }
    }
}
