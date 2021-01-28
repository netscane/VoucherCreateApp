using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoucherCreateApp.Model;
using System.Data;

using System.Collections.ObjectModel;

namespace VoucherCreateApp.Dao
{
    class UserDao
    {
        public List<User> getUserList()
        {
            String sql = "select FUserID,FName from t_user";
            List<User> lst = new List<User>();
            foreach(DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                User user = new User();
                user.UserID = Convert.ToInt32(row["FUserID"]);
                user.Name = Convert.ToString(row["FName"]);
                lst.Add(user);
            }
            return lst;
        }

        public ObservableCollection<User> getCollection()
        {
            String sql = "select FUserID,FName from t_user where FUserID > 0 order by FName";
            ObservableCollection<User> obs = new ObservableCollection<User>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                User user = new User();
                user.UserID = Convert.ToInt32(row["FUserID"]);
                user.Name = Convert.ToString(row["FName"]);
                obs.Add(user);
            }
            return obs;
        }

        public DataSet getUserSet()
        {
            String sql = "select FUserID as UserID,FName as Name from t_user";
            return DbHelper.getInstance().Query(sql);
        }
    }
}
