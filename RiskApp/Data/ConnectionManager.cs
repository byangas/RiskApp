using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Data
{
    public class ConnectionManager
    {

        // for local development
        private string connectionString = "";
        private NpgsqlConnection npgsqlConnection;
        public ConnectionManager(IConfiguration environment)
        {
            connectionString = environment.GetValue<string>("RISKMINUTE_DB", null);
        }
        public NpgsqlTransaction Transaction { get; protected set; }


        public NpgsqlTransaction BeginTransaction()
        {
            if (Transaction != null)
            {
                return Transaction;
            }

            NpgsqlTransaction transaction = GetConnection().BeginTransaction();
            Transaction = transaction;
            return transaction;
        }


        private NpgsqlConnection GetConnection()
        {
            if (Transaction != null)
            {
                if (Transaction.Connection.State == System.Data.ConnectionState.Closed)
                {
                    Transaction.Connection.Open();
                }
                return Transaction.Connection;
            }
            if (npgsqlConnection != null)
            {
                if (npgsqlConnection.State == System.Data.ConnectionState.Closed)
                    npgsqlConnection.Open();

                return npgsqlConnection;
            }

            npgsqlConnection = new NpgsqlConnection(connectionString);
            npgsqlConnection.Open();
            return npgsqlConnection;
        }

        public void CloseConnection()
        {
            if (Transaction == null)
            {
                npgsqlConnection.Close();
            }
        }

        public void CommitTransaction()
        {
            Transaction.Commit();
            //so that the Transaction doesn't get re-used after commit. The next "GetConnection" will get a new connection
            if (Transaction.Connection != null && Transaction.Connection.State != System.Data.ConnectionState.Closed)
            {
                Transaction.Connection.Close();
            }
            Transaction = null;
        }

        public NpgsqlCommand GetCommand(string SQL)
        {

            NpgsqlCommand npgsqlCommand = new NpgsqlCommand(SQL, GetConnection());
            if (Transaction != null)
            {
                npgsqlCommand.Transaction = Transaction;
            }
            return npgsqlCommand;
        }

        protected void Dispose()
        {
            if (Transaction != null)
            {
                if (Transaction.Connection != null && Transaction.Connection.State != System.Data.ConnectionState.Closed)
                {
                    Transaction.Connection.Close();
                }

                Transaction.Dispose();
                Transaction = null;
            }
            else if (npgsqlConnection != null)
            {
                if (npgsqlConnection.State != System.Data.ConnectionState.Closed)
                {
                    try
                    {
                        npgsqlConnection.Close();
                        npgsqlConnection.Dispose();
                    }
                    catch
                    {

                    }
                }
                npgsqlConnection = null;
            }
        }

        public void Rollback()
        {
            if (Transaction != null && !Transaction.IsCompleted)
            {
                Transaction.Rollback();
                try
                {
                    if (Transaction.Connection != null && Transaction.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Transaction.Connection.Close();
                    }
                }
                catch (Exception)
                {
                }


                Transaction = null;
            }

        }
    }
}
