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
        private readonly ConcurrentQueue<BaseMessage> _requestMessageQueue = new ConcurrentQueue<BaseMessage>();

        /// <summary>
        /// 消息接收队列
        /// </summary>
        private readonly ConcurrentQueue<BaseMessage> _receiveMessageQueue = new ConcurrentQueue<BaseMessage>();

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

        private Thread _receiveThread;

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

            _receiveThread = new Thread(ReceiveMessageThreadMethod)
            {
                IsBackground = true
            };
            _receiveThread.Start();
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _innerSocket.Close();
                _innerSocket.Dispose();

                foreach (BaseMessage baseMessage in _requestMessageQueue)
                {
                    baseMessage.Dispose();
                }

                foreach (BaseMessage baseMessage in _receiveMessageQueue)
                {
                    baseMessage.Dispose();
                }

                _requestMessageQueue.Clear();
                _receiveMessageQueue.Clear();

            }
        }

        private void ReceiveMessageThreadMethod()
        {
            while (_isRunning)
            {
                if (_innerSocket.Available <= 0)
                {
                    Thread.Sleep(1);
                    continue;
                }

                if (_receiveMessageQueue.IsEmpty == false)
                {
                    _receiveMessageQueue.TryDequeue(out BaseMessage message);
                    if (message != null)
                    {
                        ModbusApplicationProtocolPart mbap = ReceiveApplicationProtocol();
                        if (mbap == null)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        switch (message.FunctionCode)
                        {
                            case FunctionCodeDefinition.READ_COILS:




                                break;

                            case FunctionCodeDefinition.READ_DISCRETE_INPUTS:
                                break;

                            case FunctionCodeDefinition.READ_HOLDING_REGISTERS:
                                break;

                            case FunctionCodeDefinition.READ_INPUT_REGISTERS:
                                break;

                            case FunctionCodeDefinition.WRITE_SINGLE_COIL:
                                break;

                            case FunctionCodeDefinition.WRITE_SINGLE_REGISTER:
                                break;

                            case FunctionCodeDefinition.WRITE_MULTIPLE_COILS:
                                break;

                            case FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS:
                                break;

                            case FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS:
                                break;
                            default:
                                throw new ArgumentException("invalid function code");

                        }

                    }
                }

                Thread.Sleep(1);

            }
        }

        /// <summary>
        /// 从套接字缓冲区中接收MBAP部分的数据
        /// </summary>
        /// <returns>返回MBAP数据</returns>
        private ModbusApplicationProtocolPart ReceiveApplicationProtocol()
        {
            //MBAP部分的长度
            const int modbusAppProtocolLen = 7;
            //累计接收的长度
            int fullReceivedSize = 0;
            //剩余需要接收的长度
            int remainLen = modbusAppProtocolLen;
            //开始往Buffer中写入的开始偏移量
            int startOffset = 0;
            byte[] buffer = new byte[modbusAppProtocolLen];
            while (_innerSocket.Available > 0)
            {
                //如果连接已经中断直接返回null
                if (_innerSocket.Connected == false)
                    return null;

                try
                {
                    int receivedSize = _innerSocket.Receive(buffer, startOffset, remainLen, SocketFlags.None);
                    fullReceivedSize += receivedSize;

                    if (fullReceivedSize == modbusAppProtocolLen)
                    {
                        ModbusApplicationProtocolPart mbap = new ModbusApplicationProtocolPart();

                        if (BitConverter.IsLittleEndian)
                        {
                            mbap.TransactionId = BitConverter.ToUInt16(new[] { buffer[1], buffer[0] }, 0);
                        }
                        else
                        {
                            mbap.TransactionId = BitConverter.ToUInt16(buffer, 0);
                        }

                        mbap.ProtocolId = 0;

                        if (BitConverter.IsLittleEndian)
                        {
                            mbap.Length = BitConverter.ToUInt16(new[] { buffer[5], buffer[4] }, 0);
                        }
                        else
                        {
                            mbap.TransactionId = BitConverter.ToUInt16(buffer, 4);
                        }

                        mbap.UnitId = buffer[6];
                        return mbap;

                    }

                    remainLen = modbusAppProtocolLen - fullReceivedSize;
                    startOffset = fullReceivedSize;

                }
                catch (SocketException e)
                {
                    //套接字超时读取后，会直接抛出异常
                    Console.WriteLine(e);
                }

            }
            return null;
        }

        private void SendMessageThreadMethod()
        {
            while (_isRunning)
            {
                if (_requestMessageQueue.IsEmpty == false)
                {
                    _requestMessageQueue.TryDequeue(out BaseMessage message);
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


        private void SendMessage(BaseMessage request)
        {
            _requestMessageQueue.Enqueue(request);
        }


    }
}
