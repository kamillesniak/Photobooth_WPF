   M S B A M L   `   `   `  ����H  DPhotoboothWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:PhotoboothWpfPhotoboothWpf  5   	   #    PhotoboothWpf.EmailSendDialog   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:PhotoboothWpf     5   	   -   5          � Title$  Email Dialog��5   	    � SizeToContent$ WidthAndHeight=�6    � WindowStartupLocation$ CenterScreen=�6=   .��5
      � $��15q�6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��labelEmailQuestion��5      $��1��6*   .��6:   	Question:6E   �� 5   
   -   5        $��textBoxEmailAnswer��5      $��1��6,   $��1��6<   $	��250��6I   .��6X   Answer6`   � 5   
   $��2��6   $��2��6!   $��Right=�65   $��0,15,0,0q�6Q   .+�5      �� -   5        $��btnDialogOk��5   &    �� 	IsDefault . $��80��6S   $��0,0,10,0q�6a   .��6s   _Ok6x   �� 5       �� IsCancel . $��80��6%   .��63   	_Cancel6<   5      5      5      