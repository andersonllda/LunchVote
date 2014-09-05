using System;
using System.Windows.Input;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;

namespace HMV.PEP.ViewModel.Commands
{
    public class SaveSumarioAltaCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;

        #endregion

        #region Construtor

        public SaveSumarioAltaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmSumarioAlta))
            {
                if (!(ViewModel as vmSumarioAlta).MostraAbas)
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

            if (ViewModel.GetType() == typeof(vmSumarioAlta))
            {
                try
                {
                    //(ViewModel as vmSumarioAlta).SumarioAlta.Save();
                    (ViewModel as vmSumarioAlta).Save();
                }
                catch (Exception ex)
                {
                    DXMessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}

