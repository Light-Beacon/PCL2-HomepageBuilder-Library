using PageBuilder.Cards;
using static PageBuilder.Debug;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
#if Client
using NewsHomepageHelper.View;
using WpfApp1;
using GitHelper;
#endif

namespace PageBuilder
{
    public delegate bool AddCardToLibDel(ContentCard card);
    public static class Common
    {
        public static string page404 = "404";
        public static string CurrentProjectPath => CurrentProject.ProjectPath + Path.DirectorySeparatorChar;
        public static HomePageProject CurrentProject;
        public static TemplateManager templateManager => CurrentProject.templateManager;
        public static AddCardToLibDel addCardToLib;
    }

    public class HomePageProject
    {
#if Client
        public Repo repo;
#endif
        public ImageMap imageMap = new ImageMap();
        public int project_version;
        public TemplateManager templateManager;
        string animationPartPath;
        public string AnimationPartPath
        {
            get { return animationPartPath.Replace("~", ProjectPath); }
            set { animationPartPath = value; }
        }
        string stylePartPath;
        public string StylePartPath
        {
            get { return stylePartPath.Replace("~", ProjectPath); }
            set { stylePartPath = value; }
        }
        string markPartPath;
        public string MarkPartPath
        {
            get { return markPartPath.Replace("~", ProjectPath); }
            set { markPartPath = value; }
        }
        string imageMapPath = "";
        public string ImageMapPath
        {
            get { return imageMapPath.Replace("~", ProjectPath); }
            set { imageMapPath = value; }
        }
        /// <summary>
        /// 输出目录列表
        /// </summary>
        public List<string> OutputPath;
        /// <summary>
        /// 卡片资源库
        /// </summary>
        public LibManager libs;
        /// <summary>
        /// 页面列表
        /// </summary>
        public List<IPage> pages;
        /// <summary>
        /// 项目文件位置
        /// </summary>
        public string ProjectFilePath;
        /// <summary>
        /// 项目文件夹位置
        /// </summary>
        public string ProjectPath
        {
            get
            {
                if (ProjectFilePath != null)
                    return Path.GetDirectoryName(ProjectFilePath);
                else
                    return null;
            }
        }
        public HomePageProject(string projectFilePath)
        {
            ProjectFilePath = projectFilePath;
            Init();
        }
        /// <summary>
        /// 初始化项目
        /// </summary>
        private void Init()
        {
            templateManager = new TemplateManager();
            OutputPath = new List<string>();
            libs = new LibManager();
            pages = new List<IPage>();
            Common.CurrentProject = this;
        }
        #region Load
        const string PROJECT_VER_HEADER = "$project_version=";
        /// <summary>
        /// 加载项目（但不加载页面）
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public void LoadProject()
        {
            if (ProjectFilePath == null)
                throw new NotBindedWithFileException("尝试加载项目时出现未绑定文件异常。");
            string[] fileContent = File.ReadAllLines(ProjectFilePath);
            int mode = 0;
            GenerateManager.Clear();
            if(fileContent[0].StartsWith(PROJECT_VER_HEADER))
                project_version = int.Parse(fileContent[0].Substring(PROJECT_VER_HEADER.Length));
            else
                project_version = 0;
            Log($"[工程|加载] 当前工程文件版本:{project_version}", 5);
            List<Action> afterLoadedActions = new List<Action>();
            foreach (string FileContentItem in fileContent)
            {
                string content = FileContentItem.Replace(" ", "").Replace('/', Path.DirectorySeparatorChar);
                if (content == "")
                    continue;
                if (FileContentItem[0] == '#')
                {
                    switch (FileContentItem[1..])
                    {
                        case "libs":
                            mode = 1;
                            break;
                        case "base":
                            mode = 2;
                            break;
                        case "output":
                            mode = 3;
                            break;
                        case "pages":
                            mode = 4;
                            break;
                        default:
                            mode = 0;
                            break;
                    }
                    continue;
                }
                switch (mode)
                {
                    case 0:
                        //无状态时跳过
                        break;
                    case 1:
                        //#lib - 导入至资源库
                        content = content.Replace("~", ProjectPath).Replace('\\',Path.DirectorySeparatorChar);
                        Log($"[工程|加载] 导入: {content} 至资源库", 3);
                        if (!File.Exists(content))
                            throw new Exception($"文件不存在：{content}");
                        afterLoadedActions.Add(new Action(() => libs.Add(new CardLib(content))));
                        break;
                    case 2:
                        //#base - 导入至基础库
                        string target = string.Empty;
                        string value = string.Empty;
                        int arr = 0;
                        foreach (char chr in content)
                        {
                            if (chr == '=')
                            {
                                value = content.Remove(0, arr + 1);
                                break;
                            }
                            target += chr;
                            arr++;
                        }
                        value = value.Replace('\\', Path.DirectorySeparatorChar);
                        if (!File.Exists(value.Replace("~", ProjectPath)))
                            throw new Exception($"文件不存在：{value}");
                        switch (target)
                        {
                            case "anim":
                                Log($"[工程|加载] 导入动画文件：{value}", 3);
                                AnimationPartPath = value;
                                break;
                            case "foot":
                                Log($"[工程|加载] 因弃用，略过脚注文件：{value}", 3);
                                break;
                            case "styl":
                                Log($"[工程|加载] 导入主题文件：{value}", 3);
                                StylePartPath = value;
                                break;
                            case "mark":
                                Log($"[工程|加载] 导入头标文件：{value}", 3);
                                MarkPartPath = value;
                                break;
                            case "imagemap":
                                Log($"[工程|加载] 导入图片链接映射文件：{value}", 3);
                                ImageMapPath = value;
                                imageMap = new ImageMap(File.ReadAllText(ImageMapPath));
                                break;
                            default:
                                Log($"未定义的参数名:{target},可能是更新版本？", 6);
                                break;
                        }
                        break;
                    case 3:
                        //#output - 输出目录
                        OutputPath.Add(content.Replace("~", ProjectPath));
#if Client
                        //ThreadHelper.RunInNewThread(() => repo = Repo.LoadFromDire(OutputPath[0]), "EXEGITCMD");
#endif
                        break;
                    case 4:
                        //#pages - 注册页面
                        string tmp = content.Replace("~", ProjectPath).Replace('\\', Path.DirectorySeparatorChar);
                        if(tmp.EndsWith(".json"))
                            pages.Add(new XamlPage(ProjectPath, tmp));
                        if (tmp.EndsWith(".xaml"))
                            pages.Add(new RawPage(tmp));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            foreach(var act in afterLoadedActions)
            {
                act.Invoke();
            }
            List<string> files = new List<string>(Directory.GetFiles($"{ProjectPath}{Path.DirectorySeparatorChar}Templates"));
            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                bool bk = true;
                foreach (var c in name)
                {
                    if (c >= 'a')
                    {
                        bk = false;
                    }
                }
                if (bk)
                    continue;
                templateManager.RegistElement(name, File.ReadAllText(file));
            }
        }
        /// <summary>
        /// 加载全部页面
        /// </summary>
        public void LoadAllPages()
        {
            foreach (IPage page in pages)
            {
                page.Load();
            }
        }
        /// <summary>
        /// 加载全部内容
        /// </summary>
        public void LoadAll()
        {
            DateTime start_time = DateTime.Now;
            DateTime part_start_time = DateTime.Now;
            Log("————————————————————————", 5);
            Log("[工程|加载] 开始加载", 5);
            Init();
            Log($"[工程|加载] 加载初始化用时{(DateTime.Now - part_start_time).TotalSeconds}，总计用时{(DateTime.Now - start_time).TotalSeconds}", 5);
            part_start_time = DateTime.Now;
            LoadProject();
            Log($"[工程|加载] 加载项目用时{(DateTime.Now - part_start_time).TotalSeconds}，总计用时{(DateTime.Now - start_time).TotalSeconds}", 5);
            part_start_time = DateTime.Now;
            LoadAllPages();
            Log($"[工程|加载] 加载页面用时{(DateTime.Now - part_start_time).TotalSeconds}，总计用时{(DateTime.Now - start_time).TotalSeconds}", 5);
            Log("[工程|加载] 加载完毕", 5);
            Log("————————————————————————", 5);
        }
        #endregion
        #region Save
        /// <summary>
        /// 保存所有卡片
        /// </summary>
        public void SaveAllCards() => libs.SaveAll();
        /// <summary>
        /// 保存所有页面
        /// </summary>
        public void SaveAllPages()
        {
            foreach (var page in pages)
            {
  
                Log("[工程|保存] 开始保存页面", 1);
                page.Save();
                Log("[工程|保存] 保存页面完毕！", 1);
               
            }
        }
        /// <summary>
        /// 保存项目（但不保存其它内容）
        /// </summary>
        public void SaveProject() => File.WriteAllText(ProjectFilePath, ToString());
        public void SaveBase()
        {
            if(ImageMapPath.Length > 0)
            {
                File.WriteAllText(ImageMapPath, imageMap.ToFile());
            }
        }
        public void SaveAll()
        {
            Log("————————————————————————", 5);
            Log("[工程|保存] 开始保存", 5);
            SaveAllPages();
            SaveAllCards();
            SaveProject();
            Log("[工程|保存] 保存完毕", 5);
            Log("————————————————————————", 5);
        }
        #endregion
        #region Generate

#if Client
        public void GenerateAll()
        {
            ProgressBarWindow progressBarWindow = new ProgressBarWindow("生成代码中", pages.Count+1, (sender, e) =>
            {
                DateTime time = DateTime.Now;
                BackgroundWorker bgWorker = sender as BackgroundWorker;
                bgWorker.ReportProgress(0, $"保存项目中");
                SaveAll();
                bgWorker.ReportProgress(1, $"初始化中");
                int endNumber = 0;
                if (e.Argument != null)
                {
                    endNumber = (int)e.Argument;
                }
                foreach (var page in pages)
                {
                    GeneratePage(page, bgWorker);
                }
                Log("生成完毕", 2);
                Log("生成代码花费时间：" + (DateTime.Now - time).ToString(), 3);
                //Thread.Sleep(500);
            });
            progressBarWindow.ShowDialog();
            MessageBox.Show("已生成文件");
        }

        public void GeneratePage(IPage page,BackgroundWorker worker)
        {   
            if (!(page is XamlPage))
                return;
            var xamlpage = page as XamlPage;
            new Thread(() =>
            {
                string code = string.Empty;
                App.Current.Dispatcher.Invoke(() =>
                {
                    code = GenerateXAMLCode(xamlpage,worker);
                });
                //Clipboard.SetText(code);
                Log($"开始生成：{page.Name}", 5);
                foreach (string outputpath in OutputPath)
                {
                    var p = Path.GetDirectoryName($"{outputpath}{Path.DirectorySeparatorChar}{xamlpage.outputPath}");
                    if (!Directory.Exists(p))
                        Directory.CreateDirectory(p);
                    File.WriteAllText($"{outputpath}{Path.DirectorySeparatorChar}{xamlpage.outputPath}", code);
                }
            }).Start();
        }

        string GenerateXAMLCode(XamlPage page, BackgroundWorker bgWorker)
        {
            int taskNow = 0;
            var result = string.Empty;
            DateTime time = DateTime.Now;
            result = page.GenerateCode(new GenerateArgs(), (MarkPartPath, AnimationPartPath, StylePartPath));
            bgWorker.ReportProgress(1, $"生成{page.Name}完毕！");
            Log("生成完毕", 2);
            Log("生成代码花费时间：" + (DateTime.Now - time).ToString(), 3);
            result += $"\n<!-- 由 新闻主页构建器 生成，生成时间：{DateTime.Now:G} -->";
            return result;
        }
#else
        public void GenerateAllFiles(string? outputpath = null)
        {
            foreach (var page in pages)
            {
                GenerateFile(page, outputpath);
            }
        }
        public void GenerateFile(IPage page,string? outputpath)
        {
            if (!(page is XamlPage))
                return;
            var xamlpage = page as XamlPage;
            new Thread(() =>
            {
                string code = string.Empty;
                code = GenerateXAMLCode(xamlpage, new GenerateArgs());
                //Clipboard.SetText(code);
                Log($"开始生成：{page.Name}", 5);
                if(outputpath == null)
                    foreach (string path in OutputPath)
                        WriteFile(path,xamlpage,code);
                else
                    WriteFile(outputpath, xamlpage, code);
            }).Start();
        }
        public void WriteFile(string path,XamlPage xamlpage,string code)
        {
            var p = Path.GetDirectoryName($"{path}{Path.DirectorySeparatorChar}{xamlpage.outputPath}");
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);
            File.WriteAllText($"{path}{Path.DirectorySeparatorChar}{xamlpage.outputPath}", code);
        }
        public void GenerateAll()
        {
            //SaveAll();
            foreach (var page in pages)
            {
                GeneratePage(page);
            }
        }
        public string CheckPage(string pageName)
        {
            foreach (var page in pages)
            {
                if (page.Name == null)
                    continue;
                if (page.Name.ToLower() == pageName.ToLower())
                    return page.Name;
                else
                    foreach (var a in page.Alias)
                        if (a.ToString() == pageName)
                            return page.Name;
            }
            return Common.page404;
        }
        public string GeneratePage(string name)
        {
            return GeneratePage(new GenerateArgs(name));
        }
        public string GeneratePage(GenerateArgs args)
        {
            foreach (var page in pages)
                if (page.Name == args.pagename)
                    return GenerateXAMLCode(page,args);
            return string.Empty;
        }
        public string GeneratePageJson(string name)
        {
            foreach (var page in pages)
                if (page.Name == name)
                    return GeneratePageJson(page);
            return string.Empty;
        }
        public string GeneratePageJson(IPage page)=>page.GetPageJson();
        public string GeneratePage(IPage page) => GenerateXAMLCode(page,new GenerateArgs(page.Name));
        public string GeneratePage(IPage page,GenerateArgs args) => GenerateXAMLCode(page,args);
        public string GenerateXAMLCode(IPage page,GenerateArgs args)
        {
            return page.GenerateCode(args,(MarkPartPath, AnimationPartPath,StylePartPath));
        }
#endif
        #endregion
        public ContentCard GetCard(CardRef cardref) => libs.GetCard(cardref);

        private HomePageProject() { }
        public HomePageProject Copy(bool includeLibs = true, bool includeBases = true)
        {
            var newProject = new HomePageProject();
            if (includeBases)
            {
                newProject.AnimationPartPath = animationPartPath;
                newProject.StylePartPath = stylePartPath;
            }
            if (includeLibs)
            {
                newProject.libs = libs;
            }
            newProject.OutputPath = OutputPath;
            newProject.ProjectFilePath = ProjectFilePath;
            return newProject;
        }
        public override string ToString()
        {
            string output = $"{PROJECT_VER_HEADER}{project_version}\n";
            output += "#libs\n";
            foreach (var libpair in libs.CardLibs)
                output += libpair.Value.LibPath.Replace(ProjectPath, "~").Replace(Path.DirectorySeparatorChar,'\\') + "\n";
            output += "\n";
            output += "#output\n";
            foreach (string str in OutputPath)
                output += str.Replace(ProjectPath, "~").Replace(Path.DirectorySeparatorChar, '\\') + "\n";
            output += "\n";
            output += "#base\n";
            if (MarkPartPath != null)
                output += $"mark = {markPartPath}\n";
            if (AnimationPartPath != null)
                output += $"anim = {animationPartPath}\n";
            if (StylePartPath != null)
                output += $"styl = {stylePartPath}\n";
            if (ImageMapPath != null)
                output += $"imagemap = {imageMapPath}\n";
            output += "\n";
            output += "#pages\n";
            foreach (var page in pages)
                output += page.PagePath.Replace(ProjectPath, "~").Replace(Path.DirectorySeparatorChar, '\\') + "\n";
            return output;
        }
    }

    public struct GenerateArgs
    {
        public string pagename;
        public bool jsonFile;
        public bool CDNMode;
        public bool SwapAllCard;
        public GenerateArgs(string _page) { pagename = _page; SwapAllCard = false;CDNMode = false;jsonFile = false; }
        public GenerateArgs(string _page,string configStr)
        {
            int bytes;
            pagename = _page;
            SwapAllCard = false;
            CDNMode = false;
            jsonFile = false;
            try
            {
                bytes = (int)Convert.ToUInt32(configStr);
            }
            catch
            {
                return;
            }
            SwapAllCard = (bytes & 1) == 1;
        }
    }
}