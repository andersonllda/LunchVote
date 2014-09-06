using HMV.Core.Domain.Repository.PEP.Evolucao;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using HMV.Core.Framework.Extensions;
using System.Collections.Generic;
using HMV.Core.Domain.Model.PEP.EvolucaoNova;
using System.Linq;
using DevExpress.Xpf.Core;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmPEPEvolucaoPadrao : ViewModelBase
    {
        #region Contrutor
        public vmPEPEvolucaoPadrao(wrpPEPEvolucao pEvolucao)
        {
            _conselho = new wrpConselho(pEvolucao.Usuario.Prestador.DomainObject.Conselho);
            _evolucao = pEvolucao;            
            CarregaEvolucoes();
        }


        #endregion

        #region Propriedades Publicas
        public wrpPEPEvolucaoPadraoCollection EvolucoesPadrao { get { return _evolucoespadrao; } }
        public wrpPEPEvolucaoPadrao EvolucaoSelecionada
        {
            get
            {
                return _evolucaopadraoselecionada;
            }
            set
            {
                _evolucaopadraoselecionada = value;
                this.OnPropertyChanged<vmPEPEvolucaoPadrao>(x => x.EvolucaoSelecionada);
            }
        }

        public string Titulo
        {
            get { return _titulo; }
            set { _titulo = value; OnPropertyChanged<vmPEPEvolucaoPadrao>(x => x.Titulo); }
        }
        public string Descricao
        {
            get { return _descricao; }
            set { _descricao = value; OnPropertyChanged<vmPEPEvolucaoPadrao>(x => x.Descricao); }
        }
        #endregion

        #region Commands
        protected override bool CommandCanExecuteSalvar(object param)
        {
            return (_titulo.IsNotEmptyOrWhiteSpace() && _descricao.IsNotEmptyOrWhiteSpace());
        }
         
        protected override void CommandSalvar(object param)
        {
            var evol = new wrpPEPEvolucaoPadrao(_titulo, _descricao);
            evol.Usuario = _evolucao.Usuario;
            evol.Conselho = _conselho;
            evol.Save();
            CarregaEvolucoes();            
            //base.CommandSalvar(param);
        }
        protected override bool CommandCanExecuteExcluir(object param)
        {
            return _evolucaopadraoselecionada.IsNotNull();
        }
        protected override void CommandExcluir(object param)
        {
            IRepositorioDePEPEvolucaoPadrao rep = ObjectFactory.GetInstance<IRepositorioDePEPEvolucaoPadrao>();
            rep.Delete(_evolucaopadraoselecionada.DomainObject);
            CarregaEvolucoes();
        }
        #endregion

        #region Metodos
        private void CarregaEvolucoes()
        {
            IRepositorioDePEPEvolucaoPadrao rep = ObjectFactory.GetInstance<IRepositorioDePEPEvolucaoPadrao>();
            var ret = rep.OndeConselho(_conselho.cd_conselho).List();
            if (ret.HasItems())
                _evolucoespadrao = new wrpPEPEvolucaoPadraoCollection(ret.ToList());
            else
                _evolucoespadrao = new wrpPEPEvolucaoPadraoCollection(new List<PEPEvolucaoPadrao>());

            if (_evolucoespadrao.HasItems())
                _evolucaopadraoselecionada = _evolucoespadrao.FirstOrDefault();
            else
                _evolucaopadraoselecionada = null;

            _titulo = string.Empty;
            _descricao = string.Empty;

            OnPropertyChanged<vmPEPEvolucaoPadrao>(x => x.Descricao);
            OnPropertyChanged<vmPEPEvolucaoPadrao>(x => x.Titulo);
            OnPropertyChanged<vmPEPEvolucaoPadrao>(x => x.EvolucoesPadrao);
        }

        public void SetaEvolucaoPadrao()
        {
            if (_evolucaopadraoselecionada.IsNull())
            {
                DXMessageBox.Show("Selecione uma Evolução Padrão!", "Atenção", System.Windows.MessageBoxButton.OK);
                return;
            }
            _evolucao.Evolucao = _evolucaopadraoselecionada.Descricao;
        }
        #endregion

        #region Propriedades Privadas
        private wrpConselho _conselho;
        private wrpPEPEvolucaoPadraoCollection _evolucoespadrao;
        private wrpPEPEvolucaoPadrao _evolucaopadraoselecionada;
        private wrpPEPEvolucao _evolucao;       
        private string _titulo;
        private string _descricao;
        #endregion
    }
}
