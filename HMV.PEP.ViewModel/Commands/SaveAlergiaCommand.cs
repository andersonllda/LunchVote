using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.PEP;

namespace HMV.PEP.ViewModel.Commands
{
    public class SaveAlergiaCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor
        public SaveAlergiaCommand(object pViewModel)
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
                if ((ViewModel as vmAlergias).NovaAlergia)
                {
                    ICommand AddAlergiaCommand = new AddAlergiaCommand(ViewModel);
                    AddAlergiaCommand.Execute(null);                             
                }
                (ViewModel as vmAlergias).Paciente.Save();
            }
        }
        #endregion
    }
}
