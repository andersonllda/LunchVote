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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.SumarioDeAlta;
using HMV.PEP.WPF.Cadastros;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucListaProblemaSumario.xaml
    /// </summary>
    public partial class ucListaProblemaSumario : UserControlBase, IUserControl
    {
        public ucListaProblemaSumario()
        {
            InitializeComponent();
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetData(object pData)
        {
            this.DataContext = pData as vmListaProblemaSumario;
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadListaProblema win = new winCadListaProblema((this.DataContext as vmListaProblemaSumario).Atendimento.Paciente.DomainObject, (this.DataContext as vmListaProblemaSumario).Atendimento.DomainObject);
            win.ObrigarCIDeOcultarEventosRelevantes = true;

            if (win.ShowDialog(base.OwnerBase) == true)
            {
                (this.DataContext as vmListaProblemaSumario).Refresh(win.DataContext as ProblemasPaciente);
            }
        }

        private void viewListaProblemas_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value); 
        }   
    }
}
