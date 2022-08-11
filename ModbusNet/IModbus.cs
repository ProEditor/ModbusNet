using System.Collections.Generic;

namespace ModbusNet
{
    public interface IModbusMaster
    {

        #region 读取多个线圈量，离散量输入

        public void ReadCoils(ushort start, ushort quantity);

        public void ReadDiscreteInputs(ushort start, ushort quantity);

        #endregion

        #region 读取多个保持寄存器

        public void ReadInt16HoldingRegisters(ushort start, ushort quantity);

        public void ReadInt32HoldingRegisters(ushort start, ushort quantity);

        public void ReadFloatHoldingRegisters(ushort start, ushort quantity);

        public void ReadDoubleHoldingRegisters(ushort start, ushort quantity);

        #endregion


        #region 读取多个输入寄存器

        public void ReadInputRegisters(ushort start, ushort quantity);

        public void ReadIntInputRegisters(ushort start, ushort quantity);

        public void ReadFloatInputRegisters(ushort start, ushort quantity);

        public void ReadDoubleInputRegisters(ushort start, ushort quantity);

        #endregion

        #region 写入单个值

        public void WriteSingleCoil(ushort start, bool status);

        public void WriteSingleRegister(ushort start, short value);

        public void WriteIntSingleRegister(ushort start, int value);

        public void WriteFloatSingleRegister(ushort start, float value);

        public void WriteDoubleSingleRegister(ushort start, double value);

        #endregion

        #region 写入多个值

        public void WriteMultipleCoils(ushort start, List<bool> values);

        public void WriteMultipleRegisters(ushort start, List<short> values);

        public void WriteIntMultipleRegisters(ushort start, List<int> values);

        public void WriteFloatMultipleRegisters(ushort start, List<float> values);

        public void WriteDoubleMultipleRegisters(ushort start, List<double> values);

        #endregion



        #region 读写多个寄存器

        public void ReadWriteMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<short> values);

        public void ReadWriteIntMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<int> values);

        public void ReadWriteFloatMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<float> values);

        public void ReadWriteDoubleMultipleRegisters(ushort readStartAddress, ushort readQuantity, ushort writeStartAddress, List<double> values);

        #endregion

    }
}

