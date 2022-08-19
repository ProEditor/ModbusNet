using System;
using System.Runtime.InteropServices;
using ModbusNet.Enum;
using ModbusNet.Extension;

namespace ModbusNet.Message.Request
{
    /// <summary>
    /// 读取离散量输入
    /// 请求格式和读取线圈量是一致的，直接继承读取线圈量即可
    /// </summary>
    public class ReadDiscreteInputsRequestMessage : ReadCoilsRequestMessage
    {
        private const int MessageLength = 12;

        public override byte FunctionCode => FunctionCodeDefinition.READ_DISCRETE_INPUTS;

        public override Span<byte> ToBinary()
        {
            this.NativePtr = Marshal.AllocHGlobal(MessageLength);
            Span<byte> nativeSpan;
            unsafe
            {
                nativeSpan = new Span<byte>(this.NativePtr.ToPointer(), MessageLength);
            }

            BuildMbap(nativeSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();
            nativeSpan[8] = addressBytes[0];
            nativeSpan[9] = addressBytes[1];

            byte[] quantityBytes = BitConverter.GetBytes(Quantity).ToPlatform();
            nativeSpan[10] = quantityBytes[0];
            nativeSpan[11] = quantityBytes[1];

            return nativeSpan;
        }

        protected override ushort GetRemainByteCount()
        {
            return 6;
        }
    }


}
