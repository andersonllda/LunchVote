using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using System.Windows;
using System.Linq;
using DevExpress.Xpf.Core;

namespace HMV.PEP.ViewModel.Commands
{
    public class RemoveExameCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor

        public RemoveExameCommand(object pViewModel)
        {
            ViewModel = pViewModel;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Se o método pode ser executado.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (ViewModel.GetType() == typeof(vmExames))
            {
                if ((ViewModel as vmExames).ExameSelecionado == null)
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

            if (ViewModel.GetType() == typeof(vmExames))
            {
                if (DXMessageBox.Show("Deseja realmente Excluir o Exame selecionado?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    (ViewModel as vmExames).Exames.Remove((ViewModel as vmExames).ExameSelecionado);
                    (ViewModel as vmExames).SumarioAlta.Save();
                }
            }
        }
        #endregion
    }
}

