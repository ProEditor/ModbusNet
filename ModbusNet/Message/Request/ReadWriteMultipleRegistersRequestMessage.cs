using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ModbusNet.Message.Request
{
    public class ReadWriteMultipleRegistersRequestMessage : AbstractRegistersRequestMessage
    {
        public override byte FunctionCode => FunctionCodeDefinition.READ_WRITE_MULTIPLE_REGISTERS;

        private ushort multipleReadWriteRegistersRemainByteNum = 0;

        /// <summary>
        /// 读写多个寄存器时的读取开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort ReadStartingAddress { get; set; }

        /// <summary>
        /// 读取的数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort ReadQuantity { get; set; }

        /// <summary>
        /// 读写多个寄存器时的写入时的开始地址
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort WriteStartingAddress { get; set; }

        /// <summary>
        /// 写入数量
        /// 仅用于(0x17) Read/Write Multiple registers
        /// </summary>
        public ushort WriteQuantity { get; set; }

        public List<object> Values { get; set; }

        public override Span<byte> ToBinary()
        {
            //1字节的功能码，2字节的开始地址，2字节的数量，1字节的字节数量
            ushort shouldSendNums = 7 + 1 + 2 + 2 + 2 + 2 + 1;

            //1字节的单元标识符，1字节的功能码，2字节的开始地址，2字节的数量，1字节的地址数量
            multipleReadWriteRegistersRemainByteNum = 1 + 1 + 2 + 2 + 2 + 2 + 1;

            var actualByteNum = GetActualByteCount();

            shouldSendNums += actualByteNum;
            multipleReadWriteRegistersRemainByteNum += actualByteNum;


            this.NativePtr = Marshal.AllocHGlobal(shouldSendNums);
            Span<byte> nativeSpan = null;
            unsafe
            {
                nativeSpan = new Span<byte>(this.NativePtr.ToPointer(), shouldSendNums);
            }

            BuildMBAP(nativeSpan);

            byte[] readStartingAddress = BitConverter.GetBytes(ReadStartingAddress).ToPlatform();
            nativeSpan[8] = readStartingAddress[0];
            nativeSpan[9] = readStartingAddress[1];


            ushort actualReadQuantity = GetActualQuantityCount(ReadQuantity);
            byte[] quantityBytes = BitConverter.GetBytes(actualReadQuantity).ToPlatform();
            nativeSpan[10] = quantityBytes[0];
            nativeSpan[11] = quantityBytes[1];


            byte[] writeStartingAddress = BitConverter.GetBytes(WriteStartingAddress).ToPlatform();
            nativeSpan[12] = writeStartingAddress[0];
            nativeSpan[13] = writeStartingAddress[1];


            ushort actualWriteQuantity = GetActualQuantityCount(WriteQuantity);
            byte[] writeQuantityBytes = BitConverter.GetBytes(actualWriteQuantity).ToPlatform();
            nativeSpan[14] = writeQuantityBytes[0];
            nativeSpan[15] = writeQuantityBytes[1];


            nativeSpan[16] = (byte)actualByteNum;
            int index = 16;

            if (NumericalType == NumericalTypeEnum.Short)
            {
                for (int i = 0; i < Values.Count; i++)
                {
                    var value = (short)Values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    index += 1;
                    nativeSpan[index] = bytes[0];

                    index += 1;
                    nativeSpan[index] = bytes[1];
                }
            }
            else if (NumericalType == NumericalTypeEnum.Integer)
            {

                for (int i = 0; i < Values.Count; i++)
                {
                    var value = (int)Values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        nativeSpan[index] = bytes[j];
                    }
                }
            }
            else if (NumericalType == NumericalTypeEnum.Float)
            {

                for (int i = 0; i < Values.Count; i++)
                {
                    var value = (float)Values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        nativeSpan[index] = bytes[j];
                    }
                }
            }
            else if (NumericalType == NumericalTypeEnum.Double)
            {
                for (int i = 0; i < Values.Count; i++)
                {
                    var value = (double)Values[i];
                    var bytes = BitConverter.GetBytes(value).ToPlatform();
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        index += 1;
                        nativeSpan[index] = bytes[j];
                    }
                }

            }

            return nativeSpan;
        }



        protected override ushort GetRemainByteCount()
        {
            return multipleReadWriteRegistersRemainByteNum;
        }
    }
}

