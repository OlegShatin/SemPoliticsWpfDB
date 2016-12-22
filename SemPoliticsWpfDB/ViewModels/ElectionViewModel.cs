using Politics;
using SemPoliticsWpfDB.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SemPoliticsWpfDB.ViewModels
{
    class ElectionViewModel : BaseViewModel
    {
        static ElectionViewModel()
        {
            Types = new List<string>();
            foreach (var type in new PoliticsDBContext().ElectionTypes)
            {
                Types.Add(type.Type);
            }
        }
        public ElectionViewModel()
        {

        }
        public ElectionViewModel(Election election, PoliticsDBContext context)
        {
            Election = election;
            dbcontext = context;
            MainLabel = "Выборы ";
            if (Election.TypeName.Equals("PARLIAMENT"))
            {
                MainLabel += "в Госдуму РФ";
            }
            if (Election.TypeName.Equals("PRESIDENT"))
            {
                MainLabel += "президента РФ";
            }
            MainLabel += " " + Election.StartTime.Day + "." + Election.StartTime.Month + "." + Election.StartTime.Year;
            Type = election.TypeName;
        }
        public Election Election { get; set; }
        public String MainLabel { get; private set; }
        protected static PoliticsDBContext dbcontext;
        public static List<string> Types { get; set; }
        public String Type
        {
            get { return Election.TypeName; }
            set
            {
                Election.TypeName = value;
            }

        }
        public DateTime Date
        {
            get { return Election.StartTime.DateTime; }
            set
            {                
                Election.StartTime = new DateTimeOffset(value,new TimeSpan(3,0,0)).AddHours(8);
                Election.FinishTime = new DateTimeOffset(value, new TimeSpan(3, 0, 0)).AddHours(20);
                MainLabel = MainLabel.Remove(MainLabel.IndexOf("РФ") + 3) + Election.StartTime.Day + "." + Election.StartTime.Month + "." + Election.StartTime.Year;
                OnPropertyChanged("Date");
                OnPropertyChanged("MainLabel");
            }
        }
        private DelegateCommand<string> _changeTypeCommand;
        public ICommand ChangeType => _changeTypeCommand ?? (_changeTypeCommand = new DelegateCommand<string>(ChangeTypeMethod, CanChangeType));
        private void ChangeTypeMethod(string newType)
        {
            Election.TypeName = newType;
        }
        private bool CanChangeType(string s)
        {
            //todo: 
            return true;
        }

        

    }
}
