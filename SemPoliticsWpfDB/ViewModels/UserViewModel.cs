using Politics;
using SemPoliticsWpfDB.Commands;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SemPoliticsWpfDB.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private PoliticsDBContext context;
        public User User { get; set; }

        public UserViewModel(User user, PoliticsDBContext context)
        {
            this.User = user;
            this.context = context;
        }
        public static string Hash(string input)
        {
            return string.Join("", (new System.Security.Cryptography.SHA1Managed().ComputeHash(System.Text.Encoding.UTF8.GetBytes(input))).Select(x => x.ToString("x2")).ToArray());
        }

        public string Name
        {
            get { return User.Name; }
            set
            {
                User.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Surname
        {
            get { return User.Surname; }
            set
            {
                User.Surname = value;
                OnPropertyChanged("Surname");
            }
        }

        public string Patronymic
        {
            get { return User.Patronymic; }
            set
            {
                User.Patronymic = value;
                OnPropertyChanged("Patronymic");
            }
        }

        public string Password
        {
            get { return User.PasswordHash; }
            set
            {
                User.PasswordHash = Hash(value);
                OnPropertyChanged("Password");
            }
        }
        private string _inputedText = "";
        private DelegateCommand<string> _changePasswordTextCommand;
        public ICommand ChangePasswordText => _changePasswordTextCommand ?? (_changePasswordTextCommand = new DelegateCommand<string>(ChangePasswordTextMethod));
        private void ChangePasswordTextMethod(string newText)
        {
            _inputedText = newText;
        }
        private DelegateCommand _changePasswordCommand;
        public ICommand ChangePassword => _changePasswordCommand ?? (_changePasswordCommand = new DelegateCommand(ChangePasswordMethod));
        private void ChangePasswordMethod()
        {
            if (!(_inputedText.Equals("") || _inputedText.Equals(Password)))
            {
                Password = _inputedText;
                _inputedText = "";
            } else
            {
                OnPropertyChanged("Password");
            }
        }

        public string Email
        {
            get { return User.Email; }
            set
            {
                //if there is any users with same email then cannot change email
                if (context.Users.Local.Where(x => x.Email.Equals(value)).Count() > 0)
                {
                    MessageBox.Show("Пользователь с таким email уже существует",
                    "Предупреждение", MessageBoxButton.OK);
                } else
                {
                    User.Email = value;
                }                
                OnPropertyChanged("Email");
            }
        }

        public string PassNum
        {
            get { return User.PassportNumber; }
            set
            {
                //if there is any users with same PassNum && PassSer then cannot change PassNum
                if (value.Length == 6)
                {
                    if (!value.Equals("000000") && context.Users.Local.Where(x => x.PassportNumber.Equals(value) && x.PassportSeries.Equals(User.PassportSeries)).Count() > 0)
                    {
                        MessageBox.Show("Пользователь с таким номером-серией паспорта уже существует. Вы можете использовать зарезервированные значения 0000 и 000000, чтобы создать пользователей с одинаковыми номером-серией паспорта",
                        "Предупреждение", MessageBoxButton.OK);
                    }
                    else
                    {
                        User.PassportNumber = value;
                    }
                } else
                {
                    MessageBox.Show("Номер паспорта состоит из 6 цифр",
                        "Предупреждение", MessageBoxButton.OK);
                }
                
                OnPropertyChanged("PassNum");
            }
        }

        public string PassSer
        {
            get { return User.PassportSeries; }
            set
            {
                //if there is any users with same PassSer && PassNum then cannot change PassSer
                if (value.Length == 4)
                {
                    if (!value.Equals("0000") && context.Users.Local.Where(x => x.PassportNumber.Equals(User.PassportNumber) && x.PassportSeries.Equals(value)).Count() > 0)
                    {
                        MessageBox.Show("Пользователь с таким номером-серией паспорта уже существует. Вы можете использовать зарезервированные значения 0000 и 000000, чтобы создать пользователей с одинаковыми номером-серией паспорта",
                        "Предупреждение", MessageBoxButton.OK);
                    }
                    else
                    {
                        User.PassportSeries = value;
                    }
                }
                else
                {
                    MessageBox.Show("Серия паспорта состоит из 4 цифр",
                        "Предупреждение", MessageBoxButton.OK);
                }

                OnPropertyChanged("PassSer");
            }
        }

        public string Timezone
        {
            get { return User.Timezone.ToString(); }
            set
            {
                int resultValue = 0;
                if (int.TryParse(value, out resultValue))
                {
                    if (resultValue >= -12 && resultValue <= 14)
                    {
                        User.Timezone = resultValue;
                    } else
                    {
                        MessageBox.Show("Значения временных зон от -12 до 14",
                        "Предупреждение", MessageBoxButton.OK);
                    }
                    
                } else
                {
                    if (value.Equals(""))
                    {
                        User.Timezone = 0;
                    } else
                    {
                        MessageBox.Show("Только целые числа",
                        "Предупреждение", MessageBoxButton.OK);
                    }
                    
                }            
                OnPropertyChanged("Timezone");
            }
        }

    }
}