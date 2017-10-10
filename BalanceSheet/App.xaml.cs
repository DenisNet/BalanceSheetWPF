using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SplashScreen = BalanceSheet.StartupScreen.SplashScreenUC;


namespace BalanceSheet
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var splashScreen = new SplashScreen();
            splashScreen.Show();
            //Отоброжает выданную лицензию
            splashScreen.Licensee = new StringBuilder("Hack Eva").ToString();

            //Task инициализируется
            var startTask = new Task(() =>
            {
                //Делаем паузу для симулирования загрузки
                Thread.Sleep(3000);
            });

            //закрываем SplashScreen при загрузке MainWindow
            startTask.ContinueWith(t =>
            {
                MainWindow startWindow = new MainWindow();
                //Закрываем SplashScreen
                startWindow.Loaded += (sender, args) => splashScreen.Close();
                //Показываем окно
                startWindow.Show();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startTask.Start();
        }
    }
}
