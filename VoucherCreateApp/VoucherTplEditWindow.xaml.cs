using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoucherCreateApp.Model;
using VoucherCreateApp.Dao;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;

namespace VoucherCreateApp
{
    /// <summary>
    /// VoucherTplEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VoucherTplEditWindow : DXWindow
    {
        private VoucherTpl voucherTpl { set; get; }
        UserDao userDao = new UserDao();
        VoucherGroupDao groupDao = new VoucherGroupDao();
        AccountDao acctDao = new AccountDao();
        AmountFromDao amtDao = new AmountFromDao();
        VoucherTplDao tplDao = new VoucherTplDao();
        ItemClassDao itemClsDao = new ItemClassDao(); 
        List<ItemClass> itemClassLst = new List<ItemClass>();

        public VoucherTplEditWindow()
        {
            InitializeComponent();
            
            InitView();
        }

        public VoucherTplEditWindow(string billNo)
        {
            InitializeComponent();
            voucherTpl = tplDao.getDetailByBillNo(billNo);
            InitView();
        }
        public void InitView()
        {

            ObservableCollection<User> userSource = userDao.getCollection();
            lookUpUser.ItemsSource = userSource;
            List<VoucherGroup> groupLst = groupDao.getList();
            cboVoucherGroup.ItemsSource = groupLst;

            ObservableCollection<Account> acctSource = acctDao.getCollection();
            lookUpAcct.ItemsSource = acctSource;

            List<AmountFrom> amtfSoure = amtDao.getList();
            cboAmountFrom.ItemsSource = amtfSoure;
            
            Direction[] directionSource = new Direction[] { new Direction { DirectionID = 0, Name = "贷" }, new Direction { DirectionID = 1, Name = "借" } };
            cboDirection.ItemsSource = directionSource;

            TransType[] transTypeSource = new TransType[] { new TransType { ID = 80, Name = "销售发票" }, new TransType { ID = 86, Name = "销售发票(普通)" } };
            cboTransType.ItemsSource = transTypeSource;

            DefaultModel[] defaultSource = new DefaultModel[] { new DefaultModel { ID = 0, Name = "否" }, new DefaultModel { ID = 1, Name = "是" } };
            cboDefault.ItemsSource = defaultSource;

            if (voucherTpl != null)
            {
                //load tpl
                gridControl1.ItemsSource = voucherTpl.EntryList;

                cboVoucherGroup.EditValue = voucherTpl.GroupID;
                lookUpUser.EditValue = voucherTpl.BillerID;
                cboTransType.EditValue = voucherTpl.TransType;
                txtDate.Text = voucherTpl.LastUpdateTime.ToLongDateString();
                cboDefault.EditValue = voucherTpl.IsDefault;
                txtVoucherName.Text = voucherTpl.Name;
                txtVoucherTplNo.Text = voucherTpl.BillNo;
                cboTransType.IsEnabled = false;
            }
            else
            {
                //create tpl
                voucherTpl = new VoucherTpl();
                voucherTpl.EntryList = new System.Collections.ObjectModel.ObservableCollection<VoucherTplEntry>();
                gridControl1.ItemsSource = voucherTpl.EntryList;

                cboVoucherGroup.EditValue = groupLst[0].GroupID;
                lookUpUser.EditValue = userSource[0].UserID;
                cboTransType.EditValue = transTypeSource[0].ID;
                txtDate.Text = DateTime.Now.ToString();
                cboDefault.EditValue = 1;
                txtVoucherName.Text = "新建模板";
                txtVoucherTplNo.Text = "自动生成";
            }
            //voucherTpl.EntryList.CollectionChanged += List_CollectionChanged; 
        }

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MessageBox.Show("我是list 集合,我被修改了，现在的操作为:" );
        }

        private void btnSaveVoucherTpl_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            ObservableCollection<VoucherTplEntry> source = gridControl1.ItemsSource as ObservableCollection<VoucherTplEntry>;
            voucherTpl.Name = txtVoucherName.Text;
            voucherTpl.GroupID = (int)cboVoucherGroup.EditValue;
            voucherTpl.BillerID = (int)lookUpUser.EditValue;
            voucherTpl.TransType = (int)cboTransType.EditValue;
            voucherTpl.IsDefault = (int)cboDefault.SelectedIndex;
           // MessageBox.Show(voucherTpl.EntryList.Count.ToString());
            tplDao.SaveVoucherTpl(voucherTpl);
            this.Close();
        }

        private void btnAppExit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnAddNewRow_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            tableView1.AddNewRow();
        }

        private void btnRemoveRow_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            tableView1.DeleteRow(tableView1.FocusedRowHandle);
        }

        private void tableView1_InitNewRow(object sender, DevExpress.Xpf.Grid.InitNewRowEventArgs e)
        {
            gridControl1.SetCellValue(e.RowHandle, "Note", "");
            ObservableCollection<Account> source = lookUpAcct.ItemsSource as ObservableCollection<Account>;
            gridControl1.SetCellValue(e.RowHandle, "AccID", source[0].AccountID);

            gridControl1.SetCellValue(e.RowHandle, "AmountFrom", 33);

            gridControl1.SetCellValue(e.RowHandle, "Note", "收入: [销售发票.购货单位][销售发票.合同单号][销售发票.进度提醒]");
            voucherTpl.EntryList[voucherTpl.EntryList.Count - 1].EntryID = voucherTpl.EntryList.Count - 1;
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(ShowSetItemsWindow));
        }

        public void ShowSetItemsWindow()
        {
            SetItemsWindow wind = new SetItemsWindow();

            wind.TransType = (int)cboTransType.EditValue;
            VoucherTplEntry entry = (VoucherTplEntry)gridControl1.GetFocusedRow();
            if (entry == null)
                return;
            wind.Data = entry.itemClassLst;
            wind.InitView();
            wind.ShowDialog();
        }

        private void tableView1_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "AccID")
            {
                VoucherTplEntry entry = e.Row as VoucherTplEntry;
                entry.itemClassLst = itemClsDao.getCollectionByAccount(entry.AccID);
            }
        }
    }
}
