using System;
using System.Collections.Generic;

namespace ModbusNet
{
    public interface IModbusMaster
    {

        #region 读取多个线圈量，离散量输入

        public void ReadCoils(ushort start, ushort quantity, ModbusSendCallback callback);

        public void ReadDiscreteInputs(ushort start, ushort quantity, ModbusSendCallback callback);

        #endregion

        #region 读取多个保持寄存器

        public void ReadInt16HoldingRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        public void ReadInt32HoldingRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        public void ReadFloatHoldingRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        public void ReadDoubleHoldingRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        #endregion


        #region 读取多个输入寄存器

        public void ReadInputRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        public void ReadIntInputRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        public void ReadFloatInputRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        public void ReadDoubleInputRegisters(ushort start, ushort quantity, ModbusSendCallback callback);

        #endregion

        #region 写入单个值

        public void WriteSingleCoil(ushort start, bool status, ModbusSendCallback callback);

        public void WriteSingleRegister(ushort start, short value, ModbusSendCallback callback);

        public void WriteIntSingleRegister(ushort start, int value, ModbusSendCallback callback);

        public void WriteFloatSingleRegister(ushort start, float value, ModbusSendCallback callback);

        public void WriteDoubleSingleRegister(ushort start, double value, ModbusSendCallback callback);

        #endregion

        #region 写入多个值

        public void WriteMultipleCoils(ushort start, List<bool> values, ModbusSendCallback callback);

        public void WriteMultipleRegisters(ushort start, List<short> values, ModbusSendCallback callback);

        public void WriteIntMultipleRegisters(ushort start, List<int> values, ModbusSendCallback callback);

        public void WriteFloatMultipleRegisters(ushort start, List<float> values, ModbusSendCallback callback);

        public void WriteDoubleMultipleRegisters(ushort start, List<double> values, ModbusSendCallback callback);

        #endregion



        #region 读写多个寄存器

        public void ReadWriteMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<short> values,
            ModbusSendCallback callback);

        public void ReadWriteIntMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<int> values,
            ModbusSendCallback callback);

        public void ReadWriteFloatMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<float> values,
            ModbusSendCallback callback);

        public void ReadWriteDoubleMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<double> values,
            ModbusSendCallback callback);

        #endregion

    }
}
