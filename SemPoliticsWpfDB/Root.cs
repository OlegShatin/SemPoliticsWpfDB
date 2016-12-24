using System;
using System.Linq;
using Politics;
using SemPoliticsWpfDB.ViewModels;
using SemPoliticsWpfDB.Commands;
using System.Windows.Input;
using System.Data.Entity.Infrastructure;
using System.Collections.ObjectModel;

namespace SemPoliticsWpfDB
{
    class Root
    {
        public ObservableCollection<ElectionViewModel> ElectionsList { get; set; }
        PoliticsDBContext context;
        public Root()
        {
            ElectionsList = new ObservableCollection<ElectionViewModel>();
            context = new PoliticsDBContext();
            ElectionViewModel.AllCandidatesList = new ObservableCollection<CandidateViewModel>();
            foreach (var candidate in context.Candidates)
            {
                ElectionViewModel.AllCandidatesList.Add(new CandidateViewModel(candidate, context));
            }

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

        private DelegateCommand _addElectionCommand;
        public ICommand AddElection => _addElectionCommand ?? (_addElectionCommand = new DelegateCommand(AddElectionMethod));
        private void AddElectionMethod()
        {
            Election election = new Election();
            election.ElectionType = context.ElectionTypes.First();
            election.StartTime = new DateTimeOffset(DateTimeOffset.Now.Date.AddHours(8));
            election.FinishTime = election.StartTime.AddHours(22);
            context.Elections.Add(election);
            ElectionsList.Add(new ElectionViewModel(election, context));

        }
    }
}

