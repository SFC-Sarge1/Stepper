using System.Configuration;
using System.Data;
using System.Windows;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            Stepper.Properties.Settings.Default.Save();
            base.OnExit(e);
        }
    }

}
