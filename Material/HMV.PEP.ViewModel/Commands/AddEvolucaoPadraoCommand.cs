using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddEvolucaoPadraoCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;

        #endregion

        #region Construtor

        public AddEvolucaoPadraoCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmEvolucaoPadrao))
                if ((ViewModel as vmEvolucaoPadrao).EvolucaoSelecionada != null)
                {
                    if (string.IsNullOrEmpty((ViewModel as vmEvolucaoPadrao).EvolucaoSelecionada.Titulo))
                        return false;
                    if (string.IsNullOrEmpty((ViewModel as vmEvolucaoPadrao).EvolucaoSelecionada.Descricao))
                        return false;
                }
                else
                {
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

            if (ViewModel.GetType() == typeof(vmEvolucaoPadrao))
            {
                (ViewModel as vmEvolucaoPadrao).EvolucoesPadrao.Add((ViewModel as vmEvolucaoPadrao).EvolucaoSelecionada);
                (ViewModel as vmEvolucaoPadrao).EvolucaoSelecionada = null;
                (ViewModel as vmEvolucaoPadrao).SavePrestadorCommand.Execute(null);
            }
        }

        #endregion
    }
}

