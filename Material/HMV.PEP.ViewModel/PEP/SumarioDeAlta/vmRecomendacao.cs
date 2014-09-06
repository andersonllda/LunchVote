using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmRecomendacao : ViewModelBase
    {
        #region Contrutor
        public vmRecomendacao(wrpSumarioAlta pSumarioAlta, Usuarios pUsuarios, bool pIsRN)
        {
            this.SumarioAlta = pSumarioAlta;
            this._usuarios = new wrpUsuarios(pUsuarios);

            this._isRN = pIsRN;

            if (this.SumarioAlta.DomainObject.ProcedimentosAlta == null)
                this.SumarioAlta.recomendacoes = new wrpSumarioRecomendacaoCollection(new List<SumarioRecomendacao>());

            this._sumarioRecomendacoes = this.SumarioAlta.recomendacoes;            
        }
        #endregion

        #region Propriedades Publicas
        public wrpUsuarios Usuarios
        {
            get
            {
                return this._usuarios;
            }
        }
        
        public wrpSumarioAlta SumarioAlta { get; set; }
        private wrpSumarioRecomendacaoCollection _sumarioRecomendacoes { get; set; }
        
        public string RevisaoMedicaEm
        {
            get
            {
                return SumarioAlta.RevisaoMedicaEm;
            }
            set
            {
                this.SumarioAlta.RevisaoMedicaEm = value;
                this.OnPropertyChanged("RevisaoMedicaEm");
            }
        }

        public string EmCasoDeUrgencia
        {
            get
            {
                return this.SumarioAlta.EmCasoDeUrgencia;
            }
            set
            {
                this.SumarioAlta.EmCasoDeUrgencia = value;
                this.OnPropertyChanged("EmCasoDeUrgencia");
            }
        }

        public wrpProcedimentoAlta procedimentoAltaSelecionado
        {
            get
            {
                return _procedimentoAltaSelecionado;
            }
            set
            {
                _procedimentoAltaSelecionado = value;
                this.OnPropertyChanged("procedimentoAltaSelecionado");
            }
        }

        public SimNao SemRecomendacao
        {
            get
            {
                return this.SumarioAlta.SemRecomendacao;
            }
            set
            {                
                if (value == SimNao.Sim)
                    if (this.SumarioAlta.recomendacoes.Where(x=> !string.IsNullOrWhiteSpace(x.Descricao) && x.Recomendacao.Status == Status.Ativo).Count() == 0)
                        this.SumarioAlta.SemRecomendacao = value;
                    else
                    {
                        this.SumarioAlta.SemRecomendacao = SimNao.Nao;
                        DXMessageBox.Show("Não é possível marcar esta opção enquanto houver recomendações cadastradas.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                else
                    this.SumarioAlta.SemRecomendacao = value;

                this.OnPropertyChanged("SemRecomendacao");
            }
        }

        public wrpSumarioRecomendacaoCollection ListaDeSumarioRecomendacao
        {
            get
            {
                IList<SumarioRecomendacao> lista = new List<SumarioRecomendacao>();
                IRepositorioDeRecomendacao rep = ObjectFactory.GetInstance<IRepositorioDeRecomendacao>();                
                rep.OndeIsRN(this._isRN);

                foreach (var item in rep.List().OrderBy(x => x.Descricao).ToList())
                {
                    string descricao = string.Empty;
                    wrpSumarioRecomendacao sum = _sumarioRecomendacoes.FirstOrDefault(x => x.Recomendacao.Id == item.Id);
                    if (sum != null)
                        descricao = sum.Descricao;
                    lista.Add(new SumarioRecomendacao(item, descricao, this.SumarioAlta.Atendimento.DomainObject));
                }
                return new wrpSumarioRecomendacaoCollection(lista);
            }
        }
        #endregion

        #region Propriedades Privadas
        private wrpProcedimentoAlta _procedimentoAltaSelecionado;
        private wrpUsuarios _usuarios;
        private bool _isRN;
        #endregion

        #region Metodos

        public void AtualizaLista()
        {
            this.OnPropertyChanged("ListaDeSumarioRecomendacao");
        }

        public void SetaListaRecomendacao(wrpSumarioRecomendacao pSum)
        {
            wrpSumarioRecomendacao wrpSum = this.SumarioAlta.recomendacoes.FirstOrDefault(x => x.Recomendacao.Id == pSum.Recomendacao.Id);
            if (wrpSum != null)
            {
                if (pSum.Descricao.Length > 0)
                    wrpSum.Descricao = pSum.Descricao;
                else
                    wrpSum.Descricao = string.Empty;
            }
            else
            {
                if (pSum.Descricao == null)
                    pSum.Descricao = string.Empty;
                pSum.Atendimento = this.SumarioAlta.Atendimento;
                this.SumarioAlta.recomendacoes.Add(pSum);
                this.SumarioAlta.Save();
            }           
        }
        #endregion
    }
}
