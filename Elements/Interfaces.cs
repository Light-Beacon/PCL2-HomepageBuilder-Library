namespace PageBuilder.Elements
{
    /// <summary>
    /// 页面元素接口，所有页面元素的基础接口
    /// </summary>
    public interface IPGElement
    {
        /// <summary>
        /// 返回元素的 Xaml 代码
        /// </summary>
        string GetXaml();
    }
    /// <summary>
    /// 使用模板生成的元素接口
    /// </summary>
    public interface ITemplateElement : IPGElement
    {
        DataPairs GetData();
    }
    /// <summary>
    /// 代码行接口
    /// </summary>
    public interface ICodeElement
    {
        ICodeElement Convert(string codeLine);
    }
    /// <summary>
    /// 文本元素接口
    /// </summary>
    interface ITextElement : IPGElement
    {
        /// <summary>
        /// 返回文本元素的文本
        /// </summary>
        string GetText();
    }
    /// <summary>
    /// 行内元素接口
    /// </summary>
    public interface InlineElement : IPGElement { }
    /// <summary>
    /// 行类元素接口
    /// </summary>
    interface LineElement : IPGElement { }
    interface InCardElement : IPGElement { }
    public interface IBulletsElement : IPGElement
    {
        void Join(string code);
    }
}