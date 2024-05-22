using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO
    {
        public static SystemAccount GetAccountById(string accountEmail)
        {
            using var db = new FunewsManagementDbContext();
            var test = db.SystemAccounts.FirstOrDefault(c => c.AccountEmail.Equals(accountEmail));
            return test;
        }

        public static List<SystemAccount> GetSystemAccounts()
        {
            var listAccounts = new List<SystemAccount>();
            try
            {
                using var context = new FunewsManagementDbContext();
                listAccounts = context.SystemAccounts.ToList();
                
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listAccounts;
        }

        public static void SaveAccount(SystemAccount account)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                context.SystemAccounts.Add(account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateAccount(SystemAccount account)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                context.Entry<SystemAccount>(account).State
                    = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public static void DeleteAccount(SystemAccount account)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                var ac = context.SystemAccounts.SingleOrDefault(acc => acc.AccountId == account.AccountId);
                context.SystemAccounts.Remove(account);
                context.SaveChanges();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
