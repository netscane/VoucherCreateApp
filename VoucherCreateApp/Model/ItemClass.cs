using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoucherCreateApp.Model
{
    public class ItemClass
    {
        public int ItemClassID { set; get; }
        public string Number { set; get; }
        public string Name { set; get; }

        public string Field { set; get; }
        public int IsEntry { set; get; }
    }
}
