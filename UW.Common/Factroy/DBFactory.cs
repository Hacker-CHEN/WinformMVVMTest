using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public sealed class DBFactory
    {
        private DataBaseType _databaseType;

        public DBFactory(DataBaseType databaseType) => this._databaseType = databaseType;

        public IDBFactory CreateInstance()
        {
            IDBFactory instance;
            if (_databaseType == DataBaseType.mysql)
            {
                instance = (IDBFactory)new MySQLFactory();
                return instance;
            }
            return null;
        }

        public IPagingHelper CreatePagingInstance()
        {
            IPagingHelper pagingInstance;
            if (_databaseType == DataBaseType.mysql)
            {
                pagingInstance = null;
                return pagingInstance;
            }
            return null;
        }

        public IDbDataParameter CreateDBParameter(string paraName, DataType dataType, int size)
        {
            IDbDataParameter dbParameter;
            if (_databaseType == DataBaseType.mysql)
            {
                dbParameter = (IDbDataParameter)this.GetMysqlParam(paraName, dataType, size);
                return dbParameter;
            }
            return null;
        }

        private SqlParameter GetSqlParam(string paraName, DataType dataType, int size)
        {
            switch (dataType)
            {
                case DataType.Decimal:
                    return new SqlParameter(paraName, SqlDbType.Decimal, size);
                case DataType.Varchar:
                    return new SqlParameter(paraName, SqlDbType.VarChar, size);
                case DataType.DateTime:
                    return new SqlParameter(paraName, SqlDbType.DateTime);
                case DataType.Image:
                    return new SqlParameter(paraName, SqlDbType.Image);
                case DataType.Int:
                    return new SqlParameter(paraName, SqlDbType.Int);
                case DataType.Text:
                    return new SqlParameter(paraName, SqlDbType.NText);
                case DataType.Float:
                    return new SqlParameter(paraName, SqlDbType.Float);
                default:
                    return new SqlParameter(paraName, SqlDbType.VarChar);
            }
        }

        private MySqlParameter GetMysqlParam(string paraName, DataType dataType, int size)
        {
            switch (dataType)
            {
                case DataType.Decimal:
                    return new MySqlParameter(paraName, MySqlDbType.Decimal, size);
                case DataType.Varchar:
                    return new MySqlParameter(paraName, MySqlDbType.String, size);
                case DataType.DateTime:
                    return new MySqlParameter(paraName, MySqlDbType.DateTime);
                case DataType.Image:
                    return new MySqlParameter(paraName, MySqlDbType.Binary);
                case DataType.Int:
                    return new MySqlParameter(paraName, MySqlDbType.Int32, size);
                case DataType.Text:
                    return new MySqlParameter(paraName, MySqlDbType.String);
                case DataType.Float:
                    return new MySqlParameter(paraName, MySqlDbType.Float);
                default:
                    return new MySqlParameter(paraName, MySqlDbType.String);
            }
        }
    }
}
