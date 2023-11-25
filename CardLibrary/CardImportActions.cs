using Newtonsoft.Json.Linq;
namespace NewsHomepageHelper.PageBuilder.CardLibrary.Actions
{
    internal interface ICardImportAction
    {
        //void Execute(ContentCard card,string target,string value);
        void Execute(JObject cardJobj,string target,string value);
    }

    internal static class ValueFormatter
    {
        static string Format(string text,JObject jObject)
        {
            Regex regex = new Regex(@"\{\$(\w+)}");
            string output = regex.Replace(text, match =>
            {
                string key = match.Group[1].Value;
                if (jObject.ContainsKey(key))
                    return jObject[key];
                else
                    return match.Value;
            })
            return output;
        }
    }

    internal class SetValueAction:ICardImportAction
    {
        public void Execute(JObject cardJobj,string target,string value)
        {
            if(!cardJobj.ContainsKey(target) || cardJobj[target].ToString() != string.Empty)
               cardJobj[target] = ValueFormatter.Format(value,cardJobj); 
        }
    }
    
    internal class ForceSetValueAction:ICardImportAction
    {
        public void Execute(JObject cardJobj,string target,string value)
        {
            cardJobj[target] = ValueFormatter.Format(value,cardJobj); 
        }
    } 
}