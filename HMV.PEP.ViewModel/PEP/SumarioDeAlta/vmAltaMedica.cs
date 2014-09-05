using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.Commands;
using StructureMap;
using StructureMap.Pipeline;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmAltaMedica : ViewModelBase
    {
        #region Contrutor

        //bool AlterouProcSUS;

        public vmAltaMedica(vmSumarioAlta pSumarioAlta, Usuarios pUsuario)
        {
            this._vmsumarioalta = pSumarioAlta;
            this._sumarioAlta = pSumarioAlta.SumarioAlta;
            this._usuario = pUsuario;
            this._habilitaSetorObito = Visibility.Collapsed;

            this.AddCidCommand = new AddCidCommand(this);
            this.RemoveCidCommand = new RemoveCidCommand(this);


            if (_sumarioAlta.DataAlta.HasValue)
            {
                this._dataSelecionada = this._sumarioAlta.DataAlta.Value;
                if (this._dataSelecionada <= DateTime.Today)
                    this._horaSelecionada = DateTime.Now.ToShortTimeString();

                if (this._sumarioAlta.DataAlta > DateTime.Today)
                    this._habilitaHora = false;
                else
                    this._habilitaHora = true;
            }
            else
            {
                this._dataSelecionada = DateTime.Today;
                //this._horaSelecionada = DateTime.Now.ToShortTimeString();
                this._habilitaHora = true;
            }

            if (this._sumarioAlta.Procedimento == null)
            {
                if (_sumarioAlta.Atendimento.DomainObject.ProcedimentoAMB != null)
                {
                    this._sumarioAlta.Procedimento = this._sumarioAlta.Atendimento.ProcedimentoAMB;
                    this._habilitaProcedimentoSUS = true;
                }
                else
                {
                    if (this._sumarioAlta.ProcedimentosAlta.Count > 0)
                    {
                        this._sumarioAlta.Procedimento = this._sumarioAlta.ProcedimentosAlta.FirstOrDefault().Cirurgia.ProFat;
                        this._habilitaProcedimentoSUS = true;
                    }
                }
            }

            if (this._sumarioAlta.Atendimento.DomainObject.Cid != null && this._sumarioAlta.CIDPrincipal == null)
                this._sumarioAlta.CIDPrincipal = this._sumarioAlta.Atendimento.Cid;

            /*if (this._sumarioAlta.ProcedimentoSUS == null)
            {
                if (this._sumarioAlta.Atendimento.DomainObject.ProcedimentoSUS != null)
                    this._sumarioAlta.ProcedimentoSUS = this._sumarioAlta.Atendimento.ProcedimentoSUS;
                else
                {
                    if (this._sumarioAlta.Procedimento != null)
                        if (this._sumarioAlta.Procedimento.ProcedimentoMVSUS.Count == 1)
                            this._sumarioAlta.ProcedimentoSUS = this._sumarioAlta.Procedimento.ProcedimentoMVSUS.FirstOrDefault().ProcedimentoSUS;
                }
            }*/

            if (this._sumarioAlta.MotivoAlta == null)
            {
                if (this._sumarioAlta.Atendimento.DomainObject.MotivoAlta != null)
                {
                    this._sumarioAlta.MotivoAlta = this._sumarioAlta.Atendimento.MotivoAlta;
                    if (this._sumarioAlta.MotivoAlta.Tipo == TipoMotivoAlta.Óbito)
                        this._habilitaSetorObito = Visibility.Visible;
                }
                else
                {
                    if (this._sumarioAlta.DomainObject.MotivoAltaDiaSeguinte != null)
                    {
                        this._sumarioAlta.MotivoAlta = this._sumarioAlta.MotivoAltaDiaSeguinte;
                        if (this._sumarioAlta.MotivoAlta.Tipo == TipoMotivoAlta.Óbito)
                            this._habilitaSetorObito = Visibility.Visible;
                    }
                }
            }

            if (this._sumarioAlta.Atendimento.DomainObject.SetorObito != null)
                this._setorObito = this._sumarioAlta.Atendimento.SetorObito;

            //IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            //foreach (var item in rep.ondeProcedimentoEstaNaContaAmbulatorial(this._sumarioAlta.Atendimento.DomainObject).List())
            //    _SumarioExames.Add(new SumarioExame(item));

            //rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
            //foreach (var item in rep.ondeProcedimentoEstaNaContaHospitalar(this._sumarioAlta.Atendimento.DomainObject).List())
            //    _SumarioExames.Add(new SumarioExame(item));
            //if (_SumarioExames != null)
            //{
            //    if (_SumarioExames.Count > 0)
            //    {
            //        this._sumarioAlta.SemExamesRealizados = null;
            //        this._sumarioAlta.SemParticularidadeExames = SimNao.Nao;
            //    }
            //    else
            //    {
            //        this._sumarioAlta.SemExamesRealizados = SimNao.Sim;
            //        this._sumarioAlta.SemParticularidadeExames = null;
            //    }
            //}
            //else
            //{
            //    this._sumarioAlta.SemExamesRealizados = SimNao.Sim;
            //    this._sumarioAlta.SemParticularidadeExames = null;
            //}
        }
        #endregion

        #region Propriedades Publicas
        public vmSumarioAlta vmSumarioAlta
        {
            get { return this._vmsumarioalta; }
        }

        public Visibility IsVisibleTipoAmbulatorial
        {
            get
            {
                if (this._sumarioAlta == null || this._sumarioAlta.Atendimento == null)
                    return Visibility.Collapsed;

                return this._sumarioAlta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string TituloBotaoConfirma
        {
            get
            {
                if (this._dataSelecionada == DateTime.Today)
                    return "Confirma Alta Médica";
                return "Confirma Dados";
            }
        }

        public IList<string> ListaDataHora
        {
            get
            {
                var lista = new List<string>();
                lista.Add(DateTime.Today.ToShortDateString());
                lista.Add(DateTime.Today.AddDays(1).ToShortDateString());
                return lista;
            }
        }

        public wrpProcedimentoCollection ProcedimentosAMBPossiveis
        {
            get
            {
                IRepositorioDeProcedimentoSumarioAlta serv = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
                List<Procedimento> procedimentos = serv.ondeProcedimentoPermitidoAlta().List().OrderBy(x => x.Descricao).ToList();
                return new wrpProcedimentoCollection(procedimentos);
            }
        }

        public wrpProcedimentoSUSCollection ProcedimentosSUSPossiveis
        {
            get
            {
                if (this._sumarioAlta.Procedimento != null)
                    return new wrpProcedimentoSUSCollection(this._sumarioAlta.Procedimento.ProcedimentoMVSUS.Select(x => x.ProcedimentoSUS.DomainObject).OrderBy(x => x.Descricao).ToList());
                return null;
            }
        }

        public wrpMotivoAltaCollection MotivosAltaPossiveis
        {
            get
            {
                if (this._sumarioAlta.Procedimento != null)
                    if (this._sumarioAlta.Procedimento.MotivosAlta != null)
                        if (this._sumarioAlta.Procedimento.MotivosAlta.Count > 0)
                        {
                            this._sumarioAlta.Procedimento.MotivosAlta.OrderBy(x => x.Ordem).ToList();
                            return this._sumarioAlta.Procedimento.MotivosAlta;
                        }

                IRepositorioDeMotivoAlta rep = ObjectFactory.GetInstance<IRepositorioDeMotivoAlta>();
                return new wrpMotivoAltaCollection(rep.List().OrderBy(x => x.Ordem).ToList());
            }
        }

        public wrpSetorCollection SetoresObitoPossiveis
        {
            get
            {
                IRepositorioDeSetor rep = ObjectFactory.GetInstance<IRepositorioDeSetor>();
                return new wrpSetorCollection(rep.OrdenaPorDescricao().List());
            }
        }

        public bool HabilitaConfirmaAlta
        {
            get { return _habilitaconfirmaalta; }
        }

        public bool HabilitaHora
        {
            get { return _habilitaHora; }
        }

        public bool HabilitaProcedimentoSUS
        {
            get { return _habilitaProcedimentoSUS; }
        }

        public Visibility HabilitaSetorObito
        {
            get { return _habilitaSetorObito; }
        }

        public string DataSelecionada
        {
            get
            {
                return _dataSelecionada.ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this._dataSelecionada = DateTime.Parse(value);
                    if (DateTime.Parse(value) > DateTime.Today)
                    {
                        this._horaSelecionada = null;
                        this._habilitaHora = false;
                    }
                    else
                    {
                        this._horaSelecionada = DateTime.Now.ToShortTimeString();
                        this._habilitaHora = true;
                    }
                }

                this.OnPropertyChanged("DataSelecionada");
                this.OnPropertyChanged("HoraSelecionada");
                this.OnPropertyChanged("TituloBotaoConfirma");
                this.OnPropertyChanged("HabilitaHora");
            }
        }

        public string HoraSelecionada
        {
            get { return this._horaSelecionada; }
            set
            {
                this._horaSelecionada = DateTime.Parse(value).ToShortTimeString();
                this.OnPropertyChanged("HoraSelecionada");
            }
        }


        //private bool ctrlProcSus = true;//controla o procedimento sus quando inicia a VM, p/ que nao receba null no evento btnProcedimentoAMB_EditValueChanging
        public wrpProcedimento ProcedimentoAMB
        {
            get { return _sumarioAlta.Procedimento; }
            set
            {
                this._sumarioAlta.Procedimento = value;
                this._sumarioAlta.Procedimento = value;
                /*if (this.AlterouProcSUS)
                {
                    if (!ctrlProcSus)
                        this._sumarioAlta.ProcedimentoSUS = null;
                    else
                        ctrlProcSus = false;
                }
                this.AlterouProcSUS = true;
                if (value != null)
                {
                    this._habilitaProcedimentoSUS = true;
                    if (value.ProcedimentoMVSUS.Count == 1)
                        this.ProcedimentoSUS = value.ProcedimentoMVSUS.FirstOrDefault().ProcedimentoSUS;
                }
                else
                    this._habilitaProcedimentoSUS = false;
                */

                //this.OnPropertyChanged("ProcedimentoSUS");
                this.OnPropertyChanged("ProcedimentosAMBPossiveis");
                this.OnPropertyChanged("MotivosAltaPossiveis");
                this.OnPropertyChanged("HabilitaProcedimentoSUS");
                this.OnPropertyChanged("ProcedimentoAMB");
                this.OnPropertyChanged("IdProcedimentoAMB");
            }
        }

        public string IdProcedimentoAMB
        {
            get
            {
                if (_sumarioAlta.Procedimento.IsNull())
                    return string.Empty;
                return _sumarioAlta.Procedimento.ID;
            }
            set
            {
                if (!value.IsEmptyOrWhiteSpace() && value.Length > 7)
                    _sumarioAlta.Procedimento = ProcedimentosAMBPossiveis.Where(x => x.ID == value).FirstOrDefault();
                else
                    _sumarioAlta.Procedimento = null;

                this.OnPropertyChanged("ProcedimentoAMB");
                this.OnPropertyChanged("IdProcedimentoAMB");
            }
        }

        /*public wrpProcedimentoSUS ProcedimentoSUS
        {
            get { return _sumarioAlta.ProcedimentoSUS; }
            set
            {
                this._sumarioAlta.ProcedimentoSUS = value;
                this.OnPropertyChanged("ProcedimentoSUS");
            }
        }*/

        public wrpCid CidPrincipal
        {
            get { return _sumarioAlta.CIDPrincipal; }
            set
            {
                if (value != null)
                {
                    if (!(value.Sexo == SexoCid.Ambos || this.SumarioAlta.Atendimento.Paciente.Sexo == Sexo.Indefinido))
                        if (!(value.Sexo.ToString() == this.SumarioAlta.Atendimento.Paciente.Sexo.ToString()))
                        {
                            DXMessageBox.Show("Este CID não atende ao sexo do paciente, selecione um novo CID.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                            this._sumarioAlta.CIDPrincipal = null;
                            return;
                        }

                    if (value.OPC.HasValue && value.OPC == SimNao.Nao)
                    {
                        DXMessageBox.Show("Este CID não atende ao sexo do paciente, selecione um novo CID.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                        this._sumarioAlta.CIDPrincipal = null;
                        return;
                    }
                }
                this._sumarioAlta.CIDPrincipal = value;
                this.OnPropertyChanged("CidPrincipal");
            }
        }

        public wrpCid CidSelecionado
        {
            get { return _cidSelecionado; }
            set
            {
                this._cidSelecionado = value;
                this.OnPropertyChanged("CidSelecionado");
            }
        }

        public wrpMotivoAlta MotivoAlta
        {
            get { return _sumarioAlta.MotivoAlta; }
            set
            {
                MotivoAltaRelatorio.DescMotivoAltaRelatorio = string.Empty;
                this.SetorObito = null;
                if (value != null)
                {
                    MotivoAltaRelatorio.DescMotivoAltaRelatorio = value.Descricao;
                    if (value.Tipo == TipoMotivoAlta.Óbito && IsVisibleTipoAmbulatorial == Visibility.Visible)
                    {
                        this._habilitaSetorObito = Visibility.Visible;
                        this._setorObito = _sumarioAlta.Atendimento.Leito.IsNull()
                            || _sumarioAlta.Atendimento.Leito.UnidadeInternacao.IsNull()
                            || _sumarioAlta.Atendimento.Leito.UnidadeInternacao.Setor.IsNull() ? null : _sumarioAlta.Atendimento.Leito.UnidadeInternacao.Setor;
                    }
                    else
                        this._habilitaSetorObito = Visibility.Collapsed;

                    if (!this._sumarioAlta.MotivoAlta.IsNull())
                    {
                        if (this._sumarioAlta.MotivoAlta.Tipo != TipoMotivoAlta.Transferido)
                        {
                            if (value.Tipo == TipoMotivoAlta.Transferido)
                            {
                                this._vmsumarioalta.HabilitaAbaTransferencia = true;
                                if (this._vmsumarioalta.MostraAbas)
                                    if (DXMessageBox.Show("Deseja informar os dados de Transferência?", "Sumário de Alta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                        this._vmsumarioalta.AbaSelecionada = vmSumarioAlta.TabsSumarioAlta.Transferencia;
                            }
                            else
                                this._vmsumarioalta.HabilitaAbaTransferencia = false;
                        }
                        else if (value.Tipo != TipoMotivoAlta.Transferido)
                            this._vmsumarioalta.HabilitaAbaTransferencia = false;
                    }
                    else
                        if (value.Tipo == TipoMotivoAlta.Transferido)
                        {
                            this._vmsumarioalta.HabilitaAbaTransferencia = true;
                            if (this._vmsumarioalta.MostraAbas)
                                if (DXMessageBox.Show("Deseja informar os dados de Transferência?", "Sumário de Alta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                    this._vmsumarioalta.AbaSelecionada = vmSumarioAlta.TabsSumarioAlta.Transferencia;
                        }
                        else
                            this._vmsumarioalta.HabilitaAbaTransferencia = false;
                }
                else
                {
                    this._habilitaSetorObito = Visibility.Collapsed;
                    this._vmsumarioalta.HabilitaAbaTransferencia = false;
                }
                this._sumarioAlta.MotivoAlta = value;
                this.OnPropertyChanged("HabilitaSetorObito");
                this.OnPropertyChanged("SetorObito");
                this.OnPropertyChanged("MotivoAlta");
            }
        }

        public wrpSetor SetorObito
        {
            get { return _setorObito; }
            set
            {
                this._setorObito = value;
                this.OnPropertyChanged("SetorObito");
            }
        }

        public wrpSumarioAlta SumarioAlta
        {
            get
            {
                return this._sumarioAlta;
            }
            set
            {
                this._sumarioAlta = value;
                this.OnPropertyChanged("SumarioAlta");
            }
        }

        public bool AbrePerguntaPrescricao { get; set; }
        #endregion

        #region Propriedades Privadas
        private Usuarios _usuario { get; set; }
        private wrpSumarioAlta _sumarioAlta { get; set; }
        private DateTime _dataSelecionada { get; set; }
        private string _horaSelecionada { get; set; }
        private bool _habilitaHora { get; set; }
        private bool _habilitaProcedimentoSUS { get; set; }
        private Visibility _habilitaSetorObito { get; set; }
        //private wrpProcedimento _procedimentoAMB { get; set; }
        //private wrpProcedimentoSUS _procedimentoSUS { get; set; }
        //private wrpCid _cidPrincipal { get; set; }
        private wrpCid _cidSelecionado { get; set; }
        //private wrpMotivoAlta _motivoAlta { get; set; }
        private wrpSetor _setorObito { get; set; }
        private vmSumarioAlta _vmsumarioalta { get; set; }
        private bool _habilitaconfirmaalta { get; set; }
        #endregion

        #region Metodos Publicos
        public void SetaCidSelecionado(Cid pCid)
        {
            this._cidSelecionado = new wrpCid(pCid);
        }

        public void SetaCidPrincipal(Cid pCid)
        {
            if (pCid != null)
                this.CidPrincipal = new wrpCid(pCid);
            else
                this.CidPrincipal = null;
            this.OnPropertyChanged("CidPrincipal");
        }

        public bool RealizaAlta()
        {
            //if (DXMessageBox.Show("Deseja realmente realizar a Alta Médica?", "Sumario de Alta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //{
            if (_sumarioAlta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao)
                return RealizaAltaInternado();
            else
                return RealizaAltaOutros();
            //}
            //return false;
        }

        public bool ValidaAlta()
        {
            AbrePerguntaPrescricao = false;
            IRepositorioDeSumarioAlta repsum = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
            var _ok = repsum.ValidaRealizaAlta(this._sumarioAlta.Atendimento.ID);
            if (_ok)
            {
                if (_sumarioAlta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao)
                    return ValidaAltaInternado();
                else
                    return ValidaAltaOutros();
            }
            else
            {
                if (_sumarioAlta.Atendimento.TipoDeAtendimento == TipoAtendimento.Ambulatorio)
                    if (_sumarioAlta.Atendimento.DataAtendimento.AddHours(23) <= DateTime.Now)
                        return ValidaAltaOutros();
                AbrePerguntaPrescricao = true;
                return false;
            }
        }

        private bool RealizaAltaInternado()
        {
            try
            {
                DateTime DataAlta;
                if (this._dataSelecionada > DateTime.Today)
                {
                    this._horaSelecionada = "00:00";
                    this._sumarioAlta.Atendimento.DataAltaMedica = null;
                }
                else if (string.IsNullOrWhiteSpace(this._horaSelecionada))
                {
                    DXMessageBox.Show("Preencha a Hora da alta médica.", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }

                DataAlta = new DateTime(this._dataSelecionada.Year, this._dataSelecionada.Month, this._dataSelecionada.Day, int.Parse(this._horaSelecionada.Split(':')[0]), int.Parse(this._horaSelecionada.Split(':')[1]), 0);

                this._sumarioAlta.RealizaAltaInternado(DataAlta, this._sumarioAlta.Procedimento.DomainObject, /*this._sumarioAlta.ProcedimentoSUS.DomainObject,*/
                this._sumarioAlta.MotivoAlta.DomainObject, this._setorObito == null ? null : this._setorObito.DomainObject, this.CidPrincipal.DomainObject, this._usuario);

                this._vmsumarioalta.setaDadosDeImpressao();

                IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
                rep.Refresh(_sumarioAlta.Atendimento.DomainObject);
                IRepositorioDeSumarioAlta repsum = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
                repsum.Refresh(this._sumarioAlta.DomainObject);
                this._vmsumarioalta.SumarioAlta = this._sumarioAlta;
                if (DataAlta.Date == DateTime.Today)
                    this._vmsumarioalta.MostraAbas = false;
                this._vmsumarioalta.AbaSelecionada = vmSumarioAlta.TabsSumarioAlta.Concluir;


                if (DataAlta.Date == DateTime.Today)
                {
                    logAlta("ALTA", "Realizou Alta Internado");
                    DXMessageBox.Show("Alta realizada com sucesso!", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    logAlta("ALTA", "Realizou Alta Internado dia seguinte");
                    DXMessageBox.Show("Alta para o dia seguinte realizada com sucesso!", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                return true;

            }
            catch (BusinessValidatorException ex)
            {
                string ret = string.Empty;
                ex.GetErros().ToList().ForEach(x => ret += x.Message + Environment.NewLine);
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void logAlta(string tabela, string descricao)
        {
            ISistemaService srvsis = ObjectFactory.GetInstance<ISistemaService>();
            Sistemas sis = srvsis.FiltraPorId(int.Parse(ConfigurationManager.AppSettings["Sistema"].ToString()));

            SistemasLog log = new SistemasLog
            {
                Sistema = sis
              ,
                Acao = Acao.Inserir
              ,
                Usuarios = this._usuario
              ,
                Chave = this._sumarioAlta.Atendimento.ID
              ,
                Data = DateTime.Now
              ,
                Tabela = tabela
              ,
                Observacao = descricao
              ,
                Dispositivo = Environment.MachineName
            };

            ExplicitArguments args = new ExplicitArguments();
            args.SetArg("sistema", sis.ID);
            IAcessoSistemaLogService srvlog = ObjectFactory.GetInstance<IAcessoSistemaLogService>(args);

            srvlog.Gravar(log);

        }

        private bool ValidaAltaInternado()
        {
            if (this._sumarioAlta.Procedimento == null)
            {
                DXMessageBox.Show("Preencha o Procedimento AMB.", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            /*if (this._sumarioAlta.ProcedimentoSUS == null)
            {
                DXMessageBox.Show("Preencha o Procedimento SUS.", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }*/

            if (this._sumarioAlta.MotivoAlta != null)
            {
                if (this.MotivoAlta.Descricao.ToString().Contains("OBITO"))
                {
                    if (DXMessageBox.Show("Um Motivo de Alta com 'ÓBITO' foi selecionado." + Environment.NewLine + Environment.NewLine +
                        "Motivo de Alta selecionado: " + this.MotivoAlta.Descricao + Environment.NewLine + Environment.NewLine +
                        "Deseja realmente realizar a Alta Médica?", "Sumario de Alta", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes)
                        return false;
                }
            }
            else
            {
                DXMessageBox.Show("Preencha o Motivo de Alta.", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (this.CidPrincipal == null)
            {
                DXMessageBox.Show("Preencha o CID principal.", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            try { this._sumarioAlta.ValidaAltaInternado(this._sumarioAlta.Procedimento.DomainObject, /*this._sumarioAlta.ProcedimentoSUS.DomainObject,*/ this._sumarioAlta.MotivoAlta.DomainObject, this._setorObito == null ? null : this._setorObito.DomainObject, this.CidPrincipal.DomainObject); }
            catch (BusinessValidatorException ex)
            {
                string ret = string.Empty;
                ex.GetErros().ToList().ForEach(x => ret += x.Message + Environment.NewLine);
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool ValidaAltaOutros()
        {
            if (this.CidPrincipal == null)
            {
                DXMessageBox.Show("Preencha o CID principal.", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (this._sumarioAlta.MotivoAlta == null)
            {
                DXMessageBox.Show("Preencha o Motivo de Alta.", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            try
            {
                this._sumarioAlta.ValidaAltaOutros(this._sumarioAlta.MotivoAlta.DomainObject, this._sumarioAlta.CIDPrincipal.DomainObject);
            }
            catch (BusinessValidatorException ex)
            {
                string ret = string.Empty;
                ex.GetErros().ToList().ForEach(x => ret += x.Message + Environment.NewLine);
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool RealizaAltaOutros()
        {
            try
            {
                this._sumarioAlta.RealizaAltaOutros(this._sumarioAlta.MotivoAlta.DomainObject, this._sumarioAlta.CIDPrincipal.DomainObject);

                this._vmsumarioalta.setaDadosDeImpressao();

                logAlta("ALTA", "Realizou Alta Outros");

                this._vmsumarioalta.MostraAbas = false;
                this._vmsumarioalta.AbaSelecionada = vmSumarioAlta.TabsSumarioAlta.Concluir;

                DXMessageBox.Show("Alta realizada com sucesso!", "Sumário de Alta", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;

            }
            catch (BusinessValidatorException ex)
            {
                string ret = string.Empty;
                ex.GetErros().ToList().ForEach(x => ret += x.Message + Environment.NewLine);
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Sumario de Alta", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        #endregion

        #region Commands
        public ICommand AddCidCommand { get; set; }
        public ICommand RemoveCidCommand { get; set; }
        #endregion
    }
}
