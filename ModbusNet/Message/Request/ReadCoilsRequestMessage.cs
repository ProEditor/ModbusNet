﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ModbusNet.Enum;
using ModbusNet.Extension;

namespace ModbusNet.Message.Request
{
    public class ReadCoilsRequestMessage : AbstractReadRequestRequestMessage
    {

        private const int MessageLength = 12;

        public override byte FunctionCode => FunctionCodeDefinition.READ_COILS;

        public override Span<byte> ToBinary()
        {
            NativePtr = Marshal.AllocHGlobal(MessageLength);
            Span<byte> nativeSpan;
            unsafe
            {
                nativeSpan = new Span<byte>(NativePtr.ToPointer(), MessageLength);
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
