using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WinformMVVM.Commands;
using WinformMVVM.Services.Users;

namespace WinformMVVM.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private UserService _userService;
        public ICommand LoginCommand { get; }


        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }


        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }


        public LoginViewModel(UserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin);

        }

        private void ExecuteLogin()
        {
            bool isValid = _userService.ValidateUser(Username, Password);
            if (isValid)
            {
                MessageBox.Show("Login successful!");
                return;
            }
            MessageBox.Show("Login failed. Invalid username or password.");
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
