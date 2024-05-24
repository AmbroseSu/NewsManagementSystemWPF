using BusinessObject;
using Repository;
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
    /// Interaction logic for ProfileUI.xaml
    /// </summary>
    public partial class ProfileUI : Window
    {

        private readonly ISystemAccountService iSystemAccountService;
        private SystemAccount accountToUpdate = null;
        public ProfileUI()
        {
            InitializeComponent();
            iSystemAccountService = new SystemAccountService();
            Loaded += Window_Loaded;
        }

        public ProfileUI(SystemAccount account)
        {
            InitializeComponent();
            iSystemAccountService = new SystemAccountService();
            Loaded += Window_Loaded;
            accountToUpdate = account;
            txtAccountId.Text = accountToUpdate.AccountId.ToString();
            txtAccountName.Text = accountToUpdate.AccountName;
            txtAccountEmail.Text = accountToUpdate.AccountEmail;
            txtAccountPassword.Text = accountToUpdate.AccountPassword;
            txtAccountRole.Text = accountToUpdate.AccountRole.ToString();
        }



        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            txtAccountId.IsEnabled = false;
        }

        private void resetInput()
        {
            txtAccountId.IsEnabled = false;
            txtAccountName.Text = "";
            txtAccountEmail.Text = "";
            txtAccountPassword.Text = "";
            txtAccountRole.Text = "";
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAccountId.Text.Length > 0)
                {
                    SystemAccount account = new SystemAccount();
                    account.AccountId = short.Parse(txtAccountId.Text);
                    account.AccountName = txtAccountName.Text;
                    account.AccountEmail = txtAccountEmail.Text;
                    account.AccountPassword = txtAccountPassword.Text;
                    account.AccountRole = int.Parse(txtAccountRole.Text);
                    iSystemAccountService.UpdateAccount(account);
                    MessageBox.Show("Account Update Successfully.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            resetInput();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
