using System;
namespace ModbusNet.Message.Response
{
    public class ExceptionResponse : BaseResponseMessage
    {

        public ExceptionResponse(ExceptionCodeDefinition exceptionCode)
        {
            ExceptionCode = exceptionCode;
        }


        /// <summary>
        /// 错误码
        /// </summary>
        public ExceptionCodeDefinition ExceptionCode { get; }

        /// <summary>
        /// 错误码的中文说明
        /// </summary>
        public string Remark => GetExceptionCodeRemark(ExceptionCode);


        public static string GetExceptionCodeRemark(ExceptionCodeDefinition exceptionCode)
        {
            switch (exceptionCode)
            {
                case ExceptionCodeDefinition.IllegalFunction:
                    return "非法功能。对于服务器（或从站）来说，询问中接收到的功能码是不可允许的操作，可能是因为功能码仅适用于新设备而被选单元中不可实现同时，" +
                           "还指出服务器（或从站）在错误状态中处理这种请求，例如：它是未配置的，且要求返回寄存器值。";

                case ExceptionCodeDefinition.IllegalDataAddress:
                    return "非法数据地址。对于服务器（或从站）来说，询问中接收的数据地址是不可允许的地址，特别是参考号和传输长度的组合是无效的。" +
                           "对于带有100个寄存器的控制器来说，偏移量96和长度4的请求会成功，而偏移量96和长度5的请求将产生异常码02。";

                case ExceptionCodeDefinition.IllegalDataValue:
                    return "非法数据值。对于服务器（或从站）来说，询问中包括的值是不可允许的值。该值指示了组合请求剩余结构中的故障。例如：隐含长度是不正确的。" +
                           "modbus协议不知道任何特殊寄存器的任何特殊值的重要意义，寄存器中被提交存储的数据项有一个应用程序期望之外的值。";

                case ExceptionCodeDefinition.SlaveDeviceFailure:
                    return "从站设备故障。当服务器（或从站）正在设法执行请求的操作时，产生不可重新获得的差错。";

                case ExceptionCodeDefinition.Acknowledge:
                    return "确认。与编程命令一起使用，服务器（或从站）已经接受请求，并且正在处理这个请求，但是需要长持续时间进行这些操作，" +
                           "返回这个响应防止在客户机（或主站）中发生超时错误，客户机（或主机）可以继续发送轮询程序完成报文来确认是否完成处理。";

                case ExceptionCodeDefinition.SlaveDeviceBusy:
                    return "从属设备忙。与编程命令一起使用。服务器(或从站)正在处理长持续时间的程序命令。张服务器(或从站)空闲时，用户(或主站)应该稍后重新传输报文。";

                case ExceptionCodeDefinition.MemoryParityError:
                    return "存储奇偶差错。与功能码20和21以及参考类型6一起使用，指示扩展文件区不能通过一致性校验。服务器(或从站)设法读取记录文件，" +
                           "但是在存储器中发现一个奇偶校验错误。客户机(或主方)可以重新发送请求，但可以在服务器(或从站)设备上要求服务。";

                case ExceptionCodeDefinition.GatewayPathUnavailable:
                    return "不可用网关路径。与网关一起使用，指示网关不能为处理请求分配输入端口至输出端口的内部通信路径。通常意味着网关是错误配置的或过载的。";

                case ExceptionCodeDefinition.GatewayTargetFailedToRespondDevice:
                    return "网关目标设备响应失败。与网关一起使用，指示没有从目标设备中获得响应。通常意味着设备未在网络中。";
            }
            return String.Empty;


        }


        public override string ToString()
        {
            return $"ExceptionCode:{ExceptionCode},ExceptionCodeStr:{ExceptionCode.ToString()},Remark:{Remark}";
        }
    }
}
