using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Model;
using HMV.PEP.ViewModel.PEP;
using System.Configuration;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Cadastros;
using System.Text.RegularExpressions;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls
{    
    /// <summary>
    /// Interaction logic for Evolucao.xaml
    /// </summary>
    public partial class ucEvolucao : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }
         

        public ucEvolucao()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
            {                
                this.DataContext = new vmRegistrosDeEvolucao((pData as Paciente), App.Usuario.Profissional, null);
            }
            else if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {                   
                this.DataContext = new vmRegistrosDeEvolucao((pData as Atendimento).Paciente, App.Usuario.Profissional, (pData as Atendimento));                
            }           
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadEvolucao win = new winCadEvolucao(this.DataContext as vmRegistrosDeEvolucao);

            win.ShowDialog(base.OwnerBase);

            (this.DataContext as vmRegistrosDeEvolucao).AtualizaListaEvolucao();
            gdEvolucao.RefreshData();            
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            rptEvolucao report = new rptEvolucao();
            report.DataSource = (this.DataContext as vmRegistrosDeEvolucao).RegistrosDeEvolucao.OrderByDescending(x => x.ID).ToList();

            winRelatorio win = new winRelatorio(report, true, "Evolução.", false);
            win.ShowDialog(base.OwnerBase);
            //report.ShowPreviewDialog();
        }
    }
}
