using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.Expression;
using HMV.PEP.Interfaces;
using StructureMap;
using System.Configuration;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.Alergia
{
    /// <summary>
    /// Interaction logic for winCadAlergia.xaml
    /// </summary>
    public partial class winCadAlergia : WindowBase
    {
        public winCadAlergia(vmAlergias pData) 
        {
            InitializeComponent();
            this.DataContext = pData;
            (this.DataContext as vmAlergias).AlergiaSelecionada.BeginEdit();
            
            /************
             *TFS - 2805*
             ************
            deDataInicio.MinValue = (this.DataContext as vmAlergias).NascPaciente;
            deDataInicio.MaxValue = DateTime.Today.AddDays(1).AddMinutes(-1);*/
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmAlergias).AlergiaSelecionada.CancelEdit();
            (this.DataContext as vmAlergias).NovaAlergia = false;
            (this.DataContext as vmAlergias).AtualizaListaAlergias();
            this.Close();
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmAlergias).AlergiaSelecionada.EndEdit();
            //(this.DataContext as vmAlergias).AtualizaListaAlergias();
            this.Close();
        }
    }
}
