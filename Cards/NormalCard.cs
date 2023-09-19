using Newtonsoft.Json.Linq;
using static PageBuilder.Debug;
using System.IO;
using System;

namespace PageBuilder.Cards
{
    public class NormalCard : ResourceBasedCard
    {
        string title;
        public override string type { get => "Normal"; }
        //public override string dataFileName => name + ".md";
        public NormalCard(string Title, string MdCode, string Name = "", bool swaped = false)
        {
            title = Title;
            resText = MdCode;
            //path = Path;
            name = (Name == "") ? Title : Name;
            isSwaped = swaped;
        }
        public override string GetCode(GenerateArgs args,bool? swap) => GetCard(args,swap).GetXaml();
        public Elements.Card GetCard(GenerateArgs args,bool? swap)
        {
            var converter = new NewConverter();
            var output = new Elements.Card(displayTitle, true, args.SwapAllCard|swap);
            output.Add(converter.Convert(resText));
            return output;
        }

        public NormalCard(JObject jobj)
        {
            if (jobj["type"].ToString().ToLower() != type.ToLower())
                throw new Exception($"{jobj[name]}卡片类型不符");
            this.title = jobj["title"].ToString();
            this.name = jobj["name"].ToString();
            //this.path = jobj["path"].ToString();
            if (jobj["mdSource"] == null)
                this.ResourceFileName = jobj["name"].ToString() + ".md";
            else
                ResourceFileName = jobj["resource"].ToString();   
            if (jobj["isswaped"] != null && jobj["isswaped"].ToString().ToLower() == "true")
                isSwaped = true;

        }

        public override JObject ToJObject(bool withdata = true)
        {
            JObject jobj = new JObject();
            jobj.Add("type", "normal");
            jobj.Add("title", title);
            jobj.Add("name", name);
            jobj.Add("resource", ResourceFileName);
            if (withdata)
                jobj.Add("mdCode", resText);
            //jobj.Add("path", path);
            return jobj;
        }
        public override string GetData() => resText;
        public override string GetDisplayTitle() => title;
    }

}
