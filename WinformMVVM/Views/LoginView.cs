using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformCore.Client;
using WinformCore.Models.Users;
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
            string connectionString = ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;
            SqlSugarClientFactory clientFactory = new SqlSugarClientFactory(connectionString,SqlSugar.DbType.MySql);

            UserService userService = new UserService(clientFactory);
            _loginViewModel = new LoginViewModel(userService);
            _loginViewModel.PropertyChanged += LoginViewModel_PropertyChanged;
            //txtUserName.DataBindings.Add("Text", _loginViewModel, "UserName", false, DataSourceUpdateMode.OnPropertyChanged);
            //txtPassword.DataBindings.Add("Text", _loginViewModel, "Password", false, DataSourceUpdateMode.OnPropertyChanged);

        }

        private void LoginViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginViewModel.UserData))
            {
                dgvData.DataSource = _loginViewModel.UserData;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {

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
