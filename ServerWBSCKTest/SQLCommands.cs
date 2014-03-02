//using ServerWBSCKTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data;



namespace ServerWBSCKTest
{
    class SQLCommands
    {
        IDbConnection connection;

        public void connect()
        {

            /*using (var db = new gsiContext())
            {
                db.Users.Add(new User { First_Name = "Klara", Last_Name = "Klok", Telefon_Nr = "99999" });
                db.SaveChanges();
                

                var quer = from b in db.Users
                           orderby b.userId
                           select b;

                foreach(var item in quer)
                {
                    Console.WriteLine(item.First_Name + " " + item.Last_Name + " " + item.Telefon_Nr);
                }
            }
            */
        }



        public static string GetServerVersion(IDbConnection connection)
        {
            // Ensure that the connection is opened (otherwise executing the command will fail)
            ConnectionState originalState = connection.State;
            if (originalState != ConnectionState.Open)
                connection.Open();
            try
            {
                // Create a command to get the server version 
                // NOTE: The query's syntax is SQL Server specific
                IDbCommand command = connection.CreateCommand();
                command.CommandText = "SELECT @@version";
                return (string)command.ExecuteScalar();
            }
            finally
            {
                // Close the connection if that's how we got it 
                if (originalState == ConnectionState.Closed)
                    connection.Close();
            }
        }
    }
}
