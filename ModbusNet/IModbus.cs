using System.Collections.Generic;

namespace ModbusNet
{
    public interface IModbusMaster
    {

        #region 读取多个线圈量，离散量输入

        public List<bool> ReadCoils(int start, ushort quantity);

        public List<bool> ReadDiscreteInputs(int start, ushort quantity);

        #endregion

        #region 读取多个保持寄存器

        public List<short> ReadInt16HoldingRegisters(int start, ushort quantity);

        public List<int> ReadInt32HoldingRegisters(int start, ushort quantity);

        public List<float> ReadFloatHoldingRegisters(int start, ushort quantity);

        public List<double> ReadDoubleHoldingRegisters(int start, ushort quantity);

        #endregion


        #region 读取多个输入寄存器

        public List<short> ReadInputRegisters(int start, ushort quantity);

        public List<int> ReadIntInputRegisters(int start, ushort quantity);

        public List<float> ReadFloatInputRegisters(int start, ushort quantity);

        public List<double> ReadDoubleInputRegisters(int start, ushort quantity);

        #endregion

        #region 写入单个值

        public void WriteSingleCoil(int start, bool status);

        public void WriteSingleRegister(int start, short value);

        public void WriteIntSingleRegister(int start, int value);

        public void WriteFloatSingleRegister(int start, float value);

        public void WriteDoubleSingleRegister(int start, double value);

        #endregion

        #region 写入多个值

        public void WriteMultipleCoils(int start, List<bool> values);

        public void WriteMultipleRegisters(int start, List<short> values);

        public void WriteIntMultipleRegisters(int start, List<int> values);

        public void WriteFloatMultipleRegisters(int start, List<float> values);

        public void WriteDoubleMultipleRegisters(int start, List<double> values);

        #endregion

    }
}

