using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ModbusNet
{
    public class TcpModbusMaster : IModbusMaster, IDisposable
    {

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

        public List<bool> ReadCoils(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_COILS);
            builder.BuildAddress(start).BuildQuantity(quantity);
            var request = builder.Build();
            var response = request.Request();

            return null;
        }

        public void WriteSingleCoil(int start, bool status)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_SINGLE_COIL);
            builder.BuildAddress(start).BuildWriteData(status);
            var request = builder.Build();
            var response = request.Request();
        }

        public List<bool> ReadDiscreteInputs(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_DISCRETE_INPUTS);
            builder.BuildAddress(start).BuildQuantity(quantity);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public List<short> ReadInt16HoldingRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public List<int> ReadInt32HoldingRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public List<float> ReadFloatHoldingRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }


        public List<double> ReadDoubleHoldingRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public List<short> ReadInputRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTER);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public List<int> ReadIntInputRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTER);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public List<float> ReadFloatInputRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTER);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public List<double> ReadDoubleInputRegisters(int start, ushort quantity)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.READ_INPUT_REGISTER);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            var response = request.Request();
            return null;
        }

        public void WriteSingleRegister(int start, short value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_SINGLE_REGISTER);
            builder.BuildAddress(start).BuildWriteData(value);
            var request = builder.Build();
            var response = request.Request();
        }

        public void WriteIntSingleRegister(int start, int value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<int>() { value }).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            var response = request.Request();
        }


        public void WriteFloatSingleRegister(int start, float value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<float>() { value }).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            var response = request.Request();
        }

        public void WriteDoubleSingleRegister(int start, double value)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<double>() { value }).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            var response = request.Request();
        }


        public void WriteMultipleCoils(int start, List<bool> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_COILS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count);
            var request = builder.Build();
            var response = request.Request();
        }


        public void WriteMultipleRegisters(int start, List<short> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count);
            var request = builder.Build();
            var response = request.Request();
        }


        public void WriteIntMultipleRegisters(int start, List<int> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            var response = request.Request();
        }


        public void WriteFloatMultipleRegisters(int start, List<float> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            var response = request.Request();
        }


        public void WriteDoubleMultipleRegisters(int start, List<double> values)
        {
            TcpModbusRequestBuilder builder = new TcpModbusRequestBuilder(InnerSocket, FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            var response = request.Request();
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

