using System;
using System.Collections.Generic;
using System.Threading;
using ModbusNet.Message;
using ModbusNet.Message.Request;

namespace ModbusNet
{
    public class TcpModbusMessageBuilder
    {

        private static int _transactionSequenceIndex;

        /// <summary>
        /// 事务Id
        /// </summary>
        public ushort TransactionId { get; set; }

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
        private ushort Address { get; set; }


        /// <summary>
        /// 数量
        /// </summary>
        private ushort Quantity { get; set; }


        /// <summary>
        /// 读写多个寄存器时的读取开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private ushort ReadStartingAddress { get; set; }


        /// <summary>
        /// 读取的数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private ushort ReadQuantity { get; set; }

        /// <summary>
        /// 读写多个寄存器时的写入时的开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private ushort WriteStartingAddress { get; set; }


        /// <summary>
        /// 写入数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        private ushort WriteQuantity { get; set; }




        public TcpModbusMessageBuilder(byte functionCode)
        {
            Interlocked.Increment(ref _transactionSequenceIndex);

            TransactionId = (ushort)_transactionSequenceIndex;
            FunctionCode = functionCode;
        }


        public TcpModbusMessageBuilder(byte functionCode, byte unitId)
        {
            Interlocked.Increment(ref _transactionSequenceIndex);

            FunctionCode = functionCode;
            TransactionId = (ushort)_transactionSequenceIndex;
            UnitId = unitId;
        }


        public TcpModbusMessageBuilder BuildUnitId(byte unitId)
        {
            UnitId = unitId;
            return this;
        }

        public TcpModbusMessageBuilder BuildWriteData(object data)
        {
            WriteData = data;
            return this;
        }


        public TcpModbusMessageBuilder BuildAddress(ushort address)
        {
            Address = address;
            return this;
        }

        public TcpModbusMessageBuilder BuildQuantity(ushort quantity)
        {
            Quantity = quantity;
            return this;
        }

        public TcpModbusMessageBuilder BuildNumericalType(NumericalTypeEnum numericalType)
        {
            NumericalType = numericalType;
            return this;
        }

        public TcpModbusMessageBuilder BuildReadStartingAddress(ushort readStartAddress)
        {
            ReadStartingAddress = readStartAddress;
            return this;
        }

        public TcpModbusMessageBuilder BuildReadQuantity(ushort readQuantity)
        {
            ReadQuantity = readQuantity;
            return this;
        }

        public TcpModbusMessageBuilder BuildWriteStartingAddress(ushort writeStartingAddress)
        {
            WriteStartingAddress = writeStartingAddress;
            return this;
        }


        public TcpModbusMessageBuilder BuildWriteQuantity(ushort writeQuantity)
        {
            WriteQuantity = writeQuantity;
            return this;
        }

        public TcpModbusMessageBuilder BuildTransactionId(ushort transactionId)
        {
            TransactionId = transactionId;
            return this;
        }

        public BaseRequestMessage Build()
        {
            switch (FunctionCode)
            {
                case FunctionCodeDefinition.READ_COILS:
                    ReadCoilsRequestMessage readCoilsRequest = new ReadCoilsRequestMessage();
                    readCoilsRequest.TransactionId = TransactionId;
                    readCoilsRequest.UnitId = UnitId;
                    readCoilsRequest.Address = Address;
                    readCoilsRequest.Quantity = Quantity;
                    return readCoilsRequest;

                case FunctionCodeDefinition.READ_DISCRETE_INPUTS:
                    ReadDiscreteInputsRequestMessage readDiscreteInputsRequest = new ReadDiscreteInputsRequestMessage();
                    readDiscreteInputsRequest.TransactionId = TransactionId;
                    readDiscreteInputsRequest.UnitId = UnitId;
                    readDiscreteInputsRequest.Address = Address;
                    readDiscreteInputsRequest.Quantity = Quantity;
                    return readDiscreteInputsRequest;

                case FunctionCodeDefinition.READ_HOLDING_REGISTERS:
                    ReadHoldingRegistersRequestMessage readHoldingRegistersRequest = new ReadHoldingRegistersRequestMessage();
                    readHoldingRegistersRequest.TransactionId = TransactionId;
                    readHoldingRegistersRequest.UnitId = UnitId;
                    readHoldingRegistersRequest.Address = Address;
                    readHoldingRegistersRequest.Quantity = Quantity;
                    return readHoldingRegistersRequest;

                case FunctionCodeDefinition.READ_INPUT_REGISTERS:
                    ReadInputRegistersRequestMessage readInputRegistersRequest = new ReadInputRegistersRequestMessage();
                    readInputRegistersRequest.TransactionId = TransactionId;
                    readInputRegistersRequest.UnitId = UnitId;
                    readInputRegistersRequest.Address = Address;
                    readInputRegistersRequest.Quantity = Quantity;
                    return readInputRegistersRequest;

                case FunctionCodeDefinition.WRITE_SINGLE_COIL:
                    WriteSingleCoilRequestMessage writeSingleCoilRequest = new WriteSingleCoilRequestMessage();
                    writeSingleCoilRequest.TransactionId = TransactionId;
                    writeSingleCoilRequest.UnitId = UnitId;
                    writeSingleCoilRequest.Address = Address;
                    writeSingleCoilRequest.CoilStatus = (bool)WriteData;
                    return writeSingleCoilRequest;

                case FunctionCodeDefinition.WRITE_SINGLE_REGISTER:
                    WriteSingleRegisterRequestMessage writeSingleRegisterRequest = new WriteSingleRegisterRequestMessage();
                    writeSingleRegisterRequest.TransactionId = TransactionId;
                    writeSingleRegisterRequest.UnitId = UnitId;
                    writeSingleRegisterRequest.Address = Address;
                    writeSingleRegisterRequest.Value = (short)WriteData;
                    return writeSingleRegisterRequest;

                case FunctionCodeDefinition.WRITE_MULTIPLE_COILS:
                    WriteMultipleCoilsRequestMessage writeMultipleCoilsRequest = new WriteMultipleCoilsRequestMessage();
                    writeMultipleCoilsRequest.TransactionId = TransactionId;
                    writeMultipleCoilsRequest.UnitId = UnitId;
                    writeMultipleCoilsRequest.Address = Address;
                    writeMultipleCoilsRequest.Values = (List<bool>)WriteData;
                    return writeMultipleCoilsRequest;

                case FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS:
                    WriteMultipleRegistersRequestMessage writeMultipleRegistersRequest = new WriteMultipleRegistersRequestMessage();
                    writeMultipleRegistersRequest.TransactionId = TransactionId;
                    writeMultipleRegistersRequest.UnitId = UnitId;
                    writeMultipleRegistersRequest.Address = Address;
                    writeMultipleRegistersRequest.Values = (List<object>)WriteData;
                    return writeMultipleRegistersRequest;

                case FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS:
                    ReadWriteMultipleRegistersRequestMessage readWriteMultipleRegistersRequest = new ReadWriteMultipleRegistersRequestMessage();
                    readWriteMultipleRegistersRequest.TransactionId = TransactionId;
                    readWriteMultipleRegistersRequest.UnitId = UnitId;
                    readWriteMultipleRegistersRequest.ReadStartingAddress = ReadStartingAddress;
                    readWriteMultipleRegistersRequest.ReadQuantity = ReadQuantity;
                    readWriteMultipleRegistersRequest.WriteStartingAddress = WriteStartingAddress;
                    readWriteMultipleRegistersRequest.WriteQuantity = WriteQuantity;
                    readWriteMultipleRegistersRequest.Values = (List<object>)WriteData;
                    return readWriteMultipleRegistersRequest;
                default:
                    throw new ArgumentException("invalid function code");

            }
        }
    }





}

