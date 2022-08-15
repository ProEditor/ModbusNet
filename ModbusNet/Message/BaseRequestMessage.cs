using System;
using System.Runtime.InteropServices;

namespace ModbusNet.Message
{
    public abstract class BaseRequestMessage : IDisposable
    {
        protected IntPtr NativePtr { get; set; } = IntPtr.Zero;


        /// <summary>
        /// 单元标识符
        /// </summary>
        public byte UnitId { get; set; } = 0;


        /// <summary>
        /// 事务Id
        /// </summary>
        public ushort TransactionId { get; set; }

        /// <summary>
        /// 功能码
        /// </summary>
        public abstract byte FunctionCode { get; }


        public abstract Span<byte> ToBinary();


        /// <summary>
        /// 获取MBAP中“剩余字节大小”
        /// </summary>
        /// <returns></returns>
        protected abstract ushort GetRemainByteCount();

        /// <summary>
        /// 构建MBAP部分+功能码
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <param name="remainByte">剩余长度</param>
        /// <param name="reqSpan">返回的字节数组</param>
        protected void BuildMBAP(Span<byte> reqSpan)
        {

            var transactionBytes = BitConverter.GetBytes(TransactionId).ToPlatform();
            reqSpan[0] = transactionBytes[0];
            reqSpan[1] = transactionBytes[1];


            //协议标识符，默认为0x0000
            reqSpan[2] = 0x00;
            reqSpan[3] = 0x00;


            //1byte 单元标识符+1byte功能码+2byte开始地址+2byte数量=6字节
            //后续的字节长度，占用2字节
            byte[] remainBytes = BitConverter.GetBytes(GetRemainByteCount()).ToPlatform();
            reqSpan[4] = remainBytes[0];
            reqSpan[5] = remainBytes[1];


            //单元标识符，占用1个字节
            reqSpan[6] = UnitId;

            //功能码，占用1个字节
            reqSpan[7] = FunctionCode;

        }

        public void Dispose()
        {
            if (NativePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(NativePtr);
            }
        }
    }
}
