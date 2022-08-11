using System;
namespace ModbusNet.Message
{
    public abstract class AbstractReadRequestMessage : BaseMessage
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

