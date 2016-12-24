using System;
using System.Linq;
using Politics;
using SemPoliticsWpfDB.ViewModels;
using SemPoliticsWpfDB.Commands;
using System.Windows.Input;
using System.Data.Entity.Infrastructure;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;

namespace SemPoliticsWpfDB
{
    class Root : BaseViewModel
    {
        public ObservableCollection<ElectionViewModel> ElectionsList { get; set; }
        PoliticsDBContext context;
        public Root()
        {
            ElectionsList = new ObservableCollection<ElectionViewModel>();
            context = new PoliticsDBContext();
            ElectionViewModel.AllCandidatesList = new ObservableCollection<CandidateViewModel>();
                     
            foreach (var candidate in context.Candidates.ToList())
            {
                ElectionViewModel.AllCandidatesList.Add(new CandidateViewModel(candidate, context));
            }
            context.CandidatesList.Load();

            foreach (var election in context.Elections)
            {
                ElectionsList.Add(new ElectionViewModel(election, context));
            }           
            
        }

        private DelegateCommand _saveChangesCommand;
        public ICommand SaveChanges => _saveChangesCommand ?? (_saveChangesCommand = new DelegateCommand(SaveChangesMethod));
        private void SaveChangesMethod()
        {
            try
            {
                    
                context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //add new election button
        private DelegateCommand _addElectionCommand;
        public ICommand AddElection => _addElectionCommand ?? (_addElectionCommand = new DelegateCommand(AddElectionMethod));
        private void AddElectionMethod()
        {
            Election election = new Election();
            election.ElectionType = context.ElectionTypes.First();
            election.StartTime = new DateTimeOffset(DateTimeOffset.Now.Date.AddHours(8));
            election.FinishTime = election.StartTime.AddHours(22);
            context.Elections.Add(election);
            CandidateOnElection defaultCand1 = new CandidateOnElection
            {
                CandidateId = context.Candidates.Where(x => x.Name.Equals("Пустой бюллетень")).Select(x => x.Id).FirstOrDefault(),
                VotesForCandidate = 0,
                Election = election

            };
            CandidateOnElection defaultCand2 = new CandidateOnElection
            {
                CandidateId = context.Candidates.Where(x => x.Name.Equals("Испортить бюллетень")).Select(x => x.Id).FirstOrDefault(),
                VotesForCandidate = 0,
                Election = election

            };
            context.CandidatesList.Add(defaultCand1);
            context.CandidatesList.Add(defaultCand2);
            ElectionsList.Add(new ElectionViewModel(election, context));
        }

        //remove elections button
        private DelegateCommand<object> _removeElectionsCommand;

        public ICommand RemoveElections => _removeElectionsCommand ??
            (_removeElectionsCommand = new DelegateCommand<object>(removeElectionsCommandMethod));

        private void removeElectionsCommandMethod(object toRemoveElectionsRaw)
        {
            bool someElectionsWasNotRemoved = false;
            System.Collections.IList items = (System.Collections.IList)toRemoveElectionsRaw;            
            foreach (var electionVM in items.Cast<ElectionViewModel>().ToList())
            {
                //check is this election was just added or it was got from db
                if (electionVM.CurrentElectionCandidatesList.Count == 0)
                {
                    if (context.Entry(electionVM.Election).State == EntityState.Added)
                    {
                        context.Elections.Local.Remove(context.Elections.Local.Where(x => x == electionVM.Election && x == electionVM.Election).FirstOrDefault());
                    }
                    else
                    {
                        context.Elections.Remove(context.Elections.Local.Where(x => x.Id == electionVM.Election.Id && x.Id == electionVM.Election.Id).FirstOrDefault());
                    }
                    ElectionsList.Remove(electionVM);
                } 
                else
                {
                    someElectionsWasNotRemoved = true;
                }               
            }
            OnPropertyChanged("ElectionsList");
            if (someElectionsWasNotRemoved)
            {
                MessageBox.Show("Некоторые выборы не были удалены, т.к. содержат зарегестрированных кандидатов. Удалите всех кандидатов из списка кандидатов выборов перед удалением",
                    "Предупреждение", MessageBoxButton.OK);
            }
        }
    }
}

