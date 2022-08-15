using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ModbusNet.Message;
using ModbusNet.Message.Response;
using NLog;
namespace ModbusNet
{
    public class TcpModbusReceiveThread
    {

        private readonly static Logger Logger = LogManager.GetCurrentClassLogger();

        //单次循环周期线程休眠的时长
        private const int DefaultSleepMilliseconds = 1;

        private readonly Thread _internalThread;

        private readonly Socket _internalSocket;

        private bool _isRunning;

        /// <summary>
        /// 消息接收队列
        /// </summary>
        private readonly ConcurrentBag<BaseRequestMessage> _receiveMessageCollection = new ConcurrentBag<BaseRequestMessage>();

        public TcpModbusReceiveThread(Socket internalSocket)
        {
            _internalThread = new Thread(ReceiveMessageThreadMethod)
            {
                IsBackground = true,
                Name = "TcpModbusReceiveThread"
            };
            _internalSocket = internalSocket;
        }

        /// <summary>
        /// 往接收队列中添加一个消息
        /// </summary>
        /// <param name="requestMessage">需要添加到接收队列中的消息</param>
        public void TryAdd(BaseRequestMessage requestMessage)
        {
            _receiveMessageCollection.Add(requestMessage);
        }





        public void Start()
        {
            _internalThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;

            foreach (BaseRequestMessage baseMessage in _receiveMessageCollection)
            {
                baseMessage.Dispose();
            }
            _receiveMessageCollection.Clear();
        }



        private void ReceiveMessageThreadMethod()
        {
            while (_isRunning)
            {
                //判断缓冲区内是否有可读的元素，如果没有直接返回
                if (_internalSocket.Available <= 0)
                {
                    Thread.Sleep(DefaultSleepMilliseconds);
                    continue;
                }

                //接收队列中是否存在需要处理的元素，没有则直接返回
                if (_receiveMessageCollection.IsEmpty)
                {
                    Thread.Sleep(DefaultSleepMilliseconds);
                    continue;
                }


                //如果读取MBAP头部失败，直接返回并休眠
                ModbusApplicationProtocolPart mbap = ReceiveApplicationProtocol();
                if (mbap == null)
                {
                    Thread.Sleep(DefaultSleepMilliseconds);
                    continue;
                }

                //再次判断是否存在有接收的数据，没有则直接返回并休眠
                if (_internalSocket.Available <= 0)
                {
                    Thread.Sleep(DefaultSleepMilliseconds);
                    continue;
                }

                byte[] receivedFunctionCode = new byte[1];
                int receivedLen = _internalSocket.Receive(receivedFunctionCode, 0, 1, SocketFlags.None);
                if (receivedLen != 1)
                {
                    Thread.Sleep(DefaultSleepMilliseconds);
                    continue;
                }

                BaseRequestMessage message = _receiveMessageCollection.FirstOrDefault(c => c.TransactionId == mbap.TransactionId);
                if (message == null)
                {
                    Logger.Error($"接收队列中没有找到事务Id对应的消息体；事务Id：{mbap.TransactionId}");
                    Thread.Sleep(DefaultSleepMilliseconds);
                    continue;
                }

                //如果返回的是发出指令失败的提示，则直接返回并休眠
                if (receivedFunctionCode[0] == message.FunctionCode + 0x80)
                {

                    byte[] receivedExceptionCode = new byte[1];
                    receivedLen = _internalSocket.Receive(receivedExceptionCode, 0, 1, SocketFlags.None);
                    if (receivedLen != 1)
                    {
                        Thread.Sleep(DefaultSleepMilliseconds);
                        continue;
                    }

                    ExceptionCodeDefinition exceptionCode = (ExceptionCodeDefinition)receivedExceptionCode[0];

                    ExceptionResponse exceptionResponse = new ExceptionResponse(exceptionCode);


                }

                switch (message.FunctionCode)
                {
                    case FunctionCodeDefinition.READ_COILS:
                        ReadCoilsResponse();



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


                Thread.Sleep(DefaultSleepMilliseconds);

            }
        }

        private void ReadCoilsResponse(BaseRequestMessage message)
        {

            ReadCoilsRequestMessage requestMessage = (ReadCoilsRequestMessage)message;
            List<bool> values = new List<bool>(requestMessage.Quantity);
            try
            {
                //获取需要读取的字节数量
                byte byteCount = 0;
                if (_internalSocket.Available > 0)
                {
                    byte[] buffer = new byte[1];
                    _internalSocket.Receive(buffer, 0, 1, SocketFlags.None);
                    byteCount = buffer[0];
                }

                Span<byte> receivedBytes = stackalloc byte[byteCount];
                int receive = _internalSocket.Receive(receivedBytes, SocketFlags.None);
                if (receive != byteCount)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    Logger.Error("读取线圈量失败，未能从缓冲区读取到有效的字节");
                    return;
                }

                BitArray bitArray = new BitArray(receivedBytes.Slice(1).ToArray());
                for (var i = 0; i < requestMessage.Quantity; i++)
                {
                    values.Add(bitArray[i]);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;

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
            while (_internalSocket.Available > 0)
            {
                //如果连接已经中断直接返回null
                if (_internalSocket.Connected == false)
                    return null;

                try
                {
                    int receivedSize = _internalSocket.Receive(buffer, startOffset, remainLen, SocketFlags.None);
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
    }
}
