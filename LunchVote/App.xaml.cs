using LunchVote.IoC;
using LunchVote.VM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LunchVote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender1, StartupEventArgs e)
        {
            IoCWorker.ConfigureFake();
            LaunchVoteView win = new LaunchVoteView(new VMVotacao());
            win.Show();
        }
    }
}
