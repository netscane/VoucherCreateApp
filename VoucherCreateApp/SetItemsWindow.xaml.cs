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
using VoucherCreateApp.Model;
using VoucherCreateApp.Dao;
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.DXErrorProvider;

namespace VoucherCreateApp
{
    /// <summary>
    /// SettItemsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetItemsWindow : DXWindow
    {
        ItemClassDao itemClsDao = new ItemClassDao();
        BillFieldDao billFieldDao = new BillFieldDao();

        public int TransType { set; get; }

        public ObservableCollection<ItemClass> Data { set; get; }

        public SetItemsWindow()
        {
            InitializeComponent();
        }

        public void InitView()
        {
            cboBillField.ItemsSource = billFieldDao.getCollection(TransType);
            gridControl1.ItemsSource = Data;
        }

        private void btnComfirm_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            ObservableCollection<ItemClass> source = gridControl1.ItemsSource as ObservableCollection<ItemClass>;

            for (int i=0; i<source.Count; ++i)
            {
                ItemClass item = source[i];
                //数据校验
                if (!string.IsNullOrWhiteSpace(item.Field))
                {
                    int itemClassID = billFieldDao.getItemClassID(TransType, item.Field);
                    if (item.ItemClassID != itemClassID)
                    {
                        MessageBox.Show("核算项目[" + item.Name + "]不能选择单据的[" + gridControl1.GetCellDisplayText(i, "Field")+"]");
                        return;
                    }
                }
                    
            }
            Data = source;
            this.Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void cboBillField_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            int itemClassID = ((ItemClass)gridControl1.GetFocusedRow()).ItemClassID;
            CriteriaOperator op = CriteriaOperator.Parse("ItemClassID=" + itemClassID);
            cboBillField.FilterCriteria = op;
        }

        private void tableView1_ValidateCell(object sender, DevExpress.Xpf.Grid.GridCellValidationEventArgs e)
        {
            int itemClassID = billFieldDao.getItemClassID(TransType, e.Value.ToString());
            if (itemClassID != ((ItemClass)e.Row).ItemClassID)
            {
                e.ErrorType = ErrorType.Warning;
                e.ErrorContent = "科目的核算项目[" + ((ItemClass)e.Row).Name + "]不能选择单据的该字段";
                e.IsValid = false;
            }
            
        }
    }
}
