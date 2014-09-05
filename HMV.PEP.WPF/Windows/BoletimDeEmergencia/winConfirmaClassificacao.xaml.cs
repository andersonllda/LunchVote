using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Domain.Model;
using System.Windows.Input;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winConfirmaClassificacao.xaml
    /// </summary>
    public partial class winConfirmaClassificacao : WindowBase
    {

        public winConfirmaClassificacao(vmBoletimEmergencia pvmBoletim)
        {
            InitializeComponent();
            this.DataContext = pvmBoletim.vmClassificacao;
        }

        public winConfirmaClassificacao(vmClassificacaoRisco pvmBoletim)
        {
            InitializeComponent();
            this.DataContext = pvmBoletim;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmClassificacaoRisco).BoletimEmergencia.Classificacoes.HasItems())
                if ((this.DataContext as vmClassificacaoRisco).Sim
                    && (this.DataContext as vmClassificacaoRisco).BoletimEmergencia.Classificacoes.OrderBy(x => x.Id).Last().Usuario.DomainObject != App.Usuario)
                    (this.DataContext as vmClassificacaoRisco).ClassificacaoSelecionada = (this.DataContext as vmClassificacaoRisco).ClassificacaoAtual;
            (this.DataContext as vmClassificacaoRisco).AddClassificacaoRiscoCommand.Execute(null);
            this.DialogResult = true;
            this.Close();
        }   
    }
}
