using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.Exception;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.DataAccess;
using HMV.Core.Interfaces;
using StructureMap;
using System.Configuration;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using System.Windows.Media;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;

namespace HMV.PEP.ViewModel
{
    public class vmMenu : ViewModelBase
    {
        #region Contrutor
        public vmMenu(Usuarios pUsuario)
        {
            this._Usuarios = new wrpUsuarios(pUsuario);

            // busca os processo que o usuairo tem acesso 
            int cSistema;
            int.TryParse(ConfigurationManager.AppSettings["Sistema"], out cSistema);
            StructureMap.Pipeline.ExplicitArguments argsMenu = new StructureMap.Pipeline.ExplicitArguments();
            argsMenu.SetArg("plogin_sigla", this._Usuarios.cd_usuario);
            argsMenu.SetArg("pseq_sistema", cSistema);

            IMenuService iMenu = StructureMap.ObjectFactory.GetInstance<IMenuService>(argsMenu);
            this._ListaDeProcessoMenu = iMenu.Carrega()
                                        .Where(x => x.ProcessoFilho.ProcessoObjeto
                                        .Where(y => y.Objeto.Tipo == TipoObjeto.MenuVertical).Count() == 0).ToList();

            this._ListaDeProcessoTreeView = iMenu.Carrega()
                                               .Where(x =>
                                               x.ProcessoFilho.ProcessoObjeto.Where(y => y.Objeto.Tipo == TipoObjeto.Menu).Count() == 0
                                               && x.ProcessoPai.Componente == "PRONTUARIO").ToList();

            this._ListaDeProcessoTreeViewProcessosEnfermagem = iMenu.Carrega()
                                               .Where(x =>
                                               x.ProcessoFilho.ProcessoObjeto.Where(y => y.Objeto.Tipo == TipoObjeto.Menu).Count() == 0
                                               && x.ProcessoPai.Componente == "PROCESSOS_DE_ENFERMAGEM").ToList();

            this._ListaDeProcessoTreeViewResto = iMenu.Carrega()
                                        .Where(x => x.ProcessoFilho.ProcessoObjeto
                                        .Where(y => y.Objeto.Tipo == TipoObjeto.Menu).Count() == 0
                                        && x.ProcessoPai.Componente != "PRONTUARIO"
                                        && x.ProcessoPai.Componente != "PROCESSOS_DE_ENFERMAGEM").ToList();

            if (this._ListaDeProcessoMenu.HasItems())
                this._Sistema = new wrpSistemas(this._ListaDeProcessoMenu.FirstOrDefault().Sistema);
            else
                DXMessageBox.Show("Usuário sem acesso ao menu do prontuário eletrônico.\r\nFavor entrar em contato com a informática para adequação de seu cadastro.\r\nObrigado.", "Atenção", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
        }
        #endregion

        #region Propriedades Publicas
        public wrpSistemas Sistema
        {
            get
            {
                return this._Sistema;
            }
        }

        public wrpUsuarios Usuarios
        {
            get
            {
                return this._Usuarios;
            }
        }

        public IList<ProcessoDTO> Processos
        {
            get
            {
                IList<ProcessoDTO> lista = new List<ProcessoDTO>();
                foreach (var item in this._ListaDeProcessoMenu.ToList())
                    lista.Add(new ProcessoDTO(item));
                return lista;
            }
        }
        public IList<ProcessoDTO> ProcessosProntuario
        {
            get
            {
                IList<ProcessoDTO> lista = new List<ProcessoDTO>();
                foreach (var item in this._ListaDeProcessoTreeView.ToList())
                    lista.Add(new ProcessoDTO(item, _ListaDeProcessoTreeViewResto));
                return lista;
            }
        }
        public IList<ProcessoDTO> ProcessosEnfermagem
        {
            get
            {
                IList<ProcessoDTO> lista = new List<ProcessoDTO>();
                foreach (var item in this._ListaDeProcessoTreeViewProcessosEnfermagem.ToList())
                    lista.Add(new ProcessoDTO(item, _ListaDeProcessoTreeViewResto));
                return lista;
            }
        }
        #endregion

        #region Propriedades Privadas
        private wrpUsuarios _Usuarios { get; set; }
        private wrpSistemas _Sistema { get; set; }
        private IList<ProcessoMenu> _ListaDeProcessoMenu { get; set; }
        private IList<ProcessoMenu> _ListaDeProcessoTreeView { get; set; }
        private IList<ProcessoMenu> _ListaDeProcessoTreeViewProcessosEnfermagem { get; set; }
        private IList<ProcessoMenu> _ListaDeProcessoTreeViewResto { get; set; }
        #endregion
    }

    public class ProcessoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string PathIco { get; set; }
        public ImageSource Imagem { get; set; }
        public string Componente { get; set; }
        public string ParametroCadastroBase { get; set; }
        public Processo Processo { get; set; }
        public Processo ProcessoPai { get; set; }
        public Processo ProcessoFilho { get; set; }
        public int Nivel { get; set; }
        public IList<ProcessoDTO> ProcessosFilhos { get; set; }
        public int Ordem { get; set; }

        public ProcessoDTO(ProcessoMenu pProcesso, IList<ProcessoMenu> pLista)
        {
            this.Id = pProcesso.ProcessoFilho.ID;
            this.Descricao = pProcesso.ProcessoFilho.Descricao;
            this.PathIco = pProcesso.ProcessoFilho.Icone;
            this.Componente = pProcesso.ProcessoFilho.Componente;
            this.Ordem = pProcesso.Ordem;
            if (pProcesso.ProcessoFilho.ProcessoParametro.Count > 0)
                if (pProcesso.ProcessoFilho.ProcessoParametro.Where(x => x.NomeProcesso == "NameSpaceWrapper").Count() > 0)
                    this.ParametroCadastroBase = pProcesso.ProcessoFilho.ProcessoParametro.SingleOrDefault(x => x.NomeProcesso == "NameSpaceWrapper").Parametro;

            this.Processo = pProcesso.ProcessoFilho;

            if (pLista != null)
            {
                this.ProcessosFilhos = new List<ProcessoDTO>();
                foreach (var item in pLista.Where(x => x.ProcessoPai.ID == Processo.ID))
                {
                    this.ProcessosFilhos.Add(new ProcessoDTO(item, pLista));
                }
            }
        }

        public ProcessoDTO(ProcessoMenu pProcesso)
        {
            this.Id = pProcesso.ProcessoFilho.ID;
            this.Descricao = pProcesso.ProcessoFilho.Descricao;
            this.PathIco = pProcesso.ProcessoFilho.Icone;
            this.Componente = pProcesso.ProcessoFilho.Componente;
            this.Ordem = pProcesso.Ordem;
            this.ProcessoFilho = pProcesso.ProcessoFilho;
            this.ProcessoPai = pProcesso.ProcessoPai;
            this.Nivel = pProcesso.Nivel;

            if (pProcesso.ProcessoFilho.ProcessoParametro.Count > 0)
                if (pProcesso.ProcessoFilho.ProcessoParametro.Where(x => x.NomeProcesso == "NameSpaceWrapper").Count() > 0)
                    this.ParametroCadastroBase = pProcesso.ProcessoFilho.ProcessoParametro.SingleOrDefault(x => x.NomeProcesso == "NameSpaceWrapper").Parametro;
            this.Processo = pProcesso.ProcessoFilho;
        }
    }
}
