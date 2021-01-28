using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoucherCreateApp.Model;
using System.Data;
using System.Collections.ObjectModel;

namespace VoucherCreateApp.Dao
{
    class AccountDao
    {
        public List<Account> getList()
        {
            String sql = "select FAccountID,FName,FNumber from t_account";
            List<Account> lst = new List<Account>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                Account acct = new Account();
                acct.AccountID = Convert.ToInt32(row["FAccountID"]);
                acct.Number = Convert.ToString(row["FNumber"]);
                acct.Name = Convert.ToString(row["FName"]);
                lst.Add(acct);
            }
            return lst;
        }
        public ObservableCollection<Account> getCollection()
        {
            String sql = "select FAccountID,FName,FNumber from t_account order by FNumber";
            ObservableCollection<Account> obs = new ObservableCollection<Account>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                Account acct = new Account();
                acct.AccountID = Convert.ToInt32(row["FAccountID"]);
                acct.Number = Convert.ToString(row["FNumber"]);
                acct.Name = Convert.ToString(row["FName"]);
                obs.Add(acct);
            }
            return obs;
        }
    }
}
