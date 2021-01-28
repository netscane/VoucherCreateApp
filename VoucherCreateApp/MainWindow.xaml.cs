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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using VoucherCreateApp.Dao;
using System.Data;
using System.Data.Sql;
using VoucherCreateApp.Model;
using System.Threading;
using System.Globalization;
using System.Data.SqlClient;
using DevExpress.XtraEditors.DXErrorProvider;

namespace VoucherCreateApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXWindow
    {
        
        public MainWindow()
        {
            
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-CN", true)
            {
                DateTimeFormat = { ShortDatePattern = "yyyy-MM-dd", FullDateTimePattern = "yyyy-MM-dd HH:mm:ss", LongTimePattern = "HH:mm:ss" }
            };
            
            DbHelper.getInstance().ConnectionString = GetSqlConnStrFromLogin(App.ArgConnStr);
            
            
         // DbHelper.getInstance().ConnectionString = "Persist Security Info=True;User ID=sa;Password=sa@123;Data Source=192.168.123.225;Initial Catalog=AIS20180111173203;";
        //    DbHelper.getInstance().ConnectionString = "Persist Security Info=True;User ID=sa;Password=sa@123;Data Source=127.0.0.1;Initial Catalog=AIS20180509135446;";
            this.Bind();
        }

        ICSaleDao mICSaleDao = new ICSaleDao();
        VoucherDao voucherDao = new VoucherDao();
        VoucherTplDao voucherTplDao = new VoucherTplDao();
        public static String GetSqlConnStrFromLogin(String LoginProps)
        {
  
            String partStr = LoginProps.Substring(LoginProps.IndexOf("{") + 1, (LoginProps.IndexOf("}") - LoginProps.IndexOf("{") - 1));

            String[] pairs = partStr.Split(';');

            String connStr = "";
            foreach (String param in pairs)
            {
                if (!param.StartsWith("Provider"))
                    connStr += param+";";
            }
            //MessageBox.Show(connStr);
            return connStr;
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            gridcontrol1.ItemsSource = mICSaleDao.getICSaleDisplayList();
        }

        private void btnShowFilter_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            gridView.ShowSearchPanel(true);
        }

        private void btnCreateVoucher_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            string name = "";
            List<ICSaleDisplay> lst = gridcontrol1.ItemsSource as List<ICSaleDisplay>;
            int j = 0;
            HashSet<string> billNoSet = new HashSet<string>(); 
            for (int i = 0; i < lst.Count; i++)
            {
                int rowHandle = this.gridcontrol1.GetRowHandleByListIndex(i);
                ICSaleDisplay sale = gridcontrol1.GetRow(rowHandle) as ICSaleDisplay;
                
                object rowCheck = gridcontrol1.GetCellValue(rowHandle, "IsSelected");
                bool ifCheck = rowCheck==null?false:(bool)rowCheck;
                if (ifCheck)
                {
                    OpResult result = new OpResult();
                    int vchNumber;
                    try
                    {
                        vchNumber = voucherDao.CreateVoucher(App.ArgUserName, sale, result);
                    }
                    catch (SqlException se)
                    {
                        MessageBox.Show("数据库异常:" + se.Message);
                        return;
                    }
                    catch (Exception ce)
                    {
                        MessageBox.Show(ce.Message);
                        return;
                    }
                    if (result.ErrorCode != 0)
                        MessageBox.Show(result.ErrMessage, "凭证生成", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("单据" + sale.FBillNo + "生成凭证成功,凭证字号" + vchNumber, "凭证生成", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            gridcontrol1.ItemsSource = mICSaleDao.getICSaleDisplayList();
        }

        private void btnAppExit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void btnChkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            bool? cbStatus = cb.IsChecked;
            List<ICSaleDisplay> lst = gridcontrol1.ItemsSource as List<ICSaleDisplay>;
            for (int i = 0; i < lst.Count; ++i)
            {
                int rowHandle = gridcontrol1.GetRowHandleByListIndex(i);
                gridcontrol1.SetCellValue(rowHandle, "IsSelected", cbStatus);
            }
        }

        public void Bind()
        {
           // MessageBox.Show("获取数据");
            gridcontrol1.ItemsSource = mICSaleDao.getICSaleDisplayList();
           // MessageBox.Show("获取数据成功");
            //gridcontrol1.Columns[4].GroupIndex = 0;
           // gridcontrol1.Columns[4].AllowCellMerge = true;
           // gridcontrol1.Columns[0].AllowCellMerge = true;
            cboVoucherTpl.ItemsSource = voucherTplDao.getAll();
        }

        private void btnVoucherTplMgr_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            VoucherTplMgrWindow window = new VoucherTplMgrWindow();
            window.ShowDialog();
        }

        private void btnCreateVoucherCombine_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            List<ICSaleDisplay> lst = gridcontrol1.ItemsSource as List<ICSaleDisplay>;
            int j = 0;
            List<ICSaleDisplay> myList = new List<ICSaleDisplay>();
            for (int i = 0; i < lst.Count; i++)
            {
                int rowHandle = this.gridcontrol1.GetRowHandleByListIndex(i);
                ICSaleDisplay sale = gridcontrol1.GetRow(rowHandle) as ICSaleDisplay;
                object rowCheck = gridcontrol1.GetCellValue(rowHandle, "IsSelected");
                bool ifCheck = rowCheck == null ? false : (bool)rowCheck;
                if (ifCheck)
                {
                    myList.Add(sale);
                }
            }

            OpResult result = new OpResult();
            int vchNumber=0;
            try
            {
                vchNumber = voucherDao.CreateVoucherCombine(App.ArgUserName, myList, result);
               // MessageBox.Show(billNoSet.ToString());
            }
            catch (SqlException se)
            {
                MessageBox.Show("数据库异常:" + se.Message);
                return;
            }
            catch (Exception ce)
            {
                MessageBox.Show(ce.Message);
                return;
            }
            if (result.ErrorCode != 0)
                MessageBox.Show(result.ErrMessage, "凭证生成", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show("单据合并生成凭证成功,凭证字号" + vchNumber, "凭证合并生成", MessageBoxButton.OK, MessageBoxImage.Information);
            gridcontrol1.ItemsSource = mICSaleDao.getICSaleDisplayList();
        }

        private void gridView_ValidateCell(object sender, DevExpress.Xpf.Grid.GridCellValidationEventArgs e)
        {
            if (e.Column.FieldName.Equals("FVoucherTplID"))
            {
                ICSaleDisplay sale = (ICSaleDisplay)e.Row;
                VoucherTplSimple vchTpl = cboVoucherTpl.GetItemFromValue(e.Value) as VoucherTplSimple;
                if (vchTpl.TransType != sale.FTransType)
                {
                    e.ErrorType = ErrorType.Warning;
                    e.ErrorContent = "模板类型错误,请选择对应单据类型的模板!";
                    e.IsValid = false;
                }
            }
        }
    }
}
