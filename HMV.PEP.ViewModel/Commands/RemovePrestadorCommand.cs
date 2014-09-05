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
    public class RemovePrestadorCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;       
        #endregion

        #region Construtor

        public RemovePrestadorCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmMedicos))
            {
                if ((ViewModel as vmMedicos).Prestadores == null || (ViewModel as vmMedicos).Prestadores.Count == 1)
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

            if (ViewModel.GetType() == typeof(vmMedicos))
            {
                if ((ViewModel as vmMedicos).Prestadores.Count == 1)
                    DXMessageBox.Show("Não é possível excluir todos os Médicos do Sumário de Alta.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else
                {
                    if (DXMessageBox.Show("Deseja realmente Excluir o Médico selecionado?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        (ViewModel as vmMedicos).Prestadores.Remove((ViewModel as vmMedicos).PrestadorSelecionado);
                        (ViewModel as vmMedicos).PrestadorSelecionado = null;
                    }
                }
            }            
        }

        #endregion
    }
}

