using Politics;
using SemPoliticsWpfDB.Commands;
using SemPoliticsWpfDB.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SemPoliticsWpfDB.ViewModels
{
    public class ElectionViewModel : BaseViewModel, IEquatable<ElectionViewModel>
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
        public ElectionViewModel(Election election, PoliticsDBContext context, Root root)
        {
            this._root = root;
            Election = election;
            _dbcontext = context;
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
        public Election Election { get; private set; }
        public String MainLabel { get; private set; }
        protected PoliticsDBContext _dbcontext;
        private Root _root;
        public static List<string> Types { get; set; }
        public String Type
        {
            get { return Election.TypeName; }
            set
            {
                Election.TypeName = value;
            }

        }
        public int Votes { get { return Election.VotedUsers.Count; } }
        public DateTime Date
        {
            get { return Election.StartTime.DateTime; }
            set
            {
                Election.StartTime = new DateTimeOffset(value, new TimeSpan(3, 0, 0)).AddHours(8);
                Election.FinishTime = new DateTimeOffset(value, new TimeSpan(3, 0, 0)).AddHours(20);
                MainLabel = MainLabel.Remove(MainLabel.IndexOf("РФ") + 3) + Election.StartTime.Day + "." + Election.StartTime.Month + "." + Election.StartTime.Year;
                OnPropertyChanged("Date");
                OnPropertyChanged("MainLabel");
            }
        }
        //change type command
        private DelegateCommand<string> _changeTypeCommand;
        public ICommand ChangeType => _changeTypeCommand ?? (_changeTypeCommand = new DelegateCommand<string>(ChangeTypeMethod, CanChangeType));
        private void ChangeTypeMethod(string newType)
        {
            Election.TypeName = newType;
            OnPropertyChanged("Type");
        }
        private bool CanChangeType(string s)
        {
            if (s != null && s.Equals(Type))
            {
                return true;
            }
            if (!(CurrentElectionCandidatesList.Count > 2))
            {
                foreach (var candidateVM in CurrentElectionCandidatesList)
                {
                    if (!(candidateVM.Candidate.Name.Equals("Испортить бюллетень") || candidateVM.Candidate.Name.Equals("Пустой бюллетень")))
                    {
                        MessageBox.Show("Снимите всех не универсальных кандидатов с этих выборов перед изменением типа выборов",
                    "Предупреждение", MessageBoxButton.OK);
                        OnPropertyChanged("Type");
                        return false;
                    }
                }
                return true;
            }
            MessageBox.Show("Снимите всех не универсальных кандидатов с этих выборов перед изменением типа выборов",
                    "Предупреждение", MessageBoxButton.OK);
            OnPropertyChanged("Type");
            return false;
        }

        //show candidates list command
        //needed to be defined from the otside before using        
        public ObservableCollection<CandidateViewModel> AllCandidatesList { get { return _root.AllCandidatesList; } }
        public ObservableCollection<CandidateViewModel> AllCurrentTypeCandidatesList
        {
            get
            {
                ObservableCollection<CandidateViewModel> result = new ObservableCollection<CandidateViewModel>();
                if (Election.TypeName.Equals("PARLIAMENT"))
                {                    
                    foreach (var candidate in AllCandidatesList.Where(x => x.Candidate.PartyId != null && !CurrentElectionCandidatesList.Contains(x)))
                    {
                        result.Add(candidate);
                    }

                    return result;
                }
                else
                {
                    foreach (var candidate in AllCandidatesList.Where(x => (x.Candidate.PartyId == null 
                    || x.Candidate.Name.Equals("Испортить бюллетень") || x.Candidate.Name.Equals("Пустой бюллетень")) && !CurrentElectionCandidatesList.Contains(x)))
                    {
                        result.Add(candidate);
                    }
                    return result;
                }
            }
            private set { }
        }
        private ObservableCollection<CandidateOnElectionViewModel> _currentElectionCandidatesList;
        public virtual ObservableCollection<CandidateOnElectionViewModel> CurrentElectionCandidatesList
        {
            get
            {
                if (_currentElectionCandidatesList == null)
                {

                    _currentElectionCandidatesList = new ObservableCollection<CandidateOnElectionViewModel>();
                    foreach (var candidateOnThisEleciton in (from candidateItem in _dbcontext.CandidatesList.Local
                                                             where candidateItem.ElectionId == Election.Id
                                                             join candidate in _dbcontext.Candidates
                                                             on candidateItem.CandidateId equals candidate.Id
                                                             select candidateItem).ToList())
                    {
                        _currentElectionCandidatesList.Add(new CandidateOnElectionViewModel(candidateOnThisEleciton, _dbcontext));
                    }
                }
                return _currentElectionCandidatesList;
            }
            set
            {
                _currentElectionCandidatesList = value;
                OnPropertyChanged("CurrentElectionCandidatesList");
            }
        }
        
        private DelegateCommand _showCandidatesCommand;
        public ICommand ShowCandidates => _showCandidatesCommand ?? (_showCandidatesCommand = new DelegateCommand(ShowCandidatesMethod));
        private void ShowCandidatesMethod()
        {            
            CandidatesOnElectionWindow w = new CandidatesOnElectionWindow();
            w.DataContext = this;
            w.Show();
        }

        //add to candiates list from AllCandidates command
        private DelegateCommand<object> _addCandidatesToElectionCommand;

        public ICommand AddCandidatesToElection => _addCandidatesToElectionCommand ??
            (_addCandidatesToElectionCommand = new DelegateCommand<object>(AddCandidatesToElectionCommandMethod));

        private void AddCandidatesToElectionCommandMethod(object toAddCandidatesRaw)
        {
            System.Collections.IList items = (System.Collections.IList)toAddCandidatesRaw;
            foreach (var candidate in items.Cast<CandidateViewModel>())
            {
                CandidateOnElection candidateItem = new CandidateOnElection();                
                candidateItem.CandidateId = candidate.Candidate.Id;
                candidateItem.ElectionId = Election.Id;
                candidateItem.VotesForCandidate = 0;
                _dbcontext.CandidatesList.Add(candidateItem);
                CurrentElectionCandidatesList.Add(new CandidateOnElectionViewModel(candidateItem, _dbcontext));
            }
            OnPropertyChanged("AllCurrentTypeCandidatesList");
            OnPropertyChanged("CurrentElectionCandidatesList");
        }
        //remove candiates list from AllCandidates command
        private DelegateCommand<object> _removeCandidatesFromElectionCommand;

        public ICommand RemoveCandidatesFromElection => _removeCandidatesFromElectionCommand ??
            (_removeCandidatesFromElectionCommand = new DelegateCommand<object>(removeCandidatesFromElectionCommandMethod));

        private void removeCandidatesFromElectionCommandMethod(object toRemoveCandidatesRaw)
        {
            bool someCandidatesWasNotRemoved = false;
            System.Collections.IList items = (System.Collections.IList)toRemoveCandidatesRaw;            
            foreach (var candidate in items.Cast<CandidateOnElectionViewModel>().ToList())
            {
                if (candidate.Votes == 0)
                {
                    //check is this election was just added or it was got from db
                    if (_dbcontext.Entry(Election).State == System.Data.Entity.EntityState.Added)
                    {
                        _dbcontext.CandidatesList.Local.Remove(_dbcontext.CandidatesList.Local.Where(x => x.Candidate == candidate.Candidate && x.Election == Election).FirstOrDefault());
                    }
                    else
                    {
                        _dbcontext.CandidatesList.Remove(_dbcontext.CandidatesList.Local.Where(x => x.CandidateId == candidate.Candidate.Id && x.Election.Id == Election.Id).FirstOrDefault());
                    }
                    CurrentElectionCandidatesList.Remove(candidate);
                }
                else
                {
                    someCandidatesWasNotRemoved = true;
                }
                
            }
            if (someCandidatesWasNotRemoved)
            {
                MessageBox.Show("Кандидатов, за которых проголосовали, нельзя снять с выборов",
                    "Предупреждение", MessageBoxButton.OK);
            }
            OnPropertyChanged("AllCurrentTypeCandidatesList");
            OnPropertyChanged("CurrentElectionCandidatesList");
        }


        public bool Equals(ElectionViewModel other)
        {
            return Election.Equals(other.Election);
        }

    }
}
