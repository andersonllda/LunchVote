using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Extensions;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using StructureMap;
using System.Windows.Input;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Framework.Types;
using System.ComponentModel;

namespace HMV.PEP.ViewModel.UTINEOFolhaP
{
    public class vmUTINEOFolhaParada : ViewModelBase
    {
        #region Construtor
        public vmUTINEOFolhaParada(Atendimento pAtendimento)
        {
            if (pAtendimento.UTINEOFolhaParada == null)
            {
                pAtendimento.UTINEOFolhaParada = new List<UTINEOFolhaParada>();
            }

            this._atendimento = new wrpAtendimento(pAtendimento);

            this._uitneofolhaparadas = this._atendimento.UTINEOFolhaParada;

            HabilitaSalvar = false;
        }
        #endregion

        #region Propriedades Publicas
        public wrpUTINEOFolhaParadaCollection UTINEOFolhaParadas
        {
            get { return this._uitneofolhaparadas; }
        }
        public wrpUTINEOFolhaParada UTINEOFolhaparadaSelecionada
        {
            get { return this._utineofolhaparadaselecionada; }
            set
            {
                this._utineofolhaparadaselecionada = value;
                this.OnPropertyChanged<vmUTINEOFolhaParada>(x => x.UTINEOFolhaparadaSelecionada);
            }
        }
        public wrpAtendimento Atendimento
        {
            get { return this._atendimento; }
            set
            {
                this._atendimento = value;
                this.OnPropertyChanged<vmUTINEOFolhaParada>(x => x.Atendimento);
            }
        }
        public wrpPaciente Paciente
        {
            get { return this._atendimento.Paciente; }
        }

        [Range(0, 70, Message = "O valor para peso deve ser entre 0 e 70")]
        public double PesoInformado
        {
            get { return this._pesoinformado; }
            set
            {
                this._pesoinformado = value;
                if (this._utineofolhaparadaselecionada != null)
                    this._utineofolhaparadaselecionada.Peso = value;

                if (value >= 0 && value <= 70)
                    this._habilitacalcular = true;
                else
                    this._habilitacalcular = false;

                HabilitaSalvar = false;
                this.OnPropertyChanged<vmUTINEOFolhaParada>(x => x.PesoInformado);
                this.OnPropertyChanged<vmUTINEOFolhaParada>(x => x.HabilitaCalcular);
            }
        }

        public bool HabilitaCalcular
        {
            get { return this._habilitacalcular; }
        }

        public bool HabilitaSalvar { get; set; }

        //public string Idade
        //{
        //    get
        //    {
        //        return this._atendimento.Paciente.Idade.GetDate(); //ToString(2);
        //    }
        //}
        #endregion

        #region Propriedades Privadas
        private wrpUTINEOFolhaParadaCollection _uitneofolhaparadas { get; set; }
        private wrpUTINEOFolhaParada _utineofolhaparadaselecionada { get; set; }

        private wrpAtendimento _atendimento { get; set; }
        private double _pesoinformado { get; set; }


        private bool _habilitacalcular { get; set; }
        #endregion

        #region Metodos Publicos
        public void Calcula()
        {
            this._utineofolhaparadaselecionada.DR_SF = Math.Round(this._pesoinformado * 10,2);
            this._utineofolhaparadaselecionada.DR_AD = Math.Round(this._pesoinformado * 0.1, 2);
            this._utineofolhaparadaselecionada.DR_BS = Math.Round(this._pesoinformado * 2, 2);

            this._utineofolhaparadaselecionada.IC_DO = Math.Round(5 * this._pesoinformado * 24 * 60 / 5000, 2);
            this._utineofolhaparadaselecionada.IC_DO_GLI = Math.Round(12 - this._utineofolhaparadaselecionada.IC_DO, 2);
            this._utineofolhaparadaselecionada.IC_DB = Math.Round(15 * this._pesoinformado * 24 * 60 / 12500, 2);
            this._utineofolhaparadaselecionada.IC_DB_GLI = Math.Round(12 - this._utineofolhaparadaselecionada.IC_DB, 2);
            this._utineofolhaparadaselecionada.IC_AD = Math.Round(0.1 * this._pesoinformado * 24 * 60 / 1000, 2);
            this._utineofolhaparadaselecionada.IC_AD_GLI = Math.Round(12 - this._utineofolhaparadaselecionada.IC_AD, 2);

            this._utineofolhaparadaselecionada.DE_PD = Math.Round(2 * this._pesoinformado, 2);
            this._utineofolhaparadaselecionada.DE_SD = Math.Round(4 * this._pesoinformado, 2);

            this._utineofolhaparadaselecionada.CA_AM = Math.Round(5 * this._pesoinformado / 50, 2);
            this._utineofolhaparadaselecionada.CA_AD = Math.Round(50 * this._pesoinformado / 3000, 2);

            this._utineofolhaparadaselecionada.AN_NA = Math.Round(0.1 * this._pesoinformado / 0.4, 2);
            this._utineofolhaparadaselecionada.AN_FL = Math.Round(0.01 * this._pesoinformado / 0.1, 2);

            this._utineofolhaparadaselecionada.RM_PA = Math.Round(0.1 * this._pesoinformado / 2, 2);
            this._utineofolhaparadaselecionada.RM_MI = Math.Round(0.1 * this._pesoinformado, 2);
            this._utineofolhaparadaselecionada.RM_DI = Math.Round(0.5 * this._pesoinformado / 5, 2);
            this._utineofolhaparadaselecionada.RM_MO = Math.Round(0.1 * this._pesoinformado, 2);

            this._utineofolhaparadaselecionada.AT_FE = Math.Round(20 * this._pesoinformado / 100, 2);
            this._utineofolhaparadaselecionada.AT_FN = Math.Round(15 * this._pesoinformado / 50, 2);

            this.OnPropertyChanged<vmUTINEOFolhaParada>(x => x.PesoInformado);
            HabilitaSalvar = true;
        }
        public bool VerificaDataNascimento()
        {
            if (!this._atendimento.Paciente.DataNascimento.HasValue)
            {
                DXMessageBox.Show("Não foi Informado a data de Nascimento do Paciente. Favor informar no Sistema MV.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }
        public void AddNovaFolha(Usuarios pUsuario)
        {
            this._utineofolhaparadaselecionada = new wrpUTINEOFolhaParada(this._atendimento);
            this._utineofolhaparadaselecionada.DataInclusao = DateTime.Today;
            this._utineofolhaparadaselecionada.Usuario = new wrpUsuarios(pUsuario);
            this._utineofolhaparadaselecionada.Paciente = this._atendimento.Paciente;
            this._pesoinformado = 0;
            this.HabilitaSalvar = false;
        }
        #endregion

        #region Metodos Privados




        #endregion

        #region Commands
        protected override void CommandSalvar(object param)
        {
            //this._atendimento.UTINEOFolhaParada.Add(this._utineofolhaparadaselecionada);            
            this._uitneofolhaparadas.Add(this._utineofolhaparadaselecionada);
            this._atendimento.Save();
            this.OnPropertyChanged<vmUTINEOFolhaParada>(x => x.UTINEOFolhaParadas);
            this.HabilitaSalvar = false;
            base.CommandSalvar(param);
        }
        #endregion     
    }
}
