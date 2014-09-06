using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using System.Drawing;
using System.Collections.Generic;
using HMV.Core.Framework.Validations;
using System.Collections.ObjectModel;
using NHibernate.Validator.Constraints;
using System.Linq;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using System;
using HMV.Core.Framework.Expression;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.ObjectWrappers.PEP.AdmissaoAssistencialCTI;
using HMV.Core.Wrappers.ObjectWrappers.PEP.ProcessosEnfermagem.AdmissaoAssistencialDeEndoscopia;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica
{
    public class vmExameFisicoEvento : ViewModelBase
    {
        #region ----- Construtor -----

        public vmExameFisicoEvento(wrpSumarioAvaliacaoPreAnestesica pSumarioAvaliacaoPreAnestesica) //, wrpEvento pEvento
        {
            this._examefisicocollection = new ObservableCollection<ExameFisicoDTO>();
                    
            wrpExameFisicoEventoCollection _SinaisVitaisCollection = null;
            IRepositorioDeExameFisicoEvento repp = ObjectFactory.GetInstance<IRepositorioDeExameFisicoEvento>();
            repp.OndeChaveIgual(pSumarioAvaliacaoPreAnestesica.ID);                 
            var ret = repp.List();
            if (ret.IsNotNull())
                _SinaisVitaisCollection = new wrpExameFisicoEventoCollection(ret);

            foreach (var item in _SinaisVitaisCollection.OrderBy(x=> x.SinaisVitaisTipo.Ordem))
            {                
                this._examefisicocollection.Add(new ExameFisicoDTO()
                {
                    Chave = item.Chave,
                    SinaisVitaisTipo = item.SinaisVitaisTipo,
                    Valor = item.Valor,
                    ID = item.ID 
                });
            }

            _sumario = pSumarioAvaliacaoPreAnestesica;

            BuscaValoresAdmissaoAssistencial(pSumarioAvaliacaoPreAnestesica);
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private ExameFisicoDTO _examefisico;
        private ObservableCollection<ExameFisicoDTO> _examefisicocollection;
        private wrpSumarioAvaliacaoPreAnestesica _sumario;
        #endregion

        #region ----- Propriedades Públicas -----
        public ObservableCollection<ExameFisicoDTO> ExamesFisicoCollection
        {
            get { return this._examefisicocollection; }
        }

        public ExameFisicoDTO ExameFisico
        {
            get { return _examefisico; }
            set { _examefisico = value; }
        }

        [ValidaMaximoEMinimo(1, 300)]
        public int Altura
        {
            get
            {
                if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.ALT.ToString()) > 0)
                    return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.ALT.ToString()).SingleOrDefault().Valor.ToInt();

                return 0;
            }
            set
            {
                this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.ALT.ToString()).SingleOrDefault().Valor = value.ToNullSafeString();
                AtualizaIMC_SC();
                this.OnPropertyChanged<vmExameFisicoEvento>(x => x.Altura);
            }
        }

        [ValidaMaximoEMinimo(1, 200)]
        public double Peso
        {
            get
            {
                if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.PESO.ToString()) > 0)
                    return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.PESO.ToString()).SingleOrDefault().Valor.ToDouble();

                return 0;
            }
            set
            {
                this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.PESO.ToString()).SingleOrDefault().Valor = value.ToNullSafeString();
                AtualizaIMC_SC();
                this.OnPropertyChanged<vmExameFisicoEvento>(x => x.Peso);
            }
        }

        [ValidaCampoObrigatorio()]
        public string PA
        {
            get
            {
                if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.PA.ToString()) > 0)
                    return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.PA.ToString()).SingleOrDefault().Valor;

                return string.Empty;
            }
            set
            {
                this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.PA.ToString()).SingleOrDefault().Valor = value;
                this.OnPropertyChanged<vmExameFisicoEvento>(x => x.PA);
            }
        }

        //[ValidaMaximoEMinimo(34, 42)]
        //public double TAX
        //{
        //    get
        //    {
        //        if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.TAX.ToString()) > 0)
        //            return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.TAX.ToString()).SingleOrDefault().Valor.ToInt();

        //        return 0;
        //    }
        //    set
        //    {
        //        this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.TAX.ToString()).SingleOrDefault().Valor = value.ToNullSafeString();
        //        this.OnPropertyChanged<vmExameFisicoEvento>(x => x.TAX);
        //    }
        //}
        [ValidaMaximoEMinimo(30, 300)]
        public double FC
        {
            get
            {
                if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.FC.ToString()) > 0)
                    return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.FC.ToString()).SingleOrDefault().Valor.ToDouble();

                return 0;
            }
            set
            {
                this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.FC.ToString()).SingleOrDefault().Valor = value.ToNullSafeString();
                this.OnPropertyChanged<vmExameFisicoEvento>(x => x.FC);
            }
        }

        //[ValidaMaximoEMinimo(0, 10)]
        //public double DOR
        //{
        //    get
        //    {
        //        if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.DOR.ToString()) > 0)
        //            return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.DOR.ToString()).SingleOrDefault().Valor.ToInt();

        //        return 0;
        //    }
        //    set
        //    {
        //        this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.DOR.ToString()).SingleOrDefault().Valor = value.ToNullSafeString();
        //        this.OnPropertyChanged<vmExameFisicoEvento>(x => x.DOR);
        //    }
        //}
        [ValidaMaximoEMinimo(0, 100)]
        public double SAT
        {
            get
            {
                if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.SAT.ToString()) > 0)
                    return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.SAT.ToString()).SingleOrDefault().Valor.ToDouble();

                return 0;
            }
            set
            {
                this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.SAT.ToString()).SingleOrDefault().Valor = value.ToNullSafeString();
                this.OnPropertyChanged<vmExameFisicoEvento>(x => x.SAT);
            }
        }

        //[ValidaMaximoEMinimo(5, 100)]
        //public double FR
        //{
        //    get
        //    {
        //        if (this._examefisicocollection.Count(x => x.SinaisVitaisTipo.Sigla == Siglas.FR.ToString()) > 0)
        //            return this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.FR.ToString()).SingleOrDefault().Valor.ToInt();

        //        return 0;
        //    }
        //    set
        //    {
        //        this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla == Siglas.FR.ToString()).SingleOrDefault().Valor = value.ToNullSafeString();
        //        this.OnPropertyChanged<vmExameFisicoEvento>(x => x.FR);
        //    }
        //}
        public double IMC
        {
            get
            {
                if (!this._examefisicocollection.HasItems())
                    return 0;

                double peso = 0;
                int altura = 0;
                if (double.TryParse(this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Descricao.Equals(Siglas.PESO.ToString())).Single().Valor, out peso))
                    if (int.TryParse(this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla.Equals(Siglas.ALT.ToString())).Single().Valor, out altura))
                        return Math.Round(peso / Math.Pow((double)altura / 100, 2), 2);
                return 0;
            }
        }

        public double SC
        {
            get
            {
                if (!this._examefisicocollection.HasItems())
                    return 0;

                double peso = 0;
                int altura = 0;
                if (double.TryParse(this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Descricao.Equals(Siglas.PESO.ToString())).Single().Valor, out peso))
                    if (int.TryParse(this._examefisicocollection.Where(x => x.SinaisVitaisTipo.Sigla.Equals(Siglas.ALT.ToString())).Single().Valor, out altura))
                        return Math.Round(0.007184 * Math.Pow(peso, 0.425) * Math.Pow((double)altura, 0.725), 2);
                return 0;
            }
        }

        #endregion

        #region ----- Métodos Privados -----
        private void BuscaValoresAdmissaoAssistencial(wrpSumarioAvaliacaoPreAnestesica pSumarioAvaliacaoPreAnestesica)
        {
            bool _importou = false;
            if (pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.IsNotNull() 
                && pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.IsNotNull() 
                && !pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.AdmissaoAssistencial.IsNull()
                && pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.AdmissaoAssistencial.Count(x => x.DataConclusao.IsNotNull() && x.DataConclusao.Value.AddHours(24) >= DateTime.Now) > 0)
            {
                var adassist = pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.AdmissaoAssistencial.Where(x => x.DataConclusao.IsNotNull()).Last();

                foreach (var item in this._examefisicocollection)
                {
                    if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        item.Valor = adassist.FrequenciaRespiratoria.HasValue ? adassist.FrequenciaRespiratoria.Value.ToString() : string.Empty;

                    else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PA.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        item.Valor = adassist.PA;

                    else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.TAX.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        item.Valor = adassist.TAX.HasValue ? adassist.TAX.Value.ToString() : string.Empty;

                    else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FC.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        item.Valor = adassist.FrequenciaCardiaca.HasValue ? adassist.FrequenciaCardiaca.Value.ToString() : string.Empty;

                    //else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.DOR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                    // item.Valor = adassist.Dor;

                    else if (item.SinaisVitaisTipo.Sigla.Equals(Siglas.ALT.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        item.Valor = adassist.Altura.HasValue ? adassist.Altura.Value.ToString() : string.Empty;

                    else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PESO.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        item.Valor = adassist.Peso.HasValue ? adassist.Peso.Value.ToString() : string.Empty;
                }
                _importou = true;
            }

            if (pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.IsNotNull() && !_importou && pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.IsNotNull())
            {
                wrpAdmissaoAssistencialCO admissaoCO = null;

                IRepositorioDeAdmissaoAssistencialCO rep = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialCO>();
                var ret = rep.OndeCodigoAtendimentoIgual(pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.DomainObject).List();
                if (ret.HasItems() && ret.Count(x => x.DataEncerramento.IsNotNull() && x.DataEncerramento.Value.AddHours(24) >= DateTime.Now) > 0)
                    admissaoCO = new wrpAdmissaoAssistencialCO(ret.Where(x => x.DataEncerramento.IsNotNull() && x.DataEncerramento.Value.AddHours(24) >= DateTime.Now).Last());

                if (admissaoCO.IsNotNull())
                {
                    foreach (var item in this._examefisicocollection)
                    {
                        if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCO.Admissao.FR.HasValue ? admissaoCO.Admissao.FR.Value.ToString() : string.Empty;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PA.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCO.Admissao.PAAlta + "/" + admissaoCO.Admissao.PABaixa;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.TAX.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCO.Admissao.Tax.HasValue ? admissaoCO.Admissao.Tax.Value.ToString() : string.Empty;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FC.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCO.Admissao.FC.HasValue ? admissaoCO.Admissao.FC.Value.ToString() : string.Empty;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.DOR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCO.Admissao.Dor.Value.GetEnumCustomDisplay();

                        else if (item.SinaisVitaisTipo.Sigla.Equals(Siglas.ALT.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCO.Admissao.Altura.HasValue ? admissaoCO.Admissao.Altura.Value.ToString() : string.Empty;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PESO.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCO.Admissao.Peso.HasValue ? admissaoCO.Admissao.Peso.Value.ToString() : string.Empty;
                    }
                    _importou = true;
                }
            }

            if (pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.IsNotNull() && !_importou && pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.IsNotNull())
            {
                wrpAdmissaoAssistencialCTINEO admissaoCTINEO = null;

                IRepositorioDeAdmissaoAssistencialCTINEO rep = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialCTINEO>();
                var ret = rep.OndeIdAtendimentoIgual(pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.DomainObject.ID).List();
                if (ret.HasItems() && ret.Count(x => x.DataEncerramento.IsNotNull() && x.DataEncerramento.Value.AddHours(24) >= DateTime.Now) > 0)
                    admissaoCTINEO = new wrpAdmissaoAssistencialCTINEO(ret.Where(x => x.DataEncerramento.IsNotNull() && x.DataEncerramento.Value.AddHours(24) >= DateTime.Now).Last());

                if (admissaoCTINEO.IsNotNull())
                {
                    foreach (var item in this._examefisicocollection)
                    {
                        if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.FR.HasValue ? admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.FR.Value.ToString() : string.Empty;

                        //else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PA.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        //item.Valor = admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.+ "/" + sumarioCO.SumarioAvaliacaoMedicaCOExameFisico.PABaixa;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.TAX.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.TAX.HasValue ? admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.TAX.Value.ToString() : string.Empty;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FC.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.FC.HasValue ? admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.FC.Value.ToString() : string.Empty;

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.DOR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoCTINEO.AdmissaoAssistencialExameFisicoCTINEO.Dor.Value.GetEnumCustomDisplay();

                        //else if (item.SinaisVitaisTipo.Sigla.Equals(Siglas.ALT.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        //item.Valor = sumarioCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Altura.HasValue ? sumarioCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.Altura.Value.ToString() : string.Empty;

                        //else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PESO.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                        //item.Valor = sumarioCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico.peso.HasValue ? sumarioCO.SumarioAvaliacaoMedicaCOExameFisico.Peso.Value.ToString() : string.Empty;
                    }
                    _importou = true;
                }
            }

            if (pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.IsNotNull() && !_importou && pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.IsNotNull())
            {
                wrpAdmissaoAssistencialEndoscopia admissaoEndoscopia = null;
                //IRepositorioDeSumarioDeAvaliacaoMedicaEndoscopia rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaEndoscopia>();                
                //var ret = rep.OndeIdAtendimentoIgual(pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.DomainObject.ID).Single();
                //if (ret.IsNotNull())
                if (pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.AdmissaoAssistencialEndoscopia.Count(x => x.DataExclusao.IsNull() 
                                                                      && x.DataEncerramento.HasValue && x.DataEncerramento.Value.AddHours(24) >= DateTime.Now) > 0)
                    admissaoEndoscopia = pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.AdmissaoAssistencialEndoscopia.Where(x => x.DataExclusao.IsNull() 
                                                                    && x.DataEncerramento.HasValue && x.DataEncerramento.Value.AddHours(24) >= DateTime.Now).Last();
                //new wrpSumarioAvaliacaoMedicaEndoscopia(ret);

                if (admissaoEndoscopia.IsNotNull())
                {
                    foreach (var item in this._examefisicocollection)
                    {
                        if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoEndoscopia.FR.ToNullSafeString();

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PA.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoEndoscopia.PA.Alta.ToNullSafeString() + "/" + admissaoEndoscopia.PA.Baixa.ToNullSafeString();

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.TAX.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoEndoscopia.TAX.ToNullSafeString();

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.FC.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoEndoscopia.FC.ToNullSafeString();

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.DOR.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoEndoscopia.DOR.ToNullSafeString();

                        else if (item.SinaisVitaisTipo.Sigla.Equals(Siglas.ALT.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoEndoscopia.Altura.ToNullSafeString();

                        else if (item.SinaisVitaisTipo.Descricao.Equals(Siglas.PESO.ToString()) && item.Valor.IsEmptyOrWhiteSpace())
                            item.Valor = admissaoEndoscopia.Peso.ToNullSafeString();
                    }
                    _importou = true;
                }
            }
        }

        private void AtualizaIMC_SC()
        {
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoEvento>(x => x.IMC));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmExameFisicoEvento>(x => x.SC));
        }
        #endregion

        #region ----- Métodos Públicos -----
        #endregion

        #region ----- Commands -----
        #endregion

        public class ExameFisicoDTO : wrpExameFisicoEvento
        {
            public override string this[string propertyName]
            {
                get
                {
                    if (!base.Valor.IsEmptyOrWhiteSpace())
                        if (!base.SinaisVitaisTipo.ValorMinimo.IsNull() || !base.SinaisVitaisTipo.ValorMaximo.IsNull())
                        {
                            decimal valoritem;
                            if (decimal.TryParse(base.Valor, out valoritem))
                            {
                                if (valoritem < base.SinaisVitaisTipo.ValorMaximo && valoritem > base.SinaisVitaisTipo.ValorMinimo)
                                {
                                    return base[propertyName];
                                }
                                return base.SinaisVitaisTipo.Mensagem;
                            }
                        }

                    return base[propertyName];
                }
            }
        }

        public void Salvar()
        {
            wrpExameFisicoEventoCollection _SinaisVitaisCollection = null;
            IRepositorioDeExameFisicoEvento repp = ObjectFactory.GetInstance<IRepositorioDeExameFisicoEvento>();
            repp.OndeChaveIgual(_sumario.ID);
            var ret = repp.List();
            if (ret.IsNotNull())
                _SinaisVitaisCollection = new wrpExameFisicoEventoCollection(ret);

            foreach (var exa in _examefisicocollection)
            {
                var jaexiste = _SinaisVitaisCollection.Where(x => x.Chave == _sumario.ID && x.ID == exa.ID).SingleOrDefault();
                if (jaexiste.IsNull())
                {
                    var novo = new wrpExameFisicoEvento
                    {
                        SinaisVitaisTipo = exa.SinaisVitaisTipo,
                        Chave = _sumario.ID,
                        Atendimento = _sumario.AvisoCirurgia.IsNotNull() ? _sumario.AvisoCirurgia.Atendimento : null,
                        Evento = new wrpEvento(new Evento() { ID = (int)Core.Domain.Enum.TipoEvento.SumarioAvaliacaoPreAnestesica }),
                        Data = DateTime.Now,
                        Usuario = _sumario.UsuarioInclusao,
                        Valor = exa.Valor
                    };
                    repp.Save(novo.DomainObject);
                }
                else
                {
                    jaexiste.Atendimento = _sumario.AvisoCirurgia.IsNotNull() ? _sumario.AvisoCirurgia.Atendimento : null;
                    //jaexiste.Data = DateTime.Now;
                    //jaexiste.Usuario = _sumario.UsuarioInclusao;
                    jaexiste.Valor = exa.Valor;
                    repp.Save(jaexiste.DomainObject);
                }
            }
        }
    }
}
