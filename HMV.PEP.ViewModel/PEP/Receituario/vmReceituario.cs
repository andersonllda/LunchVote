namespace HMV.PEP.ViewModel.PEP.Receituario
{
    using HMV.Core.Domain.Model;
    using HMV.Core.Domain.Repository.PEP;
    using HMV.Core.Framework.ViewModelBaseClasses;
    using HMV.Core.Wrappers.CollectionWrappers;
    using HMV.Core.Wrappers.ObjectWrappers;
    using StructureMap;
    using HMV.Core.Framework.Extensions;
    using System;
    using HMV.Core.Domain.Enum;
    using System.Linq;
    using HMV.Core.Domain.Model.PEP;
    using DevExpress.Xpf.Core;
    using System.Windows;

    /// <summary>
    /// UserControl view model.
    /// </summary>
    public class vmReceituario : ViewModelBase
    {
        #region ----- Construtor -----
        public vmReceituario(Paciente pPaciente, Usuarios pUsuario, Atendimento pAtendimento)
        {
            _paciente = new wrpPaciente(pPaciente);
            _usuarios = new wrpUsuarios(pUsuario);
            if (pAtendimento.IsNotNull())
                _atendimento = new wrpAtendimento(pAtendimento);
            _meusreceituarios = true;

            CarregaReceituarios();

        }
        #endregion

        void CarregaReceituarios()
        {
            _receituariocollection = null;
            _receituarioselecionado = null;
            IRepositorioDeReceituario rep = ObjectFactory.GetInstance<IRepositorioDeReceituario>();
            if (_meusreceituarios)
                rep.OndeIdUsuarioIgual(_usuarios.cd_usuario);
            else
                rep.OndeIdUsuarioDiferente(_usuarios.cd_usuario);

            var ret = rep.FiltraAtivos().OndeIdPacienteIgual(_paciente.ID).List();
            if (ret.HasItems())
                _receituariocollection = new wrpReceituarioCollection(ret);

            if (_receituariocollection.HasItems())
                _receituarioselecionado = _receituariocollection.FirstOrDefault();

            OnPropertyChanged<vmReceituario>(x => x.ReceituarioCollection);
        }

        public void CarregaReceituariosPadrao()
        {
            _receituariopadraocollection = null;
            _receituariopadraoselecionado = null;
            IRepositorioDeReceituarioPadrao rep = ObjectFactory.GetInstance<IRepositorioDeReceituarioPadrao>();
            var ret = rep.OndeIdUsuarioIgual(_usuarios.cd_usuario).List();
            if (ret.HasItems())
                _receituariopadraocollection = new wrpReceituarioPadraoCollection(ret);

            if (_receituariopadraocollection.HasItems() && _receituariopadraocollection.Count == 1)
                _receituariopadraoselecionado = _receituariopadraocollection.FirstOrDefault();

            OnPropertyChanged<vmReceituario>(x => x.ReceituarioPadraoCollection);
        }

        public void SetaReceituarioPadrao()
        {
            _receituarionovo.Descricao = _receituariopadraoselecionado.Texto;
            OnPropertyChanged<vmReceituario>(x => x.ReceituarioNovo);
        }

        public void Novo()
        {
            _receituarionovo = new wrpReceituario(_tipoespecial ? TipoReceituario.Especial : TipoReceituario.Normal, _paciente.DomainObject, _usuarios.DomainObject);
            if (_atendimento.IsNotNull())
                _receituarionovo.Atendimento = _atendimento;
            OnPropertyChanged<vmReceituario>(x => x.ReceituarioNovo);
        }
        public void Copia()
        {
            _receituarionovo.Descricao = _receituarioselecionado.Descricao;
            _receituarionovo.TipoReceituario = _receituarioselecionado.TipoReceituario;
            OnPropertyChanged<vmReceituario>(x => x.ReceituarioNovo);
        }
        #region ----- Propriedades Privadas -----
        wrpPaciente _paciente;
        wrpUsuarios _usuarios;
        wrpAtendimento _atendimento;
        wrpReceituarioCollection _receituariocollection;
        wrpReceituario _receituarioselecionado;
        wrpReceituario _receituarionovo;
        wrpReceituarioPadraoCollection _receituariopadraocollection;
        wrpReceituarioPadrao _receituariopadraoselecionado;
        bool _meusreceituarios;
        bool _tipoespecial;
        #endregion
        #region ----- Propriedades Públicas -----
        public wrpReceituarioCollection ReceituarioCollection
        {
            get
            {
                return _receituariocollection;
            }
        }

        public wrpReceituario Receituario
        {
            get
            {
                return _receituarioselecionado;
            }
            set
            {
                _receituarioselecionado = value;
                OnPropertyChanged<vmReceituario>(x => x.Receituario);
            }
        }

        public wrpReceituario ReceituarioNovo
        {
            get
            {
                return _receituarionovo;
            }
            set
            {
                _receituarionovo = value;
                OnPropertyChanged<vmReceituario>(x => x.ReceituarioNovo);
            }
        }

        public wrpReceituarioPadraoCollection ReceituarioPadraoCollection
        {
            get
            {
                return _receituariopadraocollection;
            }
        }

        public wrpReceituarioPadrao ReceituarioPadrao
        {
            get
            {
                return _receituariopadraoselecionado;
            }
            set
            {
                _receituariopadraoselecionado = value;
                OnPropertyChanged<vmReceituario>(x => x.ReceituarioPadrao);
            }
        }

        public bool MeusReceituarios
        {
            get
            {
                return _meusreceituarios;
            }
            set
            {
                _meusreceituarios = value;
                CarregaReceituarios();
                OnPropertyChanged<vmReceituario>(x => x.MeusReceituarios);
                OnPropertyChanged<vmReceituario>(x => x.ReceituarioCollection);
            }
        }

        public bool TipoEspecial
        {
            get
            {
                return _tipoespecial;
            }
            set
            {
                _tipoespecial = value;
                OnPropertyChanged<vmReceituario>(x => x.TipoEspecial);
            }
        }

        public string DescricaoPadrao { get; set; }
        public string TextoPadrao { get; set; }
        #endregion

        
       public void SalvaNovoReceituarioPadrao()
        {
            if (TextoPadrao.IsNotEmptyOrWhiteSpace() && DescricaoPadrao.IsNotEmptyOrWhiteSpace())
            {
                IRepositorioDeReceituarioPadrao rep = ObjectFactory.GetInstance<IRepositorioDeReceituarioPadrao>();
                var novo = new ReceituarioPadrao(_usuarios.DomainObject, DescricaoPadrao, TextoPadrao);
                rep.Save(novo);
                DescricaoPadrao = string.Empty;
                TextoPadrao = string.Empty;
                CarregaReceituariosPadrao();
            }
        }           

        public void ExcluiReceituarioPadrao()
        {
            if (_receituariopadraoselecionado.IsNotNull())
            {
                IRepositorioDeReceituarioPadrao rep = ObjectFactory.GetInstance<IRepositorioDeReceituarioPadrao>();
                var ret = rep.OndeIdIgual(_receituariopadraoselecionado.ID).Single();
                rep.Delete(ret);
                CarregaReceituariosPadrao();
            }
        }

        protected override bool CommandCanExecuteExcluir(object param)
        {
            return (_meusreceituarios && _receituarioselecionado.IsNotNull());
        }


        protected override void CommandExcluir(object param)
        {
            if (DXMessageBox.Show("Deseja Realmente Excluir este Receituário?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IRepositorioDeReceituario rep = ObjectFactory.GetInstance<IRepositorioDeReceituario>();
                var ret = rep.OndeIdIgual(_receituarioselecionado.ID).Single();
                ret.DataCancelamento = DateTime.Now;
                rep.Save(ret);
                CarregaReceituarios();
            }
        }

        protected override bool CommandCanExecuteAbrir(object param)
        {
            return (_meusreceituarios && _receituarioselecionado.IsNotNull());
        }

        protected override void CommandAbrir(object param)
        {
            //base.CommandAbrir(param);
        }

        protected override bool CommandCanExecuteSalvar(object param)
        {
            return (_receituarionovo.IsNotNull() && _receituarionovo.Descricao.IsNotEmptyOrWhiteSpace());
        }

        protected override void CommandSalvar(object param)
        {
            Salvar();

            base.CommandSalvar(param);
        }

        public void Salvar()
        {
            IRepositorioDeReceituario rep = ObjectFactory.GetInstance<IRepositorioDeReceituario>();
            rep.Save(_receituarionovo.DomainObject);

            CarregaReceituarios();

            _receituarioselecionado = _receituarionovo;
            //_receituarionovo = null;
        }

        protected override bool CommandCanExecuteImprimir(object param)
        {
            return (_receituarionovo.IsNotNull() && _receituarionovo.Descricao.IsNotEmptyOrWhiteSpace());
        }

        protected override bool CommandCanExecuteAbrirEx(object param)
        {
            return (_atendimento.IsNotNull() && _atendimento.SumarioAlta.IsNotNull() && _atendimento.SumarioAlta.PlanoPosAlta.HasItems());
        }

        protected override bool CommandCanExecuteAlterar(object param)
        {
            return this.Receituario.IsNotNull();
        }

        protected override void CommandAbrirEx(object param)
        {
            var conca = string.Empty;
            foreach (var pos in _atendimento.SumarioAlta.PlanoPosAlta)
            {
                conca += pos.Medicamento + Environment.NewLine + (pos.Dose.IsNotEmptyOrWhiteSpace() ? pos.Dose : string.Empty)
                                         + (pos.Via.IsNotEmptyOrWhiteSpace() ? ", " + pos.Via : string.Empty)
                                         + (pos.Frequencia.IsNotEmptyOrWhiteSpace() ? ", " + pos.Frequencia : string.Empty)
                                         + (pos.Tempo.IsNotEmptyOrWhiteSpace() ? ", " + pos.Tempo : string.Empty) + Environment.NewLine + Environment.NewLine;
            }
            _receituarionovo.Descricao = conca + _receituarionovo.Descricao;
        }

    }
}
