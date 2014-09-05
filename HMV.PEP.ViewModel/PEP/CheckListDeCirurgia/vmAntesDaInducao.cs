using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using HMV.Core.Domain.Model;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using HMV.Core.Domain.Model.PEP.CheckListCirurgia;
using HMV.Core.Framework.Exception;

namespace HMV.PEP.ViewModel.PEP.CheckListDeCirurgia
{
    public class vmAntesDaInducao : ViewModelBase
    {
        #region ----- Construtor -----
        public vmAntesDaInducao(vmCheckList pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.AntesInducaoAnestesica.IsNull())
                this._AntesInducaoAnestesica = new wrpAntesInducaoAnestesica(pvmCheckList.Usuario);
            else
                this._AntesInducaoAnestesica = this._checklist.AntesInducaoAnestesica;

        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckList _vmchecklist;
        private wrpCheckListCirurgia _checklist;
        private wrpAntesInducaoAnestesica _AntesInducaoAnestesica;
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

        public wrpAntesInducaoAnestesica AntesInducaoAnestesica
        {
            get { return this._AntesInducaoAnestesica; }
            set
            {
                this._AntesInducaoAnestesica = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.AntesInducaoAnestesica));
            }
        }

        public bool boolViaAereaSim
        {
            get { return this._AntesInducaoAnestesica.ViaAerea.Equals(SimNaoNA.Sim); }
            set
            {
                if (value)
                    this._AntesInducaoAnestesica.ViaAerea = SimNaoNA.Sim;
                else
                {
                    this._AntesInducaoAnestesica.ViaAerea = null;
                    EquipamentosDisponiveis = string.Empty;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaNA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaSim));
            }
        }

        public bool boolViaAereaNao
        {
            get { return this._AntesInducaoAnestesica.ViaAerea.Equals(SimNaoNA.Nao); }
            set
            {
                if (value)
                {
                    this._AntesInducaoAnestesica.ViaAerea = SimNaoNA.Nao;
                    EquipamentosDisponiveis = string.Empty;
                }
                else
                    this._AntesInducaoAnestesica.ViaAerea = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaNA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaNao));
            }
        }

        public bool boolViaAereaNA
        {
            get { return this._AntesInducaoAnestesica.ViaAerea.Equals(SimNaoNA.NA); }
            set
            {
                if (value)
                {
                    this._AntesInducaoAnestesica.ViaAerea = SimNaoNA.NA;
                    EquipamentosDisponiveis = string.Empty;                    
                }
                else
                    this._AntesInducaoAnestesica.ViaAerea = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolViaAereaNA));
            }
        }

        public string EquipamentosDisponiveis
        {
            get { return this._AntesInducaoAnestesica.EquipamentosDisponiveis; }
            set
            {
                this._AntesInducaoAnestesica.EquipamentosDisponiveis = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.EquipamentosDisponiveis));
            }
        }

        public bool boolPerdaSanquineaSim
        {
            get { return this._AntesInducaoAnestesica.RiscoPerdaSangue.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._AntesInducaoAnestesica.RiscoPerdaSangue = SimNao.Sim;
                else
                    this._AntesInducaoAnestesica.RiscoPerdaSangue = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolPerdaSanquineaNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolPerdaSanquineaSim));
            }
        }

        public bool boolPerdaSanquineaNao
        {
            get { return this._AntesInducaoAnestesica.RiscoPerdaSangue.Equals(SimNao.Nao); }
            set
            {
                if (value)
                    this._AntesInducaoAnestesica.RiscoPerdaSangue = SimNao.Nao;
                else
                    this._AntesInducaoAnestesica.RiscoPerdaSangue = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolPerdaSanquineaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolPerdaSanquineaNao));
            }
        }

        public bool boolExameImagemSim
        {
            get { return this._AntesInducaoAnestesica.ExamesImagens.Equals(SimNaoNA.Sim); }
            set
            {
                if (value)
                    this._AntesInducaoAnestesica.ExamesImagens = SimNaoNA.Sim;
                else
                    this._AntesInducaoAnestesica.ExamesImagens = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemNA));
            }
        }

        public bool boolExameImagemNao
        {
            get { return this._AntesInducaoAnestesica.ExamesImagens.Equals(SimNaoNA.Nao); }
            set
            {
                if (value)                
                    this._AntesInducaoAnestesica.ExamesImagens = SimNaoNA.Nao;
               
                else
                    this._AntesInducaoAnestesica.ExamesImagens = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemNA));
            }
        }

        public bool boolExameImagemNA
        {
            get { return this._AntesInducaoAnestesica.ExamesImagens.Equals(SimNaoNA.NA); }
            set
            {
                if (value)
                    this._AntesInducaoAnestesica.ExamesImagens = SimNaoNA.NA;

                else
                    this._AntesInducaoAnestesica.ExamesImagens = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolExameImagemNA));
            }
        }

        public bool boolOPMESim
        {
            get { return this._AntesInducaoAnestesica.OPME.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._AntesInducaoAnestesica.OPME = SimNao.Sim;
                else
                    this._AntesInducaoAnestesica.OPME = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolOPMENao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolOPMESim));
            }
        }

        public bool boolOPMENao
        {
            get { return this._AntesInducaoAnestesica.OPME.Equals(SimNao.Nao); }
            set
            {
                if (value)
                    this._AntesInducaoAnestesica.OPME = SimNao.Nao;
                else
                    this._AntesInducaoAnestesica.OPME = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolOPMESim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmAntesDaInducao>(x => x.boolOPMENao));
            }
        }

        #endregion

        public override bool IsValid
        {
            get
            {
                IList<string> erros = new List<string>();
                if (this._AntesInducaoAnestesica.Nome.Equals(SimNao.Nao))
                    erros.Add("Campo 'Nome do Paciente' é obrigatório!");

                if (this._AntesInducaoAnestesica.Procedimento.Equals(SimNao.Nao))
                    erros.Add("Campo 'Procedimento' é obrigatório!");

                if (this._AntesInducaoAnestesica.LadoAbordado.Equals(SimNao.Nao))
                    erros.Add("Campo 'Lado Abordado' é obrigatório!");

                if (this._AntesInducaoAnestesica.ViaAerea.IsNull())
                    erros.Add("Campo 'Dificuldade de Via Aérea / Risco de Aspiração' é obrigatório!");
                else if (this._AntesInducaoAnestesica.ViaAerea.Value.Equals(SimNaoNA.Sim) && this._AntesInducaoAnestesica.EquipamentosDisponiveis.IsEmptyOrWhiteSpace())
                    erros.Add("Informe os 'Equipamentos Disponíveis'");

                if (this._AntesInducaoAnestesica.RiscoPerdaSangue.IsNull())
                    erros.Add("Campo 'Risco de Perda Sanguínea' é obrigatório!");

                if (this._AntesInducaoAnestesica.ExamesImagens.IsNull())
                    erros.Add("Campo 'Exames e Imagens Essenciais' é obrigatório!");

                if (this._AntesInducaoAnestesica.OPME.IsNull())
                    erros.Add("Campo 'Equipamentos Específicos/OPME' é obrigatório!");

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._AntesInducaoAnestesica.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.AntesInducaoAnestesica = this._AntesInducaoAnestesica;
                return true;
            }
        }
    }
}
