using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using ModbusNet.Message;
using ModbusNet.Message.Request;

namespace ModbusNet
{
    public class TcpModbusRequestMessageBuilder
    {

        private static int TransactionSequenceIndex = 0;

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




        public TcpModbusRequestMessageBuilder(byte functionCode)
        {
            Interlocked.Increment(ref TransactionSequenceIndex);

            TransactionId = (ushort)TransactionSequenceIndex;
            FunctionCode = functionCode;
        }


        public TcpModbusRequestMessageBuilder(byte functionCode, byte unitId)
        {
            Interlocked.Increment(ref TransactionSequenceIndex);

            FunctionCode = functionCode;
            TransactionId = (ushort)TransactionSequenceIndex;
            UnitId = unitId;
        }


        public TcpModbusRequestMessageBuilder BuildUnitId(byte unitId)
        {
            UnitId = unitId;
            return this;
        }

        public TcpModbusRequestMessageBuilder BuildWriteData(object data)
        {
            WriteData = data;
            return this;
        }


        public TcpModbusRequestMessageBuilder BuildAddress(ushort address)
        {
            Address = address;
            return this;
        }

        public TcpModbusRequestMessageBuilder BuildQuantity(ushort quantity)
        {
            Quantity = quantity;
            return this;
        }

        public TcpModbusRequestMessageBuilder BuildNumericalType(NumericalTypeEnum numericalType)
        {
            NumericalType = numericalType;
            return this;
        }

        public TcpModbusRequestMessageBuilder BuildReadStartingAddress(ushort readStartAddress)
        {
            ReadStartingAddress = readStartAddress;
            return this;
        }

        public TcpModbusRequestMessageBuilder BuildReadQuantity(ushort readQuantity)
        {
            ReadQuantity = readQuantity;
            return this;
        }

        public TcpModbusRequestMessageBuilder BuildWriteStartingAddress(ushort writeStartingAddress)
        {
            WriteStartingAddress = writeStartingAddress;
            return this;
        }


        public TcpModbusRequestMessageBuilder BuildWriteQuantity(ushort writeQuantity)
        {
            WriteQuantity = writeQuantity;
            return this;
        }

        public TcpModbusRequestMessageBuilder BuildTransactionId(ushort transactionId)
        {
            TransactionId = transactionId;
            return this;
        }

        public BaseMessage Build()
        {
            switch (FunctionCode)
            {
                case FunctionCodeDefinition.READ_COILS:
                    ReadCoilsMessage readCoils = new ReadCoilsMessage();
                    readCoils.TransactionId = TransactionId;
                    readCoils.UnitId = UnitId;
                    readCoils.Address = Address;
                    readCoils.Quantity = Quantity;
                    return readCoils;

                case FunctionCodeDefinition.READ_DISCRETE_INPUTS:
                    ReadDiscreteInputsMessage readDiscreteInputs = new ReadDiscreteInputsMessage();
                    readDiscreteInputs.TransactionId = TransactionId;
                    readDiscreteInputs.UnitId = UnitId;
                    readDiscreteInputs.Address = Address;
                    readDiscreteInputs.Quantity = Quantity;
                    return readDiscreteInputs;

                case FunctionCodeDefinition.READ_HOLDING_REGISTERS:
                    ReadHoldingRegistersMessage readHoldingRegisters = new ReadHoldingRegistersMessage();
                    readHoldingRegisters.TransactionId = TransactionId;
                    readHoldingRegisters.UnitId = UnitId;
                    readHoldingRegisters.Address = Address;
                    readHoldingRegisters.Quantity = Quantity;
                    return readHoldingRegisters;

                case FunctionCodeDefinition.READ_INPUT_REGISTERS:
                    ReadInputRegistersMessage readInputRegisters = new ReadInputRegistersMessage();
                    readInputRegisters.TransactionId = TransactionId;
                    readInputRegisters.UnitId = UnitId;
                    readInputRegisters.Address = Address;
                    readInputRegisters.Quantity = Quantity;
                    return readInputRegisters;

                case FunctionCodeDefinition.WRITE_SINGLE_COIL:
                    WriteSingleCoilMessage writeSingleCoil = new WriteSingleCoilMessage();
                    writeSingleCoil.TransactionId = TransactionId;
                    writeSingleCoil.UnitId = UnitId;
                    writeSingleCoil.Address = Address;
                    writeSingleCoil.CoilStatus = (bool)WriteData;
                    return writeSingleCoil;

                case FunctionCodeDefinition.WRITE_SINGLE_REGISTER:
                    WriteSingleRegisterMessage writeSingleRegister = new WriteSingleRegisterMessage();
                    writeSingleRegister.TransactionId = TransactionId;
                    writeSingleRegister.UnitId = UnitId;
                    writeSingleRegister.Address = Address;
                    writeSingleRegister.Value = (short)WriteData;
                    return writeSingleRegister;

                case FunctionCodeDefinition.WRITE_MULTIPLE_COILS:
                    WriteMultipleCoilsMessage writeMultipleCoils = new WriteMultipleCoilsMessage();
                    writeMultipleCoils.TransactionId = TransactionId;
                    writeMultipleCoils.UnitId = UnitId;
                    writeMultipleCoils.Address = Address;
                    writeMultipleCoils.Values = (List<bool>)WriteData;
                    return writeMultipleCoils;

                case FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS:
                    WriteMultipleRegistersMessage writeMultipleRegisters = new WriteMultipleRegistersMessage();
                    writeMultipleRegisters.TransactionId = TransactionId;
                    writeMultipleRegisters.UnitId = UnitId;
                    writeMultipleRegisters.Address = Address;
                    writeMultipleRegisters.Values = (List<object>)WriteData;
                    return writeMultipleRegisters;

                case FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS:
                    ReadWriteMultipleRegistersMessage readWriteMultipleRegisters = new ReadWriteMultipleRegistersMessage();
                    readWriteMultipleRegisters.TransactionId = TransactionId;
                    readWriteMultipleRegisters.UnitId = UnitId;
                    readWriteMultipleRegisters.ReadStartingAddress = ReadStartingAddress;
                    readWriteMultipleRegisters.ReadQuantity = ReadQuantity;
                    readWriteMultipleRegisters.WriteStartingAddress = WriteStartingAddress;
                    readWriteMultipleRegisters.WriteQuantity = WriteQuantity;
                    readWriteMultipleRegisters.Values = (List<object>)WriteData;
                    return readWriteMultipleRegisters;
                default:
                    throw new ArgumentException("invalid function code");

            }
        }
    }





}

