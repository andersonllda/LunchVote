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
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winSelProfissional.xaml
    /// </summary>
    public partial class winSelPaciente : WindowBase
    {
        public Paciente Paciente { get; private set; }

        public winSelPaciente()
        {
            InitializeComponent();
            carrega();
        }

        private void carrega()
        {

            HMVloading.Visibility = System.Windows.Visibility.Visible;

            String nome = bEdit.Text;
            int retProntuario = 0;
            Int32.TryParse(bEditProntuario.Text, out retProntuario);
            IList<Paciente> paciente = new List<Paciente>();
            
            ThreadStart ts = delegate
            {
                //rz using (IUnitOfWork uow = ObjectFactory.GetInstance<IUnitOfWork>())
                {
                    IPacienteService serv = ObjectFactory.GetInstance<IPacienteService>();                    

                    if (!String.IsNullOrWhiteSpace(nome))
                        paciente = serv.FiltraPorNome(nome);
                    if (retProntuario > 0)
                        paciente.Add(serv.FiltraPorID(retProntuario));

                    gridConsulta.Dispatcher.Invoke(new Action(() =>
                        gridConsulta.ItemsSource = paciente));
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
                delegate
                {
                    HMVloading.Visibility = System.Windows.Visibility.Collapsed;
                    lblTotal.Content = "Total de Registros: " + paciente.Count;
                }, null, null);
            };

            ts.BeginInvoke(delegate(IAsyncResult aysncResult) { ts.EndInvoke(aysncResult); }, null);


        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (gridConsulta.VisibleRowCount == 0)
            {
                DXMessageBox.Show("Deve ser selecionado um Paciente.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }

            Paciente = (Paciente)gridConsulta.GetFocusedRow();
            this.Close();

        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
              btnConfirmar_Click(new object(), new RoutedEventArgs());
        }

        private void bEdit_KeyDown(object sender, KeyEventArgs e)
        {
            bEditProntuario.Text = string.Empty;
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

        private void bEditProntuario_KeyDown(object sender, KeyEventArgs e)
        {
            bEdit.Text = string.Empty;
            if (e.Key == Key.Enter)
                carrega();
        }

    }

}
