using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.EvolucaoNova;
using HMV.Core.Domain.Repository.PEP.Evolucao;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.Extensions;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using HMV.PEP.Interfaces;
using DevExpress.Xpf.Core;
using System.Windows;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository.PEP.PrevAlta;
using HMV.Core.Framework.Validations;
using HMV.Core.Domain.Repository;
using System.Configuration;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.PEP.Evolucao
{
    public class VMPrevisaoAlta : ViewModelBase
    {
        #region Construtor
        public VMPrevisaoAlta(Usuarios pUsuario, Atendimento pAtendimento, bool pMostraMotivo, bool pMostraFechar)
        {
            this._atendimento = pAtendimento;
            this._usuario = pUsuario;
            this._mostramotivo = pMostraMotivo;
            this._mostrafechar = pMostraFechar;            
            this._previsaoalta = new wrpPrevisaoAlta();
            this._previsaoalta.Atendimento = new wrpAtendimento(pAtendimento);
            this._previsaoalta.Usuario = new wrpUsuarios(pUsuario);
            this._previsaoalta.DataInclusao = DateTime.Now;

            //Parametro _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).DiasJustificativaPrevisaoAlta().Single();
            //this._diasobrigajustificativa = int.Parse(_param.Valor);
            this._diasobrigajustificativa = 0;
            IRepositorioDePrevisaoAlta rep = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>();
            int ID = rep.UltimoAtendimento(pAtendimento.ID);
            if (ID > 0)
                this._previsaoaltaAnterior = new wrpPrevisaoAlta(ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>().FiltraPorID(ID).Single());
        }

        public VMPrevisaoAlta(Usuarios pUsuario, Atendimento pAtendimento, bool pMostraMsg14Dias)
        {
            this._atendimento = pAtendimento;
            this._usuario = pUsuario;
            this._mostramotivo = true;
            this._mostrafechar = false;
            this.MostraMsg14Dias = pMostraMsg14Dias;

            IRepositorioDePrevisaoAlta rep = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>();
            int ID = rep.UltimoAtendimento(pAtendimento.ID);
            if (ID > 0)
                this._previsaoaltaAnterior = new wrpPrevisaoAlta(ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>().FiltraPorID(ID).Single());

            this._previsaoalta = new wrpPrevisaoAlta();
            this._previsaoalta.Atendimento = new wrpAtendimento(pAtendimento);
            this._previsaoalta.Usuario = new wrpUsuarios(pUsuario);
            this._previsaoalta.DataInclusao = DateTime.Now;
            if (this._previsaoaltaAnterior == null)
            {
                MostraPrevisao = true;
            }
            else
            {
                if (this._previsaoaltaAnterior.DataPrevAlta < DateTime.Today)
                {
                    MostraPrevisao = true;
                }
                else
                {
                    this._previsaoalta.QtdDias = dateDiff('d', DateTime.Now.Date, this._previsaoaltaAnterior.DataPrevAlta);
                    this._previsaoalta.DataPrevAlta = this._previsaoaltaAnterior.DataPrevAlta;
                }
            }

            //Parametro _param = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>().OndeClinicaIgual(Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"])).DiasJustificativaPrevisaoAlta().Single();
            //this._diasobrigajustificativa = int.Parse(_param.Valor);
            //this._diasobrigajustificativa = 0;
            
        }
        #endregion

        //#region Eventos
        //public event EventHandler Refresh;
        //#endregion

        #region Propriedades Privadas
        private Atendimento _atendimento { get; set; }
        private Usuarios _usuario { get; set; }
        private wrpPrevisaoAlta _previsaoalta { get; set; }
        private wrpPrevisaoAlta _previsaoaltaAnterior { get; set; }
        private bool _mostramotivo;
        private bool _mostrafechar;
        private bool _mostraprevisao;
        private int _diasobrigajustificativa { get; set; }
        private bool _mostra14dias;
        private bool _mostra30dias;
        private bool _mostraoutros;
        #endregion

        #region Metodos Publicos


        #endregion

        #region Metodos Privados
        private int dateDiff(char charInterval, DateTime dttFromDate, DateTime dttToDate)
        {
            TimeSpan tsDuration;
            tsDuration = dttToDate - dttFromDate;

            if (charInterval == 'd')
            {
                // Resultado em Dias
                return tsDuration.Days;
            }
            else if (charInterval == 'm')
            {
                // Resultado em Meses
                double dblValue = 12 * (dttFromDate.Year - dttToDate.Year) + dttFromDate.Month - dttToDate.Month;
                return Convert.ToInt32(Math.Abs(dblValue));
            }
            else if (charInterval == 'y')
            {
                // Resultado em Anos
                return Convert.ToInt32((tsDuration.Days) / 365);
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Propriedades Publicas
        public wrpPrevisaoAltaMotivoCollection PrevisaoAltaMotivo
        {
            get
            {
                IRepositorioDePrevAltaMotivo rep = ObjectFactory.GetInstance<IRepositorioDePrevAltaMotivo>();
                rep.FiltraAtivos().OrdenaPorID();
                return new wrpPrevisaoAltaMotivoCollection(rep.List());
            }
        }


        public string Mensagem
        {
            get
            {
                //if (_mostramotivo)
                //    return "Informe a nova previsão de alta para o paciente (a partir de hoje):";
                //else
                return "Informe a previsão de alta do paciente:";
            }
        }

        public wrpPrevisaoAlta PrevisaoAlta
        {
            get
            {
                return _previsaoalta;
            }
            set
            {
                this._previsaoalta = value;

                this.OnPropertyChanged("PrevisaoAlta");
            }
        }

        public int? QtdDias
        {
            get
            {
                if (_previsaoalta.QtdDias.Equals(0))
                    return null;
                else
                    return _previsaoalta.QtdDias;
            }
            set
            {
                if (value.HasValue)
                {
                    if (value.Value > 30)
                    {
                        DXMessageBox.Show("Informe no máximo 30 dias.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                        _previsaoalta.QtdDias = 0;
                        return;
                    }

                    _previsaoalta.QtdDias = value.Value;

                    //if (_previsaoaltaAnterior != null)
                    //{
                    //    DateTime dataanterior = _previsaoaltaAnterior.DataPrevAlta.AddDays(this._diasobrigajustificativa * -1).Date;
                    //    DateTime dataatual = _previsaoalta.DataInclusao.AddDays(PrevisaoAlta.QtdDias).Date;
                    //    if (dataanterior > dataatual)
                    //        MostraMotivo = true;
                    //    else if (dataatual > _previsaoaltaAnterior.DataPrevAlta.Date)
                    //        MostraMotivo = true;
                    //    else
                    //        MostraMotivo = false;
                    //}
                }
                else
                    this._previsaoalta.QtdDias = 0;

                this.OnPropertyChanged("QtdDias");
                this.OnPropertyChanged("DataPrevista");

            }
        }

        public wrpPrevisaoAltaMotivo Motivo
        {
            get
            {
                return _previsaoalta.Motivo;
            }
            set
            {
                this._previsaoalta.Motivo = value;
                _previsaoalta.Observacao = null;

                MostraOutros = value.Outros.Equals(SimNao.Sim);
                this.OnPropertyChanged("Motivo");
                this.OnPropertyChanged("MostraOutros");
                this.OnPropertyChanged("Outros");
            }

        }

        public string Outros
        {
            get
            { return _previsaoalta.Observacao; }
            set
            {
                _previsaoalta.Observacao = value;                              
                this.OnPropertyChanged("Outros");
            }

        }

        public string DataPrevista
        {
            get
            {
                if (QtdDias.HasValue)
                    return _previsaoalta.DataInclusao.AddDays(PrevisaoAlta.QtdDias).ToString("dd/MM/yyyy");
                else
                    return "Informe o número de dias";
            }
        }

        public bool MostraMotivo
        {
            get { return _mostramotivo; }
            set
            {
                _mostramotivo = value;
                this.OnPropertyChanged("MostraMotivo");
            }

        }

        public bool MostraPrevisao
        {
            get { return _mostraprevisao; }
            set
            {
                _mostraprevisao = value;
                this.OnPropertyChanged("MostraPrevisao");
            }

        }

        public bool MostraOutros
        {
            get { return _mostraoutros; }
            set
            {
                _mostraoutros = value;
                this.OnPropertyChanged("MostraOutros");
            }

        }

        public bool MostraMsg14Dias
        {
            get { return _mostra14dias; }
            set
            {
                _mostra14dias = value;
                _mostra30dias = !value;
                this.OnPropertyChanged("MostraMsg14Dias");
                this.OnPropertyChanged("MostraMsg30Dias");
            }

        }

        public bool MostraMsg30Dias
        {
            get { return _mostra30dias; }
            set
            {
                _mostra30dias = value;
                _mostra14dias = !value;
                this.OnPropertyChanged("MostraMsg14Dias");
                this.OnPropertyChanged("MostraMsg30Dias");
            }

        }

        public bool MostraFechar
        { get { return _mostrafechar; } }
        #endregion

        #region Commands
        protected override bool CommandCanExecuteSalvar(object param)
        {
            bool retorno = false;

            if (this.QtdDias.HasValue)
            {
                if (_mostramotivo)
                {
                    if (Motivo != null && Motivo.Outros.Equals(SimNao.Nao))
                        retorno = true;
                    else
                        if (Motivo != null && Motivo.Outros.Equals(SimNao.Sim) && !string.IsNullOrWhiteSpace(Outros))
                            retorno = true;
                }
                else
                    retorno = true;
            }

            return retorno;
        }

        protected override void CommandSalvar(object param)
        {
            IRepositorioDePrevisaoAlta rep = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>();

            _previsaoalta.DataPrevAlta = _previsaoalta.DataInclusao.Date.AddDays(QtdDias.Value);

            if (!MostraMotivo)
            {
                _previsaoalta.Observacao = string.Empty;
                _previsaoalta.Motivo = null;
            }

            rep.Save(_previsaoalta.DomainObject);
            base.CommandSalvar(param);
            base.CommandFechar(param);
        }

        protected override void CommandFechar(object param)
        {
            base.CommandFechar(param);
        }

        #endregion
    }
}
