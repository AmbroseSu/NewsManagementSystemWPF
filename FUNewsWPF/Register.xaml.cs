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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private readonly ISystemAccountService iSystemAccountService;
        private SystemAccount accountToUpdate = null;
        //private bool btnState = false;
        public Register()
        {
            InitializeComponent();
            iSystemAccountService = new SystemAccountService();
            Loaded += Window_Loaded;
        }

        public Register(SystemAccount account)
        {
            InitializeComponent();
            iSystemAccountService = new SystemAccountService();
            Loaded += Window_Loaded;
            accountToUpdate = account;
            txtAccountID.Text = accountToUpdate.AccountId.ToString();
            txtAccountName.Text = accountToUpdate.AccountName;
            txtAccountEmail.Text = accountToUpdate.AccountEmail;
            txtAccountPassword.Text = accountToUpdate.AccountPassword;
            txtAccountRole.Text = accountToUpdate.AccountRole.ToString();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAccountID.Text))
            {
                btnRegister.IsEnabled = false;
                txtAccountID.IsEnabled = false;
            }
            else
            {
                btnUpdate.IsEnabled = false;
            }


        }

        

        private void resetInput()
        {
            if(!string.IsNullOrEmpty(txtAccountID.Text) && btnUpdate.IsEnabled == true)
            {
                txtAccountID.IsEnabled = false;
                txtAccountName.Text = "";
                txtAccountEmail.Text = "";
                txtAccountPassword.Text = "";
                txtAccountRole.Text = "";
            }
            else
            {
                txtAccountID.Text = "";
                txtAccountName.Text = "";
                txtAccountEmail.Text = "";
                txtAccountPassword.Text = "";
                txtAccountRole.Text = "";
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var accList = iSystemAccountService.GetAccounts();
                foreach (var acc in accList)
                {
                    if (txtAccountID.Text.Equals(acc.AccountId.ToString()))
                    {
                        MessageBox.Show("ID exists");
                        //txtAccountID.Text = "";
                        return;
                    }
                }

                if(string.IsNullOrEmpty(txtAccountName.Text) ||
                    string.IsNullOrEmpty(txtAccountEmail.Text) ||
                    string.IsNullOrEmpty(txtAccountPassword.Text) ||
                    string.IsNullOrEmpty(txtAccountRole.Text))
                {
                    MessageBox.Show("All fields must be filled.");
                    return;
                }


                SystemAccount account = new SystemAccount();
                account.AccountId = short.Parse(txtAccountID.Text);
                account.AccountName = txtAccountName.Text;
                account.AccountEmail = txtAccountEmail.Text;
                account.AccountPassword = txtAccountPassword.Text;
                account.AccountRole = int.Parse(txtAccountRole.Text);
                iSystemAccountService.SaveAccount(account);
                MessageBox.Show("Create Account Successfully");
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(txtAccountID.Text.Length > 0)
                {
                    SystemAccount account = new SystemAccount();
                    account.AccountId = short.Parse(txtAccountID.Text);
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
