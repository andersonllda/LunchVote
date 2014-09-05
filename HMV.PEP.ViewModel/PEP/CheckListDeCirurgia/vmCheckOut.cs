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
    public class vmCheckOut : ViewModelBase
    {
        #region ----- Construtor -----
        public vmCheckOut(vmCheckList pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.CheckOut.IsNull())
                this._CheckOut = new wrpCheckOut(pvmCheckList.Usuario);
            else
                this._CheckOut = this._checklist.CheckOut;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckList _vmchecklist;
        private wrpCheckListCirurgia _checklist;
        private wrpCheckOut _CheckOut;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListCirurgia CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.CheckList));
            }
        }

        public wrpCheckOut CheckOut
        {
            get { return this._CheckOut; }
            set
            {
                this._CheckOut = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.CheckOut));
            }
        }

        public bool Patalogico
        {
            get { return this._CheckOut.Patalogico == SimNA.Sim; }
            set
            {
                if (value)
                    this._CheckOut.Patalogico = SimNA.Sim;
                else
                    this._CheckOut.Patalogico = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.PatalogicoNA));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.Patalogico));
            }
        }

        public bool PatalogicoNA
        {
            get { return this._CheckOut.Patalogico == SimNA.NA; }
            set
            {
                if (value)
                {
                    this._CheckOut.Patalogico = SimNA.NA;
                    this._CheckOut.Pecas = string.Empty;
                }
                else
                    this._CheckOut.Patalogico = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.Patalogico));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckOut>(x => x.PatalogicoNA));
            }
        }
        #endregion

        public override bool IsValid
        {
            get
            {
                IList<string> erros = new List<string>();
                if (this._CheckOut.Prescricao.Equals(SimNao.Nao))
                    erros.Add("Campo 'Prescrição Realizada' é obrigatório!");

                if (this._CheckOut.DescricaoCirurgica.Equals(SimNao.Nao))
                    erros.Add("Campo 'Descrição Cirúrgica' é obrigatório!");
                
                //tfs 3429 
                //if (this._CheckOut.Justificativa.Equals(SimNao.Nao))
                //    erros.Add("Campo 'Justificativa' é obrigatório!");

                if (this._CheckOut.InstrumentosContagem.Equals(SimNao.Nao))
                    erros.Add("Campo 'Contagem de Instrumentos' é obrigatório!");

                if (this._CheckOut.Debitos.Equals(SimNao.Nao))
                    erros.Add("Campo 'Débitos lançados' é obrigatório!");

                if (this._CheckOut.Equipamento.Equals(SimNao.Nao) && this._CheckOut.EquipamentoDescricao.IsEmptyOrWhiteSpace())
                    erros.Add("Marque 'Equipamentos funcionaram corretamente' ou Informe o 'Equipamento'.");
                
                if (this._CheckOut.EquipamentoDescricao.IsNotEmptyOrWhiteSpace() && (this._CheckOut.Solucionado.Equals(SimNao.Nao) && this._CheckOut.Trocado.Equals(SimNao.Nao)))
                    erros.Add("Informe se o equipamento foi 'Trocado' ou 'Solucionado'.");

                if (this._CheckOut.Patalogico.IsNull())
                    erros.Add("Campo 'Identificação de Anatomo-Patologico' é obrigatório!");
                 if (this._CheckOut.Patalogico.Equals(SimNA.Sim) && this._CheckOut.Pecas.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Número de peças anatômicas'.");

                if (this._CheckOut.Oximetria.Equals(SimNao.Nao) && this._CheckOut.EquipamentoObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe a 'Observação' da 'Oximetria de Transporte'.");

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                ValidaSondagem();

                this._CheckOut.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.CheckOut = this._CheckOut;
                return true;
            }
        }

        public void ValidaSondagem()
        {
            if ( (this._checklist.Sondagem.VesicalAlivio == SimNao.Sim || this._checklist.Sondagem.VesicalDemora == SimNao.Sim) && this._checklist.Sondagem.VesicalDemoraCirurgiao == SimNao.Nao )
                if (this._checklist.Sondagem.DataEncerramento.IsNull())
                {
                    DXMessageBox.Show("Para concluir o CheckList a Enfermeira necessita informar a Sondagem", "Atenção");
                    return;
                }
            //Caso nao precise ter sondagem finaliza o checklist
            if (this._vmchecklist.CheckListdto.CheckList.DataEncerramento.IsNull())
            {
                this._vmchecklist.CheckListdto.CheckList.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.UsuarioEncerramento = this._vmchecklist.Usuario;
            }
        }
    }
}
