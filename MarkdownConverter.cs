using PageBuilder.Elements;
using System.Collections.Generic;
using System.Linq;
using static PageBuilder.Debug;

namespace PageBuilder
{
    public class NewConverter
    {
        private int GetElementLevel(string str, char targetchr)
        {
            int level = 0;
            foreach (char chr in str)
            {
                if (chr == targetchr)
                    level++;
                else
                {
                    if (chr == ' ')
                        break;
                    else
                        return 0;
                }
            }
            return level;
        }

        public List<IPGElement> Convert(string str)
        {
            List<IPGElement> elements = new List<IPGElement>();
            //string xamlCode = string.Empty;
            string buffer = string.Empty;
            bool skipnext = false;//skip \n behind \r
            str += '\n';
            foreach (char ch in str)
            {
                if (skipnext)
                {
                    skipnext = false;
                    if (ch == '\n')
                        continue;
                }
                if ((ch == '\r' || ch == '\n') && buffer.Length > 1)
                {
                    if (ch == '\r')
                        skipnext = true;
                    int level = 0;
                    switch (buffer[0])
                    {
                        case '#':
                            string content;
                            //Title
                            level = GetElementLevel(buffer, '#');
                            if (level == 1)
                                Log("存在一级标题", 5);
                            if (level > 4)
                                Log("标题等级大于4", 5);
                            content = buffer.Remove(0, level + 1);
                            elements.Add(new Title(level, content));
                            break;
                        case ' ':
                            if (buffer[..4] == "    ")
                                goto case '+';
                            else
                                goto default;
                        case '+':
                        case '-':
                        case '*':
                        case '\t':
                            //List
                            if (buffer.Replace(" ","").Contains("|http"))
                            {
                                if (elements.Count == 0 || !(elements.Last() is ButtonGroup) && elements.Last() is Title)
                                    elements.Add(new ButtonGroup((elements.Last() as Title).Content));
                                var sp = new char[] { '|' };
                                var args = buffer.Replace(" ","")[1..].Split(sp, 2);
                                (elements[elements.Count - 1] as ButtonGroup).Add(args[0], args[1]);
                                /* 
                                 ### 方块
                                 * 蛙明灯|https:www.fandom.com/....png
                                 * 草方块|https:www.fandom.com/....png
                                 */
                            }
                            else
                            {
                                if (elements.Count == 0 || !(elements.Last() is BulletsList))
                                    elements.Add(new BulletsList());
                                (elements[elements.Count - 1] as BulletsList).Join(buffer);
                            }
                            break;
                        case '!':   //图片
                            //alt
                            string alt = string.Empty;
                            if (buffer[1] != '[')
                                goto default;
                            for (int i = 1; buffer[i] != ']'; i++)
                            {
                                if (i >= buffer.Length - 1)
                                    goto default;
                                alt += buffer[i];
                            }
                            if (buffer[alt.Length + 2] != '(')
                                goto default;
                            string link = string.Empty;
                            string title = string.Empty;
                            for (int i = alt.Length + 3; buffer[i] != ')'; i++)
                            {
                                if (i >= buffer.Length - 1)
                                    goto default;
                                if (buffer[i] == ' ' && i < buffer.Length - 2 && buffer[i + 1] == '\"')
                                {
                                    title = string.Empty;
                                    i += 2;
                                    while (buffer[i] != '\"')
                                    {
                                        if (i >= buffer.Length - 1)
                                            goto default;
                                        title += buffer[i++];
                                    }
                                    break;
                                }
                                else
                                    link += buffer[i];
                            }
                            if (title != string.Empty)
                                elements.Add(new ImageWithTitle(link,title));
                            else
                                elements.Add(new Image(link));
                            break;
                        case '<':   //代码段
                            elements.Add(new CodePart(buffer).ToElement());
                            break;
                        case 'W':
                        case 'w':
                            if (buffer[1] == ' ')
                                elements.Add(new L_Hint(buffer[2..],true));
                            else
                                goto default;
                            break;
                        case 'I':
                        case 'i':
                            if(buffer[1] == ' ')
                                elements.Add(new L_Hint(buffer[2..],false));
                            else
                                goto default;
                            break;
                        case '?':
                            var q = buffer.Split('"');
                            List<string> lineargs = new List<string>();
                            for(int i = 0; i < q.Length; i++)
                            {
                                if(i%2 == 1)
                                    lineargs.Add(q[i]);
                                else
                                {
                                    while (q[i].EndsWith(" "))
                                        q[i] = q[i][..^1];
                                    while (q[i].StartsWith(" "))
                                        q[i] = q[i][1..];
                                    if (q[i].Length == 0)
                                        continue;
                                    var sub = q[i].Split(' ');
                                    foreach(var s in sub)
                                        lineargs.Add(s);
                                }
                            }
                            if (buffer[1] == ' ')
                                switch (lineargs[1])
                                {
                                    case "B":
                                    case "Button":
                                    case "按钮":
                                        elements.Add(new L_Button(lineargs[2], lineargs[3],(lineargs.Count() >= 5) ? lineargs[4]:"",(lineargs.Count() >= 6) ? lineargs[5] : ""));
                                        break;
                                }
                            break;  
                        case '\n':  //忽略换行
                            break;
                        default:
                            elements.Add(new Line(buffer));
                            break;
                    }
                    buffer = string.Empty;
                }
                else
                {
                    buffer += ch;
                }
            }
            return elements;
        }

    }
}