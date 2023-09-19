using System;
using static PageBuilder.Debug;
using System.Collections.Generic;
using PageBuilder.Elements;

namespace PageBuilder
{
    /// <summary>
    /// 结构与管理类
    /// </summary>
    public class TemplateManager
    {
        private Dictionary<Type, string> structAssembles = new Dictionary<Type, string>()
        {
            { typeof(LinkText),""},
            { typeof(BulletsLine),""},
            { typeof(BulletsList),""},
            { typeof(HeaderImage),""},
            { typeof(Elements.Image),""},
            { typeof(ImageWithTitle),""},
            { typeof(Websites),""},
            { typeof(FootNote),""},
            { typeof(Card),""},
            { typeof(Title),"" },
            { typeof(PGTextBlock),""},
            { typeof(L_Hint),""},
            { typeof(L_Loading),""},
            { typeof(MainStructure),""},
            { typeof(Separator),"" },
            { typeof(L_Button),""},
            { typeof(LatestVersionAnimation),"" },
            { typeof(HeaderImage2),"" }
        };

        public void RegistElement(string type, string xaml)
        {
            foreach (var element in structAssembles)
            {
                if ("PageBuilder.Elements." + type == element.Key.ToString())
                {
                    Log($"[模板管理] 注册元素: {type}", 1);
                    structAssembles[element.Key] = xaml;
                    return;
                }
            }
            Log($"[模板管理] 无法注册: {type} （未识别的元素名称）", 3);
        }
        /// <summary>
        /// 将元素对象内容与结构内容拼接成XAML
        /// </summary>
        /// <param name="obj">要拼合的元素对象</param>
        /// <param name="type">指定识别的类型</param>
        /// <returns></returns>
        public string AssembleXaml(ITemplateElement obj, Type type = null)
        {
            if (type == null)
                type = obj.GetType();
            string output = structAssembles[type];
            foreach (var dataPair in obj.GetData())
            {
                output = output.Replace($"{{${dataPair.Key}}}", dataPair.Value); //{$data} => data
            }
            return output;
        }

        void InitAssembleDict()
        {
            structAssembles = new Dictionary<Type, string>() { };
            var assembly = typeof(IPGElement).Assembly;//获取当前父类所在的程序集``
            var assemblyAllTypes = assembly.GetTypes();//获取该程序集中的所有类型
            foreach (var itemType in assemblyAllTypes)//遍历所有类型进行查找
            {
                if (!itemType.IsClass)
                    continue;
                var interfaces = itemType.GetInterfaces();
                foreach (var interfacesType in interfaces)
                {
                    if (interfacesType == typeof(IPGElement))
                    {
                        structAssembles.Add(itemType, null);
                        Log($"[模板管理] 注册: {itemType} ", 1);
                    }
                }
            }
        }

        public TemplateManager()
        {
            InitAssembleDict();
        }
    }
}
