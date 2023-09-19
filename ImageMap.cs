using System;
using System.Collections.Generic;

namespace PageBuilder
{
    public class ImageMap
    {
        Dictionary<string,string> mapDict_ = new Dictionary<string,string>();
        public Dictionary<string,string> Map => mapDict_;
        public void AddPair(string filename,string imgUrl)
        {
            //Debug.Log($"[ImageMap] 导入{filename} -> {imgUrl}",6);
            mapDict_.Add(filename, imgUrl);
        }
        public void DelPair(string filename)
        {
            if (mapDict_.ContainsKey(filename))
                mapDict_.Remove(filename);
        }
        /// <summary>
        /// 将整个XAML替换代码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ReplaceXAML(string input)
        {
            //Debug.Log($"尝试替换XAML{mapDict_.Keys.Count}:{input[..200]}",666);
            foreach(var key in mapDict_.Keys)
            {
                input = input.Replace($"https://www.lightbeacon.top/pnh/newsimgs/{key}", mapDict_[key]);
                input = input.Replace($"MapedImage:{key}", mapDict_[key]);
            }
            return input;
        }
        /// <summary>
        /// 尝试获得映射后的地址，如果没有映射就使用原地址
        /// </summary>
        /// <returns></returns>
        public string TryMapString(string input)
        {
            if(mapDict_.ContainsKey(input))
                return mapDict_[input];
            return input;
        }
        const string FILEHEAD = "#FileType HomepageImageMap";
        public ImageMap(string data)
        {
            data = data.Replace("\r\n", "\n");
            if (!data.StartsWith(FILEHEAD))
                throw new ArgumentException("Illegal ImageMap Data");
            data = data[FILEHEAD.Length..];
            //Debug.Log($"[ImageMap] 导入{data}", 24);
            this.mapDict_.Clear();
            var ps = data.Split('\n');
            foreach(var p in ps)
            {
                var s = p.Replace('\t',' ').Split(' ');
                for(int i = 1; i < s.Length; i++)
                    if(s[i].Length > 0)
                        AddPair(s[0], s[i]);
            }
        }
        public string ToFile()
        {
            string output = FILEHEAD;
            foreach(var pair in mapDict_)
                output += $"{pair.Key}\t{pair.Value}\n";
            return output;
        }
        public ImageMap() { }
    }
}
