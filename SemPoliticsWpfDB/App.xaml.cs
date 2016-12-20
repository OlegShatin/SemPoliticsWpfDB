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
                
                Console.ReadLine();
            }

        }
        class Root
        {
            
            public List<ElectionViewModel> PastElectionsList { get; set; }
            public Root()
            {
                PastElectionsList = new List<ElectionViewModel>();
                PoliticsDBContext context = new PoliticsDBContext();
                foreach (var election in context.Elections)
                {
                    PastElectionsList.Add(new ElectionViewModel(election, context));
                }
            }
        }
    }
}
