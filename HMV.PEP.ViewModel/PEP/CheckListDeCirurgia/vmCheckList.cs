using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using DevExpress.Xpf.Core;
using System.Windows;

namespace HMV.PEP.ViewModel.PEP.CheckListDeCirurgia
{
    public class vmCheckList : ViewModelBase
    {
        #region ----- Construtor -----
        public vmCheckList(Atendimento pAtendimento, Usuarios pUsuario)
        {
            this._atendimento = new wrpAtendimento(pAtendimento);
            this._usuarios = new wrpUsuarios(pUsuario);

            //var t = this._atendimento.DomainObject.DescricaoCirurgica;
            var checklistitens = this.CheckListCollection;
            if (checklistitens.HasItems() && checklistitens.Count == 1)
            {
                this.CheckListdto = checklistitens.FirstOrDefault();
            }

            if (_usuarios.Prestador.IsNotNull())
            {
                IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();
                Parametro par = rep.OndeCodigosDoConselhoParaEnfermeiras().Single();

                if (par.Valor.Split(',').Contains(this._usuarios.Prestador.TipoPrestador.Id.ToString()))
                {
                    _sondagem = this._checklistdto.IsNotNull()
                        && (this._checklistdto.CheckList.IsNull() || this._checklistdto.CheckList.Sondagem.IsNull()
                        || this._checklistdto.CheckList.Sondagem.DataEncerramento.IsNull())
                        && ((this._checklistdto.CheckList.IsNotNull() && this._checklistdto.CheckList.Sondagem.IsNotNull()
                        && this._checklistdto.CheckList.Sondagem.VesicalDemoraCirurgiao == SimNao.Nao) ||
                              this._checklistdto.CheckList.IsNull() || this._checklistdto.CheckList.Sondagem.IsNull());
                }
            }
            else
                _sondagem = false;

            if (this._atendimento.DomainObject.DescricaoCirurgica.Count.Equals(0))
                DXMessageBox.Show("Não existe 'Aviso Cirurgia' para este atendimento.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            this.getBotoes();
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpAtendimento _atendimento;
        private CheckListDTO _checklistdto;
        private wrpUsuarios _usuarios;
        private vmAntesDaEntrada _vmAntesDaEntrada;
        private vmAntesDaInducao _vmAntesDaInducao;
        private vmTimeOut _vmTimeOut;
        private vmCheckOut _vmCheckOut;
        private vmTransOperatorio _vmTransOperatorio;
        private vmSondagem _vmSondagem;
        private bool _sondagem;
        private bool TemCheckList { get { return this._checklistdto.IsNotNull() && this._checklistdto.CheckList.IsNotNull(); } }
        #endregion

        #region ----- Propriedades Públicas -----

        public vmAntesDaEntrada vmAntesDaEntrada
        {
            get { return this._vmAntesDaEntrada; }
        }

        public vmAntesDaInducao vmAntesDaInducao
        {
            get { return this._vmAntesDaInducao; }
        }

        public vmTimeOut vmTimeOut
        {
            get { return this._vmTimeOut; }
        }

        public vmCheckOut vmCheckOut
        {
            get { return this._vmCheckOut; }
        }

        public vmTransOperatorio vmTransOperatorio
        {
            get { return this._vmTransOperatorio; }
        }

        public vmSondagem vmSondagem
        {
            get { return this._vmSondagem; }
            set
            {
                this._vmSondagem = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckList>(x => x.vmSondagem));
            }
        }

        public wrpUsuarios Usuario
        {
            get { return this._usuarios; }
        }

        public IList<CheckListDTO> CheckListCollection
        {
            get
            {
                return
                    (from T in this._atendimento.DomainObject.DescricaoCirurgica
                     select new CheckListDTO
                     {
                         CheckList = this.getCheckList(T),
                         Cirurgia = this.getCirurgia(T),
                         Prestador = this.getPrestador(T),
                         DataAviso = T.DataAviso,
                         AvisoCirurgia = new wrpAvisoCirurgia(T),
                         Img = this.getImagemCheckList(T),
                         UsuarioEncerramento = this.getUsuarioEncerramento(T)
                     }).ToList();
            }
        }

        public CheckListDTO CheckListdto
        {
            get { return this._checklistdto; }
            set
            {
                this._checklistdto = value;
                this.IsNovoChecklist = false;
                this.getBotoes();
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckList>(x => x.CheckListdto));
            }
        }

        public wrpPaciente Paciente
        {
            get { return this._atendimento.Paciente; }
        }

        public bool boolExcluir
        {
            get { return !this._checklistdto.IsNull() && !this._checklistdto.CheckList.IsNull(); }
        }

        public bool boolEntrada
        {
            get
            {
                if (this._atendimento.DomainObject.DescricaoCirurgica.Count.Equals(0))
                    return false;
                return !TemCheckList || (this._checklistdto.CheckList.AntesEntradaPaciente.IsNull() || this._checklistdto.CheckList.AntesEntradaPaciente.DataEncerramento.IsNull());
            }
        }
        public bool boolInducao
        {
            get
            {
                if (TemCheckList)
                    if (this._checklistdto.CheckList.AntesEntradaPaciente.IsNotNull() && this._checklistdto.CheckList.AntesEntradaPaciente.DataEncerramento.IsNotNull())
                        if (this._checklistdto.CheckList.AntesInducaoAnestesica.IsNull() || (this._checklistdto.CheckList.AntesInducaoAnestesica.IsNotNull() && this._checklistdto.CheckList.AntesInducaoAnestesica.DataEncerramento.IsNull()))
                            return true;
                return false;
            }
        }
        public bool boolIncisao //TimeOut
        {
            get
            {
                if (TemCheckList)
                    if (this._checklistdto.CheckList.AntesInducaoAnestesica.IsNotNull() && this._checklistdto.CheckList.AntesInducaoAnestesica.DataEncerramento.IsNotNull())
                        //if (this._checklistdto.CheckList.TimeOut.IsNull() || (this._checklistdto.CheckList.TimeOut.IsNotNull() && this._checklistdto.CheckList.TimeOut.DataEncerramento.IsNull()))
                        return true;
                return false;
            }
        }
        public bool boolTransOperatorio
        {
            get
            {
                if (TemCheckList)
                    if (this._checklistdto.CheckList.TimeOut.IsNotNull() && this._checklistdto.CheckList.TimeOut.DataEncerramento.IsNotNull())
                        if (this._checklistdto.CheckList.TransOperatorio.IsNull() || (this._checklistdto.CheckList.TransOperatorio.IsNotNull() && this._checklistdto.CheckList.TransOperatorio.DataEncerramento.IsNull()))
                            return true;
                return false;
            }
        }
        public bool boolCheckout
        {
            get
            {
                if (TemCheckList)
                    if (this._checklistdto.CheckList.TransOperatorio.IsNotNull() && this._checklistdto.CheckList.TransOperatorio.DataEncerramento.IsNotNull())
                        return true;
                return false;
            }
        }
        public bool boolSondagem
        {
            get
            {
                return _sondagem;
            }
        }

        public bool BotoesVisiveis
        {
            get
            {
                return this._checklistdto.IsNotNull()
                    && this._checklistdto.CheckList.IsNotNull()
                    && this._checklistdto.CheckList.Sondagem.IsNotNull()

                    && (this._checklistdto.CheckList.Sondagem.DataEncerramento.IsNotNull()
                    || (this._checklistdto.CheckList.Sondagem.VesicalDemora == SimNao.Nao && this._checklistdto.CheckList.Sondagem.VesicalAlivio == SimNao.Nao)
                    || this._checklistdto.CheckList.Sondagem.VesicalDemoraCirurgiao == SimNao.Sim)

                    && this._checklistdto.CheckList.DataEncerramento.IsNotNull();
            }
        }

        public bool IsNovoChecklist { get; set; }
        #endregion

        #region ----- Métodos Privados -----
        private wrpUsuarios getUsuarioEncerramento(AvisoCirurgia T)
        {
            if (T.CheckList.IsNull())
                return null;
            return new wrpUsuarios(T.CheckList.UsuarioEncerramento);
        }

        private wrpCheckListCirurgia getCheckList(AvisoCirurgia T)
        {
            if (T.CheckList.IsNull())
                return null;
            return new wrpCheckListCirurgia(T.CheckList);
        }

        private wrpPrestador getPrestador(AvisoCirurgia T)
        {
            if (T.CheckList != null)
                return new wrpPrestador(T.CheckList.Prestador);

            if (T.EquipesMedicas.Count(xx => xx.Principal.Equals(SimNao.Sim)) > 0)
                return new wrpPrestador(T.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador);
            return new wrpPrestador(T.EquipesMedicas.Where(x => x.AtividadeMedica.OrdemApresentacao.Equals(1)).FirstOrDefault().Prestador);
        }

        private wrpCirurgia getCirurgia(AvisoCirurgia T)
        {
            if (T.CheckList != null)
                return new wrpCirurgia(T.CheckList.Cirurgia);

            return new wrpCirurgia(T.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia);
        }

        private BitmapImage getImagemCheckList(AvisoCirurgia T)
        {
            if (T.CheckList == null)
                return null;

            return new BitmapImage(new Uri(@"/HMV.Core.Framework.WPF;component/Images/CheckList.png", UriKind.Relative));
        }

        private void getBotoes()
        {
            this.OnPropertyChanged<vmCheckList>(x => x.boolExcluir);
            this.OnPropertyChanged<vmCheckList>(x => x.boolEntrada);
            this.OnPropertyChanged<vmCheckList>(x => x.boolInducao);
            this.OnPropertyChanged<vmCheckList>(x => x.boolIncisao);
            this.OnPropertyChanged<vmCheckList>(x => x.boolCheckout);
            this.OnPropertyChanged<vmCheckList>(x => x.boolTransOperatorio);
            this.OnPropertyChanged<vmCheckList>(x => x.boolSondagem);
            this.OnPropertyChanged<vmCheckList>(x => x.BotoesVisiveis);
        }
        #endregion

        #region ----- Métodos Públicos -----
        public void Novo()
        {
            this._checklistdto.CheckList = new wrpCheckListCirurgia(this._usuarios)
            {
                Atendimento = this._atendimento,
                AvisoCirurgia = this._checklistdto.AvisoCirurgia,
                Cirurgia = this._checklistdto.Cirurgia,
                Paciente = this._atendimento.Paciente,
                Prestador = this._checklistdto.Prestador,
                Usuario = this._usuarios
            };

            this.IsNovoChecklist = true;
        }

        public void IniciaVMS()
        {
            this._vmAntesDaEntrada = new vmAntesDaEntrada(this);
            this._vmAntesDaInducao = new vmAntesDaInducao(this);
            this._vmTimeOut = new vmTimeOut(this);
            this._vmTransOperatorio = new vmTransOperatorio(this);
            this._vmCheckOut = new vmCheckOut(this);
            this._vmSondagem = new vmSondagem(this);
        }

        public void Salvar()
        {
            this._checklistdto.CheckList.Paciente.Save();
            IRepositorioDeCheckList rep = ObjectFactory.GetInstance<IRepositorioDeCheckList>();
            rep.Save(this._checklistdto.CheckList.DomainObject);
            if (this._vmAntesDaEntrada.IsNotNull())
                this._vmAntesDaEntrada.SalvaAlergias();
            this._checklistdto.CheckList.AvisoCirurgia.DomainObject.Refresh();
        }

        public void SalvarFechar(ViewModelBase pmv)
        {
            if (pmv.GetType().Equals(typeof(vmAntesDaEntrada)))
                this._checklistdto.CheckList.AntesEntradaPaciente = this._vmAntesDaEntrada.AntesEntradaPaciente;
            else if (pmv.GetType().Equals(typeof(vmAntesDaInducao)))
                this._checklistdto.CheckList.AntesInducaoAnestesica = this._vmAntesDaInducao.AntesInducaoAnestesica;
            else if (pmv.GetType().Equals(typeof(vmTimeOut)))
                this._checklistdto.CheckList.TimeOut = this._vmTimeOut.TimeOut;
            else if (pmv.GetType().Equals(typeof(vmCheckOut)))
                this._checklistdto.CheckList.CheckOut = this._vmCheckOut.CheckOut;
            else if (pmv.GetType().Equals(typeof(vmTransOperatorio)))
                this._checklistdto.CheckList.TransOperatorio = this._vmTransOperatorio.TransOperatorio;
            Salvar();
        }

        public void Remover()
        {
            this._checklistdto.CheckList.DomainObject.DataCancelamento = DateTime.Now;
            Salvar();
            this._atendimento.DomainObject.DescricaoCirurgica.FirstOrDefault(x => x.Id.Equals(this._checklistdto.AvisoCirurgia.cd_aviso_cirurgia)).Refresh();
            Refresh();
        }

        public void Refresh()
        {
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckList>(x => x.CheckListCollection));
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }

    public class CheckListDTO
    {
        public DateTime DataAviso { get; set; }
        public wrpCirurgia Cirurgia { get; set; }
        public wrpAvisoCirurgia AvisoCirurgia { get; set; }
        public wrpPrestador Prestador { get; set; }
        public BitmapImage Img { get; set; }
        public wrpCheckListCirurgia CheckList { get; set; }
        public wrpUsuarios UsuarioEncerramento { get; set; }
    }
}
