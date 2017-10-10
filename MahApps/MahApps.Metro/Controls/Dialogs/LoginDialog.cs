using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls.Dialogs
{
    public class LoginDialogSettings : MetroDialogSettings
    {
        private const string DefaultUsernameWatermark = "Login...";
        private const string DefaultPasswordWatermark = "Passwort...";
        private const Visibility DefaultNegativeButtonVisibility = Visibility.Collapsed;
        private const Visibility DefaultRegistrationButtonVisibility = Visibility.Collapsed;
        private const bool DefaultShouldHideUsername = false;
        private const bool DefaultEnablePasswordPreview = false;
        private const Visibility DefaultRememberCheckBoxVisibility = Visibility.Collapsed;
        private const string DefaultRememberCheckBoxText = "Merken";

        public LoginDialogSettings()
        {
            UsernameWatermark = DefaultUsernameWatermark;
            PasswordWatermark = DefaultPasswordWatermark;
            NegativeButtonVisibility = DefaultNegativeButtonVisibility;
            RegistrationButtonVisibility = DefaultRegistrationButtonVisibility;
            ShouldHideUsername = DefaultShouldHideUsername;
            AffirmativeButtonText = "Anmelden";
            EnablePasswordPreview = DefaultEnablePasswordPreview;
            RememberCheckBoxVisibility = DefaultRememberCheckBoxVisibility;
            RememberCheckBoxText = DefaultRememberCheckBoxText;
        }

        public string InitialUsername { get; set; }

        public string InitialPassword { get; set; }

        public string UsernameWatermark { get; set; }

        public bool ShouldHideUsername { get; set; }

        public string PasswordWatermark { get; set; }

        public Visibility NegativeButtonVisibility { get; set; }

        public Visibility RegistrationButtonVisibility { get; set; }

        public bool EnablePasswordPreview { get; set; }

        public Visibility RememberCheckBoxVisibility { get; set; }

        public string RememberCheckBoxText { get; set; }
    }

    public class LoginDialogData
    {
        public string Username { get; internal set; }
        public string Password { get; internal set; }
        public bool ShouldRemember { get; internal set; }
        public byte? ButtonAction { get; internal set; }
    }

    public partial class LoginDialog : BaseMetroDialog
    {
        internal LoginDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }
        private Client client;
        internal LoginDialog(MetroWindow parentWindow, LoginDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
            client = new Client();
            DataContext = client;
            Username = settings.InitialUsername;
            Password = settings.InitialPassword;
            UsernameWatermark = settings.UsernameWatermark;
            PasswordWatermark = settings.PasswordWatermark;
            NegativeButtonButtonVisibility = settings.NegativeButtonVisibility;
            RegistrationButtonButtonVisibility = settings.RegistrationButtonVisibility;
            ShouldHideUsername = settings.ShouldHideUsername;
            RememberCheckBoxVisibility = settings.RememberCheckBoxVisibility;
            RememberCheckBoxText = settings.RememberCheckBoxText;
        }

        public LoginDialog()
        {
        }

        internal Task<LoginDialogData> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                if (string.IsNullOrEmpty(PART_TextBox.Text) && !ShouldHideUsername)
                {
                    PART_TextBox.Focus();
                }
                else
                {
                    PART_TextBox2.Focus();
                }
            }));

            TaskCompletionSource<LoginDialogData> tcs = new TaskCompletionSource<LoginDialogData>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            RoutedEventHandler registrationHandler = null;
            KeyEventHandler registrationKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanUpHandlers();
                tcs.TrySetResult(null);
            });

            cleanUpHandlers = () =>
            {
                PART_TextBox.KeyDown -= affirmativeKeyHandler;
                PART_TextBox2.KeyDown -= affirmativeKeyHandler;

                this.KeyDown -= escapeKeyHandler;

                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;
                RegistrationButton.Click -= registrationHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
                RegistrationButton.KeyDown -= registrationKeyHandler;

                cancellationTokenRegistration.Dispose();
            };

            escapeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            negativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password, ButtonAction = null, ShouldRemember = RememberCheckBoxChecked });
                }
            };



        affirmativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();
                    tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password, ButtonAction = 1, ShouldRemember = RememberCheckBoxChecked });
                }
            };

            registrationKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();
                    tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password, ButtonAction = 2, ShouldRemember = RememberCheckBoxChecked });
                }
            };


            negativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password, ButtonAction = null, ShouldRemember = RememberCheckBoxChecked });

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password, ButtonAction = 1, ShouldRemember = RememberCheckBoxChecked });

                e.Handled = true;
            };

            registrationHandler = (sender, e) =>
            {
                cleanUpHandlers();
                tcs.TrySetResult(new LoginDialogData { Username = Username, Password = PART_TextBox2.Password, ButtonAction = 2, ShouldRemember = RememberCheckBoxChecked });

                e.Handled = true;
            };

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;
            RegistrationButton.KeyDown += registrationKeyHandler;

            PART_TextBox.KeyDown += affirmativeKeyHandler;
            PART_TextBox2.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;
            RegistrationButton.Click += registrationHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            var settings = this.DialogSettings as LoginDialogSettings;
            if (settings != null && settings.EnablePasswordPreview)
            {
                var win8MetroPasswordStyle = this.FindResource("Win8MetroPasswordBox") as Style;
                if (win8MetroPasswordStyle != null)
                {
                    PART_TextBox2.Style = win8MetroPasswordStyle;
                }
            }

            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;
            this.RegistrationButtonText = this.DialogSettings.RegistrationButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.Style = this.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register("UsernameWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty NegativeButtonButtonVisibilityProperty = DependencyProperty.Register("NegativeButtonButtonVisibility", typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty RegistrationButtonTextProperty = DependencyProperty.Register("RegistrationButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("Cancel"));

        public static readonly DependencyProperty RegistrationButtonButtonVisibilityProperty = DependencyProperty.Register("RegistrationButtonButtonVisibility", typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ShouldHideUsernameProperty = DependencyProperty.Register("ShouldHideUsername", typeof(bool), typeof(LoginDialog), new PropertyMetadata(false));
        public static readonly DependencyProperty RememberCheckBoxVisibilityProperty = DependencyProperty.Register("RememberCheckBoxVisibility", typeof(Visibility), typeof(LoginDialog), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty RememberCheckBoxTextProperty = DependencyProperty.Register("RememberCheckBoxText", typeof(string), typeof(LoginDialog), new PropertyMetadata("Remember"));
        public static readonly DependencyProperty RememberCheckBoxCheckedProperty = DependencyProperty.Register("RememberCheckBoxChecked", typeof(bool), typeof(LoginDialog), new PropertyMetadata(false));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public string UsernameWatermark
        {
            get { return (string)GetValue(UsernameWatermarkProperty); }
            set { SetValue(UsernameWatermarkProperty, value); }
        }

        public string PasswordWatermark
        {
            get { return (string)GetValue(PasswordWatermarkProperty); }
            set { SetValue(PasswordWatermarkProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)GetValue(AffirmativeButtonTextProperty); }
            set { SetValue(AffirmativeButtonTextProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }

        public Visibility NegativeButtonButtonVisibility
        {
            get { return (Visibility)GetValue(NegativeButtonButtonVisibilityProperty); }
            set { SetValue(NegativeButtonButtonVisibilityProperty, value); }
        }

        public string RegistrationButtonText
        {
            get { return (string)GetValue(RegistrationButtonTextProperty); }
            set { SetValue(RegistrationButtonTextProperty, value); }
        }

        public Visibility RegistrationButtonButtonVisibility
        {
            get { return (Visibility)GetValue(RegistrationButtonButtonVisibilityProperty); }
            set { SetValue(RegistrationButtonButtonVisibilityProperty, value); }
        }


        public bool ShouldHideUsername
        {
            get { return (bool)GetValue(ShouldHideUsernameProperty); }
            set { SetValue(ShouldHideUsernameProperty, value); }
        }

        public Visibility RememberCheckBoxVisibility
        {
            get { return (Visibility)GetValue(RememberCheckBoxVisibilityProperty); }
            set { SetValue(RememberCheckBoxVisibilityProperty, value); }
        }

        public string RememberCheckBoxText
        {
            get { return (string)GetValue(RememberCheckBoxTextProperty); }
            set { SetValue(RememberCheckBoxTextProperty, value); }
        }

        public bool RememberCheckBoxChecked
        {
            get { return (bool)GetValue(RememberCheckBoxCheckedProperty); }
            set { SetValue(RememberCheckBoxCheckedProperty, value); }
        }
    }


    public class Client : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _userLogin;
        public string UserLogin
        {
            get { return _userLogin; }
            set
            {
                _userLogin = value;
                if (string.IsNullOrWhiteSpace(_userLogin))
                {
                    //LabelStack = "";
                    throw new ApplicationException("Bitte geben Sie Ihren Benutzernamen.");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("UserLogin"));

                }
            }
        }

        private string _userPassword;
        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                _userPassword = value;
                if (string.IsNullOrWhiteSpace(_userPassword))
                {
                    //LabelStack = "";
                    throw new ApplicationException("Bitte geben Sie Ihr Passwort");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("UserPassword"));
                    //LabelStack = "";
                }
            }
        }

        private string _labelStack;
        public string LabelStack
        {
            get { return _labelStack; }
            set
            {
                _labelStack = value;
                if (string.IsNullOrWhiteSpace(_labelStack))
                {
                    //LabelStack = "";
                    throw new ApplicationException("Bitte geben Sie Ihr Passwort");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LabelStack"));
                    //LabelStack = "";
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDataErrorInfo Members

        // This is not used in XAML (was used in WinForms)
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string propertyName]
        {
            get
            {
                string userLogPass = "";
                if (propertyName == "UserPassword")
                {
                    if (string.IsNullOrWhiteSpace(_userPassword))
                        userLogPass = "Введите пожалуйста пароль"; ;
                }
                else if (propertyName == "UserLogin")
                {
                    if (string.IsNullOrWhiteSpace(_userPassword))
                        userLogPass = "Введите пожалуйста имя пользователя";
                }
                return userLogPass;
            }
        }

        #endregion

    }
}
