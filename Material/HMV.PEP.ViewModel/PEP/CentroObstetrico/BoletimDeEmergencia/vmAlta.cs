using HMV.Core.Framework.ViewModelBaseClasses;
using System.Windows;
using System.Collections;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Collections.Generic;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Framework.Extensions;
using StructureMap;
using HMV.Core.Domain.Repository.ClassificacaoPaciente;
using System.Linq;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Enum;
using DevExpress.Xpf.Core;
using System;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmAlta : ViewModelBase
    {
        #region ----- Construtor -----
        public vmAlta(wrpBoletimDeEmergencia pBoletim, wrpUsuarios pUsu)
        {
            _boletim = pBoletim;
            _usuario = pUsu;
            if ( _boletim.AltaCO == null )
                _boletim.AltaCO = new wrpAltaCentroObstetrico(pUsu);

            HoraRetorno = null;
        }
        #endregion    

        #region ----- Propriedades Privadas -----
        private wrpBoletimDeEmergencia _boletim;
        private wrpUsuarios _usuario;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpAltaCentroObstetrico Alta
        {
            get {
                return _boletim.AltaCO; 
            }
            set {
                _boletim.AltaCO = value;
                OnPropertyChanged<vmAlta>(x => x.Alta);
            }
        }

        public string Condicao
        {
            get
            {
                return _boletim.AltaCO.Condicao;
            }
            set
            {
                _boletim.AltaCO.Condicao = value;
                OnPropertyChanged<vmAlta>(x => x.Condicao);
            }
        }

        public string Orientacao
        {
            get
            {
                return _boletim.AltaCO.Orientacao;
            }
            set
            {
                _boletim.AltaCO.Orientacao = value;
                OnPropertyChanged<vmAlta>(x => x.Orientacao);
            }
        }

        public string ExameObservacao
        {
            get
            {
                return _boletim.AltaCO.ExameObservacao;
            }
            set
            {
                _boletim.AltaCO.ExameObservacao = value;
                OnPropertyChanged<vmAlta>(x => x.ExameObservacao);
            }
        }

        public decimal? HoraRetorno
        {
            get
            {
                return _boletim.AltaCO.HoraRetorno;
            }
            set
            {
                _boletim.AltaCO.HoraRetorno = value;
                OnPropertyChanged<vmAlta>(x => x.HoraRetorno);
            }
        }

        private bool _exameEntregue = false; 
        public bool ExameEntregueSim
        {
            get
            {
                return !_exameEntregue ? false : _boletim.AltaCO.ExameEntregue == SimNao.Sim;
            }
            set
            {
                if (value)
                {
                    _boletim.AltaCO.ExameObservacao = string.Empty;
                    _exameEntregue = true;
                    _boletim.AltaCO.ExameEntregue = SimNao.Sim;
                }

                OnPropertyChanged<vmAlta>(x => x.ExameEntregueSim);
                OnPropertyChanged<vmAlta>(x => x.ExameEntregueNao);
                OnPropertyChanged<vmAlta>(x => x.ExameObservacao);
            }
        }

        public bool ExameEntregueNao
        {
            get
            {
                return !_exameEntregue ? false : _boletim.AltaCO.ExameEntregue == SimNao.Nao;
            }
            set
            {
                if (value)
                {
                    _boletim.AltaCO.ExameObservacao = string.Empty;
                    _exameEntregue = true;
                    _boletim.AltaCO.ExameEntregue = SimNao.Nao;
                }

                OnPropertyChanged<vmAlta>(x => x.ExameEntregueSim);
                OnPropertyChanged<vmAlta>(x => x.ExameEntregueNao);
                OnPropertyChanged<vmAlta>(x => x.ExameObservacao);
            }
        }             

        public bool DestinoDomicilio
        {
            get { return _boletim.AltaCO.DestinoAlta == DestinoAltaCO.Domicilio; }
            set { 
                if ( value )
                    _boletim.AltaCO.DestinoAlta = DestinoAltaCO.Domicilio;
                else
                    _boletim.AltaCO.DestinoAlta = null;

                HoraRetorno = null;
                OnPropertyChanged<vmAlta>(x => x.DestinoOutroHospital);
                OnPropertyChanged<vmAlta>(x => x.DestinoInternacao);
                OnPropertyChanged<vmAlta>(x => x.DestinoDomicilio);
                OnPropertyChanged<vmAlta>(x => x.DestinoRetornoEm);
            }
        }

        public bool DestinoInternacao
        {
            get { return _boletim.AltaCO.DestinoAlta == DestinoAltaCO.Internacao; }
            set
            {
                if (value)
                    _boletim.AltaCO.DestinoAlta = DestinoAltaCO.Internacao;
                else
                    _boletim.AltaCO.DestinoAlta = null;

                HoraRetorno = null;
                OnPropertyChanged<vmAlta>(x => x.DestinoOutroHospital);
                OnPropertyChanged<vmAlta>(x => x.DestinoInternacao);
                OnPropertyChanged<vmAlta>(x => x.DestinoDomicilio);
                OnPropertyChanged<vmAlta>(x => x.DestinoRetornoEm);
            }
        }

        public bool DestinoOutroHospital
        {
            get { return _boletim.AltaCO.DestinoAlta == DestinoAltaCO.OutroHospital; }
            set
            {
                if (value)
                    _boletim.AltaCO.DestinoAlta = DestinoAltaCO.OutroHospital;
                else
                    _boletim.AltaCO.DestinoAlta = null;

                HoraRetorno = null;
                OnPropertyChanged<vmAlta>(x => x.DestinoOutroHospital);
                OnPropertyChanged<vmAlta>(x => x.DestinoInternacao);
                OnPropertyChanged<vmAlta>(x => x.DestinoDomicilio);
                OnPropertyChanged<vmAlta>(x => x.DestinoRetornoEm);
            }
        }

        public bool DestinoRetornoEm
        {
            get { return _boletim.AltaCO.DestinoAlta == DestinoAltaCO.RetornoEm; }
            set
            {
                if (value)
                    _boletim.AltaCO.DestinoAlta = DestinoAltaCO.RetornoEm;
                else
                    _boletim.AltaCO.DestinoAlta = null;

                OnPropertyChanged<vmAlta>(x => x.DestinoOutroHospital);
                OnPropertyChanged<vmAlta>(x => x.DestinoInternacao);
                OnPropertyChanged<vmAlta>(x => x.DestinoDomicilio);
                OnPropertyChanged<vmAlta>(x => x.DestinoRetornoEm);
            }
        }

        public wrpBoletimDeEmergencia Boletim
        {
            get { return _boletim; }
        }
        #endregion

        #region ----- Métodos Privados -----
        #endregion

        #region ----- Métodos Públicos -----     
        #endregion

        #region ----- Commands -----
        protected override void CommandSalvar(object param)
        {
        //    if (string.IsNullOrWhiteSpace(_boletim.AltaCO.Condicao))
        //    {
        //        DXMessageBox.Show("Condição deve ser informada.","Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
        //        return;
        //    }

        //    if (!_exameEntregue)
        //    {
        //        DXMessageBox.Show("Deve ser macado [Sim] ou [Não] para Exames Entregues.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
        //        return;
        //    }

        //    if (_boletim.AltaCO.ExameEntregue == SimNao.Sim && string.IsNullOrWhiteSpace(_boletim.AltaCO.ExameObservacao))
        //    {
        //        DXMessageBox.Show("Exames Entregues Paciente deve ser informado.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
        //        return;
        //    }

        //    if (!_boletim.AltaCO.DestinoAlta.HasValue)
        //    {
        //        DXMessageBox.Show("Destino alta deve ser informado.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
        //        return;
        //    }

        //    if (_boletim.AltaCO.DestinoAlta.HasValue && _boletim.AltaCO.DestinoAlta.Value == DestinoAltaCO.RetornoEm && !_boletim.AltaCO.HoraRetorno.HasValue)
        //    {
        //        DXMessageBox.Show("Hora Retorno deve ser informado.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
        //        return;
        //    }

            _boletim.DataAlta = DateTime.Now;
            _boletim.UsuarioAlta = _usuario;

            _boletim.Save();

            _boletim.Atendimento.DataAlta = DateTime.Now;
            _boletim.Atendimento.HoraAlta = DateTime.Now;
            _boletim.Atendimento.Save();
            
            base.CommandSalvar(param);
        }

        public bool Valida
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_boletim.AltaCO.Condicao))
                {
                    DXMessageBox.Show("Condição deve ser informada.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }

                if (!_exameEntregue)
                {
                    DXMessageBox.Show("Deve ser macado [Sim] ou [Não] para Exames Entregues.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }

                if (_boletim.AltaCO.ExameEntregue == SimNao.Sim && string.IsNullOrWhiteSpace(_boletim.AltaCO.ExameObservacao))
                {
                    DXMessageBox.Show("Exames Entregues Paciente deve ser informado.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }

                if (!_boletim.AltaCO.DestinoAlta.HasValue)
                {
                    DXMessageBox.Show("Destino alta deve ser informado.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }

                if (_boletim.AltaCO.DestinoAlta.HasValue && _boletim.AltaCO.DestinoAlta.Value == DestinoAltaCO.RetornoEm && !_boletim.AltaCO.HoraRetorno.HasValue)
                {
                    DXMessageBox.Show("Hora Retorno deve ser informado.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }

                return true;
            }
        }

        public void Salvar()
        {
            CommandSalvar(null);
        }

        protected override void CommandFechar(object param)
        {
            IRepositorioDeBoletimDeEmergencia rep = ObjectFactory.GetInstance<IRepositorioDeBoletimDeEmergencia>();
            _boletim.AltaCO = null;
            _boletim = new wrpBoletimDeEmergencia(rep.OndeIdIgual(_boletim.Id).Single());

            base.CommandFechar(param);
        }
        #endregion
    }
}
