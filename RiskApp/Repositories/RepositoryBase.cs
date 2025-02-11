using Npgsql;
using RiskApp.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace RiskApp.Repositories
{
    public class RepositoryBase
    {
        protected readonly ConnectionManager connectionManager;

        public RepositoryBase(ConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
        }

        protected object DBSafeNull(string val)
        {
            object dbValue = val;
            if (string.IsNullOrWhiteSpace(val))
            {
                dbValue = DBNull.Value;
            }

            return dbValue;
        }

        protected object DBSafeNull(object val)
        {
            object dbValue = val;
            if (val == null)
            {
                dbValue = DBNull.Value;
            }

            return dbValue;
        }

        protected T GetValue<T>(NpgsqlDataReader reader, string fieldName)
        {
            object value = reader[fieldName];
            if (value == DBNull.Value)
            {
                return default;
            }
            return (T)value;
        }


        protected int ExecuteNonQuery(string SQL, string paramName, object parameter)
        {
            using NpgsqlCommand command = connectionManager.GetCommand(SQL);
            command.Parameters.AddWithValue(paramName, parameter);
            command.Prepare();

            int affectedRecords = command.ExecuteNonQuery();
            return affectedRecords;
        }

        protected int ExecuteNonQuery(string SQL, dynamic queryparams = null)
        {
            using NpgsqlCommand command = connectionManager.GetCommand(SQL);

            if (queryparams != null)
            {
                foreach (var key in queryparams.Keys)
                {
                    command.Parameters.AddWithValue(key, queryparams[key]);
                }

            }
            command.Prepare();
            var results = command.ExecuteNonQuery();
            connectionManager.CloseConnection();
            return results;
        }

        protected T ExecuteScalar<T>(string SQL, dynamic queryparams = null)
        {
            using NpgsqlCommand command = connectionManager.GetCommand(SQL);

            if (queryparams != null)
            {
                foreach (var key in queryparams.Keys)
                {
                    command.Parameters.AddWithValue(key, queryparams[key]);
                }

            }
            command.Prepare();
            object temp = command.ExecuteScalar();
            var result = (T)temp;
            connectionManager.CloseConnection();
            return result;
        }



        protected NpgsqlDataReader ExecuteReader(string SQL, IDictionary<string, object> queryparams = null)
        {
            using var command = connectionManager.GetCommand(SQL);
            if (queryparams != null)
            {
                foreach (var key in queryparams.Keys)
                {
                    command.Parameters.AddWithValue(key, queryparams[key]);
                }
            }

            return command.ExecuteReader(GetBehavior());
        }

        private  CommandBehavior GetBehavior()
        {
            //if we are in a transaction, don't close the connection immediately. The connection will get closed 
            // when the transaction commits or rolls back.
            if(connectionManager.Transaction != null)
            {
                return CommandBehavior.SingleResult;
            }
            // if there is not transaction, then close the connection after executing Reader
            return CommandBehavior.SingleResult | CommandBehavior.CloseConnection;
        }

        protected NpgsqlDataReader ExecuteReader(string SQL, string parameterName, object parameter)
        {
            using var command = connectionManager.GetCommand(SQL);
            command.Parameters.AddWithValue(parameterName, parameter);
            return command.ExecuteReader(GetBehavior());
        }
    }
}
