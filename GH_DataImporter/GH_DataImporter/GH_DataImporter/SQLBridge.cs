using System;
using System.Collections.Generic;
using System.Text;

namespace GH_DataImporter
{
    public class SQLBridge : JSS.IO.Network.JSQLConnection
    {
        private int _nextEmployeeID = 0;
        private List<string> _transactionIDList = new List<string>();
        private List<string> _employeeNamesList = new List<string>();

        public SQLBridge()
            :base("db_ip", "3306", "db_password", "db_user", "initial_db")
        {
            InitiateConnection();
            //KillCurrentConnections(); //cleanup....
            DownloadNeededInformation();
        }

        private void DownloadNeededInformation()
        {
            //download next employee id 
            var result = RunSQLCommandWithResults("SELECT emp_id FROM gh_employee ORDER BY (emp_id) DESC LIMIT 1");
            result.Read();
            this._nextEmployeeID = (int)result["emp_id"] + 1;
            result.Close();

            //download current list of employee names
            result = RunSQLCommandWithResults("SELECT fname, lname FROM gh_employee");

            while (result.Read())
            {
                _employeeNamesList.Add(result["fname"].ToString() + " " + result["lname"].ToString());
            }

            result.Close();

            //download current list of transaction IDs
            result = RunSQLCommandWithResults("SELECT id FROM gh_transaction");

            while (result.Read())
            {
                _transactionIDList.Add(result["id"].ToString());
            }

            result.Close();
        }

        public void EndSQLConnection()
        {
            //logout of mySQL
            this.CloseConnection();
        }

        private int GetEmployeeID(string fname, string lname)
        {
            string cmd = "SELECT emp_id FROM gh_employee WHERE fname = '" + fname + "' AND lname = '" + lname + "'";
            var result = RunSQLCommandWithResults("SELECT emp_id FROM gh_employee WHERE fname = '" + fname + "' AND lname = '" + lname +"'");
            result.Read();
            int id = (int)result["emp_id"];

            result.Close();

            return id;
        }

        private void AddEmployeeToDatabaseIfUnique(Employee employee)
        {
            if (!_employeeNamesList.Exists((employeeName) => { return employee.FirstName + " " + employee.LastName == employeeName; }))
            {
                //Employee doesn't currently exist in the database so add it
                employee.EmpID = _nextEmployeeID.ToString();
                _nextEmployeeID++;

                StringBuilder sBuilder = new StringBuilder();
                sBuilder.AppendFormat("INSERT INTO gh_employee VALUES ('{0}', '{1}', '{2}')",
                                    employee.FirstName, employee.LastName, _nextEmployeeID);

                RunSQLCommand(sBuilder.ToString());

            }

            //Employee already exists in the database so don't add it
        }

        private void AddTransactionToDatabaseIfUnique(Transaction transaction)
        {
            if (!_transactionIDList.Exists((t) => { return t == transaction.ID; }))
            {
                //Transaction doesn't currently exist in the database so add it
                int empIDForTransaction = GetEmployeeID(transaction.CashierFirstName, transaction.CashierLastName);

                ScrambleTransaction(transaction);

                //INSERT INTO gh_transaction VALUES ('1', '052900', '2015-11-17', '15.27', '3.26', '1', '1.53', '19.06', '1');
                StringBuilder sBuilder = new StringBuilder();
                sBuilder.AppendFormat("INSERT INTO gh_transaction VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                                        transaction.ID,
                                        transaction.Time,
                                        transaction.Date,
                                        transaction.SubTotal,
                                        transaction.TipAmount,
                                        transaction.Discount,
                                        transaction.Tax,
                                        transaction.Total,
                                        empIDForTransaction);
                
                RunSQLCommand(sBuilder.ToString());
            }

            //Transaction already exists in the database so don't add it
        }

        private void ScrambleTransaction(Transaction transaction)
        {
            Random random = new Random();
            float r = (float)random.NextDouble() / 4f + .25f; //ranges from .25 to .5

            float[] vals = new float[3] { float.Parse(transaction.SubTotal),
                                          float.Parse(transaction.TipAmount),
                                          float.Parse(transaction.Discount)};

            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] *= r;
                vals[i] = (float)Math.Round(vals[i], 2);
            }

            transaction.SubTotal = vals[0].ToString();
            transaction.TipAmount = vals[1].ToString();
            transaction.Discount = vals[2].ToString();
            float tax = (float)Math.Round(vals[0] * .08f, 2);
            transaction.Tax = tax.ToString();
            transaction.Total = (vals[0] + vals[1] - vals[2] + tax).ToString();
        }

        public void ProcessEmployeeImports(List<Employee> employees)
        {
            foreach (Employee employee in employees)
            {
                AddEmployeeToDatabaseIfUnique(employee);
            }
        }

        public void ProcessTransactionImports(List<Transaction> transactions)
        {
            int total = transactions.Count;
            int current = 1;

            foreach (Transaction transaction in transactions)
            {
                Console.Clear();
                Console.WriteLine("Uploading transaction " + current.ToString() + " of " + total.ToString() + "...");
                AddTransactionToDatabaseIfUnique(transaction);

                current++;
            }
        }
    }
}
