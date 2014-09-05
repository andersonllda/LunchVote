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
using HMV.Core.Interfaces;
using StructureMap;
using System.Configuration;

namespace HMV.PEP.ViewModel.PEP.CheckListDeCirurgia
{
    public class vmTimeOut : ViewModelBase
    {
        #region ----- Construtor -----
        public vmTimeOut(vmCheckList pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.TimeOut.IsNull())
                this._TimeOut = new wrpTimeOut(pvmCheckList.Usuario);

            else
                this._TimeOut = this._checklist.TimeOut;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckList _vmchecklist;
        private wrpCheckListCirurgia _checklist;
        private wrpTimeOut _TimeOut;
        #endregion

        #region ----- Propriedades Privadas -----

        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListCirurgia CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<TimeOut>(x => x.CheckList));
            }
        }

        public wrpTimeOut TimeOut
        {
            get { return this._TimeOut; }
            set
            {
                this._TimeOut = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.TimeOut));
            }
        }

        public bool boolSitioSim
        {
            get { return this._TimeOut.SitioCirurgico.Equals(SimNA.Sim); }
            set
            {
                if (value)
                    this._TimeOut.SitioCirurgico = SimNA.Sim;
                else
                    this._TimeOut.SitioCirurgico = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolSitioNA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolSitioSim));
            }
        }

        public bool boolSitioNA
        {
            get { return this._TimeOut.SitioCirurgico.Equals(SimNA.NA); }
            set
            {
                if (value)
                    this._TimeOut.SitioCirurgico = SimNA.NA;
                else
                    this._TimeOut.SitioCirurgico = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolSitioSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolSitioNA));
            }
        }

        public bool boolAntimicrobianoSim
        {
            get { return this._TimeOut.Antimicrobiano.Equals(SimNA.Sim); }
            set
            {
                if (value)
                    this._TimeOut.Antimicrobiano = SimNA.Sim;
                else
                    this._TimeOut.Antimicrobiano = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolAntimicrobianoNA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolAntimicrobianoSim));
            }
        }

        public bool boolAntimicrobianoNA
        {
            get { return this._TimeOut.Antimicrobiano.Equals(SimNA.NA); }
            set
            {
                if (value)
                    this._TimeOut.Antimicrobiano = SimNA.NA;
                else
                    this._TimeOut.Antimicrobiano = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolAntimicrobianoSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolAntimicrobianoNA));
            }
        }

        public bool HabilitaControles
        {
            get
            {
                return this._TimeOut.DataEncerramento.IsNull();
            }
        }

        #endregion

        public override bool IsValid
        {
            get
            {
                IList<string> erros = new List<string>();

                if (this._TimeOut.Cirurgiao.IsNull())
                    erros.Add("Campo 'Cirurgião' é obrigatório!");

                if (this._TimeOut.Auxiliar1.IsNotNull())
                {
                    if (this._TimeOut.Auxiliar1.Registro.IsEmptyOrWhiteSpace() && !this._TimeOut.Auxiliar1.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Registro' do Auxiliar 1!");
                    else if (!this._TimeOut.Auxiliar1.Registro.IsEmptyOrWhiteSpace() && this._TimeOut.Auxiliar1.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Nome' do Auxiliar 1!");
                }

                if (this._TimeOut.Auxiliar2.IsNotNull())
                {
                    if (this._TimeOut.Auxiliar2.Registro.IsEmptyOrWhiteSpace() && !this._TimeOut.Auxiliar2.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Registro' do Auxiliar 2!");
                    else if (!this._TimeOut.Auxiliar2.Registro.IsEmptyOrWhiteSpace() && this._TimeOut.Auxiliar2.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Nome' do Auxiliar 2!");
                }

                if (this._TimeOut.Instrumentador.IsNotNull())
                {
                    if (this._TimeOut.Instrumentador.Registro.IsEmptyOrWhiteSpace() && !this._TimeOut.Instrumentador.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Registro' do Instrumentador!");
                    else if (!this._TimeOut.Instrumentador.Registro.IsEmptyOrWhiteSpace() && this._TimeOut.Instrumentador.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Nome' do Instrumentador!");
                }

                if (this._TimeOut.Circulante.IsNull())
                    erros.Add("Campo 'Circulante' é obrigatório!");
                else if (this._TimeOut.Circulante.IsNotNull())
                {
                    if (this._TimeOut.Circulante.Registro.IsEmptyOrWhiteSpace() && !this._TimeOut.Circulante.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Registro' do Circulante!");
                    else if (!this._TimeOut.Circulante.Registro.IsEmptyOrWhiteSpace() && this._TimeOut.Circulante.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Nome' do Circulante!");
                }

                if (this._TimeOut.Anestesista.IsNotNull())
                {
                    if (this._TimeOut.Anestesista.Registro.IsEmptyOrWhiteSpace() && !this._TimeOut.Anestesista.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Registro' do Anestesista!");
                    else if (!this._TimeOut.Anestesista.Registro.IsEmptyOrWhiteSpace() && this._TimeOut.Anestesista.Nome.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Nome' do Anestesista!");
                }

                //if (this._TimeOut.ConfirmarMembrosEquipe.Equals(SimNao.Nao))
                //    erros.Add("Campo 'Confirma membros da equipe Nome e Função' é obrigatório!");

                if (this._TimeOut.SitioCirurgico.IsNull())
                    erros.Add("Campo 'Reconfirmação do Sítio Cirúrgico' é obrigatório!");

                if (this._TimeOut.Antimicrobiano.IsNull())
                    erros.Add("Campo 'Antimicrobiano profilático' é obrigatório!");
                else if (this._TimeOut.Antimicrobiano.Value.Equals(SimNA.Sim) && this._TimeOut.AntimicrobianoObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Antibiótico'.");

                if (this._TimeOut.EquipamentosConformePlanejamento.Equals(SimNao.Nao))
                    erros.Add("Campo 'Equipamento conforme planejamento' é obrigatório!");

                if (this._TimeOut.EsterilizacaoInstrumental.Equals(SimNao.Nao))
                    erros.Add("Campo 'Esterilização confirmada do Intrumental' é obrigatório!");

                if (this._TimeOut.IndicadoresEsterilidade.Equals(SimNao.Nao))
                    erros.Add("Campo 'Indicadores de esterílidade estão disponíveis' é obrigatório!");

                if (this._TimeOut.OPME.Equals(SimNao.Nao) && this._TimeOut.InstrumentadorNA.Equals(SimNao.Nao))
                    erros.Add("Campo 'OPME necessário disponível' é obrigatório!");

                if (this._TimeOut.CirurgiaoEtapa.Equals(SimNao.Nao))
                    erros.Add("Campo 'Etapas criticas e inesperadas durante o procedimento' é obrigatório!");

                else if (this._TimeOut.CirurgiaoEquipe.Equals(SimNao.Sim) && this._TimeOut.CirurgiaoObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe a 'Observação'!");              

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._TimeOut.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.TimeOut = this._TimeOut;
                return true;
            }
        }
    }
}
