﻿using Politics;
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
using System.Windows.Input;

namespace SemPoliticsWpfDB.ViewModels
{
    class ElectionViewModel : BaseViewModel, IEquatable<ElectionViewModel>
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
        }
        private bool CanChangeType(string s)
        {
            //todo: 
            return true;
        }

        //show candidates list command
        //needed to be defined from the otside before using        
        public static ObservableCollection<CandidateViewModel> AllCandidatesList { get; set; }
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
                    foreach (var candidate in AllCandidatesList.Where(x => x.Candidate.PartyId == null && !CurrentElectionCandidatesList.Contains(x)))
                    {
                        result.Add(candidate);
                    }
                    return result;
                }
            }
            private set { }
        }
        public ObservableCollection<CandidateViewModel> CurrentElectionCandidatesList { get; set; }
        
        private DelegateCommand _showCandidatesCommand;
        public ICommand ShowCandidates => _showCandidatesCommand ?? (_showCandidatesCommand = new DelegateCommand(ShowCandidatesMethod));
        private void ShowCandidatesMethod()
        {
            if (CurrentElectionCandidatesList == null)
            {
                CurrentElectionCandidatesList = new ObservableCollection<CandidateViewModel>();
                foreach (var candidateOnThisEleciton in from candidateItem in _dbcontext.CandidatesList
                                                        where candidateItem.ElectionId == Election.Id
                                                        join candidate in _dbcontext.Candidates
                                                        on candidateItem.CandidateId equals candidate.Id
                                                        select candidate)
                {
                    CurrentElectionCandidatesList.Add(new CandidateViewModel(candidateOnThisEleciton, _dbcontext));
                }
            }            
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
                candidateItem.Candidate = candidate.Candidate;
                candidateItem.Election = Election;
                candidateItem.VotesForCandidate = 0;
                candidateItem.CandidateId = candidate.Candidate.Id;
                candidateItem.ElectionId = Election.Id;
                Election.CandidatesList.Add(candidateItem);
                candidate.Candidate.ElectionsResults.Add(candidateItem);
                _dbcontext.CandidatesList.Add(candidateItem);
                CurrentElectionCandidatesList.Add(candidate);
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
            System.Collections.IList items = (System.Collections.IList)toRemoveCandidatesRaw;
            var buffer = new List<CandidateViewModel>();
            foreach (var candidate in items.Cast<CandidateViewModel>())
            {
                buffer.Add(candidate);
            }
            foreach (var candidate in buffer)
            {
                //todo: write delete process
                //_dbcontext.CandidatesList.Remove(_dbcontext.CandidatesList.Where(x => x.CandidateId == candidate.Candidate.Id && x.Election.Id == Election.Id).FirstOrDefault());                
                CurrentElectionCandidatesList.Remove(candidate);
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
