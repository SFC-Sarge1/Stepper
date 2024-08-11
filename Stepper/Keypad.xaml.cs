// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 02-24-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 07-24-2024
// ***********************************************************************
// <copyright file="Keypad.xaml.cs" company="Stepper">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using MahApps.Metro.Controls;
using System.Reflection;

namespace Stepper
{
    /// <summary>
    /// Class Keypad.
    /// Implements the <see cref="MetroWindow" />
    /// Implements the <see cref="INotifyPropertyChanged" />
    /// Implements the <see cref="System.Windows.Markup.IComponentConnector" />
    /// </summary>
    /// <seealso cref="MetroWindow" />
    /// <seealso cref="INotifyPropertyChanged" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class Keypad : MetroWindow, INotifyPropertyChanged
    {

        /// <summary>
        /// The result
        /// </summary>
        private string _result;
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public string Result
        {
            get { return _result; }
            private set { _result = value; this.OnPropertyChanged("Result"); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Keypad" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public Keypad(Window owner)
        {
            InitializeComponent();
            //this.Owner = owner;
            //this.DataContext = this;            
        }

        /// <summary>
        /// Handles the Click event of the button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            HandleButtonTouchClick(sender);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="info">The information.</param>
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// Handles the TouchUp event of the button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.TouchEventArgs" /> instance containing the event data.</param>
        private void button_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            HandleButtonTouchClick(sender);
        }
        /// <summary>
        /// Handles the button touch click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void HandleButtonTouchClick(object sender)
        {
            Button button = sender as Button;
            switch (button.CommandParameter.ToString())
            {
                case "ESC":
                    DialogResult = false;
                    break;

                case "RETURN":
                    DialogResult = true;
                    break;

                case "BACK":
                    if (Result.Length > 0)
                        Result = Result.Remove(Result.Length - 1);
                    break;

                default:
                    Result += button.Content.ToString();
                    break;
            }

        }
    }
}
