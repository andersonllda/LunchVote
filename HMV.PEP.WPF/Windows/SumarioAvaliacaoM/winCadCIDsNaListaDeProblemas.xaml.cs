using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Model;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO;

namespace HMV.PEP.WPF.Cadastros
{
    /// <summary>
    /// Interaction logic for winCadCIDsNaListaDeProblemas.xaml
    /// </summary>
    public partial class winCadCIDsNaListaDeProblemas : WindowBase
    { 
        public winCadCIDsNaListaDeProblemas(SumarioAvaliacaoMedica pSumarioAM, bool pIsCidPrincipal)
        {
            InitializeComponent();
            this.DataContext = new vmListaDeProblemas(pSumarioAM, App.Usuario, pIsCidPrincipal);
            (this.DataContext as vmListaDeProblemas).ActionCommandFechar += new Action(this.Close);
        }

        public winCadCIDsNaListaDeProblemas(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO pvm, bool pIsCidPrincipal)
        {
            InitializeComponent();
            this.DataContext = new vmListaDeProblemas(pvm, App.Usuario, pIsCidPrincipal);
            (this.DataContext as vmListaDeProblemas).ActionCommandFechar += new Action(this.Close);
        }

        public winCadCIDsNaListaDeProblemas(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO pvm, bool pIsCidPrincipal)
        {
            InitializeComponent();
            this.DataContext = new vmListaDeProblemas(pvm, App.Usuario, pIsCidPrincipal);
            (this.DataContext as vmListaDeProblemas).ActionCommandFechar += new Action(this.Close);
        }  

        private void viewDiagnostico_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            (sender as TableView).CommitEditing();
        }
    }
}
