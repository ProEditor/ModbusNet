using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ModbusNet
{
    /// <summary>
    /// Modbus TCP请求内容
    /// MBAP报文头( 事务处理标识(2字节)	协议标识(2字节)	长度(2字节)	单元标识符(1字节))+请求内容
    /// </summary>
    public class TcpModbusRequest
    {
        /// <summary>
        /// 事务Id编号
        /// </summary>
        public ushort TransactionId { get; set; }

        /// <summary>
        /// 保持寄存器数据类型
        /// </summary>
        public NumericalTypeEnum NumericalType { get; set; } = NumericalTypeEnum.Short;

        /// <summary>
        /// 单元标识符
        /// </summary>
        public byte UnitId { get; set; } = 0;

        /// <summary>
        /// 请求的功能码
        /// </summary>
        public byte FunctionCode { get; set; }

        /// <summary>
        /// 写入到从站的数据(单个数据)
        /// </summary>
        public object WriteData { get; set; }

        /// <summary>
        /// 开始地址
        /// </summary>
        public ushort Address { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        public ushort Quantity { get; set; }


        /// <summary>
        /// 读写多个寄存器时的读取开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort ReadStartingAddress { get; set; }


        /// <summary>
        /// 读取的数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort ReadQuantity { get; set; }

        /// <summary>
        /// 读写多个寄存器时的写入时的开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort WriteStartingAddress { get; set; }

        /// <summary>
        /// 写入数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort WriteQuantity { get; set; }

        public Socket InnerSocket { get; set; }



        public void Request()
        {

            //实际发送的字节数量
            int actualSendNums;

            //理应发送的字节数量
            int shouldSendNums;

            IntPtr nativePtr = IntPtr.Zero;
            Span<byte> reqSpan = null;


            //写入多个线圈量时MBAP中“后续字节数量”
            int multipleWriteCoilsRemainByteNum = 0;
            int multipleWriteRegistersRemainByteNum = 0;
            int multipleReadWriteRegistersRemainByteNum = 0;
            try
            {
                switch (FunctionCode)
                {
                    case FunctionCodeDefinition.READ_COILS:
                    case FunctionCodeDefinition.READ_DISCRETE_INPUTS:
                    case FunctionCodeDefinition.READ_HOLDING_REGISTERS:
                    case FunctionCodeDefinition.READ_INPUT_REGISTERS:
                    case FunctionCodeDefinition.WRITE_SINGLE_COIL:
                    case FunctionCodeDefinition.WRITE_SINGLE_REGISTER:
                        shouldSendNums = 12;
                        nativePtr = Marshal.AllocHGlobal(shouldSendNums);
                        unsafe
                        {
                            reqSpan = new Span<byte>(nativePtr.ToPointer(), shouldSendNums);
                        }
                        break;
                    case FunctionCodeDefinition.WRITE_MULTIPLE_COILS:
                        shouldSendNums = 7 + 1 + 2 + 2 + 1;//1字节的功能码，2字节的开始地址，2字节的数量，1字节的字节数量
                        multipleWriteCoilsRemainByteNum = 1 + 1 + 2 + 2 + 1;//1字节的单元标识符，1字节的功能码，2字节的开始地址，2字节的数量，1字节的地址数量
                        int byteNum = Quantity / 8;

                        if (Quantity % 8 == 0)
                        {
                            shouldSendNums += byteNum;
                            multipleWriteCoilsRemainByteNum += byteNum;
                        }
                        else
                        {
                            shouldSendNums += byteNum + 1;
                            multipleWriteCoilsRemainByteNum += byteNum + 1;
                        }
                        nativePtr = Marshal.AllocHGlobal(shouldSendNums);
                        unsafe
                        {
                            reqSpan = new Span<byte>(nativePtr.ToPointer(), shouldSendNums);
                        }
                        break;
                    case FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS:

                        //1字节的功能码，2字节的开始地址，2字节的数量，1字节的字节数量
                        shouldSendNums = 7 + 1 + 2 + 2 + 1;

                        //1字节的单元标识符，1字节的功能码，2字节的开始地址，2字节的数量，1字节的地址数量
                        multipleWriteRegistersRemainByteNum = 1 + 1 + 2 + 2 + 1;

                        switch (NumericalType)
                        {
                            case NumericalTypeEnum.Short:
                                var byteNum1 = 2 * Quantity;
                                shouldSendNums += byteNum1;
                                multipleWriteRegistersRemainByteNum += byteNum1;
                                break;

                            case NumericalTypeEnum.Integer:
                            case NumericalTypeEnum.Float:
                                var byteNum2 = 4 * Quantity;
                                shouldSendNums += byteNum2;
                                multipleWriteRegistersRemainByteNum += byteNum2;
                                break;

                            case NumericalTypeEnum.Double:
                                var byteNum3 = 8 * Quantity;
                                shouldSendNums += byteNum3;
                                multipleWriteRegistersRemainByteNum += byteNum3;
                                break;
                        }
                        nativePtr = Marshal.AllocHGlobal(shouldSendNums);
                        unsafe
                        {
                            reqSpan = new Span<byte>(nativePtr.ToPointer(), shouldSendNums);
                        }
                        break;

                    case FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS:

                        break;
                }
                switch (FunctionCode)
                {
                    case FunctionCodeDefinition.READ_COILS:
                        InnerBuildReadCoilsRequest(reqSpan);
                        break;
                    case FunctionCodeDefinition.READ_DISCRETE_INPUTS:
                        InnerBuildReadDiscreteInputsRequest(reqSpan);
                        break;
                    case FunctionCodeDefinition.READ_HOLDING_REGISTERS:
                    case FunctionCodeDefinition.READ_INPUT_REGISTERS:
                        InnerBuildReadHoldingAndInputRegistersRequest(reqSpan);
                        break;
                    case FunctionCodeDefinition.WRITE_SINGLE_COIL:
                        InnerBuildWriteSingleCoilRequest(reqSpan);
                        break;
                    case FunctionCodeDefinition.WRITE_SINGLE_REGISTER:
                        InnerBuildWriteSingleRegisterRequest(reqSpan);
                        break;
                    case FunctionCodeDefinition.WRITE_MULTIPLE_COILS:
                        InnerBuildWriteMultipleCoilsRequest(reqSpan, (byte)multipleWriteCoilsRemainByteNum);
                        break;
                    case FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS:
                        InnerBuildMultipleWriteRegistersRequest(reqSpan, (byte)multipleWriteRegistersRemainByteNum);
                        break;

                    case FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS:
                        InnerBuildMultipleReadWriteRegistersRequest(reqSpan, (byte)multipleReadWriteRegistersRemainByteNum);
                        break;
                    default:
                        throw new InvalidOperationException("invalid function code ");

                }

                actualSendNums = InnerSocket.Send(reqSpan);

            }
            catch (Exception ex)
            {
                Marshal.FreeHGlobal(nativePtr);
                throw ex;
            }

        }



        /// <summary>
        /// 读写多个保持寄存器
        /// </summary>
        /// <param name="reqSpan"></param>
        /// <param name="remainByte"></param>
        private void InnerBuildMultipleReadWriteRegistersRequest(Span<byte> reqSpan, byte remainByte)
        {
            //剩余的字节数量=2字节开始地址+2字节数量+1字节Byte数量+数据区域数量
            InnerBuildMBAPAndFunctionCode(remainByte, reqSpan);

            byte[] readStartingAddress = BitConverter.GetBytes(ReadStartingAddress).ToPlatform();
            reqSpan[8] = readStartingAddress[0];
            reqSpan[9] = readStartingAddress[1];


            ushort actualReadQuantity = GetActualQuantityCount(ReadQuantity);
            byte[] quantityBytes = BitConverter.GetBytes(actualReadQuantity).ToPlatform();
            reqSpan[10] = quantityBytes[0];
            reqSpan[11] = quantityBytes[1];


            byte[] writeStartingAddress = BitConverter.GetBytes(WriteStartingAddress).ToPlatform();
            reqSpan[12] = writeStartingAddress[0];
            reqSpan[13] = writeStartingAddress[1];


            ushort actualWriteQuantity = GetActualQuantityCount(WriteQuantity);
            byte[] writeQuantityBytes = BitConverter.GetBytes(actualWriteQuantity).ToPlatform();
            reqSpan[14] = writeQuantityBytes[0];
            reqSpan[15] = writeQuantityBytes[1];

            if (NumericalType == NumericalTypeEnum.Short)
            {
                int dataByteCount = 2 * Quantity;
                reqSpan[16] = (byte)dataByteCount;

                List<short> values = (List<short>)WriteData;
                int index = 16;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    index += 1;
                    reqSpan[index] = bytes[0];

                    index += 1;
                    reqSpan[index] = bytes[1];
                }
            }
            else if (NumericalType == NumericalTypeEnum.Integer)
            {
                int dataByteCount = 4 * Quantity;
                reqSpan[16] = (byte)dataByteCount;

                List<int> values = (List<int>)WriteData;
                int index = 16;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        reqSpan[index] = bytes[j];
                    }
                }
            }
            else if (NumericalType == NumericalTypeEnum.Float)
            {
                int dataByteCount = 4 * Quantity;
                reqSpan[16] = (byte)dataByteCount;

                List<float> values = (List<float>)WriteData;
                int index = 16;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        reqSpan[index] = bytes[j];
                    }
                }
            }
            else if (NumericalType == NumericalTypeEnum.Double)
            {
                int dataByteCount = 8 * Quantity;
                reqSpan[16] = (byte)dataByteCount;

                List<double> values = (List<double>)WriteData;
                int index = 16;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        reqSpan[index] = bytes[j];
                    }
                }

            }

        }


        private void InnerBuildMultipleWriteRegistersRequest(Span<byte> reqSpan, byte remainByte)
        {
            //剩余的字节数量=2字节开始地址+2字节数量+1字节Byte数量+数据区域数量
            InnerBuildMBAPAndFunctionCode(remainByte, reqSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();
            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];


            ushort actualQuantity = GetActualQuantityCount(Quantity);
            byte[] quantityBytes = BitConverter.GetBytes(actualQuantity).ToPlatform();
            reqSpan[10] = quantityBytes[0];
            reqSpan[11] = quantityBytes[1];


            if (NumericalType == NumericalTypeEnum.Short)
            {
                int dataByteCount = 2 * Quantity;
                reqSpan[12] = (byte)dataByteCount;

                List<short> values = (List<short>)WriteData;
                int index = 12;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    index += 1;
                    reqSpan[index] = bytes[0];

                    index += 1;
                    reqSpan[index] = bytes[1];
                }
            }
            else if (NumericalType == NumericalTypeEnum.Integer)
            {
                int dataByteCount = 4 * Quantity;
                reqSpan[12] = (byte)dataByteCount;

                List<int> values = (List<int>)WriteData;
                int index = 12;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        reqSpan[index] = bytes[j];
                    }
                }
            }
            else if (NumericalType == NumericalTypeEnum.Float)
            {
                int dataByteCount = 4 * Quantity;
                reqSpan[12] = (byte)dataByteCount;

                List<float> values = (List<float>)WriteData;
                int index = 12;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        reqSpan[index] = bytes[j];
                    }
                }

            }
            else if (NumericalType == NumericalTypeEnum.Double)
            {
                int dataByteCount = 8 * Quantity;
                reqSpan[12] = (byte)dataByteCount;

                List<double> values = (List<double>)WriteData;
                int index = 12;
                for (int i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        reqSpan[index] = bytes[j];
                    }
                }

            }

        }

        private void InnerBuildWriteMultipleCoilsRequest(Span<byte> reqSpan, byte remainByte)
        {
            //剩余的字节数量=2字节开始地址+2字节数量+1字节Byte数量+数据区域数量
            InnerBuildMBAPAndFunctionCode(remainByte, reqSpan);


            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();

            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];


            byte[] quantityBytes = BitConverter.GetBytes(Quantity).ToPlatform();

            reqSpan[10] = quantityBytes[0];
            reqSpan[11] = quantityBytes[1];



            //数据的字节数量
            var values = (List<bool>)WriteData;
            BitArray bitArray = new BitArray(values.ToArray());

            int dataByteNum;
            if (Quantity % 8 == 0)
            {
                dataByteNum = Quantity / 8;
            }
            else
            {
                dataByteNum = Quantity / 8 + 1;
            }

            reqSpan[12] = (byte)dataByteNum;

            byte[] dataBytes = new byte[dataByteNum];
            bitArray.CopyTo(dataBytes, 0);
            for (int i = 0; i < dataBytes.Length; i++)
            {
                reqSpan[13 + i] = dataBytes[i];
            }
        }


 

 

    }
}

