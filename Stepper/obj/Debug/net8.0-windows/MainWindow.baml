   M S B A M L   `   `   `  ����T  PMahApps.Metro, Version=2.0.0.0, Culture=neutral, PublicKeyToken=51482d6f650b2b3fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5   	   I EStepper, Version=3.0.8982.25215, Culture=neutral, PublicKeyToken=null!clr-namespace:StepperStepper 5   	      Stepper.MainWindow   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Kmah;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:Stepper     5   	     � Title$(  "X,Y, Z Table Stepper Motor Control��5	   	   $	��411��64   $	��950��6A    $   MahApps.Brushes.Border.NonActive#���   6M    � WindowStartupLocation$ CenterScreen=�6�   (   "MahApps.Metro.Controls.MetroWindow   WindowTitleBrush �	#FF060404.��5      �� .+�5   
   �� ._�5      �  � Orientation$ 
Horizontal=�6   $��Right=�61   $	��Top=�6M   .+�5      �� -   5        $��
VersionTxt��5   c   $	��Top=�6   $��	0,0,20,20q�64   $
��Left=�6H   6w   5      5      �� 5   
   $	��900��6   $��50��6   $
��Left=�6)   
�������$��1q�6W    �� CornerRadius$ 10��6k   
���   � �� Padding$	 7,1q�6�   ._�5      �� $	��900��6    �� Orientation$ 
Horizontal=�6%   $��Center=�6>   $��Center=�6[   .+�5      ��  �� Padding$	 7,1q�6   $��Center=�6*    �� TextAlignment$ Center=�6E   
�������$��X axis Move To:��6o   $	��102��6�   �� 5      -   5        	 ��Name$	 txtXaxisStepperMove��5   s   $
��0.00��6   
 �� TextAlignment$
 Right=�6&   $��Right=�6<   $��Bottom=�6X   $	��7,1q�6�   $	��100��6�   6N  �� 5      $	��7,1q�6   $
��(mm)��6&   $
��Left=�65   $��Bottom=�6P   �� 5      $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��X axis Current:��6o   �� 5      -   5        $	 txtXaxisStepperCurrent��5   s   $
��0.00��6   $
 Right=�6&   $��Right=�6<   $��Bottom=�6X   $	��7,1q�6�   $	��100��6�   6  �� 5      $	��7,1q�6   $
��(mm)��6&   $
��Left=�65   $��Bottom=�6P   �� 5      $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��X axis Motor Speed:��6o   �� 5      -   5        $	 txtXaxisMotorSpeed��5   u   $��400.00��6   $
 Right=�6(   $��Right=�6>   $��Bottom=�6Z   $	��7,1q�6�   $	��100��6�   6-  �� 5      $	��7,1q�6   $��(RPM)��6&   $
��Left=�66   $��Bottom=�6Q   �� 5       -   5         ��Name$ ckbXaxisResetToZero��5    7   $��Right=�6   $��X axis Zero��6r   +   %MahApps.Metro.Controls.CheckBoxHelper"   CheckGlyphForegroundChecked
 �����6�   5!      5"      �� 5$   
   $	��900��6   $��50��6   $
��Left=�6)   
�������$��1q�6W   $ 10��6k   
���   �$	 7,1q�6�   ._�5%      �� $	��900��6   $ 
Horizontal=�6%   $��Center=�6>   $��Center=�6[   .+�5&      �� $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��Y axis Move To:��6o   $	��102��6�   �� 5'      -   5        $	 txtYaxisStepperMove��5'   s   $
��0.00��6   $
 Right=�6&   $��Right=�6<   $��Bottom=�6X   $	��7,1q�6�   $	��100��6�   6N  �� 5(      $	��7,1q�6   $
��(mm)��6&   $
��Left=�65   $��Bottom=�6P   �� 5*      $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��Y axis Current:��6o   �� 5+      -   5        $	 txtYaxisStepperCurrent��5+   s   $
��0.00��6   $
 Right=�6&   $��Right=�6<   $��Bottom=�6X   $	��7,1q�6�   $	��100��6�   6  �� 5,      $	��7,1q�6   $
��(mm)��6&   $
��Left=�65   $��Bottom=�6P   �� 5.      $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��Y axis Motor Speed:��6o   �� 5/      -   5        $	 txtYaxisMotorSpeed��5/   u   $��400.00��6   $
 Right=�6(   $��Right=�6>   $��Bottom=�6Z   $	��7,1q�6�   $	��100��6�   6-  �� 50      $	��7,1q�6   $��(RPM)��6&   $
��Left=�66   $��Bottom=�6Q   �� 52      -	   5        $ ckbYaxisResetToZero��52   7   $��Right=�6   $��Y axis Zero��6r   
 �����6�   53      54      �� 56   
   $	��900��6   $��50��6   $
��Left=�6)   
�������$��1q�6W   $ 10��6k   
���   �$	 7,1q�6�   ._�57      �� $	��900��6   $ 
Horizontal=�6%   $��Center=�6>   $��Center=�6[   .+�58      �� $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��Z axis Move To:��6o   $	��102��6�   �� 59      -
   5        $	 txtZaxisStepperMove��59   t   $
��0.00��6   $
 Right=�6&   $��Right=�6=   $��Bottom=�6Y   $	��7,1q�6�   $	��100��6�   6O  �� 5:      $	��7,1q�6   $
��(mm)��6&   $
��Left=�65   $��Bottom=�6P   �� 5<      $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��Z axis Current:��6o   �� 5=      -   5        $	 txtZaxisStepperCurrent��5=   s   $
��0.00��6   $
 Right=�6&   $��Right=�6<   $��Bottom=�6X   $	��7,1q�6�   $	��100��6�   6  �� 5>      $	��7,1q�6   $
��(mm)��6&   $
��Left=�65   $��Bottom=�6P   �� 5@      $	 7,1q�6   $��Center=�6*   $ Center=�6E   
�������$��Z axis Motor Speed:��6o   �� 5A      -   5        $	 txtZaxisMotorSpeed��5A   u   $��400.00��6   $
 Right=�6(   $��Right=�6>   $��Bottom=�6Z   $	��7,1q�6�   $	��100��6�   6-  �� 5B      $	��7,1q�6   $��(RPM)��6&   $
��Left=�66   $��Bottom=�6Q   �� 5D      -   5        $ ckbZaxisResetToZero��5D   7   $��Right=�6   $��Z axis Zero��6r   
 �����6�   5E      5F      �� 5H   
   $	��900��6   $��92��6   $
��Left=�6)   
�������$��1q�6W   $ 10��6k   
���   �$	 7,1q�6�   $	��Top=�6�   ._�5I      �� $	��900��6   $ 
Horizontal=�6%   .+�5J      �� $	��150��6   $ Vertical=�6)   $��Center=�6@   $��Center=�6]   .+�5K      �� -   5         ��Name$ btnRunXAxis��5K   >   $��	10,0,10,0q�6   $	��7,1q�60   $��
Run X Axis��6S   $	��Top=�6h   $	��120��6�   $��34��6�   $��Center=�6�   �� 5L      -   5        $ btnXAxisPort��5L   >   $��	10,0,10,0q�6   $	��7,1q�60   $��X Axis Port��6T   $	��Top=�6j   $	��120��6�   $��34��6�   $��Center=�6�   5M      �� 5N      $	��150��6   $ Vertical=�6)   $��Center=�6@   $��Center=�6]   .+�5O      �� -   5        $ btnRunYAxis��5O   ?   $��	10,0,10,0q�6   $	��7,1q�61   $��
Run Y Axis��6T   $	��Top=�6i   $	��120��6�   $��34��6�   $��Center=�6�   �� 5P      -   5        $ btnYAxisPort��5P   ?   $��	10,0,10,0q�6   $	��7,1q�61   $��Y Axis Port��6U   $	��Top=�6k   $	��120��6�   $��34��6�   $��Center=�6�   5Q      �� 5S      $	��150��6   $ Vertical=�6)   $��Center=�6@   $��Center=�6]   .+�5T      �� -   5        $ btnRunZAxis��5T   ?   $��	10,0,10,0q�6   $	��7,1q�61   $��
Run Z Axis��6T   $	��Top=�6i   $	��120��6�   $��34��6�   $��Center=�6�   �� 5U      -   5        $ btnZAxisPort��5U   ?   $��	10,0,10,0q�6   $	��7,1q�61   $��Z Axis Port��6U   $	��Top=�6k   $	��120��6�   $��34��6�   $��Center=�6�   5V      �� 5X      $	��150��6   $ Vertical=�6)   $��Center=�6@   $��Center=�6]   .+�5Y      �� -   5        $ btnRunXYAxis��5Y   >   $��	10,0,10,0q�6   $	��7,1q�60   $��Run X,Y Axis��6T   $	��Top=�6k   $	��120��6�   $��34��6�   $��Center=�6�   �� 5Z      -   5        $ btnAppSettings��5Z   >   $��	10,0,10,0q�6   $	��7,1q�60   $��App Settings��6V   $	��Top=�6m   $	��120��6�   $��34��6�   $��Center=�6�   5[      5]      5^      �� 5`   
   $	��900��6   $��54��6   $
��Left=�6)   
�������$��1q�6W   $ 10��6k   
���   �$	 7,1q�6�   ._�5a      �� $	��900��6   $ 
Horizontal=�6%   .+�5h      �� -   5        $��CountdownLabel��5h   &   $	��7,1q�6   $	��500��6<   $��22(�6H   $�� ��6V   $
��Left=�6a   $��Center=�6|   5i      5j      5k      5l      