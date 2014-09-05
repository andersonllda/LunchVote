using System;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddBoletimAvaliacaoCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddBoletimAvaliacaoCommand(object pViewModel)
        {
            this.ViewModel = pViewModel;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Se o método pode ser executado.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (ViewModel.GetType() == typeof(vmItensRegistro))
            {
                if (string.IsNullOrEmpty((ViewModel as vmItensRegistro).TextoSelecionado) || (ViewModel as vmItensRegistro).TipoAvaliacaoSelecionado == null)
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

            if (ViewModel.GetType() == typeof(vmItensRegistro))
            {
                (ViewModel as vmItensRegistro).AdicionaItem();
                (ViewModel as vmItensRegistro).Editou();
            }
        }
        #endregion
    }
}

