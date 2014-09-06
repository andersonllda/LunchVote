using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Repository;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winMensagemSumarioNaoIniciado.xaml
    /// </summary>
    public partial class winMensagemPlanoEducacionalNaoIniciado : WindowBase
    {
        public enum ActionResultPEP
        {
            Prescricao = 1,
            PlanoEducacional = 2
        }

        ActionResultPEP retAction = ActionResultPEP.Prescricao;

        public winMensagemPlanoEducacionalNaoIniciado(Atendimento atendimento)
        {
            InitializeComponent();
            this.DataContext = atendimento;
        }

        public ActionResultPEP Inicializa(Window pOwner)
        {
            btnPrescricao.IsEnabled = false;
            if (DateTime.Now.Subtract((this.DataContext as Atendimento).DataAtendimento).Days <= 1 && !(this.DataContext as Atendimento).PerguntasPaciente.HasItems())
                btnPrescricao.IsEnabled = true;
            else 
            if ((this.DataContext as Atendimento).PerguntasPaciente.HasItems() && (this.DataContext as Atendimento).PerguntasPaciente.Count(x => x.Usuario.Prestador.IsNurse) > 0)
                return ActionResultPEP.Prescricao;
            else
                btnPrescricao.IsEnabled = false;

            this.ShowDialog(pOwner);
            return retAction;
        }

        private void btnPrescricao_Click(object sender, RoutedEventArgs e)
        {
            retAction = ActionResultPEP.Prescricao;
            this.Close();
        }

        private void btnAvalRisco_Click(object sender, RoutedEventArgs e)
        {
            retAction = ActionResultPEP.PlanoEducacional;
            this.Close();
        }
    }
}
