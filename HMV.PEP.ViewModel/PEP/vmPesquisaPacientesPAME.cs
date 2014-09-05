using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmPesquisaPacientesPAME : ViewModelBase
    {
        #region Construtor

        #endregion

        #region Propriedades Privadas

        private ObservableCollection<wrpPaciente> _listapacienteCollection { get; set; }
        private wrpPaciente _pacienteselecionado { get; set; }
        private string _cpf { get; set; }
        private string _identidade { get; set; }
        private string _prontuario { get; set; }
        private string _datanascimento { get; set; }
        private string _nome { get; set; }
        private string _nomeDaMae { get; set; }
        private bool _botaopesquisa { get; set; }
        #endregion

        #region Propriedades Públicas

        public string CPF
        {
            get
            {
                return this._cpf;
            }
            set
            {
                this._cpf = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.CPF));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.BotaoPesquisa));
            }
        }

        public string Identidade
        {
            get
            {
                return this._identidade;
            }
            set
            {
                this._identidade = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.Identidade));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.BotaoPesquisa));
            }
        }

        public string Prontuario
        {
            get
            {
                return this._prontuario;
            }
            set
            {
                this._prontuario = value;
                //if (_prontuario.IsNotNull())
                //    if (int.Parse(_prontuario) == 0)
                //        _prontuario = string.Empty;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.Prontuario));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.BotaoPesquisa));
            }
        }

        public string DataNascimento
        {
            get
            {
                return this._datanascimento;
            }
            set
            {
                this._datanascimento = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.DataNascimento));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.BotaoPesquisa));
            }
        }

        public string Nome
        {
            get
            {
                return this._nome;
            }
            set
            {
                this._nome = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.Nome));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.BotaoPesquisa));
            }
        }

        public string NomeDaMae
        {
            get
            {
                return this._nomeDaMae;
            }
            set
            {
                this._nomeDaMae = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.NomeDaMae));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.BotaoPesquisa));
            }
        }

        public string Registros
        {
            get
            {
                return (ListaPacientesCollection.IsNull()) ? "Total de Registros: " : "Total de Registros: " + ListaPacientesCollection.Count.ToString();
            }
        }

        public bool BotaoPesquisa
        {
            get
            {
                return CPF.Combine(Identidade).Combine(Prontuario).Combine(DataNascimento).Combine(Nome).Combine(NomeDaMae).Length > 0 ? true : false;
            }
        }

        public ObservableCollection<wrpPaciente> ListaPacientesCollection
        {
            get
            {
                return this._listapacienteCollection;
            }
        }

        public wrpPaciente PacienteSelecionado
        {
            get
            {
                return this._pacienteselecionado;
            }
            set
            {
                this._pacienteselecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.PacienteSelecionado));
            }
        }

        #endregion

        #region Métodos Privados

        private void ListaPacientes()
        {
            IRepositorioDePacientes rep = ObjectFactory.GetInstance<IRepositorioDePacientes>();

            var result = rep.OndeTemPame();

            if (!Prontuario.IsEmptyOrWhiteSpace())
                rep.OndeCodigoIgual(Convert.ToInt32(Prontuario));

            if (!Nome.IsEmptyOrWhiteSpace())
                rep.OndeNomeContem(Nome.ToUpper());

            //if (!CPF.IsEmptyOrWhiteSpace())
            //    rep.OndeCPFIgual(CPF);

            //if (!Identidade.IsEmptyOrWhiteSpace())
            //    rep.OndeIdentidadeIgual(Identidade.ToUpper());

            //if (!DataNascimento.IsEmptyOrWhiteSpace())
            //    rep.OndeDataNascimentoIgual(Convert.ToDateTime(DataNascimento));

            //if (!NomeDaMae.IsEmptyOrWhiteSpace())
            //    rep.OndeNomeDaMaeIgual(NomeDaMae.ToUpper());

            this._listapacienteCollection = new ObservableCollection<wrpPaciente>((from x in rep.List() select new wrpPaciente(x)).ToList().OrderBy(x => x.Nome));

            this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.ListaPacientesCollection));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmPesquisaPacientes>(x => x.Registros));
        }

        #endregion

        #region Métodos Públicos

        public void PesquisaPacientes()
        {
            ListaPacientes();
        }

        public void Fechar()
        {
            this._pacienteselecionado = null;
        }

        #endregion

        #region Commands

        #endregion
    }
}
