using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using System.Linq;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Collections.Generic;
using System;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmOrientacoesDoMedicoAssistente : ViewModelBase
    {
        #region ----- Construtor -----
        public vmOrientacoesDoMedicoAssistente(vmBoletimEmergenciaCO pVm)
        {
            pVm.DictionaryCO.Add(vmBoletimEmergenciaCO.TabsBoletimEmergenciaCO.OrientacoesDoMedicoAssistente, this);
            _boletim = pVm.Boletim;
            _boletimCO = pVm.BoletimCO;
            _usuario = pVm.Usuarios;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpBoletimDeEmergencia _boletim;
        private wrpBoletimCentroObstetrico _boletimCO;
        private wrpOrientacaoMedica _orientacao;
        private wrpUsuarios _usuario;
        #endregion

        #region ----- Propriedades Públicas -----
        public string Descricao
        {
            get
            {
                return _orientacao.IsNull() ? "" : _orientacao.Descricao;
            }
            set
            {
                if (value.IsEmptyOrWhiteSpace() && this.Prestador.IsNull())
                {
                    if (_orientacao.IsNotNull())
                    {
                        _boletimCO.DomainObject.Orientacoes.Remove(_orientacao.DomainObject);
                        _boletimCO.Orientacoes.Remove(_orientacao);
                        _orientacao = null;
                    }
                }
                else
                {
                    if (_orientacao.IsNull())
                    {
                        _orientacao = new wrpOrientacaoMedica(_usuario);
                        _orientacao.BoletimDeEmergencia = _boletim;

                        _boletimCO.Orientacoes.Add(_orientacao);
                        OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.Orientacoes);
                    }
                    _orientacao.Descricao = value;

                }
                OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.Descricao);
                OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.Orientacoes);
            }
        }

        public SimNao MedicoNaoDefinido
        {
            get { return _orientacao.IsNull() ? SimNao.Nao : _orientacao.MedicoNaoDefinido; }
            set
            {
                if (value == SimNao.Sim)
                {
                    if (Descricao.IsNotEmptyOrWhiteSpace())
                    {
                        if (DXMessageBox.Show("As informações digitadas na descrião abaixo serão perdidos, deseja continuar ?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            _orientacao.MedicoNaoDefinido = SimNao.Nao;
                            OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.MedicoNaoDefinido);
                            return;
                        }
                    }
                    Descricao = "Medico ainda não definido pelo paciente";
                    Prestador = null;
                }
                else
                    Descricao = "";

                if (_orientacao.IsNotNull())
                    _orientacao.MedicoNaoDefinido = value;

                OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.MedicoNaoDefinido);
                OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.Orientacoes);
            }
        }

        public wrpPrestador Prestador
        {
            get { return _orientacao.IsNull() ? null : _orientacao.Prestador; }
            set
            {
                if (value == null && string.IsNullOrWhiteSpace(this.Descricao))
                {
                    if (_orientacao.IsNotNull())
                    {
                        _boletimCO.DomainObject.Orientacoes.Remove(_orientacao.DomainObject);
                        _boletimCO.Orientacoes.Remove(_orientacao);
                        _orientacao = null;
                    }
                }
                else
                {
                    if (_orientacao.IsNull())
                    {
                        _orientacao = new wrpOrientacaoMedica(_usuario);
                        _orientacao.BoletimDeEmergencia = _boletim;

                        _boletimCO.Orientacoes.Add(_orientacao);
                        OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.Orientacoes);
                    }


                    _orientacao.Prestador = value;
                }

                OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.Orientacoes);
                OnPropertyChanged<vmOrientacoesDoMedicoAssistente>(x => x.Prestador);
            }

        }

        //public wrpOrientacaoMedicaCollection Orientacoes
        //{
        //    get
        //    {
        //        return new wrpOrientacaoMedicaCollection(_boletimCO.DomainObject.Orientacoes.OrderByDescending(x => x.DataInclusao).ToList());
        //    }
        //}

        public IList<BoletimCOHistoricoDTO> Orientacoes
        {
            get
            {
                IList<BoletimCOHistoricoDTO> lista = new List<BoletimCOHistoricoDTO>();

                foreach (var item in new wrpOrientacaoMedicaCollection(_boletimCO.DomainObject.Orientacoes.OrderByDescending(x => x.DataInclusao).ToList()))
                {
                    lista.Add(new BoletimCOHistoricoDTO(true, "Data/Hora: " + Environment.NewLine + "Usuário: ", item.DataInclusao.ToShortDateString() + " " + item.DataInclusao.ToShortTimeString() + Environment.NewLine + item.Usuario.AssinaturaNaLinha));
                    if ( item.Prestador != null ) 
                        lista.Add(new BoletimCOHistoricoDTO("Médico Assistente:", item.Prestador.Nome, true));
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
