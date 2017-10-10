using MahApps.Metro.Controls;
using Microsoft.Data.Entity;
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
    /// Interaction logic for RenameCostIncome.xaml
    /// </summary>
    public partial class RenameCostIncome : MetroWindow
    {
        public int ID { get; internal set; }
        public string BaseName { get; internal set; }
        public string PathDataBase { get; internal set; }
        public bool CostOrIncome { get; internal set; }
        public RenameCostIncome()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(1);
            this.BorderBrush = null;
            this.SetResourceReference(MetroWindow.GlowBrushProperty, "AccentColorBrush");

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            #region SQLite
            var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
            optionsBuilder.UseSqlite("Filename=" + PathDataBase + BaseName);

            if (CostOrIncome)
            {
                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    DataBaseUsingEF.Cost cost = (from c in dataBase.Costs where c.Id == ID select c).SingleOrDefault();
                    cost.NameOfCost = textBox.Text;
                    dataBase.SaveChanges();
                }
            }
            else
            {
                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    DataBaseUsingEF.Income income = (from c in dataBase.Incomes where c.Id == ID select c).SingleOrDefault();
                    income.NameOfIncome = textBox.Text;
                    dataBase.SaveChanges();
                }
            }
            Close();
            #endregion


            #region SQLServer
            //if (CostOrIncome)
            //{
            //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(BaseName))
            //    {
            //        DataBaseUsingEF.Cost cost = (from c in dataBase.Costs where c.Id == ID select c).SingleOrDefault();
            //        cost.NameOfCost = textBox.Text;
            //        dataBase.SaveChanges();
            //    }
            //}
            //else
            //{
            //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(BaseName))
            //    {
            //        DataBaseUsingEF.Income income = (from c in dataBase.Incomes where c.Id == ID select c).SingleOrDefault();
            //        income.NameOfIncome = textBox.Text;
            //        dataBase.SaveChanges();
            //    }
            //}
            //Close();
            #endregion
        }
    }
}
