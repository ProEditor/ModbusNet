using System;
namespace ModbusNet
{
    /// <summary>
    /// Modbus官方定义的功能码
    /// </summary>
    public static class FunctionCodeDefinition
    {

        /// <summary>
        /// 读取输入离散量
        /// </summary>
        public const byte READ_DISCRETE_INPUTS = 0x02;


        /// <summary>
        /// 读取线圈量
        /// </summary>
        public const byte READ_COILS = 0x01;


        /// <summary>
        /// 写入单个线圈量
        /// </summary>
        public const byte WRITE_SINGLE_COIL = 0x05;


        /// <summary>
        /// 写入多个线圈量
        /// </summary>
        public const byte WRITE_MULTIPLE_COILS = 0x0F;


        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        public const byte READ_INPUT_REGISTERS = 0x04;


        /// <summary>
        /// 读取保持寄存器
        /// </summary>
        public const byte READ_HOLDING_REGISTERS = 0x03;


        /// <summary>
        /// 写入单个寄存器
        /// </summary>
        public const byte WRITE_SINGLE_REGISTER = 0x06;

        /// <summary>
        /// 写入多个寄存器
        /// </summary>
        public const byte WRITE_MULTIPLE_REGISTERS = 0x10;


        /// <summary>
        /// 读写多个寄存器，将一个读操作和一个写操作合并成一个操作，写操作执行在读操作之后
        /// </summary>
        public const byte READ_WRITE_MULTIPLE_REGISTERS=0x17;

    }
}

