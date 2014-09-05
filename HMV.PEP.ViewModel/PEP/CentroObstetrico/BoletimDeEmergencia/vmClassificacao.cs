using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Framework.Extensions;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Linq;
using HMV.Core.Domain.Model;
using System.Collections.Generic;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmClassificacao : ViewModelBase
    {
        #region ----- Construtor -----
        public vmClassificacao(wrpBoletimDeEmergencia pBoletim, wrpUsuarios pUsuarios, vmBoletimEmergenciaCO pvm)
        {
            this._boletim = pBoletim;
            this._vm = pvm;
            this._usuarios = pUsuarios;
            if (this._boletim.DomainObject.Classificacoes != null && this._boletim.DomainObject.Classificacoes.Count == 0 )
                this._boletim.Classificacoes = null;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpCorRiscoCollection _corriscocollection;
        private wrpBoletimDeEmergencia _boletim;
        private wrpCorRisco _corriscoselecionado;
        private vmBoletimEmergenciaCO _vm;
        private wrpUsuarios _usuarios;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCorRiscoCollection ClassificacaoRiscoCores
        {
            get
            {
                if (this._corriscocollection.IsNull())
                {
                    IRepositorioCorRisco rep = ObjectFactory.GetInstance<IRepositorioCorRisco>();
                    this._corriscocollection= new wrpCorRiscoCollection(rep.FiltraAtivos().FiltraOrdenado().List());
                }
                return this._corriscocollection;
            }
        }

        public wrpCorRisco ClassificacaoSelecionada
        {
            get
            {
                return _corriscoselecionado;
            }
            set
            {
                _corriscoselecionado = value;
                this.OnPropertyChanged<vmClassificacao>(x=> x.ClassificacaoSelecionada);
            }
        }

        public wrpClassificacaoRiscoCollection ClassificacaoRisco
        {
            get
            {
                return new wrpClassificacaoRiscoCollection(this._boletim.DomainObject.Classificacoes
                    .OrderByDescending(x => x.DataHoraInclusao).ToList());
            }
        }       
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----
        protected override void CommandSelecionar(object param)
        {
            if (this._corriscoselecionado.IsNotNull())
            {
                wrpClassificacaoRiscoCollection risco = this._boletim.Classificacoes;
                this._boletim.Classificacoes.Add(new wrpClassificacaoRisco(this._corriscoselecionado.DomainObject, this._usuarios.DomainObject, this._boletim.DomainObject));
                
                this._vm.RefreshCor();
                this.OnPropertyChanged<vmClassificacao>(x => x.ClassificacaoRisco);
            }
        }
        #endregion
    }
}
