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

namespace HMV.PEP.ViewModel.PEP.Evolucao
{
    public class VMPEPEvolucao : ViewModelBase
    {
        #region Construtor
        public VMPEPEvolucao(Paciente pPaciente, Usuarios pUsuario, Atendimento pAtendimento)
        {
            this._atendimento = pAtendimento;
            this._usuario = pUsuario;
            this._paciente = pPaciente;
            this._datainicial = DateTime.Now.AddDays(-10);
            this._datafinal = DateTime.Now;

            if (pAtendimento != null)
            {
                IParametroPEPService srv = ObjectFactory.GetInstance<IParametroPEPService>();
                Parametro par = srv.PEPEvolucaoOrigens();
                IList<string> lst = par.Valor.Split(',');

                if (lst.Contains(pAtendimento.OrigemAtendimento.ID.ToString()) && (_usuario.isMedico() || _usuario.isFarmaceutico()))                    
                    this._permiteinclusao = true;
            }

            if (_usuario.isMedico())
                _tipoevolucaoselecionado = "Médica";
            else
                _tipoevolucaoselecionado = "Todas";

            CarregaDados();
        }
        #endregion

        #region Eventos
        public event EventHandler Refresh;
        #endregion

        #region Propriedades Privadas
        private Atendimento _atendimento { get; set; }
        private Paciente _paciente { get; set; }
        private Usuarios _usuario { get; set; }
        private DateTime _datainicial { get; set; }
        private DateTime _datafinal { get; set; }
        private wrpPEPEvolucao _evolucao { get; set; }
        private bool _permiteinclusao;
        private string _tipoevolucaoselecionado { get; set; }
        #endregion

        #region Metodos Publicos
        public Atendimento Atendimento { get { return this._atendimento; } }

        public IList<PEPEvolucao> CarregaDados()
        {
            IRepositorioDePEPEvolucao rep;
            List<PEPEvolucao> retorno = new List<PEPEvolucao>();
            Atendimento aten;
            var atend = _atendimento.Paciente.Atendimentos.Where(x => x.ID != _atendimento.ID
                                    && x.DataAltaMedica.IsNotNull() && x.AtendimentoPai.IsNotNull()
                                    && x.DataAltaMedica.Value <= _atendimento.DataHoraAtendimento.AddHours(6)).ToList();
            if (atend.HasItems())
            {
                aten = atend.FirstOrDefault();
                rep = ObjectFactory.GetInstance<IRepositorioDePEPEvolucao>();
                rep.OndePaciente(this._paciente.ID);
                rep.OndeAtendimento(aten.ID);
                rep.OndePeriodoIgual(this._datainicial, this._datafinal);
                //rep.OrdernarPorDataDesc();
                var ret = rep.List().ToList();
                retorno = ret;
            }

            rep = ObjectFactory.GetInstance<IRepositorioDePEPEvolucao>();
            rep.OndePaciente(this._paciente.ID);
            rep.OndeAtendimento(_atendimento.ID);
            rep.OndePeriodoIgual(this._datainicial, this._datafinal);
            //rep.OrdernarPorDataDesc();
            var ret1 = rep.List();
            retorno.AddRange(ret1);

            if (_tipoevolucaoselecionado == "Médica")
                retorno = retorno.Where(x => x.Usuario.isMedico()).ToList();
            else if (_tipoevolucaoselecionado == "Assistencial")
                retorno = retorno.Where(x => !x.Usuario.isMedico()).ToList();

            retorno = retorno.OrderByDescending(x => x.Data).ToList();

            return retorno;
        }

        public IList<PEPEvolucao> BuscaEvolucoesNaoImpressas()
        {
            IRepositorioDePEPEvolucao rep = ObjectFactory.GetInstance<IRepositorioDePEPEvolucao>();
            rep.OndePaciente(this._paciente.ID);
            rep.FiltraNaoImpressas();            
            return rep.List();
        }

        public bool MostraEvolucaoPadrao
        {
            get
            {
                return (_usuario.isFarmaceutico());
            }
        }
        #endregion

        #region Propriedades Publicas      
        public DateTime DataInicial
        {
            get
            {
                return _datainicial;
            }
            set
            {
                this._datainicial = value;
                if (value > this.DataFinal)
                {
                    this.DataFinal = value;
                    this.OnPropertyChanged("DataFinal");
                }

                this.OnPropertyChanged("DataInicial");
            }
        }

        public DateTime DataFinal
        {
            get
            {
                return _datafinal;
            }
            set
            {
                this._datafinal = value;
                if (value < this.DataInicial)
                {
                    this.DataInicial = value;
                    this.OnPropertyChanged("DataInicial");
                }
                this.OnPropertyChanged("DataFinal");
            }
        }

        public wrpPEPEvolucao Evolucao
        {
            get
            {
                return _evolucao;
            }
            set
            {
                this._evolucao = value;
                this.OnPropertyChanged("Evolucao");
            }
        }      

        public List<string> TiposEvolucoes
        {
            get
            {
                return new List<string> { "Médica", "Assistencial", "Todas" };
            }
        }

        public string TipoEvolucaoSelecionado
        {
            get
            {
                return _tipoevolucaoselecionado;
            }
            set
            {
                _tipoevolucaoselecionado = value;
                if (Refresh != null)
                    Refresh(this, null);
                this.OnPropertyChanged<VMPEPEvolucao>(x => x.TipoEvolucaoSelecionado);
            }
        }
        #endregion

        #region Commands
        protected override void CommandIncluir(object param)
        {
            this.BeginEdit();
            this.Evolucao = new wrpPEPEvolucao();
            this.Evolucao.Paciente = new wrpPaciente(_paciente);
            this.Evolucao.Atendimento = new wrpAtendimento(_atendimento);
            this.Evolucao.Usuario = new wrpUsuarios(this._usuario);

            base.CommandIncluir(param);
        }

        protected override bool CommandCanExecuteIncluir(object param)
        {
            return _permiteinclusao;
        }

        protected override bool CommandCanExecuteSalvar(object param)
        {
            return !this._evolucao.IsNull() && !string.IsNullOrEmpty(this._evolucao.Evolucao);
        }

        protected override void CommandSalvar(object param)
        {
            IRepositorioDePEPEvolucao rep = ObjectFactory.GetInstance<IRepositorioDePEPEvolucao>();
            this.Evolucao.Data = DateTime.Now;

            rep.Save(this.Evolucao.DomainObject);
            this.EndEdit();

            if (Refresh != null)
                Refresh(this, null);
            base.CommandSalvar(param);
        }

        protected override void CommandFechar(object param)
        {
            if (!this._evolucao.IsNull() && !string.IsNullOrEmpty(this._evolucao.Evolucao))
                if (DXMessageBox.Show("Deseja realmente fechar a evolução sem salvá-la?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;            
            base.CommandFechar(param);
        }
        #endregion
    }
}
