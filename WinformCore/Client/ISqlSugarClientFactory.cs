using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformCore.Client
{
    public interface ISqlSugarClientFactory
    {
        SqlSugarClient CreateClient();
    }
}
