using System;
using System.Collections.Generic;
using ModbusNet;

namespace ModbusNetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpModbusMaster tcpModbusMaster = new TcpModbusMaster("10.211.55.3", 502);
            tcpModbusMaster.Start();


            //     var dd = tcpModbusMaster.ReadCoils(0, 5);//

            //tcpModbusMaster.WriteSingleCoil(0, true);
            //tcpModbusMaster.WriteSingleCoil(1, true);
            //tcpModbusMaster.WriteSingleCoil(2, true);
            //tcpModbusMaster.WriteSingleCoil(3, true);
            //tcpModbusMaster.WriteSingleCoil(4, true);
            //tcpModbusMaster.WriteSingleCoil(5, true);

            //tcpModbusMaster.ReadCoils(0, 10);

            //tcpModbusMaster.ReadInt16HoldingRegisters(0, 10);
            //tcpModbusMaster.ReadInt32HoldingRegisters(11, 10);

            //tcpModbusMaster.WriteSingleRegister(0, 100);
            //tcpModbusMaster.WriteSingleRegister(1, 200);
            //tcpModbusMaster.WriteSingleRegister(2, 300);
            //tcpModbusMaster.WriteSingleRegister(3, 400);
            //tcpModbusMaster.WriteSingleRegister(4, 500);
            //tcpModbusMaster.WriteMultipleCoils(0, new System.Collections.Generic.List<bool>() { true, true, true, false, false, false, true, true });
            tcpModbusMaster.WriteFloatMultipleRegisters(0, new List<float>() { 1.125f, 0.524f, 1.111f });
            //tcpModbusMaster.WriteMultipleRegisters(0, new List<short>() { 0,1, 2, 3,4,5,6,7,8,9,10,11,12,13,14,15,16 });

            Console.WriteLine("dd");
        }
    }
}

