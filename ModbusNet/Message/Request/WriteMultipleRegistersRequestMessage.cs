using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ModbusNet.Message.Request
{
    public class WriteMultipleRegistersRequestMessage : AbstractRegistersRequestRequestMessage
    {

        public override byte FunctionCode => FunctionCodeDefinition.WRITE_MULTIPLE_REGISTERS;

        private ushort multipleWriteRegistersRemainByteNum;


        public List<object> Values { get; set; }


        public override Span<byte> ToBinary()
        {
            //1字节的功能码，2字节的开始地址，2字节的数量，1字节的字节数量
            ushort shouldSendNums = 7 + 1 + 2 + 2 + 1;

            //1字节的单元标识符，1字节的功能码，2字节的开始地址，2字节的数量，1字节的地址数量
            multipleWriteRegistersRemainByteNum = 1 + 1 + 2 + 2 + 1;

            var actualByteNum = GetActualByteCount();

            shouldSendNums += actualByteNum;
            multipleWriteRegistersRemainByteNum += actualByteNum;


            this.NativePtr = Marshal.AllocHGlobal(shouldSendNums);
            Span<byte> nativeSpan = null;
            unsafe
            {
                nativeSpan = new Span<byte>(this.NativePtr.ToPointer(), shouldSendNums);
            }

            BuildMBAP(nativeSpan);

            byte[] addressBytes = BitConverter.GetBytes(Address).ToPlatform();
            nativeSpan[8] = addressBytes[0];
            nativeSpan[9] = addressBytes[1];


            ushort actualQuantity = GetActualQuantityCount(Quantity);
            byte[] quantityBytes = BitConverter.GetBytes(actualQuantity).ToPlatform();
            nativeSpan[10] = quantityBytes[0];
            nativeSpan[11] = quantityBytes[1];

            nativeSpan[12] = (byte)actualByteNum;

            int index = 12;

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
            return multipleWriteRegistersRemainByteNum;
        }
    }
}

