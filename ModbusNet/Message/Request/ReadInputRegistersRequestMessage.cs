using System;
using System.Runtime.InteropServices;

namespace ModbusNet.Message.Request
{
    public class ReadInputRegistersRequestMessage:AbstractRegistersRequestRequestMessage
    {
        private const int MESSAGE_LENGTH = 12;

        public override byte FunctionCode => FunctionCodeDefinition.READ_INPUT_REGISTERS;

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

            ushort actualQuantity = GetActualQuantityCount(Quantity);
            byte[] quantityBytes = BitConverter.GetBytes(actualQuantity).ToPlatform();

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

