   M S B A M L   `   `   `  ����K  GVoucherCreateApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VoucherCreateAppVoucherCreateApp  5   	   +    %VoucherCreateApp.VoucherTplEditWindow   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   0localclr-namespace:VoucherCreateApp     5   	   a ]DevExpress.Xpf.Core.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   <dxb2http://schemas.devexpress.com/winfx/2008/xaml/bars  5   	   f bDevExpress.Xpf.Grid.v15.2.Core, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aa ]DevExpress.Xpf.Grid.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxg2http://schemas.devexpress.com/winfx/2008/xaml/grid   5   	   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	     � Title$  编辑凭证模板��5	   	   $	��400��6   $	��720��6%    � WindowStartupLocation$ CenterScreen=�61    � Icon$7 1/VoucherCreateApp;component/Images/voucherApp.png��6V    � 	Resources 5
      )  #VoucherCreateApp.RowNumberConverter   RowNumberConverter  rowIndicatorContentTemplate �� TextAlignment �� Path �� 	Converter �� 
TargetType �� Property	 �� Value+  %DevExpress.Xpf.Grid.LookUp.LookUpEdit��%�  &	        &	      7   '�� �     '�� Y    '�� �    '�� =    '  �    '� !      5   
   6#   �� 5   
   .R�5      �� $��Center=�6   $��Center=�5      .+�5      �� $ Center=�5      $��Center=�63   ��5      �� $ 	RowHandle�8   5   3   5      5      �� 5   
   " ��6   .�5      ��  � ��$	 Center=�64   �� 5       �@��Orientation$	 
Horizontal=�6,   5      �� 5   
   " ��6   .�5      ��  � ��$	 Center=�62   �� 5       � ��$
	 10,0q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$	 Center=�62   �� 5       � ��$
	 10,0q�6'   5      �� 5   
   " ��6   .�5       ��  � ��$	 Center=�62   �� 5!       � ��$
	 10,0q�6'   5"      �� 5#   
   "  6   .�5$      ��  � ��$	 Center=�62   �� 5%       � ��$
	 10,0q�6'   5&      �� 5'   
   " �6   5)      5*      .��5+      � 
 � RowDefinitions
 5,   
   �� 5-      $
��Auto �6   �� 5.      $
��Auto �6   �� 5/      $��* �6   �� 50      $
��Auto �6   .+�52   
   )  #DevExpress.Xpf.Bars.MainMenuControl  $��0��6   ,  &DevExpress.Xpf.Bars.ToolBarControlBase   Items. 53      '  !DevExpress.Xpf.Bars.BarButtonItem  -   5         �Name$ btnSaveVoucherTpl��53   -   !  DevExpress.Xpf.Bars.BarItem   Content)  #DevExpress.Xpf.Core.ObjectConverter$ 保存 6       BarItemDisplayMode$ ContentAndGlyph=�6q      GlyphAlignment$	 Top=�6�      Glyph 6G   *  $DevExpress.Xpf.Core.DXImageExtension     Image1	  +DevExpress.Xpf.Core.Native.DXImageConverter$ Save_32x32.png	 6�     54      -   5        $ btnAddNewRow��54   -   $ 添加 6    $ ContentAndGlyph=�6k   $	 Top=�6�    6B     $ Add_32x32.png	 6�     55      -   5        $ btnRemoveRow��55   -   $ 删除 6    $ ContentAndGlyph=�6n   $	 Top=�6�    6B     $ Remove_32x32.png	 6�     56      -   5        $ 
btnAppExit��56   -   $ 退出 6    $ ContentAndGlyph=�6j   $	 Top=�6�    6?     $ Close_32x32.png	 6�   57      � 58   
   $��1��6   $��10,10q�6    � ColumnDefinitions 59      �� 5:      $��* �6#   �� 5;      $��2* �6#   �� 5<      $��* �6#   .+�5>      �� $��0��6   .+�5?      �� .��6   模板编号6   �� 5@      -   5        $��txtVoucherTplNo��5@      	TPL00016<   5A      �� 5B      $��1��6    �� Orientation$ 
Horizontal=�6)   $��Center=�6B   .+�5C      �� .��6   模板名称6   �� 5D      -   5        $��txtVoucherName��5D   &   $	��150��6   .��6<   	Tpl00016E   5E      �� 5F      $��2��6   .+�5G      �� .��6   	凭证字6   )
  #DevExpress.Xpf.Editors.ComboBoxEdit
  5H      -   5        $��cboVoucherGroup��5H   #   $��60��6:   +  %DevExpress.Xpf.Editors.LookUpEditBase   DisplayMember$
 Name��6E      ValueMember$ GroupID��6Z   5I      5J      5K      %  DevExpress.Xpf.Grid.GridControl  5L   
   -   5        $��gridControl1��5L   '   $��2��6      Columns 5M      $  DevExpress.Xpf.Grid.GridColumn  5N      $  DevExpress.Xpf.Grid.ColumnBase   	FieldName$ AccID��6!   $  DevExpress.Xpf.Grid.BaseColumn   Header$ 科目 63      AllowAutoFilter .     AllowColumnFiltering] YDevExpress.Data.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a5  /DevExpress.Utils.Design.DefaultBooleanConverter$ False 6W      EditSettings 5O      3  -DevExpress.Xpf.Grid.LookUp.LookUpEditSettings  5P      -	   5        $ 
lookUpAcct��5P   1   <  6DevExpress.Xpf.Editors.Settings.LookUpEditSettingsBase   DisplayMember$
 Name��5Q         ValueMember$ 	AccountID��5R         AutoComplete .    AutoPopulateColumns .      IncrementalFiltering$
  TrueX�5U      !   ImmediatePopup! . ;  5DevExpress.Xpf.Editors.Settings.PopupBaseEditSettings"   PopupContentTemplate" 5W      �� 5X   "   .R�5Y   &     $��PART_GridControl��66    5Z   *     5[   .   $ Number��6=   $ 科目代码 6P     5\   .   $
 Name��6=   $ 科目名称 6N   #   View# 5^   *   #  DevExpress.Xpf.Grid.TableView  5_   .   $   	AutoWidth$ . &   DevExpress.Xpf.Grid.GridViewBase%   ShowGroupPanel% .  &   DevExpress.Xpf.Grid.DataViewBase&   ShowSearchPanelMode$& Always=�6d   5a   '   5b   #   5d      5f        5g      $ 	Direction��6!   $ 借贷方向 67   '   AllowEditing$
' True 6E   $ False 6Y    5h      :  4DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings  5i      -
   5        $ cboDirection��5i   3   $
 Name��6G   $ DirectionID��6\   6  0DevExpress.Xpf.Editors.Settings.BaseEditSettings(   StyleSettings( 5j      2  ,DevExpress.Xpf.Editors.ComboBoxStyleSettings  5k   "   5m      5o        5p      $ 
AmountFrom��6!   $ 金额来源 68   $
' True 6F   $ False 6Z    5q        5r      -   5        $ cboAmountFrom��5r   3   $ 	FieldName��6H   $ InterID��6b   8  2DevExpress.Xpf.Editors.Settings.ButtonEditSettings)   IsTextEditable$) FalseX�6x    . ( 5t        5u   "   5w      5y        5z      $
 Note��6!   $ 摘要 62   $
' True 6?   $ False 6S    5{      6  0DevExpress.Xpf.Editors.Settings.TextEditSettings  5|      5~        5      $ 核算项目 6!   $ items��6/   $ False 6A    5�        5�      -   5        $ 	itemsEdit��5�   L   *   AllowDefaultButton$* FalseX�61   +   Buttons+ 5�      '  !DevExpress.Xpf.Editors.ButtonInfo  5�   "   -   5        5�   1   5�      5�      # 5�        5�      -   5        $��
tableView1��5�       ,   AllowPerPixelScrolling, . -   ShowTotalSummary- .  % .  .   AllowCascadeUpdate. . /   NewItemRowPosition$/ Bottom=�5�       $ .  0   IndicatorWidth$0 40Q�5�   2   1   FontSize$1 10Q�6F   2   AllowEditing2 . "3   RowIndicatorContentTemplate#3 [ 5�       6�   5�      � 5�   
   $��3��6   $��10,10q�6    5�      �� 5�      $��* �6#   �� 5�      $��* �6#   �� 5�      $��* �6#   �� 5�      $��* �6#   .+�5�      �� $��0��6   .+�5�      �� .��6   时间6   �� 5�      -   5        $��txtDate��5�      2019-03-26 12:36:566@   5�      �� 5�      $��1��6   $ 
Horizontal=�6)   $��Center=�6B   .+�5�      �� .��6   	制单人6     5�      -   5        $��
lookUpUser��5�   !   $
 Name��5�      $ UserID��5�      4   AutoPopulateColumns4 .  5   AutoComplete5 . 6   IncrementalFiltering$
6 TrueX�5�      7   ImmediatePopup7 . 8   IsPopupAutoWidth8 .  $��Center=�5�      $��80��5�      *  $DevExpress.Xpf.Editors.PopupBaseEdit9   PopupContentTemplate9 5�      �� 5�      .R�5�        $��PART_GridControl��6.    5�   "     5�   &   $ UserID��65   $ 用户ID 6H     5�   &   $
 Name��65   $ 	用户名 6F   # 5�   "     5�   &   $ . % .  $& Always=�6\   5�      5�      5�      5�      �� 5�      $��2��6   .+�5�      �� .��6   单据类型6   
  5�      -   5        $��cboTransType��5�   #   $	��100��67   $
 Name��6C   $ ID��6X   5�      5�      �� 5�      $��3��6   .+�5�      �� .��6   默认模板6   
  5�      -   5        $��
cboDefault��5�   #   $��40��65   $
 Name��6@   $ ID��6U   5�      5�      5�      5�      5�      