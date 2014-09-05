﻿using System;
using System.Linq;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.ViewModel.PEP;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddComentarioCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor
        public AddComentarioCommand(object pViewModel)
        {
            this.ViewModel = pViewModel;
        }
        #endregion

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            if (ViewModel.GetType() == typeof(vmAlergias))
            {
                if (String.IsNullOrWhiteSpace((ViewModel as vmAlergias).ComentarioSelecionado))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Dispara quando o canexecute muda.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>        
        /// Executa o procedimento do comando referente a tarefa.
        /// </summary>
        public void Execute(object parameter)
        {
            if (!this.CanExecute(parameter)) return;

            if (ViewModel.GetType() == typeof(vmAlergias))
            {
                (ViewModel as vmAlergias).SaveComentario();
                (ViewModel as vmAlergias).ComentarioSelecionado = null;
            }
        }
        #endregion
    }
}

