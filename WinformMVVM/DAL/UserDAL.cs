using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UW.Common;
using WinformMVVM.Models.Users;

namespace WinformMVVM.DAL
{
    public class UserDAL
    {
        public static List<UserModel> Query_AllUser()
        {
            string sqlStr = "SELECT * FROM sys_user;";
            return DatabaseHelper.Instance.Query(sqlStr).Tables[0].ToList<UserModel>();
        }

        public static UserModel Query_UserByUserName(string userName)
        {
            string sqlStr = $"SELECT * FROM sys_user WHERE UserName = '{userName}';";
            List<UserModel> list = DatabaseHelper.Instance.Query(sqlStr).Tables[0].ToList<UserModel>();
            return list.Count > 0 ? list[0] : (UserModel)null;
        }
    }
}
