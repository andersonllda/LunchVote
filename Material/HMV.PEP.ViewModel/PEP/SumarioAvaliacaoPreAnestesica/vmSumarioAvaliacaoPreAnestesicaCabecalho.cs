using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.DTO;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Framework.Extensions;
using HMV.PEP.Interfaces;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica
{
    public class vmSumarioAvaliacaoPreAnestesicaCabecalho : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSumarioAvaliacaoPreAnestesicaCabecalho(wrpSumarioAvaliacaoPreAnestesica pSumarioAvaliacaoPreAnestesica)
        {
            this._sumarioavaliacaopreanestesica = pSumarioAvaliacaoPreAnestesica;
            if (pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.IsNotNull() && pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.IsNotNull())
            {
                IRepositorioDeSumarioAvaliacaoPreAnestesica rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoPreAnestesica>();
                _guiasenha = rep.BuscaGuiaSenha(pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.ID, pSumarioAvaliacaoPreAnestesica.AvisoCirurgia.cd_aviso_cirurgia);
            }
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoPreAnestesica _sumarioavaliacaopreanestesica { get; set; }
        private GuiaSenhaDTO _guiasenha;
        #endregion

        #region ----- Propriedades Públicas -----

        public string Cirurgiao
        {
            get { return this._sumarioavaliacaopreanestesica.PrestadorCirurgiao == null ? null : "[" + this._sumarioavaliacaopreanestesica.PrestadorCirurgiao.Registro + "] - " + this._sumarioavaliacaopreanestesica.PrestadorCirurgiao.NomeExibicao; } // .NomeExibicaoPrestador
        }

        public string Anestesista
        {
            get { return this._sumarioavaliacaopreanestesica.PrestadorAnestesia == null ? null : "[" + this._sumarioavaliacaopreanestesica.PrestadorAnestesia.Registro + "] - " + this._sumarioavaliacaopreanestesica.PrestadorAnestesia.NomeExibicao; } //.NomeExibicaoPrestador
        }

        public string CirurgiaProcedimento
        {
            get
            {
                if (_sumarioavaliacaopreanestesica.AvisoCirurgia == null || _sumarioavaliacaopreanestesica.AvisoCirurgia.ProcedimentosCirurgicos == null || _sumarioavaliacaopreanestesica.AvisoCirurgia.ProcedimentosCirurgicos.Count(x => x.Principal == SimNao.Sim) == 0)
                    return string.Empty;
                return _sumarioavaliacaopreanestesica.AvisoCirurgia.ProcedimentosCirurgicos.Single(x => x.Principal == SimNao.Sim).Cirurgia.ds_cirurgia;
            }
        }

        public string DataHoraCirurgia
        {
            get
            {
                if (this._sumarioavaliacaopreanestesica.AvisoCirurgia == null || this._sumarioavaliacaopreanestesica.AvisoCirurgia.dt_aviso_cirurgia == null)
                    return string.Empty;
                return this._sumarioavaliacaopreanestesica.AvisoCirurgia.dt_aviso_cirurgia.ToString("dd/MM/yyyy - HH:mm");
            }
        }

        public string CIDPrincipal
        {
            get
            {
                if (_sumarioavaliacaopreanestesica.AvisoCirurgia == null || _sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento == null || _sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.Cid == null)
                    return string.Empty;
                return this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.Cid.Id + " - " + this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.Cid.Descricao;
            }
        }

        public string CID
        {
            get
            {
                if (this._sumarioavaliacaopreanestesica.CID == null || this._sumarioavaliacaopreanestesica.CID.CidMV == null)
                    return string.Empty;
                return _sumarioavaliacaopreanestesica.CID.Id + " - " + _sumarioavaliacaopreanestesica.CID.Descricao;
            }
            set
            {
            }
        }

        public bool IsEnabledRemoveCid
        {
            get { return this._sumarioavaliacaopreanestesica.CID != null; }
        }

        public string Guia
        {
            get
            {
                if (this._guiasenha.IsNotNull())
                    return this._guiasenha.Guia;
                return string.Empty;
            }
        }

        public string Senha
        {
            get
            {
                if (this._guiasenha.IsNotNull())
                    return this._guiasenha.Senha;
                return string.Empty;
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        public void IncluirCid(wrpCid pCID)
        {
            if (this._sumarioavaliacaopreanestesica.CID == null)
                this._sumarioavaliacaopreanestesica.CID = new wrpCid();
            this._sumarioavaliacaopreanestesica.CID = pCID;
           
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaCabecalho>(x => x.CID));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaCabecalho>(x => x.IsEnabledRemoveCid));
        }

        public void RemoveCid()
        {
            this._sumarioavaliacaopreanestesica.CID = null;
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaCabecalho>(x => x.CID));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaCabecalho>(x => x.IsEnabledRemoveCid));
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
