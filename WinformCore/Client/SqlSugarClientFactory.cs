using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformCore.Client
{
    public class SqlSugarClientFactory : ISqlSugarClientFactory
    {
        private readonly string _connectionString;
        private readonly DbType _dbType;

        public SqlSugarClientFactory(string connectionString, DbType dbType)
        {
            _connectionString = connectionString;
            _dbType = dbType;
        }

        public SqlSugarClient CreateClient()
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = true
            });
        }
    }
}
