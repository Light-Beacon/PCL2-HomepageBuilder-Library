using Newtonsoft.Json.Linq;
using System;
using System.IO;
using static PageBuilder.Debug;

namespace PageBuilder.Cards
{
    public class VersionCard : ResourceBasedCard
    {
        string versionID;
        MCVType versionType;
        string headerImageUrl;
        string wikiUrl;
        string mcbbsUrl;
        string websiteUrl;
        string footnote;
        string title;
        //string fatherVersion;
        bool latest;
        public override string name => versionID;
        public override string type => "Version";
        private string mdCode { get => resText; set { resText = value; } }
        //public override string dataFileName => fatherVersion + Path.DirectorySeparatorChar + versionID + ".md";
        public override string GetData()
        {
            // * -> tab
            var code = mdCode.Replace("\r","");
            var codeLines = code.Split('\n');
            code = string.Empty;
            foreach (var line in codeLines)
            {
                if(line == string.Empty)
                    continue;
                var temp = line;
                string tabs = string.Empty;
                while(temp.Length > 4 && temp[0..4] == "    ")
                {
                    tabs += '\t';
                    temp = temp[4..];
                }
                while(temp.Length > 2 && (temp[0..2] == "**" || temp[0..2] == "\t*"))
                {
                    tabs += '\t';
                    temp =  temp[1..];
                }
                code += tabs + temp + "\r\n";
            }
            mdCode = code;
            return mdCode;
        }
        public bool isLatest
        {
            get { return latest; }
            set
            {
                if (!value)
                {
                    title = versionID;
                    isSwaped = true;
                }  
                else
                {
                    title = "最新" + GetVerTypeStr() + " - " + versionID;
                    isSwaped = false;
                }
                latest = value;
            }
        }

        public override string GetDisplayTitle() => title;
        public MCVType GetVersionType() => versionType;
        public override string GetCode(GenerateArgs args, bool? swap) => GetCard(args,swap).GetXaml();
        public string GetCode(GenerateArgs args, bool? swap,bool latest)
        {
            bool tmp = isLatest;
            this.isLatest = latest;
            string code = GetCode(args, swap);
            this.isLatest = tmp;
            return code;
        }

        private string GetVerTypeStr()
        {
            string versiontype;
            switch (versionType)
            {
                case MCVType.Release:
                    versiontype = "正式版";
                    break;
                case MCVType.Pre_Release:
                    versiontype = "预发布版";
                    break;
                case MCVType.Release_Candidate:
                    versiontype = "候选版";
                    break;
                case MCVType.Snapshot:
                    versiontype = "快照";
                    break;
                case MCVType.Experimental_Snaphot:
                    versiontype = "实验性快照";
                    break;
                case MCVType.Aprilfools_Version:
                    versiontype = "愚人节版本";
                    break;
                default:
                    versiontype = "其它版本";
                    break;
            }
            return versiontype;
        }

        public Elements.Card GetCard(GenerateArgs args,bool? swap)
        {
            //var converter = new MarkdownToXamlConverter();
            //Debug.Log($"正在生成{title}...");
            var converter = new NewConverter();
            var output = new Elements.Card(displayTitle, true, swap);
            if(this.isLatest)
            {
                string str = RandomName.GiveOne();
                output.Add(new Elements.LatestVersionAnimation(str));
                output.Add(new Elements.HeaderImage(versionID.ToUpper(), headerImageUrl,str));
            }
            else
            {
                output.Add(new Elements.HeaderImage(versionID.ToUpper(), headerImageUrl));
            }
            output.Add(converter.Convert(mdCode));//code => elementlist
            output.Add(new Elements.Websites(wikiUrl,mcbbsUrl, websiteUrl));
            if(footnote.Length >0)
                output.Add(new Elements.FootNote(footnote));
            //Debug.Log($"正在生成{title}：结束");
            return output;
        }

        public VersionCard(string _version, MCVType _versionType, string _headerImageUrl, string _mdCode, string _wikiUrl, string _mcbbsUrl, string _websiteUrl, string _footnote, bool isLatest = false,bool swaped = true)
        {
            this.versionID = _version;
            this.versionType = _versionType;
            this.headerImageUrl = _headerImageUrl;
            this.mdCode = _mdCode;
            this.wikiUrl = _wikiUrl;
            this.mcbbsUrl = _mcbbsUrl;
            this.websiteUrl = _websiteUrl;
            this.footnote = _footnote;
            this.latest = isLatest;
            //this.fatherVersion = _fatherVersion;
            title = versionID;
            isSwaped = swaped;
        }

        public static explicit operator JObject(VersionCard card)
        {
            return card.ToJObject();
        }

        public override JObject ToJObject(bool withdata = true)
        {
            JObject jobj = new JObject();
            jobj.Add("name", name);
            jobj.Add("type", "version");
            //jobj.Add("fatherversion", fatherVersion);
            jobj.Add("versionID", versionID);
            jobj.Add("versionType", versionType.ToString());
            jobj.Add("headerImageUrl", headerImageUrl);
            jobj.Add("wikiUrl", wikiUrl);
            jobj.Add("mcbbsUrl", mcbbsUrl);
            jobj.Add("websiteUrl", websiteUrl);
            jobj.Add("footnote", footnote);
            jobj.Add("resource", ResourceFileName);
            if (withdata)
                jobj.Add("mdcode", mdCode);
            return jobj;
        }

        public VersionCard(JObject jobj)
        {
            if (jobj["type"].ToString().ToLower() != type.ToLower())
                throw new Exception("卡片类型不符:" + jobj["versionID"].ToString());
            if (jobj["resource"] == null)
                this.ResourceFileName = jobj["filename"].ToString();
            else
                ResourceFileName = jobj["resource"].ToString();
            this.versionType    = (MCVType)Enum.Parse(typeof(MCVType), jobj["versionType"].ToString());
            this.headerImageUrl = jobj["headerImageUrl"].ToString();
            //this.fatherVersion = jobj["fatherversion"].ToString();
            this.name       = jobj["name"].ToString();
            this.versionID  = jobj["versionID"].ToString();
            this.wikiUrl    = jobj["wikiUrl"].ToString();
            this.mcbbsUrl   = jobj["mcbbsUrl"].ToString();
            this.websiteUrl = jobj["websiteUrl"].ToString();
            this.footnote   = jobj["footnote"].ToString();
            this.latest     = false;
            title = versionID;
            if (jobj["isswaped"] != null && jobj["isswaped"].ToString().ToLower() == "true")
                isSwaped = true;
            else
                isSwaped = false;
        }

        #region MarkdownIO
        
        #endregion
    }
}
