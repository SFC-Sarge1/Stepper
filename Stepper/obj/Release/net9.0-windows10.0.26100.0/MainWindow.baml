   M S B A M L   `   `   `  ���� T  PMahApps.Metro, Version=3.0.0.0, Culture=neutral, PublicKeyToken=51482d6f650b2b3fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  I EStepper, Version=3.0.9059.25213, Culture=neutral, PublicKeyToken=null!clr-namespace:StepperStepper    Stepper.MainWindow   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Kmah;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:Stepper       � Title$(  "X,Y, Z Table Stepper Motor Control��$	��411��$	��950�� $   MahApps.Brushes.Border.NonActive#���    � WindowStartupLocation$ CenterScreen=�(   "MahApps.Metro.Controls.MetroWindow   WindowTitleBrush �	#FF060404.���� .+��� ._��  � Orientation$ 
Horizontal=�$��Right=�$	��Top=�.+��� -   $��
VersionTxt��$	��Top=�$��	0,0,20,20q�$
��Left=��� $	��900��$��50��$
��Left=�
�������$��1q� �� CornerRadius$ 10��
���   � �� Padding$	 7,1q�._��� $	��900�� �� Orientation$ 
Horizontal=�$��Center=�$��Center=�.+���  �� Padding$	 7,1q�$��Center=� �� TextAlignment$ Center=�
�������$��X axis Move To:��$	��102���� -   	 ��Name$	 txtXaxisStepperMove��$
��0.00��
 �� TextAlignment$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$
��(mm)��$
��Left=�$��Bottom=��� $	 7,1q�$��Center=�$ Center=�
�������$��X axis Current:���� -   $	 txtXaxisStepperCurrent��$
��0.00��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$
��(mm)��$
��Left=�$��Bottom=��� $	 7,1q�$��Center=�$ Center=�
�������$��X axis Motor Speed:���� -   $	 txtXaxisMotorSpeed��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$��(RPM)��$
��Left=�$��Bottom=��� -    ��Name$ ckbXaxisResetToZero��$��Right=�$��X axis Zero��+   %MahApps.Metro.Controls.CheckBoxHelper"   CheckGlyphForegroundChecked
 ������� $	��900��$��50��$
��Left=�
�������$��1q�$ 10��
���   �$	 7,1q�._��� $	��900��$ 
Horizontal=�$��Center=�$��Center=�.+��� $	 7,1q�$��Center=�$ Center=�
�������$��Y axis Move To:��$	��102���� -   $	 txtYaxisStepperMove��$
��0.00��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$
��(mm)��$
��Left=�$��Bottom=��� $	 7,1q�$��Center=�$ Center=�
�������$��Y axis Current:���� -   $	 txtYaxisStepperCurrent��$
��0.00��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$
��(mm)��$
��Left=�$��Bottom=��� $	 7,1q�$��Center=�$ Center=�
�������$��Y axis Motor Speed:���� -   $	 txtYaxisMotorSpeed��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$��(RPM)��$
��Left=�$��Bottom=��� -	   $ ckbYaxisResetToZero��$��Right=�$��Y axis Zero��
 ������� $	��900��$��50��$
��Left=�
�������$��1q�$ 10��
���   �$	 7,1q�._��� $	��900��$ 
Horizontal=�$��Center=�$��Center=�.+��� $	 7,1q�$��Center=�$ Center=�
�������$��Z axis Move To:��$	��102���� -
   $	 txtZaxisStepperMove��$
��0.00��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$
��(mm)��$
��Left=�$��Bottom=��� $	 7,1q�$��Center=�$ Center=�
�������$��Z axis Current:���� -   $	 txtZaxisStepperCurrent��$
��0.00��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$
��(mm)��$
��Left=�$��Bottom=��� $	 7,1q�$��Center=�$ Center=�
�������$��Z axis Motor Speed:���� -   $	 txtZaxisMotorSpeed��$
 Right=�$��Right=�$��Bottom=�$	��7,1q�$	��100���� $	��7,1q�$��(RPM)��$
��Left=�$��Bottom=��� -   $ ckbZaxisResetToZero��$��Right=�$��Z axis Zero��
 ������� $	��900��$��92��$
��Left=�
�������$��1q�$ 10��
���   �$	 7,1q�$	��Top=�._��� $	��900��$ 
Horizontal=�.+��� $	��150��$ Vertical=�$��Center=�$��Center=�.+��� -    ��Name$ btnRunXAxis��$��	10,0,10,0q�$	��7,1q�$��
Run X Axis��$	��Top=�$	��120��$��34��$��Center=��� -   $ btnXAxisPort��$��	10,0,10,0q�$	��7,1q�$��X Axis Port��$	��Top=�$	��120��$��34��$��Center=��� $	��150��$ Vertical=�$��Center=�$��Center=�.+��� -   $ btnRunYAxis��$��	10,0,10,0q�$	��7,1q�$��
Run Y Axis��$	��Top=�$	��120��$��34��$��Center=��� -   $ btnYAxisPort��$��	10,0,10,0q�$	��7,1q�$��Y Axis Port��$	��Top=�$	��120��$��34��$��Center=��� $	��150��$ Vertical=�$��Center=�$��Center=�.+��� -   $ btnRunZAxis��$��	10,0,10,0q�$	��7,1q�$��
Run Z Axis��$	��Top=�$	��120��$��34��$��Center=��� -   $ btnZAxisPort��$��	10,0,10,0q�$	��7,1q�$��Z Axis Port��$	��Top=�$	��120��$��34��$��Center=��� $	��150��$ Vertical=�$��Center=�$��Center=�.+��� -   $ btnRunXYAxis��$��	10,0,10,0q�$	��7,1q�$��Run X,Y Axis��$	��Top=�$	��120��$��34��$��Center=��� -   $ btnAppSettings��$��	10,0,10,0q�$	��7,1q�$��App Settings��$	��Top=�$	��120��$��34��$��Center=��� $	��900��$��54��$
��Left=�
�������$��1q�$ 10��
���   �$	 7,1q�._��� $	��900��$ 
Horizontal=�.+��� -   $��CountdownLabel��$	��7,1q�$	��500��$��22(�$�� ��$
��Left=�$��Center=�