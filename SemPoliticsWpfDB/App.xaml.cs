using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Politics;

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
                var agents = db.users.Where(x => x.timezone > 3);
                foreach (users agent in agents)
                {
                    Console.WriteLine(agent.name);
                }
                Console.Read();
            }

        }
    }
}
