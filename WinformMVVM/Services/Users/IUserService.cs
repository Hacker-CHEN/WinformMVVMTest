using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformCore.Models.Users;

namespace WinformMVVM.Services.Users
{
    public interface IUserService
    {
        List<UserModel> GetData();

        void Add(UserModel userModel);
    }
}
