using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace ModbusNet
{
    public class TcpModbusRequestBuilder
    {

        private long TransactionSequenceIndex = 1;

        /// <summary>
        /// 单元标识符
        /// </summary>
        private byte UnitId { get; set; } = 0;


        public NumericalTypeEnum NumericalType { get; set; } = NumericalTypeEnum.Short;


        /// <summary>
        /// 请求的功能码
        /// </summary>
        private byte FunctionCode { get; set; }

        /// <summary>
        /// 写入到从站的数据
        /// </summary>
        private object WriteData { get; set; }

        /// <summary>
        /// 开始地址
        /// </summary>
        private int Address { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        private ushort Quantity { get; set; }


        /// <summary>
        /// 读写多个寄存器时的读取开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private int ReadStartingAddress { get; set; }


        /// <summary>
        /// 读取的数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private int ReadQuantity { get; set; }

        /// <summary>
        /// 读写多个寄存器时的写入时的开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private int WriteStartingAddress { get; set; }


        /// <summary>
        /// 写入数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private int WriteQuantity { get; set; }


        private Socket InnerSocket { get; set; }


        public TcpModbusRequestBuilder(Socket innerSocket, byte functionCode)
        {
            FunctionCode = functionCode;
            InnerSocket = innerSocket;
        }


        public TcpModbusRequestBuilder(Socket innerSocket, byte functionCode, byte unitId)
        {
            FunctionCode = functionCode;
            UnitId = unitId;
            InnerSocket = innerSocket;

        }


        public TcpModbusRequestBuilder BuildUnitId(byte unitId)
        {
            UnitId = unitId;
            return this;
        }

        public TcpModbusRequestBuilder BuildWriteData(object data)
        {
            WriteData = data;
            return this;
        }


        public TcpModbusRequestBuilder BuildAddress(int address)
        {
            Address = address;
            return this;
        }
        public TcpModbusRequestBuilder BuildQuantity(ushort quantity)
        {
            Quantity = quantity;
            return this;
        }

        public TcpModbusRequestBuilder BuildNumericalType(NumericalTypeEnum numericalType)
        {
            NumericalType = numericalType;
            return this;
        }

        public TcpModbusRequestBuilder BuildReadStartingAddress(int readStartAddress)
        {
            ReadStartingAddress = readStartAddress;
            return this;
        }

        public TcpModbusRequestBuilder BuildReadQuantity(int readQuantity)
        {
            ReadQuantity = readQuantity;
            return this;
        }

        public TcpModbusRequestBuilder BuildWriteStartingAddress(int writeStartingAddress)
        {
            WriteStartingAddress = writeStartingAddress;
            return this;
        }


        public TcpModbusRequestBuilder BuildWriteQuantity(int writeQuantity)
        {
            WriteQuantity = writeQuantity;
            return this;
        }


        public TcpModbusRequest Build()
        {
            //原子更新事务Id
            Interlocked.Increment(ref TransactionSequenceIndex);

            TcpModbusRequest request = new TcpModbusRequest();
            request.TransactionId = (ushort)TransactionSequenceIndex;
            request.UnitId = UnitId;
            request.FunctionCode = FunctionCode;
            request.WriteData = WriteData;
            request.Address = Address;
            request.Quantity = Quantity;
            request.ReadStartingAddress = ReadStartingAddress;
            request.ReadQuantity = ReadQuantity;
            request.WriteStartingAddress = WriteStartingAddress;
            request.WriteQuantity = WriteQuantity;
            request.NumericalType = NumericalType;
            request.InnerSocket = InnerSocket;

            return request;
        }
    }



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
        public int Address { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        public ushort Quantity { get; set; }


        /// <summary>
        /// 读写多个寄存器时的读取开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public int ReadStartingAddress { get; set; }


        /// <summary>
        /// 读取的数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public int ReadQuantity { get; set; }

        /// <summary>
        /// 读写多个寄存器时的写入时的开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public int WriteStartingAddress { get; set; }


        /// <summary>
        /// 写入数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public int WriteQuantity { get; set; }

        public Socket InnerSocket { get; set; }

        public TcpModbusResponse Request()
        {

            //实际发送的字节数量
            int actualSendNums;

            //理应发送的字节数量
            int shouldSendNums = 0;

            IntPtr nativePtr = IntPtr.Zero;
            Span<byte> reqSpan = null;


            //写入多个线圈量时MBAP中“后续字节数量”
            int multipleWriteCoilsRemainByteNum = 0;
            int multipleWriteRegistersRemainByteNum = 0;
            try
            {
                switch (FunctionCode)
                {
                    case FunctionCodeDefinition.READ_COILS:
                    case FunctionCodeDefinition.READ_DISCRETE_INPUTS:
                    case FunctionCodeDefinition.READ_HOLDING_REGISTERS:
                    case FunctionCodeDefinition.READ_INPUT_REGISTER:
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
                    case FunctionCodeDefinition.READ_INPUT_REGISTER:
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
                    default:
                        throw new InvalidOperationException("invalid function code ");

                }

                actualSendNums = InnerSocket.Send(reqSpan);

                if (actualSendNums != shouldSendNums)
                {
                    Console.WriteLine("发送的数据量不一致");
                    return new TcpModbusResponse();
                }
                else
                {

                    //先读取MBAP部分的数据,根据MBAP数据长度部分计算后续需要接收的数据长度
                    const ushort MBAP_LENGTH = 7;
                    Span<byte> mbapBuffer = stackalloc byte[MBAP_LENGTH];

                    int readNums = InnerSocket.Receive(mbapBuffer, SocketFlags.None);
                    if (readNums == MBAP_LENGTH)
                    {
                        ushort remainderLen;
                        //如果是小端序系统，比如说Mac需要额外处理这个字节序列
                        if (BitConverter.IsLittleEndian)
                        {
                            var tmpBuffer = mbapBuffer.Slice(4, 2);
                            tmpBuffer.Reverse();
                            remainderLen = BitConverter.ToUInt16(tmpBuffer);
                        }
                        else
                        {
                            remainderLen = BitConverter.ToUInt16(mbapBuffer.Slice(4, 2));
                        }

                        //需要减去1个单元标识符号
                        remainderLen = (ushort)(remainderLen - 1);
                        Span<byte> remainderBuffer = stackalloc byte[remainderLen];
                        readNums = InnerSocket.Receive(remainderBuffer, SocketFlags.None);

                        if (readNums == remainderLen)
                        {
                            var functionCode = (ushort)remainderBuffer[0];

                            //处理异常请求响应
                            if (functionCode == 0x80 + FunctionCode)
                            {
                                var exceptionCode = remainderBuffer[1];

                                TcpModbusResponse response = new TcpModbusResponse();
                                response.Success = false;
                                response.ExceptionCode = exceptionCode;
                                return response;
                            }
                            else
                            {
                                var dataBuffer = remainderBuffer.Slice(1);
                                switch (FunctionCode)
                                {
                                    case FunctionCodeDefinition.READ_COILS:
                                        return ResolveReadCoilsResponse(dataBuffer);
                                    case FunctionCodeDefinition.READ_DISCRETE_INPUTS:
                                        return ResolveReadDiscreteInputsResponse(dataBuffer);
                                    case FunctionCodeDefinition.READ_HOLDING_REGISTERS:
                                        return ResolveReadHoldingRegistersResponse(dataBuffer);

                                }
                            }


                        }
                        else
                        {
                            //读取的字节不一致，需要记录日志，重试读取
                        }


                    }
                    else
                    {


                    }
                }
            }
            catch (Exception ex)
            {
                Marshal.FreeHGlobal(nativePtr);
                throw ex;
            }

            return null;
        }

        private void InnerBuildMultipleWriteRegistersRequest(Span<byte> reqSpan, byte remainByte)
        {
            //剩余的字节数量=2字节开始地址+2字节数量+1字节Byte数量+数据区域数量
            InnerBuildMBAPAndFunctionCode(remainByte, reqSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();
            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];


            ushort actualQuantity = (ushort)GetActualQuantityCount();
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
                        reqSpan[index] = 0;
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



        /// <summary>
        /// 构建保持寄存器请求参数
        /// </summary>
        /// <param name="reqSpan"></param>
        private void InnerBuildReadHoldingAndInputRegistersRequest(Span<byte> reqSpan)
        {
            InnerBuildMBAPAndFunctionCode(6, reqSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();

            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];

            int actualQuantity = GetActualQuantityCount();
            byte[] quantityBytes = BitConverter.GetBytes(actualQuantity).ToPlatform();

            reqSpan[10] = quantityBytes[0];
            reqSpan[11] = quantityBytes[1];

        }

        /// <summary>
        /// 根据请求的数据类型返回需要实际读取的地址数量（Modbus官方只支持16Bit数据类型，32bit的数据类型需要用2个地址扩充）
        /// </summary>
        /// <returns></returns>
        private int GetActualQuantityCount()
        {
            int actualQuantityCount = 2;
            switch (NumericalType)
            {
                case NumericalTypeEnum.Short:
                    actualQuantityCount = Quantity;
                    break;
                case NumericalTypeEnum.Integer:
                case NumericalTypeEnum.Float:
                    actualQuantityCount = Quantity * 2;
                    break;
                case NumericalTypeEnum.Double:
                    actualQuantityCount = Quantity * 4;
                    break;
            }

            return actualQuantityCount;
        }

        /// <summary>
        /// 读取保持寄存器，需要处理
        /// </summary>
        /// <param name="dataBuffer"></param>
        /// <returns></returns>
        private TcpModbusResponse ResolveReadHoldingRegistersResponse(Span<byte> dataBuffer)
        {
            TcpModbusResponse response = new TcpModbusResponse();

            if (NumericalType == NumericalTypeEnum.Short)
            {
                List<short> values = new List<short>();
                //0为保持寄存器数据区域的字节长度，从索引1位置开始保持寄存器返回的值
                dataBuffer = dataBuffer.Slice(1);
                for (int i = 0; i < Quantity; i++)
                {
                    var registerValueSpan = dataBuffer.Slice(i * 2, 2);
                    if (BitConverter.IsLittleEndian)
                    {
                        registerValueSpan.Reverse();
                    }
                    var registerValue = BitConverter.ToInt16(registerValueSpan);
                    values.Add(registerValue);
                }
                response.Result = values;
            }
            else if (NumericalType == NumericalTypeEnum.Integer)
            {
                List<int> values = new List<int>();
                //0为保持寄存器数据区域的字节长度，从索引1位置开始保持寄存器返回的值
                dataBuffer = dataBuffer.Slice(1);
                for (int i = 0; i < Quantity; i++)
                {
                    var registerValueSpan = dataBuffer.Slice(i * 4, 4);
                    if (BitConverter.IsLittleEndian)
                    {
                        registerValueSpan.Reverse();
                    }
                    var registerValue = BitConverter.ToInt32(registerValueSpan);
                    values.Add(registerValue);
                }
                response.Result = values;
            }

            else if (NumericalType == NumericalTypeEnum.Float)
            {
                List<float> values = new List<float>();
                //0为保持寄存器数据区域的字节长度，从索引1位置开始保持寄存器返回的值
                dataBuffer = dataBuffer.Slice(1);
                for (int i = 0; i < Quantity; i++)
                {
                    var registerValueSpan = dataBuffer.Slice(i * 8, 4);
                    if (BitConverter.IsLittleEndian)
                    {
                        registerValueSpan.Reverse();
                    }
                    var registerValue = BitConverter.ToSingle(registerValueSpan);
                    values.Add(registerValue);
                }
                response.Result = values;
            }
            else if (NumericalType == NumericalTypeEnum.Double)
            {
                List<double> values = new List<double>();
                //0为保持寄存器数据区域的字节长度，从索引1位置开始保持寄存器返回的值
                dataBuffer = dataBuffer.Slice(1);
                for (int i = 0; i < Quantity; i++)
                {
                    var registerValueSpan = dataBuffer.Slice(i * 2, 8);
                    if (BitConverter.IsLittleEndian)
                    {
                        registerValueSpan.Reverse();
                    }
                    var registerValue = BitConverter.ToDouble(registerValueSpan);
                    values.Add(registerValue);
                }
                response.Result = values;
            }

            return response;

        }

        /// <summary>
        /// 解析离散量输入状态清单
        /// </summary>
        /// <param name="dataBuffer">数据缓冲区</param>
        /// <returns></returns>
        private TcpModbusResponse ResolveReadDiscreteInputsResponse(Span<byte> dataBuffer)
        {
            TcpModbusResponse response = new TcpModbusResponse();

            var resByteCount = dataBuffer[0];
            var inputStatus = new BitArray(dataBuffer.Slice(1, resByteCount).ToArray());
            List<bool> result = new List<bool>(Quantity);
            for (int i = 0; i < Quantity; i++)
            {
                var status = inputStatus[i];
                result.Add(status);
            }
            response.Success = true;
            response.Result = result;


            return response;

        }
        /// <summary>
        /// 读取线圈量
        /// </summary>
        /// <param name="remainderBuffer"></param>
        private TcpModbusResponse ResolveReadCoilsResponse(Span<byte> remainderBuffer)
        {
            TcpModbusResponse response = new TcpModbusResponse();

            var resByteCount = remainderBuffer[0];

            var coilStatus = new BitArray(remainderBuffer.Slice(1, resByteCount).ToArray());
            List<bool> result = new List<bool>(Quantity);
            for (int i = 0; i < Quantity; i++)
            {
                var status = coilStatus[i];
                result.Add(status);
            }
            response.Success = true;
            response.Result = result;


            return response;
        }

        private void InnerBuildWriteSingleCoilRequest(Span<byte> reqSpan)
        {

            InnerBuildMBAPAndFunctionCode(6, reqSpan);

            bool coilStatus = (bool)WriteData;

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();

            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];


            if (coilStatus == true)
            {
                reqSpan[10] = 0xFF;
                reqSpan[11] = 0x00;
            }
            else
            {
                reqSpan[10] = 0x00;
                reqSpan[11] = 0x00;
            }
        }

        /// <summary>
        /// 写入单个保持寄存器
        /// </summary>
        /// <param name="reqSpan"></param>
        private void InnerBuildWriteSingleRegisterRequest(Span<byte> reqSpan)
        {

            InnerBuildMBAPAndFunctionCode(6, reqSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();
            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];


            var value = (short)WriteData;
            byte[] valueBytes = BitConverter.GetBytes(value).ToPlatform();
            reqSpan[10] = valueBytes[0];
            reqSpan[11] = valueBytes[1];

        }



        /// <summary>
        /// 构建读取线圈量请求数组
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private void InnerBuildReadCoilsRequest(Span<byte> reqSpan)
        {
            InnerBuildMBAPAndFunctionCode(6, reqSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();
            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];


            byte[] quantityBytes = BitConverter.GetBytes(Quantity).ToPlatform();
            reqSpan[10] = quantityBytes[0];
            reqSpan[11] = quantityBytes[1];

        }
        /// <summary>
        /// 构建读取线圈量请求数组
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private void InnerBuildReadDiscreteInputsRequest(Span<byte> reqSpan)
        {
            InnerBuildMBAPAndFunctionCode(6, reqSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();
            reqSpan[8] = addressBytes[0];
            reqSpan[9] = addressBytes[1];


            byte[] quantityBytes = BitConverter.GetBytes(Quantity).ToPlatform();
            reqSpan[10] = quantityBytes[0];
            reqSpan[11] = quantityBytes[1];

        }

        /// <summary>
        /// 构建MBAP部分以及功能码的数据
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <param name="remainByte">剩余长度</param>
        /// <param name="reqSpan">返回的字节数组</param>
        private void InnerBuildMBAPAndFunctionCode(ushort remainByte, Span<byte> reqSpan)
        {

            var transactionBytes = BitConverter.GetBytes(TransactionId).ToPlatform();
            reqSpan[0] = transactionBytes[0];
            reqSpan[1] = transactionBytes[1];


            //协议标识符，默认为0x0000
            reqSpan[2] = 0x00;
            reqSpan[3] = 0x00;


            //1byte 单元标识符+1byte功能码+2byte开始地址+2byte数量=6字节
            //后续的字节长度，占用2字节

            byte[] remainBytes = BitConverter.GetBytes(remainByte).ToPlatform();
            reqSpan[4] = remainBytes[0];
            reqSpan[5] = remainBytes[1];


            //单元标识符，占用1个字节
            reqSpan[6] = UnitId;

            //功能码，占用1个字节
            reqSpan[7] = FunctionCode;

        }
    }
}

