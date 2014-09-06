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
    public partial class winMensagemAdmissaoAssitencialNaoIniciado : WindowBase
    {
        public ActionResultPEP retAction = ActionResultPEP.Prescricao;
        private Atendimento _Atendimento;
        public enum ActionResultPEP
        {
            Prescricao = 1,
            Admissao = 2,
            AdmissaoCO
        }
        public winMensagemAdmissaoAssitencialNaoIniciado(Atendimento pAtendimento, bool HabilitaPrescicao)
        {
            InitializeComponent();
            btnPrescricao.IsEnabled = HabilitaPrescicao;
            _Atendimento = pAtendimento;
        }

        private void btnAdmAssistencial_Click(object sender, RoutedEventArgs e)
        {
            IRepositorioDeParametrosInternet repCO = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
            ParametroInternet parametroCO = repCO.OndeOrigemParaAdmissaoCO().Single();
            if (parametroCO.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCO.valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(_Atendimento.OrigemAtendimento.ID))
                    {
                        retAction = ActionResultPEP.AdmissaoCO;
                        this.Close();
                        return;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO deve ser inteiro e separado por virgula.");
                }
            }

            retAction = ActionResultPEP.Admissao;
            this.Close();
        }

        private void btnPrescricao_Click(object sender, RoutedEventArgs e)
        {
            retAction = ActionResultPEP.Prescricao;
            this.Close();
        }
    }
}
