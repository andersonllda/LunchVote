using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.Consult;
using HMV.PEP.DTO;
using StructureMap;
using HMV.Core.Framework.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using HMV.PEP.ViewModel.Commands;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;

namespace HMV.PEP.ViewModel.SumarioDeAtendimento
{
    public class vmPIN2 : ViewModelBase
    {
        #region Contrutor

        public vmPIN2(vmMotivoInternacaoPim2 pvmMotivoInternacaoPim2)
        {
            this._vmMotivoInternacaoPin2 = pvmMotivoInternacaoPim2;
        }
        #endregion

        #region Propriedades Publicas
        public wrpAtendimento Atendimento
        {
            get { return this._vmMotivoInternacaoPin2.Atendimento; }
        }
        public wrpPIN2Collection ListaPIN2
        {
            get { return this._vmMotivoInternacaoPin2.Atendimento.PIN2; }
        }
        public wrpPIN2 Pin2Selecionado
        {
            get { return this._pin2; }
            set
            {
                this._pin2 = value;
                if (value != null)
                {
                    this.OnPropertyChanged("Elective");
                    this.OnPropertyChanged("Recovery");
                    this.OnPropertyChanged("ByPass");
                    this.OnPropertyChanged("Haut");
                    this.OnPropertyChanged("BAS");
                    this.OnPropertyChanged("Pupilles");
                    this.OnPropertyChanged("Ventil");
                    this.OnPropertyChanged("PAS");
                    this.OnPropertyChanged("BE");
                    this.OnPropertyChanged("Ratio");
                }
            }
        }

        public bool TemElective
        {
            get { return this._temelective; }
            set
            {
                this._temelective = value;
                if (value)
                    this._pin2.Elective = -0.9282;
                else
                    this._pin2.Elective = 0;

                this.Calcula();
                this.OnPropertyChanged("TemElective");
                this.OnPropertyChanged("Elective");
            }
        }
        public bool TemRecovery
        {
            get { return this._temrecovery; }
            set
            {
                this._temrecovery = value;
                if (value)
                    this._pin2.Recovery = -1.0244;
                else
                    this._pin2.Recovery = 0;

                this.Calcula();
                this.OnPropertyChanged("TemRecovery");
                this.OnPropertyChanged("Recovery");
            }
        }
        public bool TemByPass
        {
            get { return this._tembypass; }
            set
            {
                this._tembypass = value;
                if (value)
                    this._pin2.Bypass = 0.7507;
                else
                    this._pin2.Bypass = 0;

                this.Calcula();
                this.OnPropertyChanged("TemByPass");
                this.OnPropertyChanged("ByPass");
            }
        }
        public bool TemHaut
        {
            get { return this._temhaut; }
            set
            {
                this._temhaut = value;
                if (value)
                    this._pin2.Haut = 1.6829;
                else
                    this._pin2.Haut = 0;

                this.Calcula();
                this.OnPropertyChanged("TemHaut");
                this.OnPropertyChanged("Haut");
            }
        }
        public bool TemBAS
        {
            get { return this._tembas; }
            set
            {
                this._tembas = value;
                if (value)
                    this._pin2.BAS = -1.5770;
                else
                    this._pin2.BAS = 0;

                this.Calcula();
                this.OnPropertyChanged("TemBAS");
                this.OnPropertyChanged("BAS");
            }
        }
        public bool TemPupilles
        {
            get { return this._tempupilles; }
            set
            {
                this._tempupilles = value;
                if (value)
                    this._pin2.Pupilles = 3.0791;
                else
                    this._pin2.Pupilles = 0;

                this.Calcula();
                this.OnPropertyChanged("TemPupilles");
                this.OnPropertyChanged("Pupilles");
            }
        }
        public bool TemVentil
        {
            get { return this._temventil; }
            set
            {
                this._temventil = value;
                if (value)
                    this._pin2.Ventil = 1.3352;
                else
                    this._pin2.Ventil = 0;

                this.Calcula();
                this.OnPropertyChanged("TemVentil");
                this.OnPropertyChanged("Ventil");
            }
        }

        public double Elective
        {
            get { return this._pin2 == null ? 0 : this._pin2.Elective; }
            set
            {
                this._pin2.Elective = value;
                this.Calcula();
            }
        }
        public double Recovery
        {
            get { return this._pin2 == null ? 0 : this._pin2.Recovery; }
            set
            {
                this._pin2.Recovery = value;
                this.Calcula();
            }
        }
        public double ByPass
        {
            get { return this._pin2 == null ? 0 : this._pin2.Bypass; }
            set
            {
                this._pin2.Bypass = value;
                this.Calcula();
            }
        }
        public double Haut
        {
            get { return this._pin2 == null ? 0 : this._pin2.Haut; }
            set
            {
                this._pin2.Haut = value;
                this.Calcula();
            }
        }
        public double BAS
        {
            get { return this._pin2 == null ? 0 : this._pin2.BAS; }
            set
            {
                this._pin2.BAS = value;
                this.Calcula();
            }
        }
        public double Pupilles
        {
            get { return this._pin2 == null ? 0 : this._pin2.Pupilles; }
            set
            {
                this._pin2.Pupilles = value;
                this.Calcula();
            }
        }
        public double Ventil
        {
            get { return this._pin2 == null ? 0 : this._pin2.Ventil; }
            set
            {
                this._pin2.Ventil = value;
                this.Calcula();
            }
        }
        public double PAS
        {
            get { return this._pin2 == null ? 0 : this._pin2.PAS; }
            set
            {
                this._pin2.PAS = value;
                this.Calcula();
            }
        }
        public double BE
        {
            get { return this._pin2 == null ? 0 : this._pin2.BE; }
            set
            {
                this._pin2.BE = value;
                this.Calcula();
            }
        }
        public double Ratio
        {
            get { return this._pin2 == null ? 0 : this._pin2.Ratio; }
            set
            {
                this._pin2.Ratio = value;
                this.Calcula();
            }
        }
        public double Total
        {
            get { return this._pin2 == null ? 0 : this._pin2.Total; }
            set { this._pin2.Total = value; }
        }

        #region  RELATORIO


        #endregion

        #endregion

        #region Metodos Privados
        private void Calcula()
        {
            if (this._pin2.Bypass > 0 && this._pin2.Recovery == 0)
                DXMessageBox.Show("Recuperação pós procedimento deveria estar marcado para selecionar Cirurgia com circulação.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            if (this._pin2.Haut > 0 && this._pin2.BAS < 0)
                DXMessageBox.Show("Diagnóstico deveria ser de Alto Risco ou Baixo Risco.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            var result = this._pin2.Elective + this._pin2.Recovery + this._pin2.Bypass + this._pin2.Haut + this._pin2.BAS + this._pin2.Pupilles + this._pin2.Ventil;
            var result2 = Math.Abs((this._pin2.PAS - 120) * 0.01395);
            result += result2;
            result2 = Math.Abs(this._pin2.BE * 0.1040);
            result += result2;
            result2 = this._pin2.Ratio * 0.2888;
            result += result2;
            result -= 4.8841;
            result = Math.Exp(result) / (1 + Math.Exp(result));

            this._pin2.Total = Math.Round(result * 100, 1);
            this.OnPropertyChanged("Total");
        }
        #endregion

        #region Propriedades Privadas
        private wrpPIN2 _pin2 { get; set; }
        //private wrpUsuarios _usuarios { get; set; }
        //private wrpAtendimento _atendimento { get; set; }
        private vmMotivoInternacaoPim2 _vmMotivoInternacaoPin2;
        private bool _temelective { get; set; }
        private bool _temrecovery { get; set; }
        private bool _tembypass { get; set; }
        private bool _temhaut { get; set; }
        private bool _tembas { get; set; }
        private bool _tempupilles { get; set; }
        private bool _temventil { get; set; }
        private bool _tempas { get; set; }
        private bool _tembe { get; set; }
        private bool _temratio { get; set; }
        #endregion

        #region Metodos Publicos
        public void NovoPIN2()
        {

            this._vmMotivoInternacaoPin2.vmMotivoDeInternacao.Inicializa();

            this._pin2 = new wrpPIN2(this._vmMotivoInternacaoPin2.Atendimento.DomainObject);
            this._pin2.Status = Status.Ativo;
            this._pin2.Usuario = this._vmMotivoInternacaoPin2.Usuario;
            this._pin2.DataInclusao = DateTime.Now;

            this._pin2.BAS = 0;
            this._pin2.Bypass = 0;
            this._pin2.Elective = 0;
            this._pin2.Haut = 0;
            this._pin2.Pupilles = 0;
            this._pin2.Recovery = 0;
            this._pin2.Ventil = 0;
            this._pin2.PAS = 120;
            this._pin2.BE = 0;
            this._pin2.Ratio = 0;
        }
        #endregion

        #region Commands
        //public ICommand SavePIN2Command { get; set; }
        //public ICommand RemovePIN2Command { get; set; }
        #endregion

        #region Relatorio

        public bool BotaoImprimir
        {
            get { return (ListaPIN2.Count > 0 ? true : false); }
        }

        public List<rListaValores> RelListaValores
        {
            get
            {
                List<rListaValores> qry = null;
                if (ListaPIN2.Count == 0)
                    qry = null;
                else
                    qry = (from Lista in ListaPIN2
                           select new rListaValores
                           {
                               Usuario = Lista.Usuario.Usuario.nome.ToString(),
                               DataInclusao = Lista.DataInclusao.ToString("dd/MM/yyyy hh:mm"),
                               Total = Lista.Total.ToString()
                           }).ToList();

                return qry;
            }
        }

        public List<rCabecalho> RelCabecalho
        {
            get
            {
                List<rCabecalho> qry = new List<rCabecalho>();
                rCabecalho cab = new rCabecalho();

                cab.Nome = this._vmMotivoInternacaoPin2.Atendimento.Paciente.ID + " - " + this.Atendimento.Paciente.Nome.ToString();
                cab.Idade = this._vmMotivoInternacaoPin2.Atendimento.Paciente.Idade.GetDate(this.Atendimento.HoraAtendimento); //ToString(2);
                cab.Sexo = this.Atendimento.Paciente.Sexo.ToString();
                cab.Atendimento = this.Atendimento.ID.ToString();
                cab.Data = this.Atendimento.DataAtendimento.ToString("dd/MM/yyyy");
                cab.Medico = this.Atendimento.Prestador.Nome.ToString();
                
                if (this.Atendimento.Convenio.IsNotNull())
                    cab.Convenio = this.Atendimento.Convenio.Descricao.ToString();
                if (this._vmMotivoInternacaoPin2.Atendimento.Leito.IsNotNull())
                cab.QuartoLeito = this._vmMotivoInternacaoPin2.Atendimento.Leito.Descricao.ToString();

                qry.Add(cab);

                return qry;
            }
        }
        #endregion

        #region ClassesRelatorio
        public class rCabecalho
        {
            public string Nome { get; set; }
            public string Idade { get; set; }
            public string Sexo { get; set; }
            public string Atendimento { get; set; }
            public string Data { get; set; }
            public string Medico { get; set; }
            public string Convenio { get; set; }
            public string QuartoLeito { get; set; }
        }

        public class rListaValores
        {
            public string Usuario { get; set; }
            public string DataInclusao { get; set; }
            public string Total { get; set; }
        }
        #endregion

    }
}
