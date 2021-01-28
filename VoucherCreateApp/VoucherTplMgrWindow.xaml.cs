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
using DevExpress.Xpf.Core;
using VoucherCreateApp.Dao;
using VoucherCreateApp.Model;

namespace VoucherCreateApp
{
    /// <summary>
    /// VoucherTplMgrWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VoucherTplMgrWindow : DXWindow
    {
        VoucherTplDao tplDao = new VoucherTplDao();
        public VoucherTplMgrWindow()
        {
            InitializeComponent();
            gridControl1.ItemsSource = tplDao.getListInfo();
        }

        private void btnEditVoucherTpl_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            List<VoucherTplInfo> lst = gridControl1.ItemsSource as List<VoucherTplInfo>;

            int selectedCount = 0;
            string billNo = "";
            for (int i = 0; i < lst.Count; i++)
            {
                int rowHandle = this.gridControl1.GetRowHandleByListIndex(i);
                object wholeRowObject = gridControl1.GetRow(rowHandle);
                object rowCheck = gridControl1.GetCellValue(rowHandle, "IsSelected");
                bool ifCheck = rowCheck == null ? false : (bool)rowCheck;
                if (ifCheck)
                {
                    object rowname = gridControl1.GetCellValue(rowHandle, "BillNo");
                    selectedCount++;
                    billNo = rowname.ToString();
                }
            }
            if (selectedCount < 1)
            {
                MessageBox.Show("请勾选指定模板后进行编辑!");
                return;
            }
            else if (selectedCount > 1)
            {
                MessageBox.Show("一次只能编辑一个模板!");
                return;
            }
                
            Dispatcher.BeginInvoke(new Action<string>(ShowTplEditWindow), billNo);
        }

        private void btnDeleteVoucherTpl_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

            List<VoucherTplInfo> lst = gridControl1.ItemsSource as List<VoucherTplInfo>;

            for (int i = 0; i < lst.Count; i++)
            {
                int rowHandle = this.gridControl1.GetRowHandleByListIndex(i);
                object wholeRowObject = gridControl1.GetRow(rowHandle);
                object rowCheck = gridControl1.GetCellValue(rowHandle, "IsSelected");
                bool ifCheck = rowCheck == null ? false : (bool)rowCheck;
                if (ifCheck)
                {
                    object rowname = gridControl1.GetCellValue(rowHandle, "BillNo");
                    tplDao.DeleteVoucherTpl(rowname.ToString());
                }
            }
            gridControl1.ItemsSource = tplDao.getListInfo();
        }

        private void btnAppExit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnChkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            bool? cbStatus = cb.IsChecked;
            List<VoucherTplInfo> lst = this.gridControl1.ItemsSource as List<VoucherTplInfo>;
            for (int i = 0; i < lst.Count; ++i)
            {
                int rowHandle = gridControl1.GetRowHandleByListIndex(i);
                gridControl1.SetCellValue(rowHandle, "IsSelected", cbStatus);
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            gridControl1.ItemsSource = tplDao.getListInfo();
        }

        private void btnShowFilter_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            tableView1.ShowSearchPanel(true);
        }

        private void btnAddVoucherTpl_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action<string>(ShowTplEditWindow),"");
        }

        public void ShowTplEditWindow(string billNo)
        {
            VoucherTplEditWindow window;
            if (!string.IsNullOrEmpty(billNo))
            {
                window = new VoucherTplEditWindow(billNo);
            }
            else
                window = new VoucherTplEditWindow();
            window.ShowDialog();
            gridControl1.ItemsSource = tplDao.getListInfo();
        }
    }
}
