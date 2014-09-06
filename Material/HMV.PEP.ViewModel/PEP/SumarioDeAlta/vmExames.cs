using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.ViewModel.Commands;
using StructureMap;
using HMV.Core.Framework.Expression;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmExames : ViewModelBase
    {
        #region Contrutor
        public vmExames(wrpSumarioAlta pSumarioAlta, Usuarios pUsuarios)
        {
            this.SumarioAlta = pSumarioAlta;
            this._usuarios = pUsuarios;
            this.AddExameCommand = new AddExameCommand(this);
            this.RemoveExameCommand = new RemoveExameCommand(this);

            //if (this.SumarioAlta.DomainObject.SumarioExames == null)
            //    this.SumarioAlta.SumarioExames = new wrpSumarioExameCollection(new List<SumarioExame>());
            this.Exames = this.SumarioAlta.SumarioExames;

            if (this.SumarioAlta.SumarioExames.Count() == 0 && this.SumarioAlta.DomainObject.SemParticularidadeExames == null)
            {
                IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
                foreach (var item in rep.ondeProcedimentoEstaNaContaAmbulatorial(this.SumarioAlta.Atendimento.DomainObject).List().Distinct())
                    this.SumarioAlta.SumarioExames.Add(new wrpSumarioExame(item) { Atendimento = this.SumarioAlta.Atendimento });

                rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
                foreach (var item in rep.ondeProcedimentoEstaNaContaHospitalar(this.SumarioAlta.Atendimento.DomainObject).List().Distinct())
                    this.SumarioAlta.SumarioExames.Add(new wrpSumarioExame(item) { Atendimento = this.SumarioAlta.Atendimento });

                if (this.SumarioAlta.SumarioExames.Count > 0)
                {
                    this.SumarioAlta.DomainObject.SemExamesRealizados = null;
                    this.SumarioAlta.DomainObject.SemParticularidadeExames = SimNao.Nao;
                }
                else
                {
                    this.SumarioAlta.DomainObject.SemExamesRealizados = SimNao.Nao;
                    this.SumarioAlta.DomainObject.SemParticularidadeExames = null;
                }
            }
        }
        #endregion

        #region Propriedades Publicas
        public wrpSumarioAlta SumarioAlta { get; set; }
        public wrpSumarioExameCollection Exames { get; set; }

        public string TituloExamesRealizados
        {
            get
            {               
                return "Não foram realizados exames durante internação";             
            }
        }

        public SimNao? ExamesRealizados
        {
            get
            {               
                return this.SumarioAlta.SemExamesRealizados == null ? SimNao.Nao : this.SumarioAlta.SemExamesRealizados.Value;
            }
            set
            {
                if (value == SimNao.Sim && this.Exames.Count > 0)
                {
                    DXMessageBox.Show("Não é possível marcar esta opção enquanto houver Exames cadastrados.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    this.SumarioAlta.SemExamesRealizados = SimNao.Nao;
                }
                else
                    this.SumarioAlta.SemExamesRealizados = value;

                this.OnPropertyChanged("ExamesRealizados");
            }
        }

        public wrpSumarioExame ExameSelecionado
        {
            get
            {
                return this._exameselecionado;
            }
            set
            {
                this._exameselecionado = value;
                this.OnPropertyChanged("ExameSelecionado");
            }
        }

        public Procedimento ProcedimentoSelecionado
        {
            get
            {
                return this._procedimentoselecionado;
            }
            set
            {
                this._procedimentoselecionado = value;
                this.OnPropertyChanged("ProcedimentoSelecionado");
            }
        }

        public string Observacao
        {
            get
            {
                return this.SumarioAlta.ExameObservacao;
            }
            set
            {
                this.SumarioAlta.ExameObservacao = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExames>(x => x.Observacao));
            }
        }

        public string AnaliseClinica
        {
            get
            {
                return this.SumarioAlta.AnaliseClinica;
            }
            set
            {
                this.SumarioAlta.AnaliseClinica = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmExames>(x => x.AnaliseClinica));
            }
        }

        public IList<Procedimento> ExamesItens
        {
            get
            {
                IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
                rep.ondeProcedimentoIsExame();
                return rep.List().OrderBy(x => x.Descricao).ToList();
            }
        }
        #endregion

        #region Commands
        public ICommand AddExameCommand { get; set; }
        public ICommand RemoveExameCommand { get; set; }
        #endregion

        #region Propriedades Privadas
        wrpSumarioExame _exameselecionado { get; set; }
        Procedimento _procedimentoselecionado { get; set; }
        Usuarios _usuarios { get; set; }
        #endregion

        #region Metodos
        public void SetaExame()
        {
            if (this._procedimentoselecionado != null)
            {
                this.ExameSelecionado = new wrpSumarioExame((this._procedimentoselecionado));
                this.ExameSelecionado.Atendimento = this.SumarioAlta.Atendimento;
                this.AddExameCommand.Execute(null);
            }
        }

        //public void ProcedimentoContas()
        //{
        //    if (this.SumarioAlta.SumarioExames.Count() == 0 && this.SumarioAlta.DomainObject.SemParticularidadeExames == null )
        //    {
        //        IRepositorioDeProcedimentoSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
        //        foreach (var item in rep.ondeProcedimentoEstaNaContaAmbulatorial(this.SumarioAlta.Atendimento.DomainObject).List().Distinct())
        //            this.SumarioAlta.SumarioExames.Add(new wrpSumarioExame(item) { Atendimento = this.SumarioAlta.Atendimento });

        //        rep = ObjectFactory.GetInstance<IRepositorioDeProcedimentoSumarioAlta>();
        //        foreach (var item in rep.ondeProcedimentoEstaNaContaHospitalar(this.SumarioAlta.Atendimento.DomainObject).List().Distinct())
        //            this.SumarioAlta.SumarioExames.Add(new wrpSumarioExame(item) { Atendimento = this.SumarioAlta.Atendimento });

        //        if (this.SumarioAlta.SumarioExames.Count > 0)
        //        {
        //            this.SumarioAlta.DomainObject.SemExamesRealizados = null;
        //            this.SumarioAlta.DomainObject.SemParticularidadeExames = SimNao.Nao;
        //        }
        //        else
        //        {
        //            this.SumarioAlta.DomainObject.SemExamesRealizados = SimNao.Sim;
        //            this.SumarioAlta.DomainObject.SemParticularidadeExames = null;
        //        }
        //    }            
        //}

        #endregion

    }
}
