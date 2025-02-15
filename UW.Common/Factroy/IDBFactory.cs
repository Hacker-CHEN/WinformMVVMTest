using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public interface IDBFactory
    {
        IDbConnection CreateConnection();

        IDbConnection CreateConnection(string connString);

        IDbCommand CreateCommand();

        IDbDataAdapter CreateDataAdapter();

        IDbTransaction CreateTransaction(IDbConnection myDbConnection);

        IDataReader CreateDataReader(IDbCommand myDbCommand);

        IDbDataParameter GetReturnValuePara();
    }
}
