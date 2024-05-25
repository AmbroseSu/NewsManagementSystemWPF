using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using Service;
using BusinessObject;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FUNewsWPF
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly ISystemAccountService iSystemAccountService;
        public SystemAccount LoggedInAccount { get; private set; }
        private string defaultEmail;
        private string defaultPassword;
        public LoginRole Role { get; private set; }
        public Login()
        {
            InitializeComponent();
            iSystemAccountService = new SystemAccountService();
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 

            IConfiguration configuration = builder.Build();

            defaultEmail = configuration.GetSection("DefaultAccount")["Email"];
            defaultPassword = configuration.GetSection("DefaultAccount")["Password"];

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(txtUser.Text.Equals(defaultEmail) && txtPass.Password.Equals(defaultPassword))
            {
                //AccountManagement accountManagement = new AccountManagement();
                //accountManagement.Show();
                Role = LoginRole.Admin;
                this.DialogResult = false;
                this.Close();
            }
            else
            {
                SystemAccount systemAccount = iSystemAccountService.GetAccountByEmail(txtUser.Text);

                if (systemAccount != null && systemAccount.AccountPassword.Equals(txtPass.Password) && systemAccount.AccountRole == 1)
                {
                    /*                this.Hide();
                                    NewsArticleUI newsArticleUI = new NewsArticleUI(systemAccount);
                                    newsArticleUI.Show();*/
                    /*CategoryUI categoryUI = new CategoryUI();
                    categoryUI.Show();*/
                    LoggedInAccount = systemAccount;
                    Role = LoginRole.Staff;
                    this.DialogResult = true; 
                    this.Close();

                }
                else
                {
                    if (systemAccount != null && systemAccount.AccountPassword.Equals(txtPass.Password) && systemAccount.AccountRole == 2)
                    {
                        LoggedInAccount = systemAccount;
                        Role = LoginRole.Lecturer;
                        this.DialogResult = true; 
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("You are not permission !");
                    }
                        
                }
            }


            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
