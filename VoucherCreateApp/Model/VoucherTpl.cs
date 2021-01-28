using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace VoucherCreateApp.Model
{
    public class VoucherTpl
    {
        public int InterID { set; get; }
        public String BillNo{set;get;}
        public int GroupID{set;get;}
        public String Name { set; get; }
        public DateTime LastUpdateTime { set; get; }
        public int BillerID { set; get; }
        public int IsDefault { set; get; }
        public ObservableCollection<VoucherTplEntry> EntryList { set; get; }
       // public String BillerName { set; get; }
        public int TransType { set; get; }
    }

    public class VoucherTplSimple
    {
        public int InterID { set; get; }
        public String Name { set; get; }
        public int TransType { set; get; }
    }

    public class VoucherTplInfo
    {
        public bool IsSelected { set; get; }
        public String BillNo { set; get; }
        public String GroupName { set; get; }
        public String Name { set; get; }
        public DateTime LastUpdateTime { set; get; }
        public String BillerName { set; get; }
        public String IsDefault { set; get; }
        public String TransTypeName { set; get; }
    }
}
