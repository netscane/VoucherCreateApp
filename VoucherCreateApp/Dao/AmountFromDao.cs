using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VoucherCreateApp.Model;

namespace VoucherCreateApp.Dao
{
    class AmountFromDao
    {
        public List<AmountFrom> getList()
        {
            String sql = "select * from ICVoucherTField where FTplType = 801";
            List<AmountFrom> lst = new List<AmountFrom>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                AmountFrom amtf = new AmountFrom();
                amtf.Field = Convert.ToString(row["FField"]);
                amtf.FieldName = Convert.ToString(row["FFieldName"]);
                amtf.InterID = Convert.ToInt32(row["FInterID"]);
                lst.Add(amtf);
            }
            return lst;
        }
    }
}
