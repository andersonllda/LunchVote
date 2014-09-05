using System;
using System.Linq;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.ViewModel.PEP;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddAlergiaCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddAlergiaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmAlergias))
            {
                if ((ViewModel as vmAlergias).AlergiaSelecionada == null)
                    return false;
                if (!(ViewModel as vmAlergias).AlergiaSelecionada.IsValid)
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
                if ((ViewModel as vmAlergias).NovaAlergia)
                {
                    (ViewModel as vmAlergias).adicionaAlergia();
                    (ViewModel as vmAlergias).AlergiaSelecionada = null;
                    (ViewModel as vmAlergias).NovaAlergia = false;
                    (ViewModel as vmAlergias).AtualizaListaAlergias();
                }
            }
        }
        #endregion
    }
}

