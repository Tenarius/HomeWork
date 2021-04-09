using System;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Test_№_1
            /*Console.Write("Print address: ");
            string address = Console.ReadLine();
            Console.WriteLine();
            _ = new Test1(address);
            Console.Read();*/
            #endregion

            #region Test_№_2
            Console.Write("Enter name city: ");
            string city = Console.ReadLine();
            Console.WriteLine();
            Test2 test2 = new Test2(city);
            test2.GetWeather();
            Console.Read();
            #endregion

            #region Test_№_3

            #endregion
        }
    }
}
