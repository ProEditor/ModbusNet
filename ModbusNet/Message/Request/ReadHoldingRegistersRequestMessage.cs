using System;
using System.Runtime.InteropServices;
using ModbusNet.Enum;
using ModbusNet.Extension;

namespace ModbusNet.Message.Request
{
    public class ReadHoldingRegistersRequestMessage : AbstractRegistersRequestRequestMessage
    {
        private const int MessageLength = 12;

        public override byte FunctionCode => FunctionCodeDefinition.READ_HOLDING_REGISTERS;

        public override Span<byte> ToBinary()
        {
            this.NativePtr = Marshal.AllocHGlobal(MessageLength);
            Span<byte> nativeSpan = null;
            unsafe
            {
                nativeSpan = new Span<byte>(this.NativePtr.ToPointer(), MessageLength);
            }

            BuildMbap(nativeSpan);


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
