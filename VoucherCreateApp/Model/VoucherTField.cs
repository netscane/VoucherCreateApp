using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoucherCreateApp.Model
{
    /**
     * 可选的金额来源
     * */
    class VoucherTField
    {
        public int InterID { set; get; }
        public int BillType { set; get; }
        public String Field { set; get; }
        public int TplType { set; get; }
        public String FieldName { set; get; }
    }
}
