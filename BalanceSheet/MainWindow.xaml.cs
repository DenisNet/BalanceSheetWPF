using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#region for SQLServer
//using System.Data.Entity;
#endregion

using System.Threading;
using ScottLogic.Shapes;
using System.Windows.Media.Effects;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using System.ComponentModel;
using Microsoft.Data.Entity;
using System.Globalization;
using System.Windows.Media;

namespace BalanceSheet
{
    /// <summary>
    /// Логика взаимодействия для Приложения
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //private DataBaseUsingEF.DataBaseUsingEF dataBase;
        private CollectionViewSource costsViewSource;
        private CollectionViewSource incomesViewSource;
        private static string baseName;
        //Для смены отображения расходов и доходов Click
        private bool change;
        static ComboBox copyComboBoxMonatName;
        private static string path = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\BalanceSheet\";
        private static string userName;


        public MainWindow()
        {
            InitializeComponent();
            this.BorderThickness = new Thickness(1);
            this.BorderBrush = null;
            this.SetResourceReference(MetroWindow.GlowBrushProperty, "AccentColorBrush");
        }

        bool exitIndex;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlurEffect effect = new BlurEffect();
            effect.Radius = 25;
            mainGrid.Effect = effect;
            //Загружаем дату сегодня
            DateHeute.Content = DateTime.Today.ToShortDateString();
            //Показать все элементы
            MenuSP.Visibility = Visibility.Visible;
            DateLabel.Visibility = Visibility.Visible;
            AddCosts.Visibility = Visibility.Visible;
            lMonat.Visibility = Visibility.Visible;
            monatName.Visibility = Visibility.Visible;
            copyComboBoxMonatName = monatName;
            await Authentification();
            if (exitIndex)
            {
                effect.Radius = 25;
                mainGrid.Effect = effect;
            }
            else
            {
                effect.Radius = 0;
                mainGrid.Effect = effect;
            }
        }

        #region Dialog Authentification

        string tempLogin, tempPassword;
        private async Task Authentification()
        {
            var metroWindow = new MetroWindow();
            metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            LoginDialogData result = await this.ShowLoginAsync("Benutzerauthentifizierung", "Bitte, geben Sie Ihren Benutzernamen und Passwort ein: ", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, EnablePasswordPreview = true, NegativeButtonVisibility = Visibility.Visible, NegativeButtonText = "Beenden", RegistrationButtonText = "Registrieren", RegistrationButtonVisibility = Visibility.Visible, RememberCheckBoxVisibility = Visibility.Visible });
            //!!!!!
            metroProgres.Visibility = Visibility.Visible;
            if (result.ButtonAction == null)
            {
                exitIndex = true;
                App_Closing();
            }
            else if (result.ButtonAction == 1)
            {
                tempLogin = result.Username;
                tempPassword = result.Password;
                exitIndex = false;
                userName = result.Username;
                baseName = result.Username + "_BalanceSheet.dbo";
                bool resultLogin = await LoginDataBaseExist(result.Username, result.Password);
                if (resultLogin)
                {
                    baseName = result.Username + "_BalanceSheet.dbo";
                    //await Login(result.Username, result.Password);
                    monatName.SelectedIndex = DateTime.Now.Month - 1;
                    //Можно внести потом последнее время входа!!!!!!!!!!!

                    //Можно сделать разным цвет входа в программу
                    //var mySettings = new MetroDialogSettings()
                    //{
                    //    ColorScheme = MetroDialogColorScheme.Theme
                    //};

                    MessageDialogResult messageResult = await this.ShowMessageAsync("Herzlich willkommen " + result.Username, "");
                    metroProgres.Visibility = Visibility.Collapsed;
                    BlurEffect effect = new BlurEffect();
                    effect.Radius = 0;
                    mainGrid.Effect = effect;
                }
                else
                {
                    MessageDialogResult messageResult = await this.ShowMessageAsync("Falscher Benutzername oder Passwort. Überprüfen Sie bitte die eingegebenen Daten.", "");
                    await Authentification();
                }
            }
            else if (result.ButtonAction == 2)
            {
                exitIndex = false;
                baseName = result.Username + "_BalanceSheet.dbo";
                if (!LoginDataBaseExist(result.Username, result.Password).Result)
                {
                    userName = result.Username;
                    baseName = result.Username + "_BalanceSheet.dbo";
                    metroProgres.Visibility = Visibility.Visible;
                    await RegistrationAsync(result.Username, result.Password);
                    metroProgres.Visibility = Visibility.Collapsed;
                    //Можно внести потом последнее время входа!!!!!!!!!!!
                    MessageDialogResult messageResult = await this.ShowMessageAsync("Die Registrierung des Benuzers mit dem Namen " + result.Username + " ist beendet.", "Bitte melden Sie sich mit Ihrem Login und Passwort an.");
                    
                    await Authentification();
                }
                else
                {
                    MessageDialogResult messageResult = await this.ShowMessageAsync("Bitte geben Sie einen anderen Benutzernamen ein.", "");
                    await Authentification();
                }
            }

        }
        #endregion

        #region Вход пользователя
        //private async Task Login(string login, string password)
        //{
        //    #region SQLite
        //    ////Входим в базу данных
        //    //using (var dataBase = new DataBaseUsingEF.DataBaseInFile())
        //    //{
        //    //    //Generics von String где находятся все найденные элементы
        //    //    IQueryable<string> balSheet =
        //    //        from item in dataBase.Logins
        //    //        where item.Login1 == login && item.Password == password
        //    //        select item.Login1;
        //    //    if (balSheet.GetEnumerator().MoveNext())
        //    //    {
        //    //        this.Title = "BalanceSheet " + login;

        //    //    }
        //    //    //Если не правильное имя или пароль выдать сообщение
        //    //    else
        //    //    {
        //    //    }
        //    //}
        //    #endregion

        //    #region SQLServer
        //    //Входим в базу данных
        //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
        //    {
        //        //Generics von String где находятся все найденные элементы
        //        IQueryable<string> balSheet =
        //            from item in dataBase.Logins
        //            where item.Login1 == login && item.Password == password
        //            select item.Login1;
        //        if (balSheet.GetEnumerator().MoveNext())
        //        {
        //            this.Title = "BalanceSheet " + login;
                    
        //        }
        //        //Если не правильное имя или пароль выдать сообщение
        //        else
        //        {
        //        }
        //    }
        //    #endregion
        //}
        
        //Проверяем существует ли данная база данных и пароль
        private async Task<bool> LoginDataBaseExist(string login, string password)
        {
            #region SQLite

            bool exist = false;
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + login + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    //Если базы данных существует
                    try
                    {
                        //Generics von String где находятся все найденные элементы
                        IQueryable<string> balSheet =
                            from item in dataBase.Logins
                            where item.Login1 == login && item.Password == password
                            select item.Login1;
                        if (balSheet.GetEnumerator().MoveNext())
                        {
                            exist = true;
                        }
                    }
                    catch (Exception)
                    {
                        exist = false;
                    }
                    return exist;
                }

            }
            catch (Exception e)
            {
                //?????????????
                throw;
            }
            #endregion

            #region SQLServer
            ////SQLServer
            //bool exist = false;
            //try
            //{
            //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        Task<bool> task1 = Task<bool>.Factory.StartNew(() =>
            //        {
            //            //Если базы данных существует
            //            if (dataBase.Database.Exists())
            //            {
            //                //Generics von String где находятся все найденные элементы

            //                IQueryable<string> balSheet =
            //                    from item in dataBase.Logins
            //                    where item.Login1 == login && item.Password == password
            //                    select item.Login1;
            //                if (balSheet.GetEnumerator().MoveNext())
            //                {
            //                    exist = true;
            //                }
            //                else
            //                {
            //                    exist = false;
            //                }
            //            }
            //            return exist;
            //        });
            //        return await task1;
            //    }

            //}
            //catch (Exception e)
            //{
            //    //?????????????
            //    throw;
            //}
            #endregion
        }

        #endregion


        #region Асинхронная регистрация
        private async Task RegistrationAsync(string name, string password)
        {
            #region SQLite
            //Создаем базу данных
            //Entity Framwork + LINQ
            //Запускаем параллельный поток для длитетельной оперции
            //создания базы данных  и регистрации пользователя
            try
            {
                await Task.Factory.StartNew(() =>
                { 
                    string directoryName = Directory.CreateDirectory(path + name).ToString();

                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + directoryName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        //записываем введенные значения
                        var login = new DataBaseUsingEF.Login
                        {
                            Id = 1,
                            Login1 = name,
                            Password = password,
                            DateOfModify = DateTime.Now.ToShortDateString()
                        };
                        dataBase.Database.EnsureCreated();
                        dataBase.Logins.Add(login);
                        dataBase.SaveChanges();
                    };
                });
            }
            catch (Exception em)
            {
                MessageBox.Show(em.Message);
            }
            #endregion


            #region SQLServer
            //Создаем базу данных
            //Entity Framwork + LINQ
            //Запускаем параллельный поток для длитетельной оперции
            //создания базы данных  и регистрации пользователя
            //try
            //{
            //    await Task.Factory.StartNew(() =>
            //    {
            //        using (var database = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //        {
            //            //записываем введенные значения
            //            var login = new DataBaseUsingEF.Login
            //            {
            //                Id = 1,
            //                Login1 = name,
            //                Password = password,
            //                DateOfModify = DateTime.Now.ToShortDateString()
            //            };
            //            database.Database.Create();
            //            database.Logins.Add(login);
            //            database.SaveChanges();
            //        };
            //    });

            //}
            //catch (Exception)
            //{
            //    ////????????????????
            //    throw;
            //}
            #endregion
        }

        #endregion


        #region MenuEdit
        private void MenuEditIsEnable()
        {
            if (change)
            {
                if (dataGridIncomes.Items.Count == 0)
                {
                    MenuEditNotEnable();
                }
                else
                {
                    MenuEditEnable();
                }
            }
            else
            {
                if (dataGridCosts.Items.Count == 0)
                {
                    MenuEditNotEnable();
                }
                else
                {
                    MenuEditEnable();
                }
            }
        }

        private void MenuEditEnable()
        {
            //Main Menu
            menuRename.IsEnabled = true;
            menuDelete.IsEnabled = true;
            //Context Menu
            menuContextCostRename.IsEnabled = true;
            menuContextCostDelete.IsEnabled = true;
            menuContextIncomeRename.IsEnabled = true;
            menuContextIncomeDelete.IsEnabled = true;
        }

        private void MenuEditNotEnable()
        {
            //Main Menu
            menuDelete.IsEnabled = false;
            menuRename.IsEnabled = false;
            //Context Menu
            menuContextCostRename.IsEnabled = false;
            menuContextCostDelete.IsEnabled = false;
            menuContextIncomeRename.IsEnabled = false;
            menuContextIncomeDelete.IsEnabled = false;

        }

        #endregion

        int tempMonth = 0;
        int tempMonthIncome = 0;
        bool add = true;
        bool yearIndex = true;
        //Создаем данные для диаграммы
        private void CreateDateDiagramm()
        {
            if (change)
            {
                #region Month
                ObservableCollection<ErgebnisDiagramm> diagrammMonth = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammIncomesMonth());
                gridGroup0.DataContext = diagrammMonth;
                #endregion

                #region Quartal
                if (copyComboBoxMonatName.SelectedIndex + 1 <= 3)
                {
                    int quartalNumm = 1;
                    if (tempMonthIncome != quartalNumm || add)
                    {

                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammIncomesQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonthIncome = quartalNumm;
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 4 && copyComboBoxMonatName.SelectedIndex + 1 <= 6)
                {
                    int quartalNumm = 2;
                    if (tempMonthIncome != quartalNumm || add)
                    {

                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammIncomesQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonthIncome = quartalNumm;
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 7 && copyComboBoxMonatName.SelectedIndex + 1 <= 9)
                {
                    int quartalNumm = 3;
                    if (tempMonthIncome != quartalNumm || add)
                    {

                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammIncomesQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonthIncome = quartalNumm;
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 10 && copyComboBoxMonatName.SelectedIndex + 1 <= 12)
                {
                    int quartalNumm = 4;
                    if (tempMonthIncome != quartalNumm || add)
                    {

                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammIncomesQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonthIncome = quartalNumm;
                }
                #endregion

                #region Year
                if (yearIndex)
                {

                    ObservableCollection<ErgebnisDiagramm> diagrammYear = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammIncomesYear());
                    gridGroup2.DataContext = diagrammYear;
                }
                #endregion
            }
            else
            {
                #region Month
                ObservableCollection<ErgebnisDiagramm> diagrammMonth = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammCostsMonth());
                gridGroup0.DataContext = diagrammMonth;
                #endregion

                #region Quartal
                if (copyComboBoxMonatName.SelectedIndex + 1 <= 3)
                {
                    int quartalNumm = 1;
                    if (tempMonth != quartalNumm || add)
                    {
                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammCostsQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonth = quartalNumm;
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 4 && copyComboBoxMonatName.SelectedIndex + 1 <= 6)
                {
                    int quartalNumm = 2;
                    if (tempMonth != quartalNumm || add)
                    {
                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammCostsQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonth = quartalNumm;
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 7 && copyComboBoxMonatName.SelectedIndex + 1 <= 9)
                {
                    int quartalNumm = 3;
                    if (tempMonth != quartalNumm || add)
                    {
                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammCostsQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonth = quartalNumm;
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 10 && copyComboBoxMonatName.SelectedIndex + 1 <= 12)
                {
                    int quartalNumm = 4;
                    if (tempMonth != quartalNumm || add)
                    {
                        ObservableCollection<ErgebnisDiagramm> diagrammQuartal = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammCostsQuartal());
                        gridGroup1.DataContext = diagrammQuartal;
                        add = false;
                    }
                    tempMonth = quartalNumm;
                }
                #endregion

                #region Year
                if (yearIndex)
                {
                    ObservableCollection<ErgebnisDiagramm> diagrammYear = new ObservableCollection<ErgebnisDiagramm>(ErgebnisDiagramm.ConstructionDiagrammCostsYear());
                    gridGroup2.DataContext = diagrammYear;
                    yearIndex = false;
                }
                #endregion
            }
        }


        #region Click_Добавить расходы и доходы

        //Добавить расходы
        private async void AddCosts_Click(object sender, RoutedEventArgs e)
        {
            AddCostIncome.Add addCost = new AddCostIncome.Add();
            addCost.Title = "Ausgabe hinzufügen";
            addCost.DataBaseIn(baseName, path + userName);
            addCost.Owner = this;
            //Загружаем окно для добавления, после закрытия поток продолжает работу в этом методе
            addCost.ShowDialog();
            //Если отмена тогда закрываем окно
            if (addCost.DialogResult == false)
            {
                return;
            }
            add = true;
            yearIndex = true;
            await UpdateCostViewDataGridAsync();
        }

        //Добавить доходы
        private async void AddIncome_Click(object sender, RoutedEventArgs e)
        {
            AddCostIncome.Add addIncome = new AddCostIncome.Add();
            addIncome.Title = "Einkommen hinzufügen";
            addIncome.DataBaseIn(baseName, path + userName);
            addIncome.Owner = this;
            //Загружаем окно для добавления, после закрытия поток продолжает работу в этом методе
            addIncome.ShowDialog();
            //Если отмена тогда закрываем окно
            if (addIncome.DialogResult == false)
            {
                return;
            }
            yearIndex = true;
            add = true;
            await UpdateIncomeViewDataGridAsync();
        }

        #endregion

        #region Обновляем таблицу из базы данных расходов и доходов 

        //Обновление datagrid из базы данных расходов
        private async Task UpdateCostViewDataGridAsync()
        {
            #region SQLite
            try
            {
                int year = DateTime.Now.Year;
                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                Task<decimal?> task1 = Task<decimal?>.Factory.StartNew((mon) =>
                {

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        var summe =
                         (from summ in dataBase.Costs
                          where summ.DateOfCost.Month == (int)mon && summ.DateOfCost.Year == year
                          select (decimal?)Decimal.Parse(summ.PreisOfCost, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"))/*Convert.ToDecimal(summ.PreisOfCost, CultureInfo.CreateSpecificCulture("DE-de"))*/).SumAsync();
                        return summe.Result;
                    }
                }, monatName.SelectedIndex + 1);
                Task.WaitAll(task1);

                costsViewSource = ((CollectionViewSource)(this.FindResource("costsViewSource")));
                //Загружаем базу данных выбранного месяца
                var optionsBuilder1 = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder1.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder1.Options))
                {
                    var query =
                    from cost in dataBase.Costs
                    where cost.DateOfCost.Month == monatName.SelectedIndex + 1 && cost.DateOfCost.Year == DateTime.Now.Year
                    orderby cost.DateOfCost descending
                    select cost;
                    await query.LoadAsync();
                    costsViewSource.Source = query;
                }
                if (task1.Result == null)
                {
                    TextBlockWithCostPreisSumm(task1.Result);
                }
                else
                {
                    TextBlockWithCostPreisSumm(task1.Result);
                }
                //Создаем данные для диаграммы
                CreateDateDiagramm();
                MenuEditIsEnable();
                await UpdateIncomeViewDataGridAsync();
            }
            catch(Exception em)
            {
                MessageBox.Show(em.Message);
            }
            decimal tempIncome = Decimal.Parse(textBlockIncome.Text, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"));
            decimal tempCost = Decimal.Parse(textBlockCost.Text, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"));
            if (tempIncome - tempCost > 0)
            {
                labelBalance.Content = "+ " + (tempIncome - tempCost).ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));
                labelBalance.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                labelBalance.Content = (tempIncome - tempCost).ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));
                labelBalance.Foreground = new SolidColorBrush(Colors.Red);
            }
            #endregion

            #region SQLServer
            //int year = DateTime.Now.Year;
            //Task<decimal?> task1 = Task<decimal?>.Factory.StartNew((mon) =>
            //{
            //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        var summe =
            //         (from summ in dataBase.Costs
            //          where summ.DateOfCost.Month == (int)mon && summ.DateOfCost.Year == year
            //          select (decimal?)summ.PreisOfCost).SumAsync();
            //        return summe.Result;
            //    }
            //}, monatName.SelectedIndex + 1);
            //Task.WaitAll(task1);

            //costsViewSource = ((CollectionViewSource)(this.FindResource("costsViewSource")));
            ////Загружаем базу данных выбранного месяца
            //using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //{
            //    var query =
            //    from cost in dataBase.Costs
            //    where cost.DateOfCost.Month == monatName.SelectedIndex + 1 && cost.DateOfCost.Year == DateTime.Now.Year
            //    orderby cost.DateOfCost descending
            //    select cost;
            //    query.Load();

            //    if (task1.Result == null)
            //    {
            //        textBlockCost.Text = "0 €";
            //    }
            //    else
            //    {
            //        textBlockCost.Text = task1.Result.ToString() + " €";
            //    }
            //    costsViewSource.Source = dataBase.Costs.Local;
            //}
            ////Создаем данные для диаграммы
            //CreateDateDiagramm();
            //MenuEditIsEnable();
            #endregion
        }

        void TextBlockWithCostPreisSumm(decimal? result)
        {
            decimal tempPreisDecimal = Convert.ToDecimal(result);
            //Add Euro to end
            textBlockCost.Text = tempPreisDecimal.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));
        }

        //Обновление datagrid из базы данных доходов
        private async Task UpdateIncomeViewDataGridAsync()
        {
            #region SQLite
            try
            {
                int year = DateTime.Now.Year;
                Task<decimal?> task1 = Task<decimal?>.Factory.StartNew((mon) =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        var summe =
                        (from summ in dataBase.Incomes
                         where summ.DateOfIncome.Month == (int)mon && summ.DateOfIncome.Year == year
                         select (decimal?)Decimal.Parse(summ.PreisOfIncome, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"))).SumAsync();
                        return summe.Result;
                    }
                }, monatName.SelectedIndex + 1);

                Task.WaitAll(task1);

                incomesViewSource = ((CollectionViewSource)(this.FindResource("incomesViewSource")));
                //Загружаем базу данных выбранного месяца
                var optionsBuilder1 = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder1.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder1.Options))
                {
                    var query =
                    from incom in dataBase.Incomes
                    where incom.DateOfIncome.Month == monatName.SelectedIndex + 1 && incom.DateOfIncome.Year == DateTime.Now.Year
                    orderby incom.DateOfIncome descending
                    select incom;
                    await query.LoadAsync();
                    incomesViewSource.Source = query;
                }

                if (task1.Result == null)
                {
                    TextBlockWithIncomePreisSumm(task1.Result);
                }
                else
                {
                    TextBlockWithIncomePreisSumm(task1.Result);
                }

                //Создаем данные для диаграммы
                CreateDateDiagramm();
                MenuEditIsEnable();
            }
            catch(Exception em)
            {
                MessageBox.Show(em.Message);
            }
            #endregion


            #region SQLServer

            //int year = DateTime.Now.Year;
            //Task<decimal?> task1 = Task<decimal?>.Factory.StartNew((mon) =>
            //{
            //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        var summe =
            //        (from summ in dataBase.Incomes
            //         where summ.DateOfIncome.Month == (int)mon && summ.DateOfIncome.Year == year
            //         select (decimal?)summ.PreisOfIncome).SumAsync();
            //        return summe.Result;
            //    }
            //}, monatName.SelectedIndex + 1);

            //Task.WaitAll(task1);

            //incomesViewSource = ((CollectionViewSource)(this.FindResource("incomesViewSource")));
            ////Загружаем базу данных выбранного месяца
            //using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //{
            //    var query =
            //    from incom in dataBase.Incomes
            //    where incom.DateOfIncome.Month == monatName.SelectedIndex + 1 && incom.DateOfIncome.Year == DateTime.Now.Year
            //    orderby incom.DateOfIncome descending
            //    select incom;
            //    await query.LoadAsync();
            //    incomesViewSource.Source = dataBase.Incomes.Local;
            //}

            //if (task1.Result == null)
            //{
            //    textBlockIncome.Text = "0 €";
            //}
            //else
            //{
            //    textBlockIncome.Text = task1.Result.ToString() + " €";
            //}

            ////Создаем данные для диаграммы
            //CreateDateDiagramm();
            //MenuEditIsEnable();
            #endregion
        }

        void TextBlockWithIncomePreisSumm(decimal? result)
        {
            decimal tempPreisDecimal = Convert.ToDecimal(result);
            //Add Euro to end
            textBlockIncome.Text = tempPreisDecimal.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));
        }
        #endregion


        #region Обновляем список в таблице после смены месяца 
        //Обновление списка расходов после смены месяца
        private async void monatName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (change)
            {
                await UpdateIncomeViewDataGridAsync();
            }
            else
            {
                await UpdateCostViewDataGridAsync();
            }
        }

        #endregion

        private bool _shutdown;

        //Button MenuClose
        private async void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ja",
                NegativeButtonText = "Nein",
                AnimateShow = true,
                AnimateHide = false,
                ColorScheme = this.MetroDialogOptions.ColorScheme
            };
            BlurEffect effect = new BlurEffect();
            effect.Radius = 25;
            mainGrid.Effect = effect;

            var result = await this.ShowMessageAsync("Sind Sie sicher, dass Sie beenden möchten?",
                "Beenden",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);
            _shutdown = result == MessageDialogResult.Affirmative;

            if (_shutdown)
            {
                Application.Current.Shutdown();
            }
            else
            {
                effect.Radius = 0;
                mainGrid.Effect = effect;
            }

        }

        private async void App_Closing()
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ja",
                NegativeButtonText = "Nein",
                AnimateShow = true,
                AnimateHide = false,
                ColorScheme = this.MetroDialogOptions.ColorScheme
            };
            BlurEffect effect = new BlurEffect();
            effect.Radius = 25;
            mainGrid.Effect = effect;

            var result = await this.ShowMessageAsync("Sind Sie sicher, dass Sie beenden möchten?",
                "Beenden",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);
            _shutdown = result == MessageDialogResult.Affirmative;

            if (_shutdown)
            {
                Application.Current.Shutdown();
            }
            else
            {
                await Authentification();
            }
        }
        //Button WindowsClose
        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BlurEffect effect = new BlurEffect();
            effect.Radius = 25;
            //effect.BeginAnimation()
            mainGrid.Effect = effect;
            if (e.Cancel) return;
            e.Cancel = !_shutdown;
            if (_shutdown) return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ja",
                NegativeButtonText = "Nein",
                AnimateShow = true,
                AnimateHide = false,
                ColorScheme = this.MetroDialogOptions.ColorScheme 
        };

            var result = await this.ShowMessageAsync("Sind Sie sicher, dass Sie beenden möchten?",
                "Beenden",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _shutdown = result == MessageDialogResult.Affirmative;

            if (_shutdown)
            {
                Application.Current.Shutdown();
            }
            else
            {
                effect.Radius = 0;
                mainGrid.Effect = effect;
            }
        }

        //Изменить отображение Доходов - Расходов
        private async void ChangeIncomeCost_Click(object sender, RoutedEventArgs e)
        {
            if (!change)
            {
                //Costs
                AddCosts.Visibility = Visibility.Collapsed;
                dataGridCosts.Visibility = Visibility.Collapsed;
                txtBlCosts.Visibility = Visibility.Collapsed;
                textBlockCost.Visibility = Visibility.Collapsed;
                //Incomes
                AddIncome.Visibility = Visibility.Visible;
                dataGridIncomes.Visibility = Visibility.Visible;
                txtBlIncome.Visibility = Visibility.Visible;
                textBlockIncome.Visibility = Visibility.Visible;
                //indexIncomesDiagramm = true;
                change = true;
                yearIndex = true;
                add = true;
                await UpdateIncomeViewDataGridAsync();
            }
            else
            {
                //Incomes
                AddIncome.Visibility = Visibility.Collapsed;
                dataGridIncomes.Visibility = Visibility.Collapsed;
                txtBlIncome.Visibility = Visibility.Collapsed;
                textBlockIncome.Visibility = Visibility.Collapsed;
                //Costs
                AddCosts.Visibility = Visibility.Visible;
                dataGridCosts.Visibility = Visibility.Visible;
                txtBlCosts.Visibility = Visibility.Visible;
                textBlockCost.Visibility = Visibility.Visible;
                //indexCostsDiagramm = true;
                change = false;
                yearIndex = true;
                add = true;
                await UpdateCostViewDataGridAsync();
            }
        }


        #region Diagramm
        public class ErgebnisDiagramm : INotifyPropertyChanged
        {
            #region SQLite                            

            //protected static DataBaseUsingEF.DataBaseUsingEF dataBase;
            private String myClass;


            public String Class
            {
                get { return myClass; }
                set
                {
                    myClass = value;
                    RaisePropertyChangeEvent("Class");
                }
            }

            private double? category;

            public double? Category
            {
                get { return category; }
                set
                {
                    category = value;
                    RaisePropertyChangeEvent("Category");
                }
            }

            #region Диаграмма для расходов за месяц
            private static double? SummeCostsRechnenMonth(string str)
            {
                double? d = 0;

                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    d =
                        (from summ in dataBase.Costs
                         where summ.DateOfCost.Month == copyComboBoxMonatName.SelectedIndex + 1 &&
                         summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
                         select (double?)Decimal.Parse(summ.PreisOfCost, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CreateSpecificCulture("DE-de"))/*Convert.ToDecimal(summ.PreisOfCost, CultureInfo.CreateSpecificCulture("de-DE"))*/).Sum();
                    //Если равняется null нужно присвоить 0 переменной
                    if (d == null)
                    {
                        d = 0.00;
                    }
                }
                return d;
            }

            public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammCostsMonth()
            {
                var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Fixkosten", Category = SummeCostsRechnenMonth("Fixkosten") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Lebensmittel", Category = SummeCostsRechnenMonth("Lebensmittel") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Verkehr", Category = SummeCostsRechnenMonth("Verkehr") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Kraftfahrzeug", Category = SummeCostsRechnenMonth("Kraftfahrzeug") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Unterhaltung", Category = SummeCostsRechnenMonth("Unterhaltung") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Persönliche kosten", Category = SummeCostsRechnenMonth("Persönliche kosten") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Urlaub", Category = SummeCostsRechnenMonth("Urlaub") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Soft/Service", Category = SummeCostsRechnenMonth("Soft/Service") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Alles ins Haus", Category = SummeCostsRechnenMonth("Alles ins Haus") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Bildung", Category = SummeCostsRechnenMonth("Bildung") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Andere", Category = SummeCostsRechnenMonth("Andere") });
                return ergebnisListe;
            }

            #endregion

            #region Диаграмма для доходов за месяц
            private static double? SummeIncomesRechnenMonth(string str)
            {
                double? d = 0;
                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    d =
                        (from summ in dataBase.Incomes
                         where summ.DateOfIncome.Month == copyComboBoxMonatName.SelectedIndex + 1 &&
                         summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
                         select (double?)Decimal.Parse(summ.PreisOfIncome, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"))).Sum();
                    //Если равняется null нужно присвоить 0 переменной
                    if (d == null)
                    {
                        d = 0.00;
                    }
                }
                return d;
            }

            public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammIncomesMonth()
            {
                var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Gehalt", Category = SummeIncomesRechnenMonth("Gehalt") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Andere Einkommen", Category = SummeIncomesRechnenMonth("Andere Einkommen") });
                //ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnen("Зарплата") });
                return ergebnisListe;
            }

            #endregion

            #region Диаграмма для расходов за квартал

            static double? quartal;
            static double? month1;
            static double? month2;
            static double? month3;
            //Quartal nummer setzen
            private static double? SummeCostsRechnenQuartal(string str)
            {
                int quartalNummer;
                if (copyComboBoxMonatName.SelectedIndex + 1 <= 3)
                {
                    quartalNummer = 1;
                    SummeCostsFirstQuartalAsync(quartalNummer, str);
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 4 && copyComboBoxMonatName.SelectedIndex + 1 <= 6)
                {
                    quartalNummer = 2;
                    SummeCostsFirstQuartalAsync(quartalNummer, str);
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 7 && copyComboBoxMonatName.SelectedIndex + 1 <= 9)
                {
                    quartalNummer = 3;
                    SummeCostsFirstQuartalAsync(quartalNummer, str);
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 10 && copyComboBoxMonatName.SelectedIndex + 1 <= 12)
                {
                    quartalNummer = 4;
                    SummeCostsFirstQuartalAsync(quartalNummer, str);
                }

                return quartal;
            }

            private static void SummeCostsFirstQuartalAsync(int quartalNum, string str)
            {
                if (quartalNum == 1)
                {
                    RechnenCostQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }
                else if (quartalNum == 2)
                {
                    RechnenCostQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }
                else if (quartalNum == 3)
                {
                    RechnenCostQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }
                else if (quartalNum == 4)
                {
                    RechnenCostQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }

            }

            private static void ZwischenSummeQuartalAsync(string str, int monthIndex1, int monthIndex2, int monthIndex3)
            {
                Task task1 = Task.Factory.StartNew(() =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        month1 =
                        (from summ in dataBase.Costs
                         where summ.DateOfCost.Month == monthIndex1 &&
                         summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
                         select (double?)Decimal.Parse(summ.PreisOfCost, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CreateSpecificCulture("DE-de"))/*Convert.ToDecimal(summ.PreisOfCost, CultureInfo.CreateSpecificCulture("de-DE"))*/).Sum();
                    }
                });
                Task task2 = Task.Factory.StartNew(() =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        month2 =
                        (from summ in dataBase.Costs
                         where summ.DateOfCost.Month == monthIndex2 &&
                         summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
                         select (double?)Decimal.Parse(summ.PreisOfCost, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CreateSpecificCulture("DE-de"))/*Convert.ToDecimal(summ.PreisOfCost, CultureInfo.CreateSpecificCulture("de-DE"))*/).Sum();
                    }
                });
                Task task3 = Task.Factory.StartNew(() =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        month3 =
                        (from summ in dataBase.Costs
                         where summ.DateOfCost.Month == monthIndex3 &&
                         summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
                         select (double?)Decimal.Parse(summ.PreisOfCost, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CreateSpecificCulture("DE-de"))/*Convert.ToDecimal(summ.PreisOfCost, CultureInfo.CreateSpecificCulture("de-DE"))*/).Sum();
                    }
                });
                Task.WaitAll(task1, task2, task3);
            }

            private static void RechnenCostQuartalAsync(int quartalNum, string str)
            {
                if (quartalNum == 1)
                {
                    ZwischenSummeQuartalAsync(str, 1, 2, 3);
                }
                else if (quartalNum == 2)
                {
                    ZwischenSummeQuartalAsync(str, 4, 5, 6);
                }
                else if (quartalNum == 3)
                {
                    ZwischenSummeQuartalAsync(str, 7, 8, 9);
                }
                else if (quartalNum == 4)
                {
                    ZwischenSummeQuartalAsync(str, 10, 11, 12);
                }
            }
            public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammCostsQuartal()
            {
                var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Fixkosten", Category = SummeCostsRechnenMonth("Fixkosten") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Lebensmittel", Category = SummeCostsRechnenMonth("Lebensmittel") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Verkehr", Category = SummeCostsRechnenMonth("Verkehr") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Kraftfahrzeug", Category = SummeCostsRechnenMonth("Kraftfahrzeug") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Unterhaltung", Category = SummeCostsRechnenMonth("Unterhaltung") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Persönliche kosten", Category = SummeCostsRechnenMonth("Persönliche kosten") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Urlaub", Category = SummeCostsRechnenMonth("Urlaub") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Soft/Service", Category = SummeCostsRechnenMonth("Soft/Service") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Alles ins Haus", Category = SummeCostsRechnenMonth("Alles ins Haus") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Bildung", Category = SummeCostsRechnenMonth("Bildung") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Andere", Category = SummeCostsRechnenMonth("Andere") });
                return ergebnisListe;
            }

            #endregion

            #region Диаграмма для доходов за квартал
            private static double? SummeIncomesRechnenQuartal(string str)
            {
                int quartalNummer;
                if (copyComboBoxMonatName.SelectedIndex + 1 <= 3)
                {
                    quartalNummer = 1;
                    SummeIncomesFirstQuartalAsync(quartalNummer, str);
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 4 && copyComboBoxMonatName.SelectedIndex + 1 <= 6)
                {
                    quartalNummer = 2;
                    SummeIncomesFirstQuartalAsync(quartalNummer, str);
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 7 && copyComboBoxMonatName.SelectedIndex + 1 <= 9)
                {
                    quartalNummer = 3;
                    SummeIncomesFirstQuartalAsync(quartalNummer, str);
                }
                else if (copyComboBoxMonatName.SelectedIndex + 1 >= 10 && copyComboBoxMonatName.SelectedIndex + 1 <= 12)
                {
                    quartalNummer = 4;
                    SummeIncomesFirstQuartalAsync(quartalNummer, str);
                }

                return quartal;
            }

            private static void SummeIncomesFirstQuartalAsync(int quartalNum, string str)
            {
                if (quartalNum == 1)
                {
                    RechnenIncomesQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }
                else if (quartalNum == 2)
                {
                    RechnenIncomesQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }
                else if (quartalNum == 3)
                {
                    RechnenIncomesQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }
                else if (quartalNum == 4)
                {
                    RechnenIncomesQuartalAsync(quartalNum, str);
                    //Если равняется null нужно присвоить 0 переменной
                    IfNullMonth();
                    quartal = month1 + month2 + month3;
                }

            }
            private static void IfNullMonth()
            {
                //Если равняется null нужно присвоить 0 переменной
                if (month1 == null)
                {
                    month1 = 0.00;
                }
                if (month2 == null)
                {
                    month2 = 0.00;
                }
                if (month3 == null)
                {
                    month3 = 0.00;
                }
            }

            private static void RechnenIncomesQuartalAsync(int quartalNum, string str)
            {
                if (quartalNum == 1)
                {
                    ZwischenSummeIncommesQuartalAsync(str, 1, 2, 3);
                }
                else if (quartalNum == 2)
                {
                    ZwischenSummeIncommesQuartalAsync(str, 4, 5, 6);
                }
                else if (quartalNum == 3)
                {
                    ZwischenSummeIncommesQuartalAsync(str, 7, 8, 9);
                }
                else if (quartalNum == 4)
                {
                    ZwischenSummeIncommesQuartalAsync(str, 10, 11, 12);
                }
            }

            private static void ZwischenSummeIncommesQuartalAsync(string str, int v1, int v2, int v3)
            {
                Task task1 = Task.Factory.StartNew(() =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        month1 =
                        (from summ in dataBase.Incomes
                         where summ.DateOfIncome.Month == v1 &&
                         summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
                         select (double?)Decimal.Parse(summ.PreisOfIncome, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"))).Sum();
                    }
                });
                Task task2 = Task.Factory.StartNew(() =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        month2 =
                        (from summ in dataBase.Incomes
                         where summ.DateOfIncome.Month == v2 &&
                         summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
                         select (double?)Decimal.Parse(summ.PreisOfIncome, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"))).Sum();
                    }
                });
                Task task3 = Task.Factory.StartNew(() =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        month3 =
                        (from summ in dataBase.Incomes
                         where summ.DateOfIncome.Month == v3 &&
                         summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
                         select (double?)Decimal.Parse(summ.PreisOfIncome, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"))).Sum();
                    }
                });
                Task.WaitAll(task1, task2, task3);
            }

            public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammIncomesQuartal()
            {
                var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Gehalt", Category = SummeIncomesRechnenMonth("Gehalt") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Andere Einkommen", Category = SummeIncomesRechnenMonth("Andere Einkommen") });
                //ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnen("Зарплата") });
                return ergebnisListe;
            }

            #endregion

            #region Диаграмма для расходов за год
            private static double? SummeCostsRechnenYear(string str)
            {
                double? year = 0;
                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    year =
                        (from summ in dataBase.Costs
                         where summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
                         select (double?)Decimal.Parse(summ.PreisOfCost, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CreateSpecificCulture("DE-de"))/*Convert.ToDecimal(summ.PreisOfCost, CultureInfo.CreateSpecificCulture("de-DE"))*/).Sum();
                    //Если равняется null нужно присвоить 0 переменной
                    if (year == null)
                    {
                        year = 0.00;
                    }
                }
                return year;
            }

            public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammCostsYear()
            {
                var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Fixkosten", Category = SummeCostsRechnenMonth("Fixkosten") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Lebensmittel", Category = SummeCostsRechnenMonth("Lebensmittel") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Verkehr", Category = SummeCostsRechnenMonth("Verkehr") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Kraftfahrzeug", Category = SummeCostsRechnenMonth("Kraftfahrzeug") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Unterhaltung", Category = SummeCostsRechnenMonth("Unterhaltung") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Persönliche kosten", Category = SummeCostsRechnenMonth("Persönliche kosten") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Urlaub", Category = SummeCostsRechnenMonth("Urlaub") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Soft/Service", Category = SummeCostsRechnenMonth("Soft/Service") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Alles ins Haus", Category = SummeCostsRechnenMonth("Alles ins Haus") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Bildung", Category = SummeCostsRechnenMonth("Bildung") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Andere", Category = SummeCostsRechnenMonth("Andere") });
                return ergebnisListe;
            }

            #endregion

            #region Диаграмма для доходов за год
            private static double? SummeIncomesRechnenYear(string str)
            {
                double? year = 0;
                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    year =
                        (from summ in dataBase.Incomes
                         where summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
                         select (double?)Decimal.Parse(summ.PreisOfIncome, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CreateSpecificCulture("DE-de"))).Sum();
                    //Если равняется null нужно присвоить 0 переменной
                    if (year == null)
                    {
                        year = 0.00;
                    }
                }
                return year;
            }

            public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammIncomesYear()
            {
                var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Gehalt", Category = SummeIncomesRechnenMonth("Gehalt") });
                ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Andere Einkommen", Category = SummeIncomesRechnenMonth("Andere Einkommen") });
                //ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnen("Зарплата") });
                return ergebnisListe;
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            private void RaisePropertyChangeEvent(String propertyName)
            {
                if (PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion

            #endregion


            #region SQLServer

            //protected static DataBaseUsingEF.DataBaseUsingEF dataBase;
            //private String myClass;


            //public String Class
            //{
            //    get { return myClass; }
            //    set
            //    {
            //        myClass = value;
            //        RaisePropertyChangeEvent("Class");
            //    }
            //}

            //private double? category;

            //public double? Category
            //{
            //    get { return category; }
            //    set
            //    {
            //        category = value;
            //        RaisePropertyChangeEvent("Category");
            //    }
            //}

            //#region Диаграмма для расходов за месяц
            //private static double? SummeCostsRechnenMonth(string str)
            //{
            //    double? d = 0;
            //    using (dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        d =
            //            (from summ in dataBase.Costs
            //             where summ.DateOfCost.Month == copyComboBoxMonatName.SelectedIndex + 1 &&
            //             summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //             select (double?)summ.PreisOfCost).Sum();
            //        //Если равняется null нужно присвоить 0 переменной
            //        if (d == null)
            //        {
            //            d = 0.00;
            //        }
            //    }
            //    return d;
            //}

            //public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammCostsMonth()
            //{
            //    var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Фиксированные", Category = SummeCostsRechnenMonth("Фиксированные расходы") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Продукты", Category = SummeCostsRechnenMonth("Продукты") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Транспорт", Category = SummeCostsRechnenMonth("Транспорт") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Автомобиль", Category = SummeCostsRechnenMonth("Автомобиль") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Развлечения", Category = SummeCostsRechnenMonth("Развлечения") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Расходы на себя", Category = SummeCostsRechnenMonth("Расходы на себя") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Отпуск", Category = SummeCostsRechnenMonth("Отпуск") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Софт/Сервисы ", Category = SummeCostsRechnenMonth("Софт/Сервисы") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Товары для дома", Category = SummeCostsRechnenMonth("Товары для дома") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Образование", Category = SummeCostsRechnenMonth("Образование") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Разное", Category = SummeCostsRechnenMonth("Разное") });
            //    return ergebnisListe;
            //}

            //#endregion

            //#region Диаграмма для доходов за месяц
            //private static double? SummeIncomesRechnenMonth(string str)
            //{
            //    double? d = 0;
            //    using (dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        d =
            //            (from summ in dataBase.Incomes
            //             where summ.DateOfIncome.Month == copyComboBoxMonatName.SelectedIndex + 1 &&
            //             summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
            //             select (double?)summ.PreisOfIncome).Sum();
            //        //Если равняется null нужно присвоить 0 переменной
            //        if (d == null)
            //        {
            //            d = 0.00;
            //        }
            //    }
            //    return d;
            //}

            //public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammIncomesMonth()
            //{
            //    var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnenMonth("Зарплата") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Другие источники", Category = SummeIncomesRechnenMonth("Другие источники") });
            //    //ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnen("Зарплата") });
            //    return ergebnisListe;
            //}

            //#endregion

            //#region Диаграмма для расходов за квартал

            //static double? quartal;
            //static double? month1;
            //static double? month2;
            //static double? month3;
            ////Quartal nummer setzen
            //private static double? SummeCostsRechnenQuartal(string str)
            //{
            //    int quartalNummer;
            //    if (copyComboBoxMonatName.SelectedIndex + 1 <= 3)
            //    {
            //        quartalNummer = 1;
            //        SummeCostsFirstQuartalAsync(quartalNummer, str);
            //    }
            //    else if (copyComboBoxMonatName.SelectedIndex + 1 >= 4 && copyComboBoxMonatName.SelectedIndex + 1 <= 6)
            //    {
            //        quartalNummer = 2;
            //        SummeCostsFirstQuartalAsync(quartalNummer, str);
            //    }
            //    else if (copyComboBoxMonatName.SelectedIndex + 1 >= 7 && copyComboBoxMonatName.SelectedIndex + 1 <= 9)
            //    {
            //        quartalNummer = 3;
            //        SummeCostsFirstQuartalAsync(quartalNummer, str);
            //    }
            //    else if (copyComboBoxMonatName.SelectedIndex + 1 >= 10 && copyComboBoxMonatName.SelectedIndex + 1 <= 12)
            //    {
            //        quartalNummer = 4;
            //        SummeCostsFirstQuartalAsync(quartalNummer, str);
            //    }


            //    #region comment
            //    //else if (copyComboBoxMonatName.SelectedIndex + 1 >= 7 && copyComboBoxMonatName.SelectedIndex + 1 <= 9)
            //    //{
            //    //    quartalNummer = 3;
            //    //    //// Здесь можно сделать асинхронный вызов для быстрого подсчета
            //    //    //quartal1 =
            //    //    //     (from summ in dataBase.Costs
            //    //    //      where summ.DateOfCost.Month == 7 &&
            //    //    //      summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //    //    //      select (double?)summ.PreisOfCost).Sum();
            //    //    //quartal2 =
            //    //    //      (from summ in dataBase.Costs
            //    //    //       where summ.DateOfCost.Month == 8 &&
            //    //    //       summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //    //    //       select (double?)summ.PreisOfCost).Sum();
            //    //    //quartal3 =
            //    //    //     (from summ in dataBase.Costs
            //    //    //      where summ.DateOfCost.Month == 9 &&
            //    //    //      summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //    //    //      select (double?)summ.PreisOfCost).Sum();
            //    //    ////Если равняется null нужно присвоить 0 переменной
            //    //    //if (quartal1 == null)
            //    //    //{
            //    //    //    quartal1 = 0.00;
            //    //    //}
            //    //    //if (quartal2 == null)
            //    //    //{
            //    //    //    quartal2 = 0.00;
            //    //    //}
            //    //    //if (quartal3 == null)
            //    //    //{
            //    //    //    quartal3 = 0.00;
            //    //    //}
            //    //    //quartal = quartal1 + quartal2 + quartal3;
            //    //}
            //    //else if (copyComboBoxMonatName.SelectedIndex + 1 >= 10 && copyComboBoxMonatName.SelectedIndex + 1 <= 12)
            //    //{
            //    //    quartalNummer = 4;
            //    //    //// Здесь можно сделать асинхронный вызов для быстрого подсчета
            //    //    //quartal1 =
            //    //    //     (from summ in dataBase.Costs
            //    //    //      where summ.DateOfCost.Month == 10 &&
            //    //    //      summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //    //    //      select (double?)summ.PreisOfCost).Sum();
            //    //    //quartal2 =
            //    //    //      (from summ in dataBase.Costs
            //    //    //       where summ.DateOfCost.Month == 11 &&
            //    //    //       summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //    //    //       select (double?)summ.PreisOfCost).Sum();
            //    //    //quartal3 =
            //    //    //     (from summ in dataBase.Costs
            //    //    //      where summ.DateOfCost.Month == 12 &&
            //    //    //      summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //    //    //      select (double?)summ.PreisOfCost).Sum();
            //    //    ////Если равняется null нужно присвоить 0 переменной
            //    //    //if (quartal1 == null)
            //    //    //{
            //    //    //    quartal1 = 0.00;
            //    //    //}
            //    //    //if (quartal2 == null)
            //    //    //{
            //    //    //    quartal2 = 0.00;
            //    //    //}
            //    //    //if (quartal3 == null)
            //    //    //{
            //    //    //    quartal3 = 0.00;
            //    //    //}
            //    //    //quartal = quartal1 + quartal2 + quartal3;
            //    //}
            //    #endregion
            //    return quartal;
            //}

            //private static void SummeCostsFirstQuartalAsync(int quartalNum, string str)
            //{
            //    if (quartalNum == 1)
            //    {
            //        RechnenCostQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }
            //    else if (quartalNum == 2)
            //    {
            //        RechnenCostQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }
            //    else if (quartalNum == 3)
            //    {
            //        RechnenCostQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }
            //    else if (quartalNum == 4)
            //    {
            //        RechnenCostQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }

            //}

            //private static void ZwischenSummeQuartalAsync(string str, int monthIndex1, int monthIndex2, int monthIndex3)
            //{
            //    Task task1 = Task.Factory.StartNew(() =>
            //    {
            //        using (var db = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //        {
            //            month1 =
            //            (from summ in db.Costs
            //             where summ.DateOfCost.Month == monthIndex1 &&
            //             summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //             select (double?)summ.PreisOfCost).Sum();
            //        }
            //    });
            //    Task task2 = Task.Factory.StartNew(() =>
            //    {
            //        using (var db = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //        {
            //            month2 =
            //            (from summ in db.Costs
            //             where summ.DateOfCost.Month == monthIndex2 &&
            //             summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //             select (double?)summ.PreisOfCost).Sum();
            //        }
            //    });
            //    Task task3 = Task.Factory.StartNew(() =>
            //    {
            //        using (var db = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //        {
            //            month3 =
            //            (from summ in db.Costs
            //             where summ.DateOfCost.Month == monthIndex3 &&
            //             summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //             select (double?)summ.PreisOfCost).Sum();
            //        }
            //    });
            //    Task.WaitAll(task1, task2, task3);
            //}

            //private static void RechnenCostQuartalAsync(int quartalNum, string str)
            //{
            //    if (quartalNum == 1)
            //    {
            //        ZwischenSummeQuartalAsync(str, 1, 2, 3);
            //    }
            //    else if (quartalNum == 2)
            //    {
            //        ZwischenSummeQuartalAsync(str, 4, 5, 6);
            //    }
            //    else if (quartalNum == 3)
            //    {
            //        ZwischenSummeQuartalAsync(str, 7, 8, 9);
            //    }
            //    else if (quartalNum == 4)
            //    {
            //        ZwischenSummeQuartalAsync(str, 10, 11, 12);
            //    }
            //}
            //public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammCostsQuartal()
            //{
            //    var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();

            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Фиксированные", Category = SummeCostsRechnenQuartal("Фиксированные расходы") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Продукты", Category = SummeCostsRechnenQuartal("Продукты") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Транспорт", Category = SummeCostsRechnenQuartal("Транспорт") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Автомобиль", Category = SummeCostsRechnenQuartal("Автомобиль") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Развлечения", Category = SummeCostsRechnenQuartal("Развлечения") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Расходы на себя", Category = SummeCostsRechnenQuartal("Расходы на себя") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Отпуск", Category = SummeCostsRechnenQuartal("Отпуск") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Софт/Сервисы ", Category = SummeCostsRechnenQuartal("Софт/Сервисы") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Товары для дома", Category = SummeCostsRechnenQuartal("Товары для дома") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Образование", Category = SummeCostsRechnenQuartal("Образование") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Разное", Category = SummeCostsRechnenQuartal("Разное") });
            //    return ergebnisListe;
            //}

            //#endregion

            //#region Диаграмма для доходов за квартал
            //private static double? SummeIncomesRechnenQuartal(string str)
            //{
            //    int quartalNummer;
            //    if (copyComboBoxMonatName.SelectedIndex + 1 <= 3)
            //    {
            //        quartalNummer = 1;
            //        SummeIncomesFirstQuartalAsync(quartalNummer, str);
            //    }
            //    else if (copyComboBoxMonatName.SelectedIndex + 1 >= 4 && copyComboBoxMonatName.SelectedIndex + 1 <= 6)
            //    {
            //        quartalNummer = 2;
            //        SummeIncomesFirstQuartalAsync(quartalNummer, str);
            //    }
            //    else if (copyComboBoxMonatName.SelectedIndex + 1 >= 7 && copyComboBoxMonatName.SelectedIndex + 1 <= 9)
            //    {
            //        quartalNummer = 3;
            //        SummeIncomesFirstQuartalAsync(quartalNummer, str);
            //    }
            //    else if (copyComboBoxMonatName.SelectedIndex + 1 >= 10 && copyComboBoxMonatName.SelectedIndex + 1 <= 12)
            //    {
            //        quartalNummer = 4;
            //        SummeIncomesFirstQuartalAsync(quartalNummer, str);
            //    }

            //    return quartal;
            //}

            //private static void SummeIncomesFirstQuartalAsync(int quartalNum, string str)
            //{
            //    if (quartalNum == 1)
            //    {
            //        RechnenIncomesQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }
            //    else if (quartalNum == 2)
            //    {
            //        RechnenIncomesQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }
            //    else if (quartalNum == 3)
            //    {
            //        RechnenIncomesQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }
            //    else if (quartalNum == 4)
            //    {
            //        RechnenIncomesQuartalAsync(quartalNum, str);
            //        //Если равняется null нужно присвоить 0 переменной
            //        IfNullMonth();
            //        quartal = month1 + month2 + month3;
            //    }

            //}
            //private static void IfNullMonth()
            //{
            //    //Если равняется null нужно присвоить 0 переменной
            //    if (month1 == null)
            //    {
            //        month1 = 0.00;
            //    }
            //    if (month2 == null)
            //    {
            //        month2 = 0.00;
            //    }
            //    if (month3 == null)
            //    {
            //        month3 = 0.00;
            //    }
            //}

            //private static void RechnenIncomesQuartalAsync(int quartalNum, string str)
            //{
            //    if (quartalNum == 1)
            //    {
            //        ZwischenSummeIncommesQuartalAsync(str, 1, 2, 3);
            //    }
            //    else if (quartalNum == 2)
            //    {
            //        ZwischenSummeIncommesQuartalAsync(str, 4, 5, 6);
            //    }
            //    else if (quartalNum == 3)
            //    {
            //        ZwischenSummeIncommesQuartalAsync(str, 7, 8, 9);
            //    }
            //    else if (quartalNum == 4)
            //    {
            //        ZwischenSummeIncommesQuartalAsync(str, 10, 11, 12);
            //    }
            //}

            //private static void ZwischenSummeIncommesQuartalAsync(string str, int v1, int v2, int v3)
            //{
            //    Task task1 = Task.Factory.StartNew(() =>
            //    {
            //        using (var db = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //        {
            //            month1 =
            //            (from summ in db.Incomes
            //             where summ.DateOfIncome.Month == v1 &&
            //             summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
            //             select (double?)summ.PreisOfIncome).Sum();
            //        }
            //    });
            //    Task task2 = Task.Factory.StartNew(() =>
            //    {
            //        using (var db = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //        {
            //            month2 =
            //            (from summ in db.Incomes
            //             where summ.DateOfIncome.Month == v2 &&
            //             summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
            //             select (double?)summ.PreisOfIncome).Sum();
            //        }
            //    });
            //    Task task3 = Task.Factory.StartNew(() =>
            //    {
            //        using (var db = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //        {
            //            month3 =
            //            (from summ in db.Incomes
            //             where summ.DateOfIncome.Month == v3 &&
            //             summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
            //             select (double?)summ.PreisOfIncome).Sum();
            //        }
            //    });
            //    Task.WaitAll(task1, task2, task3);
            //}

            //public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammIncomesQuartal()
            //{
            //    var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnenQuartal("Зарплата") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Другие источники", Category = SummeIncomesRechnenQuartal("Другие источники") });
            //    return ergebnisListe;
            //}

            //#endregion

            //#region Диаграмма для расходов за год
            //private static double? SummeCostsRechnenYear(string str)
            //{
            //    double? year = 0;
            //    using (dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        year =
            //            (from summ in dataBase.Costs
            //             where summ.DateOfCost.Year == DateTime.Now.Year && summ.CategoryOfCost == str
            //             select (double?)summ.PreisOfCost).Sum();
            //        //Если равняется null нужно присвоить 0 переменной
            //        if (year == null)
            //        {
            //            year = 0.00;
            //        }
            //    }
            //    return year;
            //}

            //public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammCostsYear()
            //{
            //    var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Фиксированные", Category = SummeCostsRechnenYear("Фиксированные расходы") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Продукты", Category = SummeCostsRechnenYear("Продукты") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Транспорт", Category = SummeCostsRechnenYear("Транспорт") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Автомобиль", Category = SummeCostsRechnenYear("Автомобиль") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Развлечения", Category = SummeCostsRechnenYear("Развлечения") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Расходы на себя", Category = SummeCostsRechnenYear("Расходы на себя") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Отпуск", Category = SummeCostsRechnenYear("Отпуск") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Софт/Сервисы ", Category = SummeCostsRechnenYear("Софт/Сервисы") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Товары для дома", Category = SummeCostsRechnenYear("Товары для дома") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Образование", Category = SummeCostsRechnenYear("Образование") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Разное", Category = SummeCostsRechnenYear("Разное") });
            //    return ergebnisListe;
            //}

            //#endregion

            //#region Диаграмма для доходов за год
            //private static double? SummeIncomesRechnenYear(string str)
            //{
            //    double? year = 0;
            //    using (dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        year =
            //            (from summ in dataBase.Incomes
            //             where summ.DateOfIncome.Year == DateTime.Now.Year && summ.CategoryOfIncome == str
            //             select (double?)summ.PreisOfIncome).Sum();
            //        //Если равняется null нужно присвоить 0 переменной
            //        if (year == null)
            //        {
            //            year = 0.00;
            //        }
            //    }
            //    return year;
            //}

            //public static ObservableCollection<ErgebnisDiagramm> ConstructionDiagrammIncomesYear()
            //{
            //    var ergebnisListe = new ObservableCollection<ErgebnisDiagramm>();
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnenYear("Зарплата") });
            //    ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Другие источники", Category = SummeIncomesRechnenYear("Другие источники") });
            //    //ergebnisListe.Add(new ErgebnisDiagramm() { Class = "Зарплата", Category = SummeIncomesRechnen("Зарплата") });
            //    return ergebnisListe;
            //}

            //#endregion

            //#region INotifyPropertyChanged Members

            //public event PropertyChangedEventHandler PropertyChanged;

            //private void RaisePropertyChangeEvent(String propertyName)
            //{
            //    if (PropertyChanged != null)
            //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            //}

            //#endregion

            #endregion
        }
        #endregion

        #region Rename
        private async void RenameCosts_Click(object sender, RoutedEventArgs e)
        {
            #region SQLite
            try
            {
                AddCostIncome.RenameCostIncome rename = new AddCostIncome.RenameCostIncome();
                rename.Owner = Window.GetWindow(this);
                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    rename.BaseName = baseName;
                    rename.PathDataBase = path + userName + @"\";
                    rename.CostOrIncome = true;
                    rename.ID = (dataGridCosts.SelectedItem as DataBaseUsingEF.Cost).Id;
                }
                rename.ShowDialog();
                await UpdateCostViewDataGridAsync();
            }
            catch(Exception em)
            {
                MessageBox.Show(em.Message);
            }
            #endregion


            #region SQLServer

            ////AddCostIncome.RenameCostIncome rename = new AddCostIncome.RenameCostIncome();
            ////rename.Owner = Window.GetWindow(this);
            ////using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            ////{
            ////    rename.BaseName = baseName;
            ////    rename.CostOrIncome = true;
            ////    rename.ID = (dataGridCosts.SelectedItem as DataBaseUsingEF.Cost).Id;
            ////}
            ////rename.ShowDialog();
            ////await UpdateCostViewDataGridAsync();
            #endregion
        }

        private async void RenameIncomes_Click(object sender, RoutedEventArgs e)
        {
            #region SQLite
            try
            {
                AddCostIncome.RenameCostIncome rename = new AddCostIncome.RenameCostIncome();
                rename.Owner = Window.GetWindow(this);
                var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                {
                    rename.BaseName = baseName;
                    rename.PathDataBase = path + userName + @"\";
                    rename.CostOrIncome = false;
                    rename.ID = (dataGridIncomes.SelectedItem as DataBaseUsingEF.Income).Id;
                }
                rename.ShowDialog();
                await UpdateIncomeViewDataGridAsync();
            }
            catch(Exception em)
            {
                MessageBox.Show(em.Message);
            }
            #endregion

            #region SQLServer
            //AddCostIncome.RenameCostIncome rename = new AddCostIncome.RenameCostIncome();
            //rename.Owner = Window.GetWindow(this);
            //using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //{
            //    rename.BaseName = baseName;
            //    rename.CostOrIncome = false;
            //    rename.ID = (dataGridIncomes.SelectedItem as DataBaseUsingEF.Income).Id;
            //}
            //rename.ShowDialog();
            //await UpdateIncomeViewDataGridAsync();
            #endregion
        }


        #endregion

        #region Delete
        private async void DeleteIncomes_Click(object sender, RoutedEventArgs e)
        {
            #region SQLite
            try
            {
                AddCostIncome.DeleteDialog delete = new AddCostIncome.DeleteDialog();
                delete.Owner = Window.GetWindow(this);
                delete.ShowDialog();
                if (delete.Ergebnis)
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        int incomeId = (dataGridIncomes.SelectedItem as DataBaseUsingEF.Income).Id;
                        DataBaseUsingEF.Income income = (from c in dataBase.Incomes where c.Id == incomeId select c).SingleOrDefault();
                        dataBase.Incomes.Remove(income);
                        dataBase.SaveChanges();
                    }
                    add = true;
                    yearIndex = true;
                    await UpdateIncomeViewDataGridAsync();
                }
            }
            catch(Exception em)
            {
                MessageBox.Show(em.Message);
            }
                #endregion

                #region SQLServer

                //AddCostIncome.DeleteDialog delete = new AddCostIncome.DeleteDialog();
                //delete.Owner = Window.GetWindow(this);
                //delete.ShowDialog();
                //if (delete.Ergebnis)
                //{

                //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
                //    {
                //        int incomeId = (dataGridIncomes.SelectedItem as DataBaseUsingEF.Income).Id;
                //        DataBaseUsingEF.Income income = (from c in dataBase.Incomes where c.Id == incomeId select c).SingleOrDefault();
                //        dataBase.Incomes.Remove(income);
                //        dataBase.SaveChanges();
                //    }
                //    add = true;
                //    yearIndex = true;
                //    await UpdateIncomeViewDataGridAsync();
                //}
                #endregion

            }

        private async void DeleteCosts_Click(object sender, RoutedEventArgs e)
        {
            #region SQLite
            try
            {
                AddCostIncome.DeleteDialog delete = new AddCostIncome.DeleteDialog();
                delete.Owner = Window.GetWindow(this);
                delete.ShowDialog();
                if (delete.Ergebnis)
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                    optionsBuilder.UseSqlite("Filename=" + path + userName + @"\" + baseName);

                    using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                    {
                        int costId = (dataGridCosts.SelectedItem as DataBaseUsingEF.Cost).Id;
                        DataBaseUsingEF.Cost cost = (from c in dataBase.Costs where c.Id == costId select c).SingleOrDefault();
                        dataBase.Costs.Remove(cost);
                        dataBase.SaveChanges();
                    }
                    add = true;
                    yearIndex = true;
                    await UpdateCostViewDataGridAsync();
                }
            }
            catch (Exception em)
            {
                MessageBox.Show(em.Message);
            }
            #endregion

            #region SQLServer
            //AddCostIncome.DeleteDialog delete = new AddCostIncome.DeleteDialog();
            //delete.Owner = Window.GetWindow(this);
            //delete.ShowDialog();
            //if (delete.Ergebnis)
            //{
            //    using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //    {
            //        int costId = (dataGridCosts.SelectedItem as DataBaseUsingEF.Cost).Id;
            //        DataBaseUsingEF.Cost cost = (from c in dataBase.Costs where c.Id == costId select c).SingleOrDefault();
            //        dataBase.Costs.Remove(cost);
            //        dataBase.SaveChanges();
            //    }
            //    add = true;
            //    yearIndex = true;
            //    await UpdateCostViewDataGridAsync();
            //}
            #endregion
        }


        #endregion

        private void MenuAdd_Click(object sender, RoutedEventArgs e)
        {
            if (change)
            {
                AddIncome_Click(sender, e);
            }
            else
            {
                AddCosts_Click(sender, e);
            }
        }

        private void MenuRename_Click(object sender, RoutedEventArgs e)
        {
            if (change)
            {
                RenameIncomes_Click(sender, e);
            }
            else
            {
                RenameCosts_Click(sender, e);
            }

        }

        private void MenuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (change)
            {
                DeleteIncomes_Click(sender, e);
            }
            else
            {
                DeleteCosts_Click(sender, e);
            }

        }

        #region Move SQL Base to SQLite
        //private void moveSQLBase_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string directoryName = Directory.GetCurrentDirectory();
        //        DirectoryInfo neueDirectory = Directory.CreateDirectory(directoryName + @"\Source");
        //        using (var dataBaseSQLServer = new DataBaseUsingEF.DataBaseUsingEF(baseName))
        //        {
        //            var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
        //            optionsBuilder.UseSqlite("Filename=Source/" + baseName);

        //            using (var dataBaseSQLite = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
        //            {

        //                foreach (var item in dataBaseSQLServer.Costs)
        //                {
        //                    dataBaseSQLite.Costs.Add(item);
        //                }
        //                foreach (var item in dataBaseSQLServer.Incomes)
        //                {
        //                    dataBaseSQLite.Incomes.Add(item);
        //                }
        //                foreach (var item in dataBaseSQLServer.Kontakts)
        //                {
        //                    dataBaseSQLite.Kontakts.Add(item);
        //                }
        //                foreach (var item in dataBaseSQLServer.Logins)
        //                {
        //                    dataBaseSQLite.Logins.Add(item);
        //                }
        //                foreach (var item in dataBaseSQLServer.Users)
        //                {
        //                    dataBaseSQLite.Users.Add(item);
        //                }

        //                if (dataBaseSQLite.Database.EnsureCreated())
        //                {
        //                    dataBaseSQLite.SaveChanges();
        //                }
        //                else
        //                {
        //                    dataBaseSQLite.Database.EnsureDeleted();
        //                    dataBaseSQLite.Database.EnsureCreated();
        //                    dataBaseSQLite.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception em)
        //    {
        //        MessageBox.Show(em.Message);
        //    }
        //}
        #endregion

        #region Convert English point to Germany Kommas
        //private void convertBase_Click(object sender, RoutedEventArgs e)
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
        //    optionsBuilder.UseSqlite("Filename=Source/" + baseName);
        //    try
        //    {

        //        using (var dataBaseSQLite = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
        //        {

        //            foreach (var item in dataBaseSQLite.Costs)
        //            {
        //                decimal d = Convert.ToDecimal(item.PreisOfCost, CultureInfo.CreateSpecificCulture("en-EN"));
        //                string costString = d.ToString("C", CultureInfo.CreateSpecificCulture("DE-de"));
        //                item.PreisOfCost = costString;
        //                dataBaseSQLite.Costs.Add(item);
        //            }
        //            foreach (var item in dataBaseSQLite.Incomes)
        //            {
        //                decimal d = Convert.ToDecimal(item.PreisOfIncome, CultureInfo.CreateSpecificCulture("en-EN"));
        //                string incomeString = d.ToString("C", CultureInfo.CreateSpecificCulture("DE-de"));
        //                item.PreisOfIncome = incomeString;
        //                dataBaseSQLite.Incomes.Add(item);
        //            }
        //            foreach (var item in dataBaseSQLite.Kontakts)
        //            {
        //                dataBaseSQLite.Kontakts.Add(item);
        //            }
        //            foreach (var item in dataBaseSQLite.Logins)
        //            {
        //                dataBaseSQLite.Logins.Add(item);
        //            }
        //            foreach (var item in dataBaseSQLite.Users)
        //            {
        //                dataBaseSQLite.Users.Add(item);
        //            }

        //            if (dataBaseSQLite.Database.EnsureCreated())
        //            {
        //                dataBaseSQLite.SaveChanges();
        //            }
        //            else
        //            {
        //                dataBaseSQLite.Database.EnsureDeleted();
        //                dataBaseSQLite.Database.EnsureCreated();
        //                dataBaseSQLite.SaveChanges();
        //            }
        //        }
        //        MessageBox.Show("Alles OK!!!");
        //    }
        //    catch(Exception em)
        //    {
        //        MessageBox.Show(em.Message);
        //    }
        //}
        #endregion

        private void MenuHelpAboutProg_Click(object sender, RoutedEventArgs e)
        {
            MenuHelp.AboutProgramm about = new MenuHelp.AboutProgramm();

            about.Show();
        }
    }
}