using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using HMV.Core.DTO;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoM
{
    public class vmNotasAdicionaisUC : ViewModelBase
    {
        #region ----- Construtor -----
        public vmNotasAdicionaisUC(wrpSumarioAvaliacaoMedica pSumarioAvaliacaoMedica, string pPropriedade)
        {
            this._sumarioavaliacaomedica = pSumarioAvaliacaoMedica;
            this._nomepropriedade = pPropriedade;
            this._atualizalista();
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedica _sumarioavaliacaomedica { get; set; }
        private ObservableCollection<PropriedadeClasse> _propriedadescollection { get; set; }
        private string _notaadicional { get; set; }
        private PropriedadeClasse _propriedade { get; set; }
        private string _nomepropriedade { get; set; }
        private bool _isstring { get; set; }
        private string _nomepropriedadecollection { get; set; }
        private IList<SumarioAvaliacaoMedicaItensDetalheDTO> _collection { get; set; }
        private int _tamanhomaximo { get; set; }
        private bool _habilitatexto { get; set; }
        #endregion

        #region ----- Propriedades Públicas -----
        public ObservableCollection<PropriedadeClasse> PropriedadesCollection
        {
            get { return this._propriedadescollection; }
        }

        public PropriedadeClasse Propriedade
        {
            get { return this._propriedade; }
            set
            {
                this._propriedade = value;
                this._habilitatexto = true;

                if (value != null)
                    if (value.Valor.Contains("**Informações Adicionais:"))
                    {
                        var texto = value.Valor.Mid(value.Valor.IndexOf('*'));
                        if (texto.Length < 1000)
                        {
                            if ((900 - texto.Length) > 100)
                                this._tamanhomaximo = 900 - texto.Length;
                            else
                                this._habilitatexto = false;
                        }
                        else
                            this._habilitatexto = false;
                    }
                    else
                    {
                        this._tamanhomaximo = 900;
                    }

                this.OnPropertyChanged(ExpressionEx.PropertyName<vmNotasAdicionaisUC>(x => x.TamanhoMaximo));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmNotasAdicionaisUC>(x => x.HabilitaTexto));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmNotasAdicionaisUC>(x => x.Propriedade));
            }
        }

        public int TamanhoMaximo
        {
            get
            {
                return this._tamanhomaximo;
            }
        }

        public bool HabilitaTexto
        {
            get
            {
                return this._habilitatexto;
            }
        }

        public string NotaAdicional
        {
            get { return this._notaadicional; }
            set
            {
                this._notaadicional = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmNotasAdicionaisUC>(x => x.NotaAdicional));
            }
        }
        #endregion

        #region ----- Métodos Privados -----
        private void _atualizalista()
        {
            this._propriedadescollection = new ObservableCollection<PropriedadeClasse>();

            PropertyInfo prop = this._sumarioavaliacaomedica.GetType().GetProperty(this._nomepropriedade);
            if (prop.IsNotNull())
                if (prop.GetValue(this._sumarioavaliacaomedica, null).GetType() == typeof(string))
                {
                    PropriedadeClasse novo = new PropriedadeClasse();
                    novo.NomePropriedade = prop.Name;
                    novo.Valor = prop.GetValue(this._sumarioavaliacaomedica, null).ToNullSafeString();
                    novo.Ordem = 0;
                    this._propriedadescollection.Add(novo);
                    this.OnPropertyChanged(ExpressionEx.PropertyName<vmNotasAdicionaisUC>(x => x.PropriedadesCollection));
                    this._isstring = true;
                }
                else if (prop.GetValue(this._sumarioavaliacaomedica, null).GetType().GetBaseTree().Where(x => x == typeof(ViewModelBase)).Count() > 0)
                {
                    object obj = prop.GetValue(this._sumarioavaliacaomedica, null);

                    int ord = 0;

                    foreach (var p in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.DeclaringType == obj.GetType()))
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            PropriedadeClasse novo = new PropriedadeClasse();
                            novo.NomePropriedade = p.Name;
                            novo.Valor = p.GetValue(obj, null).ToNullSafeString();
                            novo.Ordem = ord++;
                            this._propriedadescollection.Add(novo);
                        }

                        if (p.PropertyType == typeof(IList<SumarioAvaliacaoMedicaItensDetalheDTO>))
                        {
                            this._collection = p.GetValue(obj, null).CastSafeTo<IList<SumarioAvaliacaoMedicaItensDetalheDTO>>();
                            this._nomepropriedadecollection = p.Name;

                            foreach (var itemcol in this._collection.OrderBy(x => x.Ordem))
                            {
                                PropriedadeClasse novo = new PropriedadeClasse();
                                novo.IsCollection = true;
                                novo.Ordem = ord++;
                                foreach (var p2 in itemcol.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.DeclaringType == itemcol.GetType()))
                                {
                                    if (p2.Name == "ID")
                                        novo.ID = p2.GetValue(itemcol, null).ToNullSafeString().ToInt();
                                    if (p2.Name == "Descricao")
                                        novo.NomePropriedade = p2.GetValue(itemcol, null).ToNullSafeString();
                                    if (p2.Name == "Observacoes")
                                        novo.Valor = p2.GetValue(itemcol, null).ToNullSafeString();
                                }
                                this._propriedadescollection.Add(novo);
                            }
                        }
                    }

                    this.OnPropertyChanged(ExpressionEx.PropertyName<vmNotasAdicionaisUC>(x => x.PropriedadesCollection));
                }
        }
        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----
        protected override void CommandIncluir(object param)
        {
            this._propriedade.Valor += String.Format(
                Environment.NewLine +
                " **Informações Adicionais: Data - {0} Hora - {1} ** {2}",
                DateTime.Today.ToShortDateString(),
                DateTime.Now.ToShortTimeString(),
                Environment.NewLine + this._notaadicional);

            if (this._isstring)
                this._sumarioavaliacaomedica.GetType().GetProperty(this._nomepropriedade).SetValue(this._sumarioavaliacaomedica, this._propriedade.Valor, null);
            else
            {
                if (!this._propriedade.IsCollection)
                {
                    object obj = this._sumarioavaliacaomedica.GetType().GetProperty(this._nomepropriedade).GetValue(this._sumarioavaliacaomedica, null);
                    obj.GetType().GetProperty(this._propriedade.NomePropriedade).SetValue(obj, this._propriedade.Valor, null);
                }
                else
                {
                    this._collection.Where(x => x.ID == this._propriedade.ID).Single().Observacoes = this._propriedade.Valor;

                    object obj = this._sumarioavaliacaomedica.GetType().GetProperty(this._nomepropriedade).GetValue(this._sumarioavaliacaomedica, null);
                    obj.GetType().GetProperty(this._nomepropriedadecollection).SetValue(obj, this._collection, null);
                }
            }

            this._sumarioavaliacaomedica.Save();
            this._notaadicional = string.Empty;
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmNotasAdicionaisUC>(x => x.NotaAdicional));
            this._atualizalista();
        }

        protected override bool CommandCanExecuteIncluir(object param)
        {
            if (string.IsNullOrWhiteSpace(this._notaadicional) || this._propriedade.IsNull())
                return false;
            return true;
        }
        #endregion

        public class PropriedadeClasse
        {
            public int ID { get; set; }
            public string NomePropriedade { get; set; }
            public string Valor { get; set; }
            public int Ordem { get; set; }
            public bool IsCollection { get; set; }
        }
    }
}
