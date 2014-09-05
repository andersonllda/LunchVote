using System;
using System.Linq;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddExameCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddExameCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmExames))
            {
                if ((ViewModel as vmExames).ExameSelecionado != null)
                    if ((ViewModel as vmExames).Exames.Where(x => x.Procedimento.ID == (ViewModel as vmExames).ExameSelecionado.Procedimento.ID).Count() > 0)
                    {
                        return false;
                    }
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
                (ViewModel as vmExames).Exames.Add((ViewModel as vmExames).ExameSelecionado);
                //(ViewModel as vmExames).ExameSelecionado = null;
            }
        }

        #endregion
    }
}

