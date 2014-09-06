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
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem.ClassificacaoPaciente;
using HMV.Core.Domain.Repository.ClassificacaoPaciente;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winMensagemNASNaoIniciado.xaml
    /// </summary>
    public partial class winMensagemNASNaoIniciado : WindowBase
    {
        public enum ActionResultPEP
        {
            Prescricao = 1,
            NAS = 2
        }

        ActionResultPEP retAction = ActionResultPEP.Prescricao;

        public winMensagemNASNaoIniciado(Atendimento atendimento)
        {
            InitializeComponent();
            this.DataContext = atendimento;
        }

        public ActionResultPEP Inicializa(Window pOwner)
        {
            btnPrescricao.IsEnabled = false;
            Atendimento atend = (this.DataContext as Atendimento);

            IRepositorioDeTipoAvaliacaoNAS srv = ObjectFactory.GetInstance<IRepositorioDeTipoAvaliacaoNAS>();
            var dataintCTI = srv.BuscaUltimaInternacaoCTI(atend.ID);

            if (dataintCTI.HasValue)
            {
                if (atend.NAS.HasItems() && atend.NAS.Where(x => x.DataExclusao.IsNull()).OrderByDescending(x => x.ID).FirstOrDefault().IsNotNull())
                {
                    NAS nas = atend.NAS.Where(x => x.DataExclusao.IsNull()).OrderByDescending(x => x.ID).FirstOrDefault();

                    if (DateTime.Now < dataintCTI.Value.AddDays(1)) //periodo de tolerancia
                    {
                        Mensagem24.Visibility = Visibility.Visible;
                        btnPrescricao.IsEnabled = true;
                    }
                    else
                        if (nas.Data.AddDays(1) < DateTime.Now)
                        {
                            Mensagem24.Visibility = Visibility.Visible;
                            btnPrescricao.IsEnabled = false;
                        }
                        else
                            return ActionResultPEP.Prescricao;
                }
                else
                {
                    if (DateTime.Now < dataintCTI.Value.AddDays(1))
                        btnPrescricao.IsEnabled = true;
                    else
                        btnPrescricao.IsEnabled = false;
                }
            }
            else
                return ActionResultPEP.Prescricao;


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
            retAction = ActionResultPEP.NAS;
            this.Close();
        }
    }
}
