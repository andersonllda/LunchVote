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

namespace HMV.PEP.ViewModel.PEP.CheckListDeUDI
{
    public class vmTimeOut : ViewModelBase
    {
        #region ----- Construtor -----
        public vmTimeOut(vmCheckListUDI pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.TimeOutUDI.IsNull())
                this._TimeOut = new wrpTimeOutUDI(pvmCheckList.Usuario);

            else
                this._TimeOut = this._checklist.TimeOutUDI;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckListUDI _vmchecklist;
        private wrpCheckListUDI _checklist;
        private wrpTimeOutUDI _TimeOut;
        #endregion

        #region ----- Propriedades Privadas -----

        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListUDI CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<TimeOut>(x => x.CheckList));
            }
        }

        public wrpTimeOutUDI TimeOut
        {
            get { return this._TimeOut; }
            set
            {
                this._TimeOut = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.TimeOut));
            }
        }

        public bool boolProcedimentoAnestesiaSim
        {
            get { return this._TimeOut.ProcedimentoAnestesia.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TimeOut.ProcedimentoAnestesia = SimNao.Sim;
                else
                    this._TimeOut.ProcedimentoAnestesia = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolProcedimentoAnestesiaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolProcedimentoAnestesiaNao));
            }
        }

        public bool boolProcedimentoAnestesiaNao
        {
            get { return this._TimeOut.ProcedimentoAnestesia.Equals(SimNao.Nao); }
            set
            {
                if (value)
                    this._TimeOut.ProcedimentoAnestesia = SimNao.Nao;
                else
                    this._TimeOut.ProcedimentoAnestesia = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolProcedimentoAnestesiaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolProcedimentoAnestesiaNao));
            }
        }

        public bool boolCarroSim
        {
            get { return this._TimeOut.Carro.Equals(SimNA.Sim); }
            set
            {
                if (value)
                    this._TimeOut.Carro = SimNA.Sim;
                else
                    this._TimeOut.Carro = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolCarroSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolCarroNA));
            }
        }

        public bool boolCarroNA
        {
            get { return this._TimeOut.Carro.Equals(SimNA.NA); }
            set
            {
                if (value)
                    this._TimeOut.Carro = SimNA.NA;
                else
                    this._TimeOut.Carro = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolCarroSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolCarroNA));
            }
        }

        public bool boolMonitorSim
        {
            get { return this._TimeOut.Monitor.Equals(SimNA.Sim); }
            set
            {
                if (value)
                    this._TimeOut.Monitor = SimNA.Sim;
                else
                    this._TimeOut.Monitor = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolMonitorSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolMonitorNA));
            }
        }

        public bool boolMonitorNA
        {
            get { return this._TimeOut.Monitor.Equals(SimNA.NA); }
            set
            {
                if (value)
                    this._TimeOut.Monitor = SimNA.NA;
                else
                    this._TimeOut.Monitor = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolMonitorSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolMonitorNA));
            }
        }

        public bool boolOximetroSim
        {
            get { return this._TimeOut.Oximetro.Equals(SimNA.Sim); }
            set
            {
                if (value)
                    this._TimeOut.Oximetro = SimNA.Sim;
                else
                    this._TimeOut.Oximetro = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolOximetroSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolOximetroNA));
            }
        }

        public bool boolOximetroNA
        {
            get { return this._TimeOut.Oximetro.Equals(SimNA.NA); }
            set
            {
                if (value)
                    this._TimeOut.Oximetro = SimNA.NA;
                else
                    this._TimeOut.Oximetro = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolOximetroSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTimeOut>(x => x.boolOximetroNA));
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

                if (this._TimeOut.NA.IsNull())
                {
                    if (this._TimeOut.NomeCompleto.IsNull() || this._TimeOut.Procedimento.IsNull() || this._TimeOut.Lado.IsNull())
                        erros.Add("Informe os campos 'Nome Completo', 'Procedimento' e 'Lado Abordado' ou marque 'NA'!");
                }

                if (this._TimeOut.ProcedimentoAnestesia.IsNull())
                {
                    erros.Add("Informe o campo 'Procedimento com Anestesia'!");
                }

                if (this._TimeOut.Carro.IsNull())
                {
                    erros.Add("Informe o campo 'Carro Anestésico'!");
                }

                if (this._TimeOut.Monitor.IsNull())
                {
                    erros.Add("Informe o campo 'Monitor Cardíaco'!");
                }

                if (this._TimeOut.Oximetro.IsNull())
                {
                    erros.Add("Informe o campo 'Oxímetro'!");
                }

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._TimeOut.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.TimeOutUDI = this._TimeOut;
                return true;
            }
        }
    }
}
