using System;
using System.Runtime.InteropServices;

namespace ModbusNet.Message.Request
{
    public class WriteSingleRegisterRequestMessage : BaseMessage
    {
        private const int MESSAGE_LENGTH = 12;

        public override byte FunctionCode => FunctionCodeDefinition.WRITE_SINGLE_REGISTER;

        /// <summary>
        /// 需要写入的线圈量地址
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// 写入的状态
        /// </summary>
        public short Value { get; set; }

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

            byte[] valueBytes = BitConverter.GetBytes(Value).ToPlatform();
            nativeSpan[10] = valueBytes[0];
            nativeSpan[11] = valueBytes[1];


            return nativeSpan;
        }

        protected override ushort GetRemainByteCount()
        {
            return 6;
        }
    }
}

