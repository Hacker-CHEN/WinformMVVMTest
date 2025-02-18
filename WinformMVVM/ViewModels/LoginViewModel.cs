using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WinformCore.Models.Users;
using WinformMVVM.Commands;
using WinformMVVM.Services.Users;

namespace WinformMVVM.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private UserService _userService;
        private List<UserModel> _userData;

        public ICommand LoginCommand { get; }

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(LoadData);

        }

        public List<UserModel> UserData
        {
            get => _userData;
            set
            {
                _userData = value;
                OnPropertyChanged(nameof(UserData));
            }
        }

        private void LoadData()
        {
            UserData = _userService.GetData();
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
