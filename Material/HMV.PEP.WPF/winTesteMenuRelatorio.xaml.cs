using System;
using System.Reflection;
using System.Windows;
using System.IO;
using HMV.Core.Framework.WPF;
using HMV.Core.DTO;
using HMV.Core.Interfaces;
using HMV.PEP.WPF.UserControls;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winTesteMenuRelatorio.xaml
    /// </summary>
    public partial class winTesteMenuRelatorio : WindowBase
    {
        public winTesteMenuRelatorio()
        {
            String stringConexao = System.Configuration.ConfigurationManager.ConnectionStrings["BANCO"].ToString();
            HMV.Core.DataAccess.SessionManager.ConfigureDataAccess(stringConexao.Replace("@BANCO", App.Banco),
            System.Configuration.ConfigurationManager.AppSettings["ConfigNHibernate"].ToString()
            + this.GetType().Assembly.GetName().Version.ToString().Replace(".", null));
            
            HMV.PEP.IoC.IoCWorker.ConfigureWIN();            


            InitializeComponent();

        }



        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            MessageBox.Show("cliquei!");
        }
    }
}
