using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static PageBuilder.Debug;
using PageBuilder.Cards;
using PageBuilder.Elements;
#if Client
using SaveDlg = Microsoft.Win32.SaveFileDialog;
#endif
using System.Windows;

namespace PageBuilder
{
    public delegate string Generater();
    public static class PageManager
    {
        public static Dictionary<string,XamlPage> Pages;
        static void AddPage(XamlPage page)
        {
            Pages.Add(page.Name, page);
        }
        static void Load(string[] paths)
        {
            foreach(string path in paths)
            {

            }
        }
    }

    /// <summary>
    /// 页面结构与生成器管理类
    /// </summary>
    public static class GenerateManager
    {
        static Dictionary<string, string> structures = new Dictionary<string, string>();
        //static Dictionary<Guid, Dictionary<string, Generater>> generaters = new Dictionary<Guid, Dictionary<string, Generater>>();//<Gen所有者，<Gen名，Gen委托>>
        /// <summary>
        /// 返回结构内容
        /// </summary>
        /// <param name="structName">结构名称</param>
        /// <returns>结构内容字符串</returns>
        public static string GetStruct(string structName)
        {
            return structures[structName];
        }
        /// <summary>
        /// 清空结构与生成器的注册
        /// </summary>
        public static void Clear()
        {
            structures.Clear();
            Log("[生成管理] 已清除结构数据",1);
        }
        /// <summary>
        /// 未注册异常
        /// </summary>
        class NotRegistedException:Exception
        {
            public NotRegistedException():base() { }
            public NotRegistedException(string message):base(message){ }
        }

    }

    public interface IPage
    {
        public string GenerateCode(GenerateArgs args, (string, string, string) paths);
        public void Save();
        public void Load();
        public string GetPageJson();
        public string Name { get; }
        public JArray Alias { get; }
        public string PagePath { get; }
        public bool IsHidden { get; }
    }

    public enum PageGenMode
    {
        Auto,SwapAll,UnswapAll,ForceSwapAll,ForceUnswapall,UnswapFirstVersion
    }

    public class RawPage : IPage
    {
        Guid guid;
        public Guid UUID => guid;
        string name;
        public string Name => name;
        protected JArray alias = new JArray();
        public JArray Alias => alias;
        string pagePath;
        public string PagePath => pagePath;
        string code;
        public bool IsHidden => true;
        public string GenerateCode(GenerateArgs args, (string, string, string) paths)
        {
            return code;
        }
        public RawPage(string path, bool preLoad = false)
        {
            pagePath = path;
            guid = Guid.NewGuid();
            name = Path.GetFileNameWithoutExtension(path);
            if (preLoad)
                Load();
        }
        public void Load()
        {
            try
            {
                code = File.ReadAllText(pagePath);
                code = Common.CurrentProject.imageMap.ReplaceXAML(code);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void Save()
        {
            try
            {
                File.WriteAllText(pagePath, code);
            }
            catch (Exception e)
            {
                LogError(e);
#if Client
                MessageBox.Show(e.Message);
#endif
            }
        }

        public string GetPageJson()
        {
            var obj = new JObject();
            obj.Add("Title", Name);
            return obj.ToString();
        }
    }

    public class XamlPage : IPage
    {
        string pagePath;
        public string PagePath => pagePath;
        public string ProjectPath { get; }
        string name;
        public string outputPath;
        public string Name => name;
        public bool NoStyle = false;
        public Cards cards;
        //Generater generateCards;
        protected JArray alias = new JArray();
        public JArray Alias => alias;
        PageGenMode genMode = PageGenMode.Auto;
        public bool IsHidden { get; set; }
        void Init()
        {
            cards = new Cards();
            //generateCards = new Generater(cards.GenerateCode);
        }

        public string GenerateCode(GenerateArgs args, (string, string, string) paths)
        {
            string result;
            DateTime time = DateTime.Now;
            var aniStr = "";
            var styStr = "";
            Log("获取 新闻主页开头注释", 2);
            result = File.ReadAllText(paths.Item1);
            if (!NoStyle)
            {
                Log("获取 新闻主页动画", 2);
                aniStr = File.ReadAllText(paths.Item2);//2
                Log("获取 新闻主页样式", 2);
                styStr = File.ReadAllText(paths.Item3);//3
            }
            var code = string.Empty;
            bool foundFirstSSCard = false;
            bool foundFirstRSCard = false;
            foreach (CardRef cr in this.cards)
            {
                var card = cr.GetCard();
                Log($"生成 {card.name}", 2);
                if (genMode == PageGenMode.UnswapFirstVersion && card.type == "Version")
                {
                    if((card as VersionCard).GetVersionType() == MCVType.Release)
                    {
                        code += (card as VersionCard).GetCode(args, cr.IsSwaped ?? foundFirstRSCard, !foundFirstRSCard);
                        if (!foundFirstRSCard)
                            foundFirstRSCard = true;
                    }
                    else
                    {
                        code += (card as VersionCard).GetCode(args, cr.IsSwaped ?? foundFirstSSCard, !foundFirstSSCard);
                        if (!foundFirstSSCard)
                            foundFirstSSCard = true;
                    }
                }
                else
                    code += card.GetCode(args,cr.GetInPageSwapStatus(genMode));
            }
            result += Common.templateManager.AssembleXaml(new MainStructure(aniStr, styStr, code));
            Log("格式化代码", 2);
            result = XAMLFormatter.FormatXAML(result); //6
            Log("生成完毕", 2);
            Log("生成代码花费时间：" + (DateTime.Now - time).ToString(), 2);
            if (args.CDNMode)
            {
                result = result.Replace("https://www.lightbeacon.top/pnh/", "http://news.obj.thestack.top/pnh/");
                result = result.Replace("https://test.bugjump.net/news/", "http://news.obj.thestack.top/news/");
                //result = result.Replace("新闻","块讯");
            }
            return result;
        }

        public string GetPageJson()
        {
            var obj = new JObject();
            obj.Add("Title", Name);
            return obj.ToString();
        }

        /// <summary>
        /// 页面构造函数
        /// </summary>
        /// <param name="projectPath">项目路径</param>
        /// <param name="path">页面路径</param>
        /// <param name="preLoad">是否预载（默认为否）</param>
        public XamlPage(string projectPath,string path,string OutputPath,string _name = null,bool preLoad = false)
        {
            pagePath = path;
            outputPath = OutputPath;
            if (name != null)
                name = _name;
            ProjectPath = projectPath;
            Init();
            if (preLoad)
                Load();
        }

        public XamlPage(string projectPath, string path, bool preLoad = false)
        {
            pagePath = path;
            ProjectPath = projectPath;
            Init();
            if (preLoad)
                Load();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~XamlPage()
        {
            //GenerateManager.RemoveOwner(UUID);
        }

        public void Save()
        {
            try
            {
                Log($"[Page] 正在保存：{pagePath}", 1);
                JObject savingProject = new JObject();
                savingProject.Add("name", name);
                savingProject.Add("outputPath", outputPath.Replace(ProjectPath, "~"));
                if(NoStyle)
                    savingProject.Add("NOSTYLE", NoStyle);
                if (alias.Count > 0)
                    savingProject.Add("alias", alias);
                if (IsHidden)
                    savingProject.Add("IsHidden", IsHidden);
                if (genMode != PageGenMode.Auto)
                    savingProject.Add("genmode", genMode.ToString());
                JArray jArray = new JArray();
                foreach (var cr in cards)
                    jArray.Add(cr.ToString());
                savingProject.Add("cards", jArray);
                File.WriteAllText(pagePath, savingProject.ToString());
            }
            catch(Exception e)
            {
                LogError(e);
                #if Client
                MessageBox.Show(e.Message);
                #endif
            }
        }

        /// <summary>
        /// 加载页面内容
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Load()
        {
            try
            {
                string fileContent = File.ReadAllText(pagePath);
                    var jobj = JObject.Parse(fileContent);
                    name = jobj["name"].ToString();
                    outputPath = jobj["outputPath"].ToString().Replace("~", ProjectPath);
                    if (jobj.ContainsKey("NOSTYLE"))
                        NoStyle = (bool)jobj["NOSTYLE"];
                    if (jobj.ContainsKey("IsHidden"))
                        IsHidden = (bool)jobj["IsHidden"];
                    if (jobj.ContainsKey("alias"))
                        alias = jobj["alias"] as JArray;
                    if (jobj.ContainsKey("genmode"))
                        genMode = (PageGenMode)Enum.Parse(typeof(PageGenMode), jobj["genmode"].ToString());
                    LoadCards((JArray)jobj["cards"]);
                
                //GenerateManager.RegistGenerater(UUID, "card", cards.GenerateCode);
            }
            catch(Exception e)
            {
                throw e;
            }  
        }

        void LoadCards(JArray array)
        {
            CardRef cr;
            foreach (var obj in array)
            {
                if(obj is JObject)
                {
                    cr = new CardRef(obj["name"].ToString());
                    if (((JObject)obj).ContainsKey("isswaped"))
                        cr.IsSwaped = obj["isswaped"].ToString() == "True";
                }
                else
                    cr = new CardRef(obj.ToString());
                cards.Add(cr);
            }
        }
        
        /// <summary>
        /// 页面的卡片类
        /// </summary>
        public class Cards : ObservableCollection<CardRef>
        {
            public new void Move(int from, int des)
            {
                if (from < 0 || from > this.Count())
                    throw new IndexOutOfRangeException("目标卡片不存在");
                if (des < 0)
                    throw new IndexOutOfRangeException("目标位置小于0");
                if (des > this.Count())
                    throw new IndexOutOfRangeException("目标位置大于列表里所拥有的卡片");
                base.Move(from, des);
            }

            public string GenerateCode(GenerateArgs args)
            {
                string result = string.Empty;
                foreach (var cardref in this)
                {
                    var card = cardref.GetCard();
                    Log($"生成 {cardref}",2);
                    result += card.GetCode(args, cardref.IsSwaped);
                }
                return result;
            }
        }
    }
}
