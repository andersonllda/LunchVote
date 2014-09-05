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
    public class vmSondagem : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSondagem(vmCheckList pvmCheckList)
        {
            this._usuarios = pvmCheckList.Usuario;
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.Sondagem.IsNull())
                this._Sondagem = new wrpSondagem();
            else
                this._Sondagem = this._checklist.Sondagem;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckList _vmchecklist;
        private wrpCheckListCirurgia _checklist;
        private wrpSondagem _Sondagem;
        private wrpUsuarios _usuarios;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListCirurgia CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.CheckList));
            }
        }

        public wrpSondagem Sondagem
        {
            get { return this._Sondagem; }
            set
            {
                this._Sondagem = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.Sondagem));
            }
        }

        public bool boolVesicalDemoraSim
        {
            get { return this._Sondagem.VesicalDemora.HasValue && this._Sondagem.VesicalDemora.Value.Equals(SimNao.Sim); }
            set
            {
                this._Sondagem.VesicalDemora = null;
                if (value)
                    this._Sondagem.VesicalDemora = SimNao.Sim;
                else
                    VolumeSala = string.Empty;    

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalDemoraNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalDemoraSim));
            }
        }

        public bool boolVesicalDemoraCirurgiao
        {
            get { return this._Sondagem != null && this._Sondagem.VesicalDemoraCirurgiao.Equals(SimNao.Sim); }
            set
            {
                this._Sondagem.VesicalDemoraCirurgiao = value ? SimNao.Sim : SimNao.Nao;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalDemoraCirurgiao));
            }
        }

        public bool boolVesicalDemoraNao
        {
            get { return this._Sondagem.VesicalDemora.HasValue && this._Sondagem.VesicalDemora.Value.Equals(SimNao.Nao); }
            set
            {
                this._Sondagem.VesicalDemora = null;
                if (value)
                {
                    boolVesicalDemoraCirurgiao = false;
                    this._Sondagem.VesicalDemora = SimNao.Nao;
                    VolumeSala = string.Empty;    
                }

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalDemoraSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalDemoraNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalDemoraCirurgiao));
            }
        }

        public string VolumeSala
        {
            get { return this._Sondagem.VolumeSala; }
            set
            {
                this._Sondagem.VolumeSala = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.VolumeSala));
            }
        }

        public bool boolVesicalAlivioSim
        {
            get { return this._Sondagem.VesicalAlivio.HasValue && this._Sondagem.VesicalAlivio.Value.Equals(SimNao.Sim); }
            set
            {
                this._Sondagem.VesicalAlivio = null;
                if (value)
                    this._Sondagem.VesicalAlivio = SimNao.Sim;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalAlivioNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalAlivioSim));
            }
        }

        public bool boolVesicalAlivioNao
        {
            get { return this._Sondagem.VesicalAlivio.HasValue && this._Sondagem.VesicalAlivio.Value.Equals(SimNao.Nao); }
            set
            {
                this._Sondagem.VesicalAlivio = null;
                if (value)
                    this._Sondagem.VesicalAlivio = SimNao.Nao;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalAlivioSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSondagem>(x => x.boolVesicalAlivioNao));
            }
        }

        #endregion

        public IList<string> Valida()
        {
            IList<string> erros = new List<string>();
            if (this._Sondagem.VesicalDemora.IsNull())
                erros.Add("Campo 'Sondagem Vesical de Demora' é obrigatório!");
            else if (this._Sondagem.VesicalDemora.Value.Equals(SimNao.Sim))
            {
                //TFS:3305
                //if (this._Sondagem.VolumeSala.IsEmptyOrWhiteSpace())
                //  erros.Add("Campo 'Volume em Sala' é obrigatório!");

                if (this._Sondagem.FoleyNumero.IsEmptyOrWhiteSpace())
                    erros.Add("Campo 'Foley' é obrigatório!");

                if (this._Sondagem.VolumeBalonete.IsEmptyOrWhiteSpace())
                    erros.Add("Campo 'Volume Balonete' é obrigatório!");

                if (this._Sondagem.AspectoDiurese.IsEmptyOrWhiteSpace())
                    erros.Add("Campo 'Aspecto Diurese' é obrigatório!");
            }

            if (this._Sondagem.VesicalAlivio.IsNull())
                erros.Add("Campo 'Sondagem Vesical de Alívio' é obrigatório!");
            else if (this._Sondagem.VesicalAlivio.Value.Equals(SimNao.Sim) && this._Sondagem.VesicalAlivioVolume.IsEmptyOrWhiteSpace())
                erros.Add("Campo 'Volume' da Sondagem Vesical de Alívio é obrigatório!");

            return erros;
        }
           

        public override bool IsValid
        {
            get
            {
                IList<string> erros = Valida();

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._Sondagem.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.Sondagem = this._Sondagem;

                if (this._vmchecklist.CheckListdto.CheckList.TransOperatorio.IsNotNull() && this._vmchecklist.CheckListdto.CheckList.TransOperatorio.DataEncerramento.IsNotNull() && this._vmchecklist.CheckListdto.CheckList.DataEncerramento.IsNull())
                {
                    this._vmchecklist.CheckListdto.CheckList.DataEncerramento = DateTime.Now;
                    this._vmchecklist.CheckListdto.CheckList.UsuarioEncerramento = this._vmchecklist.Usuario;                
                }

                if (this._Sondagem.VesicalDemora.Value.Equals(SimNao.Sim)) 
                    this._Sondagem.Usuario = this._usuarios;
                
                return true;
            }
        }
    }
}
