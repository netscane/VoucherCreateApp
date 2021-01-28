using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoucherCreateApp.Model;
using System.Data;
using System.Collections.ObjectModel;

namespace VoucherCreateApp.Dao
{
    class BillFieldDao
    {
        public List<BillField> getList(int transType)
        {
            String sql = "select distinct FAuditName,FItemClassID,FAuditField,FIsEntry from ICVoucherTBill where FBillType = 80 or FBillType = 86";
            List<BillField> lst = new List<BillField>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                BillField field = new BillField();
                field.ItemClassID = Convert.ToInt32(row["FItemClassID"]);
                field.Field = Convert.ToString(row["FAuditField"]);
                field.Name = Convert.ToString(row["FAuditName"]);
                field.IsEntry = Convert.ToInt32(row["FIsEntry"]);
                lst.Add(field);
            }
            return lst;
        }

        public ObservableCollection<BillField> getCollection(int transType)
        {
            String sql = "select distinct FAuditName,FItemClassID,FAuditField,FIsEntry from ICVoucherTBill where FBillType = 80 or FBillType = 86";
            ObservableCollection<BillField> obs = new ObservableCollection<BillField>();
            int i = 1;
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                BillField field = new BillField();
                field.ItemClassID = Convert.ToInt32(row["FItemClassID"]);
                field.Field = Convert.ToString(row["FAuditField"]);
                field.Name = Convert.ToString(row["FAuditName"]);
                field.IsEntry = Convert.ToInt32(row["FIsEntry"]);
                field.TestID = i++;
                obs.Add(field);
            }
            return obs;
        }

        public int getItemClassID(int BillType,string fieldName)
        {
            String sqlF = "select FItemClassID from ICVoucherTBill where (FBillType=80 or FBillType=86) and FAuditField='{1}'";
            string sql = string.Format(sqlF, BillType, fieldName);
            Object val = DbHelper.getInstance().SelectOnlyValue(sql);
            return Convert.ToInt32(val);
        }
    }
}
