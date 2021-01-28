using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoucherCreateApp.Model
{
    class ICSale
    {
        public bool? IsSelected { get; set; }
        public String FTransTypeIDName { get; set; }
        public String FCheck { get; set; }
        public DateTime FDate { get; set; }
        public DateTime FSettleDate { get; set; }
        public String FCustIDName { get; set; }
        public String FBillNo { get; set; }
        public String FDeptIDName { get; set; }
        public String FEmpIDName { get; set; }
        public String FCurrencyIDName { get; set; }
        public String FItemName { get; set; }
        public String FItemModel { get; set; }
        public String FBaseUnitID { get; set; }
        public String FAuxQty { get; set; }
        public String FAuxPrice { get; set; }
        public String FTaxAmount { get; set; }
        public String FAllAmount { get; set; }
        public String FItemNumber { get; set; }
        public String FVoucherNumber { get; set; }
        public String FAuxTaxPrice { get; set; }
        public String FStdAllAmount { get; set; }
    }

    class ICSaleDisplay
    {
        public bool? IsSelected { get; set; }
        public int FVoucherTplID { set; get; }
        public int FTransType { set; get; }
        public String FTransTypeIDName { get; set; }
        public String FCheck { get; set; }
        public DateTime FDate { get; set; }
        public DateTime FSettleDate { get; set; }
        public String FCustIDName { get; set; }
        public String FBillNo { get; set; }
        public String FDeptIDName { get; set; }
        public String FEmpIDName { get; set; }
        public String FCurrencyIDName { get; set; }
        public List<ICSaleEntryDisplay> Entries { set; get; }
    }
    class ICSaleEntryDisplay
    {
        public String FItemName { get; set; }
        public String FItemModel { get; set; }
        public String FBaseUnitID { get; set; }
        public String FAuxQty { get; set; }
        public String FAuxPrice { get; set; }
        public String FTaxAmount { get; set; }
        public String FAllAmount { get; set; }
        public String FItemNumber { get; set; }
        public String FVoucherNumber { get; set; }
        public String FAuxTaxPrice { get; set; }
        public String FStdAllAmount { get; set; }
    }
    class ICSaleHeader
    {
        public int InterID {set;get;}
        public string BillNo { set; get; }
        public int TranType { set; get; }
        public DateTime Dt { set; get; }
        public int CustID { set; get; }
        public int CurrencyID { set; get; }
        public int DeptID { set; get; }
        public int EmpID { set; get; }
        public int VchInterID { set; get; }
        public float ExchangeRate { set; get; }
    }

    class ICSaleEntry
    {
        public int InterID { set; get; }
        public int EntryID { set; get; }
        public int ItemID { set; get; }
        public float Qty { set; get; }
        public float Price { set; get; }
        public float Amount { set; get; }
        public float TaxRate { set; get; }
        public float TaxAmount { set; get; }
        public float StdAmount { set; get; }
    }
}
