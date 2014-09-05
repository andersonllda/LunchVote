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

namespace HMV.PEP.ViewModel.UrgenciaP
{
    public class vmUrgenciaPediatrica : ViewModelBase
    {
        #region Construtor
        public vmUrgenciaPediatrica(Atendimento pAtendimento)
        {
            //IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            //rep.Refresh(pAtendimento);

            if (pAtendimento.UrgenciasPediatricas == null)
            {
                pAtendimento.UrgenciasPediatricas = new List<UrgenciaPediatricaAtendimento>();
            }

            this.AddUrgenciaPediatricaCommand = new AddUrgenciaPediatricaCommand(this);

            this._atendimento = new wrpAtendimento(pAtendimento);

            this._urgenciapediatricaatendimentos = new wrpUrgenciaPediatricaAtendimentoCollection(this._atendimento.DomainObject.UrgenciasPediatricas);
            //this._urgenciapediatricaatendimentos.Sort(x => x.HoraInclusao);

            this._sc = string.Empty;

            HabilitaSalvar = false;
        }
        #endregion

        #region Propriedades Publicas
        public wrpUrgenciaPediatricaAtendimentoCollection UrgenciaPediatricaAtendimentos
        {
            get { return this._urgenciapediatricaatendimentos; }
        }
        public wrpUrgenciaPediatricaAtendimento UrgenciaPediatricaAtendimentoSelecionado
        {
            get { return this._urgenciapediatricaatendimentoselecionado; }
            set
            {
                this._urgenciapediatricaatendimentoselecionado = value;
                this.OnPropertyChanged("UrgenciaPediatricaAtendimentoSelecionado");
            }
        }
        public wrpAtendimento Atendimento
        {
            get { return this._atendimento; }
            set
            {
                this._atendimento = value;
                this.OnPropertyChanged("Atendimento");
            }
        }
        public wrpPaciente Paciente
        {
            get { return new wrpPaciente(this._atendimento.Paciente.DomainObject); }
        }

        [Range(0, 70, Message = "O valor para peso deve ser entre 0 e 70")]
        public double PesoInformado
        {
            get { return this._pesoinformado; }
            set
            {
                this._urgenciaitemscabecalho1 = new List<ItemUrgenciaCabecalho>();
                this._urgenciaitemscabecalho2 = new List<ItemUrgenciaCabecalho>();
                this._urgenciaitems = new List<ItemUrgencia>();
                this._sc = string.Empty;
                this._pesoinformado = value;
                if (this._urgenciapediatricaatendimentoselecionado != null)
                    this._urgenciapediatricaatendimentoselecionado.Peso = value;

                if (value >= 0 && value <= 70)
                    this._habilitacalcular = true;
                else
                    this._habilitacalcular = false;

                HabilitaSalvar = false;
                this.OnPropertyChanged("PesoInformado");
                this.OnPropertyChanged("HabilitaCalcular");
                this.OnPropertyChanged("SC");
                this.OnPropertyChanged("UrgenciaItemsCabecalho1");
                this.OnPropertyChanged("UrgenciaItemsCabecalho2");
                this.OnPropertyChanged("UrgenciaItems");
            }
        }

        public IList<ItemUrgenciaCabecalho> UrgenciaItemsCabecalho1
        {
            get
            {
                return this._urgenciaitemscabecalho1;
            }
        }

        public IList<ItemUrgenciaCabecalho> UrgenciaItemsCabecalho2
        {
            get
            {
                return this._urgenciaitemscabecalho2;
            }
        }

        public IList<ItemUrgencia> UrgenciaItems
        {
            get
            {
                return this._urgenciaitems;
            }
        }

        public string SC
        {
            get { return this._sc; }
        }

        public bool HabilitaCalcular
        {
            get { return this._habilitacalcular; }
        }

        public bool HabilitaSalvar { get; set; }

        public string Idade
        {
            get
            {
                return this._atendimento.Paciente.Idade.GetDate(); //ToString(2);
            }
        }
        #endregion

        #region Propriedades Privadas
        private wrpUrgenciaPediatricaAtendimentoCollection _urgenciapediatricaatendimentos { get; set; }
        private wrpUrgenciaPediatricaAtendimento _urgenciapediatricaatendimentoselecionado { get; set; }
        private wrpAtendimento _atendimento { get; set; }
        private double _pesoinformado { get; set; }
        private List<ItemUrgenciaCabecalho> _urgenciaitemscabecalho1 { get; set; }
        private List<ItemUrgenciaCabecalho> _urgenciaitemscabecalho2 { get; set; }
        private List<ItemUrgencia> _urgenciaitems { get; set; }
        private string _sc { get; set; }
        private bool _habilitacalcular { get; set; }
        #endregion

        #region Metodos Publicos
        public void Calcula()
        {
            this._urgenciaitemscabecalho1 = new List<ItemUrgenciaCabecalho>();
            this._urgenciaitems = new List<ItemUrgencia>();

            this._urgenciapediatricaatendimentoselecionado.SC = Math.Round(((this._pesoinformado * 4) + 7) / (this._pesoinformado + 90), 1);
            this._sc = this._urgenciapediatricaatendimentoselecionado.SC.ToString() + " m²";

            if (this._pesoinformado <= 10)
                this._urgenciapediatricaatendimentoselecionado.TamanhoPa = TipoPaciente.Pediatrico.ToString();
            else
                this._urgenciapediatricaatendimentoselecionado.TamanhoPa = TipoPaciente.Adulto.ToString();

            IRepositorioDeUrgenciaPediatricaGrupo rep = ObjectFactory.GetInstance<IRepositorioDeUrgenciaPediatricaGrupo>();
            rep.OrdenaPorOrdem();
            var itensgrupo = rep.List().OrderBy(x => x.Ordem);
            var itens = itensgrupo.Where(x => x.UrgenciaPediatricaItens != null).SelectMany(x => x.UrgenciaPediatricaItens).ToList();

            var cheech = (from it in itens
                          join ig in itensgrupo on it.UrgenciaPediatricaGrupo.Id equals ig.Id into mix
                          from yeah in mix.DefaultIfEmpty()
                          where it.Status == Status.Ativo && yeah.Cabecalho == SimNao.Sim
                          orderby it.Ordem
                          select new ItemUrgenciaCabecalho
                          {
                              Item = it,
                              Descricao = it.Droga,
                              Valor = it.DescricaoDose
                          }).ToList();

            foreach (var kanks in cheech)
            {
                this.CalculaItensCabecalho(kanks);
            }

            var chong = (from it in itens
                         join ig in itensgrupo on it.UrgenciaPediatricaGrupo.Id equals ig.Id into mix
                         from yeah in mix.DefaultIfEmpty()
                         where it.Status == Status.Ativo && yeah.Status == Status.Ativo && yeah.Cabecalho == SimNao.Nao
                         orderby yeah.Ordem
                         select new ItemUrgencia
                         {
                             Item = it,
                             Apresentacao = it.Apresentacao,
                             Dose = it.Dose,
                             Droga = it.Droga,
                             Grupo = yeah.Descricao,
                             Valor = it.DescricaoDose,
                             Ordem = yeah.Ordem,
                             Titulo1 = yeah.Titulo1,
                             Titulo2 = yeah.Titulo1,
                             Titulo3 = yeah.Titulo3
                         }).ToList();

            foreach (var hemp in chong)
            {
                this.CalculaItens(hemp);
            }

            var separa = this._urgenciaitemscabecalho1.GetRange(0, Math.Abs(this._urgenciaitemscabecalho1.Count / 2));
            if (this._urgenciaitemscabecalho1.Count > 0)
                this._urgenciaitemscabecalho2 = this._urgenciaitemscabecalho1.GetRange(Math.Abs(this._urgenciaitemscabecalho1.Count / 2), this._urgenciaitemscabecalho1.Count - separa.Count);
            this._urgenciaitemscabecalho1 = separa;

            //this._pesoinformado = 0;
            //this._habilitacalcular = false;
            this.OnPropertyChanged("PesoInformado");
            //this.OnPropertyChanged("HabilitaCalcular");
            this.OnPropertyChanged("SC");
            this.OnPropertyChanged("UrgenciaItemsCabecalho1");
            this.OnPropertyChanged("UrgenciaItemsCabecalho2");
            this.OnPropertyChanged("UrgenciaItems");
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
        public void AddNovaUrgencia(Usuarios pUsuario)
        {
            this._urgenciapediatricaatendimentoselecionado = new wrpUrgenciaPediatricaAtendimento(this.Atendimento.DomainObject);
            this._urgenciapediatricaatendimentoselecionado.DataInclusao = DateTime.Today;
            this._urgenciapediatricaatendimentoselecionado.HoraInclusao = DateTime.Now;
            this._urgenciapediatricaatendimentoselecionado.Usuario = new wrpUsuarios(pUsuario);
            this._urgenciapediatricaatendimentoselecionado.UrgenciaPediatricaAtendimentoItens = new wrpUrgenciaPediatricaAtendimentoItemCollection(new List<UrgenciaPediatricaAtendimentoItem>());
            this._urgenciaitemscabecalho1 = new List<ItemUrgenciaCabecalho>();
            this._urgenciaitemscabecalho2 = new List<ItemUrgenciaCabecalho>();
            this._urgenciaitems = new List<ItemUrgencia>();
            this._sc = string.Empty;
            this._pesoinformado = 0;
        }
        public List<ItemUrgencia> GetUrgenciaItensRel()
        {
            List<ItemUrgencia> itens = new List<ItemUrgencia>();

            if (this._urgenciapediatricaatendimentoselecionado != null && this._urgenciapediatricaatendimentoselecionado.DomainObject.UrgenciaPediatricaAtendimentoItens != null)
            {
                foreach (var item in this._urgenciapediatricaatendimentoselecionado.DomainObject.UrgenciaPediatricaAtendimentoItens.Where(x => x.UrgenciaPediatricaItem.UrgenciaPediatricaGrupo.Cabecalho == SimNao.Nao))
                {
                    itens.Add(new ItemUrgencia
                                            {
                                                Apresentacao = item.Apresentacao,
                                                Droga = item.Droga,
                                                Dose = item.Dose,
                                                Valor = item.DoseCalculada,
                                                Grupo = item.UrgenciaPediatricaItem.UrgenciaPediatricaGrupo.Descricao,
                                                Ordem = item.UrgenciaPediatricaItem.UrgenciaPediatricaGrupo.Ordem,
                                                Titulo1 = item.UrgenciaPediatricaItem.UrgenciaPediatricaGrupo.Titulo1,
                                                Titulo2 = item.UrgenciaPediatricaItem.UrgenciaPediatricaGrupo.Titulo2,
                                                Titulo3 = item.UrgenciaPediatricaItem.UrgenciaPediatricaGrupo.Titulo3.Replace("@TAM_PAS", PesoInformado <= 10 ? "Pediátrico" : "Adulto")
                                            }
                        );
                }
            }

            return itens;
        }

        public string GetUrgenciaCabecalhoItens(CabecalhoItem Campo)
        {
            if (this._urgenciapediatricaatendimentoselecionado.DomainObject.UrgenciaPediatricaAtendimentoItens != null)
            {
                var lista = this._urgenciapediatricaatendimentoselecionado.DomainObject.UrgenciaPediatricaAtendimentoItens.Where(x => x.UrgenciaPediatricaItem.UrgenciaPediatricaGrupo.Cabecalho == SimNao.Sim);
                return lista.Where(x => x.UrgenciaPediatricaItem.Id == int.Parse(Enum<CabecalhoItem>.GetDescriptionOf(Campo))).SingleOrDefault().DoseCalculada;
            }
            return string.Empty;
        }

        #endregion

        #region Metodos Privados
        private bool TestaMultiplicacaoMenor(double val1, double val2, double val3)
        {
            if (val1 * val2 < val3)
                return true;
            return false;
        }
        private bool TestaMultiplicacaoMaior(double val1, double val2, double val3)
        {
            if (val1 * val2 > val3)
                return true;
            return false;
        }

        private void CalculaItens(ItemUrgencia pItem)
        {
            switch (pItem.Item.Id)
            {
                case 7: //Soro Fisiológico
                    pItem.Valor = Math.Round((this._pesoinformado * 20 + 0.01 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 8: //Adrenalina
                    pItem.Valor = Math.Round((this._pesoinformado * 0.1 + 0.01 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 9: //Atropina
                    pItem.Valor = (TestaMultiplicacaoMenor(0.1, this._pesoinformado, 0.4) ? 0.4 : Math.Round(this._pesoinformado * 0.1 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 10: //Bicabornato
                    pItem.Valor = (TestaMultiplicacaoMaior(2, this._pesoinformado, 40) ? 40 : Math.Round(this._pesoinformado * 2 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 11: //Midazolan
                    pItem.Valor = (TestaMultiplicacaoMaior(0.04, this._pesoinformado, 2) ? 2 : Math.Round(this._pesoinformado * 0.04 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 12: // Diazepan
                    pItem.Valor = (TestaMultiplicacaoMaior(0.1, this._pesoinformado, 2) ? 2 : Math.Round(this._pesoinformado * 0.1 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 13: //Fentanil
                    pItem.Valor = (TestaMultiplicacaoMaior(0.04, this._pesoinformado, 2) ? 2 : Math.Round(this._pesoinformado * 0.04 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 14: //Quetamina
                    pItem.Valor = Math.Round((this._pesoinformado * 0.04 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 15: //Tiopental
                    pItem.Valor = (TestaMultiplicacaoMaior(0.04, this._pesoinformado, 2) ? 2 : Math.Round(this._pesoinformado * 0.04 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 16: //Hidrato Cloral
                    pItem.Valor = Math.Round((this._pesoinformado * 0.15 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 17: //Pancurônio
                    pItem.Valor = (TestaMultiplicacaoMaior(0.05, this._pesoinformado, 2.5) ? 2.5 : Math.Round(this._pesoinformado * 0.05 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 18: //Atracurion
                    pItem.Valor = Math.Round((this._pesoinformado * 0.04 + 0.01 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 19: //Succinilcolina
                    pItem.Valor = Math.Round((this._pesoinformado * 0.05 + 0.01 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 20: //Rocurônio
                    pItem.Valor = Math.Round((this._pesoinformado * 0.06 + 0.01 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 21: //Naloxone
                    pItem.Valor = (TestaMultiplicacaoMaior(0.25, this._pesoinformado, 5) ? 5 : Math.Round(this._pesoinformado * 0.25 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 22: //Flumazenil
                    pItem.Valor = (TestaMultiplicacaoMaior(0.1, this._pesoinformado, 2) ? 2 : Math.Round(this._pesoinformado * 0.1 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 23: //Adrenalina
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo1 + " | " + pItem.Titulo3;
                    pItem.Apresentacao = Math.Round((this._pesoinformado * 0.6 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 24: //Dopamina
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo1 + " | " + pItem.Titulo3;
                    pItem.Apresentacao = Math.Round((this._pesoinformado * 1.2 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 25: //Dobutamina
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo1 + " | " + pItem.Titulo3;
                    pItem.Apresentacao = Math.Round((this._pesoinformado * 0.48 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 26: //Lidocaína
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo1 + " | " + pItem.Titulo3;
                    pItem.Apresentacao = Math.Round((this._pesoinformado * 0.6 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 27: //Amiodarona
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo1 + " | " + pItem.Titulo3;
                    pItem.Apresentacao = Math.Round((this._pesoinformado * 0.6 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 28: //Milrinone
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo1 + " | " + pItem.Titulo3;
                    pItem.Apresentacao = Math.Round((this._pesoinformado * 0.15 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 29: //1a. Dose
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo3.Replace("@TAM_PAS", this._urgenciapediatricaatendimentoselecionado.TamanhoPa);
                    pItem.Valor = Math.Round((this._pesoinformado * 2 + 0.01), 1).ToString() + " J";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 30: //2a. e Demais doses
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo3.Replace("@TAM_PAS", this._urgenciapediatricaatendimentoselecionado.TamanhoPa);
                    pItem.Valor = Math.Round((this._pesoinformado * 4 + 0.01), 1).ToString() + " J";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 31: //Amiodarona
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo3.Replace("@TAM_PAS", this._urgenciapediatricaatendimentoselecionado.TamanhoPa);
                    pItem.Valor = Math.Round((this._pesoinformado * 0.1 + 0.01), 1).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 32: //Lidocaina
                    pItem.Grupo = pItem.Grupo + " | " + pItem.Titulo3.Replace("@TAM_PAS", this._urgenciapediatricaatendimentoselecionado.TamanhoPa);
                    pItem.Valor = (TestaMultiplicacaoMaior(0.1, this._pesoinformado, 20) ? 20 : Math.Round(this._pesoinformado * 0.1 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
                case 33: //Morfina                    
                    pItem.Valor = (TestaMultiplicacaoMaior(0.1, this._pesoinformado, 10) ? 10 : Math.Round(this._pesoinformado * 0.1 + 0.01, 1)).ToString() + " ml";
                    this._urgenciaitems.Add(pItem);
                    break;
            }
        }

        private void CalculaItensCabecalho(ItemUrgenciaCabecalho pItem)
        {
            switch (pItem.Item.Id)
            {
                case 1:
                    this.CalculaTubo(pItem);
                    break;
                case 2:
                    this.CalculaLaringoscopio(pItem);
                    break;
                case 3:
                    this.CalculaInsercao(pItem);
                    break;
                case 5:
                    this.CalculaSuccao(pItem);
                    break;
                default:
                    this._urgenciaitemscabecalho1.Add(pItem);
                    break;
            }
        }

        private void CalculaSuccao(ItemUrgenciaCabecalho pItem)
        {
            string succao = string.Empty;
            if (this._atendimento.Paciente.Idade.Years <= 0)
            {
                if (this._atendimento.Paciente.Idade.Months < 12)
                    succao = "6";
                else
                    succao = "8";
            }
            else
            {
                succao = "10";
            }

            pItem.Valor = succao + " Fr";
            this._urgenciaitemscabecalho1.Add(pItem);
        }

        private void CalculaInsercao(ItemUrgenciaCabecalho pItem)
        {
            string insercao = string.Empty;
            if (this._atendimento.Paciente.Idade.Years <= 0)
            {
                if (this._atendimento.Paciente.Idade.Months < 1)
                    insercao = "9";
                else if (this._atendimento.Paciente.Idade.Months >= 1 && this._atendimento.Paciente.Idade.Months <= 6)
                    insercao = "10,5";
                else if (this._atendimento.Paciente.Idade.Months >= 6 && this._atendimento.Paciente.Idade.Months <= 12)
                    insercao = "12";
                else if (this._atendimento.Paciente.Idade.Months > 12)
                    insercao = "13";
            }
            else
            {
                insercao = ((this._atendimento.Paciente.Idade.Years / 2) + 12).ToString();
            }

            pItem.Valor = insercao + " cm";
            this._urgenciaitemscabecalho1.Add(pItem);
        }

        private void CalculaLaringoscopio(ItemUrgenciaCabecalho pItem)
        {
            string laringo = string.Empty;
            if (this._atendimento.Paciente.Idade.Years <= 0)
            {
                laringo = "1";
            }
            else
            {
                if (this._atendimento.Paciente.Idade.Years <= 3)
                    laringo = "1";
                else if (this._atendimento.Paciente.Idade.Years > 3 && this._atendimento.Paciente.Idade.Years <= 8)
                    laringo = "2";
                else if (this._atendimento.Paciente.Idade.Years > 8)
                    laringo = "3";
            }

            pItem.Valor = laringo;
            this._urgenciaitemscabecalho1.Add(pItem);
        }

        private void CalculaTubo(ItemUrgenciaCabecalho pItem)
        {
            string tubo = string.Empty;
            if (this._atendimento.Paciente.Idade.Years <= 0)
            {
                if (this._atendimento.Paciente.Idade.Months < 1)
                    tubo = "3";
                else if (this._atendimento.Paciente.Idade.Months >= 1 && this._atendimento.Paciente.Idade.Months < 6)
                    tubo = "3,5";
                else if (this._atendimento.Paciente.Idade.Months >= 6 && this._atendimento.Paciente.Idade.Months < 12)
                    tubo = "3,5-4,0";
                else if (this._atendimento.Paciente.Idade.Months >= 12)
                    tubo = "4,0-4,5";
            }
            else
            {
                if (this._atendimento.Paciente.Idade.Years > 2 && this._atendimento.Paciente.Idade.Years < 5)
                    tubo = "4,5-5,0";
                else if (this._atendimento.Paciente.Idade.Years >= 5 && this._atendimento.Paciente.Idade.Years < 7)
                    tubo = "5,0-5,5";
                else if (this._atendimento.Paciente.Idade.Years >= 7 && this._atendimento.Paciente.Idade.Years < 9)
                    tubo = "5,4-6,0";
                else if (this._atendimento.Paciente.Idade.Years >= 9 && this._atendimento.Paciente.Idade.Years < 11)
                    tubo = "6,0-6,5";
                else if (this._atendimento.Paciente.Idade.Years >= 11 && this._atendimento.Paciente.Idade.Years < 13)
                    tubo = "6,5-7,0";
                else if (this._atendimento.Paciente.Idade.Years >= 13)
                    tubo = "7,0-7,5";
            }

            pItem.Valor = tubo;
            this._urgenciaitemscabecalho1.Add(pItem);
        }

        #endregion

        #region Commands
        public ICommand AddUrgenciaPediatricaCommand { get; set; }
        #endregion

        public class ItemUrgenciaCabecalho
        {
            public UrgenciaPediatricaItem Item { get; set; }
            public string Descricao { get; set; }
            public string Valor { get; set; }
        }
        public class ItemUrgencia
        {
            public UrgenciaPediatricaItem Item { get; set; }
            public string Grupo { get; set; }
            public string Droga { get; set; }
            public string Apresentacao { get; set; }
            public string Dose { get; set; }
            public string Valor { get; set; }
            public string Titulo1 { get; set; }
            public string Titulo2 { get; set; }
            public string Titulo3 { get; set; }
            public int Ordem { get; set; }
        }
        public enum CabecalhoItem
        {
            [Description("1")]
            Tubotraqueal,
            [Description("2")]
            LaminaLaringo,
            [Description("3")]
            Insercao,
            [Description("4")]
            AMBU,
            [Description("5")]
            SondaSuccao,
            [Description("6")]
            FluxoO2
        }


    }
}
