using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformMVVM.DAL;
using WinformMVVM.Models.Users;

namespace WinformMVVM.Services.Users
{
    public class UserService : IUserService
    {
        public UserModel GetUserByUsername(string userName)
        {
            return new UserModel();
        }

        public bool ValidateUser(string username,string password)
        {
            UserModel userModel = GetUserByUsername(username);
            if (userModel != null&& userModel.Password==password)
            {
                return true;
            }
            return false;
        }
    }
}
