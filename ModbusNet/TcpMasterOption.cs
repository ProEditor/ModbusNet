using System;
namespace ModbusNet
{
    public class TcpMasterOption
    {
        /// <summary>
        /// Modbus从站的IP地址
        /// </summary>
        public string IPAddress { get; set; } = "127.0.0.1";


        /// <summary>
        /// Modbus从站的端口
        /// </summary>
        public int Port { get; set; } = 502;


        /// <summary>
        /// 从站Id
        /// </summary>
        public int SlaveId { get; set; } = 1;

        /// <summary>
        /// 字节序列的标识方式，默认为“大端序”
        /// </summary>
        public EncodingModeEnum EncodingMode { get; set; } = EncodingModeEnum.BigEndian;


        /// <summary>
        /// 是否为Debug模式，设置该属性为True将会输出全部的请求，响应字节信息
        /// </summary>
        public bool Debug { get; set; } = false;


        /// <summary>
        /// 当连接中断时，是否重试
        /// </summary>
        public bool Retry { get; set; } = true;


        /// <summary>ßß
        /// 当连接中断时，重试的最大次数
        /// </summary>
        public int MaxRetry { get; set; } = int.MaxValue;


        /// <summary>
        /// 重试间隔，单位（秒），默认是5秒
        /// </summary>
        public int RetryInterval { get; set; } = 5;

    }
}

