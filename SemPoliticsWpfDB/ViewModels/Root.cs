﻿using System;
using System.Linq;
using Politics;
using SemPoliticsWpfDB.Commands;
using System.Windows.Input;
using System.Data.Entity.Infrastructure;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;


namespace SemPoliticsWpfDB.ViewModels
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

            VotersVMList = new ObservableCollection<UserViewModel>();
            foreach (var voter in context.Users.Where(x => x.RoleName.Equals("USER")).ToList())
            {
                VotersVMList.Add(new UserViewModel(voter, context));
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
                        context.Elections.Local.Remove(context.Elections.Local.Where(x => x == electionVM.Election).FirstOrDefault());
                    }
                    else
                    {
                        context.Elections.Remove(context.Elections.Local.Where(x => x.Id == electionVM.Election.Id).FirstOrDefault());
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

        //users tab
        public UserViewModel AdminVM { get; set; }
        public ObservableCollection<UserViewModel> VotersVMList { get; set; }
        public ObservableCollection<UserViewModel> AgentsVMList { get; set; }

        //add new Voter and Agent buttons
        private DelegateCommand _addVoterCommand;
        public ICommand AddVoter => _addVoterCommand ?? (_addVoterCommand = new DelegateCommand(AddVoterMethod));
        private DelegateCommand _addAgentCommand;
        public ICommand AddAgent => _addAgentCommand ?? (_addAgentCommand = new DelegateCommand(AddAgentMethod));
        private void AddVoterMethod()
        {
            while (VotersVMList.Where(x => x.User.Email.Contains("newUSER_" + _newUsersCounter + "@example.com")).Count() > 0)
            {
                _newUsersCounter++;
            }
            var newVM = new UserViewModel(AddUser("USER"), context);
            VotersVMList.Add(newVM);
            OnPropertyChanged("VotersVMList");
            newVM.Password = "1234";
        }
        private void AddAgentMethod()
        {
            while (AgentsVMList.Where(x => x.User.Email.Contains("newAGENT_" + _newUsersCounter + "@example.com")).Count() > 0)
            {
                _newUsersCounter++;
            }
            var newVM = new UserViewModel(AddUser("AGENT"), context);
            AgentsVMList.Add(newVM);
            OnPropertyChanged("VotersVMList");
            newVM.Password = "1234";
        }
        private int _newUsersCounter = 0;
        private User AddUser(string role)
        {            
            User newUser = new User()
            {
                Name = "newUser_" + _newUsersCounter,
                Surname = "newUser_" + _newUsersCounter,
                Patronymic = "newUser_" + _newUsersCounter,
                PassportNumber = "000000",
                PassportSeries = "0000",
                //password: 1234
                PasswordHash = "1234",
                Email = "new" + role + "_" + _newUsersCounter + "@example.com",
                RoleName = role,
                Role = context.UserRoles.Where(x => x.Role.Equals(role)).FirstOrDefault(),
                Timezone = 3,
                Birthday = new DateTime(1970, 1, 1)

            };
            context.Users.Add(newUser);
            return newUser;            
        }

        //remove Voters and Agents button
        private DelegateCommand<object> _removeVotersCommand;
        public ICommand RemoveVoters => _removeVotersCommand ??
            (_removeVotersCommand = new DelegateCommand<object>(removeVotersCommandMethod));


        private void removeVotersCommandMethod(object toRemoveVotersRaw)
        {
            bool someVotersWasNotRemoved = false;
            System.Collections.IList items = (System.Collections.IList)toRemoveVotersRaw;
            foreach (var voterVM in items.Cast<UserViewModel>().ToList())
            {
                //check is this voter was just added or it was got from db
                if (voterVM.User?.VotedElections.Count == 0)
                {
                    if (context.Entry(voterVM.User).State == EntityState.Added)
                    {
                        context.Users.Local.Remove(context.Users.Local.Where(x => x == voterVM.User).FirstOrDefault());
                    }
                    else
                    {
                        context.Users.Remove(context.Users.Local.Where(x => x.Id == voterVM.User.Id).FirstOrDefault());
                    }
                    VotersVMList.Remove(voterVM);
                }
                else
                {
                    someVotersWasNotRemoved = true;
                }
            }
            OnPropertyChanged("VotersVMList");
            if (someVotersWasNotRemoved)
            {
                MessageBox.Show("Некоторые голосующие не были удалены, т.к. содержат зарегестрированных кандидатов. Удалите всех кандидатов из списка кандидатов выборов перед удалением",
                    "Предупреждение", MessageBoxButton.OK);
            }
        }

        private DelegateCommand<object> _removeAgentsCommand;
        public ICommand RemoveAgents => _removeAgentsCommand ??
            (_removeAgentsCommand = new DelegateCommand<object>(removeAgentsCommandMethod));

        private void removeAgentsCommandMethod(object toRemoveAgentsRaw)
        {
            bool someAgentsWasNotRemoved = false;
            System.Collections.IList items = (System.Collections.IList)toRemoveAgentsRaw;
            foreach (var agentVM in items.Cast<UserViewModel>().ToList())
            {
                //check is this agent was just added or it was got from db
                if (agentVM.User?.VotedElections.Count == 0)
                {
                    if (context.Entry(agentVM.User).State == EntityState.Added)
                    {
                        context.Users.Local.Remove(context.Users.Local.Where(x => x == agentVM.User).FirstOrDefault());
                    }
                    else
                    {
                        context.Users.Remove(context.Users.Local.Where(x => x.Id == agentVM.User.Id).FirstOrDefault());
                    }
                    AgentsVMList.Remove(agentVM);
                }
                else
                {
                    someAgentsWasNotRemoved = true;
                }
            }
            OnPropertyChanged("AgentsVMList");
            if (someAgentsWasNotRemoved)
            {
                MessageBox.Show("Некоторые Агенты не были удалены, т.к. содержат зарегестрированных кандидатов. Удалите всех кандидатов из списка кандидатов выборов перед удалением",
                    "Предупреждение", MessageBoxButton.OK);
            }
        }
    }



    
}

