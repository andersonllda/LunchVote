using System;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmComentario : ViewModelBase
    {
        #region Construtor
        public vmComentario(string pComentarios, string pNomeTela, wrpUsuarios pUsuario)
        {
            this._nometela = pNomeTela;
            this._comentarios = pComentarios;
            this._usuarios = pUsuario;
            this.BeginEdit();
        }
        #endregion

        #region Propriedades Privadas
        private string _comentarios;
        private string _comentario;
        private string _nometela;
        private wrpUsuarios _usuarios;
        #endregion

        #region Propriedades Publicas
        public string Comentarios
        {
            get
            {
                return this._comentarios;
            }
            set
            {
                this._comentarios = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmComentario>(x => x.Comentarios));
            }
        }

        public string Comentario
        {
            get
            {
                return this._comentario;
            }
            set
            {
                this._comentario = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmComentario>(x => x.Comentario));
            }
        }

        public string NomeTela
        {
            get
            {
                return this._nometela;
            }
        }
        #endregion

        #region Metodos Publicos
  
        #endregion

        #region Metodos Privados
        
        #endregion

        #region Commands
        protected override void CommandIncluir(object param)
        {
            this._comentarios += (String.IsNullOrEmpty(this._comentarios) ? String.Empty : Environment.NewLine) + new Comentario(_usuarios.DomainObject, this._comentario).ToString();
            this._comentario = string.Empty;
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmComentario>(x => x.Comentario));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmComentario>(x => x.Comentarios));
        }

        protected override bool CommandCanExecuteIncluir(object param)
        {
            return !this._comentario.IsEmptyOrWhiteSpace();
        }

        protected override void CommandSalvar(object param)
        {
            this.EndEdit();
            base.CommandSalvar(param);
        }

        protected override void CommandFechar(object param)
        {
            this.CancelEdit();
            base.CommandFechar(param);
        }
        #endregion
    }
}
