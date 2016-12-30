using Politics;

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
                User.Patronymic = value;
                OnPropertyChanged("Password");
            }
        }
    }
}