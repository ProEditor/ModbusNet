using System;
namespace ModbusNet
{
    public static class ByteArrayExtension
    {

        /// <summary>
        /// 将小端序字节数组转换成大端序数组
        /// </summary>
        /// <param name="raws">原始的小端序数组</param>
        /// <returns></returns>
        public static byte[] ToPlatform(this byte[] raws)
        {
            if (BitConverter.IsLittleEndian)
            {
                var span=raws.AsSpan();
                span.Reverse();
                return span.ToArray();
            }
            return raws;
        }


    }
}

