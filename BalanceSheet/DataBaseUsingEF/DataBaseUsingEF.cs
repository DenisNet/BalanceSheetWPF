using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

using System.Data.EntityClient;
using System.Data.Metadata.Edm;




//using Microsoft.Data.Entity;

namespace BalanceSheet.DataBaseUsingEF
{

    public class DataBaseUsingEF : System.Data.Entity.DbContext
    {
        // Контекст настроен для использования строки подключения "DataBaseUsingEF" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "BalanceSheet.DataBaseUsingEF.DataBaseUsingEF" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "DataBaseUsingEF" 
        // в файле конфигурации приложения.
        public DataBaseUsingEF(string connection)
            : base(connection)
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.
        public System.Data.Entity.DbSet<User> Users { get; set; }
        public System.Data.Entity.DbSet<Login> Logins { get; set; }
        public System.Data.Entity.DbSet<Income> Incomes { get; set; }
        public System.Data.Entity.DbSet<Cost> Costs { get; set; }
        public System.Data.Entity.DbSet<Kontakt> Kontakts { get; set; }
    }
    class DataBaseInFile : Microsoft.Data.Entity.DbContext
    {
        public DataBaseInFile(Microsoft.Data.Entity.Infrastructure.DbContextOptions<DataBaseInFile> options) : base(options)
        {
        }

        public Microsoft.Data.Entity.DbSet<User> Users { get; set; }
        public Microsoft.Data.Entity.DbSet<Login> Logins { get; set; }
        public Microsoft.Data.Entity.DbSet<Income> Incomes { get; set; }
        public Microsoft.Data.Entity.DbSet<Cost> Costs { get; set; }
        public Microsoft.Data.Entity.DbSet<Kontakt> Kontakts { get; set; }
    }


    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите фамилию")]
        public string Nachname { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите имя")]
        public string Vorname { get; set; }

        [MaxLength(50)]
        public string Middlename { get; set; }

        [MaxLength(8)]
        public string Titel { get; set; }

        [Required(ErrorMessage = "Введите дату рождения")]
        public DateTime Geburtsdatum { get; set; }

        [MaxLength(50)]
        public string Company { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите предпочтение языка")]
        public string PrefferedLanguage { get; set; }

        [Required(ErrorMessage = "Введите дату регистрации")]
        public DateTime DateOfRegistration { get; set; }

        [Required()]
        public DateTime DateOfModify { get; set; }
    }

    public class Login
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50), Required(ErrorMessage = "Введите логин")]
        public string Login1 { get; set; }
        [MaxLength(50), Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
        [Required()]
        public string DateOfModify { get; set; }
    }

    public class Income
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите название дохода")]
        public string NameOfIncome { get; set; }

        //[Required(ErrorMessage = "Введите сумму")]
        //public decimal PreisOfIncome { get; set; }
        [Required(ErrorMessage = "Введите сумму")]
        public string PreisOfIncome { get; set; }


        [Required(ErrorMessage = "Введите дату")]
        public DateTime DateOfIncome { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Выберите категорию")]
        public string CategoryOfIncome { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Выберите подкатегорию")]
        public string CategoryUnderOfIncome { get; set; }
    }

    public class Cost
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите название расходов")]
        public string NameOfCost { get; set; }

        [Required(ErrorMessage = "Введите сумму")]
        public string PreisOfCost { get; set; }

        //[Required(ErrorMessage = "Введите сумму")]
        //public decimal PreisOfCost { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        public DateTime DateOfCost { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Выберите категорию")]
        public string CategoryOfCost { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Выберите подкатегорию")]
        public string CategoryUnderOfCost { get; set; }
    }

    public class Kontakt
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите страну")]
        public string Land { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите город")]
        public string City { get; set; }

        [MaxLength(5), Required(ErrorMessage = "Введите почтовый индекс")]
        public string PLZ { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите название улицы")]
        public string Strasse { get; set; }

        [MaxLength(1000), Required(ErrorMessage = "Введите номер")]
        public string Nummer { get; set; }

        [MaxLength(50), Required(ErrorMessage = "Введите @mail")]
        public string Mail { get; set; }

        [MaxLength(100)]
        public string Telefon { get; set; }
    }
}