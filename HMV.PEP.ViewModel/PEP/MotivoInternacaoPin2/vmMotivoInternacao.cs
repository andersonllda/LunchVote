using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository;
using StructureMap;
using System.Linq;
using HMV.Core.Framework.Expression;
using System.Windows.Media;
using System.Collections.Generic;
using System;
using HMV.Core.Domain.Enum;
namespace HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2
{
    public class vmMotivoInternacao : ViewModelBase
    {
        #region ----- Construtor -----
        public vmMotivoInternacao(vmMotivoInternacaoPim2 pvmMotivoInternacaoPin2)
        {
            this._vmMotivoInternacaoPin2 = pvmMotivoInternacaoPin2;
            IRepositorioMotivoItem rep = ObjectFactory.GetInstance<IRepositorioMotivoItem>();
            this._motivoitenscollecions = new wrpMotivoSubItemCollection(rep.List().SelectMany(x => x.SubItens).OrderBy(x => x.Descricao).ToList());
            Inicializa();
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmMotivoInternacaoPim2 _vmMotivoInternacaoPin2;
        private wrpMotivoSubItemCollection _motivoitenscollecions;
        private wrpMotivoSubItem _motivoitem;
        private wrpMotivoInternacao _motivointernacao;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpMotivoSubItemCollection MotivosSubItensCollecions
        {
            get { return this._motivoitenscollecions; }
        }

        public wrpMotivoSubItem MotivoItem
        {
            get { return this._motivoitem; }
            set
            {
                this._motivoitem = value;

                if (this._motivoitem != null)
                {
                    if (this._motivoitem.CorBack == Brushes.White)
                    {
                        this._motivointernacao = new wrpMotivoInternacao(this._vmMotivoInternacaoPin2.Atendimento.DomainObject, this._vmMotivoInternacaoPin2.Usuario.DomainObject);
                        this._motivointernacao.SubItem = value;
                        this._motivoitem.CorBack = Brushes.LightGreen;
                        this._vmMotivoInternacaoPin2.Atendimento.MotivoInternacao.Add(_motivointernacao);
                    }
                    else
                    {
                        this._motivoitem.CorBack = Brushes.White;
                        var esta_VAR_Serve_Somente_Para_Remover_o_item = this._vmMotivoInternacaoPin2.Atendimento.MotivoInternacao;
                        esta_VAR_Serve_Somente_Para_Remover_o_item.Remove(esta_VAR_Serve_Somente_Para_Remover_o_item.FirstOrDefault(x => x.SubItem.ID == value.ID));
                    }
                }

                OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacao>(x => x.QtdTotalSelecionada));
                OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacao>(x => x.MotivoItem));
            }
        }

        public wrpMotivoInternacaoCollection MotivoInternacaoCollection
        {
            get { return this._vmMotivoInternacaoPin2.Atendimento.MotivoInternacao; }
        }

        public int QtdTotalSelecionada
        {
            get { return this._vmMotivoInternacaoPin2.Atendimento.DomainObject.MotivosInternacao.Count; }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        public void Inicializa()
        {
            foreach (wrpMotivoSubItem item in this._motivoitenscollecions)
            {
                if (this._vmMotivoInternacaoPin2.Atendimento.DomainObject.MotivosInternacao.Count(x => x.SubItem.ID == item.ID) > 0)
                    item.CorBack = Brushes.LightGreen;
                else
                    item.CorBack = Brushes.White;
            }
            OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacao>(x => x.MotivosSubItensCollecions));
        }

        #endregion

        #region ----- relatorio -----
        public List<rMotivoInternacao> RelMotivoInternacao
        {
            get
            {
                List<rMotivoInternacao> qry = null;
                if (this._vmMotivoInternacaoPin2.Atendimento.MotivoInternacao.Count == 0)
                    qry = null;
                else
                    qry = (from Lista in this._vmMotivoInternacaoPin2.Atendimento.MotivoInternacao
                           select new rMotivoInternacao
                           {
                               Classificacao = Lista.SubItem.MotivoItem.Descricao,
                               MotivoItem = Lista.SubItem.Descricao,
                               Usuario = Lista.Usuario.Nome.ToString(),
                               DataInclusao = Lista.DataInclusao.ToString("dd/MM/yyyy hh:mm"),
                           }).ToList();

                return qry;
            }
        }
        #endregion
        #region ClassesRelatorio
        public class rMotivoInternacao
        {
            public string Classificacao { get; set; }
            public string MotivoItem { get; set; }
            public string Usuario { get; set; }
            public string DataInclusao { get; set; }
        }
        #endregion

    }
}
