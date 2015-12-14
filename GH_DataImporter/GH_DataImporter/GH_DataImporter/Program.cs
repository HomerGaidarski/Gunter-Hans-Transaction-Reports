using System;
using System.Collections.Generic;

namespace GH_DataImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser p = new Parser();

            string directory = "C:\\Users\\Jared\\Desktop\\To Upload\\"; //Directory.GetCurrentDirectory();

            var t = p.ParseFolder(directory);
            List<Transaction> transactions = t.Item1;
            List<Employee> employees = t.Item2;

            SQLBridge bridge = new SQLBridge();
            bridge.InitiateConnection();
            bridge.ProcessEmployeeImports(employees);
            bridge.ProcessTransactionImports(transactions);
            bridge.CloseConnection();

            Console.ReadLine();
            return;
        }
    }
}
