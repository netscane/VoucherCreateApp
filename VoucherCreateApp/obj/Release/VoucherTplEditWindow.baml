   M S B A M L   `   `   `  ���� K  GVoucherCreateApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VoucherCreateAppVoucherCreateApp  +    %VoucherCreateApp.VoucherTplEditWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     0localclr-namespace:VoucherCreateApp     a ]DevExpress.Xpf.Core.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  <dxb2http://schemas.devexpress.com/winfx/2008/xaml/bars  f bDevExpress.Xpf.Grid.v15.2.Core, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aa ]DevExpress.Xpf.Grid.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxg2http://schemas.devexpress.com/winfx/2008/xaml/grid   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors    � Title$  编辑凭证模板��$	��400��$	��720�� � WindowStartupLocation$ CenterScreen=� � Icon$7 1/VoucherCreateApp;component/Images/voucherApp.png�� � 	Resources )  #VoucherCreateApp.RowNumberConverter   RowNumberConverter  rowIndicatorContentTemplate �� TextAlignment �� Path �� 	Converter �� 
TargetType �� Property	 �� Value+  %DevExpress.Xpf.Grid.LookUp.LookUpEdit��%  &	        &	      7   '�� l     '�� �     '�� �     '�� ;    '  z    '� �      �� .R��� $��Center=�$��Center=�.+��� $ Center=�$��Center=����� $ 	RowHandle�8   �� " ��.���  � ��$	 Center=���  �@��Orientation$	 
Horizontal=��� " ��.���  � ��$	 Center=���  � ��$
	 10,0q��� " ��.���  � ��$	 Center=���  � ��$
	 10,0q��� " ��.���  � ��$	 Center=���  � ��$
	 10,0q��� "  .���  � ��$	 Center=���  � ��$
	 10,0q��� " �.��� 
 � RowDefinitions
 �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+�)  #DevExpress.Xpf.Bars.MainMenuControl  $��0��,  &DevExpress.Xpf.Bars.ToolBarControlBase   Items. '  !DevExpress.Xpf.Bars.BarButtonItem  -    �Name$ btnSaveVoucherTpl��!  DevExpress.Xpf.Bars.BarItem   Content)  #DevExpress.Xpf.Core.ObjectConverter$ 保存    BarItemDisplayMode$ ContentAndGlyph=�   GlyphAlignment$	 Top=�   Glyph *  $DevExpress.Xpf.Core.DXImageExtension     Image1	  +DevExpress.Xpf.Core.Native.DXImageConverter$ Save_32x32.png	   -   $ btnAddNewRow��$ 添加 $ ContentAndGlyph=�$	 Top=�   $ Add_32x32.png	   -   $ btnRemoveRow��$ 删除 $ ContentAndGlyph=�$	 Top=�   $ Remove_32x32.png	   -   $ 
btnAppExit��$ 退出 $ ContentAndGlyph=�$	 Top=�   $ Close_32x32.png	 � $��1��$��10,10q� � ColumnDefinitions �� $��* ��� $��2* ��� $��* �.+��� $��0��.+��� .��模板编号�� -   $��txtVoucherTplNo��	TPL0001�� $��1�� �� Orientation$ 
Horizontal=�$��Center=�.+��� .��模板名称�� -   $��txtVoucherName��$	��150��.��	Tpl0001�� $��2��.+��� .��	凭证字)
  #DevExpress.Xpf.Editors.ComboBoxEdit
  -   $��cboVoucherGroup��$��60��+  %DevExpress.Xpf.Editors.LookUpEditBase   DisplayMember$
 Name��   ValueMember$ GroupID��%  DevExpress.Xpf.Grid.GridControl  -   $��gridControl1��$��2��   Columns $  DevExpress.Xpf.Grid.GridColumn  $  DevExpress.Xpf.Grid.ColumnBase   	FieldName$ AccID��$  DevExpress.Xpf.Grid.BaseColumn   Header$ 科目    AllowAutoFilter .     AllowColumnFiltering] YDevExpress.Data.v15.2, Version=15.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a5  /DevExpress.Utils.Design.DefaultBooleanConverter$ False    EditSettings 3  -DevExpress.Xpf.Grid.LookUp.LookUpEditSettings  -	   $ 
lookUpAcct��<  6DevExpress.Xpf.Editors.Settings.LookUpEditSettingsBase   DisplayMember$
 Name��   ValueMember$ 	AccountID��   AutoComplete .    AutoPopulateColumns .      IncrementalFiltering$
  TrueX�!   ImmediatePopup! . ;  5DevExpress.Xpf.Editors.Settings.PopupBaseEditSettings"   PopupContentTemplate" �� .R�  $��PART_GridControl��   $ Number��$ 科目代码   $
 Name��$ 科目名称 #   View# #  DevExpress.Xpf.Grid.TableView  $   	AutoWidth$ . &   DevExpress.Xpf.Grid.GridViewBase%   ShowGroupPanel% .  &   DevExpress.Xpf.Grid.DataViewBase&   ShowSearchPanelMode$& Always=�  $ 	Direction��$ 借贷方向 '   AllowEditing$
' True $ False  :  4DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings  -
   $ cboDirection��$
 Name��$ DirectionID��6  0DevExpress.Xpf.Editors.Settings.BaseEditSettings(   StyleSettings( 2  ,DevExpress.Xpf.Editors.ComboBoxStyleSettings    $ 
AmountFrom��$ 金额来源 $
' True $ False    -   $ cboAmountFrom��$ 	FieldName��$ InterID��8  2DevExpress.Xpf.Editors.Settings.ButtonEditSettings)   IsTextEditable$) FalseX� . (     $
 Note��$ 摘要 $
' True $ False  6  0DevExpress.Xpf.Editors.Settings.TextEditSettings    $ 核算项目 $ items��$ False    -   $ 	itemsEdit��*   AllowDefaultButton$* FalseX�+   Buttons+ '  !DevExpress.Xpf.Editors.ButtonInfo  -   #   -   $��
tableView1��,   AllowPerPixelScrolling, . -   ShowTotalSummary- .  % .  .   AllowCascadeUpdate. . /   NewItemRowPosition$/ Bottom=�$ .  0   IndicatorWidth$0 40Q�1   FontSize$1 10Q�2   AllowEditing2 . "3   RowIndicatorContentTemplate#3 [ � $��3��$��10,10q� �� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0��.+��� .��时间�� -   $��txtDate��2019-03-26 12:36:56�� $��1��$ 
Horizontal=�$��Center=�.+��� .��	制单人  -   $��
lookUpUser��$
 Name��$ UserID��4   AutoPopulateColumns4 .  5   AutoComplete5 . 6   IncrementalFiltering$
6 TrueX�7   ImmediatePopup7 . 8   IsPopupAutoWidth8 .  $��Center=�$��80��*  $DevExpress.Xpf.Editors.PopupBaseEdit9   PopupContentTemplate9 �� .R�  $��PART_GridControl��   $ UserID��$ 用户ID   $
 Name��$ 	用户名 #   $ . % .  $& Always=��� $��2��.+��� .��单据类型
  -   $��cboTransType��$	��100��$
 Name��$ ID���� $��3��.+��� .��默认模板
  -   $��
cboDefault��$��40��$
 Name��$ ID��