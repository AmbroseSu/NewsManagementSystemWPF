using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using BusinessObject;

namespace Repository
{
    public interface ISystemAccountRepository
    {
        List<SystemAccount> GetSystemAccounts();
        SystemAccount GetSystemAccountById(string accountEmail);
        void SaveAccount(SystemAccount account);
        void UpdateAccount(SystemAccount account);
        void DeleteAccount(SystemAccount account);
    }
}
