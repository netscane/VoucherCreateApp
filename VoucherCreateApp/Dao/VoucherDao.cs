using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoucherCreateApp.Model;

namespace VoucherCreateApp.Dao
{
    class VoucherDao
    {
        DbHelper db = DbHelper.getInstance();
        VoucherTplDao tplDao = new VoucherTplDao();
        ICSaleDao saleDao = new ICSaleDao();
        public int CreateVoucher(string userName,string billNo,OpResult result)
        {
            if (IsBillVoucherExisted(billNo))
            {
                result.ErrorCode = 3;
                result.ErrMessage = "单据" + billNo + "已经生成凭证";
                return 0;
            }

            ICSaleHeader saleHeader= saleDao.getICSaleHeader(billNo);
            List<ICSaleEntry> saleEntryLst = saleDao.getICSaleEntry(saleHeader.InterID);

            List<string> sqllst = new List<string>();
            string sql, sqlF;
            //寻找默认模板
            int year = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentYear' and FCategory='IC'"));
            int period = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentPeriod' and FCategory='IC'"));
         //   int year = Convert.ToInt32(db.SelectOnlyValue("select FYear from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
         //   int period = Convert.ToInt32(db.SelectOnlyValue("select FPeriod from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
            DateTime voucherDate = Convert.ToDateTime(db.SelectOnlyValue("select FEndDate from T_PeriodDate where FYear = "+year+" and FPeriod = "+period));
            int tranType = Convert.ToInt32(db.SelectOnlyValue("SELECT FTranType FROM ICSale where FBillNo ='" + billNo + "'"));

            sql = "select FBillNo from fsICVoucherTpl where FTplType=" +tranType + " and FIsDefault = 1";

            object val = db.SelectOnlyValue(sql);
            //如果没有一个默认模板，则返回
            if (val == null || val == DBNull.Value)
            {
                result.ErrorCode = 2;
                result.ErrMessage = "单据类型没有设置默认模板!";
                return 0;
            }
                
            VoucherTpl vchTpl = tplDao.getDetailByBillNo(val.ToString());

            int userID = Convert.ToInt32(db.SelectOnlyValue("select FUserID  from t_user where FName='"+userName+"'"));

            //0-贷方,1- 借方
            double debitTotal=0, creditAmount=0;

            //生成编号VoucherID,SerialNum,FNumber;
            int voucherId = Convert.ToInt32(db.SelectOnlyValue("select MAX(FVoucherID)+1 from t_Voucher"));
            val = db.SelectOnlyValue(@"SELECT MAX(FSerialNum) FROM (
     select * from t_Voucher 
     union all 
     select * from t_VoucherBlankout 
     union all 
     select * from t_VoucherAdjust) v 
 Where FYear="+year);
            int serialNum = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val);
            ++serialNum;
            sql = "Select max(FNumber)  from (select * from t_Voucher union all select * from t_VoucherBlankout) v where FYear = "+ year +" and FPeriod = " +period+ " and FGroupID = "+vchTpl.GroupID;
            val = db.SelectOnlyValue(sql);
            int vchNumber = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val)+1;

            int vchEntryID = 0;

            //Note处理 收入: [销售发票.购货单位][销售发票.合同单号][销售发票.进度提醒]
            string note = "";
            sql = @"select '收入: '+CONVERT(NVARCHAR,t1.FName) + ISNULL(t0.FNote,'') + ISNULL(t0.FHeadSelfI0559,'')+ISNULL(t0.FHeadSelfI0461,'') from ICSale t0
left join t_Item t1 on t0.FCustID = t1.FItemID
where FBillNo  ='" + billNo + "'";
            try
            {
                note = db.SelectOnlyValue(sql).ToString();
            }
            catch (Exception e)
            {
                note = "凭证分录摘要获取异常";
            }
            

            //插入凭证分录
            foreach (VoucherTplEntry entry in vchTpl.EntryList)
            {
                //核算项目是否有发票分录字段
                bool hasEntryItem = false;
                //有没有核算项目
                int itemsCount = 0;
                HashSet<int> acctSet = new HashSet<int>();
                foreach (ItemClass item in entry.itemClassLst)
                {
                    if (item.IsEntry == 1) 
                    {
                        hasEntryItem = true;
                    }
                    if (!string.IsNullOrWhiteSpace(item.Field))
                        ++itemsCount;
                }
                //选择对方科目
                sql = "select FAccID from fsICVoucherTplEntry where FInterID=" + entry.InterID + " and FAccID <> " + entry.AccID;
                int acct2 = Convert.ToInt32(db.SelectOnlyValue(sql));
                
                
                if (hasEntryItem == true)
                {
                    //插入核算项目,每条数据分录都要插入核算项目
                    foreach (ICSaleEntry saleEntry in saleEntryLst)
                    {
                        //select FDetailID from t_ItemDetail where FDetailCount= 2 and F1=11323 and F2003=232
                        sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                        foreach (ItemClass item in entry.itemClassLst)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                sql += " and F" + item.ItemClassID + " = ";
                                int itemValue = 0;
                                //取核算项目值
                                if (item.IsEntry == 1)
                                {
                                    string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);
                                }
                                else
                                {
                                    string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);
                                }

                                sql += itemValue;
                            }
                        }

                        int detailID = 0;
                        val = db.SelectOnlyValue(sql);
                        if (val == null || val == DBNull.Value)
                        {
                            //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                            int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                            detailID = maxDetailID + 1;
                            List<string> tmpSqlLst = new List<string>();
                            //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                            sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                            foreach (ItemClass item in entry.itemClassLst)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    sql += ",F" + item.ItemClassID;
                                }
                            }

                            sql += ") values(" + detailID + "," + itemsCount;

                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值
                                    if (item.IsEntry == 1)
                                    {
                                        string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    else
                                    {
                                        string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    sql += "," + itemValue;
                                }
                            }

                            sql += ")";

                            tmpSqlLst.Add(sql);
                            //插入 t_itemDetailV;
                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值
                                    if (item.IsEntry == 1)
                                    {
                                        string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    else
                                    {
                                        string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                    tmpSqlLst.Add(sql);
                                }
                            }
                            db.LongHaul(tmpSqlLst);
                        }
                        else
                            detailID = Convert.ToInt32(val);

                        sqlF = @"INSERT INTO t_VoucherEntry
                           (FBrNo
                           ,FVoucherID
                           ,FEntryID
                           ,FExplanation
                           ,FAccountID
                           ,FDetailID
                           ,FCurrencyID
                           ,FExchangeRate
                           ,FDC
                           ,FAmountFor
                           ,FAmount
                           ,FQuantity
                           ,FMeasureUnitID
                           ,FUnitPrice
                           ,FInternalInd
                           ,FAccountID2
                           ,FSettleTypeID
                           ,FSettleNo
                           ,FTransNo
                           ,FCashFlowItem
                           ,FTaskID
                           ,FResourceID
                           ,FExchangeRateType
                           ,FSideEntryID)
                     VALUES
                           (0
                           ,{0}
                           ,{1}
                           ,'{2}'
                           ,{3}
                           ,{4}
                           ,{5}
                           ,{6}
                           ,{7}
                           ,{8}
                           ,{9}
                           ,0
                           ,0
                           ,0
                           ,'Industry'
                           ,{10}
                           ,0
                           ,null
                           ,null
                           ,0
                           ,0
                           ,0
                           ,1
                           ,0)";

                        //取amount
                        double amount = 0;
                        string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID="+entry.AmountFrom).ToString();
                        sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID="+ saleHeader.InterID +" and t2.FEntryID="+saleEntry.EntryID;
                        amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                        if (entry.Direction == 0)
                            debitTotal += amount;
                        else
                            creditAmount += amount;
                        sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                            saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                        sqllst.Add(sql);
                        vchEntryID++;//凭证分录序号加1
                    }
                }
                else
                {
                    //如果核算项目没有分录选项
                    sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                    foreach (ItemClass item in entry.itemClassLst)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Field))
                        {
                            sql += " and F" + item.ItemClassID + " = ";
                            int itemValue = 0;

                            string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                            object tmpVal = db.SelectOnlyValue(tmpSql);
                            if (tmpVal == null || tmpVal == DBNull.Value)
                            {
                                result.ErrorCode = 1;
                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                return 0;
                            }
                            itemValue = Convert.ToInt32(tmpVal);

                            sql += itemValue;
                        }
                    }

                    int detailID = 0;
                    val = db.SelectOnlyValue(sql);
                    if (val == null || val == DBNull.Value)
                    {
                        //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                        int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                        detailID = maxDetailID + 1;
                        List<string> tmpSqlLst = new List<string>();
                        //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                        sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                        foreach (ItemClass item in entry.itemClassLst)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                sql += ",F" + item.ItemClassID;
                            }
                        }

                        sql += ") values(" + detailID + "," + itemsCount;

                        foreach (ItemClass item in entry.itemClassLst)
                        {

                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                int itemValue = 0;
                                //取核算项目值

                                string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                object tmpVal = db.SelectOnlyValue(tmpSql);
                                if (tmpVal == null || tmpVal == DBNull.Value)
                                {
                                    result.ErrorCode = 1;
                                    result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                    return 0;
                                }
                                itemValue = Convert.ToInt32(tmpVal);

                                sql += "," + itemValue;
                            }
                        }

                        sql += ")";

                        tmpSqlLst.Add(sql);
                        //插入 t_itemDetailV;
                        foreach (ItemClass item in entry.itemClassLst)
                        {

                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                int itemValue = 0;
                                //取核算项目值

                                string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                object tmpVal = db.SelectOnlyValue(tmpSql);
                                if (tmpVal == null || tmpVal == DBNull.Value)
                                {
                                    result.ErrorCode = 1;
                                    result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                    return 0;
                                }
                                itemValue = Convert.ToInt32(tmpVal);

                                sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                tmpSqlLst.Add(sql);
                            }
                        }
                        db.LongHaul(tmpSqlLst);
                    }
                    else
                        detailID = Convert.ToInt32(val);
                    //核算项目处理结束
                    sqlF = @"INSERT INTO t_VoucherEntry
                           (FBrNo
                           ,FVoucherID
                           ,FEntryID
                           ,FExplanation
                           ,FAccountID
                           ,FDetailID
                           ,FCurrencyID
                           ,FExchangeRate
                           ,FDC
                           ,FAmountFor
                           ,FAmount
                           ,FQuantity
                           ,FMeasureUnitID
                           ,FUnitPrice
                           ,FInternalInd
                           ,FAccountID2
                           ,FSettleTypeID
                           ,FSettleNo
                           ,FTransNo
                           ,FCashFlowItem
                           ,FTaskID
                           ,FResourceID
                           ,FExchangeRateType
                           ,FSideEntryID)
                     VALUES
                           (0
                           ,{0}
                           ,{1}
                           ,'{2}'
                           ,{3}
                           ,{4}
                           ,{5}
                           ,{6}
                           ,{7}
                           ,{8}
                           ,{9}
                           ,0
                           ,0
                           ,0
                           ,'Industry'
                           ,{10}
                           ,0
                           ,null
                           ,null
                           ,0
                           ,0
                           ,0
                           ,1
                           ,0)";
                    //取amount
                    double amount = 0;
                    string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID=" + entry.AmountFrom).ToString();
                    sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID=" + saleHeader.InterID;
                    amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                    if (entry.Direction == 0)
                        debitTotal += amount;
                    else
                        creditAmount += amount;
                    sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                        saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                    sqllst.Add(sql);
                    vchEntryID++;//凭证分录序号加1
                }
                
            }
            sqlF = @"INSERT INTO t_Voucher
                   (FBrNo
                   ,FVoucherID
                   ,FDate
                   ,FYear
                   ,FPeriod
                   ,FGroupID
                   ,FNumber
                   ,FExplanation
                   ,FAttachments
                   ,FEntryCount
                   ,FDebitTotal
                   ,FCreditTotal
                   ,FInternalInd
                   ,FChecked
                   ,FPosted
                   ,FPreparerID
                   ,FCheckerID
                   ,FPosterID
                   ,FCashierID
                   ,FHandler
                   ,FOwnerGroupID
                   ,FObjectName
                   ,FParameter
                   ,FSerialNum
                   ,FTranType
                   ,FTransDate
                   ,FFrameWorkID)
                    VALUES
                   (0
                   ,{0}
                   ,'{13}'
                   ,{1}
                   ,{2}
                   ,{3}
                   ,{4}
                   ,'{5}'
                   ,0
                   ,{6}
                   ,{7}
                   ,{8}
                   ,'Industry'
                   ,0
                   ,0
                   ,{12}
                   ,-1
                   ,-1
                   ,-1
                   ,null
                   ,0
                   ,null
                   ,null
                   ,{9}
                   ,{10}
                   ,'{11}'
                   ,-1)";

            
            
            sql = string.Format(sqlF, voucherId, year, period, vchTpl.GroupID, vchNumber, note, vchEntryID,
                debitTotal, creditAmount, serialNum, vchTpl.TransType, saleHeader.Dt, userID, voucherDate);
            sqllst.Add(sql);

            //更新发票与票据关系

            sql = "Update ICSale SET  FVchInterID=" + voucherId + "  WHERE  FInterID =" + saleHeader.InterID;
            sqllst.Add(sql);

            sql = "Update t1 SET t1.FVoucherID=" + voucherId + " ,t1.FGroupID=t2.FGroupID,  t1.FStatus=t1.FStatus | 2 FROM t_RP_Contact t1,t_Voucher t2 where t1.FInvoiceID=" + saleHeader.InterID + " and t1.FType=3  and t1.FK3Import=1 and t2.FVoucherID=" + voucherId;
            sqllst.Add(sql);
            sql = "insert into ICVouchBillTranType(FTranType,FVoucherID) values (801,"+ voucherId +")";
            sqllst.Add(sql);
            db.LongHaul(sqllst);
            return vchNumber;
        }

        public int CreateVoucher(string userName, ICSaleDisplay sale, OpResult result)
        {
            if (IsBillVoucherExisted(sale.FBillNo))
            {
                result.ErrorCode = 3;
                result.ErrMessage = "单据" + sale.FBillNo + "已经生成凭证";
                return 0;
            }

            ICSaleHeader saleHeader = saleDao.getICSaleHeader(sale.FBillNo);
            List<ICSaleEntry> saleEntryLst = saleDao.getICSaleEntry(saleHeader.InterID);

            List<string> sqllst = new List<string>();
            string sql, sqlF;
            //寻找默认模板
            int year = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentYear' and FCategory='IC'"));
            int period = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentPeriod' and FCategory='IC'"));
            //   int year = Convert.ToInt32(db.SelectOnlyValue("select FYear from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
            //   int period = Convert.ToInt32(db.SelectOnlyValue("select FPeriod from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
            DateTime voucherDate = Convert.ToDateTime(db.SelectOnlyValue("select FEndDate from T_PeriodDate where FYear = " + year + " and FPeriod = " + period));

            Object val = null;
            VoucherTpl vchTpl = tplDao.getDetailByInterID(sale.FVoucherTplID);

            int userID = Convert.ToInt32(db.SelectOnlyValue("select FUserID  from t_user where FName='" + userName + "'"));

            //0-贷方,1- 借方
            double debitTotal = 0, creditAmount = 0;

            //生成编号VoucherID,SerialNum,FNumber;
            int voucherId = Convert.ToInt32(db.SelectOnlyValue("select MAX(FVoucherID)+1 from t_Voucher"));
            val = db.SelectOnlyValue(@"SELECT MAX(FSerialNum) FROM (
     select * from t_Voucher 
     union all 
     select * from t_VoucherBlankout 
     union all 
     select * from t_VoucherAdjust) v 
 Where FYear=" + year);
            int serialNum = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val);
            ++serialNum;
            sql = "Select max(FNumber)  from (select * from t_Voucher union all select * from t_VoucherBlankout) v where FYear = " + year + " and FPeriod = " + period + " and FGroupID = " + vchTpl.GroupID;
            val = db.SelectOnlyValue(sql);
            int vchNumber = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val) + 1;

            int vchEntryID = 0;

            //Note处理 收入: [销售发票.购货单位][销售发票.合同单号][销售发票.进度提醒]
            string note = "";
            sql = @"select '收入: '+CONVERT(NVARCHAR,t1.FName) + ISNULL(t0.FNote,'') + ISNULL(t0.FHeadSelfI0559,'')+ISNULL(t0.FHeadSelfI0461,'') from ICSale t0
left join t_Item t1 on t0.FCustID = t1.FItemID
where FBillNo  ='" + sale.FBillNo + "'";
            try
            {
                note = db.SelectOnlyValue(sql).ToString();
            }
            catch (Exception e)
            {
                note = "凭证分录摘要获取异常";
            }


            //插入凭证分录
            foreach (VoucherTplEntry entry in vchTpl.EntryList)
            {
                //核算项目是否有发票分录字段
                bool hasEntryItem = false;
                //有没有核算项目
                int itemsCount = 0;
                HashSet<int> acctSet = new HashSet<int>();
                foreach (ItemClass item in entry.itemClassLst)
                {
                    if (item.IsEntry == 1)
                    {
                        hasEntryItem = true;
                    }
                    if (!string.IsNullOrWhiteSpace(item.Field))
                        ++itemsCount;
                }
                //选择对方科目
                sql = "select FAccID from fsICVoucherTplEntry where FInterID=" + entry.InterID + " and FAccID <> " + entry.AccID;
                int acct2 = Convert.ToInt32(db.SelectOnlyValue(sql));


                if (hasEntryItem == true)
                {
                    //插入核算项目,每条数据分录都要插入核算项目
                    foreach (ICSaleEntry saleEntry in saleEntryLst)
                    {
                        //select FDetailID from t_ItemDetail where FDetailCount= 2 and F1=11323 and F2003=232
                        sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                        foreach (ItemClass item in entry.itemClassLst)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                sql += " and F" + item.ItemClassID + " = ";
                                int itemValue = 0;
                                //取核算项目值
                                if (item.IsEntry == 1)
                                {
                                    string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);
                                }
                                else
                                {
                                    string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);
                                }

                                sql += itemValue;
                            }
                        }

                        int detailID = 0;
                        val = db.SelectOnlyValue(sql);
                        if (val == null || val == DBNull.Value)
                        {
                            //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                            int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                            detailID = maxDetailID + 1;
                            List<string> tmpSqlLst = new List<string>();
                            //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                            sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                            foreach (ItemClass item in entry.itemClassLst)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    sql += ",F" + item.ItemClassID;
                                }
                            }

                            sql += ") values(" + detailID + "," + itemsCount;

                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值
                                    if (item.IsEntry == 1)
                                    {
                                        string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    else
                                    {
                                        string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    sql += "," + itemValue;
                                }
                            }

                            sql += ")";

                            tmpSqlLst.Add(sql);
                            //插入 t_itemDetailV;
                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值
                                    if (item.IsEntry == 1)
                                    {
                                        string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    else
                                    {
                                        string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                    tmpSqlLst.Add(sql);
                                }
                            }
                            db.LongHaul(tmpSqlLst);
                        }
                        else
                            detailID = Convert.ToInt32(val);

                        sqlF = @"INSERT INTO t_VoucherEntry
                           (FBrNo
                           ,FVoucherID
                           ,FEntryID
                           ,FExplanation
                           ,FAccountID
                           ,FDetailID
                           ,FCurrencyID
                           ,FExchangeRate
                           ,FDC
                           ,FAmountFor
                           ,FAmount
                           ,FQuantity
                           ,FMeasureUnitID
                           ,FUnitPrice
                           ,FInternalInd
                           ,FAccountID2
                           ,FSettleTypeID
                           ,FSettleNo
                           ,FTransNo
                           ,FCashFlowItem
                           ,FTaskID
                           ,FResourceID
                           ,FExchangeRateType
                           ,FSideEntryID)
                     VALUES
                           (0
                           ,{0}
                           ,{1}
                           ,'{2}'
                           ,{3}
                           ,{4}
                           ,{5}
                           ,{6}
                           ,{7}
                           ,{8}
                           ,{9}
                           ,0
                           ,0
                           ,0
                           ,'Industry'
                           ,{10}
                           ,0
                           ,null
                           ,null
                           ,0
                           ,0
                           ,0
                           ,1
                           ,0)";

                        //取amount
                        double amount = 0;
                        string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID=" + entry.AmountFrom).ToString();
                        sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID=" + saleHeader.InterID + " and t2.FEntryID=" + saleEntry.EntryID;
                        amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                        if (entry.Direction == 0)
                            debitTotal += amount;
                        else
                            creditAmount += amount;
                        sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                            saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                        sqllst.Add(sql);
                        vchEntryID++;//凭证分录序号加1
                    }
                }
                else
                {
                    //如果核算项目没有分录选项
                    sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                    foreach (ItemClass item in entry.itemClassLst)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Field))
                        {
                            sql += " and F" + item.ItemClassID + " = ";
                            int itemValue = 0;

                            string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                            object tmpVal = db.SelectOnlyValue(tmpSql);
                            if (tmpVal == null || tmpVal == DBNull.Value)
                            {
                                result.ErrorCode = 1;
                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                return 0;
                            }
                            itemValue = Convert.ToInt32(tmpVal);

                            sql += itemValue;
                        }
                    }

                    int detailID = 0;
                    val = db.SelectOnlyValue(sql);
                    if (val == null || val == DBNull.Value)
                    {
                        //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                        int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                        detailID = maxDetailID + 1;
                        List<string> tmpSqlLst = new List<string>();
                        //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                        sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                        foreach (ItemClass item in entry.itemClassLst)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                sql += ",F" + item.ItemClassID;
                            }
                        }

                        sql += ") values(" + detailID + "," + itemsCount;

                        foreach (ItemClass item in entry.itemClassLst)
                        {

                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                int itemValue = 0;
                                //取核算项目值

                                string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                object tmpVal = db.SelectOnlyValue(tmpSql);
                                if (tmpVal == null || tmpVal == DBNull.Value)
                                {
                                    result.ErrorCode = 1;
                                    result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                    return 0;
                                }
                                itemValue = Convert.ToInt32(tmpVal);

                                sql += "," + itemValue;
                            }
                        }

                        sql += ")";

                        tmpSqlLst.Add(sql);
                        //插入 t_itemDetailV;
                        foreach (ItemClass item in entry.itemClassLst)
                        {

                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                int itemValue = 0;
                                //取核算项目值

                                string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                object tmpVal = db.SelectOnlyValue(tmpSql);
                                if (tmpVal == null || tmpVal == DBNull.Value)
                                {
                                    result.ErrorCode = 1;
                                    result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                    return 0;
                                }
                                itemValue = Convert.ToInt32(tmpVal);

                                sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                tmpSqlLst.Add(sql);
                            }
                        }
                        db.LongHaul(tmpSqlLst);
                    }
                    else
                        detailID = Convert.ToInt32(val);
                    //核算项目处理结束
                    sqlF = @"INSERT INTO t_VoucherEntry
                           (FBrNo
                           ,FVoucherID
                           ,FEntryID
                           ,FExplanation
                           ,FAccountID
                           ,FDetailID
                           ,FCurrencyID
                           ,FExchangeRate
                           ,FDC
                           ,FAmountFor
                           ,FAmount
                           ,FQuantity
                           ,FMeasureUnitID
                           ,FUnitPrice
                           ,FInternalInd
                           ,FAccountID2
                           ,FSettleTypeID
                           ,FSettleNo
                           ,FTransNo
                           ,FCashFlowItem
                           ,FTaskID
                           ,FResourceID
                           ,FExchangeRateType
                           ,FSideEntryID)
                     VALUES
                           (0
                           ,{0}
                           ,{1}
                           ,'{2}'
                           ,{3}
                           ,{4}
                           ,{5}
                           ,{6}
                           ,{7}
                           ,{8}
                           ,{9}
                           ,0
                           ,0
                           ,0
                           ,'Industry'
                           ,{10}
                           ,0
                           ,null
                           ,null
                           ,0
                           ,0
                           ,0
                           ,1
                           ,0)";
                    //取amount
                    double amount = 0;
                    string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID=" + entry.AmountFrom).ToString();
                    sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID=" + saleHeader.InterID;
                    amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                    if (entry.Direction == 0)
                        debitTotal += amount;
                    else
                        creditAmount += amount;
                    sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                        saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                    sqllst.Add(sql);
                    vchEntryID++;//凭证分录序号加1
                }

            }
            sqlF = @"INSERT INTO t_Voucher
                   (FBrNo
                   ,FVoucherID
                   ,FDate
                   ,FYear
                   ,FPeriod
                   ,FGroupID
                   ,FNumber
                   ,FExplanation
                   ,FAttachments
                   ,FEntryCount
                   ,FDebitTotal
                   ,FCreditTotal
                   ,FInternalInd
                   ,FChecked
                   ,FPosted
                   ,FPreparerID
                   ,FCheckerID
                   ,FPosterID
                   ,FCashierID
                   ,FHandler
                   ,FOwnerGroupID
                   ,FObjectName
                   ,FParameter
                   ,FSerialNum
                   ,FTranType
                   ,FTransDate
                   ,FFrameWorkID)
                    VALUES
                   (0
                   ,{0}
                   ,'{13}'
                   ,{1}
                   ,{2}
                   ,{3}
                   ,{4}
                   ,'{5}'
                   ,0
                   ,{6}
                   ,{7}
                   ,{8}
                   ,'Industry'
                   ,0
                   ,0
                   ,{12}
                   ,-1
                   ,-1
                   ,-1
                   ,null
                   ,0
                   ,null
                   ,null
                   ,{9}
                   ,{10}
                   ,'{11}'
                   ,-1)";



            sql = string.Format(sqlF, voucherId, year, period, vchTpl.GroupID, vchNumber, note, vchEntryID,
                debitTotal, creditAmount, serialNum, vchTpl.TransType, saleHeader.Dt, userID, voucherDate);
            sqllst.Add(sql);

            //更新发票与票据关系

            sql = "Update ICSale SET  FVchInterID=" + voucherId + "  WHERE  FInterID =" + saleHeader.InterID;
            sqllst.Add(sql);

            sql = "Update t1 SET t1.FVoucherID=" + voucherId + " ,t1.FGroupID=t2.FGroupID,  t1.FStatus=t1.FStatus | 2 FROM t_RP_Contact t1,t_Voucher t2 where t1.FInvoiceID=" + saleHeader.InterID + " and t1.FType=3  and t1.FK3Import=1 and t2.FVoucherID=" + voucherId;
            sqllst.Add(sql);
            sql = "insert into ICVouchBillTranType(FTranType,FVoucherID) values (801," + voucherId + ")";
            sqllst.Add(sql);
            db.LongHaul(sqllst);
            return vchNumber;
        }
        public bool IsBillVoucherExisted(string billNo)
        {
            string sql = "select FVchInterID from icsale where fbillno='" + billNo + "'";
            object val = db.SelectOnlyValue(sql);
            if (val == null || val == DBNull.Value || Convert.ToInt32(val) == 0)
                return false;
            else
                return true;
        }

        public int CreateVoucherCombine(string userName, List<ICSaleDisplay> saleList, OpResult result)
        {
            //检验单据是否已经生成凭证,检验单据类型是否一致
            //检验单据类型是否一致
            String commonTypeName = "";
            foreach (ICSaleDisplay sale in saleList)
            {
                if (IsBillVoucherExisted(sale.FBillNo))
                {
                    result.ErrorCode = 3;
                    result.ErrMessage = "单据" + sale.FBillNo + "已经生成凭证";
                    return 0;
                }
                String tranTypeName = Convert.ToString(db.SelectOnlyValue("select t2.FName from fsICVoucherTpl t1 left join ICTransType t2  on t1.FTplType=t2.FID where t1.FInterID ='" + sale.FVoucherTplID + "'"));
                if (!sale.FTransTypeIDName.Equals(tranTypeName))
                {
                    result.ErrorCode = 4;
                    result.ErrMessage = "单据" + sale.FBillNo + "所选凭证模板不适用";
                    return 0;
                }
                //验证所选单据列表是否为同一种类型单据
                if ("".Equals(commonTypeName))
                    commonTypeName = sale.FTransTypeIDName;
                else if (!commonTypeName.Equals(sale.FTransTypeIDName))
                {
                    result.ErrorCode = 5;
                    result.ErrMessage = "单据" + sale.FBillNo + "与其它的发票类型不一致，请选择同一类型的单据合并生成";
                    return 0;
                }
            }

            List<string> sqllst = new List<string>();
            string sql, sqlF;

            //寻找默认模板
            int year = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentYear' and FCategory='IC'"));
            int period = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentPeriod' and FCategory='IC'"));
            //   int year = Convert.ToInt32(db.SelectOnlyValue("select FYear from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
            //   int period = Convert.ToInt32(db.SelectOnlyValue("select FPeriod from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
            DateTime voucherDate = Convert.ToDateTime(db.SelectOnlyValue("select FEndDate from T_PeriodDate where FYear = " + year + " and FPeriod = " + period));
            object val = null;
            //0-贷方,1- 借方
            double debitTotal = 0, creditAmount = 0;

            //生成编号VoucherID,SerialNum,FNumber;
            int voucherId = Convert.ToInt32(db.SelectOnlyValue("select MAX(FVoucherID)+1 from t_Voucher"));
            val = db.SelectOnlyValue(@"SELECT MAX(FSerialNum) FROM (
         select * from t_Voucher 
         union all 
         select * from t_VoucherBlankout 
         union all 
         select * from t_VoucherAdjust) v 
     Where FYear=" + year);
            int serialNum = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val);
            ++serialNum;
            VoucherTpl firstVchTpl = tplDao.getDetailByInterID(saleList[0].FVoucherTplID);
            sql = "Select max(FNumber)  from (select * from t_Voucher union all select * from t_VoucherBlankout) v where FYear = " + year + " and FPeriod = " + period + " and FGroupID = " + firstVchTpl.GroupID;
            val = db.SelectOnlyValue(sql);
            int vchNumber = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val) + 1;

            int vchEntryID = 0;
            string noteHeader = "";

            //获取用户ID
            int userID = Convert.ToInt32(db.SelectOnlyValue("select FUserID  from t_user where FName='" + userName + "'"));

            foreach(ICSaleDisplay sale in saleList)
            {
                ICSaleHeader saleHeader = saleDao.getICSaleHeader(sale.FBillNo);
                List<ICSaleEntry> saleEntryLst = saleDao.getICSaleEntry(saleHeader.InterID);

                VoucherTpl vchTpl = tplDao.getDetailByInterID(sale.FVoucherTplID);

                //Note处理 收入: [销售发票.购货单位][销售发票.合同单号][销售发票.进度提醒]
                string note = "";
                sql = @"select '收入: '+CONVERT(NVARCHAR,t1.FName) + ISNULL(t0.FNote,'') + ISNULL(t0.FHeadSelfI0559,'')+ISNULL(t0.FHeadSelfI0461,'') from ICSale t0
    left join t_Item t1 on t0.FCustID = t1.FItemID
    where FBillNo  ='" + sale.FBillNo + "'";
                try
                {
                    note = db.SelectOnlyValue(sql).ToString();
                    if ("".Equals(noteHeader))
                        noteHeader = note;
                }
                catch (Exception e)
                {
                    note = "凭证分录摘要获取异常";
                }


                //插入凭证分录
                foreach (VoucherTplEntry entry in vchTpl.EntryList)
                {
                    //核算项目是否有发票分录字段
                    bool hasEntryItem = false;
                    //有没有核算项目
                    int itemsCount = 0;
                    HashSet<int> acctSet = new HashSet<int>();
                    foreach (ItemClass item in entry.itemClassLst)
                    {
                        if (item.IsEntry == 1)
                        {
                            hasEntryItem = true;
                        }
                        if (!string.IsNullOrWhiteSpace(item.Field))
                            ++itemsCount;
                    }
                    //选择对方科目
                    sql = "select FAccID from fsICVoucherTplEntry where FInterID=" + entry.InterID + " and FAccID <> " + entry.AccID;
                    int acct2 = Convert.ToInt32(db.SelectOnlyValue(sql));


                    if (hasEntryItem == true)
                    {
                        //插入核算项目,每条数据分录都要插入核算项目
                        foreach (ICSaleEntry saleEntry in saleEntryLst)
                        {
                            //select FDetailID from t_ItemDetail where FDetailCount= 2 and F1=11323 and F2003=232
                            sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                            foreach (ItemClass item in entry.itemClassLst)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    sql += " and F" + item.ItemClassID + " = ";
                                    int itemValue = 0;
                                    //取核算项目值
                                    if (item.IsEntry == 1)
                                    {
                                        string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    else
                                    {
                                        string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }

                                    sql += itemValue;
                                }
                            }

                            int detailID = 0;
                            val = db.SelectOnlyValue(sql);
                            if (val == null || val == DBNull.Value)
                            {
                                //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                                int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                                detailID = maxDetailID + 1;
                                List<string> tmpSqlLst = new List<string>();
                                //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                                sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                                foreach (ItemClass item in entry.itemClassLst)
                                {
                                    if (!string.IsNullOrWhiteSpace(item.Field))
                                    {
                                        sql += ",F" + item.ItemClassID;
                                    }
                                }

                                sql += ") values(" + detailID + "," + itemsCount;

                                foreach (ItemClass item in entry.itemClassLst)
                                {

                                    if (!string.IsNullOrWhiteSpace(item.Field))
                                    {
                                        int itemValue = 0;
                                        //取核算项目值
                                        if (item.IsEntry == 1)
                                        {
                                            string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        else
                                        {
                                            string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        sql += "," + itemValue;
                                    }
                                }

                                sql += ")";

                                tmpSqlLst.Add(sql);
                                //插入 t_itemDetailV;
                                foreach (ItemClass item in entry.itemClassLst)
                                {

                                    if (!string.IsNullOrWhiteSpace(item.Field))
                                    {
                                        int itemValue = 0;
                                        //取核算项目值
                                        if (item.IsEntry == 1)
                                        {
                                            string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        else
                                        {
                                            string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                        tmpSqlLst.Add(sql);
                                    }
                                }
                                db.LongHaul(tmpSqlLst);
                            }
                            else
                                detailID = Convert.ToInt32(val);

                            sqlF = @"INSERT INTO t_VoucherEntry
                               (FBrNo
                               ,FVoucherID
                               ,FEntryID
                               ,FExplanation
                               ,FAccountID
                               ,FDetailID
                               ,FCurrencyID
                               ,FExchangeRate
                               ,FDC
                               ,FAmountFor
                               ,FAmount
                               ,FQuantity
                               ,FMeasureUnitID
                               ,FUnitPrice
                               ,FInternalInd
                               ,FAccountID2
                               ,FSettleTypeID
                               ,FSettleNo
                               ,FTransNo
                               ,FCashFlowItem
                               ,FTaskID
                               ,FResourceID
                               ,FExchangeRateType
                               ,FSideEntryID)
                         VALUES
                               (0
                               ,{0}
                               ,{1}
                               ,'{2}'
                               ,{3}
                               ,{4}
                               ,{5}
                               ,{6}
                               ,{7}
                               ,{8}
                               ,{9}
                               ,0
                               ,0
                               ,0
                               ,'Industry'
                               ,{10}
                               ,0
                               ,null
                               ,null
                               ,0
                               ,0
                               ,0
                               ,1
                               ,0)";

                            //取amount
                            double amount = 0;
                            string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID=" + entry.AmountFrom).ToString();
                            sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID=" + saleHeader.InterID + " and t2.FEntryID=" + saleEntry.EntryID;
                            amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                            if (entry.Direction == 0)
                                debitTotal += amount;
                            else
                                creditAmount += amount;
                            sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                                saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                            sqllst.Add(sql);
                            vchEntryID++;//凭证分录序号加1
                        }
                    }
                    else
                    {
                        //如果核算项目没有分录选项
                        sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                        foreach (ItemClass item in entry.itemClassLst)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                sql += " and F" + item.ItemClassID + " = ";
                                int itemValue = 0;

                                string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                object tmpVal = db.SelectOnlyValue(tmpSql);
                                if (tmpVal == null || tmpVal == DBNull.Value)
                                {
                                    result.ErrorCode = 1;
                                    result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                    return 0;
                                }
                                itemValue = Convert.ToInt32(tmpVal);

                                sql += itemValue;
                            }
                        }

                        int detailID = 0;
                        val = db.SelectOnlyValue(sql);
                        if (val == null || val == DBNull.Value)
                        {
                            //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                            int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                            detailID = maxDetailID + 1;
                            List<string> tmpSqlLst = new List<string>();
                            //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                            sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                            foreach (ItemClass item in entry.itemClassLst)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    sql += ",F" + item.ItemClassID;
                                }
                            }

                            sql += ") values(" + detailID + "," + itemsCount;

                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值

                                    string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);

                                    sql += "," + itemValue;
                                }
                            }

                            sql += ")";

                            tmpSqlLst.Add(sql);
                            //插入 t_itemDetailV;
                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值

                                    string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);

                                    sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                    tmpSqlLst.Add(sql);
                                }
                            }
                            db.LongHaul(tmpSqlLst);
                        }
                        else
                            detailID = Convert.ToInt32(val);
                        //核算项目处理结束
                        sqlF = @"INSERT INTO t_VoucherEntry
                               (FBrNo
                               ,FVoucherID
                               ,FEntryID
                               ,FExplanation
                               ,FAccountID
                               ,FDetailID
                               ,FCurrencyID
                               ,FExchangeRate
                               ,FDC
                               ,FAmountFor
                               ,FAmount
                               ,FQuantity
                               ,FMeasureUnitID
                               ,FUnitPrice
                               ,FInternalInd
                               ,FAccountID2
                               ,FSettleTypeID
                               ,FSettleNo
                               ,FTransNo
                               ,FCashFlowItem
                               ,FTaskID
                               ,FResourceID
                               ,FExchangeRateType
                               ,FSideEntryID)
                         VALUES
                               (0
                               ,{0}
                               ,{1}
                               ,'{2}'
                               ,{3}
                               ,{4}
                               ,{5}
                               ,{6}
                               ,{7}
                               ,{8}
                               ,{9}
                               ,0
                               ,0
                               ,0
                               ,'Industry'
                               ,{10}
                               ,0
                               ,null
                               ,null
                               ,0
                               ,0
                               ,0
                               ,1
                               ,0)";
                        //取amount
                        double amount = 0;
                        string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID=" + entry.AmountFrom).ToString();
                        sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID=" + saleHeader.InterID;
                        amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                        if (entry.Direction == 0)
                            debitTotal += amount;
                        else
                            creditAmount += amount;
                        sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                            saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                        sqllst.Add(sql);
                        vchEntryID++;//凭证分录序号加1
                    }

                }
                

                //更新发票与票据关系

                sql = "Update ICSale SET  FVchInterID=" + voucherId + "  WHERE  FInterID =" + saleHeader.InterID;
                sqllst.Add(sql);

                sql = "Update t1 SET t1.FVoucherID=" + voucherId + " ,t1.FGroupID=t2.FGroupID,  t1.FStatus=t1.FStatus | 2 FROM t_RP_Contact t1,t_Voucher t2 where t1.FInvoiceID=" + saleHeader.InterID + " and t1.FType=3  and t1.FK3Import=1 and t2.FVoucherID=" + voucherId;
                sqllst.Add(sql);
                
            }
            sqlF = @"INSERT INTO t_Voucher
                       (FBrNo
                       ,FVoucherID
                       ,FDate
                       ,FYear
                       ,FPeriod
                       ,FGroupID
                       ,FNumber
                       ,FExplanation
                       ,FAttachments
                       ,FEntryCount
                       ,FDebitTotal
                       ,FCreditTotal
                       ,FInternalInd
                       ,FChecked
                       ,FPosted
                       ,FPreparerID
                       ,FCheckerID
                       ,FPosterID
                       ,FCashierID
                       ,FHandler
                       ,FOwnerGroupID
                       ,FObjectName
                       ,FParameter
                       ,FSerialNum
                       ,FTranType
                       ,FTransDate
                       ,FFrameWorkID)
                        VALUES
                       (0
                       ,{0}
                       ,'{13}'
                       ,{1}
                       ,{2}
                       ,{3}
                       ,{4}
                       ,'{5}'
                       ,0
                       ,{6}
                       ,{7}
                       ,{8}
                       ,'Industry'
                       ,0
                       ,0
                       ,{12}
                       ,-1
                       ,-1
                       ,-1
                       ,null
                       ,0
                       ,null
                       ,null
                       ,{9}
                       ,{10}
                       ,'{11}'
                       ,-1)";



            sql = string.Format(sqlF, voucherId, year, period, firstVchTpl.GroupID, vchNumber, noteHeader, vchEntryID,
                debitTotal, creditAmount, serialNum, firstVchTpl.TransType, null, userID, voucherDate);
            sqllst.Add(sql);

            sql = "insert into ICVouchBillTranType(FTranType,FVoucherID) values (801," + voucherId + ")";
            sqllst.Add(sql);
            db.LongHaul(sqllst);
            return vchNumber;
        }


        public int CreateVoucherCombine(string userName, List<string> billNoLst, OpResult result)
        {
            //检验单据是否已经生成凭证,检验单据类型是否一致
            //检验单据类型是否一致
            int billListTranType = 0;
            foreach (string billNo in billNoLst)
            {
                if (IsBillVoucherExisted(billNo))
                {
                    result.ErrorCode = 3;
                    result.ErrMessage = "单据" + billNo + "已经生成凭证";
                    return 0;
                }
                int tranType = Convert.ToInt32(db.SelectOnlyValue("SELECT FTranType FROM ICSale where FBillNo ='" + billNo + "'"));
                if (billListTranType == 0)
                    billListTranType = tranType;
                else if (billListTranType != tranType)
                {
                    result.ErrorCode = 4;
                    result.ErrMessage = "单据" + billNo + "与其它单据的类型不一致";
                    return 0;
                }
            }

            List<string> sqllst = new List<string>();
            string sql, sqlF;

            //寻找默认模板
            int year = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentYear' and FCategory='IC'"));
            int period = Convert.ToInt32(db.SelectOnlyValue("SELECT FValue FROM t_SystemProfile where FKey ='CurrentPeriod' and FCategory='IC'"));
            //   int year = Convert.ToInt32(db.SelectOnlyValue("select FYear from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
            //   int period = Convert.ToInt32(db.SelectOnlyValue("select FPeriod from T_PeriodDate where FStartDate <= '" + saleHeader.Dt + "' and FEndDate >= '" + saleHeader.Dt + "'"));
            DateTime voucherDate = Convert.ToDateTime(db.SelectOnlyValue("select FEndDate from T_PeriodDate where FYear = " + year + " and FPeriod = " + period));
            sql = "select FBillNo from fsICVoucherTpl where FTplType=" + billListTranType + " and FIsDefault = 1";
            object val = db.SelectOnlyValue(sql);
            //如果没有一个默认模板，则返回
            if (val == null || val == DBNull.Value)
            {
                result.ErrorCode = 2;
                result.ErrMessage = "单据类型没有设置默认模板!";
                return 0;
            }
            VoucherTpl vchTpl = tplDao.getDetailByBillNo(val.ToString());


            //0-贷方,1- 借方
            double debitTotal = 0, creditAmount = 0;

            //生成编号VoucherID,SerialNum,FNumber;
            int voucherId = Convert.ToInt32(db.SelectOnlyValue("select MAX(FVoucherID)+1 from t_Voucher"));
            val = db.SelectOnlyValue(@"SELECT MAX(FSerialNum) FROM (
         select * from t_Voucher 
         union all 
         select * from t_VoucherBlankout 
         union all 
         select * from t_VoucherAdjust) v 
     Where FYear=" + year);
            int serialNum = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val);
            ++serialNum;
            sql = "Select max(FNumber)  from (select * from t_Voucher union all select * from t_VoucherBlankout) v where FYear = " + year + " and FPeriod = " + period + " and FGroupID = " + vchTpl.GroupID;
            val = db.SelectOnlyValue(sql);
            int vchNumber = val == null || val == DBNull.Value ? 1 : Convert.ToInt32(val) + 1;

            int vchEntryID = 0;
            string noteHeader = "";

            //获取用户ID
            int userID = Convert.ToInt32(db.SelectOnlyValue("select FUserID  from t_user where FName='" + userName + "'"));

            foreach (string billNo in billNoLst)
            {
                ICSaleHeader saleHeader = saleDao.getICSaleHeader(billNo);
                List<ICSaleEntry> saleEntryLst = saleDao.getICSaleEntry(saleHeader.InterID);



                //Note处理 收入: [销售发票.购货单位][销售发票.合同单号][销售发票.进度提醒]
                string note = "";
                sql = @"select '收入: '+CONVERT(NVARCHAR,t1.FName) + ISNULL(t0.FNote,'') + ISNULL(t0.FHeadSelfI0559,'')+ISNULL(t0.FHeadSelfI0461,'') from ICSale t0
    left join t_Item t1 on t0.FCustID = t1.FItemID
    where FBillNo  ='" + billNo + "'";
                try
                {
                    note = db.SelectOnlyValue(sql).ToString();
                    if ("".Equals(noteHeader))
                        noteHeader = note;
                }
                catch (Exception e)
                {
                    note = "凭证分录摘要获取异常";
                }


                //插入凭证分录
                foreach (VoucherTplEntry entry in vchTpl.EntryList)
                {
                    //核算项目是否有发票分录字段
                    bool hasEntryItem = false;
                    //有没有核算项目
                    int itemsCount = 0;
                    HashSet<int> acctSet = new HashSet<int>();
                    foreach (ItemClass item in entry.itemClassLst)
                    {
                        if (item.IsEntry == 1)
                        {
                            hasEntryItem = true;
                        }
                        if (!string.IsNullOrWhiteSpace(item.Field))
                            ++itemsCount;
                    }
                    //选择对方科目
                    sql = "select FAccID from fsICVoucherTplEntry where FInterID=" + entry.InterID + " and FAccID <> " + entry.AccID;
                    int acct2 = Convert.ToInt32(db.SelectOnlyValue(sql));


                    if (hasEntryItem == true)
                    {
                        //插入核算项目,每条数据分录都要插入核算项目
                        foreach (ICSaleEntry saleEntry in saleEntryLst)
                        {
                            //select FDetailID from t_ItemDetail where FDetailCount= 2 and F1=11323 and F2003=232
                            sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                            foreach (ItemClass item in entry.itemClassLst)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    sql += " and F" + item.ItemClassID + " = ";
                                    int itemValue = 0;
                                    //取核算项目值
                                    if (item.IsEntry == 1)
                                    {
                                        string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }
                                    else
                                    {
                                        string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                        object tmpVal = db.SelectOnlyValue(tmpSql);
                                        if (tmpVal == null || tmpVal == DBNull.Value)
                                        {
                                            result.ErrorCode = 1;
                                            result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                            return 0;
                                        }
                                        itemValue = Convert.ToInt32(tmpVal);
                                    }

                                    sql += itemValue;
                                }
                            }

                            int detailID = 0;
                            val = db.SelectOnlyValue(sql);
                            if (val == null || val == DBNull.Value)
                            {
                                //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                                int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                                detailID = maxDetailID + 1;
                                List<string> tmpSqlLst = new List<string>();
                                //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                                sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                                foreach (ItemClass item in entry.itemClassLst)
                                {
                                    if (!string.IsNullOrWhiteSpace(item.Field))
                                    {
                                        sql += ",F" + item.ItemClassID;
                                    }
                                }

                                sql += ") values(" + detailID + "," + itemsCount;

                                foreach (ItemClass item in entry.itemClassLst)
                                {

                                    if (!string.IsNullOrWhiteSpace(item.Field))
                                    {
                                        int itemValue = 0;
                                        //取核算项目值
                                        if (item.IsEntry == 1)
                                        {
                                            string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        else
                                        {
                                            string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        sql += "," + itemValue;
                                    }
                                }

                                sql += ")";

                                tmpSqlLst.Add(sql);
                                //插入 t_itemDetailV;
                                foreach (ItemClass item in entry.itemClassLst)
                                {

                                    if (!string.IsNullOrWhiteSpace(item.Field))
                                    {
                                        int itemValue = 0;
                                        //取核算项目值
                                        if (item.IsEntry == 1)
                                        {
                                            string tmpSql = "select " + item.Field + " from icsaleentry where FInterID=" + saleEntry.InterID + " and FEntryID=" + saleEntry.EntryID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        else
                                        {
                                            string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleEntry.InterID;
                                            object tmpVal = db.SelectOnlyValue(tmpSql);
                                            if (tmpVal == null || tmpVal == DBNull.Value)
                                            {
                                                result.ErrorCode = 1;
                                                result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                                return 0;
                                            }
                                            itemValue = Convert.ToInt32(tmpVal);
                                        }
                                        sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                        tmpSqlLst.Add(sql);
                                    }
                                }
                                db.LongHaul(tmpSqlLst);
                            }
                            else
                                detailID = Convert.ToInt32(val);

                            sqlF = @"INSERT INTO t_VoucherEntry
                               (FBrNo
                               ,FVoucherID
                               ,FEntryID
                               ,FExplanation
                               ,FAccountID
                               ,FDetailID
                               ,FCurrencyID
                               ,FExchangeRate
                               ,FDC
                               ,FAmountFor
                               ,FAmount
                               ,FQuantity
                               ,FMeasureUnitID
                               ,FUnitPrice
                               ,FInternalInd
                               ,FAccountID2
                               ,FSettleTypeID
                               ,FSettleNo
                               ,FTransNo
                               ,FCashFlowItem
                               ,FTaskID
                               ,FResourceID
                               ,FExchangeRateType
                               ,FSideEntryID)
                         VALUES
                               (0
                               ,{0}
                               ,{1}
                               ,'{2}'
                               ,{3}
                               ,{4}
                               ,{5}
                               ,{6}
                               ,{7}
                               ,{8}
                               ,{9}
                               ,0
                               ,0
                               ,0
                               ,'Industry'
                               ,{10}
                               ,0
                               ,null
                               ,null
                               ,0
                               ,0
                               ,0
                               ,1
                               ,0)";

                            //取amount
                            double amount = 0;
                            string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID=" + entry.AmountFrom).ToString();
                            sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID=" + saleHeader.InterID + " and t2.FEntryID=" + saleEntry.EntryID;
                            amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                            if (entry.Direction == 0)
                                debitTotal += amount;
                            else
                                creditAmount += amount;
                            sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                                saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                            sqllst.Add(sql);
                            vchEntryID++;//凭证分录序号加1
                        }
                    }
                    else
                    {
                        //如果核算项目没有分录选项
                        sql = "select FDetailID from t_ItemDetail where FDetailCount=" + itemsCount;

                        foreach (ItemClass item in entry.itemClassLst)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Field))
                            {
                                sql += " and F" + item.ItemClassID + " = ";
                                int itemValue = 0;

                                string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                object tmpVal = db.SelectOnlyValue(tmpSql);
                                if (tmpVal == null || tmpVal == DBNull.Value)
                                {
                                    result.ErrorCode = 1;
                                    result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                    return 0;
                                }
                                itemValue = Convert.ToInt32(tmpVal);

                                sql += itemValue;
                            }
                        }

                        int detailID = 0;
                        val = db.SelectOnlyValue(sql);
                        if (val == null || val == DBNull.Value)
                        {
                            //如果数据组合不存在,就要插入t_itemdetail,t_itemdetailv数据组合
                            int maxDetailID = Convert.ToInt32(db.SelectOnlyValue("select max(FDetailID) from t_itemdetail"));
                            detailID = maxDetailID + 1;
                            List<string> tmpSqlLst = new List<string>();
                            //sql = "insert t_ItemDetail(FDetailID, FDetailCount,F1) values(127,1,11312)"
                            sql = "insert t_ItemDetail(FDetailID, FDetailCount";

                            foreach (ItemClass item in entry.itemClassLst)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    sql += ",F" + item.ItemClassID;
                                }
                            }

                            sql += ") values(" + detailID + "," + itemsCount;

                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值

                                    string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);

                                    sql += "," + itemValue;
                                }
                            }

                            sql += ")";

                            tmpSqlLst.Add(sql);
                            //插入 t_itemDetailV;
                            foreach (ItemClass item in entry.itemClassLst)
                            {

                                if (!string.IsNullOrWhiteSpace(item.Field))
                                {
                                    int itemValue = 0;
                                    //取核算项目值

                                    string tmpSql = "select " + item.Field + " from icsale where FInterID=" + saleHeader.InterID;
                                    object tmpVal = db.SelectOnlyValue(tmpSql);
                                    if (tmpVal == null || tmpVal == DBNull.Value)
                                    {
                                        result.ErrorCode = 1;
                                        result.ErrMessage = "发票上核算项目字段未正确填写,无法生成凭证!";
                                        return 0;
                                    }
                                    itemValue = Convert.ToInt32(tmpVal);

                                    sql = "insert into t_itemDetailV values(" + detailID + "," + item.ItemClassID + "," + itemValue + ")";
                                    tmpSqlLst.Add(sql);
                                }
                            }
                            db.LongHaul(tmpSqlLst);
                        }
                        else
                            detailID = Convert.ToInt32(val);
                        //核算项目处理结束
                        sqlF = @"INSERT INTO t_VoucherEntry
                               (FBrNo
                               ,FVoucherID
                               ,FEntryID
                               ,FExplanation
                               ,FAccountID
                               ,FDetailID
                               ,FCurrencyID
                               ,FExchangeRate
                               ,FDC
                               ,FAmountFor
                               ,FAmount
                               ,FQuantity
                               ,FMeasureUnitID
                               ,FUnitPrice
                               ,FInternalInd
                               ,FAccountID2
                               ,FSettleTypeID
                               ,FSettleNo
                               ,FTransNo
                               ,FCashFlowItem
                               ,FTaskID
                               ,FResourceID
                               ,FExchangeRateType
                               ,FSideEntryID)
                         VALUES
                               (0
                               ,{0}
                               ,{1}
                               ,'{2}'
                               ,{3}
                               ,{4}
                               ,{5}
                               ,{6}
                               ,{7}
                               ,{8}
                               ,{9}
                               ,0
                               ,0
                               ,0
                               ,'Industry'
                               ,{10}
                               ,0
                               ,null
                               ,null
                               ,0
                               ,0
                               ,0
                               ,1
                               ,0)";
                        //取amount
                        double amount = 0;
                        string amountStr = db.SelectOnlyValue("select FField from ICVoucherTField where FTplType = 801 and FInterID=" + entry.AmountFrom).ToString();
                        sql = "select " + amountStr + " from ICSale t1 left join ICSaleEntry t2 on t1.FInterID=t2.FInterID where t2.FInterID=" + saleHeader.InterID;
                        amount = Convert.ToDouble(db.SelectOnlyValue(sql));
                        if (entry.Direction == 0)
                            debitTotal += amount;
                        else
                            creditAmount += amount;
                        sql = string.Format(sqlF, voucherId, vchEntryID, note, entry.AccID, detailID, saleHeader.CurrencyID,
                            saleHeader.ExchangeRate, entry.Direction, amount, amount, acct2);
                        sqllst.Add(sql);
                        vchEntryID++;//凭证分录序号加1
                    }

                }


                //更新发票与票据关系

                sql = "Update ICSale SET  FVchInterID=" + voucherId + "  WHERE  FInterID =" + saleHeader.InterID;
                sqllst.Add(sql);

                sql = "Update t1 SET t1.FVoucherID=" + voucherId + " ,t1.FGroupID=t2.FGroupID,  t1.FStatus=t1.FStatus | 2 FROM t_RP_Contact t1,t_Voucher t2 where t1.FInvoiceID=" + saleHeader.InterID + " and t1.FType=3  and t1.FK3Import=1 and t2.FVoucherID=" + voucherId;
                sqllst.Add(sql);

            }
            sqlF = @"INSERT INTO t_Voucher
                       (FBrNo
                       ,FVoucherID
                       ,FDate
                       ,FYear
                       ,FPeriod
                       ,FGroupID
                       ,FNumber
                       ,FExplanation
                       ,FAttachments
                       ,FEntryCount
                       ,FDebitTotal
                       ,FCreditTotal
                       ,FInternalInd
                       ,FChecked
                       ,FPosted
                       ,FPreparerID
                       ,FCheckerID
                       ,FPosterID
                       ,FCashierID
                       ,FHandler
                       ,FOwnerGroupID
                       ,FObjectName
                       ,FParameter
                       ,FSerialNum
                       ,FTranType
                       ,FTransDate
                       ,FFrameWorkID)
                        VALUES
                       (0
                       ,{0}
                       ,'{13}'
                       ,{1}
                       ,{2}
                       ,{3}
                       ,{4}
                       ,'{5}'
                       ,0
                       ,{6}
                       ,{7}
                       ,{8}
                       ,'Industry'
                       ,0
                       ,0
                       ,{12}
                       ,-1
                       ,-1
                       ,-1
                       ,null
                       ,0
                       ,null
                       ,null
                       ,{9}
                       ,{10}
                       ,'{11}'
                       ,-1)";



            sql = string.Format(sqlF, voucherId, year, period, vchTpl.GroupID, vchNumber, noteHeader, vchEntryID,
                debitTotal, creditAmount, serialNum, vchTpl.TransType, null, userID, voucherDate);
            sqllst.Add(sql);

            sql = "insert into ICVouchBillTranType(FTranType,FVoucherID) values (801," + voucherId + ")";
            sqllst.Add(sql);
            db.LongHaul(sqllst);
            return vchNumber;
        }
    }
}
