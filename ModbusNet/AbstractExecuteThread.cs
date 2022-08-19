using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using ModbusNet.Message;
using NLog;
namespace ModbusNet
{
    public abstract class AbstractExecuteThread
    {
        private readonly static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 单次循环周期线程休眠的时长
        /// </summary>
        protected const int DefaultSleepMilliseconds = 2;

        /// <summary>
        /// 执行线程是否在运行中
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// 执行线程的名称，方便日志输出已经调试的目的
        /// </summary>
        private readonly string _executeName;

        /// <summary>
        /// 内部的套接字
        /// </summary>
        private readonly Socket _internalSocket;

        /// <summary>
        /// 单次重试间隔
        /// </summary>
        private readonly TimeSpan _retryGap = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 上次重试时间
        /// </summary>
        private DateTime _lastRetryTime = DateTime.MinValue;

        /// <summary>
        /// 累计重试次数
        /// </summary>
        private long _accumulativeRetryCount;

        /// <summary>
        /// 执行线程
        /// </summary>
        private Thread _executeThread;

        private readonly TcpMasterOption _tcpMasterOption;

        /// <summary>
        /// 消息发送队列
        /// </summary>
        private readonly ConcurrentQueue<BaseRequestMessage> _requestMessageQueue = new ConcurrentQueue<BaseRequestMessage>();

        protected AbstractExecuteThread(string executeName, TcpMasterOption option, Socket internalSocket)
        {
            _executeName = executeName;
            _internalSocket = internalSocket;
            _tcpMasterOption = option;
        }

        /// <summary>
        /// 返回套接字对象
        /// </summary>
        /// <returns></returns>
        protected Socket GetSocket()
        {
            return _internalSocket;
        }

        public void Start()
        {
            _isRunning = true;
            _executeThread = new Thread(InternalExecute)
            {
                IsBackground = true,
                Name = _executeName
            };
            _executeThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;
        }


        private void InternalExecute()
        {
            while (_isRunning)
            {
                //如果连接已经中断，需要自动重连
                if (!_internalSocket.Connected)
                {
                    RetryConnect();
                }

                //由子类实现具体的执行逻辑
                Execute();

                Thread.Sleep(DefaultSleepMilliseconds);
            }
        }

        /// <summary>
        /// 重试连接
        /// </summary>
        private void RetryConnect()
        {
            //如果从未重连过，则上次重连时间为DateTime.MinValue
            if (_lastRetryTime == DateTime.MinValue)
            {
                RetryConnectSocket();
                return;
            }
            //如果上次重试时间与当前时间的间隔大于等于
            if (DateTime.Now.Subtract(_lastRetryTime) > _retryGap)
            {
                RetryConnectSocket();
                return;
            }
            Logger.Info($"未到重试时间，系统将在{(DateTime.Now - _lastRetryTime).Seconds}秒后重试");
        }

        private void RetryConnectSocket()
        {
            try
            {
                Interlocked.Increment(ref _accumulativeRetryCount);
                _lastRetryTime = DateTime.Now;
                _internalSocket.Connect(_tcpMasterOption.IPAddress, _tcpMasterOption.Port);

                //如果重新连接成功，则重置重试次数为0
                if (_internalSocket.Connected)
                {
                    _accumulativeRetryCount = 0;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"重建套接字连接失败，重试时间：{_lastRetryTime:yyyy-MM-dd HH:mm:ss},累计重试次数：{_accumulativeRetryCount}");
            }
        }

        /// <summary>
        /// 线程需要执行的具体事务
        /// </summary>
        protected abstract void Execute();


        /// <summary>
        /// 获取请求队列
        /// </summary>
        /// <returns>返回当前请求队列</returns>
        public ConcurrentQueue<BaseRequestMessage> GetMessageQueue()
        {
            return _requestMessageQueue;
        }

        public void Add(BaseRequestMessage message)
        {
            _requestMessageQueue.Enqueue(message);
        }

        public void Remove()
        {

        }

    }
}
