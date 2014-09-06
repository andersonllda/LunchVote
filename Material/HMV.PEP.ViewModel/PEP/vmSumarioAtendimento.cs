using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Consult;
using HMV.PEP.DTO;
using StructureMap;
using HMV.PEP.ViewModel.PEP;
using System;
using HMV.Core.Domain.Repository.PEP;
using HMV.Core.Domain.Repository.PEP.Evolucao;

namespace HMV.PEP.ViewModel.SumarioDeAtendimento
{
    public class vmSumarioAtendimento : ViewModelBase
    {
        #region Contrutor
        public vmSumarioAtendimento(Paciente pPaciente, TipoAtendimentoSumario pTipoAtendimentoSumario, bool pBoolControlaAtendimento, bool pAtendimentoComAltas, Usuarios pUsuario, Atendimento pAtendimentoPEP)
        {
            this.AtendimentoPEP = pAtendimentoPEP;
            this.BoolControlaAtendimento = pBoolControlaAtendimento;
            this.AtendimentoComAltas = pAtendimentoComAltas;
            this._paciente = pPaciente;
            this._usuarios = pUsuario;

            IRepositorioDeOrigemAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeOrigemAtendimento>();
            this._origematendimentos = new wrpOrigemAtendimentoCollection(rep.List().Where(x => x.Ativo == SimNao.Sim).ToList());
            this._origematendimentos.Add(new wrpOrigemAtendimento());
            this._origematendimentos.Sort(x => x.Descricao);

            this._vmprocedimentosRealizados = new vmProcedimentosRealizados(pPaciente, true, true);

            this._tipoatendimentoselecionado = pTipoAtendimentoSumario;

            IRepositorioDeSumariosAvaliacaoMedicaFechados repSumarios = ObjectFactory.GetInstance<IRepositorioDeSumariosAvaliacaoMedicaFechados>();
            var ts = (repSumarios.OndeTipoSumarioAmbulatorios().OndePacienteIgual(pPaciente).List());          
        }

        public vmSumarioAtendimento(SumarioDeAtendimentosDTO pSumarioDeAtendimentosDTO)
        {
            this._sumarioatendimentoselecionado = pSumarioDeAtendimentosDTO;
        }
        #endregion

        #region Propriedades Publicas
        public IList<SumarioDeAtendimentosDTO> SumarioAtendimentos
        {
            get
            {
                ISumarioDeAtendimentosConsult consult = ObjectFactory.GetInstance<ISumarioDeAtendimentosConsult>();
                if (this.AtendimentoComAltas)
                {
                    var qry = consult.carregaSumarioDeAtendimentos(
                        this._paciente,
                        this._tipoatendimentoselecionado,
                        this._servicoselecionado == null ? null : this._servicoselecionado.ID == 0 ? null : this._servicoselecionado.DomainObject).Where(x => !x.DataAlta.IsNull()).ToList();
                    return (from T in qry
                          orderby  T.DataAtendimento descending, T.IdAtendimento descending
                        select T).ToList();
                }

                var qry1 = consult.carregaSumarioDeAtendimentos(
                    this._paciente, 
                    this._tipoatendimentoselecionado, 
                    this._servicoselecionado == null ? null : this._servicoselecionado.ID == 0 ? null : this._servicoselecionado.DomainObject).ToList();
                return (from T in qry1
                          orderby  T.DataAtendimento descending, T.IdAtendimento descending
                        select T).ToList();
            }
        }

        public wrpOrigemAtendimentoCollection Servicos
        {
            get
            {
                return _origematendimentos;
            }
        }

        public Paciente Paciente
        {
            get { return this._paciente; }
        }

        public TipoAtendimentoSumario TipoAtendimentoSelecionado
        {
            get
            {
                return this._tipoatendimentoselecionado;
            }
            set
            {
                this._tipoatendimentoselecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.TipoAtendimentoSelecionado));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.SumarioAtendimentos));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.SumarioAtendimentoSelecionado));

            }
        }

        public bool VisibleButtonGED
        {
            get
            {
                bool temGED = false;

                IRepositorioDeProntuario repProntuario = ObjectFactory.GetInstance<IRepositorioDeProntuario>();
                if (this._sumarioatendimentoselecionado.IsNotNull() && this._sumarioatendimentoselecionado.IdAtendimento.HasValue)
                    temGED = repProntuario.VerificaSeDeveHabilitarOpcaoGED(this._sumarioatendimentoselecionado.IdAtendimento.Value, 0);

                if (temGED)
                {
                    Parametro par = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndePodeMostrarGED().Single();
                    if (par.IsNotNull() && par.Valor.Split(',').Contains(this._usuarios.cd_usuario.ToString()))
                        return true;
                    else if (this.AtendimentoPEP.IsNotNull())
                        return true;

                }
                //else if (this._sumarioatendimentoselecionado.IsNotNull())
                //    return true;

                return false;
            }
        }
        public bool VisibleButtonEmergencia
        {
            get
            {
                if (_sumarioatendimentoselecionado.IsNotNull() && _sumarioatendimentoselecionado.MostraBoletim == "S")
                    return true;
                return false;
            }
        }
        public bool VisibleButtonDescricaoCirurgica
        {
            get
            {
                if (_sumarioatendimentoselecionado.IsNotNull() && _sumarioatendimentoselecionado.MostraDescricaoCirurgica == "S")
                    return true;
                return false;
            }
        }
        public bool VisibleButtonAvaliacaoMedica
        {
            get
            {
                if (_sumarioatendimentoselecionado.IsNotNull() && _sumarioatendimentoselecionado.MostraAvaliacaoMedica == "S")
                    return true;
                return false;
            }
        }
        public bool VisibleButtonAltaMedica
        {
            get
            {
                if (_sumarioatendimentoselecionado.IsNotNull() && _sumarioatendimentoselecionado.MostraAltaMedica == "S")
                    return true;
                return false;
            }
        }
        public bool VisibleButtonSumarioAmbulatorial
        {
            get
            {
                if (!this.SumarioAtendimentoSelecionado.IsNull())
                    return (this.SumarioAtendimentoSelecionado.Seq_Atendimento > 0) && (this.SumarioAtendimentoSelecionado.IDSumAvalMed > 0);

                return false;
            }
        }
        public bool VisibleButtonPreAnestesica
        {
            get
            {
                return this._visiblebuttonpreanestesica;
            }
        }
        public bool VisibleButtonEvolucoes
        {
            get
            {
                return this._visiblebuttonEvolucoes;
            }
        }

        public bool BoolControlaAtendimento { get; set; } //controle para a tela winSelAtendimento, onde precisa selecionar somente o atendimento
        
        public Atendimento AtendimentoPEP { get; set; } // Controle para saber se na tela principal do PEP existe atendimento Selecionado

        public SumarioDeAtendimentosDTO SumarioAtendimentoSelecionado
        {
            get
            {
                return this._sumarioatendimentoselecionado;
            }
            set
            {
                this._sumarioatendimentoselecionado = value;
                if (!BoolControlaAtendimento)
                {                   
                    this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonSumarioAmbulatorial));
                    this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.DescricoesCirurgicas));
                }           

                //Controla Botao PreAnestesica
                this._visiblebuttonpreanestesica = false;
                this._sumariopreanestesica = null;
                this._sumarioavaliacaopreanestesicacollection = null;
                if (this._sumarioatendimentoselecionado.IsNotNull() && this._sumarioatendimentoselecionado.IdAtendimento.HasValue)
                {
                    IRepositorioDeSumarioAvaliacaoPreAnestesica rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();                    
                    this._sumarioavaliacaopreanestesicacollection = new wrpSumarioAvaliacaoPreAnestesicaCollection(rep.OndeIdAtendimentoAviso(
                        this._sumarioatendimentoselecionado.IdAtendimento.Value).List().Where(x=> x.DataEmissao.IsNotNull()).ToList());

                    if (this._sumarioavaliacaopreanestesicacollection.HasItems())
                    {
                        this._visiblebuttonpreanestesica = true;
                        this._sumariopreanestesica = this._sumarioavaliacaopreanestesicacollection.FirstOrDefault();
                    }
                }

                //Controla Botao Evolucoes
                this._visiblebuttonEvolucoes = false;
                IRepositorioDePEPEvolucao repE = ObjectFactory.GetInstance<IRepositorioDePEPEvolucao>();
                repE.OndeAtendimento(this._sumarioatendimentoselecionado.IdAtendimento.Value);
                var ret = repE.List();
                if (ret.HasItems())
                    this._visiblebuttonEvolucoes = true;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonAvaliacaoMedica));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonAltaMedica));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonEmergencia));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonDescricaoCirurgica));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonGED));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonPreAnestesica));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.VisibleButtonEvolucoes));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.SumarioAtendimentoSelecionado));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.SumarioPreAnestesicoSelecionado));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.SumarioAvaliacaoPreAnestesicaCollection));
            }
        }

        public wrpSumarioAvaliacaoPreAnestesica SumarioPreAnestesicoSelecionado
        {
            get
            {
                return _sumariopreanestesica;
            }
            set
            {
                this._sumariopreanestesica = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.SumarioPreAnestesicoSelecionado));
            }
        }

        public wrpSumarioAvaliacaoPreAnestesicaCollection SumarioAvaliacaoPreAnestesicaCollection
        {
            get
            {
                return _sumarioavaliacaopreanestesicacollection;
            }
        }

        public Atendimento Atendimento
        {
            get
            {
                if (SumarioAtendimentoSelecionado == null) return null;

                IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
                return rep.OndeCodigoAtendimentoIgual(SumarioAtendimentoSelecionado.IdAtendimento).Single();
            }
        }

        public wrpOrigemAtendimento ServicoSelecionado
        {
            get
            {
                return this._servicoselecionado;
            }
            set
            {
                this._servicoselecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.ServicoSelecionado));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.SumarioAtendimentos));
            }
        }
        
        
        public IList<AvisoCirurgia> DescricoesCirurgicas
        {
            get
            {
                IRepositorioAvisosDeCirurgia rep = ObjectFactory.GetInstance<IRepositorioAvisosDeCirurgia>();
                return rep.OndeCodigoAtendimentoIgual(_sumarioatendimentoselecionado.IdAtendimento).List().OrderByDescending(x => x.DataAviso).ToList();
            }
        }

        public AvisoCirurgia DescricaoCirurgicaSelecionada
        {
            get
            {
                if (this._DescricaoCirurgica == null)
                    if (DescricoesCirurgicas.Count() == 0)
                        return null;
                    else
                        return DescricoesCirurgicas[0];
                return this._DescricaoCirurgica;
            }
            set
            {
                this._DescricaoCirurgica = value;
            }
        }

        public IList<ProcedimentosRealizadosDTO> ProcedimentosRealizados
        {
            get
            {
                return this._vmprocedimentosRealizados.ProcedimentosRealizados;
            }
        }

        public ProcedimentosRealizadosDTO ProcedimentosRealizadoSelecionado
        {
            get
            {
                return this._procedimentosrealizadoselecionado;
            }
            set
            {
                this._procedimentosrealizadoselecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAtendimento>(x => x.ProcedimentosRealizadoSelecionado));
            }
        }


        public vmProcedimentosRealizados vmProcedimentosRealizados
        {
            get { return _vmprocedimentosRealizados; }
        }

        #endregion

        #region Metodos Privados

        #endregion

        #region Propriedades Privadas
        private wrpOrigemAtendimentoCollection _origematendimentos { get; set; }
        private TipoAtendimentoSumario _tipoatendimentoselecionado { get; set; }
        private SumarioDeAtendimentosDTO _sumarioatendimentoselecionado { get; set; }
        private wrpOrigemAtendimento _servicoselecionado { get; set; }
        private Paciente _paciente { get; set; }
        private ProcedimentosRealizadosDTO _procedimentosrealizadoselecionado { get; set; }
        private AvisoCirurgia _DescricaoCirurgica { get; set; }
        private IList<AvisoCirurgia> _DescricoesCirurgicas { get; set; }
        private bool AtendimentoComAltas;
        private Usuarios _usuarios;
        private bool _visiblebuttonpreanestesica;
        private bool _visiblebuttonEvolucoes;
        private wrpSumarioAvaliacaoPreAnestesicaCollection _sumarioavaliacaopreanestesicacollection;
        private wrpSumarioAvaliacaoPreAnestesica _sumariopreanestesica;
        private vmProcedimentosRealizados _vmprocedimentosRealizados;
        #endregion

        #region Metodos Publicos

        #endregion

        #region Commands
        
        #endregion

        #region SUBRELATORIO DESCRICOES
        public class DescricoesCirurg
        {
            public string Descricao { get; set; }
        }

        public List<DescricoesCirurg> RelDescricoes
        {
            get
            {
                List<DescricoesCirurg> qry = new List<DescricoesCirurg>();

                if (this._DescricaoCirurgica != null && this._DescricaoCirurgica.DescricaoCirurgia != null)
                {
                    var listaDescr = this._DescricaoCirurgica.DescricaoCirurgia.TrimEnd(Environment.NewLine.ToCharArray()).Split(new char[] { '\n' }).ToList();

                    for (int i = 0; i < listaDescr.Count(); i++)
                    {
                        DescricoesCirurg dc = new DescricoesCirurg();
                        if (!string.IsNullOrEmpty(listaDescr[i].ToString()))
                        {
                            dc.Descricao = listaDescr[i].ToString();
                            qry.Add(dc);
                        }
                    }
                }
                return qry;
            }
        }


        #endregion
               
    }
}
