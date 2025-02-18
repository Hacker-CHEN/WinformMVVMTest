using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformCore.Client;
using WinformCore.Models.Users;
using WinformMVVM.DAL;

namespace WinformMVVM.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ISqlSugarClientFactory _clientFactory;

        public UserService(ISqlSugarClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<UserModel> GetData()
        {
            var db = _clientFactory.CreateClient();
            return db.Queryable<UserModel>().ToList();
        }

        public void Add(UserModel userModel)
        {
            throw new NotImplementedException();
        }
    }
}
