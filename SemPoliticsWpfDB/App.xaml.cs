using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Politics;
using SemPoliticsWpfDB.Views;
using SemPoliticsWpfDB.ViewModels;
using SemPoliticsWpfDB.Commands;
using System.Windows.Input;
using System.Data.Entity.Infrastructure;
using System.Collections.ObjectModel;

namespace SemPoliticsWpfDB
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
           using (var db = new PoliticsDBContext())
            {
                var agents = db.Users.Where(x => x.Email.Contains("agent"));
                foreach (User agent in agents)
                {
                    Console.WriteLine(agent.Name);
                }
                MainWindow w = new MainWindow();

                Root root = new Root();
                w.DataContext = root;                
                w.Show();
                MessageBox.Show(w, "SomeText", "Caption", MessageBoxButton.OK);
                Console.ReadLine();
            }

        }
        class Root
        {            
            public ObservableCollection<ElectionViewModel> ElectionsList { get; set; }
            PoliticsDBContext context;
            public Root()
            {
                ElectionsList = new ObservableCollection<ElectionViewModel>();
                context = new PoliticsDBContext();
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
                ElectionsList.Add(new ElectionViewModel(election,context));

            }
        }
    }
}
