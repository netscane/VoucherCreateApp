using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VoucherCreateApp.Model;

namespace VoucherCreateApp.Dao
{
    class ICSaleDao
    {
        public List<ICSaleDisplay> getICSaleDisplayList()
        {
            String sql = @"SELECT
            t_fstpl.FInterID as FVoucherTplID,
            u1.FDetailID AS FListEntryID,
            0  AS FSel,
            t18.FName AS FPlanVchTplName,
            t19.FName AS FActualVchTplName,
            v1.FPlanVchTplID AS FPlanVchTplID,
            v1.FActualVchTplID AS FActualVchTplID,
            (u1.FQty-u1.FAllHookQTY) AS FHookQTY,
            ISNULL(v1.FVchInterID,0) AS FVchInterID,
            v1.FTranType AS FTranType,
            t_trans.FName as FTransTypeIDName,
            v1.FInterID AS FInterID,
            u1.FEntryID AS FEntryID,
            v1.FCheckerID AS FCheckerID,
            case  when v1.FCheckerID>0 then 'Y' when v1.FCheckerID<0 then 'Y' else '' end  AS FCheck,
            v1.Fdate,
            v1.FSettleDate AS FSettleDate,
            t4.FName AS FCustIDName,
            v1.FBillNo AS FBillNo,
            v1.FPrintCount AS FPrintCount,
            t12.FName AS FDeptIDName,
            t13.FName AS FEmpIDName,
            t15.FName AS FCurrencyIDName,
            t17.Fname AS FItemName,
            t17.Fmodel AS FItemModel,
            t20.FName AS FUnitIDName,
            u1.Fauxqty AS Fauxqty,
            u1.Fauxprice AS Fauxprice,
            v1.FOrgBillInterID AS FOrgBillInterID,
            u1.FTaxAmount AS FTaxAmount,
            case v1.FTranType when 80 then u1.FAmount+u1.FTaxAmount else u1.FAmount end AS FAllAmount,
            t25.FName AS FBaseUnitID,
            t4.FItemID AS FCustID,
            t17.FQtyDecimal AS FQtyDecimal,
            t17.FPriceDecimal AS FPriceDecimal,
            t17.FNumber AS FItemNumber,
            (SELECT (SELECT FName FROM t_VoucherGroup WHERE FGroupID=t_Voucher.FGroupID)+'-'+CONVERT(Varchar(30),FNumber)   FROM  t_Voucher  WHERE  FVoucherid=v1.FVchInterID)  AS FVoucherNumber,
            case when v1.FCancellation=1 then 'Y' else '' end AS FCancellation,
            case when (v1.FOrgBillInterID <> 0) then 'Y' else ''  end AS FHasSplitBill,
            CASE WHEN v1.FHookStatus=1 THEN 'P' WHEN V1.FHookStatus=2 THEN 'Y' ELSE '' END  AS FHookStatus,
            u1.FAuxTaxPrice AS FAuxTaxPrice,
            case when (v1.FROB <> 1) then 'Y' else '' end AS FRedFlag,
            case v1.FTranType when 80 then u1.FStdAmount+u1.FStdTaxAmount else u1.FStdAmount end AS FStdAllAmount,
            Case v1.FCheckStatus when 1 then 'Y'  when 2 then 'P'  else 'N' end  AS FArapStatus,
            Case WHEN t17.FSaleUnitID=0 THEN '' Else  t500.FName end AS FCUUnitName,
            Case When v1.FCurrencyID is Null Or v1.FCurrencyID='' then (Select FScale From t_Currency Where FCurrencyID=1) else t504.FScale end   AS FAmountDecimal,
            t506.FName AS FSecUnitName,
            u1.FSecCoefficient AS FSecCoefficient,
            u1.FSecQty AS FSecQty,
            CASE WHEN v1.FTranStatus=1 THEN 'Y' ELSE '' END AS FTranStatus,
            v1.FJSBillNo AS FJSBillNo,
            Case ISNULL(v1.FJSExported,0) when 1 then 'Y' else '' end  AS FJSExported,
            v1.FConfirmDate AS FConfirmDate,
            CASE v1.FConfirmFlag WHEN 1 THEN 'Y' WHEN 2 THEN 'N' ELSE '' END AS FConfirmFlag

            FROM ICSale v1 INNER JOIN ICSaleEntry u1 ON     v1.FInterID = u1.FInterID   AND u1.FInterID <>0 
             INNER JOIN t_Organization t4 ON     v1.FCustID = t4.FItemID   AND t4.FItemID <>0 
             LEFT OUTER JOIN t_Department t12 ON     v1.FDeptID = t12.FItemID   AND t12.FItemID <>0 
             LEFT OUTER JOIN t_Emp t13 ON     v1.FEmpID = t13.FItemID   AND t13.FItemID <>0 
             INNER JOIN t_Currency t15 ON     v1.FCurrencyID = t15.FCurrencyID   AND t15.FCurrencyID <>0 
             INNER JOIN t_ICItem t17 ON     u1.FItemID = t17.FItemID   AND t17.FItemID <>0 
             INNER JOIN t_MeasureUnit t20 ON     u1.FUnitID = t20.FItemID   AND t20.FItemID <>0 
             INNER JOIN t_MeasureUnit t25 ON     t17.FUnitID = t25.FItemID   AND t25.FItemID <>0 
             LEFT OUTER JOIN t_User t6 ON     v1.FHookerID = t6.FUserID   AND t6.FUserID <>0  AND  v1.FHookerID <> 0 
             LEFT OUTER JOIN ICVoucherTpl t18 ON     v1.FPlanVchTplID = t18.FInterID   AND t18.FInterID <>0 
             LEFT OUTER JOIN ICVoucherTpl t19 ON     v1.FActualVchTplID = t19.FInterID   AND t19.FInterID <>0 
             LEFT OUTER JOIN t_MeasureUnit t500 ON     t17.FSaleUnitID = t500.FItemID   AND t500.FItemID <>0 
             LEFT OUTER JOIN t_Currency t504 ON     v1.FCurrencyID = t504.FCurrencyID   AND t504.FCurrencyID <>0 
             LEFT OUTER JOIN t_MeasureUnit t506 ON     t17.FSecUnitID = t506.FItemID   AND t506.FItemID <>0 
             LEFT OUTER JOIN ICTransType t_trans on t_trans.FID = v1.FTranType
             LEFT OUTER JOIN fsICVoucherTpl t_fstpl on v1.FTranType = t_fstpl.FTplType and t_fstpl.FIsDefault=1
             WHERE v1.FVchInterID is null
             AND v1.FCheckerID is not null
             AND (v1.FTranType IN (80,86) AND (  (v1.FSaleStyle NOT IN (100,102,103,20298,20297,20296))   AND v1.FCheckerID>0  AND u1.FAmount<>0 ))
             ORDER BY v1.FInterID";
            DataSet ds = DbHelper.getInstance().Query(sql);
            
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<ICSaleDisplay> lst = new List<ICSaleDisplay>();
                ICSaleDisplay sale = null;
                int interID = 0;
                int lastInterID=0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    interID = Convert.ToInt32(row["FInterID"]);
                    ICSaleEntryDisplay entry = new ICSaleEntryDisplay();
                    
                    entry.FItemName = Convert.ToString(row["FItemName"]);
                    entry.FItemModel = Convert.ToString(row["FItemModel"]);
                    entry.FBaseUnitID = Convert.ToString(row["FBaseUnitID"]);
                    entry.FAuxQty = Convert.ToString(row["FAuxQty"]);
                    entry.FAuxPrice = Convert.ToString(row["FAuxPrice"]);
                    entry.FTaxAmount = Convert.ToString(row["FTaxAmount"]);
                    entry.FAllAmount = Convert.ToString(row["FAllAmount"]);
                    entry.FItemNumber = Convert.ToString(row["FItemNumber"]);
                    entry.FVoucherNumber = Convert.ToString(row["FVoucherNumber"]);
                    entry.FAuxTaxPrice = Convert.ToString(row["FAuxTaxPrice"]);
                    entry.FStdAllAmount = Convert.ToString(row["FStdAllAmount"]);
                    if (sale == null || lastInterID!=interID)
                    {
                        sale = new ICSaleDisplay();
                        sale.Entries = new List<ICSaleEntryDisplay>();
                        sale.IsSelected = false;
                        sale.FVoucherTplID = Convert.ToInt32(row["FVoucherTplID"]);
                        sale.FTransType = Convert.ToInt32(row["FTranType"]);
                        sale.FTransTypeIDName = Convert.ToString(row["FTransTypeIDName"]);
                        sale.FCheck = Convert.ToString(row["FCheck"]);
                        sale.FDate = Convert.ToDateTime(row["FDate"]);
                        sale.FSettleDate = Convert.ToDateTime(row["FSettleDate"]);
                        sale.FCustIDName = Convert.ToString(row["FCustIDName"]);
                        sale.FBillNo = Convert.ToString(row["FBillNo"]);
                        sale.FDeptIDName = Convert.ToString(row["FDeptIDName"]);
                        sale.FEmpIDName = Convert.ToString(row["FEmpIDName"]);
                        sale.FCurrencyIDName = Convert.ToString(row["FCurrencyIDName"]);
                        lst.Add(sale);
                    }
                    lst[lst.Count-1].Entries.Add(entry);
                    lastInterID = interID;
                }
                return lst;
            }
            else
                return null;
        }


        public ICSaleHeader getICSaleHeader(string billNo)
        {
            DbHelper db = DbHelper.getInstance();
            DataSet ds = db.Query("select * from icsale where fbillno='" + billNo + "'");

            if (!db.IsEmpty(ds))
            {
                DataRow row = ds.Tables[0].Rows[0];
                ICSaleHeader header = new ICSaleHeader();
                header.InterID = Convert.ToInt32(row["FInterID"]);
                header.BillNo = Convert.ToString(row["FBillNo"]);
                header.CurrencyID = Convert.ToInt32(row["FCurrencyID"]);
                header.CustID = Convert.ToInt32(row["FCustID"]);
                header.DeptID = Convert.ToInt32(row["FDeptID"]);
                header.Dt = Convert.ToDateTime(row["FDate"]);
                header.EmpID = Convert.ToInt32(row["FEmpID"]);
                header.ExchangeRate = (float)Convert.ToDouble(row["FExchangeRate"]);
                return header;
            }
            else
                return null;
        }

        public List<ICSaleEntry> getICSaleEntry(int interID)
        {
            DbHelper db = DbHelper.getInstance();
            DataSet ds = db.Query("select * from icsaleentry where finterid=" + interID);

            if (!db.IsEmpty(ds))
            {
                List<ICSaleEntry> lst = new List<ICSaleEntry>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ICSaleEntry entry = new ICSaleEntry();

                    entry.Amount = (float)Convert.ToDouble(row["FAmount"]);
                    entry.EntryID = Convert.ToInt32(row["FEntryID"]);
                    entry.InterID = Convert.ToInt32(row["FInterID"]);
                    entry.ItemID = Convert.ToInt32(row["FItemID"]);
                    entry.Price = (float)Convert.ToDouble(row["FPrice"]);
                    entry.Qty = (float)Convert.ToDouble(row["FQty"]);
                    entry.StdAmount = (float)Convert.ToDouble(row["FStdAmount"]);
                    entry.TaxAmount = (float)Convert.ToDouble(row["FTaxAmount"]);
                    entry.TaxRate = (float)Convert.ToDouble(row["FTaxRate"]);
                    lst.Add(entry);
                }
                
                return lst;
            }
            else
                return null;
        }
    }
}
