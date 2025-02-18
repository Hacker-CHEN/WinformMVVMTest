using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformMVVM.Services.Users;
using WinformMVVM.ViewModels;

namespace WinformMVVM.Views
{
    public partial class LoginView : Form
    {
        private readonly LoginViewModel _loginViewModel;

        public LoginView()
        {
            InitializeComponent();
            UserService userService = new UserService();
            _loginViewModel = new LoginViewModel(userService);

            txtUserName.DataBindings.Add("Text", _loginViewModel, "UserName", false, DataSourceUpdateMode.OnPropertyChanged);
            txtPassword.DataBindings.Add("Text", _loginViewModel, "Password", false, DataSourceUpdateMode.OnPropertyChanged);


        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (_loginViewModel.Username == "" || _loginViewModel.Password == "")
                {
                    MessageBox.Show("Please enter your username and password");
                    return;
                }

                if (_loginViewModel.LoginCommand.CanExecute(null))
                {
                    _loginViewModel.LoginCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
