using PageBuilder.Elements;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PageBuilder.Cards
{
#if Client
    public abstract class ContentCard : IDataObject
#else
    public abstract class ContentCard
#endif
    {
        /// <summary> 获取或设置卡片是否折叠 </summary>
        public virtual bool isSwaped { get; set; }
        /// <summary> 获取或设置卡片的定义位置 </summary>
        //public virtual string path { get; set; }
        /// <summary> 获取或设置卡片的ID名 </summary>
        public virtual string name { get; set; }
        /// <summary> 获取或设置卡片数据文件名 </summary>
        //public virtual string dataFileName { get => name; }
        /// <summary> 获取卡片在编辑器中显示的名称 </summary>
        public abstract string GetDisplayTitle();
        /// <returns> 卡片XAML代码 </returns>
        public abstract string GetCode(GenerateArgs args,bool? IsSwaped);
        /// <summary>
        /// 将 ContentCard 对象转为 Jobject 对象
        /// </summary>
        /// <param name="withdata">转换时是否包含数据（一般只有在保存时不包含）</param>
        public abstract JObject ToJObject(bool withdata = true);
        /// <summary>
        /// 返回该卡片包含数据
        /// </summary>
        /// <returns>卡片内数据</returns>
        public abstract string GetData();
        /// <summary>
        /// 保存卡片数据
        /// </summary>
        //public abstract void SaveData(string basePath);
        /// <summary>
        /// 获取卡片种类
        /// </summary>
        public abstract string type { get; }
        /// <summary>
        /// 获取卡片XAML代码
        /// </summary>
        public string displayTitle
        {
            get
            {
                return GetDisplayTitle();
            }
        }
        /// <summary>
        /// 获取卡片折叠状态文字显示
        /// </summary>
        public string statusText
        {
            get
            {
                if (isSwaped)
                    return "▽";
                else
                    return "";
            }
        }
        /// <summary>
        /// 切换卡片折叠状态
        /// </summary>
        public void switchSwapStats()
        {
            isSwaped = !isSwaped;
        }

        public virtual object GetData(string format) => GetData(format, false);
        public virtual object GetData(Type format)
        {
            if (GetDataPresent(format))
                return GetData(format.FullName);
            else
                return null;
        }

        public Dictionary<ValueTuple<string ,bool>, Func<object>> getDataFuncs = new Dictionary<ValueTuple<string, bool>, Func<object>>();
        public Dictionary<ValueTuple<string, bool>, Action<object>> setDataActions = new Dictionary<(string, bool), Action<object>>();
        public ContentCard()
        {
            getDataFuncs.Add((this.GetType().FullName,false), () => this);
            getDataFuncs.Add((typeof(ContentCard).FullName, false),() => this);
            getDataFuncs.Add((this.GetType().FullName, true), () => this);
            getDataFuncs.Add((typeof(ContentCard).FullName, true), () => this);
        }
        public virtual object GetData(string format, bool autoConvert)
        {
            try
            {
                if (GetDataPresent(format,autoConvert))
                    return getDataFuncs[(format,autoConvert)].Invoke();
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        public virtual string[] GetFormats(bool autoConvert)
        {
            var result = new List<string>();
            foreach(var pair in getDataFuncs)
            {
                if(pair.Key.Item2 == autoConvert)
                    result.Add(pair.Key.Item1);
            }
            return result.ToArray();
        }
        public virtual void SetData(string format, object data, bool autoConvert)
        {
            setDataActions[(format,autoConvert)].Invoke(data);
        }

        public bool GetDataPresent(string format) => GetDataPresent(format, false);
        public bool GetDataPresent(Type format)
        {
            return GetDataPresent(format.FullName);
        }
        public bool GetDataPresent(string format, bool autoConvert)
        {
            return getDataFuncs.ContainsKey((format, autoConvert));
        }
        public string[] GetFormats() => GetFormats(false);

        public void SetData(object data) => SetData(data.GetType().FullName, data, false);
        public void SetData(string format, object data) => SetData(format, data, false);
        public void SetData(Type format, object data) => SetData(format.FullName, data, false);
    }

    public struct CardRef
    {
        public string LibName { get; set; }
        public string CardName { get; set; }
        public bool? IsSwaped { get; set; }
        public bool? GetInPageSwapStatus(PageGenMode arg)
        {
            if (arg == PageGenMode.ForceSwapAll)
                return true;
            if (arg == PageGenMode.ForceUnswapall)
                return false;
            if (IsSwaped != null)
                return IsSwaped;
            if (arg == PageGenMode.Auto)
                return null;
            if (arg == PageGenMode.SwapAll)
                return true;
            return false;
        }
        public override string ToString()
        {
            return $"{GetSwapStatusHead(IsSwaped)}{LibName}:{CardName}";
        }
        public CardRef(string name)
        {
            if (name[0] == '+')
            {
                IsSwaped = false;
                name = name[1..];
            }
            else if (name[0] == '-')
            {
                IsSwaped = true;
                name = name[1..];
            }
            else
                IsSwaped = null;
            var tmp = name.Split(':');
            if (tmp.Length == 2)
            {
                LibName = tmp[0];
                CardName = tmp[1];
            }
            else
                throw new ArgumentException($"{name}:命名不规范，亲人两行泪");
        }
        public static char? GetSwapStatusHead(bool? isswaped)
        {
            if (isswaped == null)
                return null;
            if (isswaped == true)
                return '-';
            else
                return '+';
        }
        public ContentCard GetCard()
        {
            return Common.CurrentProject.GetCard(this);
        }
        public static implicit operator CardRef(string str)
        {
            return new CardRef(str);
        }
        public string displayTitle => GetCard().displayTitle;
        public string statusText => GetCard().statusText;
    }
}
