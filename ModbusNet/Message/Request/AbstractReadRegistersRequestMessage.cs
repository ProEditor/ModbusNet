using System;
namespace ModbusNet.Message.Request
{
    public abstract class AbstractRegistersRequestMessage : AbstractReadRequestMessage
    {

        public NumericalTypeEnum NumericalType { get; set; }


        /// <summary>
        /// 根据请求的数据类型返回需要实际读取的地址数量（Modbus官方只支持16Bit数据类型，32bit的数据类型需要用2个地址扩充）
        /// </summary>
        /// <returns></returns>
        protected ushort GetActualQuantityCount(ushort rawQuantity)
        {
            ushort actualQuantityCount = 2;
            switch (NumericalType)
            {
                case NumericalTypeEnum.Short:
                    actualQuantityCount = rawQuantity;
                    break;
                case NumericalTypeEnum.Integer:
                case NumericalTypeEnum.Float:
                    actualQuantityCount = (ushort)(rawQuantity * 2);
                    break;
                case NumericalTypeEnum.Double:
                    actualQuantityCount = (ushort)(rawQuantity * 4);
                    break;
            }
            return actualQuantityCount;

        }


        protected ushort GetActualByteCount()
        {
            switch (NumericalType)
            {
                case NumericalTypeEnum.Short:
                default:
                    return (ushort)(2 * Quantity);

                case NumericalTypeEnum.Integer:
                case NumericalTypeEnum.Float:
                    return (ushort)(4 * Quantity);

                case NumericalTypeEnum.Double:
                    return (ushort)(8 * Quantity);
            }

        }
    }
}

