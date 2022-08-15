using System;
using System.Runtime.InteropServices;

namespace ModbusNet.Message.Request
{
    /// <summary>
    /// 读取离散量输入
    /// 请求格式和读取线圈量是一致的，直接继承读取线圈量即可
    /// </summary>
    public class ReadDiscreteInputsMessage : ReadCoilsMessage
    {
        private const int MESSAGE_LENGTH = 12;

        public override byte FunctionCode => FunctionCodeDefinition.READ_DISCRETE_INPUTS;

        public override Span<byte> ToBinary()
        {
            this.NativePtr = Marshal.AllocHGlobal(MESSAGE_LENGTH);
            Span<byte> nativeSpan = null;
            unsafe
            {
                nativeSpan = new Span<byte>(this.NativePtr.ToPointer(), MESSAGE_LENGTH);
            }

            BuildMBAP(nativeSpan);

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
            return (ushort)6;
        }
    }


}

