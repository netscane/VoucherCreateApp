using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoucherCreateApp.Model;
using System.Data;
using System.Collections.ObjectModel;

namespace VoucherCreateApp.Dao
{
    class VoucherTplDao
    {
        DbHelper db = DbHelper.getInstance();
        public VoucherTpl getDetailByBillNo(String BillNo)
        {
            if (String.IsNullOrEmpty(BillNo))
                return null;
            String sql = "select * from fsICVoucherTpl t1 where t1.FBillNo='"+ BillNo +"'";
            DataSet ds = DbHelper.getInstance().Query(sql);
            if (!DbHelper.getInstance().IsEmpty(ds))
            {
                VoucherTpl tpl = new VoucherTpl();
                //取表头
                tpl.InterID = Convert.ToInt32(ds.Tables[0].Rows[0]["FInterID"]);
                tpl.Name = Convert.ToString(ds.Tables[0].Rows[0]["FName"]);
                tpl.GroupID = Convert.ToInt32(ds.Tables[0].Rows[0]["FGroupID"]);
                tpl.BillerID = Convert.ToInt32(ds.Tables[0].Rows[0]["FBillerID"]);
                tpl.BillNo = Convert.ToString(ds.Tables[0].Rows[0]["FBillNo"]);
                tpl.LastUpdateTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["FDate"]);
                tpl.TransType = Convert.ToInt32(ds.Tables[0].Rows[0]["FTplType"]);
                tpl.IsDefault = Convert.ToInt32(ds.Tables[0].Rows[0]["FIsDefault"]);
                //取表体

                sql = "select * from fsICVoucherTplEntry where FInterID=" + tpl.InterID;
                DataSet ds2 = DbHelper.getInstance().Query(sql);
                ObservableCollection<VoucherTplEntry> entryList = new ObservableCollection<VoucherTplEntry>();
                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    VoucherTplEntry entry = new VoucherTplEntry();
                    
                    entry.InterID = tpl.InterID;
                    entry.AccID = Convert.ToInt32(row["FAccID"]);
                    entry.AmountFrom = Convert.ToInt32(row["FAmountFrom"]);
                    entry.EntryID = Convert.ToInt32(row["FEntryID"]);
                    entry.Note = Convert.ToString(row["FNote"]);
                    entry.Direction = Convert.ToInt32(row["FAccDC"]);
                    //取核算项目
                    sql = "select * from fsICVoucherTAudit where FInterID=" + tpl.InterID + " and FEntryID=" + entry.EntryID;
                    DataSet ds3 = db.Query(sql);
                    ObservableCollection<ItemClass> itemLst = new ObservableCollection<ItemClass>();

                    foreach (DataRow itemRow in ds3.Tables[0].Rows)
                    {
                        ItemClass item = new ItemClass();
                        item.Field = Convert.ToString(itemRow["FFieldName"]);
                        item.IsEntry = Convert.ToInt32(itemRow["FIsEntry"]);
                        item.ItemClassID = Convert.ToInt32(itemRow["FItemClassID"]);
                        item.Name = Convert.ToString(itemRow["FName"]);
                        item.Number = Convert.ToString(itemRow["FNumber"]);
                        itemLst.Add(item);
                    }
                    entry.itemClassLst = itemLst;
                    

                    entryList.Add(entry);
                }
                tpl.EntryList = entryList;
                return tpl;
            }
            return null;
        }
        public VoucherTpl getDetailByInterID(int InterID)
        {
            String sql = "select * from fsICVoucherTpl t1 where t1.FInterID=" + InterID + "";
            DataSet ds = DbHelper.getInstance().Query(sql);
            if (!DbHelper.getInstance().IsEmpty(ds))
            {
                VoucherTpl tpl = new VoucherTpl();
                //取表头
                tpl.InterID = Convert.ToInt32(ds.Tables[0].Rows[0]["FInterID"]);
                tpl.Name = Convert.ToString(ds.Tables[0].Rows[0]["FName"]);
                tpl.GroupID = Convert.ToInt32(ds.Tables[0].Rows[0]["FGroupID"]);
                tpl.BillerID = Convert.ToInt32(ds.Tables[0].Rows[0]["FBillerID"]);
                tpl.BillNo = Convert.ToString(ds.Tables[0].Rows[0]["FBillNo"]);
                tpl.LastUpdateTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["FDate"]);
                tpl.TransType = Convert.ToInt32(ds.Tables[0].Rows[0]["FTplType"]);
                tpl.IsDefault = Convert.ToInt32(ds.Tables[0].Rows[0]["FIsDefault"]);
                //取表体

                sql = "select * from fsICVoucherTplEntry where FInterID=" + tpl.InterID;
                DataSet ds2 = DbHelper.getInstance().Query(sql);
                ObservableCollection<VoucherTplEntry> entryList = new ObservableCollection<VoucherTplEntry>();
                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    VoucherTplEntry entry = new VoucherTplEntry();

                    entry.InterID = tpl.InterID;
                    entry.AccID = Convert.ToInt32(row["FAccID"]);
                    entry.AmountFrom = Convert.ToInt32(row["FAmountFrom"]);
                    entry.EntryID = Convert.ToInt32(row["FEntryID"]);
                    entry.Note = Convert.ToString(row["FNote"]);
                    entry.Direction = Convert.ToInt32(row["FAccDC"]);
                    //取核算项目
                    sql = "select * from fsICVoucherTAudit where FInterID=" + tpl.InterID + " and FEntryID=" + entry.EntryID;
                    DataSet ds3 = db.Query(sql);
                    ObservableCollection<ItemClass> itemLst = new ObservableCollection<ItemClass>();

                    foreach (DataRow itemRow in ds3.Tables[0].Rows)
                    {
                        ItemClass item = new ItemClass();
                        item.Field = Convert.ToString(itemRow["FFieldName"]);
                        item.IsEntry = Convert.ToInt32(itemRow["FIsEntry"]);
                        item.ItemClassID = Convert.ToInt32(itemRow["FItemClassID"]);
                        item.Name = Convert.ToString(itemRow["FName"]);
                        item.Number = Convert.ToString(itemRow["FNumber"]);
                        itemLst.Add(item);
                    }
                    entry.itemClassLst = itemLst;


                    entryList.Add(entry);
                }
                tpl.EntryList = entryList;
                return tpl;
            }
            return null;
        }
        public List<VoucherTplSimple> getAll()
        {
            String sql = "select * from fsICVoucherTpl";
            DataSet ds = db.Query(sql);
            List<VoucherTplSimple> lst = new List<VoucherTplSimple>();
            if (!DbHelper.getInstance().IsEmpty(ds))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    VoucherTplSimple tpl = new VoucherTplSimple();
                    //取表头
                    tpl.InterID = Convert.ToInt32(row["FInterID"]);
                    tpl.Name = Convert.ToString(row["FName"]);
                    tpl.TransType = Convert.ToInt32(row["FTplType"]);
                    lst.Add(tpl);
                }
            }
            return lst;
        }
        public List<VoucherTplInfo> getListInfo()
        {
            String sql = @"select t1.FBillNo,t1.FName ,t1.FDate FLastUpdateTime,t2.FName FBillerName,t3.FName FGroupName,t4.FName FTransTypeName,
case when t1.FIsDefault = 0 then '否' else '是' end as FIsDefault
from fsICVoucherTpl t1
left join t_user t2 on t1.FBillerID = t2.FUserID
left join t_VoucherGroup t3 on t1.FGroupID = t3.FGroupID
left join ICTransType t4 on t1.FTplType = t4.FID";
            DataSet ds = db.Query(sql);
            List<VoucherTplInfo> lst = new List<VoucherTplInfo>();
            if (!DbHelper.getInstance().IsEmpty(ds))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    VoucherTplInfo tpl = new VoucherTplInfo();
                    //取表头
                    tpl.BillNo = Convert.ToString(row["FBillNo"]);
                    tpl.Name = Convert.ToString(row["FName"]);
                    tpl.GroupName = Convert.ToString(row["FGroupName"]);
                    tpl.BillerName = Convert.ToString(row["FBillerName"]);
                    tpl.TransTypeName = Convert.ToString(row["FTransTypeName"]);
                    tpl.LastUpdateTime = Convert.ToDateTime(row["FLastUpdateTime"]);
                    tpl.IsDefault = Convert.ToString(row["FIsDefault"]);
                    
                    lst.Add(tpl);
                }
            }
            return lst;
        }
        public void DeleteVoucherTpl(string billNo)
        {
            int interID = Convert.ToInt32( db.SelectOnlyValue("select FInterID from fsICVoucherTpl where FBillNo='" + billNo + "'"));
            List<string> sqllst = new List<string>();
            sqllst.Add("delete from fsICVoucherTpl where FInterID=" + interID);
            sqllst.Add("delete from fsICVoucherTplEntry where FInterID=" + interID);
            sqllst.Add("delete from fsICVoucherTAudit where FInterID=" + interID);
            db.LongHaul(sqllst);
        }
        public void SaveVoucherTpl(VoucherTpl voucherTpl)
        {
            string sql, sqlF;
            List<string> sqllst = new List<string>();

            if (voucherTpl.InterID != 0)
            {
                //更新
                //更新表头
                sqlF = "update fsICVoucherTpl set FName='{0}',FDate=GetDate(),FGroupID={1},FIsDefault={2},FBillerID={3} where FInterID="+voucherTpl.InterID;
                sql = string.Format(sqlF, voucherTpl.Name,
                    voucherTpl.GroupID, voucherTpl.IsDefault, voucherTpl.BillerID);
                sqllst.Add(sql);
                //移除表体
                sql = "delete from fsICVoucherTplEntry where FInterID=" + voucherTpl.InterID;
                sqllst.Add(sql);
                sql = "delete from fsICVoucherTAudit where FInterID=" + voucherTpl.InterID;
                sqllst.Add(sql);
            }
            else
            {
                //新建
                //生成编号
                Object val = db.SelectOnlyValue("select MAX(FInterID)+1 from fsICVoucherTpl");
                int interID = (val == null || val == DBNull.Value) ? 1001 : Convert.ToInt32(val);
                voucherTpl.InterID = interID;

                //存表头
                string BillNo = "TPL" + (voucherTpl.InterID - 1000).ToString().PadLeft(4, '0');
                voucherTpl.BillNo = BillNo;

                sqlF = @"insert into fsICVoucherTpl(
                    FInterID,FName,FDate,FTplType,
                    FBillNo,FGroupID,FCountPrice,FIsSystem,
                    FIsDefault,FBillerID,FSubSysID) VALUES(
                    {0},'{1}',GETDATE(),{2},
                    '{3}',{4},0,0,
                    {5},{6},0  
                    )";
                sql = string.Format(sqlF, voucherTpl.InterID, voucherTpl.Name,
                    voucherTpl.TransType, voucherTpl.BillNo, voucherTpl.GroupID, voucherTpl.IsDefault, voucherTpl.BillerID);
                sqllst.Add(sql);

            }

            //修改默认模板
            if (voucherTpl.IsDefault == 1)
            {
                sql = "update fsICVoucherTpl set FIsDefault=0 where FTplType=" + voucherTpl.TransType + " and FInterID <>"+voucherTpl.InterID;
                sqllst.Add(sql);
            }

            //存表体
            foreach (VoucherTplEntry entry in voucherTpl.EntryList)
            {
                sqlF = @"insert into fsICVoucherTplEntry(
                        FInterID,FEntryID,FNote,FAccID,
                        FAccProperty,FAmountFrom,FAccDC) VALUES(
                        {0},{1},'{2}',{3},
                        0,{4},{5}
                        )";
                sql = string.Format(sqlF, voucherTpl.InterID, entry.EntryID, entry.Note, entry.AccID,
                      entry.AmountFrom, entry.Direction);
                sqllst.Add(sql);
                //存核算项目
                foreach (ItemClass item in entry.itemClassLst)
                {
                    sql = "select FIsEntry from ICVoucherTBill where (FBillType = 80 or FBillType = 86) and FAuditField='" + item.Field + "'";
                    int isEntry = Convert.ToInt32(db.SelectOnlyValue(sql));

                    sqlF = @"insert into fsICVoucherTAudit(
                            FInterID,FEntryID,FItemClassID,
                            FNumber,FName,FFieldName,FIsEntry
                            ) VALUES(
                            {0},{1},{2},
                            '{3}','{4}','{5}',{6}
                            )";
                    sql = string.Format(sqlF, voucherTpl.InterID, entry.EntryID, item.ItemClassID,
                        item.Number, item.Name, item.Field, isEntry);
                    sqllst.Add(sql);
                }
            }
            db.LongHaul(sqllst);
        }
    }
}
