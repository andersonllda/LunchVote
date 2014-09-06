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
using HMV.Core.Framework.Extensions;
using System;
using System.Reflection;
using System.Configuration;
using HMV.Core.Framework.WPF;
using HMV.Core.WCF.Interfaces.Acesso;

namespace HMV.PEP.ViewModel.BoletimEmergencia
{
    public class vmItensRegistro : ViewModelBase
    {
        #region Contrutor
        public vmItensRegistro(vmBoletimEmergencia pvmBoletim)
        {
            this._vmboletimdeemergencia = pvmBoletim;
            if (pvmBoletim.BoletimEmergencia != null)
            {
                this._boletimdeemergencia = pvmBoletim.BoletimEmergencia;
                this.Usuarios = pvmBoletim.Usuarios;

                this.AddBoletimAvaliacaoCommand = new AddBoletimAvaliacaoCommand(this);
                
                IRepositorioDeTipoAvaliacao rep = ObjectFactory.GetInstance<IRepositorioDeTipoAvaliacao>();
                var lst = rep.FiltraAtivos().List();
                if (lst != null)
                    this._tiposavaliacao = new wrpTipoAvaliacaoCollection(lst);
                else
                    this._tiposavaliacao = new wrpTipoAvaliacaoCollection(new List<TipoAvaliacao>());

                if (this._boletimdeemergencia.DomainObject.BoletimAvaliacao == null)
                    this._boletimdeemergencia.BoletimAvaliacao = new wrpBoletimAvaliacaoCollection(new List<BoletimAvaliacao>());

                this._tipoavaliacaoselecionado = this._tiposavaliacao.First();

                this.Habilita = true;
            }
            else
                this.Habilita = false;
         

            this.OnPropertyChanged("Habilita");
        }
        #endregion

        #region Propriedades Publicas
        public string TextoSelecionado
        {
            get
            {
                return this._textoselecionado;
            }
            set
            {
                this._textoselecionado = value;
                Editou();
                this.OnPropertyChanged("TextoSelecionado");
            }
        }

        public wrpTipoAvaliacaoCollection TiposAvaliacao
        {
            get
            {
                this._tiposavaliacao.Sort(x => x.OrdemTela);
                return this._tiposavaliacao;
            }
        }

        public wrpTipoAvaliacao TipoAvaliacaoSelecionado
        {
            get
            {
                return this._tipoavaliacaoselecionado;
            }
            set
            {
                if (this._textoselecionado.IsNotEmptyOrWhiteSpace())
                    AdicionaItem();

                if (value != null)
                    this._tipoavaliacaoselecionado = value;

                if (this._tipoavaliacaoselecionado != null)
                {
                    if (this._tipoavaliacaoselecionado.Exame == SimNao.Sim)
                        AbreExamesSolicitados = true;
                    else
                        AbreExamesSolicitados = false;

                    if (this._tipoavaliacaoselecionado.Alta == SimNao.Sim)
                        AbreCadastroAlta = true;
                    else
                        AbreCadastroAlta = false;
                }              

                this._textoselecionado = string.Empty;
                this.OnPropertyChanged("TextoSelecionado");
                this.OnPropertyChanged("BoletimAvaliacoes");
                this.OnPropertyChanged("AbreExamesSolicitados");
                this.OnPropertyChanged("AbreCadastroAlta");
                this.OnPropertyChanged("TipoAvaliacaoSelecionado");
            }
        }

        public IList<wrpBoletimAvaliacao> BoletimAvaliacoes
        {
            get
            {
                //if (this._tipoavaliacaoselecionado != null)
                //{
                //    var ret = this._boletimdeemergencia.BoletimAvaliacao.Where(x => x.TipoAvaliacao.ID == this._tipoavaliacaoselecionado.ID).ToList();
                //    _boletimavaliacoestodas = new wrpBoletimAvaliacaoCollection(ret.Select(x => x.DomainObject).ToList());
                //}
                if (this._tipoavaliacaoselecionado != null)
                {
                    return this._boletimdeemergencia.BoletimAvaliacao.Where(x => x.TipoAvaliacao.ID == this._tipoavaliacaoselecionado.ID).ToList();
                }
                return null;
            }
        }

        public bool Habilita { get; set; }
        public Usuarios Usuarios { get; set; }
        public wrpBoletimDeEmergencia BoletimEmergencia
        {
            get
            {
                return this._boletimdeemergencia;
            }
        }
        public bool AbreExamesSolicitados { get; set; }
        public bool AbreCadastroAlta { get; set; }
        public bool BoletimFechado
        {
            get
            {
                if (this._boletimdeemergencia.DataAlta != null)
                    return true;
                return false;
            }
        }

        public vmBoletimEmergencia vmBoletimEmergencia
        {
            get
            {
                return this._vmboletimdeemergencia;
            }
        }
        #endregion

        #region Metodos Privados

        #endregion

        #region Propriedades Privadas
        private wrpBoletimDeEmergencia _boletimdeemergencia;
        //private wrpBoletimAvaliacao _boletimavaliacaoselecionado;
        private wrpTipoAvaliacaoCollection _tiposavaliacao;
        private wrpTipoAvaliacao _tipoavaliacaoselecionado;
        private string _textoselecionado;
        private vmBoletimEmergencia _vmboletimdeemergencia;
        //private wrpBoletimAvaliacaoCollection _boletimavaliacoes;
        //private wrpBoletimAvaliacaoCollection _boletimavaliacoestodas;
        #endregion

        #region Metodos Publicos
        public void RefreshBoletim()
        {
            this.OnPropertyChanged("BoletimAvaliacoes");
            this.OnPropertyChanged("TextoSelecionado");
            this.OnPropertyChanged("TipoAvaliacaoSelecionado");
        }

        public void RegistraExamesSolicitados(IList<Procedimento> pProcedimentos)
        {
            foreach (var item in pProcedimentos)
            {
                this._textoselecionado = item.Descricao;
                this.AddBoletimAvaliacaoCommand.Execute(null);
            }
        }
        public void Editou()
        {
            this._vmboletimdeemergencia.Editou = true;
        }
        public bool Valida()
        {
            foreach (var item in this._vmboletimdeemergencia.vmItensDeRegistro.TiposAvaliacao)
            {
                if (item.ObrigatorioBoletim == SimNao.Sim)
                {
                    if (this.BoletimEmergencia.BoletimAvaliacao.Where(x => x.TipoAvaliacao.ID == item.ID).Count() == 0)
                    {
                        DXMessageBox.Show("Informe o campo " + item.Descricao + " para finalizar o boletim.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                }
            }

            if (this.BoletimEmergencia.Classificacoes.Count == 0)
            {
                DXMessageBox.Show("Informe uma classificação para finalizar o boletim.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (((this._vmboletimdeemergencia.Usuarios.Prestador.IsNotNurse) && (this._vmboletimdeemergencia.IsPame))||(!this._vmboletimdeemergencia.IsPame))
                if (this._vmboletimdeemergencia.BoletimEmergencia.Atendimento.CIDs.Count == 0)
                {
                    DXMessageBox.Show("Informe um CID do atendimento para finalizar o boletim.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                    this._vmboletimdeemergencia.TabSelecionada = "CID";
                    return false;
                }
            return true;
        }

        public void AdicionaItem()
        {
            if (string.IsNullOrEmpty(this.TextoSelecionado) || this.TipoAvaliacaoSelecionado == null)
                return;

            wrpBoletimAvaliacao novoboletim = new wrpBoletimAvaliacao(this.BoletimEmergencia.DomainObject, this.Usuarios);
            novoboletim.DataInclusao = DateTime.Today;
            novoboletim.HoraInclusao = DateTime.Now.ToString("HH:mm");
            novoboletim.Texto = this.TextoSelecionado;
            novoboletim.TipoAvaliacao = this.TipoAvaliacaoSelecionado;

            this._boletimdeemergencia.BoletimAvaliacao.Add(novoboletim);
            this._boletimdeemergencia.DomainObject.BoletimAvaliacao.Add(novoboletim.DomainObject);
            this._boletimdeemergencia.Save();

            Log(this.GetType().Assembly, "LOG_ITENS_BOLETIM", this._boletimdeemergencia.Id, this.TextoSelecionado, this.Usuarios.ID);

            this.TextoSelecionado = string.Empty;
            this.RefreshBoletim();
        }
        #endregion

        #region Commands
        public ICommand AddBoletimAvaliacaoCommand { get; set; }
        #endregion

        public static void Log(Assembly pAssembly, string texto, int? chave, string observacao, string user)
        {
            try
            {
                //HMV.Core.Framework.Web.BaseServiceParametro.Banco = "";
                int idSistema = int.Parse(ConfigurationManager.AppSettings["Sistema"].ToString());
                ILogWCF servLog = ObjectFactory.GetInstance<ILogWCF>();
                HMV.Core.DTO.LogDTO log = new HMV.Core.DTO.LogDTO
                {
                    IDSistema = idSistema,
                    Acao = Acao.Inserir.ToString(),
                    IDUsuario = user,
                    Chave = chave,
                    Data = DateTime.Now,
                    Tabela = texto,
                    Observacao =  observacao,
                    Dispositivo = Environment.MachineName
                };

                servLog.SalvarLog(log);
            }
            catch (Exception err)
            {
                HMVEmail.SendErro(err, pAssembly);
            }
        }       
    }
}
