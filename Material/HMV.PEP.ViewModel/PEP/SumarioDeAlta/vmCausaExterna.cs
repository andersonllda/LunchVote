using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmCausaExterna : ViewModelBase
    {
        #region Contrutor
        public vmCausaExterna(wrpSumarioAlta pSumarioAlta)
        {
            this.SumarioAlta = pSumarioAlta;
            this.AddCausaExternaCommand = new AddCausaExternaCommand(this);
            this.RemoveCausaExternaCommand = new RemoveCausaExternaCommand(this);

            //if (this.SumarioAlta.CausaExterna == null)
            //{
            //    //this.SumarioAlta.DomainObject.CausaExterna = new 
            //    this.SumarioAlta.CausaExterna = new wrpCausaExternaCollection(new List<CausaExterna>());
            //}

            this.CausasExternas = this.SumarioAlta.CausaExterna;
        }
        #endregion

        #region Propriedades Publicas
        public wrpSumarioAlta SumarioAlta { get; set; }

        public wrpCausaExternaCollection CausasExternas { get; set; }       

        public wrpCausaExterna CausaExternaSelecionada
        {
            get
            {
                return _causaexternaselecionada;
            }
            set
            {
                this._causaexternaselecionada = value;                
                this.OnPropertyChanged("IdCid");
                this.OnPropertyChanged("DescricaoCid");
                this.OnPropertyChanged("CausaExternaSelecionada");
            }
        }

        public string IdCid
        {
            get
            {
                if (this._causaexternaselecionada != null && this._causaexternaselecionada.Cid != null)
                    return this._causaexternaselecionada.Cid.Cid.Id;
                return string.Empty;
            }
        }

        public string DescricaoCid
        {
            get
            {
                if (this._causaexternaselecionada != null && this._causaexternaselecionada.Cid !=null)
                    return this._causaexternaselecionada.Cid.Cid.Descricao;
                return string.Empty;
            }
        }

        public SimNao NaoSeAplica
        {
            get
            {
                return this.SumarioAlta.SemCausaExterna;
            }
            set
            {
                if (value == SimNao.Sim)
                    if (this.SumarioAlta.CausaExterna == null || this.SumarioAlta.CausaExterna.Count == 0)
                        this.SumarioAlta.SemCausaExterna = value;
                    else
                    {
                        this.SumarioAlta.SemCausaExterna = SimNao.Nao;
                        DXMessageBox.Show("Não é possível marcar esta opção enquanto houver causas externas cadastradas.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                else
                    this.SumarioAlta.SemCausaExterna = value;

                this.OnPropertyChanged("NaoSeAplica");
            }
        }

        #endregion

        #region Commands
        public ICommand AddCausaExternaCommand { get; set; }
        public ICommand RemoveCausaExternaCommand { get; set; }
        #endregion

        #region Propriedades Privadas
        wrpCausaExterna _causaexternaselecionada { get; set; }
        #endregion

        #region Metodos Publicos
        public void SetaCausaExternaSelecionada(Cid pCid)
        {
            if (pCid == null)
                this.CausaExternaSelecionada = null;
            else
            {
                this.CausaExternaSelecionada = new wrpCausaExterna(pCid.CidMV);
                this.CausaExternaSelecionada.Atendimento = this.SumarioAlta.Atendimento;
            }
        }

        public void adicionaCausaExterna()
        {
            this.CausasExternas.Add(this.CausaExternaSelecionada);
            this.CausaExternaSelecionada = null;
            this.OnPropertyChanged("CausasExternas");
        }

        public void Nova()
        {
           
        }
        #endregion

       
    }
}
