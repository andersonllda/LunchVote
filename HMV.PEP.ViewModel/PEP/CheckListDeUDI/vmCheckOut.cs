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
using HMV.Core.Domain.Model.PEP.CheckListUDI;

namespace HMV.PEP.ViewModel.PEP.CheckListDeUDI
{
    public class vmCheckOut : ViewModelBase
    {
        #region ----- Construtor -----
        public vmCheckOut(vmCheckListUDI pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.CheckOutUDI.IsNull())
                this._CheckOut = new wrpCheckOutUDI(pvmCheckList.Usuario);
            else
                this._CheckOut = this._checklist.CheckOutUDI;

            if (this._CheckOut.CheckOutMaterialUDI.HasItems())
                this._checkoutmaterialudilista = this._CheckOut.CheckOutMaterialUDI;
            else
                _checkoutmaterialudilista = new wrpCheckOutMaterialUDICollection(new List<CheckOutMaterialUDI>());
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckListUDI _vmchecklist;
        private wrpCheckListUDI _checklist;
        private wrpCheckOutUDI _CheckOut;
        private wrpCheckOutMaterialUDICollection _checkoutmaterialudilista;
        private wrpCheckOutMaterialUDI _checkoutmaterialudiSelecionado;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListUDI CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.CheckList));
            }
        }

        public wrpCheckOutUDI CheckOut
        {
            get { return this._CheckOut; }
            set
            {
                this._CheckOut = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.CheckOut));
            }
        }

        public bool MaterialSim
        {
            get { return this._CheckOut.Material == SimNao.Sim; }
            set
            {
                if (value)
                    this._CheckOut.Material = SimNao.Sim;
                else
                    this._CheckOut.Material = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.MaterialSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.MaterialNao));
            }
        }

        public bool MaterialNao
        {
            get { return this._CheckOut.Material == SimNao.Nao; }
            set
            {
                if (value)
                {
                    this._CheckOut.Material = SimNao.Nao;                   
                }
                else
                    this._CheckOut.Material = null;

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.MaterialSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.MaterialNao));
            }
        }

        public wrpCheckOutMaterialUDICollection CheckOutMaterialUDILista
        {
            get
            {
                return _checkoutmaterialudilista;
            }
        }

        public wrpCheckOutMaterialUDI CheckOutMaterialUDISelecionado
        {
            get
            {
                return _checkoutmaterialudiSelecionado;
            }
            set
            {
                this._checkoutmaterialudiSelecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.CheckOutMaterialUDISelecionado));
            }
        }

        public string Descricao
        {
            get
            {
                return _checkoutmaterialudiSelecionado.Descricao;
            }
            set
            {
                _checkoutmaterialudiSelecionado.Descricao = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.Descricao));
            }
        }
          
        public string NumeroPecas
        {
            get
            {
                return _checkoutmaterialudiSelecionado.NumeroPecas;
            }
            set
            {
                _checkoutmaterialudiSelecionado.NumeroPecas = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.NumeroPecas));
            }
        }

        #endregion

        public override bool IsValid
        {
            get
            {
                IList<string> erros = new List<string>();
                if (this._CheckOut.Material.Equals(SimNao.Sim))
                    if (!this._CheckOut.CheckOutMaterialUDI.HasItems())
                        erros.Add("Informe ao menos um Laboratório!");               

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._CheckOut.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.CheckOutUDI = this._CheckOut;
                this._vmchecklist.CheckListdto.CheckList.CheckOutUDI.CheckListUDI = _checklist;
               
                //Finaliza CheckListUDI
                this._vmchecklist.CheckListdto.CheckList.DataEncerramento = DateTime.Now;
                return true;
            }
        }       

        protected override bool CommandCanExecuteExcluir(object param)
        {
            return this._checkoutmaterialudiSelecionado.IsNotNull();
        }

        protected override void CommandExcluir(object param)
        {
            var item = this._CheckOut.CheckOutMaterialUDI.Where(x=> x.ID == this._checkoutmaterialudiSelecionado.ID).SingleOrDefault();
            this._CheckOut.CheckOutMaterialUDI.Remove(item);

            this._checkoutmaterialudilista.Remove(this._checkoutmaterialudiSelecionado);
            this._checkoutmaterialudiSelecionado = null;
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.CheckOutMaterialUDILista));
        }

        public void Novo()
        {
            this._checkoutmaterialudiSelecionado = new wrpCheckOutMaterialUDI(this._checklist);
        }

        public void Salva()
        {
            this._CheckOut.CheckOutMaterialUDI.Add(this._checkoutmaterialudiSelecionado);
            this._checkoutmaterialudilista.Add(this._checkoutmaterialudiSelecionado);
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.CheckOutMaterialUDILista));
        }
    }
}
