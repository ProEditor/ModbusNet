using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusNet
{
    public class TcpModbusMaster : IModbusMaster, IDisposable
    {

        private int TransactionSequenceIndex = 0;

        /// <summary>
        /// 请求队列，key为事务Id，value
        /// </summary>
        private ConcurrentDictionary<int, TcpModbusRequest> RequestMessageQueue = new ConcurrentDictionary<int, TcpModbusRequest>();


        /// <summary>
        /// 内部套接字连接对象
        /// </summary>
        private Socket InnerSocket;

        /// <summary>
        /// 主站连接配置参数
        /// </summary>
        private TcpMasterOption Option;


        public TcpModbusMaster()
        {

        }

        public TcpModbusMaster(string ipAddress, int port)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentNullException(string.Format("argument %s is must not null", nameof(ipAddress)));
            }
            if (port <= 0 || port > 65535)
            {
                throw new ArgumentException(string.Format("argument %s must range(0-65535)", nameof(port)));
            }

            Option = new TcpMasterOption();
            Option.IPAddress = ipAddress;
            Option.Port = port;

        }

        public TcpModbusMaster(TcpMasterOption option)
        {

            if (string.IsNullOrEmpty(option.IPAddress))
            {
                throw new ArgumentNullException(string.Format("argument %s is must not null", nameof(option.IPAddress)));
            }

            if (option.Port <= 0 || option.Port > 65535)
            {
                throw new ArgumentException(string.Format("argument %s must range(0-65535)", nameof(option.Port)));
            }

            Option = option;

        }

        public void Start()
        {
            InnerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            InnerSocket.SendTimeout = 5000;
            InnerSocket.ReceiveTimeout = 5000;

            InnerSocket.Connect(Option.IPAddress, Option.Port);
            if (InnerSocket.Connected)
            {
                Console.WriteLine("连接Modbus成功");
            }
        }

        public void ReadCoils(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_COILS);
            builder.BuildAddress(start).BuildQuantity(quantity);
            var request = builder.Build();
            Task.Factory.StartNew(() =>
             {
                 request.Request();
                 return new List<bool>();
             });

        }

        public void WriteSingleCoil(ushort start, bool status)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_SINGLE_COIL);
            builder.BuildAddress(start).BuildWriteData(status);
            var request = builder.Build();

        }

        public void ReadDiscreteInputs(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_DISCRETE_INPUTS);
            builder.BuildAddress(start).BuildQuantity(quantity);
            var request = builder.Build();

        }

        public void ReadInt16HoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();
        }

        public void ReadInt32HoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();

        }

        public void ReadFloatHoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();


        }


        public void ReadDoubleHoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();

        }

        public void ReadInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();


        }

        public void ReadIntInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();


        }

        public void ReadFloatInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();

        }

        public void ReadDoubleInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();

        }

        public void WriteSingleRegister(ushort start, short value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_SINGLE_REGISTER);
            builder.BuildAddress(start).BuildWriteData(value);
            var request = builder.Build();

        }

        public void WriteIntSingleRegister(ushort start, int value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<int>() { value }).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();

        }


        public void WriteFloatSingleRegister(ushort start, float value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<float>() { value }).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();

        }

        public void WriteDoubleSingleRegister(ushort start, double value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<double>() { value }).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();

        }


        public void WriteMultipleCoils(ushort start, List<bool> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_COILS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count);
            var request = builder.Build();

        }


        public void WriteMultipleRegisters(ushort start, List<short> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count);
            var request = builder.Build();

        }


        public void WriteIntMultipleRegisters(ushort start, List<int> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();

        }


        public void WriteFloatMultipleRegisters(ushort start, List<float> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();

        }


        public void WriteDoubleMultipleRegisters(ushort start, List<double> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();

        }

        public void ReadWriteMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<short> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();

        }

        public void ReadWriteIntMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<int> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();

        }

        public void ReadWriteFloatMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<float> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();

        }

        public void ReadWriteDoubleMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<double> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();

        }



        public void Dispose()
        {
            if (InnerSocket != null)
            {
                InnerSocket.Close();
                InnerSocket.Dispose();
            }
        }


    }
}

