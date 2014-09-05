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
    public partial class winMensagemAvaliacaoDeRiscoNaoIniciado : WindowBase
    {
        public enum ActionResultPEP
        {
            Prescricao = 1,
            AvaliacaoDeRisco = 2
        }

        ActionResultPEP retAction = ActionResultPEP.Prescricao;

        public winMensagemAvaliacaoDeRiscoNaoIniciado(Atendimento atendimento)
        {
            InitializeComponent();
            this.DataContext = atendimento;
        }

        public ActionResultPEP Inicializa(Window pOwner)
        {
            btnPrescricao.IsEnabled = false;
            Atendimento atend = (this.DataContext as Atendimento);

            if (atend.TipoDeAtendimento == Core.Domain.Enum.TipoAtendimento.Internacao)
            {
                if (atend.Leito != null && atend.Leito.UnidadeInternacao != null)
                {
                    IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
                    Parametro par = srv.UnidadesInternacaoLiberadas();
                    IList<string> lst = par.Valor.Split(',');

                    if (lst.Contains(atend.Leito.UnidadeInternacao.ID.ToString()))
                        return ActionResultPEP.Prescricao;

                    if (atend.AvaliacaoRisco.HasItems() && atend.AvaliacaoRisco.Where(x => x.DataExclusao.IsNull()).OrderByDescending(x => x.Id).FirstOrDefault().IsNotNull())
                    {
                        AvaliacaoRisco avalrisco = atend.AvaliacaoRisco.Where(x => x.DataExclusao.IsNull()).OrderByDescending(x => x.Id).FirstOrDefault();

                        if (DateTime.Now < atend.DataAtendimento.AddDays(1) && avalrisco.DataEncerramento.IsNull())
                        {
                            Mensagem24.Visibility = Visibility.Visible;
                            btnPrescricao.IsEnabled = true;
                        }
                        else if (avalrisco.DataEncerramento.IsNotNull() && avalrisco.DataEncerramento.Value < atend.DataAtendimento.AddDays(1))//24h
                        {
                            return ActionResultPEP.Prescricao;
                        }
                        else if (avalrisco.DataEncerramento.IsNotNull() && avalrisco.DataEncerramento.Value.AddDays(2) < DateTime.Now)//48h
                        {
                            // btnPrescricao.IsEnabled = true;
                            Mensagem48.Visibility = Visibility.Visible;
                        }
                        else if (avalrisco.DataEncerramento.IsNull())
                        {
                            Mensagem48.Visibility = Visibility.Visible;
                            //btnPrescricao.IsEnabled = true;
                        }
                        else
                            return ActionResultPEP.Prescricao;
                    }
                    else
                    {
                        Mensagem24.Visibility = Visibility.Visible;
                        if (DateTime.Now < atend.DataAtendimento.AddDays(1))
                            btnPrescricao.IsEnabled = true;
                        else
                            btnPrescricao.IsEnabled = false;
                    }
                }
            }
            else
            {
                var _ultimaavaliacao = atend.Paciente.Atendimentos.SelectMany(x => x.AvaliacaoRisco).Where(x => x.DataExclusao.IsNull() && x.DataEncerramento.IsNotNull())
                    .OrderByDescending(x => x.DataEncerramento).FirstOrDefault();
                if (_ultimaavaliacao.IsNotNull())
                {
                    IRepositorioDeAvaliacaoRiscoOrigemTempo rep = ObjectFactory.GetInstance<IRepositorioDeAvaliacaoRiscoOrigemTempo>();
                    var ret = rep.OndeOrigemDocumentoIgual(atend.OrigemAtendimento.OrigemDocumento).Single();
                    if (ret.IsNotNull())
                    {
                        var maxdia = _ultimaavaliacao.DataEncerramento.Value.AddDays(ret.Tempo);
                        if (maxdia >= DateTime.Now)
                            return ActionResultPEP.Prescricao;
                    }
                    else
                        return ActionResultPEP.Prescricao;
                }
            }

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
            retAction = ActionResultPEP.AvaliacaoDeRisco;
            this.Close();
        }
    }
}
