using System;
using System.Configuration;
using System.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Constant;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.Commands;
using StructureMap;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmAlergias : ViewModelBase
    {
        #region Construtor
        public vmAlergias(Paciente pPaciente, Usuarios pUsuario, bool pIsCorpoClinico, Atendimento pAtendimento = null)
        {
            this._atendimento = pAtendimento;
            this._paciente = new wrpPaciente(pPaciente);
            this._listaAlergias = this._paciente.Alergias;
            this._profissional = new wrpSiga_Profissional(pUsuario.Profissional);
            this._usuario = new wrpUsuarios(pUsuario);
            this._isCorpoClinico = pIsCorpoClinico;

            this.AlergiaCommand = new SaveAlergiaCommand(this);
            this.AddComentarioCommand = new SaveComentarioCommand(this);
            this.RemoveAlergiaCommand = new RemoveAlergiaSaveCommand(this);

            if (this._listaAlergias.Count(x => x.Agente == Constantes.coSemAlergiasConhecidas && x.Status == StatusAlergiaProblema.Ativo) > 0 
                && this._listaAlergias.Count(x => x.Status == StatusAlergiaProblema.Ativo) == 1)
                this._semAlergiasConhecidas = true;
            MostraAtivos = true;

            //TEM QUE TIRAR O 2 PQ O SISTEMA ANTIGO AINDA O UTILIZA!
            IRepositorioAlergiaTipo rep = ObjectFactory.GetInstance<IRepositorioAlergiaTipo>();            
            this._alergiaTipo = new wrpAlergiaTipoCollection(rep.FiltraAtivos().OrdenaPorDescricao().List().Where(x => x.ID != 2).ToList());
        }
     

        #endregion

        #region Propriedades Publicas

        public Atendimento Atendimento
        {
            get
            {
                return this._atendimento;
            }
        }

        public wrpAlergiaCollection ListaAlergias
        {
            get
            {
                if (this._mostraexcluidos)
                    return new wrpAlergiaCollection(this._listaAlergias.Select(x => x.DomainObject).Where(x => x.Status == StatusAlergiaProblema.Excluído).ToList());
                else if (this._mostraativos)
                    return new wrpAlergiaCollection(this._listaAlergias.Select(x => x.DomainObject).Where(x => x.Status == StatusAlergiaProblema.Ativo).ToList());
                else if (this._mostrainativos)
                    return new wrpAlergiaCollection(this._listaAlergias.Select(x => x.DomainObject).Where(x => x.Status == StatusAlergiaProblema.Inativo).ToList());
                else
                    return new wrpAlergiaCollection(this._listaAlergias.Select(x => x.DomainObject).Where(x => x.Status != StatusAlergiaProblema.Temporario).ToList());
            }
        }

        ///// <summary>
        ///// não pode ter filtro pq cria uma nova coleção
        ///// </summary>
        //public wrpAlergiaCollection ListaAlergiasAdd
        //{
        //    get
        //    {
        //        return this._listaAlergias;
        //    }
        //}

        public DateTime? NascPaciente
        {
            get
            {
                return this._paciente.DataNascimento;
            }
        }

        public bool MostraExcluidos
        {
            get
            {
                return _mostraexcluidos;
            }
            set
            {
                this._mostraexcluidos = value;
                this.OnPropertyChanged("MostraExcluidos");
                this.OnPropertyChanged("ListaAlergias");
            }
        }

        public bool MostraAtivos
        {
            get
            {
                return _mostraativos;
            }
            set
            {
                this._mostraativos = value;
                this.OnPropertyChanged("MostraAtivos");
                this.OnPropertyChanged("ListaAlergias");
            }
        }

        public bool MostraInativos
        {
            get
            {
                return _mostrainativos;
            }
            set
            {
                this._mostrainativos = value;
                this.OnPropertyChanged("MostraInativos");
                this.OnPropertyChanged("ListaAlergias");
            }
        }

        public wrpAlergia AlergiaSelecionada
        {
            get
            {
                return this._alergiaselecionada;
            }
            set
            {
                this._alergiaselecionada = value;
                if (value.IsNotNull())
                this._alergiatiposelecionada = _alergiaTipo.Where(x=> x.ID == value.AlergiaTipo.ID).SingleOrDefault();


                this.OnPropertyChanged("HabilitaBotoes");
                this.OnPropertyChanged("BotaoImprime");
                this.OnPropertyChanged("AlergiaSelecionada");
            }
        }

        public string ComentarioSelecionado
        {
            get
            {
                return this._comentarioselecionado;
            }
            set
            {
                this._comentarioselecionado = value;
                this.OnPropertyChanged("ComentarioSelecionado");
            }
        }

        public wrpAlergiaTipoCollection ListaAlergiasTipo
        {
            get
            {                
                return _alergiaTipo;
            }
        }

        public wrpAlergiaTipo AlergiaTipo
        {
            get 
            { 
                return _alergiatiposelecionada; 
            }
            set
            {
                this._alergiatiposelecionada = value;
                this._alergiaselecionada.AlergiaTipo = value;
                base.OnPropertyChanged("AlergiaTipo");
            }
        }

        public string NomeTela
        {
            get
            {
                return "Comentários da Alergia";
            }
        }

        public wrpPaciente Paciente
        {
            get
            {
                return this._paciente;
            }
        }

        public bool SemAlergiasConhecidas
        {
            get
            {
                return this._semAlergiasConhecidas;
            }
            set
            {
                if (this._listaAlergias != null && this._listaAlergias.Count(x => x.Agente != Constantes.coSemAlergiasConhecidas && x.Status == StatusAlergiaProblema.Ativo) > 0)
                {
                    DXMessageBox.Show("Não é possivel marcar 'Sem Alergias Conhecidas' pois o paciente tem alergia cadastrada.", "Alergias", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                    this._semAlergiasConhecidas = false;
                }
                else
                {
                    this._semAlergiasConhecidas = value;
                    SaveSemAlergiasConhecidas();
                }
                this.OnPropertyChanged("SemAlergiasConhecidas");
            }
        }

        public bool NovaAlergia { get; set; }

        public bool HabilitaBotoes
        {
            get
            {
                return (_isCorpoClinico && (this._alergiaselecionada != null && this._alergiaselecionada.Status != StatusAlergiaProblema.Excluído));
            }
        }

        public bool Inativo
        {
            get
            {
                if (this._alergiaselecionada.Status == StatusAlergiaProblema.Inativo)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    this._alergiaselecionada.Status = StatusAlergiaProblema.Inativo;
                else
                    this._alergiaselecionada.Status = StatusAlergiaProblema.Ativo;
            }
        }

        public bool BotaoImprime
        {
            get { this.OnPropertyChanged("BotaoComentario"); return (ListaAlergias.Count > 0 ? true : false); }
        }

        public bool BotaoComentario
        {
            get { return (AlergiaSelecionada != null ? true : false); }
        }
        #endregion

        #region Metodos Publicos
        public void NovoRegistro()
        {
            this._alergiaselecionada = new wrpAlergia(new Alergia(this._usuario.DomainObject, this._paciente.DomainObject, this._profissional.DomainObject));
            this._alergiaselecionada.DataInicio = DateTime.Now;
            this.NovaAlergia = true;
            this._alergiatiposelecionada = null;
            this.OnPropertyChanged("AlergiaSelecionada");
        }

        public void SaveComentario()
        {
            this._alergiaselecionada.DomainObject.addComentario(new Comentario(this._usuario.DomainObject, this.ComentarioSelecionado));
            this.EndEdit();

            this.OnPropertyChanged("AlergiaSelecionada");
            AtualizaListaAlergias();
        }

        public void ExcluiAlergia()
        {
            this._alergiaselecionada.Status = StatusAlergiaProblema.Excluído;
            this._alergiaselecionada.DomainObject.addComentario(new Comentario(this._usuario.DomainObject, "EXCLUSÃO: " + this.ComentarioSelecionado));
            AtualizaListaAlergias();
        }

        public void SaveSemAlergiasConhecidas()
        {
            if (this._semAlergiasConhecidas)
                this._paciente.DomainObject.AddSemAlergiasConhecidas(this._usuario.DomainObject, this._profissional.DomainObject);
            else
                this._paciente.DomainObject.RemoveSemAlergiasConhecidas();

            this._paciente = new wrpPaciente(this._paciente.DomainObject);
            this._listaAlergias = this._paciente.Alergias;

            AtualizaListaAlergias();
        }

        internal void Save()
        {
            this._paciente.Save();
            AtualizaListaAlergias();
        }

        public void AtualizaListaAlergias(Paciente pPaciente = null)
        {
            if (pPaciente.IsNotNull())
                this._paciente = new wrpPaciente(pPaciente);
            this._listaAlergias = this._paciente.Alergias;
            this.OnPropertyChanged("ListaAlergias");
            this.OnPropertyChanged("BotaoImprime");
        }

        public void adicionaAlergia()
        {
            if (this._semAlergiasConhecidas)
            {
                this._semAlergiasConhecidas = false;
                SaveSemAlergiasConhecidas();
            }
            this._listaAlergias.Add(this.AlergiaSelecionada);
        }

        #endregion

        #region Commands
        public ICommand AlergiaCommand { get; set; }
        public ICommand AddComentarioCommand { get; set; }
        public ICommand RemoveAlergiaCommand { get; set; }
        #endregion

        #region Propriedades Privadas
        private Atendimento _atendimento { get; set; }
        private wrpAlergia _alergiaselecionada { get; set; }
        private wrpAlergiaCollection _listaAlergias { get; set; }
        private wrpAlergiaTipoCollection _alergiaTipo { get; set; }
        private wrpAlergiaTipo _alergiatiposelecionada;
        private wrpPaciente _paciente { get; set; }
        private wrpSiga_Profissional _profissional { get; set; }
        private wrpUsuarios _usuario { get; set; }
        private String _comentarioselecionado { get; set; }
        private bool _mostraexcluidos { get; set; }
        private bool _mostraativos { get; set; }
        private bool _mostrainativos { get; set; }
        private bool _semAlergiasConhecidas { get; set; }
        private bool _botaoExcluir { get; set; }
        private bool _isCorpoClinico { get; set; }
        #endregion
    }
}
