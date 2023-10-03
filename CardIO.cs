using PageBuilder.Cards;
using Newtonsoft.Json.Linq;
using static PageBuilder.Debug;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.IO;

namespace PageBuilder
{
    /// <summary>
    /// 卡片的读写类
    /// </summary>
    public static class CardIO
    {
        public static Dictionary<string, ContentCard> GetCardsFromJson(JArray array)
        {
            var cards = new Dictionary<string, ContentCard>();
            ContentCard card;
            foreach (JObject obj in array.Cast<JObject>())
            {
                card = obj["type"].ToString().ToLower() switch
                {
                    "version" => new VersionCard(obj),
                    "separator" => new Separator(obj),
                    "normal" => new NormalCard(obj),
                    "custom" => new CustomCard(obj),
                    _ => throw new ArgumentException($"未能找到类型为 {obj["type"]} 的卡片")

                };
                if (obj["isswaped"] != null && obj["isswaped"].ToString().ToLower() == "true")
                    card.isSwaped = true;
                else
                    card.isSwaped = false;
                Log($"[卡片IO] 读取卡片: {obj["name"]}", 1);
                cards.Add(card.name,card);
            }
            return cards;
        }

        public static async Task<Dictionary<string, ContentCard>> GetCardsFromJsonAsync(JArray array, string path)
        {
            return await Task.Run(() => GetCardsFromJson(array, path));
        }

        public static Dictionary<string, ContentCard> GetCardsFromJson(JArray array, string path)
        {
            var temp = GetCardsFromJson(array);
            foreach(var card in temp)
                if(card.Value is ResourceBasedCard)
                {
                    (card.Value as ResourceBasedCard).SetResourceBasePath(Path.GetDirectoryName(path));
                    (card.Value as ResourceBasedCard).LoadResourceAsync();
                }
            return temp;
        }

        public static JArray SaveCardToJson(Dictionary<string,ContentCard> cards)
        {
            JArray array = new JArray();
            ContentCard card;
            foreach (var pair in cards)
            {
                Log($"[卡片IO] 保存卡片: {pair.Value.name}", 1);
                card = pair.Value;
                if (card is ResourceBasedCard)
                    (card as ResourceBasedCard).SaveResource();
                array.Add(card.ToJObject(false));
            }
            return array;
        }

        /// <summary>
        /// 保存卡片集合
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="path"></param>
        public static void SaveCard(Dictionary<string, ContentCard> cards,string path)
        {
            try
            {
                File.WriteAllText(path, SaveCardToJson(cards).ToString());
            }
            catch(Exception e)
            {
                LogError(e);
                
            }
        }
    }
}