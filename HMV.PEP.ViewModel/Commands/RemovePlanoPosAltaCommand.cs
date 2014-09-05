using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Wrappers;
using HMV.Core.Framework.Exception;
using NHibernate.Validator.Engine;
using System.Windows;
using DevExpress.Xpf.Core;

namespace HMV.PEP.ViewModel.Commands
{
    public class RemovePlanoPosAltaCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor

        public RemovePlanoPosAltaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmPosAlta))
            {
                if ((ViewModel as vmPosAlta).PlanoPosAltaSelecionado == null)                    
                    return false;                
                if ((ViewModel as vmPosAlta).PlanosPosAlta.Count == 0)
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

            if (ViewModel.GetType() == typeof(vmPosAlta))
            {
                if (DXMessageBox.Show("Deseja realmente Excluir o Plano Pos Alta selecionado?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    (ViewModel as vmPosAlta).PlanosPosAlta.Remove((ViewModel as vmPosAlta).PlanoPosAltaSelecionado);
                    (ViewModel as vmPosAlta).PlanoPosAltaSelecionado = null;
                }
            }
        }

        #endregion
    }
}

