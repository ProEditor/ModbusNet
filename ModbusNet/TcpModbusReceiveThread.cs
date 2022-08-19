using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using ModbusNet.Enum;
using ModbusNet.Message;
using ModbusNet.Message.Request;
using NLog;
namespace ModbusNet
{
    public class TcpModbusReceiveThread : AbstractExecuteThread
    {

        private readonly static Logger Logger = LogManager.GetCurrentClassLogger();

        public TcpModbusReceiveThread(TcpMasterOption option, Socket internalSocket) : base("TcpModbusReceiveThread", option, internalSocket)
        {

        }

        protected override void Execute()
        {

            Socket socket = GetSocket();
            //判断缓冲区内是否有可读的元素，如果没有直接返回
            if (socket.Available <= 0)
            {
                Thread.Sleep(DefaultSleepMilliseconds);
                return;
            }

            //接收队列中是否存在需要处理的元素，没有则直接返回
            if (GetMessageQueue().IsEmpty)
            {
                Thread.Sleep(DefaultSleepMilliseconds);
                return;
            }


            //如果读取MBAP头部失败，直接返回并休眠
            ModbusApplicationProtocolPart mbap = ReceiveApplicationProtocol();
            if (mbap == null)
            {
                Thread.Sleep(DefaultSleepMilliseconds);
                return;
            }


            byte[] respFunctionCode = new byte[1];
            int receivedLen = socket.Receive(respFunctionCode, 0, 1, SocketFlags.None);
            if (receivedLen != 1)
            {
                Thread.Sleep(DefaultSleepMilliseconds);
                return;
            }

            BaseRequestMessage message = GetMessageQueue().FirstOrDefault(c => c.TransactionId == mbap.TransactionId);
            if (message == null)
            {
                Logger.Error($"接收队列中没有找到事务Id对应的消息体；事务Id：{mbap.TransactionId}");
                Thread.Sleep(DefaultSleepMilliseconds);
                return;
            }

            //如果返回的是发出指令失败的提示，则直接返回并休眠
            if (respFunctionCode[0] == message.FunctionCode + 0x80)
            {

                byte[] receivedExceptionCode = new byte[1];
                receivedLen = socket.Receive(receivedExceptionCode, 0, 1, SocketFlags.None);
                if (receivedLen != 1)
                {
                    Thread.Sleep(DefaultSleepMilliseconds);
                    return;
                }

                message.Callback(new TcpModbusResponse(mbap.TransactionId, (ExceptionCodeDefinition)receivedExceptionCode[0]));

            }

            switch (message.FunctionCode)
            {
                case FunctionCodeDefinition.READ_COILS:
                    var coilsStatus = ReadCoilsResponse(message);
                    message.Callback(new TcpModbusResponse(mbap.TransactionId, coilsStatus));
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

        private List<bool> ReadCoilsResponse(BaseRequestMessage message)
        {
            ReadCoilsRequestMessage requestMessage = (ReadCoilsRequestMessage)message;
            List<bool> values = new List<bool>(requestMessage.Quantity);

            int byteCount = 1 + (requestMessage.Quantity % 8 == 0 ? requestMessage.Quantity / 8 : requestMessage.Quantity / 8 + 1);
            var buffer = ReceiveRawDataSpan(byteCount);
            if (buffer != null)
            {
                Span<byte> bufferSpan = new Span<byte>(buffer);
                BitArray bitArray = new BitArray(bufferSpan.Slice(1).ToArray());
                for (var i = 0; i < requestMessage.Quantity; i++)
                {
                    values.Add(bitArray[i]);
                }
            }
            return values;
        }

        private byte[] ReceiveRawDataSpan(int byteCount)
        {
            //累计接收的长度
            int fullReceivedSize = 0;

            //剩余需要接收的长度
            int remainLen = byteCount;

            //开始往Buffer中写入的开始偏移量
            int startOffset = 0;

            byte[] buffer = new byte[byteCount];
            int retryCount = 1;

            Socket socket = GetSocket();

            while (true)
            {
                //如果连接已经中断直接返回null
                if (socket.Connected == false)
                    return null;

                if (socket.Available == 0)
                    continue;


                if (retryCount == 5)
                {
                    Logger.Error("从Socket中读取数据失败，累计重试了5次仍未能有效的读取到数据");
                    return null;
                }

                try
                {
                    int receivedSize = socket.Receive(buffer, startOffset, remainLen, SocketFlags.None);
                    fullReceivedSize += receivedSize;
                    if (fullReceivedSize == byteCount)
                    {
                        return buffer;
                    }
                    remainLen = byteCount - fullReceivedSize;
                    startOffset = fullReceivedSize;
                }
                catch (SocketException e)
                {
                    Logger.Error(e, "读取字节流失败");
                }

                Thread.Sleep(DefaultSleepMilliseconds);
                retryCount += 1;

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

            Socket socket = GetSocket();
            while (socket.Available > 0)
            {
                //如果连接已经中断直接返回null
                if (socket.Connected == false)
                    return null;

                try
                {
                    int receivedSize = socket.Receive(buffer, startOffset, remainLen, SocketFlags.None);
                    fullReceivedSize += receivedSize;

                    if (fullReceivedSize == modbusAppProtocolLen)
                    {
                        ModbusApplicationProtocolPart mbap = new ModbusApplicationProtocolPart
                        {
                            TransactionId = BitConverter.ToUInt16(BitConverter.IsLittleEndian ? new[] { buffer[1], buffer[0] } : buffer, 0),
                            ProtocolId = 0,
                            UnitId = buffer[6],
                            Length = BitConverter.IsLittleEndian
                                ? BitConverter.ToUInt16(new[] { buffer[5], buffer[4] }, 0)
                                : BitConverter.ToUInt16(buffer, 4)
                        };
                        return mbap;
                    }

                    remainLen = modbusAppProtocolLen - fullReceivedSize;
                    startOffset = fullReceivedSize;

                }
                catch (SocketException e)
                {
                    Logger.Error(e, "从套接字中获取MBAP失败");
                }

            }
            return null;
        }


    }
}
