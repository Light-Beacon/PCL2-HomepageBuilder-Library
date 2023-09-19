using PageBuilder.Elements;
using System;

namespace PageBuilder
{
    /// <summary>
    /// MD代码块 - 因兼容性考虑
    /// </summary>
    internal class CodePart
    {
        public string ToXamlCode()
        {
            switch (type)
            {
                case "Button":
                    return "";//ToButtonXaml();
                case "ButtonGroup":
                    return "";// ToButtonGroupXaml();
                case "ButtonGroupEnd":
                    return "";// Resource.BtnGroupEnd;
                default:
                    throw new ArgumentException();
            }
        }

        public Custom ToElement()
        {
            //TODO REMASTER
            return new Custom(ToXamlCode());
        }

        string type;
        string[] args;
        public CodePart(string code)
        {
            if (code[0] != '<')
                throw new ArgumentException();
            string temp = string.Empty;
            int start = 1;
            string addtion = string.Empty;
            if(code[1] == '/')
            {
                start++;
                addtion = "End";
            }
            for(int i = start; ;i++)
            {
                if (i >= code.Length - 1)
                    throw new ArgumentException();           
                if (code[i+1] == '>' && code[i] != '\\')
                {
                    if (code[i]=='/')
                        break;
                    temp += code[i];
                    break;
                }
                else
                    temp += code[i];
            }
            args = temp.Split(' ');
            type = args[0]+addtion;
        }
    }
}
