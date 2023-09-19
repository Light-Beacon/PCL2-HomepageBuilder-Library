using PageBuilder.Cards;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static PageBuilder.Debug;
using static PageBuilder.XamlPage;
using Path = System.IO.Path;

namespace PageBuilder
{
    /// <summary>
    /// 卡片库管理类
    /// </summary>
    public class LibManager
    {
        private Dictionary<string,CardLib> _cardLibs = new Dictionary<string, CardLib>();
        /// <summary>
        /// 卡片库字典
        /// </summary>
        public Dictionary<string, CardLib> CardLibs
        {
            get
            {
                var tmp = new Dictionary<string, CardLib>();
                foreach(var libpair in _cardLibs)
                    if(!(libpair.Value is VirtualCardLib))
                        tmp.Add(libpair.Key, libpair.Value);
                return tmp;
            }
        }
        delegate void RegistDelegate(CardLib lib,bool forceRigst = false);
        /// <summary>
        /// 获取卡片的引用
        /// </summary>
        /// <param name="name">卡片库:卡片名</param>
        /// <returns>卡片的引用</returns>
        /// <exception cref="ArgumentException">不符合命名规则</exception>
        public ContentCard GetCard(CardRef cardRef)
        {
            return GetCard(cardRef.LibName, cardRef.CardName);
        }
        /// <summary>
        /// 获取卡片的引用
        /// </summary>
        /// <param name="libname">卡片库</param>
        /// <param name="name">卡片名</param>
        /// <returns>卡片的引用</returns>
        public ContentCard GetCard(string libname,string name)
        {
            try
            {
                return _cardLibs[libname].GetCard(name);
            }
            catch(Exception e)
            {
                Log($"[库管理器] 获取卡片 {libname}.{name} 失败:{e.Message}",2);
                return null;
            }
        }
        /// <summary>
        /// 向库管理器添加库
        /// </summary>
        /// <param name="lib">要添加的库</param>
        public void Add(CardLib lib)
        {
            if (!_cardLibs.ContainsKey(lib.Name))
                _cardLibs.Add(lib.Name, lib);
            else
                Log($"{lib.Name}库已存在", 5, 1);
        }
        /// <summary>
        /// 移除某个库
        /// </summary>
        /// <param name="id">库的ID</param>
        void Remove(string name)
        {
            _cardLibs.Remove(name);
        }
        /// <summary>
        /// 移除某个库
        /// </summary>
        /// <param name="lib">库</param>
        void Remove(CardLib lib)
        {
            _cardLibs.Remove(lib.Name);
        }
        /// <summary>
        /// 保存所有库里的卡片
        /// </summary>
        public void SaveAll()
        {
            Log($"[库管理器] 开始保存所有库文件", 1);
            foreach (var lib in _cardLibs)
            {
                Log($"[库管理器] 正在保存库：{lib.Value.LibPath}", 1);
                lib.Value.Save();
            }
            Log($"[库管理器] 所有库文件保存完毕！", 1);
        }
        public CardLib LoadLibFromFile(string path) => new CardLib(path);
        public LibManager()
        {
            VirtualLib = new VirtualCardLib();
            Add(VirtualLib);
        }
        /// <summary>
        /// 运行时的虚拟库
        /// </summary>
        public VirtualCardLib VirtualLib { get; set; }
    }

    public class CardLib
    {
        public readonly Guid UUID;
        /// <summary>
        /// 卡片库名称
        /// </summary>
        public virtual string Name => _name;
        private string _name;
        protected Dictionary<string, ContentCard> cards;
        public List<ContentCard> ToList()
        {
            var temp = new List<ContentCard>();
            foreach (var cardPair in cards)
                temp.Add(cardPair.Value);
            return temp;
        }
        public List<CardRef> ToCRList()
        {
            var temp = new List<CardRef>();
            foreach (var cardPair in cards)
                temp.Add($"{Name}:{cardPair.Value.name}");
            return temp;
        }
        public readonly string LibPath;
        public virtual string DisplayName
        {
            get { return Path.GetFileNameWithoutExtension(LibPath); }
        }
        internal CardLib() { }
        public CardLib(string path)
        {
            UUID = Guid.NewGuid();
            LibPath = path;
            Load();
        }
        public async void LoadAsync()
        {
            await Task.Run(() => Load());
        }
        public virtual void Load()
        {
            try
            {
                JObject jobj = JObject.Parse(File.ReadAllText(LibPath));
                _name = jobj["name"].ToString();
                cards = CardIO.GetCardsFromJson((JArray)jobj["cards"], LibPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public virtual void Save()
        {
            try
            {
                JObject jobj = new JObject();
                jobj["name"] = Name;
                jobj["cards"] = CardIO.SaveCardToJson(cards);
                File.WriteAllText(LibPath, jobj.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public virtual void Add(ContentCard card)
        {
            if(card is ResourceBasedCard)
            {
                (card as ResourceBasedCard).SetResourceBasePath(Path.GetDirectoryName(LibPath)+Path.DirectorySeparatorChar);
                if((card as ResourceBasedCard).ResourceFileName == null)
                {
                    if (card is VersionCard || card is NormalCard)
                        (card as ResourceBasedCard).ResourceFileName = card.name + ".md";
                    else if (card is CustomCard)
                        (card as ResourceBasedCard).ResourceFileName = card.name + ".xaml";
                }
            }
            cards.Add(card.name, card);
        }
        public virtual void Remove(string name)
        {
            cards.Remove(name);
        }
        public virtual ContentCard GetCard(string name)
        {
            return cards[name];
        }
    }

    public class VirtualCardLib: CardLib
    {
        public override string Name => "Virtual";
        public override string DisplayName => Name;
        public override void Load() { }
        public override void Save() { }
        public override void Remove(string name) { }
        public override ContentCard GetCard(string name)
        {
            if(!cards.ContainsKey(name))
                return new Separator(name);
            else
                return cards[name];
        }
        public VirtualCardLib() { cards = new Dictionary<string, ContentCard>(); }

    }
}
