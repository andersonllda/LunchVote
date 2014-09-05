using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP;
using System.Threading;
using System.Windows.Threading;
using System;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winPesquisaPaciente.xaml
    /// </summary>
    public partial class winPesquisaPaciente : WindowBase
    {
        public wrpPaciente Paciente
        {
            get 
            {
                return (this.DataContext as vmPesquisaPacientes).PacienteSelecionado;
            }
        }

        public winPesquisaPaciente()
        {
            InitializeComponent();
            this.DataContext = new vmPesquisaPacientes();
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            //HMVloading.Visibility = Visibility.Visible;
            //ThreadStart ts = delegate
            //{
            //    {
                    Filtra();
            //        grdPacientes.Dispatcher.Invoke(new Action(() => grdPacientes.ItemsSource = (this.DataContext as vmPesquisaPacientes).ListaPacientesCollection));
            //    }
            //    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
            //    delegate
            //    {
            //        HMVloading.Visibility = System.Windows.Visibility.Collapsed;
            //    }, null, null);
            //};
            //ts.BeginInvoke(delegate(IAsyncResult aysncResult) { ts.EndInvoke(aysncResult); }, null);
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

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPesquisaPacientes).Fechar();
            this.Close();
        }

        private void tableView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                btnConfirmar_Click(sender, new RoutedEventArgs());
        }

        private void Filtra()
        {
            (this.DataContext as vmPesquisaPacientes).PesquisaPacientes();
        }

        private void txtCPF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientes).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientes).Fechar();
                this.Close();
            }
        }

        private void txtIdentidade_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientes).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientes).Fechar();
                this.Close();
            }
        }

        private void txtRegistro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientes).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientes).Fechar();
                this.Close();
            }
        }

        private void txtNome_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientes).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientes).Fechar();
                this.Close();
            }
        }

        private void txtDtNascimento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientes).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientes).Fechar();
                this.Close();
            }
        }

        private void txtNomeMae_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) && (this.DataContext as vmPesquisaPacientes).BotaoPesquisa)
                Filtra();
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientes).Fechar();
                this.Close();
            }
        }

        private void WindowBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                (this.DataContext as vmPesquisaPacientes).Fechar();
                this.Close();
            }
        }
    }
}
