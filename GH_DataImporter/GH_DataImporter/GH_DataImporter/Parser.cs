using System;
using System.Collections.Generic;
using System.IO;

namespace GH_DataImporter
{
    public class Parser
    {
        public Tuple<List<Transaction>, List<Employee>> ParseFolder(string foldername)
        {
            string[] files = Directory.GetFiles(foldername);

            List<Transaction> transactions = new List<Transaction>();
            List<Employee> employees = new List<Employee>();

            foreach (string filename in files)
            {
                if (filename.EndsWith(".csv"))
                {
                    var tuple = ParseFile(filename);
                    MergeTransactions(transactions, tuple.Item1);
                    MergeEmployees(employees, tuple.Item2);
                }
            }

            return new Tuple<List<Transaction>, List<Employee>>(transactions, employees);
        }

        public void Merge<T>(List<T> main, IEnumerable<T> toMerge, Func<T, T, bool> isEqual)
        {
            foreach (T itemToMerge in toMerge)
            {
                bool itemInMain = false;

                foreach (T itemInMainToCompare in main)
                {
                    if (isEqual(itemToMerge, itemInMainToCompare))
                    {
                        itemInMain = true;
                        break;
                    }
                }

                if (itemInMain == false)
                {
                    //add the item to the list
                    main.Add(itemToMerge);
                }
            }
        }

        public void MergeTransactions(List<Transaction> main, List<Transaction> toAdd)
        {
            Merge<Transaction>(main, toAdd, (t1, t2) => { return t1.ID == t2.ID; });
        }

        public void MergeEmployees(List<Employee> main, List<Employee> toAdd)
        {
            Merge<Employee>(main, toAdd, (e1, e2) => { return e1.FirstName == e2.FirstName && e1.LastName == e2.LastName; });
        }

        public void AddTransaction(List<Transaction> main, Transaction toAdd)
        {
            MergeTransactions(main, new List<Transaction>() { toAdd });
        }

        public void AddEmployee(List<Employee> main, Employee employee)
        {
            MergeEmployees(main, new List<Employee>() { employee });
        }

        public Tuple<List<Transaction>,List<Employee>> ParseFile(string filename)
        {
            /*
                0 - Transaction - ID
                1 - Time
                2 - Register
                3 - Cashier
                4 - Operation Type
                5 - Customer Name
                6 - Customer Email
                7 - Subtotal
                8 - New Liabilities
                9 - Discount
                10 - Tax
                11 - Total
                12 - Gratuity
                13 - Receipt Number
            */

            List<Employee> employeeList = new List<Employee>();
            List<Transaction> transactionList = new List<Transaction>();

            using (FileStream filestream = File.Open(filename, FileMode.Open))
            using (StreamReader reader = new StreamReader(filestream))
            {
                //Skip first line...
                reader.ReadLine();

                while (reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();

                    if (line.Trim() == "")
                    {
                        continue;
                    }

                    Employee employee = new Employee();
                    Transaction transaction = new Transaction();

                    var split = line.Split(",".ToCharArray());

                    //setup name of the cashier (special case)
                    string fname = split[3].Split(" ".ToCharArray())[0];
                    string lname = split[3].Split(" ".ToCharArray())[1];

                    //setup transaction
                    transaction.ID = split[0];
                    transaction.Time = ParseTime(split[1]);
                    transaction.Date = ParseDate(split[1]);
                    transaction.CashierFirstName = fname;
                    transaction.CashierLastName = lname;
                    transaction.SubTotal = split[7];
                    transaction.Discount = split[9];
                    transaction.Tax = split[10];
                    transaction.Total = split[11];
                    transaction.TipAmount = split[12];

                    //Setup employee
                    employee.FirstName = fname;
                    employee.LastName = lname;

                    //Add if unique
                    AddEmployee(employeeList, employee);
                    AddTransaction(transactionList, transaction);
                }
            }

            return new Tuple<List<Transaction>, List<Employee>> (transactionList, employeeList);
        }

        private string ParseTime(string dateTimeString)
        {
            return dateTimeString.Split(" ".ToCharArray())[1];
        }

        private string ParseDate(string dateTimeString)
        {
            return dateTimeString.Split(" ".ToCharArray())[0];
        }
    }
}
