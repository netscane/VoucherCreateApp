using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoucherCreateApp.Model
{
    class BillField
    {
        public int BillType { set; get; }
        public string Field{set;get;}
        public string Name {set;get;}
        public int ItemClassID { set; get; }
        public int IsEntry { set; get; }
        public int TestID { set; get; }
    }
}
