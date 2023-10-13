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
        string Namespace;
        string Name;
    }

    internal class CardCollector
    {
        CollectMethod method;
        
    }

}
