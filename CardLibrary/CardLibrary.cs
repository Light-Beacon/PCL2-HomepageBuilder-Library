using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsHomepageHelper.PageBuilder.CardLibrary
{
    /// <summary>
    /// 收集卡片的方式
    /// </summary>
    enum CollectMethod
    {
        /// <summary>
        /// 不主动收集卡片
        /// </summary>
        disabled,
        /// <summary>
        /// 枚举每个 .json 文件
        /// </summary>
        enumfile,
        /// <summary>
        /// 枚举每个文件夹，获取里边的 card.json 文件
        /// </summary>
        enumdir,
        /// <summary>
        /// 枚举 .json 和 每个文件夹里的 card.json
        /// </summary>
        enumboth,
        /// <summary>
        /// 分为多个库注册进库列表
        /// </summary>
        group,
        /// <summary>
        /// 组合为一个库注册进库列表
        /// </summary>
        combine,
    }

    internal class CardLibrary
    {
        string Namespace { get; set;};
        string Name {  get; set;};
        CollectMethod LibCollectMethod { get; set};
        List<ContentCard> _CardList;
        Queue<ImportAction> ImportActions;
        public CardLibrary(){}
        public CardLibrary(string filepath)
        {
            try
            {
                return CreatFromFile(filepath);
            }
            catch
            {
                return null;
            }
        }
        public static CardLibrary CreatFromFile(string filepath)
        {
            CardLibrary lib = new();
            JObject jobj = JObject.Parse(File.ReadAllText(LibPath));
            if(jobj.ContainsKey(Actions))
            {
                foreach(actionObj in (jobj as JArray))
                {
                    ImportActions.Join(new ImportAction(actionObj["Action"],actionObj["Target"],actionObj["Value"]??"");
                    ImportActions.Last.Execute(jobj);
                }
            }
            lib.Namespace = jobj["namespace"].ToString();
            lib.Name = (jobj["name"] ?? jobj["namespace"]).ToString();
            lib.LibCollectMethod = (CollectMethod)Enum.Prase(typeof(CollectMethod),
                                        jobj["CollectMethod"].ToString());
        }
    }

    internal class ImportAction
    {
        private static Dictionary<string,ICardImportAction> _cardImportActionMapping;
        string Action { get; set;};
        string Target { get; set;};
        string Value { get; set;};
        ImportAction(string action,string target,string value)
        {
            Action = action;
            Target = target;
            Value = value;
        }
        void Execute(JObject CardJobj)
        {
            _cardImportActionMapping[Action].Execute(CardJobj,Target,Value);
        }
    }

    internal class CardCollector
    {
        
    }

}
