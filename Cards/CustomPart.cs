using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows;
using static PageBuilder.Debug;

namespace PageBuilder.Cards
{
    public class CustomCard : ResourceBasedCard
    {
        string title;
        string xaml { get => Common.CurrentProject.imageMap.ReplaceXAML(resText); set { resText = value; } }
        public override string type { get => "Custom"; }
        public override string GetDisplayTitle()
        {
            return title + " (C)";
        }
        public override string GetCode(GenerateArgs args,bool? swap)
        {
            Log($"正在生成{title}：返回代码", 1);
            return xaml;
        }
        public override JObject ToJObject(bool withdata = true)
        {
            JObject jobj = new JObject();
            jobj.Add("type", "custom");
            jobj.Add("title", title);
            jobj.Add("name", name);
            jobj.Add("resource", ResourceFileName);
            if (withdata)
                jobj.Add("xaml", xaml);
            //jobj.Add("path", path);
            return jobj;
        }
        public CustomCard(string _name, string _title, string _xaml)
        {
            name = _name;
            title = _title;
            xaml = _xaml;
            //path = _path;
        }
        public CustomCard(JObject jobj)
        {
            if (jobj["type"].ToString().ToLower() != type.ToLower())
                throw new Exception($"{jobj[name]}卡片类型不符");
            name = jobj["name"].ToString();
            title = jobj["title"].ToString();
            if (jobj["resource"] == null)
                ResourceFileName = name + ".xaml";
            else
                ResourceFileName = jobj["resource"].ToString();
        }

        public override string GetData()
        {
            return resText;
        }

        public override bool isSwaped
        {
            get { return false; }
            set {
#if Client
                if (value) MessageBox.Show("你不可以对自定义部分进行折叠！"); 
#endif
                }
        }

        public static CustomCard ConstructFromFile(string title, string name, string path)
        {
            if (!File.Exists(Common.CurrentProjectPath + "Customs"+ Path.DirectorySeparatorChar + name.Replace('\\', Path.DirectorySeparatorChar) + ".xaml"))
                throw new IOException("构建CustomPart时不存在该文件");
            return new CustomCard(name, title, GetXamlFromFile(name));
        }

        static string GetXamlFromFile(string name) => File.ReadAllText(Common.CurrentProjectPath + "Customs" + Path.DirectorySeparatorChar + name.Replace('\\', Path.DirectorySeparatorChar) + ".xaml");
    }

}