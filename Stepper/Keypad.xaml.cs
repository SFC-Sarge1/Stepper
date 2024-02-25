using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using MahApps.Metro.Controls;

namespace Stepper
{
    public partial class Keypad : MetroWindow, INotifyPropertyChanged
    {

        private string _result;
        public string Result
        {
            get { return _result; }
            private set { _result = value; this.OnPropertyChanged("Result"); }
        }

        public Keypad(Window owner)
        {
            InitializeComponent();
            //this.Owner = owner;
            //this.DataContext = this;            
        }        

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.CommandParameter.ToString()) 
            {
                case "ESC":
                    this.DialogResult = false;
                    break;

                case "RETURN":
                    this.DialogResult = true;
                    break;

                case "BACK":
                    //if (Result.Length > 0)
                    //    Result = Result.Remove(Result.Length - 1);
                    break;

                default:
                    Result += button.Content.ToString();
                    break;
            }   
        }    

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
