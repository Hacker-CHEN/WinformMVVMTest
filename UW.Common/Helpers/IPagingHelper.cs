using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public interface IPagingHelper
    {
        string CreatePagingSql(int recordCount,int pageSize,int pageIndex,string safeSql,string orderField);

        string CreateCountingSql(string safeSql);
    }
}
