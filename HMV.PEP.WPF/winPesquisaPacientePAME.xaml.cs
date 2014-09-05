using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winPesquisaPacientePAME.xaml
    /// </summary>
    public partial class winPesquisaPacientePAME : WindowBase
    {
        #region Construtor

        public winPesquisaPacientePAME()
        {
            InitializeComponent();
            this.DataContext = new vmPesquisaPacientesPAME();
        }

        #endregion

        #region Propriedades Privadas

        #endregion

        #region Propriedades Públicas

        public wrpPaciente Paciente
        {
            get
            {
                return (this.DataContext as vmPesquisaPacientesPAME).PacienteSelecionado;
            }
        }

        #endregion

        #region Métodos Privados

        private void Filtra()
        {
            (this.DataContext as vmPesquisaPacientesPAME).PesquisaPacientes();
        }

        #endregion

        #region Eventos

        private void WindowBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientesPAME).Fechar();
                this.Close();
            }
        }

        private void txtNome_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientesPAME).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientesPAME).Fechar();
                this.Close();
            }
        }

        private void txtRegistro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientesPAME).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientesPAME).Fechar();
                this.Close();
            }
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            Filtra();
        }

        private void tableView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                btnConfirmar_Click(sender, new RoutedEventArgs());
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPesquisaPacientesPAME).Fechar();
            this.Close();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (grdPacientes.VisibleRowCount == 0)
            {
                DXMessageBox.Show("Deve ser selecionado um Paciente.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
            this.Close();
        }

        #endregion
    }
}
