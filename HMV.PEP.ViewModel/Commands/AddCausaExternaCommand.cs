using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddCausaExternaCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddCausaExternaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmCausaExterna))
            {
                if ((ViewModel as vmCausaExterna).CausaExternaSelecionada != null)
                {
                    if ((ViewModel as vmCausaExterna).CausaExternaSelecionada.Cid == null || (ViewModel as vmCausaExterna).CausasExternas.Where(x => x.Cid.Id == (ViewModel as vmCausaExterna).CausaExternaSelecionada.Cid.Id).Count() > 0)
                    {
                        return false;
                    }
                }
                else
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

            if (ViewModel.GetType() == typeof(vmCausaExterna))
                (ViewModel as vmCausaExterna).adicionaCausaExterna();
            (ViewModel as vmCausaExterna).SumarioAlta.Save();
        }

        #endregion
    }
}

