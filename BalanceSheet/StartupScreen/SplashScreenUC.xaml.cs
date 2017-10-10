using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Deployment.Application;
using System.Reflection;

namespace BalanceSheet.StartupScreen
{
    public class WindowSplashScreen : Window
    { }
    /// <summary>
    /// Логика взаимодействия для SplashScreenUC.xaml
    /// </summary>
    public partial class SplashScreenUC
    {
        public static readonly DependencyProperty LicenseeProperty =
            DependencyProperty.Register("Licensee", typeof(string), typeof(SplashScreenUC), new UIPropertyMetadata(null));
        public SplashScreenUC()
        {
            this.InitializeComponent();
            textBlockVersion.Text = "version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        //Лицензия для отображения в SplashScreen
        public string Licensee
        {
            get { return (string)this.GetValue(LicenseeProperty); }
            set { this.SetValue(LicenseeProperty, value); }
        }
    }
}
