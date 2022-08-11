using System;
namespace ModbusNet
{

    public delegate void TcpModbusCallback(IAsyncResult ar);


    public class TcpModbusResponse<T>
    {
        public TcpModbusResponse()
        {

        }

        /// <summary>
        /// 从站返回的事务Id
        /// </summary>
        public ushort TransactionId;


        /// <summary>
        /// 向从站发出指定是否成功
        /// </summary>
        public bool Success { get; set; }


        public ushort ExceptionCode { get; set; }

        public T Result { get; set; }
    }
}

