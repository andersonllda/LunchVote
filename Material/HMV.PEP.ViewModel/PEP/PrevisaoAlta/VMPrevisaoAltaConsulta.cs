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

namespace HMV.PEP.ViewModel.PEP.Evolucao
{
    public class VMPrevisaoAltaConsulta : ViewModelBase
    {
        #region Construtor
        public VMPrevisaoAltaConsulta(Usuarios pUsuario, Atendimento pAtendimento)
        {
            this._atendimento = pAtendimento;
            this._usuario = pUsuario;
            int id = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>().UltimoAtendimento(pAtendimento.ID);
            if (id > 0)
                this._previsaoalta = new wrpPrevisaoAlta(ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>().FiltraPorID(id).Single());

            Mensagem = "Confirma previsão de alta nas próximas 48 horas?";
        }
        #endregion


        #region Propriedades Privadas
        private Atendimento _atendimento;
        private Usuarios _usuario;
        private wrpPrevisaoAlta _previsaoalta;
        private string _mensagem;
        #endregion
        #region Propriedades Publicas
        //public string Mensagem
        //{
        //    get
        //    {
        //        if (Has24Horas)
        //            return "Confirma previsão de alta nas próximas 24 horas?";
        //        if (Has48Horas)
        //            return "Confirma previsão de alta nas próximas 48 horas?";
        //        if (Has7Dias)
        //            return "Confirma previsão de alta para: " + this._previsaoalta.DataPrevAlta.ToString("dd/MM/yyyy") + "?";

        //        return string.Empty;
        //    }
        //}
        public string Mensagem
        {
            get { return _mensagem; }
            set
            {
                _mensagem = value;
                this.OnPropertyChanged("Mensagem");
            }
        }

        public wrpPrevisaoAltaCollection Itens
        {

            get
            {
                IRepositorioDePrevisaoAlta rep = ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>();
                rep.FiltraAtendimento(this._atendimento.ID).OrdenaSeq();
                return new wrpPrevisaoAltaCollection(rep.List());
            }
        }

        public bool HasPrevisao
        {
            get
            { return (_previsaoalta.IsNotNull()); }
        }

        public bool HasPrevisaoNoDia
        {
            get
            { return (_previsaoalta.IsNotNull() && _previsaoalta.DataInclusao.Date.Equals(DateTime.Today)); }
        }

        public bool Has7Dias
        {
            get
            { return (dateDiff('d', _previsaoalta.DataInclusao.Date, DateTime.Now.Date) == 7); }
        }

        public bool HasDataPassada
        {
            get
            { return (DiasFaltando < 0); }
        }

        public bool Has24Horas
        {
            get
            { return (DiasFaltando >= 0 && DiasFaltando < 2 && _previsaoalta.DataConfirmacao.IsNull()); }
        }

        public bool Has48Horas
        {
            get
            { return (DiasFaltando == 2 && _previsaoalta.DataConfirmacao.IsNull()); }
        }

        public string QtdDias
        {
            get
            {
                return _previsaoalta.QtdDias.ToString();
            }
        }

        public int DiasFaltando
        {
            get
            {
                if (_previsaoalta.IsNull()) return 0;

                return dateDiff('d', DateTime.Now.Date, _previsaoalta.DataPrevAlta.Date);
            }
        }

        public string Data
        {
            get
            {
                return _previsaoalta.DataPrevAlta.ToString("dd/MM/yyyy");
            }
        }

        public Atendimento Atendimento { get { return _atendimento; } }

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

        protected override void CommandSalvar(object param)
        {
            HMV.Core.Domain.Model.PEP.PrevAlta.PrevisaoAlta previsaoAlta = new Core.Domain.Model.PEP.PrevAlta.PrevisaoAlta();
            if (_previsaoalta.IsNotNull())
            {
                previsaoAlta.Atendimento = _previsaoalta.Atendimento.DomainObject;
                previsaoAlta.DataPrevAlta = _previsaoalta.DataPrevAlta;
                previsaoAlta.QtdDias = _previsaoalta.QtdDias;
            }
            else 
            {
                previsaoAlta.Atendimento = _atendimento;
                previsaoAlta.DataPrevAlta = DateTime.Now.AddDays(2);
                previsaoAlta.QtdDias = 2;            
            }

            previsaoAlta.Confirmado = Core.Domain.Enum.SimNao.Sim;
            previsaoAlta.DataInclusao = DateTime.Now;
            previsaoAlta.Usuario = _usuario;
            previsaoAlta.DataConfirmacao = DateTime.Now;

            ObjectFactory.GetInstance<IRepositorioDePrevisaoAlta>().Save(previsaoAlta);

            base.CommandSalvar(param);
            base.CommandFechar(param);
        }
        #endregion


    }
}
