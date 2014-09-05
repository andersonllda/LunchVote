using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Wrappers;
using HMV.Core.Framework.Exception;
using NHibernate.Validator.Engine;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.PEP;

namespace HMV.PEP.ViewModel.Commands
{
    public class RemoveAlergiaCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor

        public RemoveAlergiaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmAlergias))
            {
                if ((ViewModel as vmAlergias).AlergiaSelecionada == null)
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
                //if (DXMessageBox.Show("Deseja realmente Excluir a Alergia selecionado?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                //{
                    //(ViewModel as vmAlergias).Paciente.Alergias.Remove((ViewModel as vmAlergias).AlergiaSelecionada);
                    (ViewModel as vmAlergias).ExcluiAlergia();
                    (ViewModel as vmAlergias).AlergiaSelecionada = null;
                    (ViewModel as vmAlergias).ComentarioSelecionado = null;
                //}

            }
        }
        #endregion
    }
}

