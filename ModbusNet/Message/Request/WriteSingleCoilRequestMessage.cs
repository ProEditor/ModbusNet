using System;
using System.Runtime.InteropServices;
using ModbusNet.Enum;
using ModbusNet.Extension;

namespace ModbusNet.Message.Request
{
    public class WriteSingleCoilRequestMessage : BaseRequestMessage
    {

        private const int MessageLength = 12;

        public override byte FunctionCode => FunctionCodeDefinition.WRITE_SINGLE_COIL;

        /// <summary>
        /// 需要写入的线圈量地址
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// 写入的状态
        /// </summary>
        public bool CoilStatus { get; set; }

        public override Span<byte> ToBinary()
        {

            this.NativePtr = Marshal.AllocHGlobal(MessageLength);
            Span<byte> nativeSpan;
            unsafe
            {
                nativeSpan = new Span<byte>(NativePtr.ToPointer(), MessageLength);
            }

            BuildMbap(nativeSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();

            nativeSpan[8] = addressBytes[0];
            nativeSpan[9] = addressBytes[1];

            if (CoilStatus)
            {
                nativeSpan[10] = 0xFF;
                nativeSpan[11] = 0x00;
            }
            else
            {
                nativeSpan[10] = 0x00;
                nativeSpan[11] = 0x00;
            }
            return nativeSpan;
        }

        protected override ushort GetRemainByteCount()
        {
            return 6;
        }
    }
}
