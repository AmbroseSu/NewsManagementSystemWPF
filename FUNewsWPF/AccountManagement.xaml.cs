using Service;
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

namespace FUNewsWPF
{
    /// <summary>
    /// Interaction logic for AccountManagement.xaml
    /// </summary>
    public partial class AccountManagement : Window
    {
        private readonly ISystemAccountService iSystemAccountService;
        public AccountManagement()
        {
            InitializeComponent();
            iSystemAccountService = new SystemAccountService();
        }

        public void LoadSystemAccountList()
        {
            try
            {
                var accountList = iSystemAccountService.GetAccounts();
                dgAccounts.ItemsSource = accountList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load list of Accounts");
            }
            finally
            {
                resetInput();
            }
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            LoadSystemAccountList();

        }

        private void resetInput()
        {
            txtAccountID.Text = "";
            txtAccountName.Text = "";
            txtAccountEmail.Text = "";
            txtAccountRole.Text = "";
            txtAccountPassword.Password = "";
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
