   M S B A M L   `   `   `  ����K  GVoucherCreateApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VoucherCreateAppVoucherCreateApp  5   	   %    VoucherCreateApp.SetItemsWindow   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   .localclr-namespace:VoucherCreateApp    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   a ]DevExpress.Xpf.Core.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   <dxb2http://schemas.devexpress.com/winfx/2008/xaml/bars  5   	   f bDevExpress.Xpf.Grid.v15.2.Core, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aa ]DevExpress.Xpf.Grid.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxg2http://schemas.devexpress.com/winfx/2008/xaml/grid   5   	   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	     � Title$  核算项目设置��5	   	   $	��360��6   $	��320��6%    � WindowStartupLocation$ CenterScreen=�61    � 
ResizeMode$ NoResize=�6V    � 	Resources 5
      )  #VoucherCreateApp.RowNumberConverter   RowNumberConverter  rowIndicatorContentTemplate �� TextAlignment �� Path �� 	Converter��%�   &	        &	      7     5   
   6#   �� 5   
   .R�5      �� $��Center=�6   $��Center=�5      .+�5      �� $ Center=�5      $��Center=�63   ��5      �� $ 	RowHandle�8   5   3   5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   %  DevExpress.Xpf.Grid.GridControl  -   5        $��gridControl1��5      $��1��6.      Columns 5      $  DevExpress.Xpf.Grid.GridColumn  5      $  DevExpress.Xpf.Grid.ColumnBase	   	FieldName$	 Number��6!   $  DevExpress.Xpf.Grid.BaseColumn
   Header)  #DevExpress.Xpf.Core.ObjectConverter$
 类别代码 64      AllowEditing] YDevExpress.Data.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a5  /DevExpress.Utils.Design.DefaultBooleanConverter$
 True 6B   5        5      $
	 Name��6!   $
 类别名称 62   $
 True 6@   5        5      $	 Field��6!   $
 对应单据上项目 63   $
 True 6D      EditSettings 5       :  4DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings  5!      -   5         �Name$ cboBillField��5!   3   <	  6DevExpress.Xpf.Editors.Settings.LookUpEditSettingsBase 	  DisplayMember$
 Name��6G    	  ValueMember$ Field��6\    	  AutoComplete .  	  AllowItemHighlighting .  6
  0DevExpress.Xpf.Editors.Settings.BaseEditSettings 
  StyleSettings 5"      2  ,DevExpress.Xpf.Editors.ComboBoxStyleSettings  5#   "   5%      5'         View 5)      #  DevExpress.Xpf.Grid.TableView  5*      -   5        $��
tableView1��5*   >      AllowPerPixelScrolling . &   DevExpress.Xpf.Grid.DataViewBase   ShowTotalSummary .  &   DevExpress.Xpf.Grid.GridViewBase   ShowGroupPanel .     	AutoWidth . "   RowIndicatorContentTemplate# [ 5+       6k   5-      5.      5/      