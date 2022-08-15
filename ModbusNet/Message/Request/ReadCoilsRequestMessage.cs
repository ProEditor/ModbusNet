using System;
using System.Runtime.InteropServices;

namespace ModbusNet.Message
{
    public class ReadCoilsMessage : AbstractReadRequestMessage
    {

        private const int MessageLength = 12;

        public override byte FunctionCode => FunctionCodeDefinition.READ_COILS;

        public override Span<byte> ToBinary()
        {
            this.NativePtr = Marshal.AllocHGlobal(MessageLength);
            Span<byte> nativeSpan = null;
            unsafe
            {
                nativeSpan = new Span<byte>(this.NativePtr.ToPointer(), MessageLength);
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

