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

namespace HMV.PEP.ViewModel.PEP.CheckListDeCirurgia
{
    public class vmAntesDaEntrada : ViewModelBase
    {
        #region ----- Construtor -----
        public vmAntesDaEntrada(vmCheckList pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.AntesEntradaPaciente.IsNull())
                this._AntesEntradaPaciente = new wrpAntesEntradaPaciente(pvmCheckList.Usuario.DomainObject);

            else
            {
                this._AntesEntradaPaciente = this._checklist.AntesEntradaPaciente;
                if (this._AntesEntradaPaciente.ConfirmacaoPeloProntuario.Equals(SimNao.Sim))
                    this._boolProntuario = true;
                else if (this._AntesEntradaPaciente.Responsavel.IsEmpty())
                    this._boolPaciente = true;
                else
                    this._boolResponsavel = true;
            }

            this._paciente = new wrpPaciente(this._vmchecklist.Paciente.DomainObject);
            this._paciente.DomainObject.Refresh();

            // Salva checkList para gerar um id para 
            _vmchecklist.Salvar();          

            wrpAlergiaEventoCollection _AlergiaCollection = null;

            IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
            repa.OndeChaveIgual(_vmchecklist.CheckListdto.CheckList.ID);
            repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.CheckList);
            var reta = repa.List();
            if (reta.IsNotNull())
                _AlergiaCollection = new wrpAlergiaEventoCollection(reta);

            this._vmalergiasevento = new vmAlergiasEvento(true, this._paciente, this._vmchecklist.Usuario, this._vmchecklist.Usuario.Prestador.IsCorpoClinico
                , TipoEvento.CheckList, _AlergiaCollection
                , _vmchecklist.CheckListdto.CheckList.ID, this._checklist.Atendimento);


            if (_vmchecklist.IsNovoChecklist)
                _vmalergiasevento.MarcarTodasAlergias();        
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckList _vmchecklist;
        private bool _boolResponsavel;
        private bool _boolPaciente;
        private bool _boolProntuario;
        private wrpCheckListCirurgia _checklist;
        private wrpAntesEntradaPaciente _AntesEntradaPaciente;
        private vmAlergiasEvento _vmalergiasevento;
        private wrpPaciente _paciente;
        //private wrpEventoCheckListCirurgia _eventochecklistcirurgia;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListCirurgia CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.CheckList));
            }
        }

        public wrpAntesEntradaPaciente AntesEntradaPaciente
        {
            get { return _AntesEntradaPaciente; }
            set
            {
                this._AntesEntradaPaciente = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaEntrada>(x => x.AntesEntradaPaciente));
            }
        }

        public bool boolPaciente
        {
            get { return this._boolPaciente; }
            set
            {
                this._boolPaciente = value;
                if (this._boolPaciente)
                    this._AntesEntradaPaciente.ConfirmacaoPeloProntuario = SimNao.Nao;
                this._AntesEntradaPaciente.Responsavel = string.Empty;
                this._AntesEntradaPaciente.Observacao = string.Empty;
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
                    this._AntesEntradaPaciente.ConfirmacaoPeloProntuario = SimNao.Nao;
                this._AntesEntradaPaciente.Observacao = string.Empty;
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
                    this._AntesEntradaPaciente.ConfirmacaoPeloProntuario = SimNao.Sim;
                this._AntesEntradaPaciente.Responsavel = string.Empty;
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

            //foreach (var ale in this._vmalergiasevento.AlergiaCollection)
            //{
            //    var jaexiste = this._eventochecklistcirurgia.AlergiaEventos.Where(x => x.Chave == this._checklist.ID && x.Alergia.ID == ale.ID).SingleOrDefault();
            //    if (jaexiste.IsNull())
            //    {
            //        if (ale.Selecionado)
            //            this._eventochecklistcirurgia.AlergiaEventos.Add(new wrpAlergiaEvento
            //            {
            //                Alergia = ale,
            //                Chave = this._checklist.ID,
            //                Atendimento = this._checklist.Atendimento,
            //                Evento = new wrpEvento(this._eventochecklistcirurgia.DomainObject),
            //                Data = DateTime.Now,
            //                Usuario = this._checklist.Usuario
            //            });
            //    }
            //    else
            //    {
            //        if (ale.Selecionado)
            //        {
            //            jaexiste.Data = DateTime.Now;
            //            jaexiste.Usuario = this._checklist.Usuario;
            //        }
            //        else
            //            this._eventochecklistcirurgia.AlergiaEventos.Remove(jaexiste);
            //    }
            //}

            //this._eventochecklistcirurgia.Save();
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
                else if (this._boolProntuario && this._AntesEntradaPaciente.Observacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe a observação do prontuário");

                if (this._vmalergiasevento.AlergiaCollection.Count(x => x.Selecionado && x.Status != StatusAlergiaProblema.Excluído) == 0)
                    erros.Add("Alergia deve ser informada ou marcar sem alergias conhecidas.");

                if (this._AntesEntradaPaciente.Nome.Equals(SimNao.Nao))
                    erros.Add("Campo 'Nome' é obrigatório!");

                if (this._AntesEntradaPaciente.Procedimento.Equals(SimNao.Nao))
                    erros.Add("Campo 'Procedimento' é obrigatório!");

                if (this._AntesEntradaPaciente.LocalProcedimento.Equals(SimNao.Nao))
                    erros.Add("Campo 'Local Procedimento' é obrigatório!");

                if (this._AntesEntradaPaciente.ConfirmacaoPeloProntuario.Equals(SimNao.Sim) && this._AntesEntradaPaciente.Observacao.IsEmpty())
                    erros.Add("Campo 'Observação' é obrigatório!");

                if (this._vmalergiasevento.IsNotNull() && !this._vmalergiasevento.SemAlergiasConhecidas && this._vmalergiasevento.AlergiaCollection.HasItems() && this._vmalergiasevento.AlergiaCollection.Count(x => x.Selecionado) == 0)
                    erros.Add("Campo 'Alergias' é obrigatório!");

                if (this._AntesEntradaPaciente.Pulseira.IsNull())
                    erros.Add("Campo 'Pulseira' é obrigatório!");

                if (this._AntesEntradaPaciente.SitioNA.Equals(SimNao.Nao))
                {
                    if (this._AntesEntradaPaciente.Local.IsEmptyOrWhiteSpace())
                        erros.Add("Campo 'Local' é obrigatório!");

                    if (this._AntesEntradaPaciente.Nivel.IsEmptyOrWhiteSpace() && this._AntesEntradaPaciente.Direito.Equals(SimNao.Nao) && this._AntesEntradaPaciente.Esquerdo.Equals(SimNao.Nao))
                        erros.Add("Informe se é o 'Direito' e/ou 'Esquerdo' ou o 'Nível'.");

                }

                if (this._AntesEntradaPaciente.ConsentimentoAnestesia.Equals(SimNao.Nao) && this._AntesEntradaPaciente.ConsentimentoAnestesiaObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Marque o campo 'Consentimento Anestesia Completo' ou informe a 'Observação'");

                if (this._AntesEntradaPaciente.AvaliacaoPreAnestesica.Equals(SimNao.Nao) && this._AntesEntradaPaciente.AvaliacaoPreAnestesicaObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Marque o campo 'Avaliação Pré Anestésica' ou informe a 'Observação'");

                if (this._AntesEntradaPaciente.ConsentimentoCirurgico.Equals(SimNao.Nao) && this._AntesEntradaPaciente.ConsentimentoCirurgicoObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Marque o campo 'Consentimento Cirúrgico Completo' ou informe a 'Observação'");

                if (this._AntesEntradaPaciente.SumarioAvaliacaoMedica.Equals(SimNao.Nao) && this._AntesEntradaPaciente.SumarioAvaliacaoMedicaObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Marque o campo 'Sumário de Avaliação Médica' ou informe a 'Observação'");

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._checklist.AntesEntradaPaciente = this._AntesEntradaPaciente;
                this._checklist.AntesEntradaPaciente.DataEncerramento = DateTime.Now;
                SalvaAlergias();

                return true;
            }
        }
    }
}
