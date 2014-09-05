using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using HMV.Core.Wrappers.CollectionWrappers;
using StructureMap;
using HMV.Core.Domain.Repository;
using System.Linq;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico;
using System.Collections.Generic;
using System;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmCondutaEReavaliacao : ViewModelBase
    {
        #region ----- Construtor -----
        public vmCondutaEReavaliacao(vmBoletimEmergenciaCO pVm)
        {
            pVm.DictionaryCO.Add(vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.CondutaEReavaliacoes, this);
            _boletim = pVm.Boletim;
            _boletimCO = pVm.BoletimCO;
            _usuario = pVm.Usuarios;
        }
        #endregion      

        #region ----- Propriedades Privadas -----
        private wrpBoletimDeEmergencia _boletim;
        private wrpBoletimCentroObstetrico _boletimCO;
        private wrpConduta _conduta;
        private wrpUsuarios _usuario;
        #endregion

        #region ----- Propriedades Públicas -----
        //public wrpCondutaCollection Condutas
        //{
        //    get
        //    {
        //        return new wrpCondutaCollection(this._boletimCO.DomainObject.Condutas.OrderByDescending(x=>x.DataInclusao).ToList());
        //    }
        //}

        public IList<BoletimCOHistoricoDTO> Condutas
        {
            get
            {
                IList<BoletimCOHistoricoDTO> lista = new List<BoletimCOHistoricoDTO>();

                foreach (var item in new wrpCondutaCollection(this._boletimCO.DomainObject.Condutas.OrderByDescending(x => x.DataInclusao).ToList()))
                {
                    lista.Add(new BoletimCOHistoricoDTO(true, "Data/Hora: " + Environment.NewLine + "Usuário: ", item.DataInclusao.ToShortDateString() + " " + item.DataInclusao.ToShortTimeString() + Environment.NewLine + item.Usuario.AssinaturaNaLinha));
                    lista.Add(new BoletimCOHistoricoDTO("Descrição:", item.Descricao, true));
                }

                return lista;
            }
        }

        public string Descricao
        {
            get
            {
                return _conduta.IsNull() ? "" : _conduta.Descricao;
            }
            set
            {
                if (_conduta.IsNull())
                {
                    _conduta = new wrpConduta(_usuario);
                    _conduta.BoletimDeEmergencia = _boletim;
                    _boletimCO.Condutas.Add(_conduta);
                    OnPropertyChanged<vmCondutaEReavaliacao>(x => x.Condutas);
                }
                _conduta.Descricao = value;

                if (value.IsEmptyOrWhiteSpace())
                {
                    _boletimCO.Condutas.Remove(_conduta);
                    _conduta = null;
                    OnPropertyChanged<vmCondutaEReavaliacao>(x => x.Condutas);
                }

                OnPropertyChanged<vmCondutaEReavaliacao>(x => x.Descricao);
                OnPropertyChanged<vmCondutaEReavaliacao>(x => x.Condutas);

            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
