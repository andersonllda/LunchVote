using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.WPF;
using System.Text;
using HMV.Core.Domain.Constant;
using HMV.Core.Domain.Model.PEP;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica
{
    public class vmSumarioAvaliacaoPreAnestesica : ViewModelBase
    {
        public bool VerificaAtualizacaoAtendimento(int IdSumario)
        {
            IRepositorioDeSumarioAvaliacaoPreAnestesica repo = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
            var sum = repo.OndeIDIgual(IdSumario).Single();
            repo.Refresh(sum);
            this._sumarioavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesica(sum);

            if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.IsNotNull())
            {
                IRepositorioAvisosDeCirurgia repav = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
                var avcir = repav.OndeCodigoDoAvisoIgual(this._sumarioavaliacaopreanestesica.AvisoCirurgia.cd_aviso_cirurgia).Single();
                repav.Refresh(avcir);
                this._avisocirurgia = new wrpAvisoCirurgia(avcir);
                if (this._avisocirurgia.Atendimento.IsNotNull())
                {
                    this._vmsumarioavaliacaopreanestesicacabecalho = null;
                    return true;
                }
            }
            return false;
        }

        #region Contrutor
        public vmSumarioAvaliacaoPreAnestesica(Paciente pPaciente, Usuarios pUsuarios, int IdSumario, Atendimento pAtendimento)
        {
            this._paciente = new wrpPaciente(pPaciente);
            this._usuarios = new wrpUsuarios(pUsuarios);
            this._atendimento = new wrpAtendimento(pAtendimento);
            this._abreselecaoavisocirurgia = false;

            IRepositorioDeSumarioAvaliacaoPreAnestesica repo = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
            var sum = repo.OndeIDIgual(IdSumario).Single();
            repo.Refresh(sum);
            this._sumarioavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesica(sum);

            if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.IsNotNull())
            {
                IRepositorioAvisosDeCirurgia repav = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
                var avcir = repav.OndeCodigoDoAvisoIgual(this._sumarioavaliacaopreanestesica.AvisoCirurgia.cd_aviso_cirurgia).Single();

                repav.Refresh(avcir);
                this._avisocirurgia = new wrpAvisoCirurgia(avcir);
                this._sumarioavaliacaopreanestesica.AvisoCirurgia = this._avisocirurgia;
                if (this._avisocirurgia.Atendimento.IsNotNull())
                {
                    this._sumarioavaliacaopreanestesica.CID = this._avisocirurgia.Atendimento.Cid.IsNotNull() ? this._avisocirurgia.Atendimento.Cid : null;
                    this._sumarioavaliacaopreanestesica.Convenio = this._avisocirurgia.Atendimento.Convenio;
                    this._sumarioavaliacaopreanestesica.Leito = this._avisocirurgia.Atendimento.Leito;
                    this._sumarioavaliacaopreanestesica.TipoAtendimento = this._avisocirurgia.Atendimento.TipoDeAtendimento;
                }
                if (this._sumarioavaliacaopreanestesica.PrestadorCirurgiao.IsNull())
                    this._vinculaCirurgiao();
            }

            IRepositorioDeSumarioAvaliacaoPreAnestesica rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
            var ret = rep.OndeIdPacienteAviso(_paciente.ID).List();
            if (ret.Where(x => x.DataEmissao.IsNotNull()).ToList().HasItems())
                _sumariosavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesicaCollection(rep.OndeIdPacienteAviso(_paciente.ID).List().Where(x => x.DataEmissao.IsNotNull()).ToList());

            this._vms = new List<vmSumarioAvaliacaoPreAnestesicaItem>();
            base.HabilitaBotaoSelecionar();

            if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.IsNotNull() && this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.IsNotNull() && this._sumarioavaliacaopreanestesica.Temporario == SimNao.Sim)
                this.ImportaAdmissaoNovamente = true;

            this._vmsumarioavaliacaopreanestesicacabecalho = new vmSumarioAvaliacaoPreAnestesicaCabecalho(this._sumarioavaliacaopreanestesica);
        }

        public vmSumarioAvaliacaoPreAnestesica(Paciente pPaciente, Usuarios pUsuarios, Atendimento pAtendimento)
        {
            this._paciente = new wrpPaciente(pPaciente);
            this._usuarios = new wrpUsuarios(pUsuarios);
            this._atendimento = new wrpAtendimento(pAtendimento);
            IRepositorioAvisosDeCirurgia repav = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
            var avcir = repav.OndeCodigoPacienteIgual(pPaciente.ID).List();
            //- Avisos confirmados e com sumário de avaliação pré-anestésico preenchido deverão permanecer em tela até a alta do paciente.
            //- Avisos confirmados e sem sumário de avaliação pré-anestésico preenchido deverão permanecer em tela //#RETIRADO NA ATIVIDADE TFS3844 até 48hs do antes da data agendada #
            //(dt_inicio_age_cir) e 48hs após a data agendada.
            //- Avisos agendados deverão permanecer em tela até 24hs do antes da data agendada (dt_inicio_age_cir) //#RETIRADO e 24hs após a data agendada.#//
            //- Avisos cancelados não deverão ser apresentados na tela.

            avcir = avcir.Where(x =>
                (x.Situacao == SituacaoAviso.Realizada && x.SumarioAvaliacaoPreAnestesica.IsNotNull() && x.Atendimento.DataAltaMedica.IsNull() && x.DataRealizacao.Value.AddHours(24) > DateTime.Now)
                || (x.Situacao == SituacaoAviso.Realizada && x.SumarioAvaliacaoPreAnestesica.IsNull()
                 && x.AgendaCirurgias.Count(y => /*y.DataInicio >= DateTime.Today.AddDays(-2) &&*/ y.DataInicio <= DateTime.Today.AddDays(2)) > 0
                 && x.DataRealizacao.Value.AddHours(24) > DateTime.Now)
                || (x.Situacao == SituacaoAviso.Agendada && x.AgendaCirurgias.Count(y => y.DataInicio >= DateTime.Today.AddDays(-1)) > 0)).ToList(); //&& y.DataInicio <= DateTime.Today.AddDays(1)

            avcir = avcir.Where(x => x.Atendimento.IsNull() || x.Atendimento.ID == pAtendimento.ID).ToList();
            this._avisocirurgiacollection = new wrpAvisoCirurgiaCollection(avcir);

            if (avcir.HasItems())
            {
                foreach (var aviso in avcir)
                    repav.Refresh(aviso);

                if (avcir.Count > 1)
                    this._abreselecaoavisocirurgia = true;
                else
                {
                    this._avisocirurgia = this._avisocirurgiacollection.First();
                    if (this._avisocirurgia.SumarioAvaliacaoPreAnestesica.IsNull())
                    {
                        if (_carregaSumarioNovo())
                            this._novo();
                    }
                    else
                    {
                        this._sumarioavaliacaopreanestesica = this._avisocirurgia.SumarioAvaliacaoPreAnestesica;
                        if (this._avisocirurgia.Atendimento.IsNotNull())
                        {
                            this._sumarioavaliacaopreanestesica.CID = this._avisocirurgia.Atendimento.Cid.IsNotNull() ? this._avisocirurgia.Atendimento.Cid : null;
                            this._sumarioavaliacaopreanestesica.Convenio = this._avisocirurgia.Atendimento.Convenio;
                            this._sumarioavaliacaopreanestesica.Leito = this._avisocirurgia.Atendimento.Leito;
                            this._sumarioavaliacaopreanestesica.TipoAtendimento = this._avisocirurgia.Atendimento.TipoDeAtendimento;
                        }
                        if (this._sumarioavaliacaopreanestesica.PrestadorCirurgiao.IsNull())
                            this._vinculaCirurgiao();
                    }
                }
            }
            else
                this._chamaNovo();

            IRepositorioDeSumarioAvaliacaoPreAnestesica rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
            var ret = rep.OndeIdPacienteAviso(_paciente.ID).List();
            if (ret.Where(x => x.DataEmissao.IsNotNull()).ToList().HasItems())
                this._sumariosavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesicaCollection(rep.OndeIdPacienteAviso(_paciente.ID).List().Where(x => x.DataEmissao.IsNotNull()).ToList());

            this._vms = new List<vmSumarioAvaliacaoPreAnestesicaItem>();
            base.HabilitaBotaoSelecionar();
        }
        #endregion

        #region Propriedades Privadas
        private wrpAnestesiaTipo _tipoanestesia;
        private vmSumarioAvaliacaoPreAnestesicaCabecalho _vmsumarioavaliacaopreanestesicacabecalho;
        private bool _abreselecaoavisocirurgia;
        private wrpAvisoCirurgiaCollection _avisocirurgiacollection;
        private wrpAvisoCirurgia _avisocirurgia;
        private wrpSumarioAvaliacaoPreAnestesica _sumarioavaliacaopreanestesica;
        private wrpUsuarios _usuarios;
        private wrpAtendimento _atendimento;
        private wrpPaciente _paciente;
        //private wrpEventoAnestesia _eventoanestesia;
        private List<vmSumarioAvaliacaoPreAnestesicaItem> _vms;
        private int _tabindex;
        private wrpSumarioAvaliacaoPreAnestesicaCollection _sumariosavaliacaopreanestesica;
        private wrpSumarioAvaliacaoPreAnestesica _sumarioavaliacaopreanestesicaimportacao;
        private wrpSumarioAvaliacaoPreAnestesicaCollection _sumariosavaliacaoweb;
        private wrpSumarioAvaliacaoPreAnestesica _sumarioavaliacaowebselecionado;
        private bool _abrevinculoweb;
        #endregion

        #region Propriedades Publicas
        public wrpAtendimento Atendimento
        {
            get
            {
                return _atendimento;
            }
        }

        public bool PodeFinalizar { get { return this._avisocirurgia.IsNotNull() && this._avisocirurgia.Atendimento.IsNotNull(); } }
        public bool MesmoAtendimento { get { return this._avisocirurgia.IsNotNull() && this._avisocirurgia.Atendimento.ID == this._atendimento.ID; } }
        public bool ImportaAdmissaoNovamente { get; set; }

        public wrpPaciente Paciente
        {
            get { return _paciente; }
        }

        public bool Novo { get; set; }
        public List<vmSumarioAvaliacaoPreAnestesicaItem> VMItens
        {
            get
            {
                return this._vms;
            }
        }

        public bool FechaSumario { get; set; }

        //public wrpEventoAnestesia EventoAnestesia
        //{
        //    get
        //    {
        //        if (this._eventoanestesia.IsNull())
        //        {
        //            IRepositorioDeEventoAnestesia repe = ObjectFactory.GetInstance<IRepositorioDeEventoAnestesia>();
        //            this._eventoanestesia = new wrpEventoAnestesia(repe.Single());
        //        }
        //        return this._eventoanestesia;
        //    }
        //}

        public wrpAnestesiaTipo AnestesiaTipo
        {
            get
            {
                if (this._tipoanestesia.IsNull())
                {
                    IRepositorioDeAnestesiaTipo rep = ObjectFactory.GetInstance<IRepositorioDeAnestesiaTipo>();
                    this._tipoanestesia = new wrpAnestesiaTipo(rep.OndeIDIgual((int)AnestesiaTipos.PreAnestesica).Single());
                }
                return this._tipoanestesia;
            }
        }

        public vmSumarioAvaliacaoPreAnestesicaCabecalho vmSumarioAvaliacaoPreAnestesicaCabecalho
        {
            get
            {
                if (this._vmsumarioavaliacaopreanestesicacabecalho.IsNull())
                    if (this._sumarioavaliacaopreanestesica.IsNotNull())
                        this._vmsumarioavaliacaopreanestesicacabecalho = new vmSumarioAvaliacaoPreAnestesicaCabecalho(this._sumarioavaliacaopreanestesica);
                return this._vmsumarioavaliacaopreanestesicacabecalho;
            }
        }

        public bool AbreSelecaoAvisoCirurgia
        {
            get
            {
                return this._abreselecaoavisocirurgia;
            }
        }

        public wrpAvisoCirurgiaCollection AvisoCirurgiaCollection
        {
            get
            {
                return this._avisocirurgiacollection;
            }
        }

        public wrpAvisoCirurgia AvisoCirurgia
        {
            get
            {
                return this._avisocirurgia;
            }
            set
            {
                this._avisocirurgia = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.AvisoCirurgia));
            }
        }

        public wrpSumarioAvaliacaoPreAnestesica SumarioAvaliacaoPreAnestesica
        {
            get
            {
                return this._sumarioavaliacaopreanestesica;
            }
        }

        public bool Selecionou { get; set; }
        public bool SelecionouWeb { get; set; }
        public int TabIndex
        {
            get { return _tabindex; }
            set
            {
                _tabindex = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.TabIndex));
            }
        }

        public bool SumarioPreAnestesicoFinalizado
        {
            get { return this._sumarioavaliacaopreanestesica.DataEmissao.IsNotNull(); }
        }

        public void FinalizaImprime()
        {
            this.CommandSalvar(null);
        }
        //Vinculo WEB
        public wrpSumarioAvaliacaoPreAnestesicaCollection SumariosAvaliacaoWeb
        {
            get { return this._sumariosavaliacaoweb; }
        }

        public wrpSumarioAvaliacaoPreAnestesica SumarioAvaliacaoWebSelecionado
        {
            get
            {
                return this._sumarioavaliacaowebselecionado;
            }
            set
            {
                this._sumarioavaliacaowebselecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.SumarioAvaliacaoWebSelecionado));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.BotaoVincular));
            }
        }

        public bool AbreVinculoWeb
        {
            get
            {
                return this._abrevinculoweb;
            }
        }

        public bool BotaoVincular { get { return this._sumarioavaliacaowebselecionado.IsNotNull(); } }

        //Importacao
        public wrpSumarioAvaliacaoPreAnestesicaCollection SumariosAvaliacaoPreAnestesica
        {
            get
            {
                return this._sumariosavaliacaopreanestesica;
            }
        }

        public wrpSumarioAvaliacaoPreAnestesica SumarioAvaliacaoPreAnestesicaImportacao
        {
            get
            {
                return this._sumarioavaliacaopreanestesicaimportacao;
            }
            set
            {
                this._sumarioavaliacaopreanestesicaimportacao = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.SumarioAvaliacaoPreAnestesicaImportacao));
            }
        }

        public bool HabilitaImportarDados
        {
            get
            {
                return _sumariosavaliacaopreanestesica.HasItems() && !this.SumarioPreAnestesicoFinalizado;
            }
        }
        #endregion

        #region Commands
        protected override bool CommandCanExecuteSelecionar(object param)
        {
            return this._avisocirurgia != null;
        }
        protected override void CommandSelecionar(object param)
        {
            if (this._avisocirurgia.SumarioAvaliacaoPreAnestesica.IsNull())
            {
                if (this._carregaSumarioNovo())
                    this._novo();
            }
            else
            {
                this._sumarioavaliacaopreanestesica = this._avisocirurgia.SumarioAvaliacaoPreAnestesica;
                if (this._avisocirurgia.Atendimento.IsNotNull())
                {
                    if (this._sumarioavaliacaopreanestesica.CID.IsNull())
                        this._sumarioavaliacaopreanestesica.CID = this._avisocirurgia.Atendimento.Cid.IsNotNull() ? this._avisocirurgia.Atendimento.Cid : null;
                    if (this._sumarioavaliacaopreanestesica.Convenio.IsNull())
                        this._sumarioavaliacaopreanestesica.Convenio = this._avisocirurgia.Atendimento.Convenio;
                    if (this._sumarioavaliacaopreanestesica.Leito.IsNull())
                        this._sumarioavaliacaopreanestesica.Leito = this._avisocirurgia.Atendimento.Leito;
                    if (this._sumarioavaliacaopreanestesica.TipoAtendimento.IsNull())
                        this._sumarioavaliacaopreanestesica.TipoAtendimento = this._avisocirurgia.Atendimento.TipoDeAtendimento;
                }
            }

            this.Selecionou = true;
            if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.IsNotNull() && this._sumarioavaliacaopreanestesica.Temporario == SimNao.Sim)
                this.ImportaAdmissaoNovamente = true;
            this._vmsumarioavaliacaopreanestesicacabecalho = new vmSumarioAvaliacaoPreAnestesicaCabecalho(this._sumarioavaliacaopreanestesica);
            base.CommandSelecionar(param);
        }
        protected override void CommandSalvar(object param)
        {
            this._validacao();

            foreach (var vm in this._vms)
            {
                if (!vm.IsNull())
                {
                    //Salva AlergiasEvento
                    if (!vm.vmAlergiasEvento.IsNull())
                    {
                        wrpAlergiaEventoCollection _AlergiaCollection = null;
                        IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
                        repa.OndeChaveIgual(this._sumarioavaliacaopreanestesica.ID);
                        repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoPreAnestesica);
                        var reta = repa.List();
                        if (reta.IsNotNull())
                            _AlergiaCollection = new wrpAlergiaEventoCollection(reta);
                        foreach (var ale in vm.vmAlergiasEvento.AlergiaCollection)
                        {
                            var jaexiste = _AlergiaCollection.Where(x => x.Chave == this._sumarioavaliacaopreanestesica.ID && x.Alergia.ID == ale.ID).SingleOrDefault();
                            if (ale.Selecionado)
                            {
                                jaexiste.Atendimento = this._avisocirurgia.IsNotNull() ? this._avisocirurgia.Atendimento : null;
                                // jaexiste.Data = DateTime.Now;
                                // jaexiste.Usuario = this._usuarios;
                                repa.Save(jaexiste.DomainObject);
                                if (ale.Status == StatusAlergiaProblema.Temporario)
                                {
                                    ale.Status = StatusAlergiaProblema.Ativo;
                                    ale.Paciente.Save();
                                }
                            }
                            else
                            {
                                //_AlergiaCollection.Remove(jaexiste);
                                if (ale.Status == StatusAlergiaProblema.Temporario)
                                {
                                    ale.Status = StatusAlergiaProblema.Excluído;
                                    ale.Paciente.Save();
                                }
                            }
                        }
                    }

                    //Salva MedicamentosEmUsoEvento
                    if (!vm.vmMedicamentosEmUsoEvento.IsNull())
                    {
                        wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection = null;
                        IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                        repp.OndeChaveIgual(this._sumarioavaliacaopreanestesica.ID);
                        repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoPreAnestesica);
                        var ret = repp.List();
                        if (ret.IsNotNull())
                            _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);
                        foreach (var med in vm.vmMedicamentosEmUsoEvento.MedicamentosCollection)
                        {
                            var jaexiste = _MedicamentosCollection.Where(x => x.Chave == this._sumarioavaliacaopreanestesica.ID && x.MedicamentosEmUso.ID == med.ID).SingleOrDefault();
                            if (med.Selecionado)
                            {
                                jaexiste.Atendimento = this._avisocirurgia.IsNotNull() ? this._avisocirurgia.Atendimento : null;
                                // jaexiste.Data = DateTime.Now;
                                // jaexiste.Usuario = this._usuarios;
                                repp.Save(jaexiste.DomainObject);
                                if (med.Status == StatusMedicamentosEmUso.Temporario)
                                {
                                    med.Status = StatusMedicamentosEmUso.EmUso;
                                    med.Paciente.Save();
                                }
                            }
                            else
                            {
                                //_MedicamentosCollection.Remove(jaexiste);
                                if (med.Status == StatusMedicamentosEmUso.Temporario)
                                {
                                    med.Status = StatusMedicamentosEmUso.Excluído;
                                    med.Paciente.Save();
                                }
                            }
                        }
                    }

                    //Salva ExameFisicoEvento
                    if (!vm.vmExameFisicoEvento.IsNull())
                    {
                        vm.vmExameFisicoEvento.Salvar();
                    }
                }
            }

            //finaliza
            if (this._avisocirurgia.IsNotNull())
            {
                this._sumarioavaliacaopreanestesica.CID = this._avisocirurgia.Atendimento.Cid;
                this._sumarioavaliacaopreanestesica.Convenio = this._avisocirurgia.Atendimento.Convenio;
                this._sumarioavaliacaopreanestesica.Leito = this._avisocirurgia.Atendimento.Leito;
            }
            this._sumarioavaliacaopreanestesica.UsuarioEmissao = this._usuarios;
            this._sumarioavaliacaopreanestesica.DataEmissao = DateTime.Now;
            this._sumarioavaliacaopreanestesica.Temporario = SimNao.Nao;
            this._sumarioavaliacaopreanestesica.Save();
            Memory.MinimizeMemory();

            DXMessageBox.Show("Sumário de Avaliação Pré-Anestésica Salvo com Sucesso!", "Sumário de Avaliação Pré-Anestésica", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Metodos privados
        private wrpPrestador cir = new wrpPrestador();
        private void _chamaNovo()
        {
            if (_usuarios.Prestador.IsNurse)
            {
                DXMessageBox.Show("O usuário logado não tem acesso para realizar sumário de avaliação pré-anestésica. Para reimpressão acesse através do menu Sumário de atendimentos.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                this.FechaSumario = true;
                return;
            }

            IRepositorioDeSumarioAvaliacaoPreAnestesica repo = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
            var ret1 = repo.OndeIdPaciente(_paciente.ID).List();
            if (ret1.Where(x => x.DataEmissao.IsNull()).ToList().HasItems())
            {
                this._sumarioavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesica(repo.OndeIdPaciente(_paciente.ID).List()
                        .Where(x => x.DataEmissao.IsNull()).ToList().OrderByDescending(x => x.ID).FirstOrDefault());
            }
            else
            {
                if (DXMessageBox.Show("O Paciente não possui Aviso de Cirurgia!" + Environment.NewLine + "Deseja criar o Sumário mesmo assim?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    this.FechaSumario = true;
                    return;
                }
                else
                    this._novo();
            }
        }
        private bool _carregaSumarioNovo()
        {
            IRepositorioDeSumarioAvaliacaoPreAnestesica repo = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
            var ret1 = repo.OndeIdPaciente(_paciente.ID).List();
            if (ret1.Where(x => x.DataEmissao.IsNull()).ToList().HasItems())
            {
                this._sumarioavaliacaopreanestesica =
                    new wrpSumarioAvaliacaoPreAnestesica(repo.OndeIdPaciente(_paciente.ID).List()
                        .Where(x => x.DataEmissao.IsNull()).ToList().OrderByDescending(x => x.ID).FirstOrDefault());
                this._sumarioavaliacaopreanestesica.AvisoCirurgia = this._avisocirurgia;

                this._vinculaCirurgiao();

                IRepositorioAvisosDeCirurgia repav = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
                repav.Refresh(this._avisocirurgia.DomainObject);
                this._avisocirurgia = new wrpAvisoCirurgia(this._avisocirurgia.DomainObject);
                this._vmsumarioavaliacaopreanestesicacabecalho = new vmSumarioAvaliacaoPreAnestesicaCabecalho(this._sumarioavaliacaopreanestesica);
                return false;
            }
            return true;
        }
        private void _vinculaCirurgiao()
        {
            wrpPrestador cir = new wrpPrestador();
            if (this._avisocirurgia.IsNotNull())
            {
                if (this._avisocirurgia.EquipesMedicas.HasItems())
                {
                    var ret = this._avisocirurgia.EquipesMedicas.Where(x => x.AtividadeMedica.ID == "1" || x.AtividadeMedica.ID == "01").ToList();
                    cir = ret.OrderByDescending(x => x.Principal == SimNao.Sim).Select(x => x.Prestador).FirstOrDefault();
                }
                if (cir.IsNull())
                {
                    throw new Exception("Não há médico cirurgião no Aviso de Cirurgia!");
                }
            }
            this._sumarioavaliacaopreanestesica.PrestadorCirurgiao = cir;
            this._sumarioavaliacaopreanestesica.Cirurgia = this._avisocirurgia.ProcedimentosCirurgicos.Single(x => x.Principal == SimNao.Sim).Cirurgia;
            this._sumarioavaliacaopreanestesica.Save();
        }
        private void _buscasumarioWEB()
        {
            this._abrevinculoweb = false;
            this._sumariosavaliacaoweb = null;
            //busca sumariosweb            
            IRepositorioDeSumarioAvaliacaoPreAnestesica repweb = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
            var retweb = repweb.OndeSumariosWeb(this._usuarios.DomainObject).List().ToList();
            if (retweb.HasItems())
            {
                List<HMV.Core.Domain.Model.PEP.SumarioDeAvaliacaoPreAnestesica.SumarioAvaliacaoPreAnestesica> final = new List<HMV.Core.Domain.Model.PEP.SumarioDeAvaliacaoPreAnestesica.SumarioAvaliacaoPreAnestesica>();
                var lst = retweb.Where(x => x.CPF == _paciente.CPF).ToList();
                if (lst.HasItems())
                {
                    final.AddRange(lst.OrderByDescending(x => x.DataInclusao).ToList());
                    foreach (var item in retweb.OrderByDescending(x => x.DataInclusao).ToList())
                        if (lst.Count(x => x.ID == item.ID) == 0)
                            final.Add(item);

                    this._sumariosavaliacaoweb = new wrpSumarioAvaliacaoPreAnestesicaCollection(final);
                    this._abrevinculoweb = true;
                }
                else
                {
                    retweb = retweb.OrderByDescending(x => x.DataInclusao).ToList();
                    this._sumariosavaliacaoweb = new wrpSumarioAvaliacaoPreAnestesicaCollection(retweb);
                    this._abrevinculoweb = true;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.BotaoVincular));
            }
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.SumariosAvaliacaoWeb));
        }
        private void _novo(bool pNaoBuscaWeb = false)
        {
            //TODOPRE
            if (!pNaoBuscaWeb)
                this._buscasumarioWEB();
            else
                this._abrevinculoweb = false;

            cir = new wrpPrestador();
            if (this._avisocirurgia.IsNotNull())
            {
                if (this._avisocirurgia.EquipesMedicas.HasItems())
                {
                    var ret = this._avisocirurgia.EquipesMedicas.Where(x => x.AtividadeMedica.ID == "1" || x.AtividadeMedica.ID == "01").ToList();
                    cir = ret.OrderByDescending(x => x.Principal == SimNao.Sim).Select(x => x.Prestador).FirstOrDefault();
                }
                if (cir.IsNull())
                {
                    throw new Exception("Não há médico cirurgião no Aviso de Cirurgia!");
                }
            }

            if (!this._abrevinculoweb)
            {
                if (this._avisocirurgia.IsNotNull())
                {
                    if (this._avisocirurgia.Atendimento.IsNotNull())
                    {
                        this._sumarioavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesica
                        {
                            AvisoCirurgia = this._avisocirurgia,
                            CID = this._avisocirurgia.Atendimento.Cid.IsNotNull() ? this._avisocirurgia.Atendimento.Cid : null,
                            Convenio = this._avisocirurgia.Atendimento.Convenio,
                            Leito = this._avisocirurgia.Atendimento.Leito,
                            PrestadorCirurgiao = cir,
                            PrestadorAnestesia = this._usuarios.Prestador,
                            TipoAtendimento = this._avisocirurgia.Atendimento.TipoDeAtendimento,
                            UsuarioInclusao = this._usuarios,
                            Cirurgia = this._avisocirurgia.ProcedimentosCirurgicos.Single(x => x.Principal == SimNao.Sim).Cirurgia,
                            Temporario = SimNao.Nao,
                            Paciente = this._paciente
                        };
                    }
                    else
                    {
                        this._sumarioavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesica
                        {
                            AvisoCirurgia = this._avisocirurgia,
                            PrestadorCirurgiao = cir,
                            PrestadorAnestesia = this._usuarios.Prestador,
                            UsuarioInclusao = this._usuarios,
                            Cirurgia = this._avisocirurgia.ProcedimentosCirurgicos.Single(x => x.Principal == SimNao.Sim).Cirurgia,
                            Temporario = SimNao.Sim,
                            Paciente = this._paciente
                        };
                    }
                }
                else
                {
                    this._sumarioavaliacaopreanestesica = new wrpSumarioAvaliacaoPreAnestesica
                    {
                        PrestadorAnestesia = this._usuarios.Prestador,
                        UsuarioInclusao = this._usuarios,
                        Temporario = SimNao.Sim,
                        Paciente = this._paciente
                    };
                }

                this._sumarioavaliacaopreanestesica.Save();

                if (this._avisocirurgia.IsNotNull())
                {
                    IRepositorioAvisosDeCirurgia repav = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
                    repav.Refresh(this._avisocirurgia.DomainObject);
                }
            }
            Novo = true;
        }
        private void _validacao()
        {
            this._tabindex = 0;
            IList<string> erros = new List<string>();

            foreach (var vm in this._vms)
            {
                vm.RefreshViewModel();

                foreach (var item in vm.ItensCollection)
                {
                    if (item.AnestesiaItem.IsObrigatorio == SimNao.Nao)
                        continue;

                    if (vm.AnestesiaGrupo.MostraExameFisico == SimNao.Sim)
                    {
                        if ((item.NaoAvaliado == item.SemParticularidade) && item.Observacao.IsEmpty())
                        {
                            erros.Add(string.Format("O item '{0}' na aba '{1}' é obrigatório", item.Descricao, vm.AnestesiaGrupo.Descricao));
                            this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                        }
                    }

                    else if (vm.AnestesiaGrupo.MostraGrid == SimNao.Sim && vm.AnestesiaGrupo.Nega == SimNao.Sim && vm.AnestesiaGrupo.NaoSeAplica == SimNao.Sim)
                    {
                        if ((item.Nega == item.NaoSeAplica) && item.Observacao.IsEmpty())
                        {
                            erros.Add(string.Format("O item '{0}' na aba '{1}' é obrigatório", item.Descricao, vm.AnestesiaGrupo.Descricao));
                            this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                        }
                    }

                    else if (vm.AnestesiaGrupo.MostraGrid == SimNao.Sim && vm.AnestesiaGrupo.ResultadoIndisponivel == SimNao.Sim && vm.AnestesiaGrupo.NaoRealizado == SimNao.Sim)
                    {
                        if (item.NaoRealizado == item.ResultadoIndisponivel && item.Resultado.IsEmpty())
                        {
                            erros.Add(string.Format("O item '{0}' na aba '{1}' é obrigatório", item.Descricao, vm.AnestesiaGrupo.Descricao));
                            this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                        }
                    }
                    else if (vm.AnestesiaGrupo.MostraGrid == SimNao.Sim && vm.AnestesiaGrupo.Nega == SimNao.Sim && vm.AnestesiaGrupo.MostraProcedimentosRealizados == SimNao.Sim)
                    {
                        if (item.Nega == SimNao.Nao && item.Observacao.IsEmpty())
                        {
                            erros.Add(string.Format("O item '{0}' na aba '{1}' é obrigatório", item.Descricao, vm.AnestesiaGrupo.Descricao));
                            this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                        }
                    }
                    else if (vm.AnestesiaGrupo.MostraGrid == SimNao.Sim && !vm.MostraCheck)
                    {
                        if (item.Valor.IsNull() && item.Observacao.IsEmpty())
                        {
                            erros.Add(string.Format("O item '{0}' na aba '{1}' é obrigatório", item.Descricao, vm.AnestesiaGrupo.Descricao));
                            this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                        }
                    }
                }

                if (vm.AnestesiaGrupo.MostraExameFisico == SimNao.Sim)
                {
                    foreach (var item in vm.vmExameFisicoEvento.ExamesFisicoCollection)
                    {
                        if (item.SinaisVitaisTipo.IsObrigatorio == SimNao.Sim)
                            if (item.Valor.IsEmpty())
                            {
                                erros.Add(string.Format("O item '{0}' é obrigatório na aba {1}", item.SinaisVitaisTipo.Descricao, vm.AnestesiaGrupo.Descricao));
                                this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                            }
                            else if (!item.SinaisVitaisTipo.ValorMinimo.IsNull() || !item.SinaisVitaisTipo.ValorMaximo.IsNull())
                            {
                                int valoritem;
                                if (int.TryParse(item.Valor, out valoritem))
                                {
                                    if (valoritem > item.SinaisVitaisTipo.ValorMaximo || valoritem < item.SinaisVitaisTipo.ValorMinimo)
                                    {
                                        erros.Add(string.Format("O item '{0}' " + item.SinaisVitaisTipo.Mensagem + " na aba {3}", item.SinaisVitaisTipo.Descricao, item.SinaisVitaisTipo.ValorMinimo, item.SinaisVitaisTipo.ValorMaximo, vm.AnestesiaGrupo.Descricao));
                                        this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                                    }
                                }
                            }
                    }
                }

                if (vm.AnestesiaGrupo.MostraMedicamentosEmUso == SimNao.Sim)
                {
                    if (vm.vmMedicamentosEmUsoEvento.MedicamentosCollection.Count(x => x.Selecionado) == 0 && vm.vmMedicamentosEmUsoEvento.SemMedicamentosEmUso == false)
                    {
                        erros.Add("Selecione um 'Medicamento Habitual' ou marque a opção 'Não Faz Uso de Medicamentos'");
                        this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                    }
                }

                if (vm.AnestesiaGrupo.MostraAlergias == SimNao.Sim)
                {
                    if (vm.vmAlergiasEvento.AlergiaCollection.Count(x => x.Selecionado) == 0 && vm.vmAlergiasEvento.SemAlergiasConhecidas == false)
                    {
                        erros.Add("Selecione uma 'Alergia' ou marque a opção 'Sem Alergias Conhecidas'");
                        this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                    }
                }

                if (vm.AnestesiaGrupo.MostraMedicamentos == SimNao.Sim)
                {
                    if (vm.vmPreMedicacao.PreMedicacaoCollecion.Count == 0 && vm.vmPreMedicacao.NaoHaPreMedicacao == SimNao.Nao)
                    {
                        erros.Add("Informe uma 'Medicação pré-anestésica' ou marque a opção 'Não Há'");
                        this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                    }
                }

                if (vm.AnestesiaGrupo.MostraNPO == SimNao.Sim)
                {
                    if (vm.NPOHora.IsNull())
                    {
                        erros.Add("Informe a 'Hora' do NPO na aba 'Conduta Anestésica'");
                        this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                    }
                    if (vm.NPOData.IsNull())
                    {
                        erros.Add("Informe a 'Data' do NPO na aba 'Conduta Anestésica'");
                        this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                    }
                    //if (vm.ItensCollection.Count(x => x.Selecionado) == 0)
                    //{
                    //    erros.Add("Marque um dos itens da 'Conduta Anestésica'");
                    //    this._tabindex = vm.SelecionaTab > this._tabindex && this._tabindex >= 0 ? this._tabindex : vm.SelecionaTab;
                    //}
                }
            }

            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.TabIndex));

            if (erros.Count > 0)
                throw new BusinessMsgException(erros, MessageImage.Error);
        }
        #endregion

        public void CriaNovo()
        {
            this._novo(true);
        }

        public void SalvarAba(vmSumarioAvaliacaoPreAnestesicaItem vm)
        {
            if (vm.ItensCollection.HasItems())
            {
                foreach (var item in vm.ItensCollection)
                {
                    var jaexiste = this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Where(x => x.AnestesiaItem.ID == item.AnestesiaItem.ID).FirstOrDefault();
                    if (jaexiste.IsNull())
                    {
                        if (vm.MostraCheck || !item.SemParticularidade.IsNull() || !item.NaoAvaliado.IsNull()
                            || !item.NaoRealizado.IsNull() || !item.ResultadoIndisponivel.IsNull() || !item.Nega.IsNull()
                            || !item.Resultado.IsEmpty() || !item.Observacao.IsNull() || !item.NaoSeAplica.IsNull() || !item.Valor.IsEmpty())
                        {
                            wrpSumarioAvaliacaoPreAnestesicaItem novo = new wrpSumarioAvaliacaoPreAnestesicaItem(this._sumarioavaliacaopreanestesica);
                            novo.AnestesiaItem = item.AnestesiaItem;
                            novo.NaoAvaliado = item.NaoAvaliado;
                            novo.NaoRealizado = item.NaoRealizado;
                            novo.NaoSeAplica = item.NaoSeAplica;
                            novo.Nega = item.Nega;
                            novo.Observacao = item.Observacao;
                            novo.Resultado = item.Resultado;
                            novo.ResultadoIndisponivel = item.ResultadoIndisponivel;
                            novo.SemParticularidade = item.SemParticularidade;
                            novo.UsuarioInclusao = this._usuarios;
                            novo.Valor = item.Valor;

                            //Salva apenas itens marcados no grupo que mostra os checkbox
                            if (vm.MostraCheck)
                            {
                                if (item.Selecionado)
                                    this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Add(novo);
                            }
                            else
                                this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Add(novo);
                        }
                    }
                    else
                    {
                        if (vm.MostraCheck || !item.SemParticularidade.IsNull() || !item.NaoAvaliado.IsNull()
                            || !item.NaoRealizado.IsNull() || !item.ResultadoIndisponivel.IsNull() || !item.Nega.IsNull()
                            || !item.Resultado.IsEmpty() || !item.Observacao.IsNull() || !item.NaoSeAplica.IsNull() || !item.Valor.IsEmpty())
                        {
                            jaexiste.NaoAvaliado = item.NaoAvaliado;
                            jaexiste.NaoRealizado = item.NaoRealizado;
                            jaexiste.NaoSeAplica = item.NaoSeAplica;
                            jaexiste.Nega = item.Nega;
                            jaexiste.Observacao = item.Observacao;
                            jaexiste.Resultado = item.Resultado;
                            jaexiste.ResultadoIndisponivel = item.ResultadoIndisponivel;
                            jaexiste.SemParticularidade = item.SemParticularidade;
                            jaexiste.UsuarioInclusao = this._usuarios;
                            jaexiste.Valor = item.Valor;
                            //Salva apenas itens marcados no grupo que mostra os checkbox senão remove.
                            if (vm.MostraCheck)
                                if (!item.Selecionado)
                                    this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Remove(jaexiste);
                        }
                        else
                            this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Remove(jaexiste);
                    }
                }
            }

            this._sumarioavaliacaopreanestesica.Save();

            //Salva AlergiasEvento           
            if (vm.vmAlergiasEvento.IsNotNull())
                vm.vmAlergiasEvento.SalvarNovo();

            //Salva MedicamentosEmUsoEvento           
            if (vm.vmMedicamentosEmUsoEvento.IsNotNull())
                vm.vmMedicamentosEmUsoEvento.SalvarNovo();

            //Salva ExameFisicoEvento            
            if (vm.vmExameFisicoEvento.IsNotNull())
                vm.vmExameFisicoEvento.Salvar();
        }

        public void ImportaDados()
        {
            if (this._sumarioavaliacaopreanestesicaimportacao.IsNotNull())
            {
                this._sumarioavaliacaopreanestesica.Outros = this._sumarioavaliacaopreanestesicaimportacao.Outros;
                this._sumarioavaliacaopreanestesica.OutrosExames = this._sumarioavaliacaopreanestesicaimportacao.OutrosExames;
                this._sumarioavaliacaopreanestesica.PreMedicacao = this._sumarioavaliacaopreanestesicaimportacao.PreMedicacao;
                this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Clear();
                this._sumarioavaliacaopreanestesica.DomainObject.SumarioAvaliacaoPreAnestesicaItens.Clear();
                this._sumarioavaliacaopreanestesica.Save();

                foreach (var item in this._sumarioavaliacaopreanestesicaimportacao.SumarioAvaliacaoPreAnestesicaItens)
                {
                    var jaexiste = this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Where(x => x.AnestesiaItem.ID == item.AnestesiaItem.ID).SingleOrDefault();
                    if (jaexiste.IsNull())
                    {
                        this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Add(new wrpSumarioAvaliacaoPreAnestesicaItem(this._sumarioavaliacaopreanestesica)
                        {
                            AnestesiaItem = item.AnestesiaItem,
                            NaoAvaliado = item.NaoAvaliado,
                            NaoRealizado = item.NaoRealizado,
                            NaoSeAplica = item.NaoSeAplica,
                            Nega = item.Nega,
                            Observacao = item.Observacao,
                            Resultado = item.Resultado,
                            ResultadoIndisponivel = item.ResultadoIndisponivel,
                            SemParticularidade = item.SemParticularidade,
                            Valor = item.Valor,
                            UsuarioInclusao = item.UsuarioInclusao
                        });
                    }
                    else
                    {
                        jaexiste.NaoAvaliado = item.NaoAvaliado;
                        jaexiste.NaoRealizado = item.NaoRealizado;
                        jaexiste.NaoSeAplica = item.NaoSeAplica;
                        jaexiste.Nega = item.Nega;
                        jaexiste.Observacao = item.Observacao;
                        jaexiste.Resultado = item.Resultado;
                        jaexiste.ResultadoIndisponivel = item.ResultadoIndisponivel;
                        jaexiste.SemParticularidade = item.SemParticularidade;
                        jaexiste.Valor = item.Valor;
                        jaexiste.UsuarioInclusao = item.UsuarioInclusao;
                    }
                }

                this._sumarioavaliacaopreanestesica.Save();
                foreach (var vm in _vms)
                {
                    //this.SalvarAba(vm);
                    vm.RefreshViewModel();
                }

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.SumarioAvaliacaoPreAnestesica));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.vmSumarioAvaliacaoPreAnestesicaCabecalho));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesica>(x => x.VMItens));
            }
        }

        public void VincularSumarioWeb()
        {
            if (DXMessageBox.Show("Deseja realmente vincular este Sumário Pré-Anestésico a este paciente?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (this._avisocirurgia.IsNotNull())
                {
                    if (this._avisocirurgia.Atendimento.IsNotNull())
                    {
                        this._sumarioavaliacaowebselecionado.AvisoCirurgia = this._avisocirurgia;
                        this._sumarioavaliacaowebselecionado.CID = this._avisocirurgia.Atendimento.Cid.IsNotNull() ? this._avisocirurgia.Atendimento.Cid : null;
                        this._sumarioavaliacaowebselecionado.Convenio = this._avisocirurgia.Atendimento.Convenio;
                        this._sumarioavaliacaowebselecionado.Leito = this._avisocirurgia.Atendimento.Leito;
                        this._sumarioavaliacaowebselecionado.PrestadorCirurgiao = cir;
                        this._sumarioavaliacaowebselecionado.TipoAtendimento = this._avisocirurgia.Atendimento.TipoDeAtendimento;
                        this._sumarioavaliacaowebselecionado.Cirurgia = this._avisocirurgia.ProcedimentosCirurgicos.Single(x => x.Principal == SimNao.Sim).Cirurgia;
                        this._sumarioavaliacaowebselecionado.Temporario = SimNao.Nao;
                    }
                    else
                    {
                        this._sumarioavaliacaowebselecionado.AvisoCirurgia = this._avisocirurgia;
                        this._sumarioavaliacaowebselecionado.PrestadorCirurgiao = cir;
                        this._sumarioavaliacaowebselecionado.Cirurgia = this._avisocirurgia.ProcedimentosCirurgicos.Single(x => x.Principal == SimNao.Sim).Cirurgia;
                        this._sumarioavaliacaowebselecionado.Temporario = SimNao.Nao;
                    }
                }
                else
                    this._sumarioavaliacaowebselecionado.Temporario = SimNao.Sim;

                wrpAlergiaEventoCollection _AlergiaCollection = null;
                IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
                repa.OndeChaveIgual(this._sumarioavaliacaowebselecionado.ID);
                repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoPreAnestesica);
                var reta = repa.List();
                if (reta.IsNotNull())
                    _AlergiaCollection = new wrpAlergiaEventoCollection(reta);

                bool removesemalergia = false;
                foreach (var item in _AlergiaCollection)
                {
                    if (item.Alergia.Paciente.IsNull())
                    {
                        IRepositorioDeAlergia repaa = ObjectFactory.GetInstance<IRepositorioDeAlergia>();
                        if ((item.Alergia.Agente == Constantes.coSemAlergiasConhecidas 
                            && this._paciente.Alergias.Count(x => x.Status != StatusAlergiaProblema.Excluído) > 0))
                        {
                            repa.Delete(item.DomainObject);
                            repaa.Delete(item.Alergia.DomainObject);
                        }
                        else
                        {
                            if (item.Alergia.Status == StatusAlergiaProblema.Excluído)
                                repa.Delete(item.DomainObject);
                            else
                            {
                                item.Alergia.Paciente = this._paciente;
                                repaa.Save(item.Alergia.DomainObject);
                                this._paciente.Alergias.Add(item.Alergia);

                                if (item.Alergia.Agente != Constantes.coSemAlergiasConhecidas)
                                    removesemalergia = true;
                            }
                        }
                    }
                }

                if (removesemalergia)
                {
                    var listremove = new wrpAlergiaCollection(new List<Alergia>());
                    this._paciente.Alergias.Each(x =>
                    {
                        if (x.Agente == Constantes.coSemAlergiasConhecidas)
                            listremove.Add(x);
                    });
                    // Deleta o evento 
                    var alergiaEvento = _AlergiaCollection.Where(x => x.Chave == this._sumarioavaliacaowebselecionado.ID && x.Alergia.Agente == Constantes.coSemAlergiasConhecidas).FirstOrDefault();
                    if (alergiaEvento != null)
                    {
                        
                        _AlergiaCollection.Remove(alergiaEvento);
                        alergiaEvento.Alergia.Selecionado = false;
                        IRepositorioDeEventoAlergias rep = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
                        rep.Delete(alergiaEvento.DomainObject);                        
                    }
                    listremove.Each(x =>
                    {
                        x.Selecionado = false;
                        x.Status = StatusAlergiaProblema.Excluído;
                    });
                }

                wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection = null;
                IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                repp.OndeChaveIgual(this._sumarioavaliacaowebselecionado.ID);
                repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoPreAnestesica);
                var ret = repp.List();
                if (ret.IsNotNull())
                    _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);

                bool removesemmed = false;
                foreach (var item in _MedicamentosCollection)
                {
                    if (item.MedicamentosEmUso.Paciente.IsNull())
                    {
                        IRepositorioDeMedicamentoEmUsoProntuario repaa = ObjectFactory.GetInstance<IRepositorioDeMedicamentoEmUsoProntuario>();

                        if (item.MedicamentosEmUso.Status == StatusMedicamentosEmUso.NaoFazUso 
                            && this._paciente.MedicamentosEmUso.Count(x => x.Status != StatusMedicamentosEmUso.NaoFazUso) > 0)
                        {
                            repp.Delete(item.DomainObject);
                            repaa.Delete(item.MedicamentosEmUso.DomainObject);
                        }
                        else
                        {
                            if (item.MedicamentosEmUso.Status == StatusMedicamentosEmUso.Excluído)
                                repp.Delete(item.DomainObject);
                            else
                            {
                                item.MedicamentosEmUso.Paciente = this._paciente;
                                repaa.Save(item.MedicamentosEmUso.DomainObject);
                                this._paciente.MedicamentosEmUso.Add(item.MedicamentosEmUso);
                                if (item.MedicamentosEmUso.Status != StatusMedicamentosEmUso.NaoFazUso)
                                    removesemmed = true;
                            }
                        }
                    }
                }
                if (removesemmed)
                {
                    // Deleta o evento 
                    var medEvento = _MedicamentosCollection.Where(x => x.Chave == this._sumarioavaliacaowebselecionado.ID && x.MedicamentosEmUso.Status == StatusMedicamentosEmUso.NaoFazUso).FirstOrDefault();
                    if (medEvento != null)
                    {
                        var listremove = new wrpMedicamentosEmUsoProntuarioCollection(new List<MedicamentosEmUsoProntuario>());
                        this._paciente.MedicamentosEmUso.Each(x =>
                        {
                            if (x.Status == StatusMedicamentosEmUso.NaoFazUso)
                                listremove.Add(x);
                        });
                        _MedicamentosCollection.Remove(medEvento);
                        medEvento.MedicamentosEmUso.Selecionado = false;
                        IRepositorioDeEventoMedicamentosEmUso rep = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                        rep.Delete(medEvento.DomainObject);
                        listremove.Each(x =>
                        {
                            x.Selecionado = false;
                            x.Status = StatusMedicamentosEmUso.Excluído;
                        });
                    }
                }

                this._sumarioavaliacaowebselecionado.Paciente = this._paciente;
                this._sumarioavaliacaowebselecionado.UsuarioInclusao = this._usuarios;
                this._sumarioavaliacaowebselecionado.PrestadorAnestesia = this._usuarios.Prestador;
                this._sumarioavaliacaowebselecionado.Web = SimNao.Nao;
                this._sumarioavaliacaowebselecionado.Save();

                this._sumarioavaliacaopreanestesica = this._sumarioavaliacaowebselecionado;

                if (this._avisocirurgia.IsNotNull())
                {
                    IRepositorioAvisosDeCirurgia repav = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
                    repav.Refresh(this._avisocirurgia.DomainObject);
                }

                this.SelecionouWeb = true;
            }
        }

        public void ExcluirSumarioWeb()
        {
            if (DXMessageBox.Show("Deseja realmente Excluir este Sumário Pré-Anestésico?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IRepositorioDeSumarioAvaliacaoPreAnestesica repo = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
                var sum = repo.OndeIDIgual(this._sumarioavaliacaowebselecionado.ID).Single();
                sum.DataCancelamento = DateTime.Now;
                repo.Save(sum);
                this._buscasumarioWEB();
            }
        }
    }
}
