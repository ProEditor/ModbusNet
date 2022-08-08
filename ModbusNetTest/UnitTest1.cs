namespace ModbusNetTest;

public class Tests
{

    [Test]
    public void Test2()
    {
        var transactionBytes = BitConverter.GetBytes((short)6554);
        Console.WriteLine(transactionBytes);
    }


    [Test]
    public void Test1()
    {
        int j = 0;
        for (int i = 0; i < 100; i++)
        {

            Interlocked.Increment(ref j);

            Console.WriteLine(j);

        }
    }
}
