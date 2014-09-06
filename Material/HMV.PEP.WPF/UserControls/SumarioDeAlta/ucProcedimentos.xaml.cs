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
using HMV.Core.Interfaces;
using HMV.Core.Wrappers;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucProcedimentos.xaml
    /// </summary>
    public partial class ucProcedimentos : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }       

        public ucProcedimentos()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = new vmProcedimento((pData as vmSumarioAlta).SumarioAlta, App.Usuario);
        }        

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winSelMedicamentos win = new winSelMedicamentos((vmProcedimento)this.DataContext);
            win.ShowDialog(base.OwnerBase);
            gdProcedimentos.RefreshData();
        }

        private void gdProcedimentos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as GridControl).View.FocusedRow = (sender as GridControl).View.FocusedRow;
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            DateTime data = ((vmProcedimento)this.DataContext).procedimentoAltaSelecionado.Data;          

            winCadProcedimentos win = new winCadProcedimentos((vmProcedimento)this.DataContext);
            if (win.ShowDialog(this.OwnerBase) == false)
                ((vmProcedimento)this.DataContext).procedimentoAltaSelecionado.Data = data;
            gdProcedimentos.RefreshData();
        }        
    }
}
