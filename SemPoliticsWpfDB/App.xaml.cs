using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Politics;
using SemPoliticsWpfDB.Views;

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
                w.Show();
                
                Console.ReadLine();
            }

        }
    }
}
