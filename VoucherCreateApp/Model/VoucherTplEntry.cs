using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace VoucherCreateApp.Model
{
    public class VoucherTplEntry
    {
        public int InterID { set; get; }
        public int EntryID { set; get; }
        public String Note { set; get; }
        public int AccID { set; get; }
        public int AmountFrom { set; get; }
        public int Direction { set; get; }

        public ObservableCollection<ItemClass> itemClassLst{set;get;}
    }
}
