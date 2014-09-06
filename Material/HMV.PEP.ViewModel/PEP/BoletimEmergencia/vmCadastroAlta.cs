using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using StructureMap;
using System;
using System.Linq;
using DevExpress.Xpf.Core;
using System.Windows;


namespace HMV.PEP.ViewModel.BoletimEmergencia
{
    public class vmCadastroAlta : ViewModelBase
    {
        #region Contrutor
        public vmCadastroAlta(vmBoletimEmergencia pvmBoletimEmergencia)
        {
            this._vmboletimemergencia = pvmBoletimEmergencia;
        }
        #endregion

        #region Propriedades Publicas
        public bool InternacaoVisible { get; set; }

        public wrpMeioDeTransporteCollection MeioTransportes
        {
            get
            {
                IRepositorioDeMeioDeTransporte rep = ObjectFactory.GetInstance<IRepositorioDeMeioDeTransporte>();
                return new wrpMeioDeTransporteCollection(rep.List());
            }
        }

        public wrpMeioDeTransporte MeioTransporteSelecionado
        {
            get
            {
                if (this._vmboletimemergencia.BoletimEmergencia != null)
                    return this._vmboletimemergencia.BoletimEmergencia.MeioTransporte;
                return null;
            }
            set
            {
                if (value != null)
                    this._vmboletimemergencia.BoletimEmergencia.MeioTransporte = value;
            }
        }

        public vmSinaisVitais vmSinaisVitais
        {
            get
            {
                return this._vmboletimemergencia.vmSinaisVitais;
            }
        }

        public wrpBoletimDeEmergencia BoletimEmergencia
        {
            get
            {
                return this._vmboletimemergencia.BoletimEmergencia;
            }
        }

        public vmBoletimEmergencia vmBoletimEmergencia
        {
            get
            {
                return this._vmboletimemergencia;
            }
        }

        public bool HabilitaSalvar
        {
            get { return this._habilitasalvar; }
            set
            {
                this._habilitasalvar = value;
                this.OnPropertyChanged("HabilitaSalvar");
            }
        }

        public wrpAltaDestinoCollection AltaDestinoCollection
        {
            get
            {
                IRepositorioDeAltaDestino rep = ObjectFactory.GetInstance<IRepositorioDeAltaDestino>();
                return new wrpAltaDestinoCollection(rep.List());
            }
        }

        public wrpAltaDestino AltaDestino
        {
            get { return this._vmboletimemergencia.BoletimEmergencia.AltaDestino; }
            set
            {
                this._vmboletimemergencia.BoletimEmergencia.AltaDestino = value;

                if (value != null)
                    this._habilitasalvar = true;

                this.OnPropertyChanged("InternacaoVisible");
                this.OnPropertyChanged("HabilitaSalvar");
                this.OnPropertyChanged("TransferenciaVisible");
                this.OnPropertyChanged("AltaDestino");
            }
        }

        public bool TransferenciaVisible
        {
            get { return this._vmboletimemergencia.BoletimEmergencia.AltaDestino != null && this._vmboletimemergencia.BoletimEmergencia.AltaDestino.Descricao == "Transferência"; }
        }
        #endregion

        #region Metodos Privados
        private CadastroAltaDestino _destinoselecionado { get; set; }
        private vmBoletimEmergencia _vmboletimemergencia { get; set; }
        private bool _habilitasalvar { get; set; }
        #endregion

        #region Propriedades Privadas

        #endregion

        #region Metodos Publicos
        public bool Salva()
        {
            if (this.IsValid)
            {
                this.BoletimEmergencia.DataAlta = DateTime.Now;
                this.BoletimEmergencia.Atendimento.DataAlta = DateTime.Now.Date;
                this.BoletimEmergencia.Atendimento.HoraAlta = DateTime.Now;
                this._vmboletimemergencia.BoletimEmergencia.UsuarioAlta = new wrpUsuarios(this._vmboletimemergencia.Usuarios);
                this._vmboletimemergencia.SalvaBoletim();
                this._vmboletimemergencia.MostraRelatorio();
                return true;
            }

            return false;
        }
        #endregion

        #region Commands
        //public ICommand AddBoletimAvaliacaoCommand { get; set; }
        #endregion

        public override bool IsValid
        {
            get
            {
                if (this._destinoselecionado == CadastroAltaDestino.Internacao || this._destinoselecionado == CadastroAltaDestino.Transferencia)
                {
                    if (this.vmSinaisVitais.SinaisVitais.Count > 0)
                    {
                        var sinal = this.vmSinaisVitais.SinaisVitais.OrderBy(x => x.Data).Last();
                        TimeSpan diffResult = DateTime.Now.Subtract(sinal.Data);
                        if (diffResult.TotalHours > 1)
                        {
                            DXMessageBox.Show("Informe os sinais vitais com até uma hora da alta.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                            return false;
                        }
                    }
                    else
                    {
                        DXMessageBox.Show("Informe os sinais vitais com até uma hora da alta.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                }

                if (this._destinoselecionado == CadastroAltaDestino.Transferencia)
                {
                    if (this._vmboletimemergencia.BoletimEmergencia.MeioTransporte == null)
                    {
                        DXMessageBox.Show("Informe o meio de transporte.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                    if (string.IsNullOrEmpty(this._vmboletimemergencia.BoletimEmergencia.MedicoInstituicaoDestino))
                    {
                        DXMessageBox.Show("Informe o médico instituição de ensino.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                    if (string.IsNullOrEmpty(this._vmboletimemergencia.BoletimEmergencia.MunicipioDestino))
                    {
                        DXMessageBox.Show("Informe o município de destino.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
