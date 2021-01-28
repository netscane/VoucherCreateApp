using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VoucherCreateApp.Model;
using System.Collections.ObjectModel;

namespace VoucherCreateApp.Dao
{
    class ItemClassDao
    {
        public ObservableCollection<ItemClass> getCollection()
        {
            String sql = "select FItemClassID,FNumber,FName as FName,' ' As FFieldName From t_itemclass Where FType=1 and FItemClassID not in(6,7,2023) and FItemClassID>0";
            ObservableCollection<ItemClass> obs = new ObservableCollection<ItemClass>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                ItemClass itemCls = new ItemClass();
                itemCls.ItemClassID = Convert.ToInt32(row["FItemClassID"]);
                itemCls.Number = Convert.ToString(row["FNumber"]);
                itemCls.Name = Convert.ToString(row["FName"]);
                itemCls.Field = Convert.ToString(row["FFieldName"]);
                obs.Add(itemCls);
            }
            return obs;
        }
        public ObservableCollection<ItemClass> getCollectionByAccount(int accountID)
        {
            //String sql = "select FItemClassID,FNumber,FName as FName,' ' As FFieldName From t_itemclass Where FType=1 and FItemClassID not in(6,7,2023) and FItemClassID>0";
            String sql = @"select FItemClassID,FNumber,FName as FName,' ' As FFieldName From t_itemclass Where FItemClassID in 
(select FItemClassID  from t_ItemDetailV left join t_account on t_account.FDetailID= t_ItemDetailV.FDetailID where t_account.FAccountID = " + accountID + " and t_Account.FDetailID>0)";
            ObservableCollection<ItemClass> obs = new ObservableCollection<ItemClass>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                ItemClass itemCls = new ItemClass();
                itemCls.ItemClassID = Convert.ToInt32(row["FItemClassID"]);
                itemCls.Number = Convert.ToString(row["FNumber"]);
                itemCls.Name = Convert.ToString(row["FName"]);
                itemCls.Field = Convert.ToString(row["FFieldName"]);
                obs.Add(itemCls);
            }
            return obs;
        }
    }
}
