using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class MySQLFactory : IDBFactory
    {
        public IDbConnection CreateConnection() => (IDbConnection)new MySqlConnection();

        public IDbConnection CreateConnection(string connString)
        {
            return (IDbConnection)new MySqlConnection(connString);
        }

        public IDbCommand CreateCommand() => (IDbCommand)new MySqlCommand();

        public IDbDataAdapter CreateDataAdapter() => (IDbDataAdapter)new MySqlDataAdapter();

        public IDbTransaction CreateTransaction(IDbConnection myDbConnection)
        {
            return myDbConnection.BeginTransaction();
        }

        public IDataReader CreateDataReader(IDbCommand myDbCommand) => myDbCommand.ExecuteReader();

        public IDbDataParameter GetReturnValuePara()
        {
            return (IDbDataParameter)new MySqlParameter("ReturnValue", MySqlDbType.Int16, 4, ParameterDirection.ReturnValue, false, (byte)0, (byte)0, string.Empty, DataRowVersion.Default, (object)null);
        }
    }
}
