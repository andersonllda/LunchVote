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
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Consult;
using HMV.PEP.DTO;
using HMV.PEP.ViewModel.Commands;
using StructureMap;
using HMV.Core.Framework.Expression;
using HMV.PEP.ViewModel.PEP.SumarioDeAlta;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmSumarioAlta : ViewModelBase
    {
        #region Contrutor
        public vmSumarioAlta(Atendimento pAtendimento, Usuarios pUsuario, bool pSalva = true)
        {
            this._usuario = pUsuario;
            this._atendimento = pAtendimento;
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.Refresh(pAtendimento);

            if (pAtendimento.SumarioAlta != null)
            {
                this._sumarioalta = new wrpSumarioAlta(pAtendimento.SumarioAlta);
                if (!this._sumarioalta.DataInclusao.HasValue)
                    this._sumarioalta.DataInclusao = DateTime.Now;
                if (this._sumarioalta.Usuario.IsNull())
                    this._sumarioalta.Usuario = new wrpUsuarios(this._usuario);
                IRepositorioDeSumarioAlta repsum = ObjectFactory.GetInstance<IRepositorioDeSumarioAlta>();
                repsum.Refresh(pAtendimento.SumarioAlta);
            }
            else
            {
                this._sumarioalta = new wrpSumarioAlta(pAtendimento, _usuario);
                this._sumarioalta.SemCausaExterna = Core.Domain.Enum.SimNao.Sim;
                if (!this._sumarioalta.DataInclusao.HasValue)
                    this._sumarioalta.DataInclusao = DateTime.Now;
            }

            if (this._sumarioalta.Transferencia == null)
            {
                wrpMeioDeTransporte MT = this.MeiosDeTransportes.FirstOrDefault();
                this._sumarioalta.Transferencia = new wrpTransferencia(MT.DomainObject);
                this._sumarioalta.Transferencia.MeioDetransporte = MT;
            }

            if (this._sumarioalta.MotivoAlta.IsNotNull() && this._sumarioalta.MotivoAlta.Tipo == TipoMotivoAlta.Transferido)
            {
                this._controlahabilitatransf = true;
                this._habilitaabatransferencia = true;
            }
            
            Parametro _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).OndePodeEditarPEP().Single();

            if ((pUsuario.Prestador.IsNotNull()) && (pUsuario.Prestador.Conselho.IsNotNull()) && (_param.Valor.Contains(pUsuario.Prestador.Conselho.ds_conselho.ToUpper())))
            {
                if (this.IsMedico && this._sumarioalta.DataAlta.IsNull() && this._sumarioalta.UsuarioImpressao.IsNotNull() && this._sumarioalta.UsuarioImpressao.cd_usuario.Equals(pUsuario.cd_usuario))
                {
                    this._sumarioalta.DataImpressao = null;
                    this._sumarioalta.UsuarioImpressao = null;
                    this.MostraAbas = true;
                }
                else if (this._sumarioalta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao)
                    this.MostraAbas = !this._sumarioalta.Atendimento.DataAltaMedica.HasValue && this.IsMedico;
                else
                    this.MostraAbas = this.IsMedico; //this._sumarioalta.Atendimento.MotivoAlta == null &&
            }
            else
                this.MostraAbas = false;         
                

            //Verifica se deve abrir o sumário RN
            //Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeLiberaRNSumarioAlta().Single();
            //if (par.IsNotNull())
           // {
               // try
                //{
                    //if (par.Valor == "S")
                   // {
                        if (this._sumarioalta.Atendimento.AtendimentoPai.IsNotNull())
                            this._isRN = true;
                        else
                        {
                            var repA = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeAtendimento>();
                            var ret = repA.OndeCodigoPacienteIgual(this._sumarioalta.Atendimento.Paciente.ID).List();
                            if (ret.IsNotNull())
                            {
                                var atendmentosanteriores = ret.Where(x => x.DataAlta.IsNotNull() && x.DataAlta.Value.AddDays(1) >= this._sumarioalta.Atendimento.DataAtendimento).ToList();
                                if (atendmentosanteriores.HasItems())
                                    if (atendmentosanteriores.Count(x => x.AtendimentoPai.IsNotNull()) > 0)
                                        this._isRN = true;
                            }
                        }
                        if (_isRN)
                        {
                            this._vmDadosNascimento = new vmDadosNascimento(this._sumarioalta);
                        }
                    //}
               // }
               // catch (Exception err)
                //{
                    //throw new Exception(err.ToString() + " Parametro LIBERA_RN_SUMARIO_ALTA deve ser inteiro e separado por virgula.");
                //}
           // }
            this._vmAltaMedica = new vmAltaMedica(this, pUsuario);
            this.SaveSumarioAltaCommand = new SaveSumarioAltaCommand(this);
            if (pSalva)
                this._sumarioalta.Save();
        }
        #endregion

        #region Propriedades Publicas
        public Atendimento Atendimento
        {
            get
            {
                return _atendimento;
            }
        }
        public wrpMeioDeTransporteCollection MeiosDeTransportes
        {
            get
            {
                IRepositorioDeMeioDeTransporte rep = ObjectFactory.GetInstance<IRepositorioDeMeioDeTransporte>();
                return new wrpMeioDeTransporteCollection(rep.List().OrderBy(x => x.Descricao).ToList());
            }
        }

        public bool IsRN
        {
            get { return _isRN; }
        }

        public bool IsMedico
        {
            get
            {
                return _usuario.Prestador != null && _usuario.Prestador.Conselho != null && _usuario.Prestador.Conselho.isMedico();
            }
        }

        public TabsSumarioAlta AbaSelecionada
        {
            get
            {
                return _abaselecionada;
            }
            set
            {
                this._abaselecionada = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.AbaSelecionada));
            }
        }

        public bool MostraAbas
        {
            get { return _mostraabas; }
            set
            {
                this._mostraabas = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.MostraAbas));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.HabilitaAbaTransferencia));
            }
        }

        public bool VerificaSePodeImprimir
        {
            get
            {
                if ((this._sumarioalta.DataAlta.HasValue && _sumarioalta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao) || this._sumarioalta.Atendimento.MotivoAlta != null)
                    return true;
                DXMessageBox.Show("Deve ser confirmada a Alta Médica antes da impressão.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
        }

        public bool HabilitaAbaTransferencia
        {
            get
            {
                if (!this._mostraabas)
                    return false;
                return _habilitaabatransferencia;
            }
            set
            {
                this._habilitaabatransferencia = value;
                this._controlahabilitatransf = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.HabilitaAbaTransferencia));
            }
        }

        public bool HabilitaTransfRelatorio
        {
            get
            {                
                return _controlahabilitatransf;
            }
        }

        public wrpSumarioAlta SumarioAlta
        {
            get
            {
                return this._sumarioalta;
            }
            set
            {
                this._sumarioalta = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.SumarioAlta));
            }
        }

        public bool JaTemAlta
        {
            get
            {
               // return false;
                if (this._sumarioalta.Atendimento.TipoDeAtendimento.Equals(TipoAtendimento.Internacao))
                    //return  this._sumarioalta.Atendimento.DataAltaMedica.IsNotNull();
                    return this._sumarioalta.DataImpressao.IsNotNull();
                return this._sumarioalta.Atendimento.MotivoAlta.IsNotNull();
               
            }
        }

        public IList<MedicamentoPosAltaDTO> VerificaSeDeveAbrirATelaParaSelecionarOsMedicamentosDaUltimaPrescricao()
        {
            IList<MedicamentoPosAltaDTO> Lista = new List<MedicamentoPosAltaDTO>();

            if (this._sumarioalta.SemMedPosAlta == Core.Domain.Enum.SimNao.Nao)
            {
                if (this.SumarioAlta.PlanoPosAlta.Count != 0)
                    return Lista;

                IPosAltaConsult iConsult = ObjectFactory.GetInstance<IPosAltaConsult>();
                Lista = iConsult.carregaMedicametosUltimaPrescricao(this.SumarioAlta.Atendimento.DomainObject);

                var repA = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeAtendimento>();
                var ret = repA.OndeCodigoPacienteIgual(this.SumarioAlta.Atendimento.Paciente.ID).List();
                if (ret.IsNotNull())
                {
                    var atendmentosanteriores = ret.Where(x => x.DataAlta >= DateTime.Now.AddDays(-1)).ToList();
                    if (atendmentosanteriores.HasItems())
                        if (atendmentosanteriores.Count(x => x.AtendimentoPai.IsNotNull()) > 0)
                        {
                            iConsult = ObjectFactory.GetInstance<IPosAltaConsult>();
                            Lista = iConsult.carregaMedicametosUltimaPrescricao(atendmentosanteriores.LastOrDefault());
                        }
                }
            }
            return Lista;
        }

        public IList<Tab> Tabs
        {
            get
            {
                IList<Tab> abas = new List<Tab>();
                Tab aba = new Tab { TipoTab = TabsSumarioAlta.AltaMedica, Descricao = "Alta Médica", Componente = "UserControls\\SumarioDeAlta\\ucAltaMedica.xaml", Index = 0 };
                abas.Add(aba);

                if (this._sumarioalta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao && !this._isRN)
                {
                    aba = new Tab { TipoTab = TabsSumarioAlta.CausaExterna, Descricao = "Causa Externa", Componente = "UserControls\\SumarioDeAlta\\ucCausaExterna.xaml", Index = 1 };
                    abas.Add(aba);
                }

                if (this._isRN)
                {
                    aba = new Tab { TipoTab = TabsSumarioAlta.CausaExterna, Descricao = "Dados do Nascimento", Componente = "UserControls\\SumarioDeAlta\\ucDadosNascimento.xaml", Index = 1 };
                    abas.Add(aba);
                }

                aba = new Tab { TipoTab = TabsSumarioAlta.Procedimentos, Descricao = "Procedimentos", Componente = "UserControls\\SumarioDeAlta\\ucProcedimentos.xaml", Index = 2 };
                abas.Add(aba);
                if (this._sumarioalta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao)
                {
                    aba = new Tab { TipoTab = TabsSumarioAlta.Farmacos, Descricao = "Fármacos", Componente = "UserControls\\SumarioDeAlta\\ucFarmacos.xaml", Index = 3 };
                    abas.Add(aba);
                }
                aba = new Tab { TipoTab = TabsSumarioAlta.Evolucao, Descricao = "Evolução", Componente = "UserControls\\SumarioDeAlta\\ucEvolucao.xaml", Index = 4 };
                abas.Add(aba);
                if (this._sumarioalta.Atendimento.TipoDeAtendimento == TipoAtendimento.Internacao)
                {
                    aba = new Tab { TipoTab = TabsSumarioAlta.Exames, Descricao = "Exames", Componente = "UserControls\\SumarioDeAlta\\ucExames.xaml", Index = 5 };
                    abas.Add(aba);
                }
                aba = new Tab { TipoTab = TabsSumarioAlta.PosAlta, Descricao = "Pós Alta", Componente = "UserControls\\SumarioDeAlta\\ucPosAlta.xaml", Index = 6 };
                abas.Add(aba);
                aba = new Tab { TipoTab = TabsSumarioAlta.Recomendacoes, Descricao = "Recomendações", Componente = "UserControls\\SumarioDeAlta\\ucRecomendacao.xaml", Index = 7 };
                abas.Add(aba);
                aba = new Tab { TipoTab = TabsSumarioAlta.Medicos, Descricao = "Médicos", Componente = "UserControls\\SumarioDeAlta\\ucMedicos.xaml", Index = 8 };
                abas.Add(aba);
                aba = new Tab { TipoTab = TabsSumarioAlta.Transferencia, Descricao = "Transferência", Componente = "UserControls\\SumarioDeAlta\\ucTransferenciaPaciente.xaml", Index = 9 };
                abas.Add(aba);
                aba = new Tab { TipoTab = TabsSumarioAlta.Concluir, Descricao = "Concluir", Componente = "UserControls\\SumarioDeAlta\\ucResumoAltaMedica.xaml", Index = 10 };
                abas.Add(aba);
                return abas;
            }
        }

        public vmAltaMedica vmAltaMedica
        {
            get { return _vmAltaMedica; }
            set
            {
                _vmAltaMedica = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.SumarioAlta));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.vmAltaMedica));
            }
        }

        public vmDadosNascimento vmDadosNascimento
        {
            get { return _vmDadosNascimento; }
            set
            {
                _vmDadosNascimento = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.SumarioAlta));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAlta>(x => x.vmDadosNascimento));
            }
        }

        public vmListaProblemaSumario vmListaProblemaSumario
        {
            get
            {
                if (this._vmListaProblemaSumario.IsNull())
                    this._vmListaProblemaSumario = new vmListaProblemaSumario(this._sumarioalta.Atendimento);

                return _vmListaProblemaSumario;
            }
        }
        #endregion

        #region Metodos Privados VAZIO

        #endregion

        #region Propriedades Privadas
        private TabsSumarioAlta _abaselecionada;
        private Usuarios _usuario;
        private wrpSumarioAlta _sumarioalta;
        private bool _habilitaabatransferencia;
        private bool _controlahabilitatransf; // GAMBIARRA pra controlar a aba de transferencia quando o sumario já está fechado... foi criado isso pq tá uma confusão esse código e como sempre nao tem tempo de refatorar!
        private bool _mostraabas;
        private vmAltaMedica _vmAltaMedica;
        private bool _isRN;
        private vmDadosNascimento _vmDadosNascimento;
        private vmListaProblemaSumario _vmListaProblemaSumario;
        private Atendimento _atendimento;
        #endregion

        #region Metodos Publicos
        public void setaDadosDeImpressao()
        {
            if (!this._sumarioalta.DataImpressao.HasValue)
            {
                if (this._usuario.Prestador.IsNurse)
                    this._sumarioalta.UsuarioImpressao = this._sumarioalta.UsuarioInclusao;
                else
                    this._sumarioalta.UsuarioImpressao = new wrpUsuarios(_usuario);

                this._sumarioalta.DataImpressao = DateTime.Now;

                this._sumarioalta.Save();
            }
        }
        
        public void Save()
        {
            this._sumarioalta.Save();
            if (this._vmDadosNascimento.IsNotNull())
                this._vmDadosNascimento.DadosNascimento.Save();
            if (this._vmListaProblemaSumario.IsNotNull())
                this._vmListaProblemaSumario.Save();

        }
        #endregion

        #region Commands
        public ICommand SaveSumarioAltaCommand { get; set; }
        #endregion

        public class Tab
        {
            public TabsSumarioAlta TipoTab { get; set; }
            public int Index { get; set; }
            public string Descricao { get; set; }
            public string Componente { get; set; }
        }

        public enum TabsSumarioAlta
        {
            AltaMedica,
            CausaExterna,
            Procedimentos,
            Farmacos,
            Evolucao,
            Exames,
            PosAlta,
            Recomendacoes,
            Medicos,
            Transferencia,
            Concluir
        }        
    }
}
