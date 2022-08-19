using System;
using System.Net.Sockets;
using ModbusNet.Message;
using NLog;
namespace ModbusNet
{
    public class TcpModbusSendThread : AbstractExecuteThread
    {

        private readonly static Logger Logger = LogManager.GetCurrentClassLogger();

        public TcpModbusSendThread(TcpMasterOption option, Socket internalSocket) : base("TcpModbusSendThread", option, internalSocket)
        {

        }

        protected override void Execute()
        {

            var messageQueue = GetMessageQueue();
            if (messageQueue.IsEmpty == false)
            {
                messageQueue.TryDequeue(out BaseRequestMessage message);
                if (message == null)
                {
                    Logger.Warn("没有从发送队列中获取到需要发送的消息");
                    return;
                }

                try
                {
                    GetSocket().Send(message.ToBinary());
                }
                catch (Exception ex)
                {
                    message.Dispose();
                    Logger.Error(ex, "发送指令到从站失败");
                }

            }
        }

    }
}
