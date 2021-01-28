using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoucherCreateApp.Model;
using System.Data;

namespace VoucherCreateApp.Dao
{
    class VoucherGroupDao
    {
        public List<VoucherGroup> getList()
        {
            String sql = "select * from t_VoucherGroup";
            List<VoucherGroup> lst = new List<VoucherGroup>();
            foreach (DataRow row in DbHelper.getInstance().Query(sql).Tables[0].Rows)
            {
                VoucherGroup group = new VoucherGroup();
                group.GroupID = Convert.ToInt32(row["FGroupID"]);
                group.Name = Convert.ToString(row["FName"]);
                lst.Add(group);
            }
            return lst;
        }
    }
}
