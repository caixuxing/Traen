using Trasen.PaperFree.Domain.Shared.Response;

namespace Trasen.PaperFree.Domain.Shared.CustomException
{
    /// <summary>
    /// 自定义业务异常
    /// </summary>
    public class BusinessException : Exception
    {
        public ResultCode ResultCode { get; private set; }

        public new object? Data { get; set; }

        public string? ShowMessage { get; private set; }

        public MessageType MessageType { get; private set; }

        /// <summary>
        /// 义业务异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorInfo"></param>
        /// <param name="data"></param>
        /// <param name="httpStatusCode"></param>
        public BusinessException(MessageType messageType, string? showMessage, string errorInfo = "", object? data = null, ResultCode resultCode = ResultCode.CUSTOM_ERROR) : base(errorInfo)
        {
            this.MessageType = messageType;
            this.ShowMessage = showMessage;
            ResultCode = resultCode;
            Data = data;
        }
    }
}