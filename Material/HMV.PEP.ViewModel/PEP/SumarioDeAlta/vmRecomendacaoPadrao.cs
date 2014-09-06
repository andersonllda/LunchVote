using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmRecomendacaoPadrao : ViewModelBase
    {
        #region Contrutor
        public vmRecomendacaoPadrao(vmRecomendacao pRecomendacao, bool pIsRN)
        {
            this.AddRecomendacaoPadraoCommand = new AddRecomendacaoPadraoCommand(this);
            this.AddRecomendacaoPadraoPrestadorCommand = new AddRecomendacaoPadraoPrestadorCommand(this);

            //IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            //rep.Refresh(pPrestador.DomainObject);
            //this.Prestador = pRecomendacao.Usuarios.Prestador;

            this.vmRecomendacao = pRecomendacao;
            this._titulo = string.Empty;

            if (pRecomendacao.Usuarios.Prestador.DomainObject.RecomendacaoPadrao == null)
                pRecomendacao.Usuarios.Prestador.RecomendacaoPadrao = new wrpRecomendacaoPadraoCollection(new List<RecomendacaoPadrao>());

            this.recomendacaoPadrao = pRecomendacao.Usuarios.Prestador.RecomendacaoPadrao;

            this._isRN = pIsRN;
        }
        #endregion

        #region Propriedades Publicas
        //public wrpPrestador Prestador { get; set; }       
        public vmRecomendacao vmRecomendacao { get; set; }
        public wrpRecomendacaoPadraoCollection recomendacaoPadrao { get; set; }
        public bool _isRN;

        public bool IsRN
        {
            get { return _isRN;}
        }

        public string NovoTitulo
        {
            get
            {
                return this._titulo;
            }
            set
            {
                this._titulo = value;
                this.OnPropertyChanged("NovoTitulo");
            }
        }

        public IList<Titulo> consultaTitulos
        {
            get
            {
                IList<Titulo> lista = new List<Titulo>();
                foreach (var item in recomendacaoPadrao.Where(x => x.IsRN == (_isRN ? SimNao.Sim : SimNao.Nao)).Select(x => x.Titulo).Distinct())
                    lista.Add(new Titulo() { Descricao = item });

                return lista;
            }
        }

        public Titulo tituloSelecionado
        {
            get
            {
                return _tituloSelecionado;
            }
            set
            {
                _tituloSelecionado = value;
                if (value != null)
                {
                    consultaRecomendacao = new List<wrpRecomendacaoPadrao>();
                    foreach (var item in recomendacaoPadrao.Where(x => x.Titulo == value.Descricao).ToList())
                        consultaRecomendacao.Add(new wrpRecomendacaoPadrao(item.DomainObject));
                }
                this.OnPropertyChanged("tituloSelecionado");
                this.OnPropertyChanged("consultaRecomendacao");
            }
        }

        public IList<wrpRecomendacaoPadrao> consultaRecomendacao { get; set; }
        #endregion

        public void Refresh()
        {
            this.OnPropertyChanged("consultaTitulos");
        }

        #region Propriedades Privadas
        Titulo _tituloSelecionado { get; set; }
        string _titulo { get; set; }
        #endregion

        #region Commands
        public ICommand AddRecomendacaoPadraoCommand { get; set; }
        public ICommand AddRecomendacaoPadraoPrestadorCommand { get; set; }
        #endregion

    }
    public class Titulo
    {
        public string Descricao { get; set; }
    }
}
