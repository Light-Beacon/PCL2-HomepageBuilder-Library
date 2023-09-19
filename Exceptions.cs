using static PageBuilder.Debug;

namespace PageBuilder
{
    /// <summary>
    /// NewsHomepageHelper 的异常基类，继承于 System.Exception
    /// </summary>
    public class Exception : System.Exception
    {
        public Exception():base()
        {
            Log($"{base.Source}抛出异常：{this.GetType()}", 8, this.GetHashCode(), this.StackTrace);
        }
        public Exception(string message):base(message)
        {
            Log($"{base.Source}抛出异常：{this.GetType()}:{message}", 8, this.GetHashCode(), this.StackTrace);
        }
        public Exception(string message, System.Exception innerException):base(message, innerException)
        {
            Log($"{base.Source}抛出异常：{this.GetType()}:{message} 内在错误代码：{innerException.GetHashCode():X}", 8, this.GetHashCode(), this.StackTrace);
        }
    }
    /// <summary>
    /// 未绑定文件异常
    /// </summary>
    public class NotBindedWithFileException : Exception
    {
        public NotBindedWithFileException() : base() { }
        public NotBindedWithFileException(string message) : base(message) { }
        public NotBindedWithFileException(string message, Exception innerException) : base(message, innerException) { }
    }
    /// <summary>
    /// 重复注册异常
    /// </summary>
    public class ReregistException:Exception
    {
        public ReregistException() : base() { }
        public ReregistException(string message) : base(message) { }
        public ReregistException(string message,object reRegistObject):base(message)
        {
            Log($"错误目标:{reRegistObject.GetHashCode():X},类型为：{reRegistObject.GetType()}", 8);
        }
        public ReregistException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CodeNotFinishException:Exception
    {
        public CodeNotFinishException() : base() { }
        public static void TODO()
        {
            throw new CodeNotFinishException();
        }
    }
}