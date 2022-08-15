namespace ModbusNet.Message
{
    /// <summary>
    /// Modbus TCP 通讯协议 MBAP部分的内容
    /// </summary>
    public class ModbusApplicationProtocolPart
    {
        /// <summary>
        /// 事务Id
        /// </summary>
        public ushort TransactionId { get; set; }

        /// <summary>
        /// 协议标识符，固定为0
        /// </summary>
        public ushort ProtocolId { get; set; } = 0;

        /// <summary>
        /// 数据长度
        /// </summary>
        public ushort Length { get; set; }

        public byte UnitId { get; set; }
    }
}
