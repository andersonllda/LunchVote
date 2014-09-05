using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using System.Linq;
using System.Collections.Generic;
using HMV.PEP.Interfaces;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmCidDiagnostico : ViewModelBase
    {
        #region ----- Construtor -----
        public vmCidDiagnostico(vmBoletimEmergenciaCO pVm)
        {
            _boletim = pVm.Boletim;
        }
        #endregion   

        #region ----- Propriedades Privadas -----
        private wrpCid _CID;
        private wrpBoletimDeEmergencia _boletim;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCid CID
        {
            get { return _CID; }
            set {
                _CID = value;
                OnPropertyChanged<vmCidDiagnostico>(x => x.CID);
            }
        }

        public List<wrpCid> CIDS
        {
            get
            {
                return (from x in _boletim.Atendimento.CIDs.ToList()
                        select new wrpCid()
                        {
                            CanDelete = !this._boletim.DataAlta.HasValue,
                            Descricao = x.Descricao,
                            Id = x.Id
                        }).ToList();
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        public void addCid(wrpCid cid, wrpUsuarios pUsuario)
        {
            cid.CanDelete = true;
            _boletim.Atendimento.DomainObject.AddCid(cid.DomainObject);
            _boletim.Save();

            ICidService sr = ObjectFactory.GetInstance<ICidService>();
            sr.verificaSeOCIDJaEstaNaListaDeProblemas(cid.DomainObject, _boletim.Atendimento.DomainObject, pUsuario.DomainObject);

            OnPropertyChanged<vmCidDiagnostico>(x => x.CIDS);
            CID = null; 
        }
        public void RemoveCID(wrpCid cid){
            _boletim.Atendimento.DomainObject.RemoveCid(_boletim.Atendimento.DomainObject.Cids().FirstOrDefault(x=>x.Id == cid.Id));
            _boletim.Save();
            OnPropertyChanged<vmCidDiagnostico>(x => x.CIDS);
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
