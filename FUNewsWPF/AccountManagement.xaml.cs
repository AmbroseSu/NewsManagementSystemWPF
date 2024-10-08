﻿using BusinessObject;
using DataAccess;
using Microsoft.Identity.Client.NativeInterop;
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
            txtAccountID.IsEnabled = false;
            txtAccountName.IsEnabled = false;
            txtAccountEmail.IsEnabled = false;
            txtAccountPassword.IsEnabled = false;
            txtAccountRole.IsEnabled = false;
            if (cboSearch.Items.Count > 0)
            {
                // Lấy ComboBoxItem đầu tiên
                ComboBoxItem firstItem = cboSearch.Items[0] as ComboBoxItem;
                if (firstItem != null)
                {
                    // Gán nội dung của ComboBoxItem đầu tiên vào cboSearch.Text
                    cboSearch.Text = firstItem.Content.ToString();
                }
            }

        }

        private void resetInput()
        {
            txtAccountID.Text = "";
            txtAccountName.Text = "";
            txtAccountEmail.Text = "";
            txtAccountRole.Text = "";
            txtAccountPassword.Text= "";
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            register.Closed += (s, args) =>
            {
                LoadSystemAccountList();
            };
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(dgAccounts.SelectedItem != null)
            {
                SystemAccount selectAccount = dgAccounts.SelectedItem as SystemAccount;
                Register register = new Register(selectAccount);
                register.Show();
                register.Closed += (s, args) =>
                {
                    LoadSystemAccountList();
                };
            }
            else
            {
                MessageBox.Show("Please select a Account to update.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtAccountID.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this account", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.OK)
                    {
                        //SystemAccount account = new SystemAccount();
                        SystemAccount account = iSystemAccountService.GetAccountById(short.Parse(txtAccountID.Text));
                        /*account.AccountId = short.Parse(txtAccountID.Text);
                        account.AccountName = txtAccountName.Text;
                        account.AccountEmail = txtAccountEmail.Text;
                        account.AccountPassword = txtAccountPassword.Text;
                        account.AccountRole = int.Parse(txtAccountRole.Text);*/
                        iSystemAccountService.DeleteAccount(account);
                        MessageBox.Show("Account delete successfully");
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("You must select a Account.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadSystemAccountList();
            }
        }

        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgAccounts.SelectedIndex >= 0)
                {
                    SystemAccount selectAccount = dgAccounts.SelectedItem as SystemAccount;
                    if (selectAccount != null)
                    {
                        txtAccountID.Text = selectAccount.AccountId.ToString();
                        txtAccountName.Text = selectAccount.AccountName;
                        txtAccountEmail.Text = selectAccount.AccountEmail;
                        txtAccountPassword.Text = selectAccount.AccountPassword;
                        txtAccountRole.Text = selectAccount.AccountRole.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.ToLower();
                string searchCriterion = (cboSearch.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrWhiteSpace(searchText) || string.IsNullOrWhiteSpace(searchCriterion))
                {
                    var accountList = iSystemAccountService.GetAccounts();
                    dgAccounts.ItemsSource = accountList;
                    return;
                }

                
                    List<SystemAccount> searchResults = new List<SystemAccount>();

                    switch (searchCriterion)
                    {
                        case "Account ID":
                            if (short.TryParse(searchText, out short accountId))
                            {
                                SystemAccount account = iSystemAccountService.GetAccountById(accountId);
                                if(account != null)
                                {
                                searchResults.Add(account);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please enter a valid Account ID.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            break;

                        case "Account Name":
                            searchResults = iSystemAccountService.GetSystemAccountsByName(searchText);
                            break;

                        case "Account Email":
                            searchResults = iSystemAccountService.GetSystemAccountsByEmail(searchText);
                            break;

                        case "Account Role":
                            searchResults = iSystemAccountService.GetSystemAccountsByRole(int.Parse(searchText));
                            break;

                        default:
                            MessageBox.Show("Please select a valid search criterion.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                    }

                    dgAccounts.ItemsSource = searchResults;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
