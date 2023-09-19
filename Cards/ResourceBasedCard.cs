using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PageBuilder.Debug;

namespace PageBuilder.Cards
{
    /// <summary>
    /// 以文件资源为基底的卡片抽象类
    /// </summary>
    public abstract class ResourceBasedCard : ContentCard
    {
        protected string resText;
        private string resBasePath_ = Common.CurrentProjectPath;
        private string resFileName_;
        public string ResourceFileName
        {
            get => resFileName_;
            internal set { resFileName_ = value.Replace('\\', Path.DirectorySeparatorChar); }
        }
        public string ResourceFilePath => resBasePath_ + ResourceFileName;

        public void SetResourceBasePath(string path)
        {
            resBasePath_ = path.Replace('\\', Path.DirectorySeparatorChar);
            if (resBasePath_.Last() != Path.DirectorySeparatorChar)
                resBasePath_ += Path.DirectorySeparatorChar;
            //Log($"{resBasePath_}",5);
        }

        public void LoadResource()
        {
            try
            {
                if (File.Exists(ResourceFilePath))
                    resText = File.ReadAllText(ResourceFilePath);
                else
                    throw new FileNotFoundException("未找到需要被读取的资源文件", ResourceFilePath);
            }
            catch (Exception ex)
            {
                LogError(ex, $"读取 {name} 卡片时出现问题：");
            }
        }

        public async void LoadResourceAsync()
        {
            await Task.Run(() => LoadResource());
        }

        public void SaveResource()
        {
            try
            {
                File.WriteAllText(ResourceFilePath, GetData());
            }
            catch(Exception ex)
            {
                LogError(ex, $"保存 {name} 卡片时出现问题：");
            }
        }
    }
}
