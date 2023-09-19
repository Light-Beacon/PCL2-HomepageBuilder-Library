    using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using static PageBuilder.Debug;

namespace PageBuilder.Cards
{
    public class Separator : ContentCard
    {
        public override string name { get => title; }
        public string title;
        public override string type { get => "Separator"; }
        public override string GetCode(GenerateArgs args,bool? swap)
        {
            return new Elements.Separator(title).GetXaml();
        }

        public override JObject ToJObject(bool _ = true)
        {
            JObject jobj = new JObject();
            jobj.Add("type", "separator");
            jobj.Add("title", title);
            return jobj;
        }

        public static explicit operator JObject(Separator separator)
        {
            return separator.ToJObject();
        }
        public static explicit operator Separator(JObject obj)
        {
            return new Separator(obj["name"].ToString());
        }
        public Separator(JObject obj)
        {
            title = obj["name"].ToString();
        }

        public Separator(string _title)
        {
            title = _title;
        }

        public override string GetDisplayTitle()
        {
            return "-- " + title + " --";
        }

        public override bool isSwaped
        {
            get { return false; }
            set {
#if Clinet
                if (value) MessageBox.Show("你不可以对分隔符进行折叠！"); 
#endif
                }
        }
        public override string GetData()
        {
            throw new NotImplementedException();
        }
    }
}
