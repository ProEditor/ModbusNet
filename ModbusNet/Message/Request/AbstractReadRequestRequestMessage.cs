using System;
namespace ModbusNet.Message.Request
{
    public abstract class AbstractReadRequestRequestMessage: BaseRequestMessage
    {

        /// <summary>
        /// 开始读取的地址
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// 需要读取的数量
        /// </summary>
        public ushort Quantity { get; set; }


    }
}
