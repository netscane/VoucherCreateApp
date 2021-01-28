using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoucherCreateApp.Model
{
    /**
     * 每条分录的核算项目
     * */
    class ICVoucherTAudit
    {
        public int InterID { set; get; }
        public int EntryID { set; get; }
        public int ItemClassID { set; get; }
        public int Number { set; get; }
        public String Name { set; get; }

        public String FieldName { set; get; }
        public bool IsEntry{set;get;}
    }
}
