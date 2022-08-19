using ModbusNet.Enum;
namespace ModbusNet
{

    public class TcpModbusResponse
    {

        public TcpModbusResponse(ushort transactionId, ExceptionCodeDefinition exceptionCode)
        {
            TransactionId = transactionId;
            ExceptionCode = exceptionCode;
        }

        public TcpModbusResponse(ushort transactionId, object result)
        {
            TransactionId = transactionId;
            Result = result;
        }


        /// <summary>
        /// 从站返回的事务Id
        /// </summary>
        public ushort TransactionId { get; }

        /// <summary>
        /// 向从站发出指定是否成功
        /// </summary>
        public bool Success => ExceptionCode == 0;

        /// <summary>
        /// 异常码
        /// </summary>
        public ExceptionCodeDefinition ExceptionCode { get; }

        public object Result { get; }
    }


}
