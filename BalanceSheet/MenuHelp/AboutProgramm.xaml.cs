using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Reflection;
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

namespace BalanceSheet.MenuHelp
{
    /// <summary>
    /// Interaction logic for AboutProgramm.xaml
    /// </summary>
    public partial class AboutProgramm : MetroWindow
    {
        public AboutProgramm()
        {
            InitializeComponent();
            textBlock.Text = "version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
