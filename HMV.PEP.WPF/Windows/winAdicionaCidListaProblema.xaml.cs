using System;
using System.Windows;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.Commands;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;


namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for AdicionaCidListaProblema.xaml
    /// </summary>
    public partial class winAdicionaCidListaProblema : WindowBase
    {
        //private bool _jafechou { get; set; }
        public bool NaoAbrir;

        public winAdicionaCidListaProblema(wrpAtendimento pAtendimento, wrpUsuarios pUsuario)
        {
            InitializeComponent();
            vmCIDListaProblema pData = new vmCIDListaProblema(pAtendimento, pUsuario);
            this.DataContext = pData;

            if ((this.DataContext as vmCIDListaProblema).Divergentes.Count == 0 && (this.DataContext as vmCIDListaProblema).ListaDeProblemasMaiorQueZero)
                this.NaoAbrir = true;

            
            (this.DataContext as vmCIDListaProblema).GoShow += new EventHandler(AbreTelaInclusaoCID);
            (this.DataContext as vmCIDListaProblema).GoClose += this.Close;
            //this._jafechou = true;
        }

        private void tbvListaCids_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            (sender as TableView).CommitEditing();
        }    

        void AbreTelaInclusaoCID(object sender, EventArgs e)
        {
            winSelCID win = new winSelCID(false);
            win.ShowDialog(this);
            if (win.CID != null)
                (this.DataContext as vmCIDListaProblema).InsereCidNovo(win.CID);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !(this.DataContext as vmCIDListaProblema).CancelaGrava();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

        }

    }
    
}
