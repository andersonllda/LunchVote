using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using StructureMap;
using System.Windows.Input;
using HMV.PEP.ViewModel.Commands;

namespace HMV.PEP.ViewModel.BoletimEmergencia
{
    public class vmClassificacaoRisco : ViewModelBase
    {
        #region Contrutor
        public vmClassificacaoRisco(wrpBoletimDeEmergencia pBoletim, Usuarios pUsuario, vmBoletimEmergencia pvm)
        {
            this._boletim = pBoletim;
            this._usuario = new wrpUsuarios(pUsuario);
            this._vm = pvm;
            this.AddClassificacaoRiscoCommand = new AddClassificacaoRiscoCommand(this);

            this._classificacaoselecionada = ClassificacaoAtual;
        }
        public vmClassificacaoRisco(wrpBoletimDeEmergencia pBoletim, Usuarios pUsuario)
        {
            this._boletim = pBoletim;
            this._usuario = new wrpUsuarios(pUsuario);
            this.AddClassificacaoRiscoCommand = new AddClassificacaoRiscoCommand(this);

            this._classificacaoselecionada = ClassificacaoAtual;
        }
        #endregion

        #region Propriedades Publicas

        public wrpUsuarios Usuario
        {
            get
            {
                return _usuario;
            }
        }

        public wrpCorRiscoCollection ClassificacaoRiscoCores
        {
            get
            {
                IRepositorioCorRisco rep = ObjectFactory.GetInstance<IRepositorioCorRisco>();
                return new wrpCorRiscoCollection(rep.FiltraAtivos().FiltraOrdenado().List());
            }
        }

        public wrpCorRisco ClassificacaoSelecionada
        {
            get
            {
                return _classificacaoselecionada;
            }
            set
            {
                _classificacaoselecionada = value;
                this.OnPropertyChanged("ClassificacaoSelecionada");
            }
        }

        public wrpClassificacaoRiscoCollection ClassificacaoRisco
        {
            get
            {
                return this._boletim.Classificacoes;
            }
        }        

        public wrpBoletimDeEmergencia BoletimEmergencia
        {
            get
            {
                return _boletim;
            }
            set
            {
                _boletim = value;
                this.OnPropertyChanged("BoletimEmergencia");
            }
        }

        public wrpCorRisco ClassificacaoAtual
        {
            get
            {
                if (this._boletim !=null && this._boletim.Classificacoes.Count > 0)
                    return this._boletim.Classificacoes.OrderBy(x => x.Id).Last().Cor;
                return null;
            }
        }

        public bool HabilitaSalvar
        {
            get
            {
                return this._sim || this._nao;
            }            
        }

        public bool Sim
        {
            get
            {
                return this._sim;
            }
            set
            {
                this._sim = value;
                this.OnPropertyChanged("Sim");
                this.OnPropertyChanged("HabilitaSalvar");
            }
        }

        public bool Nao
        {
            get
            {
                return this._nao;
            }
            set
            {
                this._nao = value;
                this.OnPropertyChanged("Nao");
                this.OnPropertyChanged("HabilitaSalvar");
            }
        }     
        #endregion

        #region Propriedades Privadas
        private wrpBoletimDeEmergencia _boletim { get; set; }
        private wrpCorRisco _classificacaoselecionada { get; set; }
        private wrpUsuarios _usuario { get; set; }
        private bool _sim { get; set; }
        private bool _nao { get; set; }
        private vmBoletimEmergencia _vm;
        #endregion

        #region Metodos Publicos
        public void Editou()
        {
            if ( this._vm != null ) 
                this._vm.Editou = true;
        }
        #endregion

        #region Metodos Privados - VAZIO

        #endregion

        #region Commands

        public ICommand AddClassificacaoRiscoCommand { get; set; }

        #endregion
   
    }
}
