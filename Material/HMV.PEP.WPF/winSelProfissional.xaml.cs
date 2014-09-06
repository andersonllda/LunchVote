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
using StructureMap;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using System.Configuration;
using HMV.Core.DataAccess;
using System.Threading;
using System.Windows.Threading;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winSelProfissional.xaml
    /// </summary>
    public partial class winSelProfissional : WindowBase
    {

        private Prestador cPrestador;
        public bool pFiltraTecnicos;

        public winSelProfissional(string pRegistro)
        {
            InitializeComponent();
            bEditCRM.Text = pRegistro;
            carrega();
        }

        public winSelProfissional()
        {
            InitializeComponent();
            carrega();
        }

        public winSelProfissional(bool pPesquisaInicial)
        {
            InitializeComponent();
            if (pPesquisaInicial)
                carrega();
        }

        public Prestador GetPrestador()
        {
            return cPrestador;
        }

        private void carrega()
        {

            HMVloading.Visibility = System.Windows.Visibility.Visible;

            IList<Prestador> listaPrestador = new List<Prestador>();
            String nome = bEdit.Text;
            String registro = bEditCRM.Text;

            int idClin;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["ClinicaDefault"], out idClin))
                throw new NullReferenceException("Parametro 'ClinicaDefault' deve existir no arquivo de configuração.");


            ThreadStart ts = delegate
            {
                //rz using (IUnitOfWork uow = ObjectFactory.GetInstance<IUnitOfWork>())
                {
                    IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();

                    if (!String.IsNullOrWhiteSpace(nome))
                        serv.FiltraPorNome(nome);
                    if (!String.IsNullOrWhiteSpace(registro))
                        serv.FiltraPorRegistro(registro);

                    serv.FiltraPorClinica(idClin);
                    serv.FiltraOndeRegistroInformado();

                    if (pFiltraTecnicos)
                        listaPrestador = serv.Carrega().Where(x => x.Conselho.cd_conselho == 2).ToList();
                    else
                        listaPrestador = serv.Carrega();

                    gridConsulta.Dispatcher.Invoke(new Action(() =>
                        gridConsulta.ItemsSource = listaPrestador));

                }

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
                delegate
                {
                    HMVloading.Visibility = System.Windows.Visibility.Collapsed;
                    lblTotal.Content = "Total de Registro: " + listaPrestador.Count;
                }, null, null);
            };

            ts.BeginInvoke(delegate(IAsyncResult aysncResult) { ts.EndInvoke(aysncResult); }, null);
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (gridConsulta.VisibleRowCount == 0)
            {
                DXMessageBox.Show("Deve ser selecionado um Prestador.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }

            cPrestador = (Prestador)gridConsulta.GetFocusedRow();

            this.DialogResult = true;
            this.Close();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btnConfirmar_Click(new object(), new RoutedEventArgs());
        }

        private void bEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                carrega();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bEdit.Focus();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            carrega();
        }

    }

}
