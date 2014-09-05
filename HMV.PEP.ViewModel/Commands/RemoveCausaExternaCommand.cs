using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Wrappers;
using HMV.Core.Framework.Exception;
using NHibernate.Validator.Engine;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Linq;

namespace HMV.PEP.ViewModel.Commands
{
    public class RemoveCausaExternaCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor

        public RemoveCausaExternaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmCausaExterna))
            {
                if ((ViewModel as vmCausaExterna).CausaExternaSelecionada == null)
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
            {
                if (DXMessageBox.Show("Deseja realmente Excluir a Causa Externa selecionada?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    (ViewModel as vmCausaExterna).CausasExternas.Remove((ViewModel as vmCausaExterna).CausasExternas.FirstOrDefault(x=>x.Cid.Id == (ViewModel as vmCausaExterna).CausaExternaSelecionada.Cid.Id));
                    //(ViewModel as vmCausaExterna).CausaExternaSelecionada = null;
                    (ViewModel as vmCausaExterna).SumarioAlta.Save();
                }

            }
        }
        #endregion
    }
}

