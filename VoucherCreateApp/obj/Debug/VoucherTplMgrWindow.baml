   M S B A M L   `   `   `  ����K  GVoucherCreateApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VoucherCreateAppVoucherCreateApp  5   	   *    $VoucherCreateApp.VoucherTplMgrWindow   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   a ]DevExpress.Xpf.Core.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   *localclr-namespace:VoucherCreateApp  5   	   <dxb2http://schemas.devexpress.com/winfx/2008/xaml/bars  5   	   f bDevExpress.Xpf.Grid.v15.2.Core, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aa ]DevExpress.Xpf.Grid.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxg2http://schemas.devexpress.com/winfx/2008/xaml/grid   5   	   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	     � Title$  凭证模板管理��5	   	   $	��300��6   $	��300��6%    � WindowStartupLocation$ CenterScreen=�61    � WindowState$ 	Maximized=�6V    � Icon$7 1/VoucherCreateApp;component/Images/voucherApp.png��6n    � 	Resources 5
      )  #VoucherCreateApp.RowNumberConverter   RowNumberConverter  rowIndicatorContentTemplate �� TextAlignment �� Path �� 	Converter��%�   &	        &	      7     5   
   6#   �� 5   
   .R�5      �� $��Center=�6   $��Center=�5      .+�5      �� $ Center=�5      $��Center=�63   ��5      �� $ 	RowHandle�8   5   3   5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   )  #DevExpress.Xpf.Bars.MainMenuControl  $��0��6   ,  &DevExpress.Xpf.Bars.ToolBarControlBase	   Items.	 5      '  !DevExpress.Xpf.Bars.BarButtonItem  -   5        
 �Name$
 
btnRefresh��5   -   !  DevExpress.Xpf.Bars.BarItem   Content)  #DevExpress.Xpf.Core.ObjectConverter$ 刷新 6       BarItemDisplayMode$ ContentAndGlyph=�6l      GlyphAlignment$	 Top=�6�      Glyph 6?   *  $DevExpress.Xpf.Core.DXImageExtension     Image1  +DevExpress.Xpf.Core.Native.DXImageConverter$ Refresh_32x32.png 6�     5      -   5        $
 btnShowFilter��5   -   $ 过滤 6    $ ContentAndGlyph=�6t   $	 Top=�6�    6B     $ MasterFilter_32x32.png 6�     5      -   5        $
 btnEditVoucherTpl��5   -   $ 编辑 6    $ ContentAndGlyph=�6q   $	 Top=�6�    6G     $ Edit_32x32.png 6�     5      -   5        $
 btnAddVoucherTpl��5   -   $ 新增 6    $ ContentAndGlyph=�6s   $	 Top=�6�    6F     $ AddItem_32x32.png 6�     5      -   5        $
 btnDeleteVoucherTpl��5   -   $ 删除 6    $ ContentAndGlyph=�6y   $	 Top=�6�    6I     $ Deletelist_32x32.png 6�     5      -   5        $
 
btnAppExit��5   -   $ 退出 6    $ ContentAndGlyph=�6j   $	 Top=�6�    6?     $ Close_32x32.png 6�   5       %	  DevExpress.Xpf.Grid.GridControl	  5!   
   -   5        $��gridControl1��5!   (   $��1��6    	  Columns 5"      $
  DevExpress.Xpf.Grid.GridColumn
  5#      $  DevExpress.Xpf.Grid.ColumnBase   	FieldName$ 
IsSelected��6!   $  DevExpress.Xpf.Grid.BaseColumn   VisibleIndex$ 0��6:      AllowEditing] YDevExpress.Data.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a5  /DevExpress.Utils.Design.DefaultBooleanConverter$ False 6K      AllowSorting$ False 6`      AllowColumnFiltering$ False 6u   '    HorizontalHeaderContentAlignment$ Center=�6�      
AllowFocus .    HeaderTemplate 5$      �� 5%      .R�5&      �� $��	btnChkAll��6D   -   5        $��Center=�5&   '   $��Center=�6U   6�   5'         CellTemplate 5)      �� 5*      .R�5+      �� $��Center=�6M   $��Center=�6j    d� 	IsChecked 6'   �� *Data.IsSelected+6�   5,      5.      
  50      $ BillNo��6!      Header$ 模板编号 64   52      
  53      $
 Name��6!   $ 模板名称 62   55      
  57      $ TransTypeName��6!   $ 模板类型 6;   59      
  5;      $ LastUpdateTime��6!   $ 修改时间 6<   5=      
  5>      $ 
BillerName��6!   $ 	制单人 68   5@      
  5A      $ 	IsDefault��6!   $ 是否默认 67   5C       	  View 5E      #  DevExpress.Xpf.Grid.TableView  5F      -	   5        $��
tableView1��5F   >      AllowPerPixelScrolling . &   DevExpress.Xpf.Grid.DataViewBase   ShowTotalSummary .  &   DevExpress.Xpf.Grid.GridViewBase   ShowGroupPanel .      AllowCascadeUpdate  . !   	AutoWidth! .  "   IndicatorWidth$" 40Q�5H   2   #   FontSize$# 10Q�6F   "$   RowIndicatorContentTemplate#$ [ 5I       5K      5L      5M      