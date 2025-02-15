using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformMVVM.Models.Users;

namespace WinformMVVM.Services.Users
{
    public interface IUserService
    {
        UserModel GetUserByUsername(string userName);
    }
}
