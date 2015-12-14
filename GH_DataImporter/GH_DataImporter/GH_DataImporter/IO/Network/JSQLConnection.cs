using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace JSS.IO.Network
{
    /// <summary>
    /// Provides a convenient interface for SQL data interactions.
    /// Each JSQLConnection object can only access a single database at a time.
    /// </summary>
    public class JSQLConnection
    {
        private string _userId;

        /// <summary>
        /// The actual SQL server connection.
        /// </summary>
        private MySqlConnection _connection;

        /// <summary>
        /// The current server address for the SQL connection.
        /// </summary>
        public string ServerIP
        {
            get { return this._connection.DataSource; }
        }

        /// <summary>
        /// Whether or not the connection is open and connected with a server - ready for commands.
        /// </summary>
        public bool ConnectionIsOpen
        {
            get
            {
                return this._connection.State == ConnectionState.Open;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverIP">The IP of the server to access.</param>
        /// <param name="port">The port number where the SQL server is installed.</param>
        /// <param name="userID">The username of the account you wish to use to access the SQL database.</param>
        /// <param name="password">The password of the account you wish to use to access the SQL database.</param>
        /// <param name="database">The name of the database to access on the SQL server.</param>
        public JSQLConnection(string serverIP, string port, string userID, string password, string database)
        {
            //Create connection string used for initiating the database connection
            string connectionString = 
                "SERVER=" + serverIP + "; " +
                "PORT=" + port + "; " +
                "UID=" + userID + "; " +
                "PASSWORD='" + password + "'; " +
                "DATABASE=" + database + ";";

            _userId = userID;
            
            //Create a MySqlConnection object to be opened later.
            _connection = new MySqlConnection(connectionString); //open connection to the desired database.
        }

        private void CheckIfValidStateForCommand()
        {
            if (this.ConnectionIsOpen == false)
            {
                //reopen connection
                _connection.Open();
            }
        }

        /// <summary>
        /// Opens the connection specified through the constructor and edited properties.
        /// </summary>
        public void InitiateConnection()
        {
            if (this.ConnectionIsOpen == true)
            {
                //Close the previous connection and open a new one.
                CloseConnection();
            }

            _connection.Open();
        }

        public void KillCurrentConnections()
        {
            CheckIfValidStateForCommand();

            var results = RunSQLCommandWithResults("SELECT CONCAT('KILL ', id) FROM information_schema.processlist WHERE user='b65a04a875d650'");
            List<string> commands = new List<string>();

            while (results.Read())
            {
                commands.Add((string)results[0]);
            }

            results.Close();

            //now call each kill command
            foreach (string command in commands)
            {
                RunSQLCommand(command);
            }
        }

        /// <summary>
        /// Closes the connection currently open.
        /// </summary>
        public void CloseConnection()
        {
            //Only close if the connection is currently open.
            if (this.ConnectionIsOpen == true)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Gets all of the table rows from the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to access.</param>
        public MySqlDataReader GetAllTableRows(string tableName)
        {
            //Execute the command and wait for the data
            return RunSQLCommandWithResults("SELECT * FROM " + tableName);
        }

        /// <summary>
        /// Runs a specified command. Doesn't return any results.
        /// </summary>
        /// <param name="commandString">The command to run in string format.</param>
        public void RunSQLCommand(string commandString)
        {
            //Run the command and close connection immediately.
            RunSQLCommandWithResults(commandString).Close();
        }

        /// <summary>
        /// Runs a specified command. Returns the results.
        /// </summary>
        /// <param name="commandString">The command to run in string format.</param>
        /// <returns></returns>
        public MySqlDataReader RunSQLCommandWithResults(string commandString)
        {
            //Validate connection state
            CheckIfValidStateForCommand();

            //Create the command
            MySqlCommand command = new MySqlCommand(commandString);
            command.Connection = _connection;

            //Run the command and return the results
            return command.ExecuteReader();
        }
    }
}
