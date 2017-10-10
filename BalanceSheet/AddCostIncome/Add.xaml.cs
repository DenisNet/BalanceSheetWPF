using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
//using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using Microsoft.Data.Entity;
using System.Globalization;

namespace BalanceSheet.AddCostIncome
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : MetroWindow
    {
        int indexCategory = 0;
        int indexUnderCategory = 0;
        string baseName;
        public string PathUser { get; internal set; }
        //Используем для базы данных
        ComboBox tempComboBox;
        //отображение ошибок
        private ErrorAbfangen errorAbfangen;

        public Add()
        {
            InitializeComponent();
            errorAbfangen = new ErrorAbfangen();
            gridMain.DataContext = errorAbfangen;
            this.BorderThickness = new Thickness(1);
            this.BorderBrush = null;
            this.SetResourceReference(MetroWindow.GlowBrushProperty, "AccentColorBrush");
        }

        //Название базы данных
        public void DataBaseIn(string dataBase, string pathUser)
        {
            baseName = dataBase;
            PathUser = pathUser;
        }
        //Вызывать последнюю введенную дату
        //сделать с помощью Serialisation в отдельный файл
        public string sKey = "Hallo BalanceSheet Was gehts neu";
        public string sIV = "Alles gut bruder";
        public void SerialCryptoDate()
        {
            string fileName = @"\Date.dat";
            string directoryName = Directory.GetCurrentDirectory();
            DirectoryInfo neueDirectory = Directory.CreateDirectory(directoryName + @"\Date");
            string path = directoryName + @"\Date" + fileName;

            #region FileAccess for date

            //File.SetAttributes(path, FileAttributes.Hidden);
            //необходимо дописать доступ к файлу, для того что бы он не изменялся
            //в файле, а был изменен только в программе

            //FileSecurity fs = new FileSecurity();
            //fs.AddAccessRule(new FileSystemAccessRule(@"DOMAINNAME\AccountName",
            //                                                FileSystemRights.ReadData,
            //                                                AccessControlType.Allow));
            //File.SetAttributes(path, FileAttributes.Encrypted);

            #endregion
            
            //Serialization
            DateTimeSpeichern dateTime = new DateTimeSpeichern();
            FileStream stream = new FileStream(path, FileMode.Create);
            dateTime.TimeSpeichern = dataPickerAdd.SelectedDate.Value.Date;
            dateTime.LetzteAnderung = DateTime.Now.Date;
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, dateTime);
            stream.Close();

            //Cryptografy
            FileStream fsOpen = File.OpenRead(path);
            byte[] input = new byte[fsOpen.Length];
            fsOpen.Read(input, 0, input.Length);
            fsOpen.Close();
            FileStream fsWrite = File.OpenWrite(path);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (CryptoStream csEncrypt = new CryptoStream(fsWrite, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(input, 0, input.Length);
                }
            }
            fsWrite.Close();
        }

        public void DeSerialCryptoDate()
        {
            string fileName = @"\Date.dat";
            string directoryName = Directory.GetCurrentDirectory();
            string path = directoryName + @"\Date" + fileName;
            DateTimeSpeichern dateTime = null;

            if (File.Exists(path))
            {
                FileStream fsOpen = File.OpenRead(path);

                byte[] bytearrayinput = new byte[fsOpen.Length];
                fsOpen.Read(bytearrayinput, 0, bytearrayinput.Length);
                fsOpen.Close();
                //FileStream fsWrite = File.OpenWrite(path);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                    aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(bytearrayinput))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            try
                            {
                                BinaryFormatter formatter = new BinaryFormatter();

                                dateTime = (DateTimeSpeichern)formatter.Deserialize(csDecrypt);
                            }
                            catch (SerializationException )
                            {
                                MessageBox.Show("Eine Fehler ist aufgetretten.");
                                throw;
                            }
                            //finally
                            //{
                            //    fs.Close();
                            //}
                        }
                    }
                }
                if (dateTime.LetzteAnderung == DateTime.Today.Date)
                {
                    dataPickerAdd.SelectedDate = dateTime.TimeSpeichern;
                }
                else
                {
                    dataPickerAdd.SelectedDate = DateTime.Today;
                }
            }
            else
            {
                dataPickerAdd.SelectedDate = DateTime.Today;
            }
        }


        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            #region SQLite
            try
            {
                decimal preis;
                if (string.IsNullOrWhiteSpace(nameTxBxAdd.Text) || string.IsNullOrWhiteSpace(priceTxBxAdd.Text))
                {
                    if (string.IsNullOrWhiteSpace(nameTxBxAdd.Text))
                    {
                        nameTxBxAdd.Focus();
                    }
                    else
                    {
                        priceTxBxAdd.Focus();
                    }
                    DatenUpdateSourсe();
                    lblError.Content = "Fehler! Sie haben nicht alle Daten eingegeben.";
                }
                else
                {
                    if (this.Title == "Ausgabe hinzufügen")
                    {
                        if (decimal.TryParse(priceTxBxAdd.Text, out preis))
                        {
                            decimal tempPreisDecimal = Convert.ToDecimal(priceTxBxAdd.Text, CultureInfo.CreateSpecificCulture("de-DE"));
                            //Add Euro to end
                            string tempPreisString = tempPreisDecimal.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));

                            var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                            optionsBuilder.UseSqlite("Filename=" + PathUser + @"\" + baseName);
                            using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                            {
                                var cost = new DataBaseUsingEF.Cost
                                {
                                    CategoryOfCost = comboBoxCostCategory.Text,
                                    CategoryUnderOfCost = tempComboBox.Text,
                                    NameOfCost = nameTxBxAdd.Text,
                                    PreisOfCost = tempPreisString,
                                    DateOfCost = dataPickerAdd.SelectedDate.Value.Date
                                };
                                dataBase.Costs.Add(cost);
                                dataBase.SaveChanges();
                            }
                            DialogResult = true;
                            //SerialCryptoDate();
                            cancelBtn_Click(sender, e);
                        }
                        else
                        {
                            priceTxBxAdd.Text = string.Empty;
                            DatenUpdateSourсe();
                            priceTxBxAdd.Focus();
                            lblError.Content = "Fehler! Sie haben wahrscheinlich Buchstaben eingeben";
                        }
                    }
                    else if (this.Title == "Einkommen hinzufügen")
                    {
                        if (decimal.TryParse(priceTxBxAdd.Text, out preis))
                        {
                            decimal tempPreisDecimal = Convert.ToDecimal(priceTxBxAdd.Text, CultureInfo.CreateSpecificCulture("de-DE"));
                            //Add Euro to end
                            string tempPreisString = tempPreisDecimal.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"));

                            var optionsBuilder = new DbContextOptionsBuilder<DataBaseUsingEF.DataBaseInFile>();
                            optionsBuilder.UseSqlite("Filename=" + PathUser + @"\" + baseName);

                            using (var dataBase = new DataBaseUsingEF.DataBaseInFile(optionsBuilder.Options))
                            {
                                var income = new DataBaseUsingEF.Income
                                {
                                    CategoryOfIncome = comboBoxIncomeCategory.Text,
                                    CategoryUnderOfIncome = tempComboBox.Text,
                                    NameOfIncome = nameTxBxAdd.Text,
                                    PreisOfIncome = tempPreisString,
                                    DateOfIncome = dataPickerAdd.SelectedDate.Value.Date
                                };
                                dataBase.Incomes.Add(income);
                                dataBase.SaveChanges();
                            }
                            DialogResult = true;
                            //SerialCryptoDate();
                            cancelBtn_Click(sender, e);
                        }
                        else
                        {
                            priceTxBxAdd.Text = string.Empty;
                            DatenUpdateSourсe();
                            priceTxBxAdd.Focus();
                            lblError.Content = "Fehler! Sie haben wahrscheinlich Buchstaben eingeben";
                        }
                    }
                }
            }
            catch(Exception em)
            {
                MessageBox.Show(em.Message);
            }
            #endregion

            #region SQLServer
            //decimal preis;
            //if (string.IsNullOrWhiteSpace(nameTxBxAdd.Text) || string.IsNullOrWhiteSpace(priceTxBxAdd.Text))
            //{
            //    if (string.IsNullOrWhiteSpace(nameTxBxAdd.Text))
            //    {
            //        nameTxBxAdd.Focus();
            //    }
            //    else
            //    {
            //        priceTxBxAdd.Focus();
            //    }
            //    DatenUpdateSourсe();
            //    lblError.Content = "Ошибка! Вы ввели не все данные";
            //}
            //else
            //{
            //    if (this.Title == "Добавить расходы")
            //    {
            //        if (decimal.TryParse(priceTxBxAdd.Text, out preis))
            //        {
            //            using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //            {
            //                var cost = new DataBaseUsingEF.Cost
            //                {
            //                    CategoryOfCost = comboBoxCostCategory.Text,
            //                    CategoryUnderOfCost = tempComboBox.Text,
            //                    NameOfCost = nameTxBxAdd.Text,
            //                    PreisOfCost = Convert.ToDecimal(priceTxBxAdd.Text, CultureInfo.InvariantCulture),
            //                    DateOfCost = dataPickerAdd.SelectedDate.Value.Date
            //                };
            //                dataBase.Costs.Add(cost);
            //                dataBase.SaveChanges();
            //            }
            //            DialogResult = true;
            //            SerialCryptoDate();
            //            cancelBtn_Click(sender, e);
            //        }
            //        else
            //        {
            //            priceTxBxAdd.Text = string.Empty;
            //            DatenUpdateSourсe();
            //            priceTxBxAdd.Focus();
            //            lblError.Content = "Ошибка! Возможно вы ввели букву";
            //        }
            //    }
            //    else if (this.Title == "Добавить доходы")
            //    {
            //        if (decimal.TryParse(priceTxBxAdd.Text, out preis))
            //        {
            //            using (var dataBase = new DataBaseUsingEF.DataBaseUsingEF(baseName))
            //            {
            //                var income = new DataBaseUsingEF.Income
            //                {
            //                    CategoryOfIncome = comboBoxIncomeCategory.Text,
            //                    CategoryUnderOfIncome = tempComboBox.Text,
            //                    NameOfIncome = nameTxBxAdd.Text,
            //                    PreisOfIncome = Convert.ToDecimal(priceTxBxAdd.Text, CultureInfo.InvariantCulture),
            //                    DateOfIncome = dataPickerAdd.SelectedDate.Value.Date
            //                };
            //                dataBase.Incomes.Add(income);
            //                dataBase.SaveChanges();
            //            }
            //            DialogResult = true;
            //            SerialCryptoDate();
            //            cancelBtn_Click(sender, e);
            //        }
            //        else
            //        {
            //            priceTxBxAdd.Text = string.Empty;
            //            DatenUpdateSourсe();
            //            priceTxBxAdd.Focus();
            //            lblError.Content = "Ошибка! Возможно вы ввели букву";
            //        }
            //    }
            //}
            #endregion
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //DeSerialCryptoDate();
            dataPickerAdd.SelectedDate = DateTime.Now;
            if (this.Title == "Ausgabe hinzufügen")
            {
                comboBoxIncomeCategory.Visibility = Visibility.Collapsed;
                comboBoxCostCategory.Visibility = Visibility.Visible;
                nameTxBxAdd.IsEnabled = false;
                priceTxBxAdd.IsEnabled = false;
                dataPickerAdd.IsEnabled = false;
                comboBoxCostCategory.SelectedIndex = 0;
                comboBoxCostUnderCategoryFiks.SelectedIndex = 0;
                comboBoxCostUnderCategoryFiks.IsEnabled = false;
                comboBoxUnderIncomeCategoryLohn.Visibility = Visibility.Collapsed;
                comboBoxCostCategory.Focus();
            }
            else if (this.Title == "Einkommen hinzufügen")
            {
                comboBoxCostUnderCategoryFiks.Visibility = Visibility.Collapsed;
                comboBoxCostCategory.Visibility = Visibility.Collapsed;
                comboBoxIncomeCategory.Visibility = Visibility.Visible;
                nameTxBxAdd.IsEnabled = false;
                priceTxBxAdd.IsEnabled = false;
                dataPickerAdd.IsEnabled = false;
                comboBoxIncomeCategory.SelectedIndex = 0;
                comboBoxUnderIncomeCategoryLohn.Visibility = Visibility.Visible;
                comboBoxUnderIncomeCategoryLohn.SelectedIndex = 0;
                comboBoxUnderIncomeCategoryLohn.IsEnabled = false;
                comboBoxIncomeCategory.Focus();
            }
        }

        #region Обновление источника для введенных строк
        void DatenUpdateSourсe()
        {
            comboBoxCostCategory.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
            //comboBoxCostUnderCategory.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
            nameTxBxAdd.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            priceTxBxAdd.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            LabelErrorStackPanel();
        }

        #endregion

        public void LabelErrorStackPanel()
        {
            lblError.Content = null;
        }

        #region Nach Category Auswahl
        private void UnderCategoryAendern(string name)
        {
            foreach (ComboBox item in gridUnderCategory.Children)
            {
                if (item is ComboBox)
                {
                    if (item.Name == name)
                    {
                        item.Visibility = Visibility.Visible;
                        item.SelectedIndex = 0;
                        item.IsEnabled = true;
                        item.Focus();
                        nameTxBxAdd.IsEnabled = false;
                        priceTxBxAdd.IsEnabled = false;
                        dataPickerAdd.IsEnabled = false;
                    }
                    else
                    {
                        item.Visibility = Visibility.Collapsed;
                        item.IsEnabled = false;
                        nameTxBxAdd.IsEnabled = false;
                        priceTxBxAdd.IsEnabled = false;
                        dataPickerAdd.IsEnabled = false;
                    }
                }
            }

        }
        //Изменение категории расходов
        private void CostCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Первое вхождение
            if (indexCategory == 0)
            {
                indexCategory++;
                return;
            }
            else
            {
                //Повторные вхождения
                if (comboBoxCostCategory.SelectedIndex == 0)
                {
                    //Отключаем все Combobox
                    foreach (ComboBox item in gridUnderCategory.Children)
                    {
                        if (item is ComboBox)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                    //Enable only one ComboBox
                    comboBoxCostUnderCategoryFiks.IsEnabled = false;
                    comboBoxCostUnderCategoryFiks.Visibility = Visibility.Visible;
                    comboBoxCostUnderCategoryFiks.SelectedIndex = 0;
                    comboBoxCostCategory.Focus();
                    nameTxBxAdd.Text = string.Empty;
                    priceTxBxAdd.Text = string.Empty;
                    //dataPickerAdd.SelectedDate = DateTime.Now;
                    nameTxBxAdd.IsEnabled = false;
                    priceTxBxAdd.IsEnabled = false;
                    dataPickerAdd.IsEnabled = false;
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Fixkosten")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryFiks");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Lebensmittel")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryProd");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Verkehr")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryTransport");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Kraftfahrzeug")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryAuto");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Unterhaltung")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryRazwlech");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Persönliche kosten")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategorySelbst");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Urlaub")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryReise");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Soft/Service")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategorySoft");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Alles ins Haus")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryHouse");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Bildung")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryStudium");
                }
                else if (((ComboBoxItem)comboBoxCostCategory.SelectedItem).Content.ToString() == "Andere")
                {
                    UnderCategoryAendern("comboBoxCostUnderCategoryAndere");
                }
            }
        }

        private void IncomeCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Первое вхождение
            if (indexCategory == 0)
            {
                indexCategory++;
                return;
            }
            else
            {
                //Повторные вхождения
                if (comboBoxIncomeCategory.SelectedIndex == 0)
                {
                    //Отключаем все Combobox
                    foreach (ComboBox item in gridUnderCategory.Children)
                    {
                        if (item is ComboBox)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                    //Enable only one ComboBox
                    comboBoxUnderIncomeCategoryLohn.IsEnabled = false;
                    comboBoxUnderIncomeCategoryLohn.Visibility = Visibility.Visible;
                    comboBoxUnderIncomeCategoryLohn.SelectedIndex = 0;
                    comboBoxIncomeCategory.Focus();
                    nameTxBxAdd.Text = string.Empty;
                    priceTxBxAdd.Text = string.Empty;
                    //dataPickerAdd.SelectedDate = DateTime.Now;
                    nameTxBxAdd.IsEnabled = false;
                    priceTxBxAdd.IsEnabled = false;
                    dataPickerAdd.IsEnabled = false;
                }
                else if (((ComboBoxItem)comboBoxIncomeCategory.SelectedItem).Content.ToString() == "Gehalt")
                {
                    UnderCategoryAendern("comboBoxUnderIncomeCategoryLohn");
                }
                else if (((ComboBoxItem)comboBoxIncomeCategory.SelectedItem).Content.ToString() == "Andere Einkommen")
                {
                    UnderCategoryAendern("comboBoxUnderIncomeCategoryAndere");
                }
            }
        }

        #endregion

        //Изменение подкатегории
        private void UnderCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Первое вхождение
            if (indexUnderCategory == 0)
            {
                indexUnderCategory++;
                return;
            }
            else
            {
                //Повторные вхождения
                foreach (ComboBox item in gridUnderCategory.Children)
                {
                    if (item is ComboBox)
                    {
                        if (item.IsEnabled && item.SelectedIndex != 0)
                        {
                            tempComboBox = item;
                            nameTxBxAdd.IsEnabled = true;
                            priceTxBxAdd.IsEnabled = true;
                            dataPickerAdd.IsEnabled = true;
                            nameTxBxAdd.Focus();
                            return;

                        }
                        else
                        {
                            nameTxBxAdd.IsEnabled = false;
                            priceTxBxAdd.IsEnabled = false;
                            dataPickerAdd.IsEnabled = false;
                        }
                    }
                }
            }
        }
    }

    //Показывает ошибки если не введены данные
    public class ErrorAbfangen : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ApplicationException("Bitte geben Sie den Namen");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        private string price;
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                if (string.IsNullOrWhiteSpace(price))
                {
                    throw new ApplicationException("Bitte geben Sie den Betrag");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Price"));
                }
            }
        }

        private ComboBoxItem category;
        public ComboBoxItem Category
        {
            get { return category; }
            set
            {
                category = value;
                if (value == null || value.Content.ToString() == "-- Wählen Sie eine Kategorie --")
                {
                    throw new ApplicationException("Bitte wählen Sie eine Kategorie");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Category"));
                }
            }
        }

        private ComboBoxItem underCategory;
        public ComboBoxItem UnderCategory
        {
            get { return underCategory; }
            set
            {
                underCategory = value;
                if (value == null || value.Content.ToString() == "-- Wählen Sie eine Unterkategorie --")
                {
                    throw new ApplicationException("Bitte wählen Sie eine Unterkategorie");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("UnderCategory"));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    [Serializable()]
    class DateTimeSpeichern
    {
        public DateTime TimeSpeichern { get; internal set; }

        public DateTime LetzteAnderung { get; internal set; }
    }
}
