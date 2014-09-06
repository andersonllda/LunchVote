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
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Framework.WPF;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.PEP.Consult;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winMensagemSumarioNaoIniciado.xaml
    /// </summary>
    public partial class winMensagemSumarioNaoIniciado : WindowBase
    {
        public enum ActionResultPEP
        {
            Prescricao = 1,
            SumarioAvaliacaoMedica = 2,
            SumarioAvaliacaoMedicaCO = 3,
            SumarioAvaliacaoMedicaRN = 4,
            SumarioAvaliacaoMedicaCTINEO = 5
        }

        ActionResultPEP retAction = ActionResultPEP.Prescricao;

        public winMensagemSumarioNaoIniciado(Atendimento atendimento)
        {
            InitializeComponent();
            this.DataContext = atendimento;
        }

        bool _achouRN = false;
        bool _achouCO = false;
        bool _achouCTINEO = false;

        public ActionResultPEP Inicializa(Window pOwner)
        {
            Atendimento atend = (this.DataContext as Atendimento);
            if (atend.Leito != null && atend.Leito.UnidadeInternacao != null)
            {
                IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
                Parametro par = srv.UnidadesInternacaoLiberadas();
                IList<string> lst = par.Valor.Split(',');
                if (lst.Contains(atend.Leito.UnidadeInternacao.ID.ToString()))
                    return ActionResultPEP.Prescricao;
            }

            IRepositorioDeParametrosClinicas repCTI = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
            Parametro parametroCTI = repCTI.OndeUnidadeCTINEO().Single();
            if (parametroCTI.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCTI.Valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(atend.Leito.UnidadeInternacao.ID))
                    {
                        _achouCTINEO = true;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO_SUMARIO deve ser inteiro e separado por virgula.");
                }
            }

            if (!_achouCTINEO)
            {
                //verifica se ja tem sumario RN nos atendimentos ateriores
                IRepositorioDeSumarioDeAvaliacaoMedicaRN reprn = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
                var rns = reprn.OndePacienteIgual(atend.Paciente).List();
                if (rns.IsNotNull())
                {
                    int dias = 0;
                    IRepositorioDeParametrosClinicas reprnp = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                    Parametro parametrorn = reprnp.DiasValidoSumarioRN().Single();
                    if (parametrorn.IsNotNull())
                        dias = parametrorn.Valor.ToInt();

                    bool tempaccomrn = rns.Where(x => x.DataEncerramento.HasValue && x.DataEncerramento.Value >= DateTime.Now.Date.AddDays(-dias)).Count() > 0;
                    if (tempaccomrn)
                        return ActionResultPEP.Prescricao;
                }
            }
            if (atend.AtendimentoPai.IsNotNull())
            {
                _achouRN = true;
            }

            IRepositorioDeParametrosInternet repCO2 = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
            ParametroInternet parametroCO2 = repCO2.OndeOrigemParaAdmissaoCO().Single();
            if (parametroCO2.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCO2.valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(atend.OrigemAtendimento.ID) && atend.SumarioAvaliacaoMedica.IsNull())
                        _achouCO = true;
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO_SUMARIO deve ser inteiro e separado por virgula.");
                }
            }


            // Se o paciente tiver os dois sumario, verifica se ambos foram realizados.
            if (_achouRN && _achouCTINEO)
            {
                IRepositorioDeSumarioDeAvaliacaoMedicaRN repRN = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
                var sumarioRN = repRN.OndeCodigoAtendimentoIgual(atend).List().FirstOrDefault();

                IRepositorioDeSumarioAvaliacaoMedicaCTINEO repCTINEO = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
                var sumarioCTINEO = repCTINEO.OndeCodigoAtendimentoIgual(atend).List().FirstOrDefault();

                if (sumarioRN.IsNotNull() && sumarioRN.DataEncerramento.HasValue && sumarioCTINEO.IsNotNull() && sumarioCTINEO.DataEncerramento.HasValue)
                    return ActionResultPEP.Prescricao;


                // Verifica se o paciente foi internado, a partir da data de liberacao do sumario de avaliacao CTINEO em produção.
                // busca parâmetro para definir data de início da regra para chamada da prescrição                        
                repCTI = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                parametroCTI = repCTI.OndeDataPrescricaoCTINEO().Single();

                DateTime dataLiberacaoCTINEO;
                if (DateTime.TryParse(parametroCTI.Valor, out dataLiberacaoCTINEO))
                {
                    ISumarioDeAvaliacaoMedicaCTINEOConsult consultctineo = ObjectFactory.GetInstance<ISumarioDeAvaliacaoMedicaCTINEOConsult>();
                    var data = consultctineo.BuscaDataPermanenciaCTINEO(atend);

                    // Se a internacao for menor que a data de liberacao da CTINEO, ignora a OBRIGATORIEDADE DO SUMARIO PREENCHIDO CTINEO.
                    if (data < dataLiberacaoCTINEO)
                        return ActionResultPEP.Prescricao;
                }

            }
            else if (_achouRN)
            {
                IRepositorioDeSumarioDeAvaliacaoMedicaRN rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
                var ret1 = rep.OndeCodigoAtendimentoIgual(atend).Single();
                if (ret1.IsNotNull())
                {
                    var rn = new wrpSumarioAvaliacaoMedicaRN(ret1);
                    if (rn.DataEncerramento.HasValue) //|| rn.Usuario.cd_usuario != App.Usuario.cd_usuario
                        return ActionResultPEP.Prescricao;
                }
            }
            else if (_achouCTINEO)
            {
                int _horasctineo = 0;
                Parametro parC = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().BuscaHorasValidadeSumarioCTINEO().Single();
                if (parC.IsNotNull())
                    _horasctineo = parC.Valor.ToInt();

                var repA = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeAtendimento>();
                var ret = repA.OndeCodigoPacienteIgual(atend.Paciente.ID).List();
                if (ret.IsNotNull())
                {
                    var atendmentosanteriores = ret.Where(x => x.DataAlta.IsNotNull() && x.DataAlta.Value.AddHours(_horasctineo) >= atend.DataAtendimento).ToList();
                    if (atendmentosanteriores.HasItems())
                        if (atendmentosanteriores.Count(x => x.AtendimentoPai.IsNotNull()) > 0)
                        {
                            IRepositorioDeSumarioAvaliacaoMedicaCTINEO repCTINEOT = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
                            var sumarioCTINEOT = repCTINEOT.OndeCodigoAtendimentoIgual(atendmentosanteriores.FirstOrDefault()).List().FirstOrDefault();
                            if (sumarioCTINEOT.IsNotNull())
                            {
                                var ctineo = new wrpSumarioAvaliacaoMedicaCTINEO(sumarioCTINEOT);
                                if (ctineo.DataEncerramento.HasValue)
                                    return ActionResultPEP.Prescricao;
                            }
                        }
                }

                IRepositorioDeSumarioAvaliacaoMedicaCTINEO repCTINEO = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
                var sumarioCTINEO = repCTINEO.OndeCodigoAtendimentoIgual(atend).List().FirstOrDefault();
                if (sumarioCTINEO.IsNotNull())
                {
                    var ctineo = new wrpSumarioAvaliacaoMedicaCTINEO(sumarioCTINEO);
                    if (ctineo.DataEncerramento.HasValue)
                        return ActionResultPEP.Prescricao;
                    else
                    {
                        // Verifica se o paciente foi internado, a partir da data de liberacao do sumario de avaliacao CTINEO em produção.
                        // busca parâmetro para definir data de início da regra para chamada da prescrição                        
                        repCTI = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                        parametroCTI = repCTI.OndeDataPrescricaoCTINEO().Single();

                        DateTime dataLiberacaoCTINEO;
                        if (DateTime.TryParse(parametroCTI.Valor, out dataLiberacaoCTINEO))
                        {
                            ISumarioDeAvaliacaoMedicaCTINEOConsult consultctineo = ObjectFactory.GetInstance<ISumarioDeAvaliacaoMedicaCTINEOConsult>();
                            var data = consultctineo.BuscaDataPermanenciaCTINEO(atend);

                            // Se a internacao for menor que a data de liberacao da CTINEO, ignora a OBRIGATORIEDADE DO SUMARIO PREENCHIDO CTINEO.
                            if (data < dataLiberacaoCTINEO)
                                return ActionResultPEP.Prescricao;
                        }
                    }
                }
            }
            else if (_achouCO)
            {
                IRepositorioDeSumarioAvaliacaoMedicaCO rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCO>();
                var ret = rep.OndeCodigoAtendimentoIgual(atend).Single();
                if (ret.IsNotNull())
                {
                    var co = new wrpSumarioAvaliacaoMedicaCO(ret);
                    if (co.DataEncerramento.HasValue) //|| co.Usuario.cd_usuario != App.Usuario.cd_usuario
                        return ActionResultPEP.Prescricao;
                }
            }
            else
                if (atend.SumarioAvaliacaoMedica != null && (atend.SumarioAvaliacaoMedica.DataEncerramento.HasValue)) //|| atend.SumarioAvaliacaoMedica.Usuario.cd_usuario != App.Usuario.cd_usuario
                    return ActionResultPEP.Prescricao;

            if (atend.DataHoraAtendimento.AddDays(1) < DateTime.Now)
            {
                //btnPrescricao.IsEnabled = false;
                btnPrescricao.Visibility = Visibility.Collapsed;
                if (App.Usuario.IsPlantonista)
                {
                    if (atend.Leito != null && atend.Leito.UnidadeInternacao != null)
                    {
                        IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
                        Parametro par = srv.UnidadesInternacaoLiberadasParaOsPlantonistas();
                        IList<string> lst = par.Valor.Split(',');
                        if (lst.Contains(atend.Leito.UnidadeInternacao.ID.ToString()))
                            //grpMensagem.IsEnabled = true;
                            grpMensagem.Visibility = Visibility.Visible;
                    }
                }
            }

            this.ShowDialog(pOwner);
            return retAction;
        }

        private void btnSumario_Click(object sender, RoutedEventArgs e)
        {
            // Se o paciente for do tipo que tem os dois sumarios, verifica qual não foi preenchido. 
            if (_achouCO && _achouCTINEO)
            {
                Atendimento atend = (this.DataContext as Atendimento);
                IRepositorioDeSumarioDeAvaliacaoMedicaRN repRN = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
                var sumarioRN = repRN.OndeCodigoAtendimentoIgual(atend).List().FirstOrDefault();

                IRepositorioDeSumarioAvaliacaoMedicaCTINEO repCTINEO = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
                var sumarioCTINEO = repCTINEO.OndeCodigoAtendimentoIgual(atend).List().FirstOrDefault();

                if (sumarioRN.IsNull() || !sumarioRN.DataEncerramento.HasValue)
                    retAction = ActionResultPEP.SumarioAvaliacaoMedicaRN;
                else if (sumarioCTINEO.IsNull() || !sumarioCTINEO.DataEncerramento.HasValue)
                    retAction = ActionResultPEP.SumarioAvaliacaoMedicaCTINEO;
            }
            else if (_achouRN)
                retAction = ActionResultPEP.SumarioAvaliacaoMedicaRN;
            else if (_achouCTINEO)
                retAction = ActionResultPEP.SumarioAvaliacaoMedicaCTINEO;
            else if (_achouCO)
                retAction = ActionResultPEP.SumarioAvaliacaoMedicaCO;
            else
                retAction = ActionResultPEP.SumarioAvaliacaoMedica;

            this.Close();
        }

        private void btnPrescricao_Click(object sender, RoutedEventArgs e)
        {
            retAction = ActionResultPEP.Prescricao;
            this.Close();
        }
    }
}
