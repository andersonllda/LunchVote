using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Collections.Generic;
using HMV.Core.Framework.Extensions;
using StructureMap;
using System.Linq;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using System;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmMotivoConsulta : ViewModelBase
    {
        #region ----- Construtor -----
        public vmMotivoConsulta(vmBoletimEmergenciaCO pVm)
        {
            pVm.DictionaryCO.Add(vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.MotivoConsulta, this);
            _boletim = pVm.Boletim;
            _usuario = pVm.Usuarios;
        }
        #endregion    

        #region ----- Propriedades Privadas -----
        private wrpBoletimDeEmergencia _boletim;
        private wrpBoletimAvaliacao _boletimAvaliacao;
        private wrpUsuarios _usuario;
        #endregion

        #region ----- Propriedades Públicas -----
        public string Descricao
        {
            get {
                return _boletimAvaliacao.IsNull() ? "" : _boletimAvaliacao.Texto;
            }
            set
            {
                if (value.IsEmptyOrWhiteSpace() && _boletimAvaliacao.IsNotNull() )
                {
                    _boletim.DomainObject.BoletimAvaliacao.Remove(_boletimAvaliacao.DomainObject);
                    _boletim.BoletimAvaliacao.Remove(_boletimAvaliacao);
                    _boletimAvaliacao = null;
                    OnPropertyChanged<vmMotivoConsulta>(x => x.Motivos);
                }
                else
                {
                    if (_boletimAvaliacao.IsNull())
                    {
                        _boletimAvaliacao = new wrpBoletimAvaliacao(_boletim.DomainObject, _usuario.DomainObject);

                        var tipos = ObjectFactory.GetInstance<IRepositorioDeTipoAvaliacao>().List();
                        _boletimAvaliacao.TipoAvaliacao = new wrpTipoAvaliacao(tipos.Where(x => x.ID == 1).FirstOrDefault());
                        _boletim.BoletimAvaliacao.Add(_boletimAvaliacao);
                        OnPropertyChanged<vmMotivoConsulta>(x => x.Motivos);
                    }
                    _boletimAvaliacao.Texto = value;
                }
                OnPropertyChanged<vmMotivoConsulta>(x => x.Descricao);
                OnPropertyChanged<vmMotivoConsulta>(x => x.Motivos);
            }
        }

        //public wrpBoletimAvaliacaoCollection Motivos
        //{
        //    get {
        //        return _boletim.MotivoConsultaCO;
        //    }
        //}

        public IList<BoletimCOHistoricoDTO> Motivos
        {
            get {
                IList<BoletimCOHistoricoDTO> lista = new List<BoletimCOHistoricoDTO>();

                foreach (var item in _boletim.MotivoConsultaCO)
                {
                    lista.Add(new BoletimCOHistoricoDTO(true, "Data/Hora: " + Environment.NewLine + "Usuário: ", item.DataInclusao.ToShortDateString() + " " + item.HoraInclusao + Environment.NewLine + item.Usuario.AssinaturaNaLinha));
                    lista.Add(new BoletimCOHistoricoDTO("Descrição:", item.Texto, true));
                }
                
                return lista;
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
