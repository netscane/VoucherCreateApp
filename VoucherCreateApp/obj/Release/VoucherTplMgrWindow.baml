   M S B A M L   `   `   `  ���� K  GVoucherCreateApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VoucherCreateAppVoucherCreateApp  *    $VoucherCreateApp.VoucherTplMgrWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     a ]DevExpress.Xpf.Core.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  *localclr-namespace:VoucherCreateApp  <dxb2http://schemas.devexpress.com/winfx/2008/xaml/bars  f bDevExpress.Xpf.Grid.v15.2.Core, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aa ]DevExpress.Xpf.Grid.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxg2http://schemas.devexpress.com/winfx/2008/xaml/grid   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors    � Title$  凭证模板管理��$	��300��$	��300�� � WindowStartupLocation$ CenterScreen=� � WindowState$ 	Maximized=� � Icon$7 1/VoucherCreateApp;component/Images/voucherApp.png�� � 	Resources )  #VoucherCreateApp.RowNumberConverter   RowNumberConverter  rowIndicatorContentTemplate �� TextAlignment �� Path �� 	Converter��%�   &	        &	      7     �� .R��� $��Center=�$��Center=�.+��� $ Center=�$��Center=����� $ 	RowHandle�8   .���  � RowDefinitions �� $
��Auto ��� $��* �.+�)  #DevExpress.Xpf.Bars.MainMenuControl  $��0��,  &DevExpress.Xpf.Bars.ToolBarControlBase	   Items.	 '  !DevExpress.Xpf.Bars.BarButtonItem  -   
 �Name$
 
btnRefresh��!  DevExpress.Xpf.Bars.BarItem   Content)  #DevExpress.Xpf.Core.ObjectConverter$ 刷新    BarItemDisplayMode$ ContentAndGlyph=�   GlyphAlignment$	 Top=�   Glyph *  $DevExpress.Xpf.Core.DXImageExtension     Image1  +DevExpress.Xpf.Core.Native.DXImageConverter$ Refresh_32x32.png   -   $
 btnShowFilter��$ 过滤 $ ContentAndGlyph=�$	 Top=�   $ MasterFilter_32x32.png   -   $
 btnEditVoucherTpl��$ 编辑 $ ContentAndGlyph=�$	 Top=�   $ Edit_32x32.png   -   $
 btnAddVoucherTpl��$ 新增 $ ContentAndGlyph=�$	 Top=�   $ AddItem_32x32.png   -   $
 btnDeleteVoucherTpl��$ 删除 $ ContentAndGlyph=�$	 Top=�   $ Deletelist_32x32.png   -   $
 
btnAppExit��$ 退出 $ ContentAndGlyph=�$	 Top=�   $ Close_32x32.png %	  DevExpress.Xpf.Grid.GridControl	  -   $��gridControl1��$��1�� 	  Columns $
  DevExpress.Xpf.Grid.GridColumn
  $  DevExpress.Xpf.Grid.ColumnBase   	FieldName$ 
IsSelected��$  DevExpress.Xpf.Grid.BaseColumn   VisibleIndex$ 0��   AllowEditing] YDevExpress.Data.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a5  /DevExpress.Utils.Design.DefaultBooleanConverter$ False    AllowSorting$ False    AllowColumnFiltering$ False '    HorizontalHeaderContentAlignment$ Center=�   
AllowFocus .    HeaderTemplate �� .R��� $��	btnChkAll��-   $��Center=�$��Center=�   CellTemplate �� .R��� $��Center=�$��Center=� d� 	IsChecked �� *Data.IsSelected+
  $ BillNo��   Header$ 模板编号 
  $
 Name��$ 模板名称 
  $ TransTypeName��$ 模板类型 
  $ LastUpdateTime��$ 修改时间 
  $ 
BillerName��$ 	制单人 
  $ 	IsDefault��$ 是否默认  	  View #  DevExpress.Xpf.Grid.TableView  -	   $��
tableView1��   AllowPerPixelScrolling . &   DevExpress.Xpf.Grid.DataViewBase   ShowTotalSummary .  &   DevExpress.Xpf.Grid.GridViewBase   ShowGroupPanel .      AllowCascadeUpdate  . !   	AutoWidth! .  "   IndicatorWidth$" 40Q�#   FontSize$# 10Q�"$   RowIndicatorContentTemplate#$ [ 