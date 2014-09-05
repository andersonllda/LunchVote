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
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Wrappers.ObjectWrappers;
using DevExpress.Xpf.Docking;
using HMV.Core.Wrappers;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Model;
using StructureMap;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoM;
using HMV.PEP.ViewModel.SumarioAvaliacaoM;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for WinImportaDados.xaml
    /// </summary>
    public partial class winImportaDados : WindowBase
    {
        public winImportaDados(vmSumarioAvaliacaoPreAnestesica pVm)
        {
            InitializeComponent();
            this.DataContext = pVm;            
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSelecionar_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("Deseja Importar os Dados?", "ATENÇÃO", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.DialogResult = true;
                (DataContext as vmSumarioAvaliacaoPreAnestesica).ImportaDados();
                this.Close();
            }
            else
                this.DialogResult = false;
        }   
    }
}

