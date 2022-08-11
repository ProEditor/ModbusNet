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


        private Socket InnerSocket { get; set; }


        public TcpModbusRequestBuilder(Socket innerSocket, byte functionCode)
        {
            Interlocked.Increment(ref TransactionSequenceIndex);

            TransactionId = (ushort)TransactionSequenceIndex;
            FunctionCode = functionCode;
            InnerSocket = innerSocket;
        }


        public TcpModbusRequestBuilder(Socket innerSocket, byte functionCode, byte unitId)
        {
            Interlocked.Increment(ref TransactionSequenceIndex);

            FunctionCode = functionCode;
            TransactionId = (ushort)TransactionSequenceIndex;
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


        public TcpModbusRequestBuilder BuildAddress(ushort address)
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

        public TcpModbusRequestBuilder BuildReadStartingAddress(ushort readStartAddress)
        {
            ReadStartingAddress = readStartAddress;
            return this;
        }

        public TcpModbusRequestBuilder BuildReadQuantity(ushort readQuantity)
        {
            ReadQuantity = readQuantity;
            return this;
        }

        public TcpModbusRequestBuilder BuildWriteStartingAddress(ushort writeStartingAddress)
        {
            WriteStartingAddress = writeStartingAddress;
            return this;
        }


        public TcpModbusRequestBuilder BuildWriteQuantity(ushort writeQuantity)
        {
            WriteQuantity = writeQuantity;
            return this;
        }

        public TcpModbusRequestBuilder BuildTransactionId(ushort transactionId)
        {
            TransactionId = transactionId;
            return this;
        }

        public TcpModbusRequest Build()
        {
            TcpModbusRequest request = new TcpModbusRequest();
            request.TransactionId = TransactionId;
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





}

