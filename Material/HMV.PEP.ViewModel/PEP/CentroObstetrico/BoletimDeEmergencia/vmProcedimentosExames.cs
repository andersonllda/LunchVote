using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using System.Linq;
using StructureMap;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico;
using HMV.Core.Domain.Enum;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Collections.Generic;
using System;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmProcedimentosExames : ViewModelBase
    {
        #region ----- Construtor -----
        public vmProcedimentosExames(vmBoletimEmergenciaCO pVm)
        {
            pVm.DictionaryCO.Add(vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.ProcedimentosExames, this);
            _boletim = pVm.Boletim;
            _boletimCO = pVm.BoletimCO;
            _usuario = pVm.Usuarios;
        }
        #endregion    

        #region ----- Propriedades Privadas -----
        private wrpBoletimDeEmergencia _boletim;
        private wrpBoletimCentroObstetrico _boletimCO;
        private wrpProcedimentoCentroObstetrico _procedimento;
        private wrpUsuarios _usuario;
        #endregion

        #region ----- Propriedades Públicas -----
        public string Descricao
        {
            get {
                return _procedimento.IsNull() ? "" : _procedimento.Descricao;
            }
            set
            {
                if (_procedimento.IsNull())
                {
                    _procedimento = new wrpProcedimentoCentroObstetrico(_usuario);
                    _procedimento.BoletimDeEmergencia = _boletim;

                    _boletimCO.Procedimentos.Add(_procedimento);
                    OnPropertyChanged<vmProcedimentosExames>(x => x.Procedimentos);
                }
                _procedimento.Descricao = value;

                if (value.IsEmptyOrWhiteSpace())
                {
                    _boletimCO.Procedimentos.Remove(_procedimento);
                    _procedimento = null; 
                    OnPropertyChanged<vmProcedimentosExames>(x => x.Procedimentos);
                }

                OnPropertyChanged<vmProcedimentosExames>(x => x.Descricao);
                OnPropertyChanged<vmProcedimentosExames>(x => x.Procedimentos);
            }
        }

        public SimNao Realizado
        {
            get { return _procedimento.IsNull() ? SimNao.Nao : _procedimento.Realizado; }
            set {
                
                if (value == SimNao.Sim)
                {
                    if (Descricao.IsNotEmptyOrWhiteSpace())
                    {
                        if (DXMessageBox.Show("As informações digitadass na descrião abaixo serão perdidos, deseja continuar ?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            _procedimento.Realizado = SimNao.Nao;
                            OnPropertyChanged<vmProcedimentosExames>(x => x.Realizado);
                            return;
                        }

                    }
                    Descricao = "Não Realizado";
                }
                else
                    Descricao = "";


                if (_procedimento.IsNotNull())
                    _procedimento.Realizado = value;

                OnPropertyChanged<vmProcedimentosExames>(x => x.Realizado);
                OnPropertyChanged<vmProcedimentosExames>(x => x.Procedimentos);

            }
        }
             
        /*public wrpProcedimentoCentroObstetricoCollection Procedimentos
        {
            get {
                return new wrpProcedimentoCentroObstetricoCollection(_boletimCO.DomainObject.Procedimentos.OrderByDescending(x=>x.DataInclusao).ToList());
            }
        }*/

        public IList<BoletimCOHistoricoDTO> Procedimentos
        {
            get
            {
                IList<BoletimCOHistoricoDTO> lista = new List<BoletimCOHistoricoDTO>();

                foreach (var item in new wrpProcedimentoCentroObstetricoCollection(_boletimCO.DomainObject.Procedimentos.OrderByDescending(x => x.DataInclusao).ToList()))
                {
                    lista.Add(new BoletimCOHistoricoDTO(true, "Data/Hora: " + Environment.NewLine + "Usuário: ", item.DataInclusao.ToShortDateString() + " " + item.DataInclusao.ToShortTimeString() + Environment.NewLine + item.Usuario.AssinaturaNaLinha));
                    lista.Add(new BoletimCOHistoricoDTO("Descrição:", item.Descricao, true));
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
