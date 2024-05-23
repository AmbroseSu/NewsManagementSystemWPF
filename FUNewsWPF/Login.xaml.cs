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
using Service;
using BusinessObject;

namespace FUNewsWPF
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly ISystemAccountService iSystemAccountService;
        public SystemAccount LoggedInAccount { get; private set; }
        public Login()
        {
            InitializeComponent();
            iSystemAccountService = new SystemAccountService();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            SystemAccount systemAccount = iSystemAccountService.GetAccountById(txtUser.Text);
            
            if (systemAccount != null && systemAccount.AccountPassword.Equals(txtPass.Password) && systemAccount.AccountRole == 1)
            {
/*                this.Hide();
                NewsArticleUI newsArticleUI = new NewsArticleUI(systemAccount);
                newsArticleUI.Show();*/
                /*CategoryUI categoryUI = new CategoryUI();
                categoryUI.Show();*/
                LoggedInAccount = systemAccount;
                this.DialogResult = true; // Đóng cửa sổ và trả về kết quả thành công
                this.Close();

            }
            else
            {
                MessageBox.Show("You are not permission !");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
