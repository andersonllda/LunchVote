using System;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.ProcessosEnfermagem.ViewModel;
using StructureMap;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.ViewModel.PEP.CheckListDeUDI
{
    public class vmAntesDaEntrada : ViewModelBase
    {
        #region ----- Construtor -----
        public vmAntesDaEntrada(vmCheckListUDI pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            bool _novo = false;

            if (this._checklist.AntesEntradaUDI.IsNull())
            {
                this._AntesEntradaPaciente = new wrpAntesEntradaUDI(pvmCheckList.Usuario);
                _novo = true;
            }
            else
            {
                this._AntesEntradaPaciente = this._checklist.AntesEntradaUDI;
                if (this._AntesEntradaPaciente.Responsavel.IsEmpty() && this._AntesEntradaPaciente.Prontuario.IsEmpty())
                    this._boolPaciente = true;
                else if (this._AntesEntradaPaciente.Responsavel.IsNotEmpty())
                    this._boolResponsavel = true;
                else if (this._AntesEntradaPaciente.Prontuario.IsNotEmpty())
                    this._boolProntuario = true;
            }

            this._paciente = new wrpPaciente(this._vmchecklist.Paciente.DomainObject);
            this._paciente.DomainObject.Refresh();

            // Salva checkList para gerar um id
            _vmchecklist.Salvar();

            wrpAlergiaEventoCollection _AlergiaCollection = null;

            IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
            repa.OndeChaveIgual(_vmchecklist.CheckListdto.CheckList.ID);
            repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.CheckListUDI);
            var reta = repa.List();
            if (reta.IsNotNull())
                _AlergiaCollection = new wrpAlergiaEventoCollection(reta);

            this._vmalergiasevento = new vmAlergiasEvento(true, this._paciente, this._vmchecklist.Usuario, this._vmchecklist.Usuario.Prestador.IsCorpoClinico
                , TipoEvento.CheckListUDI, _AlergiaCollection
                , _vmchecklist.CheckListdto.CheckList.ID, this._checklist.Atendimento);

            if (_novo)
                this._vmalergiasevento.MarcarTodasAlergias();
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckListUDI _vmchecklist;
        private bool _boolResponsavel;
        private bool _boolPaciente;
        private bool _boolProntuario;
        private wrpCheckListUDI _checklist;
        private wrpAntesEntradaUDI _AntesEntradaPaciente;
        private vmAlergiasEvento _vmalergiasevento;
        private wrpPaciente _paciente;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListUDI CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.CheckList));
            }
        }

        public wrpAntesEntradaUDI AntesEntradaPaciente
        {
            get { return _AntesEntradaPaciente; }
            set
            {
                this._AntesEntradaPaciente = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AntesEntradaPaciente));
            }
        }


        public bool ConsentimentoContraste
        {
            get
            {
                return this._AntesEntradaPaciente.ConsentimentoContraste == SimNA.Sim;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ConsentimentoContraste = SimNA.Sim;
                else
                    this._AntesEntradaPaciente.ConsentimentoContraste = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoContraste));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoContrasteNA));
            }
        }
        public bool ConsentimentoContrasteNA
        {
            get
            {
                return this._AntesEntradaPaciente.ConsentimentoContraste == SimNA.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ConsentimentoContraste = SimNA.NA;
                else
                    this._AntesEntradaPaciente.ConsentimentoContraste = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoContraste));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoContrasteNA));
            }
        }

        public bool PesquisaAlergico
        {
            get
            {
                return this._AntesEntradaPaciente.PesquisaAlergico == SimNA.Sim;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.PesquisaAlergico = SimNA.Sim;
                else
                    this._AntesEntradaPaciente.PesquisaAlergico = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PesquisaAlergico));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PesquisaAlergicoNA));
            }
        }
        public bool PesquisaAlergicoNA
        {
            get
            {
                return this._AntesEntradaPaciente.PesquisaAlergico == SimNA.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.PesquisaAlergico = SimNA.NA;
                else
                    this._AntesEntradaPaciente.PesquisaAlergico = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PesquisaAlergico));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PesquisaAlergicoNA));
            }
        }

        public bool ConsentimentoProcedimento
        {
            get
            {
                return this._AntesEntradaPaciente.ConsentimentoProcedimento == SimNA.Sim;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ConsentimentoProcedimento = SimNA.Sim;
                else
                    this._AntesEntradaPaciente.ConsentimentoProcedimento = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoProcedimento));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoProcedimentoNA));
            }
        }
        public bool ConsentimentoProcedimentoNA
        {
            get
            {
                return this._AntesEntradaPaciente.ConsentimentoProcedimento == SimNA.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ConsentimentoProcedimento = SimNA.NA;
                else
                    this._AntesEntradaPaciente.ConsentimentoProcedimento = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoProcedimento));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoProcedimentoNA));
            }
        }

        public bool AvaliacaoPreAnestesica
        {
            get
            {
                return this._AntesEntradaPaciente.AvaliacaoPreAnestesica == SimNA.Sim;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.AvaliacaoPreAnestesica = SimNA.Sim;
                else
                    this._AntesEntradaPaciente.AvaliacaoPreAnestesica = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AvaliacaoPreAnestesica));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AvaliacaoPreAnestesicaNA));
            }
        }
        public bool AvaliacaoPreAnestesicaNA
        {
            get
            {
                return this._AntesEntradaPaciente.AvaliacaoPreAnestesica == SimNA.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.AvaliacaoPreAnestesica = SimNA.NA;
                else
                    this._AntesEntradaPaciente.AvaliacaoPreAnestesica = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AvaliacaoPreAnestesica));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AvaliacaoPreAnestesicaNA));
            }
        }


        public bool ConsentimentoAnestesia
        {
            get
            {
                return this._AntesEntradaPaciente.ConsentimentoAnestesia == SimNA.Sim;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ConsentimentoAnestesia = SimNA.Sim;
                else
                    this._AntesEntradaPaciente.ConsentimentoAnestesia = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoAnestesia));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoAnestesiaNA));
            }
        }
        public bool ConsentimentoAnestesiaNA
        {
            get
            {
                return this._AntesEntradaPaciente.ConsentimentoAnestesia == SimNA.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ConsentimentoAnestesia = SimNA.NA;
                else
                    this._AntesEntradaPaciente.ConsentimentoAnestesia = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoAnestesia));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ConsentimentoAnestesiaNA));
            }
        }

        public bool PuncaoVenosaMSD
        {
            get
            {
                return this._AntesEntradaPaciente.PuncaoVenosa == PuncaoVenosa.MSD;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.PuncaoVenosa = PuncaoVenosa.MSD;
                else
                    this._AntesEntradaPaciente.PuncaoVenosa = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaMSD));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaMSE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaNA));
            }
        }
        public bool PuncaoVenosaMSE
        {
            get
            {
                return this._AntesEntradaPaciente.PuncaoVenosa == PuncaoVenosa.MSE;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.PuncaoVenosa = PuncaoVenosa.MSE;
                else
                    this._AntesEntradaPaciente.PuncaoVenosa = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaMSD));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaMSE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaNA));
            }
        }
        public bool PuncaoVenosaNA
        {
            get
            {
                return this._AntesEntradaPaciente.PuncaoVenosa == PuncaoVenosa.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.PuncaoVenosa = PuncaoVenosa.NA;
                else
                    this._AntesEntradaPaciente.PuncaoVenosa = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaMSD));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaMSE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.PuncaoVenosaNA));
            }
        }

        public bool ExamesAnteriores
        {
            get
            {
                return this._AntesEntradaPaciente.ExamesAnteriores == SimNA.Sim;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ExamesAnteriores = SimNA.Sim;
                else
                    this._AntesEntradaPaciente.ExamesAnteriores = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ExamesAnteriores));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ExamesAnterioresNA));
            }
        }
        public bool ExamesAnterioresNA
        {
            get
            {
                return this._AntesEntradaPaciente.ExamesAnteriores == SimNA.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.ExamesAnteriores = SimNA.NA;
                else
                    this._AntesEntradaPaciente.ExamesAnteriores = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ExamesAnteriores));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ExamesAnterioresNA));
            }
        }

        public bool NPO
        {
            get
            {
                return this._AntesEntradaPaciente.NPO == SimNA.Sim;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.NPO = SimNA.Sim;
                else
                    this._AntesEntradaPaciente.NPO = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NPO));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NPONA));
            }
        }
        public bool NPONA
        {
            get
            {
                return this._AntesEntradaPaciente.NPO == SimNA.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.NPO = SimNA.NA;
                else
                    this._AntesEntradaPaciente.NPO = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NPONA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NPO));
            }
        }

        public bool Direito
        {
            get
            {
                return this._AntesEntradaPaciente.Lado == Posicao.Direito;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Lado = Posicao.Direito;
                else
                    this._AntesEntradaPaciente.Lado = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Ambos));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Esquerdo));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Direito));
            }
        }

        public bool Esquerdo
        {
            get
            {
                return this._AntesEntradaPaciente.Lado == Posicao.Esquerdo;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Lado = Posicao.Esquerdo;
                else
                    this._AntesEntradaPaciente.Lado = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Ambos));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Esquerdo));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Direito));
            }
        }

        public bool Ambos
        {
            get
            {
                return this._AntesEntradaPaciente.Lado == Posicao.Ambos;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Lado = Posicao.Ambos;
                else
                    this._AntesEntradaPaciente.Lado = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Ambos));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Esquerdo));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Direito));
            }
        }

        public bool NA
        {
            get
            {
                return this._AntesEntradaPaciente.Lado == Posicao.NA;
            }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Lado = Posicao.NA;
                else
                    this._AntesEntradaPaciente.Lado = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Ambos));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Esquerdo));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Direito));
            }
        }

        //public bool ProcedimentoNA
        //{
        //    get
        //    {
        //        return this._AntesEntradaPaciente.ProcedimentoNA == SimNao.Sim;
        //    }
        //    set
        //    {
        //        if (value)
        //        {
        //            this._AntesEntradaPaciente.ProcedimentoNA = SimNao.Sim;
        //            this._AntesEntradaPaciente.Lado = null;
        //            this._AntesEntradaPaciente.Regiao = string.Empty;
        //        }
        //        else
        //            this._AntesEntradaPaciente.ProcedimentoNA = SimNao.Nao;


        //        this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.NA));
        //        this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Ambos));
        //        this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Esquerdo));
        //        this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Direito));
        //        this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.ProcedimentoNA));
        //        this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Regiao));
        //    }
        //}

        public string Regiao
        {
            get
            {
                return this._AntesEntradaPaciente.Regiao;
            }

            set 
            {
                this._AntesEntradaPaciente.Regiao = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.Regiao));
            }
        }

        public bool boolPaciente
        {
            get { return this._boolPaciente; }
            set
            {
                this._boolPaciente = value;
                if (this._boolPaciente)
                    this._AntesEntradaPaciente.ConfirmadoPor = SimNao.Sim;

                this._AntesEntradaPaciente.Responsavel = string.Empty;
                this._AntesEntradaPaciente.Prontuario = string.Empty;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AntesEntradaPaciente));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolPaciente));
            }
        }

        public bool boolResponsavel
        {
            get { return this._boolResponsavel; }
            set
            {
                this._boolResponsavel = value;
                if (this._boolResponsavel)
                    this._AntesEntradaPaciente.ConfirmadoPor = SimNao.Nao;

                this._AntesEntradaPaciente.Responsavel = string.Empty;
                this._AntesEntradaPaciente.Prontuario = string.Empty;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AntesEntradaPaciente));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolResponsavel));
            }
        }

        public bool boolProntuario
        {
            get { return this._boolProntuario; }
            set
            {
                this._boolProntuario = value;
                if (_boolProntuario)
                    this._AntesEntradaPaciente.ConfirmadoPor = SimNao.Nao;

                this._AntesEntradaPaciente.Responsavel = string.Empty;
                this._AntesEntradaPaciente.Prontuario = string.Empty;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AntesEntradaPaciente));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolProntuario));
            }
        }

        public bool boolMSD
        {
            get { return this._AntesEntradaPaciente.Pulseira.IsNotNull() && this._AntesEntradaPaciente.Pulseira.Value.Equals(Pulseira.MSD); }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Pulseira = Pulseira.MSD;
                else
                    this._AntesEntradaPaciente.Pulseira = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMID));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMIE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSD));
            }
        }

        public bool boolMSE
        {
            get { return this._AntesEntradaPaciente.Pulseira.IsNotNull() && this._AntesEntradaPaciente.Pulseira.Value.Equals(Pulseira.MSE); }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Pulseira = Pulseira.MSE;
                else
                    this._AntesEntradaPaciente.Pulseira = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSD));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMID));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMIE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSE));

            }
        }

        public bool boolMID
        {
            get { return this._AntesEntradaPaciente.Pulseira.IsNotNull() && this._AntesEntradaPaciente.Pulseira.Value.Equals(Pulseira.MID); }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Pulseira = Pulseira.MID;
                else
                    this._AntesEntradaPaciente.Pulseira = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSD));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMIE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMID));
            }
        }

        public bool boolMIE
        {
            get { return this._AntesEntradaPaciente.Pulseira.IsNotNull() && this._AntesEntradaPaciente.Pulseira.Value.Equals(Pulseira.MIE); }
            set
            {
                if (value)
                    this._AntesEntradaPaciente.Pulseira = Pulseira.MIE;
                else
                    this._AntesEntradaPaciente.Pulseira = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSD));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMSE));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMID));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.boolMIE));
            }
        }

        public vmAlergiasEvento vmAlergias
        {
            get
            {
                if (_vmchecklist.CheckListdto.CheckList.IsNull())
                    return null;
                return this._vmalergiasevento;
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        public void SalvaAlergias()
        {
            this._paciente.Save();
            this._vmalergiasevento.SalvarNovo();
        }
        #endregion

        #region ----- Commands -----

        #endregion

        public override bool IsValid
        {
            get
            {
                IList<string> erros = new List<string>();
                if (!this._boolPaciente && !this._boolResponsavel && !this._boolProntuario)
                    erros.Add("marque um dos campos Confirmado por 'Paciente', 'Responsável' ou 'Prontuário'!");
                else if (this._boolResponsavel && this._AntesEntradaPaciente.Responsavel.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o nome do responsável");
                else if (this._boolProntuario && this._AntesEntradaPaciente.Prontuario.IsEmptyOrWhiteSpace())
                    erros.Add("Informe a observação do prontuário");

                if (this._vmalergiasevento.AlergiaCollection.Count(x => x.Selecionado && x.Status != StatusAlergiaProblema.Excluído) == 0)
                    erros.Add("Alergia deve ser informada ou marcar sem alergias conhecidas.");

                if (!this._AntesEntradaPaciente.Paciente.Equals(SimNao.Sim))
                    erros.Add("Campo 'Nome' é obrigatório!");

                if (!this._AntesEntradaPaciente.Procedimento.Equals(SimNao.Sim))
                    erros.Add("Campo 'Procedimento' é obrigatório!");

                if (this._vmalergiasevento.IsNotNull() && !this._vmalergiasevento.SemAlergiasConhecidas && this._vmalergiasevento.AlergiaCollection.HasItems()
                    && this._vmalergiasevento.AlergiaCollection.Count(x => x.Selecionado) == 0)
                    erros.Add("Campo 'Alergias' é obrigatório!");

                //if ((this._AntesEntradaPaciente.ProcedimentoNA.IsNull() || this._AntesEntradaPaciente.ProcedimentoNA == SimNao.Nao)
                //    && this._AntesEntradaPaciente.Regiao.IsEmptyOrWhiteSpace())
                //    erros.Add("Informe 'Procedimento' ou 'Região'");
                if (this._AntesEntradaPaciente.Regiao.IsEmptyOrWhiteSpace())
                    erros.Add("Campo 'Região' é obrigatório");

                if (this._AntesEntradaPaciente.Regiao.IsNotEmptyOrWhiteSpace())
                    if (this._AntesEntradaPaciente.Lado.IsNull())
                        erros.Add("Campo 'Lado' deve ser informado!");

                if (this._AntesEntradaPaciente.NPO.IsNull())
                    erros.Add("Campo 'NPO' deve ser informado!");

                if (this._AntesEntradaPaciente.ExamesAnteriores.IsNull())
                    erros.Add("Campo 'Exames Anteriores' deve ser informado!");

                if (this._AntesEntradaPaciente.Pulseira.IsNull())
                    erros.Add("Campo 'Pulseira' deve ser informado!");

                if (this._AntesEntradaPaciente.PuncaoVenosa.IsNull())
                    erros.Add("Campo 'Punção Venosa' deve ser informado!");

                if (this._AntesEntradaPaciente.ConsentimentoAnestesia.IsNull())
                    erros.Add("Campo 'Consentimento de Anestesia' deve ser informado!");

                if (this._AntesEntradaPaciente.AvaliacaoPreAnestesica.IsNull())
                    erros.Add("Campo 'Avaliação Pré-Anestésica' deve ser informado!");

                if (this._AntesEntradaPaciente.ConsentimentoProcedimento.IsNull())
                    erros.Add("Campo 'Consentimento Procedimento' deve ser informado!");

                if (this._AntesEntradaPaciente.PesquisaAlergico.IsNull())
                    erros.Add("Campo 'Pesquisa de Pacientes Alérgicos' deve ser informado!");

                if (this._AntesEntradaPaciente.ConsentimentoContraste.IsNull())
                    erros.Add("Campo 'Consentimento Contraste Iodado' deve ser informado!");

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._checklist.AntesEntradaUDI = this._AntesEntradaPaciente;
                this._checklist.AntesEntradaUDI.DataEncerramento = DateTime.Now;
                SalvaAlergias();

                return true;
            }
        }
    }
}
