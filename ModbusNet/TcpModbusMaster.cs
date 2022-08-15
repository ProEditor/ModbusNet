using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using ModbusNet.Message;

namespace ModbusNet
{
    public class TcpModbusMaster : IModbusMaster
    {
        /// <summary>
        /// 消息发送队列
        /// </summary>
        private readonly ConcurrentQueue<BaseRequestMessage> _requestMessageQueue = new ConcurrentQueue<BaseRequestMessage>();
        
        /// <summary>
        /// 内部套接字连接对象
        /// </summary>
        private Socket _innerSocket;

        /// <summary>
        /// 主站连接配置参数
        /// </summary>
        private readonly TcpMasterOption _option;

        private volatile bool _isRunning;

        private Thread _sendThread;

        private TcpModbusReceiveThread _receiveThread;


        public TcpModbusMaster(string ipAddress, int port)
        {

            _option = new TcpMasterOption
            {
                IPAddress = ipAddress,
                Port = port
            };

            if (string.IsNullOrEmpty(_option.IPAddress))
            {
                throw new ArgumentNullException($"argument {nameof(_option.IPAddress)} is must not null");
            }

            if (_option.Port <= 0 || _option.Port > 65535)
            {
                throw new ArgumentException($"argument {nameof(_option.Port)} must range(0-65535)");
            }

        }

        public TcpModbusMaster(TcpMasterOption option)
        {

            if (string.IsNullOrEmpty(option.IPAddress))
            {
                throw new ArgumentNullException($"argument {nameof(option.IPAddress)} is must not null");
            }

            if (option.Port <= 0 || option.Port > 65535)
            {
                throw new ArgumentException($"argument {nameof(option.Port)} must range(0-65535)");
            }
            _option = option;
        }

        public void Start()
        {
            if (_isRunning)
            {
                return;
            }

            _innerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _innerSocket.SendTimeout = 5000;
            _innerSocket.ReceiveTimeout = 5000;

            _innerSocket.Connect(_option.IPAddress, _option.Port);
            if (_innerSocket.Connected)
            {
                Console.WriteLine("连接Modbus成功");
            }

            _isRunning = true;

            _sendThread = new Thread(SendMessageThreadMethod)
            {
                IsBackground = true
            };
            _sendThread.Start();

            _receiveThread = new TcpModbusReceiveThread(_innerSocket);
            _receiveThread.Start();

        }

        /// <summary>
        /// 停止TcpModbus 客户端
        /// </summary>
        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _innerSocket.Close();
                _innerSocket.Dispose();

                foreach (BaseRequestMessage baseMessage in _requestMessageQueue)
                {
                    baseMessage.Dispose();
                }


                _requestMessageQueue.Clear();

            }
        }


        private void SendMessageThreadMethod()
        {
            while (_isRunning)
            {
                if (_requestMessageQueue.IsEmpty == false)
                {
                    _requestMessageQueue.TryDequeue(out BaseRequestMessage message);
                    if (message != null)
                    {
                        try
                        {
                            _innerSocket.Send(message.ToBinary());
                        }
                        catch (Exception ex)
                        {
                            message.Dispose();
                            //TODO 需要记录日志
                            Console.WriteLine(ex);
                        }
                    }
                }
                Thread.Sleep(1);
            }

        }

        public void ReadCoils(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_COILS);
            builder.BuildAddress(start).BuildQuantity(quantity);
            var request = builder.Build();
            SendMessage(request);

        }



        public void WriteSingleCoil(ushort start, bool status)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_SINGLE_COIL);
            builder.BuildAddress(start).BuildWriteData(status);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadDiscreteInputs(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_DISCRETE_INPUTS);
            builder.BuildAddress(start).BuildQuantity(quantity);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadInt16HoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadInt32HoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadFloatHoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            SendMessage(request);
        }


        public void ReadDoubleHoldingRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_HOLDING_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadIntInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadFloatInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadDoubleInputRegisters(ushort start, ushort quantity)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_INPUT_REGISTERS);
            builder.BuildAddress(start).BuildQuantity(quantity).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            SendMessage(request);
        }

        public void WriteSingleRegister(ushort start, short value)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_SINGLE_REGISTER);
            builder.BuildAddress(start).BuildWriteData(value);
            var request = builder.Build();
            SendMessage(request);
        }

        public void WriteIntSingleRegister(ushort start, int value)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<int>() { value }).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            SendMessage(request);
        }


        public void WriteFloatSingleRegister(ushort start, float value)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<float>() { value }).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            SendMessage(request);
        }

        public void WriteDoubleSingleRegister(ushort start, double value)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(new List<double>() { value }).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            SendMessage(request);
        }


        public void WriteMultipleCoils(ushort start, List<bool> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_COILS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count);
            var request = builder.Build();
            SendMessage(request);
        }


        public void WriteMultipleRegisters(ushort start, List<short> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count);
            var request = builder.Build();
            SendMessage(request);
        }


        public void WriteIntMultipleRegisters(ushort start, List<int> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            SendMessage(request);
        }


        public void WriteFloatMultipleRegisters(ushort start, List<float> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            SendMessage(request);
        }


        public void WriteDoubleMultipleRegisters(ushort start, List<double> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS);
            builder.BuildAddress(start).BuildWriteData(values).BuildQuantity((ushort)values.Count).BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadWriteMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<short> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Short);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadWriteIntMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<int> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Integer);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadWriteFloatMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<float> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Float);
            var request = builder.Build();
            SendMessage(request);
        }

        public void ReadWriteDoubleMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<double> values)
        {
            TcpModbusMessageBuilder builder = new TcpModbusMessageBuilder(FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS);
            builder
                .BuildReadStartingAddress(readStartAddress)
                .BuildReadQuantity(readQuantity)
                .BuildWriteStartingAddress(writeStartAddress)
                .BuildWriteData(values)
                .BuildWriteQuantity((ushort)values.Count)
                .BuildNumericalType(NumericalTypeEnum.Double);
            var request = builder.Build();
            SendMessage(request);
        }


        private void SendMessage(BaseRequestMessage request)
        {
            _requestMessageQueue.Enqueue(request);
        }


    }
}
