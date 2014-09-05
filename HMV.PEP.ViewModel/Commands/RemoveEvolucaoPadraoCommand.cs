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
    public class RemoveEvolucaoPadraoCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;       
        #endregion

        #region Construtor

        public RemoveEvolucaoPadraoCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmEvolucaoPadrao))
            {
                if ((ViewModel as vmEvolucaoPadrao).EvolucaoSelecionada == null)
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
                if (DXMessageBox.Show("Deseja realmente Excluir a Evolução selecionada?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    (ViewModel as vmEvolucaoPadrao).EvolucoesPadrao.Remove((ViewModel as vmEvolucaoPadrao).EvolucaoSelecionada);
                    (ViewModel as vmEvolucaoPadrao).SavePrestadorCommand.Execute(null);
                }
            }            
        }

        #endregion
    }
}

