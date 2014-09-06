using System.Collections.Generic;
using System.Windows.Input;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Wrappers;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmEvolucaoPadrao : ViewModelBase
    {
        #region Contrutor
        public vmEvolucaoPadrao(Prestador pPrestador, wrpSumarioAlta pSumarioAlta)
        {            
            this._sumarioalta = pSumarioAlta;
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            rep.Refresh(pPrestador);
            this._prestador = new wrpPrestador(pPrestador);
            this.Inicializa();
        }
        public vmEvolucaoPadrao(Prestador pPrestador, string pDescricao)
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            rep.Refresh(pPrestador);
            this._prestador = new wrpPrestador(pPrestador);
            this._evolucaopadraoselecionada = new wrpEvolucaoPadrao(string.Empty, pDescricao);
            this._evolucaopadraoselecionada.Prestador = this.Prestador;
            this._evolucaopadraoselecionada2 = this._evolucaopadraoselecionada;
            this.Inicializa();
        }

        private void Inicializa()
        {
            if (this.Prestador.EvolucaoPadrao == null)
                Prestador.EvolucaoPadrao = new wrpEvolucaoPadraoCollection(new List<EvolucaoPadrao>());

            this.EvolucoesPadrao = Prestador.EvolucaoPadrao;
            this.SavePrestadorCommand = new SavePrestadorCommand(this);
            this.AddEvolucaoPadraoCommand = new AddEvolucaoPadraoCommand(this);
            this.RemoveEvolucaoPadraoCommand = new RemoveEvolucaoPadraoCommand(this);
        }
        #endregion

        #region Propriedades Publicas
        public wrpPrestador Prestador { get {return _prestador; } }
        public wrpEvolucaoPadraoCollection EvolucoesPadrao { get; set; }
        public wrpEvolucaoPadrao EvolucaoSelecionada
        {
            get
            {
                return _evolucaopadraoselecionada;
            }
            set
            {
                _evolucaopadraoselecionada = value;                
                this.OnPropertyChanged("EvolucaoSelecionada");
            }
        }
        #endregion

        #region Commands
        public ICommand SavePrestadorCommand { get; set; }
        public ICommand AddEvolucaoPadraoCommand { get; set; }
        public ICommand RemoveEvolucaoPadraoCommand { get; set; }
        #endregion

        #region Metodos
        public void SetaEvolucaoPadrao()
        {
            if (_sumarioalta == null || this.EvolucaoSelecionada == null) return;
            _sumarioalta.Evolucao = this.EvolucaoSelecionada.Descricao;
        }
        #endregion

        #region Propriedades Privadas
        private wrpSumarioAlta _sumarioalta { get; set; }
        private wrpEvolucaoPadrao _evolucaopadraoselecionada { get; set; }
        private wrpEvolucaoPadrao _evolucaopadraoselecionada2 { get; set; }
        private wrpPrestador _prestador { get; set; }
        #endregion
    }
}
