using System;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmJustificativaExclusao : ViewModelBase
    {
        #region Construtor
        public vmJustificativaExclusao(string pJustificativas, wrpUsuarios pUsuario)
        {
            this._justificativas = pJustificativas;
            this._usuarios = pUsuario;
            this.BeginEdit();
        }
        #endregion

        #region Propriedades Privadas
        private string _justificativas;
        private string _justificativa;
        private wrpUsuarios _usuarios;
        private bool _cancelou;
        #endregion

        #region Propriedades Publicas 
        public string Justificativa
        {
            get
            {
                return this._justificativa;
            }
            set
            {
                this._justificativa = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmJustificativaExclusao>(x => x.Justificativa));
            }
        }

        public string Justificativas
        {
            get
            {
                return this._justificativas;
            }
            set
            {
                this._justificativas = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmJustificativaExclusao>(x => x.Justificativas));
            }
        }

        public bool Cancelou
        {
            get
            {
                return this._cancelou;
            }
        }
        #endregion

        #region Metodos Publicos
  
        #endregion

        #region Metodos Privados
        
        #endregion

        #region Commands
 
        protected override bool  CommandCanExecuteSalvar(object param)
        {
            return !this._justificativa.IsEmptyOrWhiteSpace();
        }

        protected override void CommandSalvar(object param)
        {
            this._justificativas += (String.IsNullOrEmpty(this._justificativas) ? String.Empty : Environment.NewLine) + new Comentario(_usuarios.DomainObject, "EXCLUSÃO: " + this._justificativa).ToString();
            this._justificativa = string.Empty;
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmJustificativaExclusao>(x => x.Justificativa));    
            this.EndEdit();
            base.CommandSalvar(param);
        }

        protected override void CommandFechar(object param)
        {
            this._cancelou = true;
            this.CancelEdit();
            base.CommandFechar(param);
        }
        #endregion
    }
}
