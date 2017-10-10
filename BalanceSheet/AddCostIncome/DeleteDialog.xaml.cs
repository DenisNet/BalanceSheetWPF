using MahApps.Metro.Controls;
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

namespace BalanceSheet.AddCostIncome
{
    /// <summary>
    /// Interaction logic for DeleteDialog.xaml
    /// </summary>
    public partial class DeleteDialog : MetroWindow
    {
        public bool Ergebnis { get; internal set; }
        public DeleteDialog()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(1);
            this.BorderBrush = null;
            this.SetResourceReference(MetroWindow.GlowBrushProperty, "AccentColorBrush");

        }

        private void buttonYes_Click(object sender, RoutedEventArgs e)
        {
            Ergebnis = true;
            Close();
        }

        private void buttonNo_Click(object sender, RoutedEventArgs e)
        {
            Ergebnis = false;
            Close();
        }
    }
}
