using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformCore.Client;

namespace WinformCore.Models.Users
{
    [SugarTable("user")]
    public class UserModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int UserId { get; set; }//主键；自增
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
